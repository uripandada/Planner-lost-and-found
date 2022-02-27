using Planner.Application.Infrastructure.Signalr.Messages;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.ClientDefinitions
{
	public interface ICpsatCleaningPlannerClientMethods
	{
		Task ReceiveCpsatCleaningPlanningProgressChanged(RealTimeCpsatCleaningPlanningProgressChangedMessage message);
		Task ReceiveCpsatCleaningPlanningFinished(RealTimeCpsatCleaningPlanningFinishedMessage message);
	}
}
