using Microsoft.AspNetCore.SignalR;
using Planner.Application.Infrastructure.Signalr.ClientDefinitions;
using Planner.Application.Infrastructure.Signalr.Hubs;
using Planner.Application.Infrastructure.Signalr.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.Services
{
	public class RoomsOverviewSignalrService
	{
		private readonly IHubContext<RoomsOverviewHub, IRoomsOverviewClientMethods> _roomsOverviewHub;

		public RoomsOverviewSignalrService(IHubContext<RoomsOverviewHub, IRoomsOverviewClientMethods> roomsOverviewHub)
		{
			this._roomsOverviewHub = roomsOverviewHub;
		}

		public async Task SendRefreshRoomsOverviewDashboardMessage(Guid hotelGroupId, IEnumerable<Guid> roomIds)
		{
			var message = new RealTimeRefreshRoomsOverviewDashboardMessage
			{
				RoomIds = roomIds
			};

			await this._roomsOverviewHub
				.Clients
				.Group(hotelGroupId.ToString())
				.RefreshRoomsOverviewDashboard(message);
				//.SendAsync(nameof(IRoomsOverviewClientMethods.RefreshRoomsOverviewDashboard), message);
		}

		public async Task SendMessagesChangedMessage(Guid hotelGroupId, IEnumerable<Guid> roomIds, IEnumerable<string> reservationIds)
		{
			var message = new RealTimeMessagesChangedMessage
			{
				RoomIds = roomIds,
				ReservationIds = reservationIds,
			};

			await this._roomsOverviewHub
				.Clients
				.Group(hotelGroupId.ToString())
				.MessagesChanged(message);
		}

		//public async Task SendWorkerStatusChangedMessage(Guid hotelGroupId, string hotelId, Guid roomId, Guid userId, string statusDescription, bool isOnDuty)
		//{
		//	var messages = new RealTimeRoomsOverviewWorkerStatusChangedMessage[]
		//	{
		//		new RealTimeRoomsOverviewWorkerStatusChangedMessage { HotelId = hotelId, RoomId = roomId, UserId = userId, DutyDescription = statusDescription, IsOnDuty = isOnDuty },
		//	};

		//	await this._roomsOverviewHub
		//		.Clients
		//		.Group(hotelGroupId.ToString())
		//		.SendAsync(nameof(IRoomsOverviewClientMethods.ReceiveWorkerStatusChanges), messages);
		//}

		//public async Task SendRoomStatusChangedMessage(Guid hotelGroupId, string hotelId, Guid roomId, Guid? userId, bool isClean, bool isOccupied)
		//{
		//	var messages = new RealTimeRoomsOverviewRoomStatusChangedMessage[]
		//	{
		//		new RealTimeRoomsOverviewRoomStatusChangedMessage
		//		{
		//			HotelId = hotelId,
		//			IsClean = isClean,
		//			IsOccupied = isOccupied,
		//			RoomId = roomId
		//		}
		//	};

		//	await this._roomsOverviewHub
		//		.Clients
		//		.Group(hotelGroupId.ToString())
		//		.SendAsync(nameof(IRoomsOverviewClientMethods.ReceiveRoomStatusChanges), messages);
		//}

		//public async Task SendDashboardWorkerItemsChangesMessage(Guid hotelGroupId, RoomViewDashboardWorkerItem[] items, DateTime at)
		//{
		//	var message = new RealTimeDashboardWorkerItemsChangedMessage
		//	{
		//		Items = items,
		//		At = at
		//	};

		//	await this._roomsOverviewHub
		//		.Clients
		//		.Group(hotelGroupId.ToString())
		//		.SendAsync(nameof(IRoomsOverviewClientMethods.ReceiveDashboardWorkerItemsChanges), message);
		//}

	}


}
