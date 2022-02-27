using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Planner.CpsatCleaningCalculator;

using Entities = Planner.Domain.Entities;

using Xunit;
using Xunit.Abstractions;

namespace Planner.Tests
{
    public class PlanningCPSATTestsBase : IDisposable
    {
        protected ITestOutputHelper TestOutputHelper;

        protected Cleaning[] Cleanings { get; set; }
        protected Attendant[] Attendants { get; private set; }
        protected Distances Distances { get; set; }
        protected HotelStrategy Strategy { get; } = new HotelStrategy();

        private TimeSlot GetDefaultTimeSlot()
        {
            var timeSlot = new TimeSlot
            {
                Affinities = new List<Affinity>(),
                From = TimeSpan.FromHours(8),
                To = TimeSpan.FromHours(20),
                IsPreferred = false,
                Levels = new List<string>(),
                MaxCredits = 0,
            };
            return timeSlot;
        }
        public PlanningCPSATTestsBase(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
            Strategy.CPSat = new CPSat();
        }

        protected void InitializeAttendantTimeSlots(Attendant attendant)
        {
            attendant.CurrentTimeSlots = new TimeSlot[]{Strategy.GetDefaultTimeSlot()};
            attendant.CurrentTimeSlot.Affinities = new List<Affinity>();
        }

        // protected void AssignCleaningAsPrePlan(Attendant attendant, Cleaning cleaning, DateTime _start = default(DateTime))
        // {
        //     var start = cleaning.From;
        //     var earliest_date = start.Add(-start.TimeOfDay);
        //     if (_start != default(DateTime))
        //     {
        //         start = _start;
        //     }
        //     else
        //     {
        //         if (attendant.CurrentTimeSlot != null &&
        //             earliest_date + attendant.CurrentTimeSlot.From > start)
        //             start = earliest_date + attendant.CurrentTimeSlot.From;
        //         if (attendant.Cleanings.Count() > 0)
        //         {
        //             var clast = attendant.Cleanings.Max(a => a.To);
        //             if (clast > start)
        //                 start = clast;
        //         }
        //     }
        //     var end = start + TimeSpan.FromMinutes(cleaning.Credits);
        //     Console.WriteLine($"start is {start} end is {end}, credits is {cleaning.Credits}");
        //     // to mimic the UI, need to copy the cleaning here, rather than adding it directly to the attendant's cleanings
        //     var cleaning_copy = new Cleaning{
        //         Id = cleaning.Id,
        //         IsActive = cleaning.IsActive,
        //         IsCustom = cleaning.IsCustom,
        //         IsPostponed = cleaning.IsPostponed,
        //         IsChangeSheets = cleaning.IsChangeSheets,

        //         From = start,
        //         To = end,
        //         Type = cleaning.Type,
        //         ArrivalExpected = cleaning.ArrivalExpected,
        //         Room = new Room { RoomName=cleaning.Room.RoomName },
        //         Credits = cleaning.Credits
        //     };
        //     // note: cleaning info is NOT passed from UI as a       //
        //     // cleaning.Plan = new CleaningPlan
        //     // {
        //     //     WorkerUsername = attendant.Username,
        //     //     Attendant = attendant,
        //     //     From = start,
        //     //     To = end,
        //     //     CreatedBy = cleaning.Plan?.CreatedBy ?? "Planner"
        //     // };
        //     // instead, modify the cleaning from and to
        //     attendant.Cleanings.Add(cleaning_copy);
        // }

        // protected void AssignRoomNameAsPrePlan(Attendant attendant, Cleaning cleaning, DateTime _start = default(DateTime))
        // {
        //     var start = cleaning.From;
        //     var earliest_date = start.Add(-start.TimeOfDay);
        //     if (_start != default(DateTime))
        //     {
        //         start = _start;
        //     }
        //     else
        //     {
        //         if (attendant.CurrentTimeSlot != null &&
        //             earliest_date + attendant.CurrentTimeSlot.From > start)
        //             start = earliest_date + attendant.CurrentTimeSlot.From;
        //         if (attendant.Cleanings.Count() > 0)
        //         {
        //             var clast = attendant.Cleanings.Max(a => a.To);
        //             if (clast > start)
        //                 start = clast;
        //         }
        //     }
        //     var end = start + TimeSpan.FromMinutes(cleaning.Credits);
        //     // to mimic the UI, need to copy just the room name here, rather than adding it directly to the attendant's cleanings
        //     Console.WriteLine($"start {start}, end {end}");

