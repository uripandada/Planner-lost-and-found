using Planner.Application.Infrastructure.Signalr.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.ClientDefinitions
{
	public interface IRoomsOverviewClientMethods
	{
		//Task ReceiveRoomStatusChanges(RealTimeRoomsOverviewRoomStatusChangedMessage[] message);
		//Task ReceiveWorkerStatusChanges(RealTimeRoomsOverviewWorkerStatusChangedMessage[] message);
		//Task ReceiveDashboardWorkerItemsChanges(RealTimeDashboardWorkerItemsChangedMessage message);
		Task RefreshRoomsOverviewDashboard(RealTimeRefreshRoomsOverviewDashboardMessage message);
		Task MessagesChanged(RealTimeMessagesChangedMessage message);
	}
}
