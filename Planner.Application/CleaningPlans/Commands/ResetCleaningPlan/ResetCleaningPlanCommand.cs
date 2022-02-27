using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Planner.Application.CleaningPlans.Commands.ResetCleaningPlan
{
	public class ResetCleaningPlanCommand : IRequest<ProcessResponse>
	{
		public Guid CleaningPlanId { get; set; }
		public bool IsTodaysCleaningPlan { get; set; }
	}

	public class ResetCleaningPlanCommandHandler : IRequestHandler<ResetCleaningPlanCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ResetCleaningPlanCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(ResetCleaningPlanCommand request, CancellationToken cancellationToken)
		{
			var plan = (Domain.Entities.CleaningPlan)null;
			var plannedItems = (IEnumerable<CleaningTimelineItemData>)null;
			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				plan = await this._databaseContext.CleaningPlans.FindAsync(request.CleaningPlanId);

				var items = await this._databaseContext
					.CleaningPlanItems
					.Where(cpi => cpi.CleaningPlanGroup.CleaningPlanId == request.CleaningPlanId && cpi.IsPlanned)
					.ToListAsync();

				foreach(var pc in items)
				{
					pc.IsPlanned = false;
					pc.StartsAt = null;
					pc.EndsAt = null;
					pc.DurationSec = null;
					pc.CleaningPlanGroupId = null;
				}

				plannedItems = items.Select(pc => new CleaningTimelineItemData
				{
					Id = pc.Id.ToString(),
					CleaningPluginId = pc.CleaningPluginId,
					CleaningPluginName = pc.Description,
					Credits = pc.Credits,
					IsPostponed = pc.IsPostponed,
					IsActive = pc.IsActive,
					IsCustom = pc.IsCustom,
					RoomId = pc.RoomId,
					CleaningDescription = pc.Description,
					IsChangeSheets = pc.IsChangeSheets,
					IsPriority = pc.IsPriority,
					ItemTypeKey = "CLEANING",
					BedId = pc.RoomBedId,
					IsSent = pc.CleaningId != null,
					
				}).ToArray();

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);

			}

			if (plannedItems.Any())
			{
				var planFromDate = plan.Date.Date;
				var planToDate = planFromDate.AddDays(1);
				var roomsMap = await this._databaseContext.Rooms.GetRoomsWithStructureAndActiveReservationsQuery(plan.HotelId, planFromDate, null, null, null, null, null, true, request.IsTodaysCleaningPlan, true).ToDictionaryAsync(r => r.Id, r => r);

				var hotel = await this._databaseContext.Hotels.FindAsync(plan.HotelId);
				var timeZoneId = Infrastructure.HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
				var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
				var cleaningDateUtc = TimeZoneInfo.ConvertTimeToUtc(planFromDate, timeZoneInfo);

				foreach (var cleaning in plannedItems)
				{
					var room = roomsMap[cleaning.RoomId];

					cleaning.RefreshCleaningStatus(cleaningDateUtc, timeZoneId, room);
				}
			}

			return new ProcessResponse<IEnumerable<CleaningTimelineItemData>>
			{
				Data = plannedItems,
				HasError = false,
				IsSuccess = true,
				Message = "Cleanings reset."
			};
		}
	}
}
