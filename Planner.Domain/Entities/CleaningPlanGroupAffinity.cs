using Planner.Common.Enums;
using System;

namespace Planner.Domain.Entities
{
	public class CleaningPlanGroupAffinity
	{
		public Guid CleaningPlanGroupId { get; set; }
		public CleaningPlanGroup CleaningPlanGroup { get; set; }

		public string ReferenceId { get; set; }
		public CleaningPlanGroupAffinityType AffinityType { get; set; }
	}
}