        //     var cleaning_copy = new Cleaning
        //     {
        //         From = start,
        //         To = end,
        //         Type = cleaning.Type,
        //         ArrivalExpected = cleaning.ArrivalExpected,
        //         Room = new Room { RoomName=cleaning.Room.RoomName },
        //         Credits = cleaning.Credits
        //     };
        //     attendant.Cleanings.Add(cleaning_copy);
        // }

        protected void AssignFloorAffinity(Attendant attendant, string levelname)
        {
            // eventually need to add building name too
            var affinity = new Affinity();
            // affinity.Building = buildingname;
            affinity.Floor = levelname;
            if (attendant.CurrentTimeSlot?.Affinities != null)
            {
                attendant.CurrentTimeSlot.Affinities.Add(affinity);
            }
            else
            {
                attendant.CurrentTimeSlot.Affinities = new List<Affinity>(){affinity};

            }
        }

        protected void AssignBuildingAffinity(Attendant attendant, string buildingname)
        {
            var affinity = new Affinity();
            affinity.Building = buildingname;
            if (attendant.CurrentTimeSlot?.Affinities != null)
            {
                attendant.CurrentTimeSlot.Affinities.Add(affinity);
            }
            else
            {
                attendant.CurrentTimeSlot.Affinities = new List<Affinity>(){affinity};

            }
        }

