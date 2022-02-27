using MediatR;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningPlugins.Queries.GetCleaningPluginDetails
{
	public enum EveryNumberOfDaysFromEnum
	{
		CHECK_IN,
		FIRST_MONDAY,
		FIRST_TUESDAY,
		FIRST_WEDNESDAY,
		FIRST_THURSDAY,
		FIRST_FRIDAY,
		FIRST_SATURDAY,
		FIRST_SUNDAY
	}

	//public enum IntervalBalancingTypeEnum
	//{
	//	AUTOBALANCE,
	//	EVERY_N_DAYS
	//}

	public enum IntervalTypeEnum
	{
		FROM,
		MORE_THAN
	}


	public class CleaningPluginDetailsPeriodicalInterval
	{
		public int NumberOfCleanings { get; set; }
		public int EveryNumberOfDays { get; set; }
		public int FromNights { get; set; }
		public int ToNights { get; set; }
		public string FromDayKey { get; set; } // CHECK_IN, FIRST_MONDAY, FIRST_TUESDAY, FIRST_WEDNESDAY, FIRST_THURSDAY, FIRST_FRIDAY, FIRST_SATURDAY, FIRST_SUNDAY
		public string PeriodTypeKey { get; set; } // BALANCE_OVER_RESERVATION, BALANCE_OVER_PERIOD, ONCE_EVERY_N_DAYS
		public string IntervalTypeKey { get; set; } // FROM, MORE_THAN
	}

	public class CleaningPluginDetailsData
	{
		public Guid Id { get; set; }

		public int OrdinalNumber { get; set; }

		public bool IsActive { get; set; }
		public string HotelId { get; set; }
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
		public bool IsTopRule { get; set; }
		public string MonthlyCleaningTypeTimeOfMonthKey { get; set; }
		public string Name { get; set; }
		public bool PostponeUntilVacant { get; set; }
		public int? StartsCleaningAfter { get; set; }
		public string TypeKey { get; set; }
		public IEnumerable<int> WeekBasedCleaningTypeWeeks { get; set; }


		public IEnumerable<CleaningPluginDetailsPeriodicalInterval> PeriodicalIntervals { get; set; }
		public bool? PeriodicalPostponeSundayCleaningsToMonday { get; set; }

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

		public string WeekBasedCleaningDayOfTheWeekKey { get; set; } // MONDAY; TUESDAY; WEDNESDAY,...

		public IEnumerable<CleaningPluginDetailsBasedOnData> BasedOns { get; set; }

	}

	public class CleaningPluginDetailsBasedOnData
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

		public string NightsTypeKey { get; set; }
		public int? NightsEveryNumberOfDays { get; set; }
		public string NightsFromKey { get; set; }

		public string CleanlinessKey { get; set; }
		//public IEnumerable<Guid> RoomIds { get; set; }
		public IEnumerable<Guid> FoorIds { get; set; }
		public IEnumerable<string> Sections { get; set; }
		public IEnumerable<string> SubSections { get; set; }
		public IEnumerable<int> Nights { get; set; }
		public IEnumerable<string> ReservationSpaceCategories { get; set; }
		public IEnumerable<string> ProductsTags { get; set; }

		public IEnumerable<HotelRoomCreditsData> Rooms { get; set; }
		public IEnumerable<KeyValue> OtherProperties { get; set; }
		public IEnumerable<CleaningPluginDetailsBasedOnRoomCategoryData> Categories { get; set; }
		public IEnumerable<CleaningPluginDetailsBasedOnProdutsTagsExtendedData> ProductsTagsExtended { get; set; }
		public IEnumerable<CleaningPluginDetailsBasedOnOtherPropertiesExtendedData> OtherPropertiesExtended { get; set; }
	}

	public class HotelRoomCreditsData
	{
		public Guid RoomId { get; set; }
		public int? Credits { get; set; }
	}

	public class KeyValue
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class CleaningPluginDetailsBasedOnRoomCategoryData
	{
		public Guid CategoryId { get; set; }
		public bool IsSelected { get; set; }
		public int Credits { get; set; }
	}
	
	public class CleaningPluginDetailsBasedOnProdutsTagsExtendedData
	{
		/// <summary>
		/// BasedOnProductsTagsType enum
		/// </summary>
		public string BasedOnProductsTagsTypeKey { get; set; } 
		public bool IsCaseSensitive { get; set; }
		public string ComparisonValue { get; set; } 
		public string ProductId { get; set; }
	}
	public class CleaningPluginDetailsBasedOnOtherPropertiesExtendedData
	{
		/// <summary>
		/// BasedOnOtherPropertiesType enum
		/// </summary>
		public string BasedOnOtherPropertiesTypeKey { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class GetCleaningPluginDetailsQuery : IRequest<CleaningPluginDetailsData>
	{
		public Guid Id { get; set; }
	}
	public class GetCleaningPluginDetailsQueryHandler : IRequestHandler<GetCleaningPluginDetailsQuery, CleaningPluginDetailsData>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public GetCleaningPluginDetailsQueryHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<CleaningPluginDetailsData> Handle(GetCleaningPluginDetailsQuery request, CancellationToken cancellationToken)
		{
			var cp = await this.databaseContext.CleaningPlugins.FindAsync(request.Id);

			return new CleaningPluginDetailsData
			{
				OrdinalNumber = cp.OrdinalNumber,
				PeriodicalPostponeSundayCleaningsToMonday = cp.Data.PeriodicalPostponeSundayCleaningsToMonday,
				PeriodicalIntervals = cp.Data.PeriodicalIntervals.Select(pi => new CleaningPluginDetailsPeriodicalInterval 
				{ 
					FromDayKey = pi.FromDayKey,
					EveryNumberOfDays = pi.EveryNumberOfDays,
					NumberOfCleanings = pi.NumberOfCleanings,
					FromNights = pi.FromNights,
					PeriodTypeKey = pi.PeriodTypeKey,
					IntervalTypeKey = pi.IntervalTypeKey,
					ToNights = pi.ToNights,
				}).ToArray(),
				Id = cp.Id,
				IsActive = cp.IsActive,
				ChangeSheets = cp.Data.ChangeSheets,
				IsNightlyCleaningPlugin = cp.Data.IsNightlyCleaningPlugin,
				CleanOnHolidays = cp.Data.CleanOnHolidays,
				CleanOnSaturday = cp.Data.CleanOnSaturday,
				CleanOnSunday = cp.Data.CleanOnSunday,
				Color = cp.Data.Color,
				DailyCleaningTimeTypeKey = cp.Data.DailyCleaningTimeTypeKey,
				DailyCleaningTypeTimes = cp.Data.DailyCleaningTypeTimes,
				DisplayStyleKey = cp.Data.DisplayStyleKey,
				HotelId = cp.HotelId,
				Instructions = cp.Data.Instructions,
				IsTopRule = cp.IsTopRule,
				MonthlyCleaningTypeTimeOfMonthKey = cp.Data.MonthlyCleaningTypeTimeOfMonthKey,
				Name = cp.Name,
				PostponeUntilVacant = cp.Data.PostponeUntilVacant,
				StartsCleaningAfter = cp.Data.StartsCleaningAfter,
				TypeKey = cp.Data.TypeKey,
				WeekBasedCleaningTypeWeeks = cp.Data.WeekBasedCleaningTypeWeeks,
				WeeklyCleaningTypeFridayTimes = cp.Data.WeeklyCleaningTypeFridayTimes,
				WeeklyCleaningTypeMondayTimes = cp.Data.WeeklyCleaningTypeMondayTimes,
				WeeklyCleaningTypeSaturdayTimes = cp.Data.WeeklyCleaningTypeSaturdayTimes,
				WeeklyCleaningTypeSundayTimes = cp.Data.WeeklyCleaningTypeSundayTimes,
				WeeklyCleaningTypeThursdayTimes = cp.Data.WeeklyCleaningTypeThursdayTimes,
				WeeklyCleaningTypeTuesdayTimes = cp.Data.WeeklyCleaningTypeTuesdayTimes,
				WeeklyCleaningTypeWednesdayTimes = cp.Data.WeeklyCleaningTypeWednesdayTimes,
				WeeklyCleanOnFriday = cp.Data.WeeklyCleanOnFriday,
				WeeklyCleanOnMonday = cp.Data.WeeklyCleanOnMonday,
				WeeklyCleanOnSaturday = cp.Data.WeeklyCleanOnSaturday,
				WeeklyCleanOnSunday = cp.Data.WeeklyCleanOnSunday,
				WeeklyCleanOnThursday = cp.Data.WeeklyCleanOnThursday,
				WeeklyCleanOnTuesday = cp.Data.WeeklyCleanOnTuesday,
				WeeklyCleanOnWednesday = cp.Data.WeeklyCleanOnWednesday,
				WeeklyTimeFridayTypeKey = cp.Data.WeeklyTimeFridayTypeKey,
				WeeklyTimeMondayTypeKey = cp.Data.WeeklyTimeMondayTypeKey,
				WeeklyTimeSaturdayTypeKey = cp.Data.WeeklyTimeSaturdayTypeKey,
				WeeklyTimeSundayTypeKey = cp.Data.WeeklyTimeSundayTypeKey,
				WeeklyTimeThursdayTypeKey = cp.Data.WeeklyTimeThursdayTypeKey,
				WeeklyTimeTuesdayTypeKey = cp.Data.WeeklyTimeTuesdayTypeKey,
				WeeklyTimeWednesdayTypeKey = cp.Data.WeeklyTimeWednesdayTypeKey,
				WeekBasedCleaningDayOfTheWeekKey = cp.Data.WeekBasedCleaningDayOfTheWeekKey,
				BasedOns = cp.Data.BasedOns.Select(bo => new CleaningPluginDetailsBasedOnData
				{
					Categories = bo.Categories.Select(c => new CleaningPluginDetailsBasedOnRoomCategoryData
					{
						CategoryId = c.CategoryId,
						Credits = c.Credits,
						IsSelected = c.IsSelected
					}).ToArray(),
					ProductsTagsExtended = bo.ProductsTagsExtended == null ? new CleaningPluginDetailsBasedOnProdutsTagsExtendedData[0] : bo.ProductsTagsExtended.Select(pte => new CleaningPluginDetailsBasedOnProdutsTagsExtendedData 
					{
						BasedOnProductsTagsTypeKey = pte.BasedOnProductsTagsTypeKey,
						ComparisonValue = pte.ComparisonValue,
						IsCaseSensitive = pte.IsCaseSensitive,
						ProductId = pte.ProductId,
					}).ToArray(),
					Rooms = bo.Rooms.Select(bor => new HotelRoomCreditsData 
					{
						RoomId = bor.RoomId,
						Credits = bor.Credits
					}),
					OtherPropertiesExtended = bo.OtherPropertiesExtended == null ? new CleaningPluginDetailsBasedOnOtherPropertiesExtendedData[0] : bo.OtherPropertiesExtended.Select(ope => new CleaningPluginDetailsBasedOnOtherPropertiesExtendedData 
					{ 
						Key = ope.Key,
						Value = ope.Value,
						BasedOnOtherPropertiesTypeKey = ope.BasedOnOtherPropertiesTypeKey,
					}).ToArray(),
					ProductsTagsConsumationIntervalFrom = bo.ProductsTagsConsumationIntervalFrom,
					ProductsTagsConsumationIntervalTo = bo.ProductsTagsConsumationIntervalTo,
					ProductsTagsMustBeConsumedOnTime = bo.ProductsTagsMustBeConsumedOnTime,
					CleanDeparture = bo.CleanDeparture,
					CleanOutOfService = bo.CleanOutOfService,
					CleanStay = bo.CleanStay,
					CleanVacant = bo.CleanVacant,
					CleanVacantEveryNumberOfDays = bo.CleanVacantEveryNumberOfDays,
					Description = bo.Description,
					FoorIds = bo.FoorIds,
					Id = bo.Id,
					Key = bo.Key,
					Name = bo.Name,
					Nights = bo.Nights,
					ProductsTags = bo.ProductsTags,
					ReservationSpaceCategories = bo.ReservationSpaceCategories,
					Sections = bo.Sections,
					SubSections = bo.SubSections,
					NightsEveryNumberOfDays = bo.NightsEveryNumberOfDays,
					NightsFromKey = bo.NightsFromKey,
					NightsTypeKey = bo.NightsTypeKey,
					OtherProperties = bo.OtherProperties.Select(op => new KeyValue { Key = op.Key, Value = op.Value }).ToArray(),
					CleanlinessKey = bo.CleanlinessKey,
				}).ToArray()
			};
		}
	}
}
