using System.Collections.Generic;

namespace Planner.RccSynchronization.Contracts
{
	public class RccPmsEventsSnapshot
	{
		public IEnumerable<RccPmsEvent> Events { get; set; }
	}
}
