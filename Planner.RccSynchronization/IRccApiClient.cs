using Planner.RccSynchronization.Contracts;
using System.Threading.Tasks;

namespace Planner.RccSynchronization
{
	public interface IRccApiClient
	{
		Task<RccReservationsSnapshot> GetReservations(string hotelId);
		Task<RccPmsEventsSnapshot> GetPmsEvents(string hotelId);
	}
}
