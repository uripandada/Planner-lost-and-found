using MediatR;
using Microsoft.AspNetCore.SignalR;
using Planner.Application.Infrastructure.PusherClient;
using Planner.Application.Infrastructure.Signalr.ClientDefinitions;
using Planner.Application.Infrastructure.Signalr.Hubs;
using Planner.Application.Infrastructure.Signalr.Messages;
using Planner.Application.Infrastructure.Signalr.Services;
using Planner.Application.Interfaces;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure
{
	public interface ISystemEventsService
	{

		Task UserCameOnDuty(Guid hotelGroupId, Guid userId, DateTime at);

		Task UserCameOffDuty(Guid hotelGroupId, Guid userId, DateTime at);

		Task GuestCurrentlyInRoomChanged(Guid hotelGroupId, Guid roomId, Guid? bedId, DateTime at, Guid userId, bool isInRoom);
		Task RoomCleaningPriorityChanged(Guid hotelGroupId, Guid roomId, Guid? bedId, DateTime at, Guid userId, bool isPriority);

		/// <summary>
		/// Called when the cleaning plan is sent to the clients.
		/// </summary>
		Task CleaningsSent(Guid hotelGroupId, IEnumerable<CleaningChangedEventData> data, bool sendNotifications = true);

		/// <summary>
		/// Called when the cleaner starts the cleaning.
		/// </summary>
		Task CleaningStarted(CleaningChangedEventData data);

		/// <summary>
		/// Called when the cleaner restarts the cleaning. I ma not sure what does restart really mean? After failing inspection? Or just starting the finished cleaning on demand?
		/// </summary>
		Task CleaningRestarted(CleaningChangedEventData data);

		/// <summary>
		/// Called when the cleaner pauses the cleaning.
		/// </summary>
		Task CleaningPaused(CleaningChangedEventData data);

		/// <summary>
		/// Called when the cleaner unpauses the cleaning.
		/// </summary>
		Task CleaningUnpaused(CleaningChangedEventData data);

		/// <summary>
		/// Called when the cleaner finishes the cleaning.
		/// </summary>
		Task CleaningFinished(CleaningChangedEventData data);

		/// <summary>
		/// Called when the cleaning is set to new - cancelled refuse, cancelled dnd, cancelled finish.
		/// </summary>
		Task CleaningNew(CleaningChangedEventData data);

		Task CleaningInspectionFinished(CleaningChangedEventData cleaningChangedEventData, string message);
		Task CleaningRefused(CleaningChangedEventData cleaningChangedEventData);
		Task CleaningDoNotDisturb(CleaningChangedEventData cleaningChangedEventData);
		Task CleaningDelayed(CleaningChangedEventData cleaningChangedEventData);


		Task CleaningInspectionDoNotDisturb(CleaningChangedEventData cleaningChangedEventData, string message);
		Task CleaningInspectionRequiresInspection(CleaningChangedEventData cleaningChangedEventData, string message);
		Task CleaningInspectionCleaningInProgress(CleaningChangedEventData cleaningChangedEventData, string message);
		Task CleaningInspectionNewCleaning(CleaningChangedEventData cleaningChangedEventData, string message);


		Task TasksChanged(Guid hotelGroupId, IEnumerable<Guid> userIds, IEnumerable<Guid> taskIds, string message, bool sendMobileNotifications = true);


		Task CleaningInspectionChanged(Guid hotelGroupId, Guid userId, Guid roomId, Guid? bedId, Guid cleaningId, Guid cleaningPlanId, Guid cleaningPlanItemId, DateTime at, string message, CleaningProcessStatus status);

		/// <summary>
		///
		/// </summary>
		/// <param name="hotelGroupId">For possible pusher beam notifications</param>
		/// <param name="roomIds">For SignalR notifications</param>
		/// <param name="reservationIds">For SignalR notifications</param>
		/// <param name="userIds">For pusher channel notifications</param>
		/// <returns></returns>
		Task RoomMessagesChanged(Guid hotelGroupId, IEnumerable<Guid> roomIds, IEnumerable<string> reservationIds, IEnumerable<Guid> userIds);
	}

	public class CleaningChangedEventData
	{
		public Guid HotelGroupId { get; set; }
		public Guid RoomId { get; set; }
		public Guid? BedId { get; set; }
		public Guid CleaningPlanId { get; set; }
		public Guid CleaningPlanItemId { get; set; }
		public Guid CleaningId { get; set; }
		public Guid UserId { get; set; }
		public DateTime At { get; set; }
		public CleaningProcessStatus Status { get; set; }

		/// <summary>
		/// This flag is set to true when the mobile attendant app does more than 1 dnd in a row for a room.
		/// </summary>
		public bool IsDndRetry { get; set; }
		/// <summary>
		/// This flag is set to true when the cleaning plan is resent and the clenaing is not planned any more.
		/// </summary>
		public bool IsRemovedFromSentPlan { get; set; }
	}
	/// <summary>
	/// Inserts events into the system history and raises notifications.
	/// </summary>
	public class SystemEventsService : ISystemEventsService
	{
		private readonly RoomsOverviewSignalrService _roomsOverviewSignalrService;
		private readonly CleaningPlannerSignalrService _cleaningPlannerSignalrService;
		private readonly UserSignalrService _userSignalrService;
		private readonly TasksSignalrService _taskSignalrService;
		private readonly CpsatCleaningPlannerSignalrService _cpsatCleaningPlannerSignalrService;
		private readonly IDatabaseContext _databaseContext;

		private readonly IPusherBeamsClient _pusherBeamsClient;
		private readonly IPusherChannelsClient _pusherChannelsClient;

		public SystemEventsService(
			IDatabaseContext databaseContext,
			IHubContext<RoomsOverviewHub, IRoomsOverviewClientMethods> roomsOverviewHub,
			IHubContext<CleaningPlannerHub> cleaningPlannerHub,
			IHubContext<UserHub> userHub,
			IHubContext<TaskHub> taskHub,
			IHubContext<CpsatCleaningPlannerHub, ICpsatCleaningPlannerClientMethods> cpsatCleaningPlannerHub,
			IPusherBeamsClient pusherClient, 
			IPusherChannelsClient pusherChannelsClient)
		{
			this._roomsOverviewSignalrService = new RoomsOverviewSignalrService(roomsOverviewHub);
			this._cleaningPlannerSignalrService = new CleaningPlannerSignalrService(cleaningPlannerHub);
			this._userSignalrService = new UserSignalrService(userHub);
			this._taskSignalrService = new TasksSignalrService(taskHub);
			this._cpsatCleaningPlannerSignalrService = new CpsatCleaningPlannerSignalrService(cpsatCleaningPlannerHub);
			this._databaseContext = databaseContext;
			this._pusherBeamsClient = pusherClient;
			this._pusherChannelsClient = pusherChannelsClient;
		}

		public async Task UserCameOnDuty(Guid hotelGroupId, Guid userId, DateTime at)
		{
			await this._SaveUserEventHistory(UserEventType.CAME_ON_DUTY, "Came on duty.", userId, at);
			await this._databaseContext.SaveChangesAsync(CancellationToken.None);

			await this._userSignalrService.SendOnDutyChangedMessage(hotelGroupId, userId, true);
		}

		public async Task UserCameOffDuty(Guid hotelGroupId, Guid userId, DateTime at)
		{
			await this._SaveUserEventHistory(UserEventType.CAME_ON_DUTY, "Came off duty.", userId, at);
			await this._databaseContext.SaveChangesAsync(CancellationToken.None);

			await this._userSignalrService.SendOnDutyChangedMessage(hotelGroupId, userId, false);
		}

		/// <summary>
		/// Called when the cleaning plan is sent to the clients.
		/// </summary>
		public async Task CleaningsSent(Guid hotelGroupId, IEnumerable<CleaningChangedEventData> data, bool sendNotifications = true)
		{
			if (!data.Any())
			{
				return;
			}

			var plannedCleaningEvents = new List<CleaningChangedEventData>();
			var unplannedCleaningEvents = new List<CleaningChangedEventData>();
			foreach(var e in data)
			{
				if (e.IsRemovedFromSentPlan) unplannedCleaningEvents.Add(e);
				else plannedCleaningEvents.Add(e);
			}

			if (plannedCleaningEvents.Any())
			{
				await this._SaveCleaningEventsHistory(CleaningEventType.SENT_TO_CLIENT, "Cleaning created.", plannedCleaningEvents);
				await this._SaveRoomEventsHistory(RoomEventType.CLEANING_CREATED, "Cleaning created.", plannedCleaningEvents);
			}

			if (unplannedCleaningEvents.Any())
			{ 
				await this._SaveCleaningEventsHistory(CleaningEventType.SENT_TO_CLIENT, "Cleaning removed from plan.", unplannedCleaningEvents); 
				await this._SaveRoomEventsHistory(RoomEventType.CLEANING_CANCELLED_BY_ADMIN, "Cleaning removed from plan.", unplannedCleaningEvents);
			}

			await this._databaseContext.SaveChangesAsync(CancellationToken.None);

			if (sendNotifications)
			{
				await this._SendCleaningEventNotifications(hotelGroupId, data);
				await this._PushBeamCleaningEventNotifications(data, "Cleanings changed", "Cleanings changed.");
				await this._PushChannelCleaningsSentNotifications(hotelGroupId);
			}
		}

		public async Task TasksChanged(Guid hotelGroupId, IEnumerable<Guid> userIds, IEnumerable<Guid> taskIds, string message, bool sendMobileNotifications = true)
		{
			if (sendMobileNotifications)
			{
				await this._pusherChannelsClient.SendHotelGroupNotification(hotelGroupId, PusherChannelEventType.TASKS_CHANGED);

				if (userIds.Any())
				{
					await this._pusherBeamsClient.SendPushNotification(userIds, "Tasks changed", message);
				}
			}

			if (taskIds.Any())
			{
				await this._taskSignalrService.TasksChanged(hotelGroupId, taskIds);
			}
		}

		private async Task _PushChannelCleaningsSentNotifications(Guid hotelGroupId)
		{
			await this._pusherChannelsClient.SendHotelGroupNotification(hotelGroupId, PusherChannelEventType.CLEANINGS_SENT);
		}
		
		private async Task _PushChannelHotelRoomUpdatedNotification(Guid hotelGroupId)
		{
			await this._pusherChannelsClient.SendHotelGroupNotification(hotelGroupId, PusherChannelEventType.HOTEL_ROOM_UPDATED);
		}

		private async Task _PushBeamCleaningEventNotifications(IEnumerable<CleaningChangedEventData> data, string title, string message)
		{
			var userIds = data.Select(d => d.UserId).Distinct().ToArray();

			if (!userIds.Any())
			{
				return;
			}

			await this._pusherBeamsClient.SendPushNotification(userIds, title, message);
		}

		/// <summary>
		/// Called when the cleaner starts the cleaning.
		/// </summary>
		public async Task CleaningStarted(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, "Cleaning started.", UserEventType.CLEANING_STARTED, RoomEventType.CLEANING_STARTED, CleaningEventType.STARTED);
		}

		/// <summary>
		/// Called when the cleaner restarts the cleaning. I ma not sure what does restart really mean? After failing inspection? Or just starting the finished cleaning on demand?
		/// </summary>
		public async Task CleaningRestarted(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, "Cleaning restarted.", UserEventType.CLEANING_RESTARTED, RoomEventType.CLEANING_RESTARTED, CleaningEventType.RESTARTED);
		}

		/// <summary>
		/// Called when the cleaner pauses the cleaning.
		/// </summary>
		public async Task CleaningPaused(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, "Cleaning paused.", UserEventType.CLEANING_PAUSED, RoomEventType.CLEANING_PAUSED, CleaningEventType.PAUSED);
		}

		/// <summary>
		/// Called when the cleaner unpauses the cleaning.
		/// </summary>
		public async Task CleaningUnpaused(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, "Cleaning unpaused.", UserEventType.CLEANING_UNPAUSED, RoomEventType.CLEANING_UNPAUSED, CleaningEventType.UNPAUSED);
		}

		/// <summary>
		/// Called when the cleaner finishes the cleaning.
		/// </summary>
		public async Task CleaningFinished(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, "Cleaning finished.", UserEventType.CLEANING_FINISHED, RoomEventType.CLEANING_FINISHED, CleaningEventType.FINISHED);
		}

		public async Task CleaningInspectionFinished(CleaningChangedEventData data, string message)
		{
			var msg = string.IsNullOrWhiteSpace(message) ? "Cleaning inspection finished." : message;

			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, message, UserEventType.CLEANING_INSPECTION_FINISHED, RoomEventType.CLEANING_INSPECTION_FINISHED, CleaningEventType.INSPECTION_FINISHED);
		}

		public async Task CleaningRefused(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, "Cleaning refused.", UserEventType.CLEANING_INSPECTION_FINISHED, RoomEventType.CLEANING_INSPECTION_FINISHED, CleaningEventType.INSPECTION_FINISHED);

		}

		public async Task CleaningDoNotDisturb(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, data.IsDndRetry ? "Cleaning retry DND." : "Cleaning DND.", UserEventType.CLEANING_DO_NOT_DISTURB, RoomEventType.CLEANING_DO_NOT_DISTURB, CleaningEventType.DO_NOT_DISTURB);
		}
		
		public async Task CleaningDelayed(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, "Cleaning delayed.", UserEventType.CLEANING_DELAYED, RoomEventType.CLEANING_DELAYED, CleaningEventType.DELAYED);
		}

		public async Task CleaningNew(CleaningChangedEventData data)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, "Cleaning new.", UserEventType.CLEANING_NEW, RoomEventType.CLEANING_NEW, CleaningEventType.NEW);
		}

		private async Task _CleaningProgressChanged(Guid hotelGroupId, CleaningChangedEventData[] data, string message, UserEventType userEventType, RoomEventType roomEventType, CleaningEventType cleaningEventType)
		{
			await this._SaveUserEventsHistory(userEventType, message, data);
			await this._SaveRoomEventsHistory(roomEventType, message, data);
			await this._SaveCleaningEventsHistory(cleaningEventType, message, data);
			await this._databaseContext.SaveChangesAsync(CancellationToken.None);


			await this._SendRefreshRoomsOverviewDashboardNotifications(hotelGroupId, data.Select(d => d.RoomId).ToArray());
			await this._SendCleaningEventNotifications(hotelGroupId, data);
			await this._PushChannelHotelRoomUpdatedNotification(hotelGroupId);
		}

		private async Task _SaveCleaningEventsHistory(CleaningEventType type, string message, IEnumerable<CleaningChangedEventData> data)
		{
			var cleaningEvents = data.Select(d =>
			{
				return new CleaningHistoryEvent
				{
					Id = Guid.NewGuid(),
					CleaningId = d.CleaningId,
					RoomId = d.RoomId,
					At = d.At,
					Message = message,
					Type = type,
					UserId = d.UserId,
					Status = d.Status,
				};
			}).ToArray();

			await this._databaseContext.CleaningHistoryEvents.AddRangeAsync(cleaningEvents);
		}

		private async Task _SaveRoomEventsHistory(RoomEventType type, string message, IEnumerable<CleaningChangedEventData> data)
		{
			var cleaningEvents = data.Select(d =>
			{
				return new RoomHistoryEvent
				{
					Id = Guid.NewGuid(),
					RoomId = d.RoomId,
					RoomBedId = d.BedId,
					At = d.At,
					Message = message,
					Type = type,
					UserId = d.UserId,
				};
			}).ToArray();

			await this._databaseContext.RoomHistoryEvents.AddRangeAsync(cleaningEvents);
		}
		private async Task _SaveRoomEventHistory(RoomEventType type, string message, Guid roomId, Guid? bedId, DateTime at, Guid userId)
		{
			var cleaningEvent = 
				new RoomHistoryEvent
				{
					Id = Guid.NewGuid(),
					RoomId = roomId,
					RoomBedId = bedId,
					At = at,
					Message = message,
					Type = type,
					UserId = userId,
			};

			await this._databaseContext.RoomHistoryEvents.AddAsync(cleaningEvent);
		}

		private async Task _SaveUserEventsHistory(UserEventType type, string message, IEnumerable<CleaningChangedEventData> data)
		{
			var cleaningEvents = data.Select(d =>
			{
				return new UserHistoryEvent
				{
					Id = Guid.NewGuid(),
					At = d.At,
					Message = message,
					Type = type,
					UserId = d.UserId,
				};
			}).ToArray();

			await this._databaseContext.UserHistoryEvents.AddRangeAsync(cleaningEvents);
		}
		
		private async Task _SaveUserEventHistory(UserEventType type, string message, DateTime at, Guid userId)
		{
			var cleaningEvent = new UserHistoryEvent
				{
					Id = Guid.NewGuid(),
					At = at,
					Message = message,
					Type = type,
					UserId = userId,
				};

			await this._databaseContext.UserHistoryEvents.AddAsync(cleaningEvent);
		}
		
		private async Task _SaveUserEventHistory(UserEventType type, string message, Guid userId, DateTime at)
		{
			var cleaningEvent = new UserHistoryEvent
			{
				Id = Guid.NewGuid(),
				At = at,
				Message = message,
				Type = type,
				UserId = userId,
			};

			await this._databaseContext.UserHistoryEvents.AddAsync(cleaningEvent);
		}

		private async Task _SendCleaningEventNotifications(Guid hotelGroupId, IEnumerable<CleaningChangedEventData> data)
		{
			var cleaningChanges = data.Select(d =>
			{
				return new RealTimeCleaningPlannerCleaningChangedMessage
				{
					RoomId = d.RoomId,
					At = d.At,
					CleaningPlanItemId = d.CleaningPlanItemId,
					CleaningProcessStatus = d.Status,
					CleaningPlanId = d.CleaningPlanId,
					CleaningId = d.CleaningId,
				};
			}).ToArray();

			await this._cleaningPlannerSignalrService.SendCleaningsChangedMessage(hotelGroupId, cleaningChanges);
		}

		private async Task _SendRefreshRoomsOverviewDashboardNotifications(Guid hotelGroupId, Guid[] roomIds)
		{
			await this._roomsOverviewSignalrService.SendRefreshRoomsOverviewDashboardMessage(hotelGroupId, roomIds);
		}

		public async Task GuestCurrentlyInRoomChanged(Guid hotelGroupId, Guid roomId, Guid? bedId, DateTime at, Guid userId, bool isInRoom)
		{
			await this._SaveRoomEventHistory(isInRoom ? RoomEventType.GUEST_ENTERED_ROOM : RoomEventType.GUEST_LEFT_ROOM, isInRoom ? "Guest locator changed to Occupied." : "Guest locator changed to Vacant.", roomId, bedId, at, userId);
			await this._SaveUserEventHistory(isInRoom ? UserEventType.REGISTERED_GUEST_ENTERED_ROOM : UserEventType.REGISTERED_GUEST_LEFT_ROOM, isInRoom ? "Changed guest locator to Occupied." : "Changed guest locator to Vacant.", at, userId);
			await this._databaseContext.SaveChangesAsync(CancellationToken.None);

			await this._pusherChannelsClient.SendHotelGroupNotification(hotelGroupId, PusherChannelEventType.GUEST_IN_ROOM_CHANGED);
		}
		
		public async Task RoomCleaningPriorityChanged(Guid hotelGroupId, Guid roomId, Guid? bedId, DateTime at, Guid userId, bool isPriority)
		{
			await this._SaveRoomEventHistory(isPriority ? RoomEventType.CLEANING_PRIORITY_ENABLED : RoomEventType.CLEANING_PRIORITY_DISABLED, isPriority ? "Cleaning priority." : "Cleaning priority removed.", roomId, bedId, at, userId);
			await this._SaveUserEventHistory(isPriority ? UserEventType.ROOM_CLEANING_PRIORITY_ENABLED : UserEventType.ROOM_CLEANING_PRIORITY_DISABLED, isPriority ? "Enabled room cleaning priority." : "Disabled room cleaning priority.", at, userId);
			await this._databaseContext.SaveChangesAsync(CancellationToken.None);

			await this._pusherChannelsClient.SendHotelGroupNotification(hotelGroupId, PusherChannelEventType.PRIORITY_CHANGED);
		}

		public async Task CleaningInspectionDoNotDisturb(CleaningChangedEventData data, string message)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, message, UserEventType.CLEANING_INSPECTION_DO_NOT_DISTURB, RoomEventType.CLEANING_INSPECTION_DO_NOT_DISTURB, CleaningEventType.INSPECTION_DO_NOT_DISTURB);
		}

		public async Task CleaningInspectionRequiresInspection(CleaningChangedEventData data, string message)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, message, UserEventType.CLEANING_INSPECTION_REQUIRES_INSPECTION, RoomEventType.CLEANING_INSPECTION_REQUIRES_INSPECTION, CleaningEventType.INSPECTION_REQUIRES_INSPECTION);
		}

		public async Task CleaningInspectionCleaningInProgress(CleaningChangedEventData data, string message)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, message, UserEventType.CLEANING_INSPECTION_CLEANING_IN_PROGRESS, RoomEventType.CLEANING_INSPECTION_CLEANING_IN_PROGRESS, CleaningEventType.INSPECTION_CLEANING_IN_PROGRESS);
		}

		public async Task CleaningInspectionNewCleaning(CleaningChangedEventData data, string message)
		{
			await this._CleaningProgressChanged(data.HotelGroupId, new CleaningChangedEventData[] { data }, message, UserEventType.CLEANING_INSPECTION_NEW_CLEANING, RoomEventType.CLEANING_INSPECTION_NEW_CLEANING, CleaningEventType.INSPECTION_NEW_CLEANING);
		}

		public async Task CleaningInspectionChanged(Guid hotelGroupId, Guid userId, Guid roomId, Guid? bedId, Guid cleaningId, Guid cleaningPlanId, Guid cleaningPlanItemId, DateTime at, string message, CleaningProcessStatus status)
		{
			var data = new CleaningChangedEventData[] 
			{ 
				new CleaningChangedEventData
				{
					At = at,
					UserId = userId,
					RoomId = roomId,
					CleaningId = cleaningId,
					BedId = bedId,
					HotelGroupId = hotelGroupId,
					Status = status,
					CleaningPlanId = cleaningPlanId,
					CleaningPlanItemId = cleaningPlanItemId,
				} 
			};

			await this._SaveUserEventsHistory(UserEventType.CLEANING_INSPECTION_CHANGED, message, data);
			await this._SaveRoomEventsHistory(RoomEventType.CLEANING_INSPECTION_CHANGED, message, data);
			await this._SaveCleaningEventsHistory(CleaningEventType.INSPECTION_CHANGED, message, data);
			await this._databaseContext.SaveChangesAsync(CancellationToken.None);
		}

		public async Task RoomMessagesChanged(Guid hotelGroupId, IEnumerable<Guid> roomIds, IEnumerable<string> reservationIds, IEnumerable<Guid> userIds)
		{
			await this._pusherChannelsClient.SendHotelGroupNotification(hotelGroupId, PusherChannelEventType.ROOM_MESSAGES_CHANGED);

			if (userIds.Any())
			{
				await this._pusherBeamsClient.SendPushNotification(userIds, "Messages changed", "");
			}
			if (roomIds.Any() || reservationIds.Any())
			{
				await this._roomsOverviewSignalrService.SendMessagesChangedMessage(hotelGroupId, roomIds, reservationIds);
			}
		}
	}
}
