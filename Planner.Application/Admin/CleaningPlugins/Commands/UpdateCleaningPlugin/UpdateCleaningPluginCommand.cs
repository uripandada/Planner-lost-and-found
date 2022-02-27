using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.CleaningPlugins.Commands.InsertCleaningPlugin;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningPlugins.Commands.UpdateCleaningPlugin
{
	public class UpdateCleaningPluginCommand : IRequest<ProcessResponse>
	{
		public UpdateCleaningPluginData Data { get; set; }
	}

	public class UpdateCleaningPluginCommandHandler : IRequestHandler<UpdateCleaningPluginCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateCleaningPluginCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateCleaningPluginCommand request, CancellationToken cancellationToken)
		{
			var existingPlugin = await this._databaseContext.CleaningPlugins.Where(cp => cp.Id == request.Data.Id).FirstOrDefaultAsync();

			if (existingPlugin == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find cleaning plugin to update."
				};
			}

			existingPlugin.IsActive = request.Data.IsActive;
			existingPlugin.Name = request.Data.Name;
			existingPlugin.Description = null;
			existingPlugin.IsTopRule = request.Data.IsTopRule;
			existingPlugin.OrdinalNumber = request.Data.OrdinalNumber;
			existingPlugin.Data = new CleaningPluginJson
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
					NightsFromKey = bo.NightsFromKey,
					NightsTypeKey = bo.NightsTypeKey,
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
			};

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				HasError = false,
				IsSuccess = true,
				Message = "Cleaning plugin updated."
			};
		}
	}
}
