using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class PlannableCleaningPlanItem
	{
		public Guid Id { get; set; }
		public string CleaningPluginName { get; set; }
		public int? Credits { get; set; }
		public bool IsActive { get; set; }
		public bool IsCustom { get; set; }
		public bool IsPostponed { get; set; }
		public bool IsPlanned { get; set; }
		public bool IsChangeSheets { get; set; }

		public Guid CleaningPlanId { get; set; }
		public CleaningPlan CleaningPlan { get; set; }

		public Guid RoomId { get; set; }
		public Room Room { get; set; }

		public Guid? CleaningPluginId { get; set; }
		public CleaningPlugin CleaningPlugin { get; set; }

		public Guid? PostponedFromPlannableCleaningPlanItemId { get; set; }
		public PlannableCleaningPlanItem PostponedFromPlannableCleaningPlanItem { get; set; }
		public IEnumerable<PlannableCleaningPlanItem> PostponedToPlannableCleaningPlanItems { get; set; }
	}
}
