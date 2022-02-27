using Microsoft.Extensions.Options;
using Planner.Common.AppSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.PusherClient
{
	public enum PusherChannelEventType
	{
		CLEANINGS_SENT,
		TASKS_CHANGED,
		HOTEL_ROOM_UPDATED,
		PRIORITY_CHANGED,
		GUEST_IN_ROOM_CHANGED,
		ROOM_MESSAGES_CHANGED,
	}

	public interface IPusherChannelsClient
	{
		Task SendHotelGroupNotification(Guid hotelGroupId, PusherChannelEventType eventType, object data = null);
	}

	public class PusherChannelsClient : IPusherChannelsClient
	{
		private readonly PusherServer.Pusher PusherServer;

		public PusherChannelsClient(IOptions<PusherChannelsSettings> channelsSettings)
		{
			this.PusherServer = new PusherServer.Pusher(
				channelsSettings.Value.AppId,
				channelsSettings.Value.Key,
				channelsSettings.Value.Secret,
				new PusherServer.PusherOptions { Cluster = channelsSettings.Value.Cluster }
			);
		}

		public async Task SendHotelGroupNotification(Guid hotelGroupId, PusherChannelEventType eventType, object data = null)
		{
			var eventNames = this._GetMobileEventName(eventType);
			foreach(var eventName in eventNames)
			{
				await this.PusherServer.TriggerAsync(hotelGroupId.ToString(), eventName, data);
			}
			return;
		}

		private IEnumerable<string> _GetMobileEventName(PusherChannelEventType type)
		{
			switch (type)
			{
				case PusherChannelEventType.CLEANINGS_SENT:
					return new string[] { "attendant_planning" };
				case PusherChannelEventType.TASKS_CHANGED:
					return new string[] { "hotel_task" };
				case PusherChannelEventType.HOTEL_ROOM_UPDATED:
					return new string[] { "hotel_room_update" };
				case PusherChannelEventType.PRIORITY_CHANGED:
					return new string[] { "attendant_planning" };
				case PusherChannelEventType.GUEST_IN_ROOM_CHANGED:
					return new string[] { "hotel_calendar", "hotel_room_update" };
				case PusherChannelEventType.ROOM_MESSAGES_CHANGED:
					return new string[] { "room_message" };
			}

			return new string[0];
		}
	}
}
