using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Tasks.Commands.InsertTaskForMobile
{
	public class InsertTaskForMobileCommand: IRequest<ProcessResponse>
	{
		public string HotelId { get; set; }
		public IEnumerable<Guid> RoomIds { get; set; }
		public Guid AssetId { get; set; }
		public Guid ActionId { get; set; }
		public int? Quantity { get; set; }
		public int? Price { get; set; }
		public int? Credits { get; set; }
		public string Comment { get; set; }
		public bool? IsHighPriority { get; set; }
		public bool? IsGuestRequest { get; set; }
		public string ImageUrl { get; set; }
		public DateTime? StartsAt { get; set; }
		public IEnumerable<Guid> UserIds { get; set; }
		public IEnumerable<Guid> UserGroupIds { get; set; }
		public IEnumerable<Guid> UserSubGroupIds { get; set; }
	}

	public class InsertTaskForMobileCommandHandler : IRequestHandler<InsertTaskForMobileCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public InsertTaskForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemTaskGenerator = systemTaskGenerator;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}

		public async Task<ProcessResponse> Handle(InsertTaskForMobileCommand request, CancellationToken cancellationToken)
		{
			var hotel = (Domain.Entities.Hotel)null;

			if (request.HotelId.IsNotNull())
			{
				hotel = await this._databaseContext.Hotels
					.Where(aa => aa.Id == request.HotelId)
					.FirstOrDefaultAsync();

				if (hotel == null)
				{
					return new ProcessResponse
					{
						IsSuccess = false,
						HasError = true,
						Message = $"Unable to find hotel with id: {request.HotelId}",
					};
				}
			}
			else
			{
				return new ProcessResponse
				{
					IsSuccess = false,
					HasError = true,
					Message = "Hotel id is not set.",
				};
			}

			var rooms = await this._databaseContext.Rooms.Where(r => request.RoomIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id);

			var wheres = request.RoomIds.Select(roomId =>
			{
				return new SystemTaskConfigurationWhere
				{
					ReferenceId = roomId.ToString(),
					ReferenceName = rooms[roomId].Name,
					TypeDescription = "Room",
					TypeKey = Common.Enums.TaskWhereType.ROOM.ToString(),
				};
			}).ToArray();

			var whos = new List<SystemTaskConfigurationWho>();
			var usersMap = new Dictionary<Guid, Domain.Entities.User>();
			if (request.UserIds != null && request.UserIds.Any())
			{
				var users = await this._databaseContext.Users.Where(u => request.UserIds.Contains(u.Id)).ToListAsync();
				foreach(var user in users)
				{
					usersMap.Add(user.Id, user);
				}
			}

			if(request.UserGroupIds != null && request.UserGroupIds.Any())
			{
				var users = await this._databaseContext.Users.Where(u => u.UserGroupId != null && request.UserGroupIds.Contains(u.UserGroupId.Value)).ToListAsync();
				foreach (var user in users)
				{
					if(!usersMap.ContainsKey(user.Id)) usersMap.Add(user.Id, user);
				}
			}

			if(request.UserSubGroupIds != null && request.UserSubGroupIds.Any())
			{
				var users = await this._databaseContext.Users.Where(u => u.UserSubGroupId != null && request.UserSubGroupIds.Contains(u.UserSubGroupId.Value)).ToListAsync();
				foreach (var user in users)
				{
					if (!usersMap.ContainsKey(user.Id)) usersMap.Add(user.Id, user);
				}
			}

			var asset = await this._databaseContext.Assets.Include(a => a.AssetGroup).FirstOrDefaultAsync(a => a.Id == request.AssetId);
			var action = await this._databaseContext.AssetActions.FirstOrDefaultAsync(aa => aa.Id == request.ActionId);
			var whats = new List<SystemTaskConfigurationWhat>
			{
				new SystemTaskConfigurationWhat
				{
					AssetId = asset.Id,
					AssetName = asset.Name,
					ActionName = action.Name,
					AssetGroupId = asset.AssetGroup.Id,
					AssetGroupName = asset.AssetGroup.Name,
					AssetQuantity = request.Quantity ?? 1,
					IsActionSystemDefined = action.IsSystemDefined,
					SystemDefinedActionIdentifierKey = action.SystemDefinedActionIdentifierKey,
					SystemDefinedActionTypeKey = action.SystemActionTypeKey,
				} 
			};

			var timeZoneId = Infrastructure.HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			var localHotelDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

			var systemTaskConfiguration = new Domain.Entities.SystemTaskConfiguration
			{
				CreatedAt = localHotelDateTime,
				CreatedById = null,
				Id = Guid.NewGuid(),
				ModifiedAt = localHotelDateTime,
				ModifiedById = null,
				Data = new SystemTaskConfigurationData
				{
					Whos = whos,
					Files = new SystemTaskConfigurationFile[0],
					Price = request.Price ?? 0,
					Whats = whats,
					Wheres = wheres,
					StartsAtTimes = new DateTime[] { request.StartsAt ?? localHotelDateTime },
					EventModifierKey = null,
					EventTimeKey = null,
					ExcludeHolidays = null,
					ExcludeWeekends = null,
					IsBlockingCleaningUntilFinished = false,
					IsGuestRequest = request.IsGuestRequest ?? false,
					Comment = request.Comment,
					Credits = request.Credits ?? 0,
					EndsAtTime = null,
					EventKey = null,
					IsMajorNotificationRaisedWhenFinished = false,
					IsRescheduledEveryDayUntilFinished = false,
					IsShownInNewsFeed = false,
					MustBeFinishedByAllWhos = true,
					PostponeWhenRoomIsOccupied = null,
					PriorityKey = (request.IsHighPriority ?? false) ? "HIGH" : "NORMAL",
					RecurringEveryNumberOfDays = 0,
					RecurringTaskRepeatTimes = null,
					RecurringTaskTypeKey = "DAILY",
					RepeatsForKey = null,
					RepeatsForNrDays = null,
					RepeatsForNrOccurences = null,
					RepeatsUntilTime = null,
					TaskTypeKey = "SINGLE",
					WhatsTypeKey = "LIST",
					WhereFrom = null,
					WhereTo = null,
					FilestackImageUrls = request.ImageUrl.IsNotNull() ? new string[] { request.ImageUrl } : new string[0],
				},

			};

			var systemTasks = new List<SystemTask>();
			var systemTaskHistories = new List<SystemTaskHistory>();

			foreach(var user in usersMap.Values)
			{
				foreach(var room in rooms.Values)
				{
					var systemTaskId = Guid.NewGuid();
					var systemTask = new SystemTask
					{
						Id = systemTaskId,
						Comment = request.Comment,
						CreatedAt = localHotelDateTime,
						CreatedById = this._userId,
						ModifiedAt = localHotelDateTime,
						ModifiedById = this._userId,
						Actions = whats.Select(ta => new SystemTaskAction
						{
							SystemTaskId = systemTaskId,
							ActionName = ta.ActionName,
							AssetGroupId = ta.AssetGroupId,
							AssetGroupName = ta.AssetGroupName,
							AssetId = ta.AssetId,
							AssetName = ta.AssetName,
							AssetQuantity = ta.AssetQuantity,
							Id = Guid.NewGuid(),
						}).ToArray(),
						Credits = systemTaskConfiguration.Data.Credits,
						Price = systemTaskConfiguration.Data.Price,
						IsGuestRequest = systemTaskConfiguration.Data.IsGuestRequest,
						PriorityKey = systemTaskConfiguration.Data.PriorityKey,
						StartsAt = systemTaskConfiguration.Data.StartsAtTimes.First(),
						IsForPlannedAttendant = false,
						UserId = user.Id,

						SystemTaskConfigurationId = systemTaskConfiguration.Id,
						MustBeFinishedByAllWhos = true,

						EventKey = null,
						EventModifierKey = null,
						EventTimeKey = null,
						RecurringTypeKey = Common.Enums.RecurringTaskType.DAILY.ToString(),
						RepeatsForKey = null,
						StatusKey = Common.Enums.TaskStatusType.PENDING.ToString(),
						TypeKey = Common.Enums.TaskType.SINGLE.ToString(),

						WhereTypeKey = "TO",

						FromHotelId = null,
						FromName = null,
						FromReservationId = null,
						FromRoomId = null,
						FromWarehouseId = null,

						ToHotelId = room.HotelId,
						ToName = room.Name,
						ToReservationId = null,
						ToRoomId = room.Id,
						ToWarehouseId = null,

						IsBlockingCleaningUntilFinished = false,
						IsMajorNotificationRaisedWhenFinished = false,
						IsManuallyModified = true,
						IsRescheduledEveryDayUntilFinished = false,
						IsShownInNewsFeed = false,

					};


					var systemTaskHistory = new SystemTaskHistory
					{
						Id = Guid.NewGuid(),
						ChangedByKey = "MOBILE_API",
						CreatedAt = localHotelDateTime,
						CreatedById = null,
						SystemTaskId = systemTaskId,
						NewData = new SystemTaskHistoryData(),
						OldData = new SystemTaskHistoryData(),
						Message = "Task created by mobile API.",
					};

					systemTasks.Add(systemTask);
					systemTaskHistories.Add(systemTaskHistory);
				}
			}

			await this._databaseContext.SystemTaskConfigurations.AddAsync(systemTaskConfiguration);
			await this._databaseContext.SystemTasks.AddRangeAsync(systemTasks);
			await this._databaseContext.SystemTaskHistorys.AddRangeAsync(systemTaskHistories);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			var userIds = usersMap.Keys.ToArray();
			var systemTaskIds = systemTasks.Select(t => t.Id).ToArray();

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, systemTaskIds, "You have new tasks.");

			return new ProcessResponse
			{
				IsSuccess = true,
				HasError = false,
				Message = "Task created.",
			};
		}
	}
}
