using Microsoft.AspNetCore.SignalR;
using Planner.Application.Infrastructure.Signalr.ClientDefinitions;
using Planner.Common.Extensions;
using System;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.Hubs
{
	public class UserHub : Hub<IUserClientMethods>
	{
		public override async Task OnConnectedAsync()
		{
			var hotelGroupId = this.Context.User.HotelGroupId();
			var userId = this.Context.User.Id();

			await Groups.AddToGroupAsync(Context.ConnectionId, hotelGroupId.ToString());
			await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var hotelGroupId = this.Context.User.HotelGroupId();
			var userId = this.Context.User.Id();

			await Groups.RemoveFromGroupAsync(Context.ConnectionId, hotelGroupId.ToString());
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
			await base.OnDisconnectedAsync(exception);
		}
	}
}
