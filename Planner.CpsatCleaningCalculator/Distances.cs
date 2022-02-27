using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Planner.CpsatCleaningCalculator
{
    public class Distances
    {
        private readonly HotelStrategy _hotelStrategy;
        // private readonly Dictionary<(string,string),List<List<object>>> _roomToRoomDistances;
        // private readonly Dictionary<string,List<List<object>>> _floorToFloorDistances;
        // private readonly List<List<object>> _buildingToBuildingDistances;
        private readonly List<List<object>> _roomToRoomDistance;

        private readonly int[][] _distances;

        public bool UseDistances => _distances != null;
        public bool NeedRefresh;

       // public static Distances GetEmpty(HotelStrategy hotelStrategy = null) => new Distances(null, null, true, hotelStrategy);

        public static Distances LoadFromJSON(string json, HotelStrategy hotelStrategy = null)
        {
            return null;
            if (String.IsNullOrEmpty(json))
                return null;
            var jObject = JsonConvert.DeserializeObject<List<List<object>>>(json);
            return new Distances(jObject, hotelStrategy);
        }

        public Distances( List<List<object>> pairwiseDistances = null, HotelStrategy hotelStrategy = null)
        {
            _roomToRoomDistance = pairwiseDistances;
            if( pairwiseDistances==null)
                _roomToRoomDistance = new List<List<object>>();
            _hotelStrategy = hotelStrategy ?? new HotelStrategy();
        }

        public int GetBuildingDistance(string originBuilding, string destinationBuilding)
        {
            if (originBuilding == destinationBuilding)
            {
                return 0;
            }
            int distance = 10 * _hotelStrategy.CPSat.buildingDistanceMultiplier;
            var roomDistance = _roomToRoomDistance.FirstOrDefault(x => x[0].ToString() == originBuilding && x[1].ToString() == destinationBuilding);

            if (roomDistance != null && roomDistance.Count == 3)
            {
                int.TryParse(roomDistance[2].ToString(), out distance);
                distance *= _hotelStrategy.CPSat.buildingDistanceMultiplier;
            }
            // else
            //     Console.WriteLine($"can't find either origin {originBuilding} or dest {destinationBuilding} in matrix");


            return distance;
        }

        static private Regex rx_ = new Regex(@"(^(\d{2})?-(\d{2}|Elevator)(bis)?)$",
                                             RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public int GetRoomsDistance(string originRoom, string destinationRoom)
        {
            if (originRoom == destinationRoom)
            {
                return 0;
            }
            //int multiplier = _hotelStrategy.CPSat.roomDistanceMultiplier ?? 1;
            int shortscale = 1;
            int longscale = 5;
            int breakpoint = 5;
            // set a default distance
            int distance = 40;
            // parse floor, room from
            var roomDistance = _roomToRoomDistance.FirstOrDefault(x =>
            {
                var matches0 = rx_.Matches(x[0].ToString());
                var matches1 = rx_.Matches(x[1].ToString());
                if (matches0.Count > 0 && matches1.Count > 0)
                {
                    var groups0 = matches0[0].Groups;
                    var groups1 = matches1[0].Groups;
                    return (groups0[1].Value == originRoom &&
                            groups1[1].Value == destinationRoom);
                }
                // Console.WriteLine($"regex failed: {x[0]}, {x[1]}");
                return false;
            });
            if (roomDistance != null && roomDistance.Count == 3)
            {
                int.TryParse(roomDistance[2].ToString(), out distance);
                if (distance < breakpoint )
                {
                    distance = (int) Math.Ceiling(distance / (double) shortscale);
                }
                else
                {
                    distance = (int) Math.Ceiling( breakpoint/2 + (distance - breakpoint) / (double) longscale);
                }

                // distance = (int) Math.Ceiling( distance / (double) scale );
                //Console.WriteLine($"distance from {originRoom} to {destinationRoom} is {distance}");
            }
            // else
            //     Console.WriteLine($"can't find either origin {originRoom} or dest {destinationRoom} in matrix");

            return distance;
        }

        public void LogMatrix()
        {
            Console.WriteLine($"Distances:\n{_distances}");
        }
    }
}
