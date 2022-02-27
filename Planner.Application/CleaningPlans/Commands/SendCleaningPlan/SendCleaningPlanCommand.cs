using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.Infrastructure;
using Planner.Application.Infrastructure.Signalr.Hubs;
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

namespace Planner.Application.CleaningPlans.Commands.SendCleaningPlan
{
	public class SendCleaningPlanResponse
	{
		public DateTime SentAt { get; set; }
	}

	public class SendCleaningPlanCommand: IRequest<ProcessResponse<SendCleaningPlanResponse>>
	{
		public Guid CleaningPlanId { get; set; }
		public bool OverrideAlreadySentPlan { get; set; }
	}

	public class SendCleaningPlanCommandHandler : IRequestHandler<SendCleaningPlanCommand, ProcessResponse<SendCleaningPlanResponse>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISystemEventsService _eventService;

		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;


		public SendCleaningPlanCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, IHubContext<RoomsOverviewHub> roomsOverviewHub, ISystemEventsService eventService)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._eventService = eventService;
			this._userId = httpContextAccessor.UserId();
			this._hotelGroupId = httpContextAccessor.HttpContext.User.HotelGroupId();
		}

		public async Task<ProcessResponse<SendCleaningPlanResponse>> Handle(SendCleaningPlanCommand request, CancellationToken cancellationToken)
		{
			var response = (ProcessResponse<SendCleaningPlanResponse>)null;
			var cleaningChangedEvents = new List<CleaningChangedEventData>();
			var dateTime = DateTime.UtcNow;
			var userIds = new HashSet<Guid>();
			var taskIds = new List<Guid>();
			var sendCleaningNotifications = true;
			var sendMobileNotificationsForTasks = true;

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var cleaningPlan = await this._databaseContext.CleaningPlans
					.Include(cp => cp.CleaningPlanCpsatConfiguration)
					.Include(cp => cp.Groups).ThenInclude(cpg => cpg.Items).ThenInclude(cpgi => cpgi.Cleaning)
					.FirstOrDefaultAsync(cp => cp.Id == request.CleaningPlanId);

				if(!request.OverrideAlreadySentPlan && cleaningPlan.IsSent)
				{
					return new ProcessResponse<SendCleaningPlanResponse>
					{
						Data = null,
						IsSuccess = false,
						HasError = true,
						Message = "Unable to send already sent cleaning plan without explicit override. To override send OverrideAlreadySentPlan request flag as true.",
					};
				}

				var allCleanings = await this._databaseContext.Cleanings.Include(c => c.CleaningPlanItems).Where(c => c.CleaningPlanId == cleaningPlan.Id).ToArrayAsync();
				var checkedCleaningIdsMap = new HashSet<Guid>();

				var cleaningPlanJson = Newtonsoft.Json.JsonConvert.SerializeObject(cleaningPlan, new Newtonsoft.Json.JsonSerializerSettings { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

				var localDateProvider = new HotelLocalDateProvider();
				dateTime = await localDateProvider.GetHotelCurrentLocalDate(this._databaseContext, cleaningPlan.HotelId, true);
				var isToday = dateTime.Date == cleaningPlan.Date.Date;
				sendCleaningNotifications = isToday;
				sendMobileNotificationsForTasks = isToday;

				var planFromDate = cleaningPlan.Date;
				var planToDate = planFromDate.AddDays(1);
				var tasksMap = await this._LoadTasksForPlannedAttendants(cleaningPlan.HotelId, planFromDate, planToDate);
				

				cleaningPlan.IsSent = true;
				cleaningPlan.SentAt = dateTime;
				cleaningPlan.SentById = this._userId;

				var cleaningsToInsert = new List<Cleaning>();
				var cleaningsToUpdate = new List<Cleaning>();

				foreach(var group in cleaningPlan.Groups)
				{
					foreach(var item in group.Items)
					{
						// Item is the currently planned cleaning and NOT the sent one.
						// Item.Cleaning is the sent one!

						//var messages = new List<string>();
						if(item.Cleaning == null)
						{
							// insert new cleaning
							var newCleaning = new Cleaning
							{
								Id = Guid.NewGuid(),
								CleanerId = group.CleanerId,
								CleaningPlanId = cleaningPlan.Id,
								CleaningPluginId = item.CleaningPluginId,
								Credits = item.Credits,
								Description = item.Description,
								EndsAt = item.EndsAt.Value,
								DurationSec = item.DurationSec.Value,
								IsActive = item.IsActive,
								InspectedById = null,
								IsChangeSheets = item.IsChangeSheets,
								IsCustom = item.IsCustom ,
								IsInspected = false,
								IsInspectionRequired = false, // TODO: LOAD PROPER CONFIGURATION FROM THE HOTEL SETTINGS?
								IsInspectionSuccess = false,
								IsPostponed = false,
								IsReadyForInspection = false,
								RoomBedId = item.RoomBedId,
								RoomId = item.RoomId,
								StartsAt = item.StartsAt.Value,
								Status = CleaningProcessStatus.NEW,
								IsPlanned = true,
								IsPriority = item.IsPriority,
								//ModifiedAt = dateTime,
							};
							cleaningsToInsert.Add(newCleaning);

							item.CleaningId = newCleaning.Id;
							item.Cleaning = newCleaning;
						}
						else
						{
							// update existing cleaning
							item.Cleaning.CleanerId = group.CleanerId;
							item.Cleaning.CleaningPluginId = item.CleaningPluginId;
							item.Cleaning.Credits = item.Credits;
							item.Cleaning.Description = item.Description;
							item.Cleaning.EndsAt = item.EndsAt.Value;
							item.Cleaning.DurationSec = item.DurationSec.Value;
							item.Cleaning.IsActive = item.IsActive;
							item.Cleaning.IsChangeSheets = item.IsChangeSheets;
							item.Cleaning.IsPostponed = item.IsPostponed;
							item.Cleaning.StartsAt = item.StartsAt.Value;
							item.Cleaning.IsPlanned = true;
							item.Cleaning.IsPriority = item.IsPriority;
							//item.Cleaning.ModifiedAt = dateTime;

							cleaningsToUpdate.Add(item.Cleaning);
							checkedCleaningIdsMap.Add(item.Cleaning.Id);
						}

						cleaningChangedEvents.Add(new CleaningChangedEventData
						{
							At = dateTime,
							IsDndRetry = false,
							HotelGroupId = this._hotelGroupId,
							RoomId = item.RoomId,
							Status = item.Cleaning.Status,
							UserId = group.CleanerId,
							CleaningPlanId = cleaningPlan.Id,
							CleaningId = item.Cleaning.Id,
							IsRemovedFromSentPlan = false,
							CleaningPlanItemId = item.Id,
						});

						if (tasksMap.ContainsKey(item.RoomId))
						{
							var tasks = tasksMap[item.RoomId];
							foreach(var task in tasks)
							{
								task.UserId = item.Cleaning.CleanerId;
								task.ModifiedAt = dateTime;
								task.ModifiedById = this._userId;

								if (task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
								taskIds.Add(task.Id);
							}
						}
					}
				}

				foreach(var cleaning in allCleanings)
				{
					if (checkedCleaningIdsMap.Contains(cleaning.Id)) continue;

					/// TODO: WARNING, CleaningPlanItemId WILL THROW EXCEPTION IF THERE ARE NO CLEANING PLANS. TEST AND FIX THIS ASAP!!
					/// TODO: WARNING, CleaningPlanItemId WILL THROW EXCEPTION IF THERE ARE NO CLEANING PLANS. TEST AND FIX THIS ASAP!!
					/// TODO: WARNING, CleaningPlanItemId WILL THROW EXCEPTION IF THERE ARE NO CLEANING PLANS. TEST AND FIX THIS ASAP!!
					/// TODO: WARNING, CleaningPlanItemId WILL THROW EXCEPTION IF THERE ARE NO CLEANING PLANS. TEST AND FIX THIS ASAP!!
					/// This will happen when the cleaning plugins change so the cleaning is no longer generated - the cleaning plan item doesn't exist any more.
					cleaning.IsPlanned = false;
					cleaningChangedEvents.Add(new CleaningChangedEventData
					{
						Status = CleaningProcessStatus.CLEANING_CANCELLED_BY_ADMIN,
						At = dateTime,
						BedId = cleaning.RoomBedId,
						CleaningId = cleaning.Id,
						CleaningPlanId = cleaningPlan.Id,
						HotelGroupId = this._hotelGroupId,
						IsDndRetry = false,
						IsRemovedFromSentPlan = true,
						RoomId = cleaning.RoomId,
						UserId = this._userId,
						CleaningPlanItemId = cleaning.CleaningPlanItems != null && cleaning.CleaningPlanItems.Any() ? cleaning.CleaningPlanItems.First().Id : Guid.Empty, 
					});
				}

				var sendingHistory = new CleaningPlanSendingHistory
				{
					Id = Guid.NewGuid(),
					SentById = this._userId,
					CleaningPlanId = cleaningPlan.Id,
					SentAt = dateTime,
					CleaningPlanJson = cleaningPlanJson,
				};

				if (cleaningsToInsert.Any())
				{
					await this._databaseContext.Cleanings.AddRangeAsync(cleaningsToInsert, cancellationToken);
				}
				await this._databaseContext.CleaningPlanSendingHistories.AddAsync(sendingHistory, cancellationToken);
				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			await this._eventService.CleaningsSent(this._hotelGroupId, cleaningChangedEvents, sendCleaningNotifications);
			if (taskIds.Any())
			{
				await this._eventService.TasksChanged(this._hotelGroupId, userIds, taskIds, "You have new tasks.", sendMobileNotificationsForTasks);
			}

			response = new ProcessResponse<SendCleaningPlanResponse>
			{
				Data = new SendCleaningPlanResponse
				{
					SentAt = dateTime
				},
				HasError = false,
				IsSuccess = true,
				Message = "Cleaning plan sent.",
			};

			return response;
		}


		private async Task<Dictionary<Guid, List<SystemTask>>> _LoadTasksForPlannedAttendants(string hotelId, DateTime planFromDate, DateTime planToDate)
		{
			var tasks = (await this._databaseContext
				.SystemTasks
				.Include(st => st.User)
				.Include(st => st.Actions)
				.Where(t =>
					(t.FromHotelId == hotelId || t.ToHotelId == hotelId) &&
					t.IsForPlannedAttendant &&
					planFromDate <= t.StartsAt &&
					planToDate >= t.StartsAt &&
					t.StatusKey != TaskStatusType.CANCELLED.ToString()
				).ToArrayAsync());

			var tasksMap = new Dictionary<Guid, List<SystemTask>>();

			foreach(var task in tasks)
			{
				if (task.FromRoomId.HasValue)
				{
					if (!tasksMap.ContainsKey(task.FromRoomId.Value))
					{
						tasksMap.Add(task.FromRoomId.Value, new List<SystemTask>());
					}
					tasksMap[task.FromRoomId.Value].Add(task);
				}

				if (task.ToRoomId.HasValue)
				{
					if (!tasksMap.ContainsKey(task.ToRoomId.Value))
					{
						tasksMap.Add(task.ToRoomId.Value, new List<SystemTask>());
					}
					tasksMap[task.ToRoomId.Value].Add(task);
				}
			}

			return tasksMap;
		}
	}
}
