using Planner.Application.Infrastructure.Signalr.Messages;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.ClientDefinitions
{
	public interface ITaskClientMethods
	{
		Task ReceiveTasksChanged(RealTimeTasksChangedMessage messages);
	}
}
