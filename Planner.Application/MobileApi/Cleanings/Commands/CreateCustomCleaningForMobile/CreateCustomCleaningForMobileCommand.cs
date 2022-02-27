using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Cleanings.Commands.CreateCustomCleaningForMobile
{
	public class CreateCustomCleaningForMobileCommand : IRequest<ProcessResponse<Guid>>
	{
		public string HotelId { get; set; }
		public string Description { get; set; }
		public int? Credits { get; set; }
		public Guid RoomId { get; set; }
		public Guid? RoomBedId { get; set; }
		public Guid CleanerId { get; set; }
		public bool? ChangeSheets { get; set; }

	}

	public class CreateCustomCleaningForMobileCommandHandler : IRequestHandler<CreateCustomCleaningForMobileCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;
		private readonly ISystemEventsService _eventsService;

		public CreateCustomCleaningForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService eventsService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HttpContext.User.HotelGroupId();
			this._eventsService = eventsService;
		}
		public async Task<ProcessResponse<Guid>> Handle(CreateCustomCleaningForMobileCommand request, CancellationToken cancellationToken)
		{
			var hotel = (Domain.Entities.Hotel)null;

			if (request.HotelId.IsNotNull())
			{
				hotel = await this._databaseContext.Hotels
					.Include(h => h.Settings)
					.Where(aa => aa.Id == request.HotelId)
					.FirstOrDefaultAsync();

				if (hotel == null)
				{
					return new ProcessResponse<Guid>
					{
						Data = Guid.Empty,
						IsSuccess = false,
						HasError = true,
						Message = $"Unable to find hotel with id: {request.HotelId}",
					};
				}
			}
			else
			{
				return new ProcessResponse<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					HasError = true,
					Message = "Hotel id is not set.",
				};
			}

			var timeZoneId = Infrastructure.HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			var localHotelDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

			var isForBed = request.RoomBedId.HasValue;
			var roomBed = (Domain.Entities.RoomBed)null;
			var room = (Domain.Entities.Room)null;

			if (isForBed)
			{
				roomBed = await this._databaseContext.RoomBeds.Include(rb => rb.Room).Where(rb => rb.Id == request.RoomBedId.Value).FirstOrDefaultAsync();
				room = roomBed.Room;
			}
			else
			{
				room = await this._databaseContext.Rooms.Where(r => r.Id == request.RoomId).FirstOrDefaultAsync();
			}

			var cleaningPlan = await this._databaseContext.CleaningPlans.Where(cp => cp.HotelId == hotel.Id && cp.Date == localHotelDateTime.Date).FirstOrDefaultAsync();

			if(cleaningPlan == null)
			{
				return new ProcessResponse<Guid>
				{
					Data = Guid.Empty,
					HasError = true,
					IsSuccess = false,
					Message = "There is no cleaning plan for today.",
				};
			}
			else if (!cleaningPlan.IsSent)
			{
				return new ProcessResponse<Guid>
				{
					Data = Guid.Empty,
					HasError = true,
					IsSuccess = false,
					Message = "Todays cleaning plan must be sent in order to create cleanings.",
				};
			}

			var cleaningPlanGroup = await this._databaseContext
				.CleaningPlanGroups
				.Where(cpg => cpg.CleanerId == request.CleanerId && cpg.CleaningPlanId == cleaningPlan.Id)
				.FirstOrDefaultAsync();

			if(cleaningPlanGroup == null)
			{
				return new ProcessResponse<Guid>
				{
					Data = Guid.Empty,
					HasError = true,
					IsSuccess = false,
					Message = "Selected cleaner is not cleaning today.",
				};

				//cleaningPlanGroup = new CleaningPlanGroup
				//{
				//	Id = Guid.NewGuid(),
				//	CleanerId = request.CleanerId,
				//	CleaningPlanId = cleaningPlan.Id,
				//	MaxCredits = null,
				//	MaxDepartures = null,
				//	MaxTwins = null,
				//	MustFillAllCredits = false,
				//	SecondaryCleanerId = null,
				//	WeeklyHours = null,
				//};

				//await this._databaseContext.CleaningPlanGroups.AddAsync(cleaningPlanGroup);
			}

			var credits = request.Credits ?? 30;
			var cleaningEndsAt = localHotelDateTime.AddMinutes(Convert.ToDouble(credits * hotel.Settings.MinutesPerCredit));
			var durationSec = Convert.ToInt32((cleaningEndsAt - localHotelDateTime).TotalSeconds);

			var cleaning = new Domain.Entities.Cleaning
			{
				CleanerId = request.CleanerId,
				CleaningPlanId = cleaningPlan.Id,
				DurationSec = durationSec,
				StartsAt = localHotelDateTime,
				EndsAt = cleaningEndsAt,
				Id = Guid.NewGuid(),
				CleaningPluginId = null,
				Credits = credits,
				IsActive = true,
				IsCustom =true,
				IsChangeSheets = request.ChangeSheets ?? false,
				Description = request.Description,
				IsPlanned = true,
				IsPostponed = false,
				RoomBedId = request.RoomBedId,
				RoomId = request.RoomId,
				IsInspected = false,
				IsInspectionRequired = true,
				Status = CleaningProcessStatus.NEW,
				InspectedById = null,
				IsInspectionSuccess = false,
				IsReadyForInspection = false,
				IsPriority = room.IsCleaningPriority,				
				
			};

			var cleaningPlanItem = new Domain.Entities.CleaningPlanItem
			{
				IsPriority = room.IsCleaningPriority,
				CleaningId = cleaning.Id,
				Id = Guid.NewGuid(),
				CleaningPlanGroupId = cleaningPlanGroup.Id,
				CleaningPlanId = cleaningPlan.Id,
				CleaningPluginId = null,
				Credits = credits,
				Description = request.Description,
				DurationSec = durationSec,
				StartsAt = localHotelDateTime,
				EndsAt = cleaningEndsAt,
				IsActive = true,
				IsChangeSheets = request.ChangeSheets ?? false,
				IsCustom = true,
				IsPlanned = true,
				IsPostponed = false,
				IsPostponee = false,
				IsPostponer = false,
				RoomId = request.RoomId,
				RoomBedId = request.RoomBedId,
				PostponeeCleaningPlanItemId = null,
				PostponerCleaningPlanItemId = null,
			};

			var plannedAttendantTasks = await this._LoadTasksForPlannedAttendants(hotel.Id, cleaningPlan.Date, request.RoomId, request.RoomBedId);
			foreach (var task in plannedAttendantTasks)
			{
				task.UserId = cleaning.CleanerId;
				task.ModifiedAt = localHotelDateTime;
				task.ModifiedById = this._userId;
			}

			await this._databaseContext.Cleanings.AddAsync(cleaning);
			await this._databaseContext.CleaningPlanItems.AddAsync(cleaningPlanItem);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			var cleaningChangedEvent = new CleaningChangedEventData
			{
				At = localHotelDateTime,
				IsDndRetry = false,
				HotelGroupId = this._hotelGroupId,
				RoomId = cleaning.RoomId,
				Status = cleaning.Status,
				UserId = cleaning.CleanerId,
				CleaningPlanId = cleaningPlan.Id,
				CleaningId = cleaning.Id,
				IsRemovedFromSentPlan = false,
				CleaningPlanItemId = cleaningPlanItem.Id,
			};

			await this._eventsService.CleaningsSent(this._hotelGroupId, new CleaningChangedEventData[] { cleaningChangedEvent });
			if (plannedAttendantTasks.Any())
			{
				await this._eventsService.TasksChanged(this._hotelGroupId, new Guid[] { cleaning.CleanerId }, plannedAttendantTasks.Select(t => t.Id).ToArray(), "You have new tasks.");
			}

			return new ProcessResponse<Guid>
			{
				Data = cleaning.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Cleaning created"
			};
		}

		/// <summary>
		/// TODO: FIX AND EXTEND FOR ROOM BEDS
		/// </summary>
		/// <param name="hotelId"></param>
		/// <param name="localPlanDate"></param>
		/// <param name="roomId"></param>
		/// <param name="roomBedId"></param>
		/// <returns></returns>
		private async Task<IEnumerable<SystemTask>> _LoadTasksForPlannedAttendants(string hotelId, DateTime localPlanDate, Guid roomId, Guid? roomBedId)
		{
			var tasksQuery = this._databaseContext
				.SystemTasks
				.Include(st => st.User)
				.Include(st => st.Actions)
				.Where(t =>
					(t.FromHotelId == hotelId || t.ToHotelId == hotelId) &&
					t.IsForPlannedAttendant &&
					t.StartsAt.Date == localPlanDate.Date &&
					t.StatusKey != TaskStatusType.CANCELLED.ToString()
				).AsQueryable();

			tasksQuery = tasksQuery.Where(t => (t.ToRoomId != null && t.ToRoomId == roomId));

			var tasks = await tasksQuery.ToListAsync();
			return tasks;
		}
	}
}