        protected List<Cleaning> BasicCPSatTest(string hotelId,
                                                string hotelString,
                                                string dataDateString,
                                                DateTime mystart,
                                                DateTime myend,
                                                int attendantCount,
                                                Distances roomDistances = null,
                                                List<CleaningType> acceptableTypes = null,
                                                List<String> floorAffinities = null)
        {
            Console.WriteLine($"{hotelString} {mystart} to {myend} \n{ToString(Strategy.CPSat)}");

            var dataJsonFile = $"Data/{hotelId}_{dataDateString}.json";

            var dataJsonString = File.ReadAllText(dataJsonFile);
            var json = JObject.Parse(dataJsonString);
            //Console.WriteLine(json);
            var hotel = json.ToObject<Hotel>(GetSerializer()).Initialize(dataJsonFile);
            // manual fixing up the json
            // hotel.RefreshRoomsForActivities();
            Hotel.GetRooms(hotel.Structure).ToList().ForEach(r =>
            {
                r.Cleanings.ForEach(c => c.Room = r);
            });

            // hotel.RefreshRoomFloors();
            var floorIndex = 0;
            hotel.Structure.SubLevels.ForEach(floor =>
            {
                floor.FloorIndex = floorIndex++;
                floor.RoomsCount = 0;
                floor.Rooms.ForEach(r => Hotel.SetFloor(r, floor, null, null));
                floor.SubLevels.ForEach(section =>
                {
                    section.Rooms.ForEach(r => Hotel.SetFloor(r, floor, section, null));
                    section.SubLevels.ForEach(subsection => Hotel.GetRooms(subsection).ToList().ForEach(r => Hotel.SetFloor(r, floor, section, subsection)));
                });
            });

            Console.WriteLine($"hotel attendants are {hotel.Attendants.Count()}");

            Attendant[] attendants = hotel.Attendants.Take(attendantCount).ToArray();
            for (int i = 0; i < attendants.Count(); ++i)
            {
                Attendant a = attendants[i];
                a.Id = Guid.NewGuid();
                InitializeAttendantTimeSlots(a);
                if (floorAffinities != null && floorAffinities.Count() > i)
                {
                    AssignFloorAffinity(a, floorAffinities[i]);
                }
                if (a.CurrentTimeSlot.Affinities != null && a.CurrentTimeSlot.Affinities.Count() > 0)
                {
                    Console.WriteLine($"{a.Name}, {a.Id}, {ToString(a.CurrentTimeSlot.Affinities[0])}");
                }
                else
                {
                    Console.WriteLine($"{a.Name}, {a.Id}, no affinities");
                }
            }
            // var workerPlanDay = new WorkerPlanDay();
            // hotel.RefreshCurrentTimeSlots(workerPlanDay);
            // hotel.RefreshLevels();
            var cleanings = Hotel.GetCleanings(hotel.Structure).ToList();
            Console.WriteLine($"initial cleanings are {cleanings.Count()}");

            var _acceptableTypes = new List<CleaningType>() {CleaningType.Vacant, CleaningType.Stay, CleaningType.Departure};
            if (acceptableTypes == null)
            {
                acceptableTypes = _acceptableTypes;
            }

            var ProblemCleanings = cleanings
                .Where( c => c.From >= mystart && c.From < myend && (c.ArrivalExpected || acceptableTypes.Contains(c.Type)));
                // .Where( c => c.Room.Floor.LevelName == "A-13");

            Console.WriteLine($"problem cleanings are {ProblemCleanings.Count()}");
            Dictionary<string, Cleaning> room_lookup = new Dictionary<string, Cleaning>();
            foreach (var c in ProblemCleanings)
            {
                //Console.WriteLine($"{c.From}, {c.To}, {ToString(c)}");
                var room_name = c.Room.RoomName;
                if (! room_lookup.ContainsKey(room_name))
                {
                    room_lookup.Add(room_name, c);
                }
            }
            foreach (var cleaning in cleanings)
            {
                var room_name = cleaning.Room.RoomName;
                if (! room_lookup.ContainsKey(room_name))
                {
                    room_lookup.Add(room_name, cleaning);
                }
            }
            // Assert.Equal(expectedProblems, ProblemCleanings.Count());
            attendants.ToList().ForEach(a => a.Name = a.Name ?? a.Username);

            Cleanings = ProblemCleanings.ToArray();
            var context = new CleaningPlannerContext(Cleanings, attendants,  null, roomDistances, Distances, Strategy, "UTC");

            // var cpsatContext = new CPSAT.CleaningPlannerContext(
            //     attendants: cpsatCleanersAndCleanings.Cleaners.ToArray(),
            //     cleanings: cpsatCleanersAndCleanings.Cleanings.ToArray(),
            //     hotelStrategy: cpsatStrategy,
            //     timeZoneId: timeZoneId,
            //     geoDistances: null,
            //     buildingDistances: CPSAT.Distances.LoadFromJSON(hotelSettings.BuildingsDistanceMatrix),
            //     roomDistances: CPSAT.Distances.LoadFromJSON(hotelSettings.LevelsDistanceMatrix)
            // );

            var cpsatCalculator = new CleaningPlannerCPSAT(context, hotelId);
            cpsatCalculator.CpsatPlannerProgressChanged += TestingPlannerProgressChangedHandler;
            cpsatCalculator.CpsatPlannerResultsGenerated += TestingPlannerResultsGeneratedHandler;
            cpsatCalculator.Plan();
            cpsatCalculator.CpsatPlannerProgressChanged -= TestingPlannerProgressChangedHandler;
            cpsatCalculator.CpsatPlannerResultsGenerated -= TestingPlannerResultsGeneratedHandler;

            foreach (var line in context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation)
               Console.WriteLine(line);

            var assigned_jobs = Cleanings.Where(c => c.Plan != null).ToList();
            Assert.NotEmpty(assigned_jobs);
            assigned_jobs.Sort(delegate (Cleaning x,
                                         Cleaning y)
                               {
                                   if (x.Plan.WorkerUsername == y.Plan.WorkerUsername)
                                       return x.Plan.From.CompareTo(y.Plan.From);
                                   return x.Plan.From.CompareTo(y.Plan.From);
                               });

            // attendants should have cleanings assigned
            var total_rooms = 0;
            var total_credits = 0;
            var total_deparr = 0;
            var total_dep = 0;
            var total_arr = 0;
            for (int i = 0; i < attendants.Count(); ++i)
            {
                Attendant a = attendants[i];
                var aJobs = assigned_jobs.Where(c => c.Plan.WorkerUsername == a.Name).ToList();

                if (aJobs.Count() == 0)
                {
                    continue;
                }
                // Assert.NotEmpty(aJobs);
                aJobs.Sort(delegate (Cleaning x,
                                     Cleaning y)
                {
                    return x.Plan.From.CompareTo(y.Plan.From);
                });
                // floor count should be one
                var rooms = aJobs.Count();
                var credits = aJobs.Sum(c => c.Credits);
                var floors = aJobs.Select(c => c.Room.Floor.LevelName).Distinct();
                var deparr = aJobs.Where(c => c.Type == CleaningType.Departure && c.ArrivalExpected).Count();
                var dep = aJobs.Where(c => c.Type == CleaningType.Departure && !c.ArrivalExpected).Count();
                var arr = aJobs.Where(c => c.Type != CleaningType.Departure && c.ArrivalExpected).Count();
                total_rooms += rooms;
                total_credits += credits;
                total_deparr += deparr;
                total_dep += dep;
                total_arr += arr;
                Console.WriteLine($"{a.Username}: rooms: {rooms}, credits: {credits}, floors: {floors.Count()}, {String.Join(',',floors)}, DEP/ARR = {deparr}, ARR = {arr}, DEP = {dep}");

                // Assert.Equal(1, floors.Count());

                // last attendant typically does less
            }
            var problem_deparr = ProblemCleanings.Where(c => c.Type == CleaningType.Departure && c.ArrivalExpected).Count();
            Console.WriteLine($"Solver: total rooms: {total_rooms} (versus {ProblemCleanings.Count()} requested), total credits: {total_credits}, total DEP/ARR = {total_deparr} (versus {problem_deparr} possible), total ARR = {total_arr}, total DEP = {total_dep}");
            Console.WriteLine("End\n");
            return assigned_jobs;
        }

