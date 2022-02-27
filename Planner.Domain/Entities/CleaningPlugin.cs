using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class CleaningPlugin
	{
		public string HotelId { get; set; }
		public Hotel Hotel { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; }
		public bool IsTopRule { get; set; }
		public int OrdinalNumber { get; set; }
		public CleaningPluginJson Data { get; set; }
	}

	public class CleaningPluginBalancingIntervalJson
	{
		public string IntervalTypeKey { get; set; } // FROM, MORE_THAN
		public int FromNights { get; set; }
		public int ToNights { get; set; }
		public string IntervalBalancingTypeKey { get; set; } // AUTOBALANCE, EVERY_N_DAYS
		public int AutobalancingNumberOfCleanings { get; set; }
		public int EveryNumberOfDays { get; set; }
		public string EveryNumberOfDaysFromKey { get; set; } // CHECK_IN, FIRST_MONDAY, FIRST_TUESDAY, FIRST_WEDNESDAY, FIRST_THURSDAY, FIRST_FRIDAY, FIRST_SATURDAY, FIRST_SUNDAY
	}

	public class CleaningPluginPeriodicalIntervalJson
	{
		public int NumberOfCleanings { get; set; }
		public int EveryNumberOfDays { get; set; }
		public int FromNights { get; set; }
		public int ToNights { get; set; }
		public string FromDayKey { get; set; } // CHECK_IN, FIRST_MONDAY, FIRST_TUESDAY, FIRST_WEDNESDAY, FIRST_THURSDAY, FIRST_FRIDAY, FIRST_SATURDAY, FIRST_SUNDAY
		public string PeriodTypeKey { get; set; } // BALANCE_OVER_RESERVATION_DURATION, BALANCE_OVER_PERIOD, ONCE_EVERY_N_DAYS
		public string IntervalTypeKey { get; set; } // FROM, MORE_THAN
	}


	public class CleaningPluginJson
	{
		//public int? BalancingFactor { get; set; }
		//public int? WeeklyBalancingFactor { get; set; }




		//public int? BalancingPeriodicalEveryNumberOfDays { get; set; }
		//public int? BalancingPeriodicalNumberOfCleanings { get; set; }
		//public IEnumerable<CleaningPluginBalancingIntervalJson> BalancingIntervals { get; set; }
		//public bool? BalancingPostponeSundayCleaningsToMonday { get; set; }

		public IEnumerable<CleaningPluginPeriodicalIntervalJson> PeriodicalIntervals { get; set; }
		public bool? PeriodicalPostponeSundayCleaningsToMonday { get; set; }





		public bool ChangeSheets { get; set; }
		public bool IsNightlyCleaningPlugin { get; set; }
		public bool CleanOnHolidays { get; set; }
		public bool CleanOnSaturday { get; set; }
		public bool CleanOnSunday { get; set; }
		public string Color { get; set; }
		public IEnumerable<string> DailyCleaningTypeTimes { get; set; }
		public string DailyCleaningTimeTypeKey { get; set; }
		public string DisplayStyleKey { get; set; }
		public string Instructions { get; set; }
		public string MonthlyCleaningTypeTimeOfMonthKey { get; set; }
		public string Name { get; set; }
		public bool PostponeUntilVacant { get; set; }
		public int? StartsCleaningAfter { get; set; }
		public string TypeKey { get; set; }
		public IEnumerable<int> WeekBasedCleaningTypeWeeks { get; set; }
		public bool? WeeklyCleanOnMonday { get; set; }
		public bool? WeeklyCleanOnTuesday { get; set; }
		public bool? WeeklyCleanOnWednesday { get; set; }
		public bool? WeeklyCleanOnThursday { get; set; }
		public bool? WeeklyCleanOnFriday { get; set; }
		public bool? WeeklyCleanOnSaturday { get; set; }
		public bool? WeeklyCleanOnSunday { get; set; }

		public IEnumerable<string> WeeklyCleaningTypeMondayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeTuesdayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeWednesdayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeThursdayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeFridayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeSaturdayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeSundayTimes { get; set; }

		public string WeeklyTimeMondayTypeKey { get; set; }
		public string WeeklyTimeTuesdayTypeKey { get; set; }
		public string WeeklyTimeWednesdayTypeKey { get; set; }
		public string WeeklyTimeThursdayTypeKey { get; set; }
		public string WeeklyTimeFridayTypeKey { get; set; }
		public string WeeklyTimeSaturdayTypeKey { get; set; }
		public string WeeklyTimeSundayTypeKey { get; set; }

		public string WeekBasedCleaningDayOfTheWeekKey { get; set; }
	
		public IEnumerable<CleaningPluginBasedOnJson> BasedOns { get; set; }
	}

	public class CleaningPluginBasedOnJson
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Key { get; set; }
		public string Description { get; set; }

		public bool? CleanDeparture { get; set; }
		public bool? CleanStay { get; set; }
		public bool? CleanVacant { get; set; }
		public int? CleanVacantEveryNumberOfDays { get; set; }
		public bool? CleanOutOfService { get; set; }

		public bool? ProductsTagsMustBeConsumedOnTime { get; set; }
		public DateTime? ProductsTagsConsumationIntervalFrom { get; set; }
		public DateTime? ProductsTagsConsumationIntervalTo { get; set; }

		public string CleanlinessKey { get; set; }
		public string NightsTypeKey { get; set; }
		public int? NightsEveryNumberOfDays { get; set; }
		public string NightsFromKey { get; set; }

		//public IEnumerable<Guid> RoomIds { get; set; }
		public IEnumerable<Guid> FoorIds { get; set; }
		public IEnumerable<string> Sections { get; set; }
		public IEnumerable<string> SubSections { get; set; }
		public IEnumerable<int> Nights { get; set; }
		public IEnumerable<string> ReservationSpaceCategories { get; set; }
		public IEnumerable<string> ProductsTags { get; set; }

		public IEnumerable<HotelRoomCreditsDataJson> Rooms { get; set; }
		public IEnumerable<CleaningPluginKeyValueJson> OtherProperties { get; set; }
		public IEnumerable<CleaningPluginBasedOnRoomCategoryJson> Categories { get; set; }
		public IEnumerable<CleaningPluginBasedOnProductsTagsExtendedJson> ProductsTagsExtended { get; set; }
		public IEnumerable<BasedOnOtherPropertiesExtendedJson> OtherPropertiesExtended { get; set; }
	}


	public class HotelRoomCreditsDataJson
	{
		public Guid RoomId { get; set; }
		public int? Credits { get; set; }
	}

	public class CleaningPluginKeyValueJson
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class BasedOnOtherPropertiesExtendedJson
	{
		/// <summary>
		/// BasedOnOtherPropertiesType enum
		/// </summary>
		public string BasedOnOtherPropertiesTypeKey { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class CleaningPluginBasedOnRoomCategoryJson
	{
		public Guid CategoryId { get; set; }
		public bool IsSelected { get; set; }
		public int Credits { get; set; }
	}

	public class CleaningPluginBasedOnProductsTagsExtendedJson
	{
		/// <summary>
		/// BasedOnProductsTagsType enum
		/// </summary>
		public string BasedOnProductsTagsTypeKey { get; set; }
		public bool IsCaseSensitive { get; set; }
		public string ComparisonValue { get; set; }
		public string ProductId { get; set; }
	}
}
