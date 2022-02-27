using System.Collections.Generic;

namespace Planner.CpsatCleaningCalculator
{

    public class CleaningPlannerContext
    {
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Cleanings are the result of cleaning plugins.
        /// </summary>
        public Cleaning[] Cleanings { get; set; }

        /// <summary>
        /// Attendants are all planned cleaners. If the cleaner already has some cleanings planned, they have to be in the Cleanings property inside.
        /// </summary>
        public Attendant[] Attendants { get; set; }

        public List<List<Room>> communicatingRoom;

        public GeoDistances GeoDistances { get; }
        public Distances RoomDistances { get; }
        public Distances BuildingDistances { get; }

        public HotelStrategy HotelStrategy { get; set; }

        public CleaningPlannerContext(Cleaning[] cleanings, Attendant[] attendants, GeoDistances geoDistances, Distances roomDistances, Distances buildingDistances, HotelStrategy hotelStrategy, string timeZoneId)
        {
            Cleanings = cleanings;
            Attendants = attendants;
            GeoDistances = geoDistances;
            RoomDistances = roomDistances;
            BuildingDistances = buildingDistances;
            HotelStrategy = hotelStrategy;
            TimeZoneId = timeZoneId;
        }
    }
}
