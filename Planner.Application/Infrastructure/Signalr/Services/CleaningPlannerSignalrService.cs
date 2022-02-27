using Microsoft.AspNetCore.SignalR;
using Planner.Application.Infrastructure.Signalr.ClientDefinitions;
using Planner.Application.Infrastructure.Signalr.Hubs;
using Planner.Application.Infrastructure.Signalr.Messages;
using System;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.Services
{
	public class CleaningPlannerSignalrService
	{
		private readonly IHubContext<CleaningPlannerHub> _cleaningPlannerHub;

		public CleaningPlannerSignalrService(IHubContext<CleaningPlannerHub> cleaningPlannerHub)
		{
			this._cleaningPlannerHub = cleaningPlannerHub;
		}

		public async Task SendCleaningsChangedMessage(Guid hotelGroupId, RealTimeCleaningPlannerCleaningChangedMessage[] messages)
		{
			await this._cleaningPlannerHub
				.Clients
				.Group(hotelGroupId.ToString())
				.SendAsync(nameof(ICleaningPlannerClientMethods.ReceiveCleaningsChanged), messages);
		}
	}
}
