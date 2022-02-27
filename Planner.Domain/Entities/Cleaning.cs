using Planner.Common.Enums;
using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class Cleaning
	{
		public Guid Id { get; set; }
		public string Description { get; set; }
		public int? Credits { get; set; }
		public bool IsActive { get; set; }
		public bool IsCustom { get; set; }
		public bool IsPostponed { get; set; }
		public bool IsChangeSheets { get; set; }
		/// <summary>
		/// If the inspection is required, the cleaning process is not really finished until the inspection is finished also.
		/// </summary>
		public bool IsInspectionRequired { get; set; }
		/// <summary>
		/// After the cleaner is done cleaning, this flag is set to true only if the inspection is required.
		/// </summary>
		public bool IsReadyForInspection { get; set; }
		/// <summary>
		/// Set to true after the inspector finishes the inspection.
		/// </summary>
		public bool IsInspected { get; set; }
		/// <summary>
		/// Set to true only if the inspaction passed successfully.
		/// </summary>
		public bool IsInspectionSuccess { get; set; }
		/// <summary>
		/// Id of the inspector.
		/// </summary>
		public Guid? InspectedById { get; set; }
		public User InspectedBy { get; set; }

		/// <summary>
		/// Describes the lifecycle status of the cleaning.
		/// </summary>
		public CleaningProcessStatus Status { get; set; }

		/// <summary>
		/// This flag is set to true if the cleaning is sent to the attendant.
		/// It becomes false if the cleaning is removed from the plan and the plan is resent.
		/// </summary>
		public bool IsPlanned { get; set; }

		public bool IsPriority { get; set; }

		public DateTime StartsAt { get; set; }
		public DateTime EndsAt { get; set; }
		public int DurationSec { get; set; }

		public Guid CleanerId { get; set; }
		public User Cleaner { get; set; }
		
		public Guid RoomId { get; set; }
		public Room Room { get; set; }
		public Guid? RoomBedId { get; set; }
		public RoomBed RoomBed { get; set; }

		public Guid? CleaningPluginId { get; set; }
		public CleaningPlugin CleaningPlugin { get; set; }

		/// <summary>
		/// History of cleaning inspections. Multiple entries when the cleaning inspection fails.
		/// </summary>
		public IEnumerable<CleaningInspection> CleaningInspections { get; set; }

		/// <summary>
		/// Cleaning history.
		/// </summary>
		public IEnumerable<CleaningHistoryEvent> CleaningHistoryEvents { get; set; }

		public Guid CleaningPlanId { get; set; }
		public CleaningPlan CleaningPlan { get; set; }

		/// <summary>
		/// WARNING!!!
		/// WARNING!!!
		/// WARNING: This property is really a "hack" around not being able to easily define 0..1:0..1 relation in EF.
		/// This array will EVER have either 1 or 0 items in this array.
		/// IMPORTANT!!!
		/// IMPORTANT!!!
		/// </summary>
		public IEnumerable<CleaningPlanItem> CleaningPlanItems { get; set; }

		//public DateTime ModifiedAt { get; set; }
	}
}
