using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Admin.CleaningPlugins.Queries.GetCleaningPluginDetails;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningPlugins.Commands.InsertCleaningPlugin
{
	public class InsertCleaningPluginData
	{
		public bool IsActive { get; set; }
		public string HotelId { get; set; }
		public int OrdinalNumber { get; set; }

		public IEnumerable<SavePeriodicalIntervalData> PeriodicalIntervals { get; set; }
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
		public bool IsTopRule { get; set; }
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

		public string WeekBasedCleaningDayOfTheWeekKey { get; set; } // MONDAY; TUESDAY; WEDNESDAY,...

		public IEnumerable<SaveBasedOnData> BasedOns { get; set; }
	}

	public class SavePeriodicalIntervalData
	{
		public int NumberOfCleanings { get; set; }
		public int EveryNumberOfDays { get; set; }
		public int FromNights { get; set; }
		public int ToNights { get; set; }
		public string FromDayKey { get; set; } // CHECK_IN, FIRST_MONDAY, FIRST_TUESDAY, FIRST_WEDNESDAY, FIRST_THURSDAY, FIRST_FRIDAY, FIRST_SATURDAY, FIRST_SUNDAY
		public string PeriodTypeKey { get; set; } // BALANCE_OVER_RESERVATION, BALANCE_OVER_PERIOD, ONCE_EVERY_N_DAYS
		public string IntervalTypeKey { get; set; } // FROM, MORE_THAN
	}

	public class SaveBasedOnData
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
		public IEnumerable<Guid> FoorIds { get; set; }
		public IEnumerable<string> Sections { get; set; }
		public IEnumerable<string> SubSections { get; set; }
		public IEnumerable<int> Nights { get; set; }
		public IEnumerable<string> ReservationSpaceCategories { get; set; }
		public IEnumerable<string> ProductsTags { get; set; }

		public IEnumerable<HotelRoomCreditsData> Rooms { get; set; }
		public IEnumerable<KeyValue> OtherProperties { get; set; }
		public IEnumerable<SaveBasedOnRoomCategory> Categories { get; set; }

		public IEnumerable<SaveBasedOnProductsTags> ProductsTagsExtended { get; set; }
		public IEnumerable<SaveBasedOnOtherProperties> OtherPropertiesExtended { get; set; }
	}

	public enum BasedOnProductsTagsType
	{
		EQUALS,
		CONTAINS,
		BEGINS_WITH,
		ENDS_WITH,
		REGEX,
		EXISTING_PRODUCT,
	}
	public enum BasedOnOtherPropertiesType
	{
		EQUALS,
		CONTAINS,
		BEGINS_WITH,
		ENDS_WITH,
		REGEX,
	}

	public class SaveBasedOnProductsTags
	{
		/// <summary>
		/// BasedOnProductsTagsType enum
		/// </summary>
		public string BasedOnProductsTagsTypeKey { get; set; }
		public bool IsCaseSensitive { get; set; }
		public string ComparisonValue { get; set; }
		public string ProductId { get; set; }
	}

	public class SaveBasedOnOtherProperties
	{
		/// <summary>
		/// BasedOnOtherPropertiesType enum
		/// </summary>
		public string BasedOnOtherPropertiesTypeKey { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
	public class SaveBasedOnRoomCategory
	{
		public Guid CategoryId { get; set; }
		public bool IsSelected { get; set; }
		public int Credits { get; set; }
	}

	public class UpdateCleaningPluginData : InsertCleaningPluginData
	{
		public Guid Id { get; set; }
	}

	public class InsertCleaningPluginCommand : IRequest<ProcessResponse<Guid>>
	{
		public InsertCleaningPluginData Data { get; set; }
	}

	public class InsertCleaningPluginCommandHandler : IRequestHandler<InsertCleaningPluginCommand, ProcessResponse<Guid>>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertCleaningPluginCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertCleaningPluginCommand request, CancellationToken cancellationToken)
		{
			var cleaningPlugin = new CleaningPlugin
			{

				Name = request.Data.Name,
				Description = null,
				Id = Guid.NewGuid(),
				HotelId = request.Data.HotelId,
				IsActive = request.Data.IsActive,
				IsTopRule = request.Data.IsTopRule,
				OrdinalNumber = request.Data.OrdinalNumber,
				Data = new CleaningPluginJson
				{
					BasedOns = request.Data.BasedOns.Select(bo => new CleaningPluginBasedOnJson
					{
						CleanDeparture = bo.CleanDeparture,
						CleanOutOfService = bo.CleanOutOfService,
						CleanStay = bo.CleanStay,
						CleanVacant = bo.CleanVacant,
						CleanVacantEveryNumberOfDays = bo.CleanVacantEveryNumberOfDays,
						Description = bo.Description,
						FoorIds = bo.FoorIds,
						Id = Guid.NewGuid().ToString(),
						Key = bo.Key,
						Name = bo.Name,
						Nights = bo.Nights,
						ProductsTags = bo.ProductsTags,
						ReservationSpaceCategories = bo.ReservationSpaceCategories,
						Sections = bo.Sections,
						SubSections = bo.SubSections,
						NightsEveryNumberOfDays = bo.NightsEveryNumberOfDays,
						NightsTypeKey = bo.NightsTypeKey,
						NightsFromKey = bo.NightsFromKey,
						CleanlinessKey = bo.CleanlinessKey,
						ProductsTagsConsumationIntervalFrom = bo.ProductsTagsConsumationIntervalFrom,
						ProductsTagsConsumationIntervalTo = bo.ProductsTagsConsumationIntervalTo,
						ProductsTagsMustBeConsumedOnTime = bo.ProductsTagsMustBeConsumedOnTime,
						Rooms = bo.Rooms == null ? new HotelRoomCreditsDataJson[0] : bo.Rooms.Select(bor => new HotelRoomCreditsDataJson { RoomId = bor.RoomId, Credits = bor.Credits }),
						OtherProperties = bo.OtherProperties.Select(op => new CleaningPluginKeyValueJson { Key = op.Key, Value = op.Value }).ToArray(),
						Categories = bo.Categories == null ? new CleaningPluginBasedOnRoomCategoryJson[0] : bo.Categories.Select(c => new CleaningPluginBasedOnRoomCategoryJson
						{
							CategoryId = c.CategoryId,
							Credits = c.Credits,
							IsSelected = c.IsSelected
						}).ToArray(),
						ProductsTagsExtended = bo.ProductsTagsExtended == null ? new CleaningPluginBasedOnProductsTagsExtendedJson[0] : bo.ProductsTagsExtended.Select(pte => new CleaningPluginBasedOnProductsTagsExtendedJson
						{
							BasedOnProductsTagsTypeKey = pte.BasedOnProductsTagsTypeKey,
							ComparisonValue = pte.ComparisonValue,
							IsCaseSensitive = pte.IsCaseSensitive,
							ProductId = pte.ProductId,
						}).ToArray(),
						OtherPropertiesExtended = bo.OtherPropertiesExtended == null ? new BasedOnOtherPropertiesExtendedJson[0] : bo.OtherPropertiesExtended.Select(ope => new BasedOnOtherPropertiesExtendedJson 
						{ 
							Key = ope.Key,
							BasedOnOtherPropertiesTypeKey = ope.BasedOnOtherPropertiesTypeKey,
							Value = ope.Value,
						}).ToArray(),
					}).ToArray(),
					PeriodicalIntervals = request.Data.PeriodicalIntervals.Select(pi => new CleaningPluginPeriodicalIntervalJson
					{
						FromDayKey = pi.FromDayKey,
						EveryNumberOfDays = pi.EveryNumberOfDays,
						NumberOfCleanings = pi.NumberOfCleanings,
						FromNights = pi.FromNights,
						PeriodTypeKey = pi.PeriodTypeKey,
						IntervalTypeKey = pi.IntervalTypeKey,
						ToNights = pi.ToNights,
					}).ToArray(),
					PeriodicalPostponeSundayCleaningsToMonday = request.Data.PeriodicalPostponeSundayCleaningsToMonday,

					ChangeSheets = request.Data.ChangeSheets,
					IsNightlyCleaningPlugin = request.Data.IsNightlyCleaningPlugin,
					CleanOnHolidays = request.Data.CleanOnHolidays,
					CleanOnSaturday = request.Data.CleanOnSaturday,
					CleanOnSunday = request.Data.CleanOnSunday,
					Color = request.Data.Color,
					DailyCleaningTimeTypeKey = request.Data.DailyCleaningTimeTypeKey,
					DailyCleaningTypeTimes = request.Data.DailyCleaningTypeTimes,
					DisplayStyleKey = request.Data.DisplayStyleKey,
					Instructions = request.Data.Instructions,
					MonthlyCleaningTypeTimeOfMonthKey = request.Data.MonthlyCleaningTypeTimeOfMonthKey,
					Name = request.Data.Name,
					PostponeUntilVacant = request.Data.PostponeUntilVacant,
					StartsCleaningAfter = request.Data.StartsCleaningAfter,
					TypeKey = request.Data.TypeKey,
					WeekBasedCleaningTypeWeeks = request.Data.WeekBasedCleaningTypeWeeks,
					WeeklyCleaningTypeFridayTimes = request.Data.WeeklyCleaningTypeFridayTimes,
					WeeklyCleaningTypeMondayTimes = request.Data.WeeklyCleaningTypeMondayTimes,
					WeeklyCleaningTypeSaturdayTimes = request.Data.WeeklyCleaningTypeSaturdayTimes,
					WeeklyCleaningTypeSundayTimes = request.Data.WeeklyCleaningTypeSundayTimes,
					WeeklyCleaningTypeThursdayTimes = request.Data.WeeklyCleaningTypeThursdayTimes,
					WeeklyCleaningTypeTuesdayTimes = request.Data.WeeklyCleaningTypeTuesdayTimes,
					WeeklyCleaningTypeWednesdayTimes = request.Data.WeeklyCleaningTypeWednesdayTimes,
					WeeklyCleanOnFriday = request.Data.WeeklyCleanOnFriday,
					WeeklyCleanOnMonday = request.Data.WeeklyCleanOnMonday,
					WeeklyCleanOnSaturday = request.Data.WeeklyCleanOnSaturday,
					WeeklyCleanOnSunday = request.Data.WeeklyCleanOnSunday,
					WeeklyCleanOnThursday = request.Data.WeeklyCleanOnThursday,
					WeeklyCleanOnTuesday = request.Data.WeeklyCleanOnTuesday,
					WeeklyCleanOnWednesday = request.Data.WeeklyCleanOnWednesday,
					WeeklyTimeFridayTypeKey = request.Data.WeeklyTimeFridayTypeKey,
					WeeklyTimeMondayTypeKey = request.Data.WeeklyTimeMondayTypeKey,
					WeeklyTimeSaturdayTypeKey = request.Data.WeeklyTimeSaturdayTypeKey,
					WeeklyTimeSundayTypeKey = request.Data.WeeklyTimeSundayTypeKey,
					WeeklyTimeThursdayTypeKey = request.Data.WeeklyTimeThursdayTypeKey,
					WeeklyTimeTuesdayTypeKey = request.Data.WeeklyTimeTuesdayTypeKey,
					WeeklyTimeWednesdayTypeKey = request.Data.WeeklyTimeWednesdayTypeKey,
					WeekBasedCleaningDayOfTheWeekKey = request.Data.WeekBasedCleaningDayOfTheWeekKey,
				}
			};

			await this._databaseContext.CleaningPlugins.AddAsync(cleaningPlugin);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = cleaningPlugin.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Cleaning plugin inserted."
			};
		}
	}
}
