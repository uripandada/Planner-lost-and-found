using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
	public class Settings : ChangeTrackingBaseEntity
	{
		// 1:1 with Hotel
		public Guid Id { get; set; }

		public string HotelId { get; set; }
		public Hotel Hotel { get; set; }

		//public bool CleanOnlyDirtyRooms { get; set; }
		//public bool CleanArrivals { get; set; }
		//public bool CleanStays { get; set; }
		//public bool CleanDepartures { get; set; }
		//public bool CleanVacants { get; set; }
		public string DefaultCheckInTime { get; set; }
		public string DefaultCheckOutTime { get; set; }
		public string DefaultAttendantStartTime { get; set; }
		public string DefaultAttendantEndTime { get; set; }
		public int? DefaultAttendantMaxCredits { get; set; }
		public int? ReserveBetweenCleanings { get; set; }
		public int? TravelReserve { get; set; }
		public bool ShowHoursInWorkerPlanner { get; set; }
		public bool UseOrderInPlanning { get; set; }
		public bool ShowCleaningDelays { get; set; }
		public bool AllowPostponeCleanings { get; set; }

		// THESE THREE PROPERTIES BELOW SHOULD BE GROUPED SOMEHOW
		public string EmailAddressesForSendingPlan { get; set; }
		public bool SendPlanToAttendantsByEmail { get; set; }
		public string FromEmailAddress { get; set; }

		//public bool FixPlannedActivitiesWhileFiltering { get; set; } // Questionable
		public bool UseGroups { get; set; } // Questionable


		public bool CleanHostelRoomBedsInGroups { get; set; }


		public string BuildingsDistanceMatrix { get; set; }
		public string LevelsDistanceMatrix { get; set; }

		public int BuildingAward { get; set; }
		public int LevelAward { get; set; }
		public int RoomAward { get; set; }

		/// <summary>
		/// TODO: REMOVE THIS PROPERTY
		/// </summary>
		public int LevelTime { get; set; }

		/// <summary>
		/// TODO: REMOVE THIS PROPERTY
		/// </summary>
		public int CleaningTime { get; set; }


		public int WeightLevelChange { get; set; }
		public int WeightCredits { get; set; }
		public decimal MinutesPerCredit { get; set; }
		public int MinCreditsForMultipleCleanersCleaning { get; set; }


		//public bool SplitRoomNamesToTwoLines { get; set; } // To remove?


		// public bool CleanArrivalsWithProducts { get; set; } // Not on interface?
		// public bool CleanScheduled { get; set; } // Not on interface?
		// public int HostingCheckInCredits { get; set; } // Not on interface?
		// public int HostingCheckOutCredits { get; set; } // Not on interface?
		// public Dictionary<string, string[]> GroupEmailAddressesForSendingPlan { get; set; } // Not on interface?
		// public string Products { get; set; } // Not on interface?
	}


	//public class HotelStrategy
	//{
	//    public bool CleanOnlyDirtyRooms { get; set; } = true;
	//    public bool CleanArrivals { get; set; } = true;
	//    public bool CleanArrivalsWithProducts { get; set; } = true;
	//    public bool CleanStays { get; set; } = true;
	//    public bool CleanDepartures { get; set; } = true;
	//    public bool CleanVacants { get; set; } = false;
	//    public bool CleanScheduled { get; set; } = true;
	//    [JsonConverter(typeof(CustomTimeSpanConverter), "hh\\:mm")]
	//    public TimeSpan DefaultCheckInTime { get; set; } = TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59));
	//    [JsonConverter(typeof(CustomTimeSpanConverter), "hh\\:mm")]
	//    public TimeSpan DefaultCheckOutTime { get; set; } = TimeSpan.Zero;
	//    [JsonConverter(typeof(CustomTimeSpanConverter), "hh\\:mm")]
	//    public TimeSpan DefaultAttendantStartTime { get; set; } = TimeSpan.FromHours(8);
	//    [JsonConverter(typeof(CustomTimeSpanConverter), "hh\\:mm")]
	//    public TimeSpan DefaultAttendantEndTime { get; set; } = TimeSpan.FromHours(20);
	//    public int DefaultMaxCredits { get; set; } = 0;
	//    public bool SplitRoomNamesToTwoLines { get; set; } = false;
	//    public int ReserveBetweenCleanings { get; set; } = 15;
	//    public int TravelReserve { get; set; } = 5;
	//    public bool FixPlannedActivitiesWhileFiltering { get; set; } = true;
	//    public bool UseGroups { get; set; } = false;
	//    public bool ShowHoursInWorkerPlanner { get; set; } = false;
	//    public bool UseOrderInPlanning { get; set; } = false;
	//    public bool ShowCleaningDelays { get; set; } = false;
	//    public bool AllowPostponeCleanings { get; set; } = false;

	//    public int HostingCheckInCredits { get; set; } = 30;
	//    public int HostingCheckOutCredits { get; set; } = 30;

	//    public string[] EmailAddressesForSendingPlan { get; set; } = new string[0];
	//    public Dictionary<string, string[]> GroupEmailAddressesForSendingPlan { get; set; } = new Dictionary<string, string[]>();
	//    public bool SendPlanToAttendantsByEmail { get; set; } = true;
	//    public string FromEmailAddress { get; set; } = "no-reply@roomchecking.com";

	//    public string Products { get; set; }
	//    public List<PluginBase> Plugins { get; set; } = new List<PluginBase>();

	//    public TimeSlot GetDefaultTimeSlot()
	//    {
	//        return new TimeSlot
	//        {
	//            From = DefaultAttendantStartTime,
	//            To = DefaultAttendantEndTime,
	//            MaxCredits = DefaultMaxCredits
	//        };
	//    }

	//    public static HotelStrategy GetHotelStrategy(Hotel hotel)
	//    {
	//        var strategy = new HotelStrategy();
	//        if (hotel.Name.Contains("Sweet Inn"))
	//        {

	//            strategy.CleanStays = false;
	//            strategy.CleanArrivals = false;
	//            strategy.DefaultCheckInTime = TimeSpan.FromHours(14);
	//            strategy.DefaultCheckOutTime = TimeSpan.FromHours(11);
	//            strategy.DefaultAttendantStartTime = TimeSpan.FromHours(11);
	//            strategy.SplitRoomNamesToTwoLines = true;
	//            strategy.ReserveBetweenCleanings = 0;
	//            strategy.TravelReserve = 0;
	//            if (hotel.Name.Contains("Sweet Inn London"))
	//            {
	//                strategy.EmailAddressesForSendingPlan = new[]
	//                    {
	//                        "sandras@sweetinn.com;",
	//                        "romainl@sweetinn.com;",
	//                        "hostinlondon@sweetinn.com;",
	//                        "mathildas@sweetinn.com;",
	//                        "mayureeb@sweetinn.com;",
	//                        "maryiamt@sweetinn.com;",
	//                        "zakia.khanom@ufcleaning.co.uk;",
	//                        "rijwana.tasnim@ufcleaning.co.uk;",
	//                        "fathimasyed.uf@gmail.com"
	//                    };
	//            }
	//        }
	//        return strategy;
	//    }

	//    public IEnumerable<TPlugin> GetPlugins<TPlugin>()
	//    {
	//        return Plugins.OfType<TPlugin>();
	//    }
	//}

}
