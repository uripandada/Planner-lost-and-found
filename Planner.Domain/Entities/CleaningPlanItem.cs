using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{

	public class CleaningPlanItem
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public int? Credits { get; set; }
		public bool IsActive { get; set; }
		public bool IsCustom { get; set; }
		public bool IsPostponed { get; set; }
		public bool IsChangeSheets { get; set; }
		public bool IsPriority { get; set; }

		public bool IsPostponer { get; set; }
		public bool IsPostponee { get; set; }
		public Guid? PostponerCleaningPlanItemId { get; set; }
		public CleaningPlanItem PostponerCleaningPlanItem { get; set; }
		public Guid? PostponeeCleaningPlanItemId { get; set; }
		public CleaningPlanItem PostponeeCleaningPlanItem { get; set; }

		public bool IsPlanned { get; set; }
		public DateTime? StartsAt { get; set; }
		public DateTime? EndsAt { get; set; }
		public int? DurationSec { get; set; }

		public Guid CleaningPlanId { get; set; }
		public CleaningPlan CleaningPlan { get; set; }

		public Guid? CleaningPlanGroupId { get; set; }
		public CleaningPlanGroup CleaningPlanGroup { get; set; }

		//public IEnumerable<CleaningPlanItem> PlannableItems { get; set; }


		public Guid RoomId { get; set; }
		public Room Room { get; set; }
		public Guid? RoomBedId { get; set; }
		public RoomBed RoomBed { get; set; }

		public Guid? CleaningPluginId { get; set; }
		public CleaningPlugin CleaningPlugin { get; set; }

		public Guid? CleaningId { get; set; }
		public Cleaning Cleaning { get; set; }
	}

}
