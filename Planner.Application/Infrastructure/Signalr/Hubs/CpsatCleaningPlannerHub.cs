using Microsoft.AspNetCore.SignalR;
using Planner.Application.Infrastructure.Signalr.ClientDefinitions;
using Planner.Common.Extensions;
using System;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.Hubs
{
	public class CpsatCleaningPlannerHub : Hub<ICpsatCleaningPlannerClientMethods>
	{
		public override async Task OnConnectedAsync()
		{
			var userId = this.Context.User.Id();

			await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var userId = this.Context.User.Id();

			await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());
			await base.OnDisconnectedAsync(exception);
		}
	}
}