        protected string ToString(List<Affinity> affinities)
        {
            if (affinities.Count() == 0)
            {
                return "";
            }
            else
            {
                string affinitiesString = string.Join("; ", affinities.Select(a => $"{ToString(a)}"));
                return affinitiesString;
            }
        }

        protected string ToString(Affinity affinity)
        {
            if (affinity != null)
                return $"affinity: bldg={affinity.Building}, floor={affinity.Floor}, section={affinity.Section}, subsection={affinity.SubSection}, room={affinity.Room}, room type={affinity.RoomType} )";
            return "no affinity set";
        }

        protected string ToString(Cleaning cleaning)
        {

            IEnumerable<string> cleaningInfo = GetCleaningInfo(cleaning);
            string ci = string.Join(", ", cleaningInfo);
            Console.WriteLine($"cleaning room is {cleaning.Room}");
            return $"Room {cleaning.Room.RoomName} ({ci})";
        }
        protected IEnumerable<string> GetCleaningInfo(Cleaning cleaning)
        {
            string state=null;
            if (cleaning.Type == CleaningType.Departure)
            {
                if (cleaning.ArrivalExpected)
                    state = "DEP/ARR";
                else
                    state = "DEP";
            }
            else if (cleaning.ArrivalExpected)
                state = "ARR";
            if (state != null)
                yield return state;
            yield return cleaning.Type.ToString();
            yield return $"Credits: {cleaning.Credits}";
            if (cleaning.Type == CleaningType.Departure)
            {
                yield return $"ETD: {cleaning.From.TimeOfDay:hh\\:mm}";
            }
            else
            {
                yield return $"start window: {cleaning.From.TimeOfDay:hh\\:mm}";
            }
            if (cleaning.ArrivalExpected)
            {
                yield return $"arrival expected, ETA: {cleaning.To.TimeOfDay:hh\\:mm}";
            }
            else
            {
                yield return $"end window: {cleaning.To.TimeOfDay:hh\\:mm}";
            }
        }
        protected string ToString(CPSat settings)
        {
            return $"CPSat Solver Settings\n   {string.Join("\n   ", GetSettingsInfo(settings))}";
        }
        protected IEnumerable<string> GetSettingsInfo(CPSat settings)
        {

            yield return $"solverRunTime                               = {settings.solverRunTime}";
            yield return $"UsePrePlan                                  = {settings.UsePrePlan}";
            yield return $"CompletePrePlan                             = {settings.CompletePrePlan}";
            //yield return $"CommunicatingRooms                          = {settings.CommunicatingRooms}";
            yield return $"preferredLevelsAreExclusive                 = {settings.preferredLevelsAreExclusive}";
            //yield return $"minCreditsForMultipleCleanersCleaning       = {settings.minCreditsForMultipleCleanersCleaning}";
            yield return $"minRooms                                    = {settings.minRooms}";
            yield return $"maxRooms                                    = {settings.maxRooms}";
            yield return $"minCredits                             = {settings.minCredits}";
            yield return $"maxCredits                                  = {settings.maxCredits}";
            yield return $"maxTravelTime                               = {settings.maxTravelTime}";
            yield return $"maxBuildingTravelTime                       = {settings.maxBuildingTravelTime}";
            yield return $"maxShiftFloorAllowed                        = {settings.maxShiftFloorAllowed}";
            yield return $"maxBuildingToBuildingDistanceAllowed        = {settings.maxBuildingToBuildingDistanceAllowed}";
            yield return $"maxNumberOfBuildingsPerAttendant            = {settings.maxNumberOfBuildingsPerAttendant}";
            yield return $"maxNumberOfLevelsPerAttendant               = {settings.maxNumberOfLevelsPerAttendant}";
            yield return $"limitAttendantsPerLevel                     = {settings.limitAttendantsPerLevel}";

            yield return $"sortBuildingIntervals                       = {settings.sortBuildingIntervals}";
            // yield return $"travelTime                               = {settings.travelTime}";
            // yield return $"credits                                  = {settings.credits}";
            // yield return $"roomsCleaned                             = {settings.roomsCleaned}";
            yield return $"epsilonCredits                              = {settings.epsilonCredits}";
            yield return $"epsilonRooms                                = {settings.epsilonRooms}";
            yield return $"weightTravelTime                            = {settings.weightTravelTime}";
            yield return $"weightLevelChange                           = {settings.weightLevelChange}";
            yield return $"weightCredits                               = {settings.weightCredits}";
            yield return $"weightRoomsCleaned                          = {settings.weightRoomsCleaned}";
            yield return $"awardRoom                                   = {settings.awardRoom}";
            yield return $"awardLevel                                  = {settings.awardLevel}";
            yield return $"awardBuilding                               = {settings.awardBuilding}";
            // yield return $"weightLevelAward                            = {settings.weightLevelAward}";
            yield return $"weightRoomAward                             = {settings.weightRoomAward}";
            // yield return $"weightBuildingAward                         = {settings.weightBuildingAward}";
            // yield return $"weightMinimizeAttendants                    = {settings.weightMinimizeAttendants}";
            yield return $"useTargetMode                               = {settings.useTargetMode}";
            yield return $"targetModeMinimizeAttendants                = {settings.targetModeMinimizeAttendants}";
            // yield return $"targetModeWeightMaxedRooms                  = {settings.targetModeWeightMaxedRooms}";
            // yield return $"targetModeWeightMaxedCredits                = {settings.targetModeWeightMaxedCredits}";
            yield return $"balanceStayDepartMode                       = {settings.balanceStayDepartMode}";
            yield return $"weightepsilonStayDepart                     = {settings.weightepsilonStayDepart}";
            // yield return $"buildingsDistanceMatrix                     = {settings.buildingsDistanceMatrix}";
            // yield return $"levelsDistanceMatrix                        = {settings.levelsDistanceMatrix}";
            // yield return $"buildingDistanceMultiplier                  = {settings.buildingDistanceMultiplier}";
            yield return $"maxDepartures                               = {settings.maxDepartures}";
            yield return $"maxStays                                    = {settings.maxStays}";
            yield return $"maxDeparturesReducesCredits                 = {settings.maxDeparturesReducesCredits}";
            yield return $"maxDeparturesEquivalentCredits              = {settings.maxDeparturesEquivalentCredits}";
            yield return $"maxDeparturesReductionThreshold             = {settings.maxDeparturesReductionThreshold}";
            yield return $"maxStaysIncreasesCredits                    = {settings.maxStaysIncreasesCredits}";
            yield return $"maxStaysEquivalentCredits                   = {settings.maxStaysEquivalentCredits}";
            yield return $"maxStaysIncreaseThreshold                   = {settings.maxStaysIncreaseThreshold}";
            yield return $"levelMovementReducesCredits                 = {settings.levelMovementReducesCredits}";
            yield return $"levelMovementEquivalentCredits              = {settings.levelMovementEquivalentCredits}";
            yield return $"levelMovementReductionThreshold             = {settings.levelMovementReductionThreshold}";
            yield return $"buildingMovementReducesCredits              = {settings.buildingMovementReducesCredits}";
            yield return $"buildingMovementEquivalentCredits           = {settings.buildingMovementEquivalentCredits}";
            yield return $"cleaningPriorities                          = {settings.cleaningPriorities}";
            yield return $"otherCleaningPriorities                     = {settings.otherCleaningPriorities}";
            // yield return $"strategy                                    = {settings.strategy}";

        }

        protected Newtonsoft.Json.JsonSerializer GetSerializer()
        {
            return new Newtonsoft.Json.JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,

            };
        }

        private async void TestingPlannerProgressChangedHandler(object sender, CleaningPlannerCPSAT.ProgressMessage e)
        {
            // await this._cleaningPlanSignalrService.CpsatPlanningProgressChanged(this._userId, this._hotelGroupId, e);
        }
        private async void TestingPlannerResultsGeneratedHandler(object sender, AutoGeneratedPlan e)
        {

        }
        public void Dispose()
        {
            // Log.CloseAndFlush();
        }
    }
}
