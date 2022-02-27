using System.Collections.Generic;

namespace Planner.RccSynchronization.Contracts
{
	public class RccReservationsSnapshot
	{
		public IEnumerable<RccReservation> Reservations { get; set; }
		public IEnumerable<string> OutOfServiceRoomNames { get; set; }
		public IEnumerable<RccProduct> Products { get; set; }
	}
}
