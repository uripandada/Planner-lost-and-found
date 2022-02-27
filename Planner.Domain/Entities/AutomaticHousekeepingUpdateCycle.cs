using System;

namespace Planner.Domain.Entities
{
	public class AutomaticHousekeepingUpdateCycle
	{
		public Guid Id { get; set; }
		public DateTime StartedAt { get; set; }
		public DateTime? EndedAt { get; set; }
		public bool InProgress { get; set; }
		public string StateChanges { get; set; }
	}
}
