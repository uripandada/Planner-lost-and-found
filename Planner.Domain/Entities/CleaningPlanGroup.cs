using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class CleaningPlanGroup
	{
		public Guid Id { get; set; }
		public Guid CleaningPlanId { get; set; }
		public CleaningPlan CleaningPlan { get; set; }
		public Guid CleanerId { get; set; }
		public User Cleaner { get; set; }

		public Guid? SecondaryCleanerId { get; set; }
		public User SecondaryCleaner { get; set; }

		public int? MaxCredits { get; set; }
		public int? MaxDepartures { get; set; }
		public int? MaxTwins { get; set; }
		public int? WeeklyHours { get; set; }
		public bool MustFillAllCredits { get; set; }

		public IEnumerable<CleaningPlanItem> Items { get; set; }
		//public IEnumerable<CleaningPlanGroupFloorAffinity> FloorAffinities { get; set; }
		public IEnumerable<CleaningPlanGroupAffinity> Affinities { get; set; }
		public IEnumerable<CleaningPlanGroupAvailabilityInterval> AvailabilityIntervals { get; set; }
	}
}
