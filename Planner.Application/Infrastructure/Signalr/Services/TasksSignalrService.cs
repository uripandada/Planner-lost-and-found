using Microsoft.AspNetCore.SignalR;
using Planner.Application.Infrastructure.Signalr.ClientDefinitions;
using Planner.Application.Infrastructure.Signalr.Hubs;
using Planner.Application.Infrastructure.Signalr.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.Services
{
	public class TasksSignalrService
	{
		private readonly IHubContext<TaskHub> _taskHub;

		public TasksSignalrService(IHubContext<TaskHub> taskHub)
		{
			this._taskHub = taskHub;
		}

		public async Task TasksChanged(Guid hotelGroupId, IEnumerable<Guid> taskIds)
		{
			var message = new RealTimeTasksChangedMessage { TaskIds = taskIds };

			await this._taskHub
				.Clients
				.Group(hotelGroupId.ToString())
				.SendAsync(nameof(ITaskClientMethods.ReceiveTasksChanged), message);
		}
	}
}
