using Planner.Application.Infrastructure.Signalr.Messages;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.ClientDefinitions
{
	public interface IUserClientMethods
	{
		Task ReceiveUserOnDutyChanged(RealTimeUserOnDutyChangedMessage[] messages);
	}
}
