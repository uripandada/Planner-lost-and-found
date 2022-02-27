using Planner.Application.CleaningPlans.Commands.GenerateCpsatCleaningPlan;
using Planner.Application.Infrastructure.Signalr.Messages;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.ClientDefinitions
{
	public interface ICleaningPlannerClientMethods
	{
		Task ReceiveCleaningsChanged(RealTimeCleaningPlannerCleaningChangedMessage[] messages);


	}   
}
