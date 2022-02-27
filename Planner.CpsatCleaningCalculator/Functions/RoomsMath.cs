using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Planner.CpsatCleaningCalculator
{
    /// <summary>
    /// Collecting functions related to calculating room to room distances.
    /// Can probably be integrated directly into Model.Room when functions have stabilized.
    /// </summary>
    public class RoomsMath
    {


        // the cost of traveling between rooms is just the
        // difference between room numbers, in minutes.  Moving
        // from room 6 to room 12 costs 6.  Moving from 4 to 3
        // costs 1, etc.
        //
        // note that if the room numbers include the level, then
        // you naturally get much larger values here.  That is why
        // % 100 is used, to strip out the "level" value from the
        // room number.
        //
        // finally, should probably include a constant here to
        // scale it properly.  It probably takes a minute for an
        // attendant to leave one room and enter another, but the
        // actual walking time will take a handful of seconds if
        // the rooms are consecutive.  Similarly, if the attendant
        // has to walk 20 rooms, that will likely NOT take 20
        // minutes.  So need a factor that will scake the
        // numerical difference into a closer approximation of
        // real minutes walked.

        // static Func<int, int, int> changing_room_cost = (x, y) => Math.Abs(x - y);
        public static Func<Room, Room, int> changing_room_cost = (x, y) => Math.Abs(x.IndexOnFloor - y.IndexOnFloor);

        static private Regex rx_ = new Regex(@"(^\d{1,2})(\d{2})(-\d)?$",
                                             RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static public string TwoDigitRoomNumber(string r)
        {

            // Find matches.
            MatchCollection matches = rx_.Matches(r);
            // what did we get?
            string result = r;

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                if (groups[1].Value.Length == 1)
                {
                    result = $"0{groups[1].Value}-{groups[2].Value}";
                }
                else
                {
                    result = $"{groups[1].Value}-{groups[2].Value}";
                }
                if (groups.Count > 3 && groups[3].Value.Length > 0)
                {
                   // Console.WriteLine($"Ignoring extra {groups[3].Value}");
                }
            }
            return result;
        }

        static public string FloorElevator(string r)
        {
            // Find matches.
            MatchCollection matches = rx_.Matches(r);
            // what did we get?
            string result = r;

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                if (groups[1].Value.Length == 1)
                {
                    result = $"0{groups[1].Value}-Elevator";
                }
                else
                {
                    result = $"{groups[1].Value}-Elevator";
                }
            }
            return result;
        }


        static public int RoomToRoomDistance(Room fromRoom, Distances c = null)
        {
            if (c == null)
            {
                // old way
                // this is placeholder, wanting a proper distance measure
                int maxtime = 1;  // hacky guess here...first and last rooms are cheap??
                int roomdiff = fromRoom.IndexOnFloor;
                return Math.Min(maxtime, roomdiff);
            }
            // move to elevator
            var numberA = TwoDigitRoomNumber(fromRoom.RoomName);
            var numberB = FloorElevator(fromRoom.RoomName);
            return c.GetRoomsDistance(numberA, numberB);
        }

        static public int RoomToRoomDistance(Room fromRoom, Room toRoom, Distances c = null)
        {
            if (fromRoom == toRoom) return 0;
            // else compute room difference
            if (c == null)
            {
                // old way
                int maxtime = 1000;
                // arbitrarily deciding to scale the room number delta
                int roomdiff = changing_room_cost(fromRoom, toRoom);
                if (roomdiff > 5)
                {
                    // scale it
                    roomdiff = (int) Math.Ceiling(5 + (double)(roomdiff - 5)/10);
                    // Console.WriteLine($"distance {fromRoom.RoomName}:{toRoom.RoomName} is {roomdiff}");
                }
                return Math.Min(maxtime, roomdiff);
            }
            // FIXME this only works properly for 8192 hotel.  special case handling of mismatched room names, etc

            // // first, what is the room number? Find the last two digits in the room name
            // // sometimes they look like "0103-5", other times "114"
            var numberA = TwoDigitRoomNumber(fromRoom.RoomName);
            var numberB = TwoDigitRoomNumber(toRoom.RoomName);
            int result =  c.GetRoomsDistance(numberA, numberB);
            // Console.WriteLine($"distance {fromRoom.RoomName}:{toRoom.RoomName} is {result}");
            return result;
        }

    }
}
