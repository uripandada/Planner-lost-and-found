using System;

namespace Planner.Domain.Entities
{
	public class CleaningPlanGroupAvailabilityInterval
	{
		public Guid Id { get; set; }
		public Guid CleaningPlanGroupId { get; set; }
		public CleaningPlanGroup CleaningPlanGroup { get; set; }
		public TimeSpan From { get; set; }
		public TimeSpan To { get; set; }
	}
}
