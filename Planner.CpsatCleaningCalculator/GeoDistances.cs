using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Planner.CpsatCleaningCalculator
{
    public class GeoDistances
    {
        private readonly bool _useAddress;
        private readonly HotelStrategy _hotelStrategy;
        private readonly Dictionary<string, string> _cleanedAddresses;
        private readonly Dictionary<string, int> _locations;
        private readonly int[][] _distances;

        public bool UseLocations => _locations != null;
        public bool UseDistances => _distances != null;
        public bool NeedRefresh;

        public static GeoDistances GetEmpty(HotelStrategy hotelStrategy = null) => new GeoDistances(null, null, true, hotelStrategy);

        public static GeoDistances LoadFromFile(string json, bool useAddress = true, HotelStrategy hotelStrategy = null)
        {
            if (String.IsNullOrEmpty(json))
                return null;
            var jObject = JObject.Parse(json);
            string[] locations = null;
            if (jObject["locations"] != null && jObject["locations"].HasValues)
                locations = jObject["locations"].Select(l => l.Value<string>()).ToArray();

            var distances = jObject["distances"].Select(r => r.Select(e => TimeToMinutes(e.Value<string>())).ToArray()).ToArray();

            var cleanedAddresses = new Dictionary<string, string>();
            if (jObject["cleaned_addresses"] != null && jObject["cleaned_addresses"].HasValues)
            {
                var caObject = (JObject) jObject["cleaned_addresses"];

                foreach (var kvp in caObject)
                {
                    cleanedAddresses.Add(kvp.Key, kvp.Value.ToString());
                }
                return new GeoDistances(locations, distances, useAddress, hotelStrategy, cleanedAddresses);
            }
            else
            {
                return new GeoDistances(locations, distances, useAddress, hotelStrategy);
            }
        }

        public static GeoDistances Parse(string distancesText, string locationsText = null, bool useAddress = true, HotelStrategy hotelStrategy = null)
        {
            var locations = locationsText?.Split(',').ToArray();
            var distances = distancesText.Split(',').Select(r => r.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()).ToArray();
            return new GeoDistances(locations, distances, useAddress, hotelStrategy);
        }

        public GeoDistances(string[] locations, int[][] distances, bool useAddress, HotelStrategy hotelStrategy = null, Dictionary<string, string> cleanedAddresses=null)
        {
            _cleanedAddresses = cleanedAddresses;
            if( cleanedAddresses==null)
                  _cleanedAddresses = new Dictionary<string, string>();

        _locations = locations?.Select((l, i) => new { l, i }).ToDictionary(p => p.l, p => p.i);
            _distances = distances;
            _useAddress = useAddress;
            _hotelStrategy = hotelStrategy ?? new HotelStrategy();
            // are the locations and cleaned address lists in sync?
            var formattedAddresses = new HashSet<string>();
            if (_cleanedAddresses != null)
            {
                foreach (var v in _cleanedAddresses.Values)
                {
                    formattedAddresses.Add(v);
                }
            }
            else
            {
                if (locations != null)
                {
                    foreach (var l in locations)
                    {
                        formattedAddresses.Add(l);
                    }
                }
            }
            if (formattedAddresses == null ||
                _locations == null ||
                formattedAddresses.Count != _locations.Count ||
                formattedAddresses.Count == 0 ||
                _locations.Count == 0)
            {
                NeedRefresh = true;
            }
            else
            {
                // Are all the locations contained in the formatted addresses set?
                for (int i = 0; i < formattedAddresses.Count; i++)
                {
                    if (!formattedAddresses.Contains(locations[i].ToString()))
                    {
                        NeedRefresh = true;
                        break;
                    }
                }
            }
        }

        // change from private to public to aid with updating distance matrix
        public static int TimeToMinutes(string timeText)
        {
            if (timeText.Contains(":"))
            {
                var time = TimeSpan.Parse(timeText);
                return TimeToMinutes(time);
                // return time.Seconds > 0 ? (int)time.TotalMinutes + 1 : (int)time.TotalMinutes;
            }
            return int.Parse(timeText);
        }
        // overloaded version of the above
        public static int TimeToMinutes(TimeSpan time)
        {
            return time.Seconds > 0 ? (int)time.TotalMinutes + 1 : (int)time.TotalMinutes;
        }

        // added this to aid saving to a file
        public string GetJSONString()
        {
            var data = new {
                cleaned_addresses = _cleanedAddresses,
                locations = _locations.Keys.ToArray(),
                distances = _distances
            };
            return JObject.FromObject(data).ToString();
        }


        // added this hack to help with updating distances.  Perhaps a bad idea
        // but the only way I can see to get at the distance matrix itself
        // without just making it public
        public int GetMatrixValue(int fromIndex, int toIndex)
        {
            return _distances[fromIndex][toIndex];
        }

        public int GetDistanceAndActivity<TActivity>(TActivity[] activities, int fromNode, int toNode) where TActivity : Activity
        {
            if (fromNode == toNode || fromNode == 0 || fromNode == activities.Length + 1 || toNode == 0)
            {
                return 0;
            }

            var originActivity = activities[fromNode - 1];
            if (toNode == activities.Length + 1)
            {
                return originActivity.Credits;
            }

            var destinationActivity = activities[toNode - 1];
            var dist = GetDistance(activities, fromNode, toNode);
            var credits = originActivity.Credits + _hotelStrategy.ReserveBetweenCleanings;
            return dist + credits;
        }

        // get the size of the matrix, the assumption being that it is square and you only need one dimension
        public int GetSize()
        {
            if (_distances == null)
                return -1;
            return _distances.Length;
        }


        public int GetDistance<TActivity>(TActivity[] activities, int fromNode, int toNode) where TActivity : Activity
        {
            if (fromNode == toNode || fromNode == 0 || fromNode == activities.Length + 1 || toNode == 0)
            {
                return 0;
            }

            var originActivity = activities[fromNode - 1];
            if (toNode == activities.Length + 1)
            {
                // return originActivity.Credits;
                return 0;
            }

            var destinationActivity = activities[toNode - 1];
            var travelTime = !UseDistances ? 0
                : !UseLocations ? _distances[fromNode - 1][toNode - 1]
                : GetTravelTime(originActivity, destinationActivity);
            travelTime += travelTime > 0 ? _hotelStrategy.TravelReserve : 0;
            // var result = originActivity.Credits + _hotelStrategy.ReserveBetweenCleanings + travelTime + (travelTime > 0 ? _hotelStrategy.TravelReserve : 0);
            return travelTime;
        }



        public int GetBuildingDistance(string originBuilding, string destinationBuilding)
        {
            var originIndex = GetBuildingIndex(originBuilding);
            var destinationIndex = GetBuildingIndex(destinationBuilding);
            return originIndex.HasValue && destinationIndex.HasValue ? _distances[originIndex.Value][destinationIndex.Value] : int.MaxValue;
        }

        public void LogMatrix(Activity[] activities, int nodesCount)
        {
            var matrix = string.Join("\n", Enumerable.Range(0, nodesCount).Select(i =>
                string.Join(",", Enumerable.Range(0, nodesCount).Select(j => GetDistance(activities, j, i)).Select(d => d.ToString("D2")))));
            Console.WriteLine($"Distances:\n{matrix}");
            // Log.GetLog().Debug($"Distances:\n{matrix}");
        }

        public string FormattedAddress(string address)
        {
            string result = "";
            if (_cleanedAddresses != null)
                _cleanedAddresses.TryGetValue(address, out result);
            return result;
        }

        public bool HasAddress(string address)
        {
            return _cleanedAddresses.ContainsKey(address);
        }

        public List<string> MissingAddresses(List<string> addresses)
        {
            var missing = new List<string>();
            foreach (var address in addresses)
            {
                if (!HasAddress(address)){
                    missing.Add(address);
                }
            }
            return missing;
        }

        public bool AnyMissingAddresses(List<string> addresses)
        {
            var unknownAddresses = MissingAddresses(addresses);
            return unknownAddresses.Count == 0;
        }

        private int GetTravelTime(Activity originActivity, Activity destinationActivity)
        {
            var originIndex = GetLocationIndex(originActivity);
            var destinationIndex = GetLocationIndex(destinationActivity);
            int result = 0;
            if (originIndex.HasValue && destinationIndex.HasValue)
            {
                if (originIndex != destinationIndex)
                {
                    // origin and destinations canonical addresses are not identical
                    result = _distances[originIndex.Value][destinationIndex.Value];
                }
                else
                {
                    // just being pedantic here.  It is already 0
                    result = 0;
                }
            }
            else
            {
                // in this case, one or both of the addresses is not found by google, does not have a canonical form, or else the distance matrix must be refreshed.
                result = int.MaxValue;
            }

            if (result > 60)
                return 14400;
            return  result;
        }

        private int? GetLocationIndex(Activity activity)
        {
            var key = _useAddress ? activity.Room.Address : $"{decimal.Parse(activity.Room.Latitude):N6},{decimal.Parse(activity.Room.Longitude):N6}";
            if (_useAddress)
            {
                // first, translate as is address into formatted address
                string formattedAddress = FormattedAddress(key);
                if (!String.IsNullOrEmpty(formattedAddress))
                {
                    key = formattedAddress;
                }
            }
            int? result = _locations.TryGetValue(key, out var index) ? index : (int?) null;
            // Console.WriteLine($"for activity.address {activity.Room.Address} using canonical address {key}, index value is {result}");
            return result;
        }

        private int? GetBuildingIndex(string building)
        {
            var key = building;
            return _locations.TryGetValue(key, out var index) ? index : (int?)null;
        }
    }
}
