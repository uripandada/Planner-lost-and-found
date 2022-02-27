using Microsoft.AspNetCore.SignalR;
using Planner.Application.Infrastructure.Signalr.ClientDefinitions;
using Planner.Application.Infrastructure.Signalr.Hubs;
using Planner.Application.Infrastructure.Signalr.Messages;
using System;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.Services
{
	public class UserSignalrService
	{
		private readonly IHubContext<UserHub> _userHub;

		public UserSignalrService(IHubContext<UserHub> userHub)
		{
			this._userHub = userHub;
		}

		public async Task SendOnDutyChangedMessage(Guid hotelGroupId, Guid userId, bool isOnDuty)
		{
			var messages = new RealTimeUserOnDutyChangedMessage[]
			{
				new RealTimeUserOnDutyChangedMessage { UserId = userId, IsOnDuty = isOnDuty }
			};

			await this._userHub
				.Clients
				.Group(hotelGroupId.ToString())
				.SendAsync(nameof(IUserClientMethods.ReceiveUserOnDutyChanged), messages);
		}
	}
}
