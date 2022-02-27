using System;

namespace Planner.Domain.Entities
{
	public class SystemTaskConfigurationWhat
	{
		public int AssetQuantity { get; set; }
		public string ActionName { get; set; }
		public string AssetName { get; set; }
		public string AssetGroupName { get; set; }
		public Guid AssetId { get; set; }
		public Guid AssetGroupId { get; set; }
		public bool IsActionSystemDefined { get; set; }

		/// <summary>
		/// Described by enum: SystemActionType
		///   LOCATION_CHANGE
		///   NONE
		/// </summary>
		public string SystemDefinedActionTypeKey { get; set; }

		/// <summary>
		/// Described by enum: SystemDefinedActionIdentifier
		///   WAREHOUSE_TO_WAREHOUSE,
		///   ROOM_TO_WAREHOUSE,
		///   WAREHOUSE_TO_ROOM,
		///   ROOM_TO_ROOM,
		///   NONE,
		/// </summary>
		public string SystemDefinedActionIdentifierKey { get; set; }

		//public Guid? DefaultAssignedUserId { get; set; }
	}
}
