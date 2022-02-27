using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// using Marten.Schema;
using Newtonsoft.Json;
// using ProtoBuf;
using Planner.CpsatCleaningCalculator;

namespace Planner.Tests
{
    // Based on the old version of Hotel object, pre-V2, to help migrating from
    // old file system based JSON.  This is just enough to get the tests going
    public class Level
    {
        public string LevelName { get; set; }
        public long Id { get; set; }

        public List<OldRoom> Rooms { get; set; } = new List<OldRoom>();

        public List<Level> SubLevels { get; set; } = new List<Level>();

        [JsonIgnore]
        public int RoomsCount { get; set; }
        [JsonIgnore]
        public int FloorIndex { get; set; }
    }

    public class OldRoom : Room
    {
        public List<Cleaning> Cleanings { get; set; } = new List<Cleaning>();
    }


    public class Hotel
    {
        public string Id { get; set; }

        public int Version { get; set; } = 1;
        public string Name { get; set; }
        public string TimeZoneId { get; set; }
        public Level Structure { get; set; }
        public List<Attendant> Attendants { get; set; } = new List<Attendant>();
        public HotelStrategy Strategy { get; set; }

        public Hotel()
        {
        }

        public Hotel(string name, string id)
        {
            Id = id;
            Name = name;
            Structure = new Level { LevelName = "Root" };
            Strategy = GenerateHotelStrategy();
        }

        public Hotel Initialize(string id)
        {
            Id = id;
            Strategy = Strategy ?? GenerateHotelStrategy();
            return this;
        }

        public static IEnumerable<Level> GetLevels(Level level)
        {
            return new[] { level }.Concat(level.SubLevels.SelectMany(l => GetLevels(l)));
        }

        public static IEnumerable<OldRoom> GetRooms(Level level)
        {
            return GetLevels(level).SelectMany(l => l.Rooms);
        }

        public static IEnumerable<Cleaning> GetCleanings(Level level)
        {
            return GetRooms(level).SelectMany(r => r.Cleanings);
        }
        public static Planner.CpsatCleaningCalculator.Level ConvertLevel(Level floor)
        {
            if (floor == null)
            {
                return new Planner.CpsatCleaningCalculator.Level();
            }
            return new Planner.CpsatCleaningCalculator.Level{
                LevelName = floor.LevelName,
                RoomsCount = floor.RoomsCount,
                FloorIndex = floor.FloorIndex
            };
        }

        public static void SetFloor(Room room, Level floor, Level section, Level subsection)
        {
            room.Floor = ConvertLevel(floor);
            room.Section = ConvertLevel(section);
            room.Subsection = ConvertLevel(subsection);
            room.IndexOnFloor = floor.RoomsCount++;
        }

        private HotelStrategy GenerateHotelStrategy()
        {

            var cpsat= new HotelStrategy
            {
                DefaultAttendantStartTime = TimeSpan.FromHours(8),
                DefaultAttendantEndTime = TimeSpan.FromHours(20),
                DefaultMaxCredits = 0,
                ReserveBetweenCleanings = 15,
                TravelReserve = 5,
                CPSat = new CPSat(),
            };



            // switch (config.PlanningStrategyTypeKey)
            // {
            //     case "BALANCE_BY_ROOMS":
            //         cpsat.CPSat.strategy = "balanceByRooms";
            //         cpsat.CPSat.epsilonCredits = 0;
            //         cpsat.CPSat.epsilonRooms = -1;
            //         cpsat.CPSat.minRooms = config.BalanceByRoomsMinRooms;
            //         cpsat.CPSat.maxRooms = config.BalanceByRoomsMaxRooms;

            //         cpsat.CPSat.useTargetMode = false;
            //         cpsat.CPSat.targetModeMinimizeAttendants = false;

            //         break;
            //     case "BALANCE_BY_CREDITS_STRICT":
            //         cpsat.CPSat.epsilonCredits = -1;
            //         cpsat.CPSat.epsilonRooms = 0;
            //         cpsat.CPSat.strategy = "balanceByCredits";

            //         cpsat.CPSat.minRooms = config.BalanceByCreditsStrictMinCredits;
            //         cpsat.CPSat.maxRooms = config.BalanceByCreditsStrictMaxCredits;

            //         cpsat.CPSat.useTargetMode = false;
            //         cpsat.CPSat.targetModeMinimizeAttendants = false;
            //         break;
            //     case "BALANCE_BY_CREDITS_WITH_AFFINITIES":
            //         cpsat.CPSat.epsilonCredits = -1;
            //         cpsat.CPSat.epsilonRooms = 0;
            //         cpsat.CPSat.strategy = "balanceByCredits";

            //         cpsat.CPSat.minCredits = config.BalanceByCreditsWithAffinitiesMinCredits;
            //         cpsat.CPSat.maxCredits = config.BalanceByCreditsWithAffinitiesMaxCredits;

            //         cpsat.CPSat.useTargetMode = false;
            //         cpsat.CPSat.targetModeMinimizeAttendants = false;

            //         break;
            //     case "TARGET_BY_ROOMS":
            //         cpsat.CPSat.epsilonCredits = 0;
            //         cpsat.CPSat.epsilonRooms = -1;
            //         cpsat.CPSat.strategy = "targetByRooms";

            //         cpsat.CPSat.maxRooms = int.Parse(config.TargetByRoomsValue);

            //         cpsat.CPSat.useTargetMode = true;
            //         cpsat.CPSat.targetModeMinimizeAttendants = false;


            //         break;
            //     case "TARGET_BY_CREDITS":
            //         cpsat.CPSat.strategy = "targetByCredits";
            //         cpsat.CPSat.epsilonCredits = -1;
            //         cpsat.CPSat.epsilonRooms = 0;

            //         cpsat.CPSat.maxCredits = int.Parse(config.TargetByCreditsValue);

            //         cpsat.CPSat.useTargetMode = true;
            //         cpsat.CPSat.targetModeMinimizeAttendants = true;

            //         break;
            // }
            return cpsat;

        }

    }
}
