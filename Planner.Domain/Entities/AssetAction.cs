using System;

namespace Planner.Domain.Entities
{
	public class AssetAction : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }

		public Guid AssetGroupId { get; set; }
		public AssetGroup AssetGroup { get; set; }
		public string Name { get; set; }
		public string QuickOrTimedKey { get; set; }
		public string PriorityKey { get; set; }
		public Guid? DefaultAssignedToUserId { get; set; }
		public User DefaultAssignedToUser { get; set; }
		public Guid? DefaultAssignedToUserGroupId { get; set; }
		public UserGroup DefaultAssignedToUserGroup { get; set; }
		public Guid? DefaultAssignedToUserSubGroupId { get; set; }
		public UserSubGroup DefaultAssignedToUserSubGroup { get; set; }
		public int? Credits { get; set; }
		public decimal? Price { get; set; }
		
		public bool IsSystemDefined { get; set; }

		/// <summary>
		/// Described by enum: SystemActionType
		///   LOCATION_CHANGE
		///   NONE
		/// </summary>
		public string SystemActionTypeKey { get; set; }

		/// <summary>
		/// Described by enum: SystemDefinedActionIdentifier
		///   WAREHOUSE_TO_WAREHOUSE,
		///   ROOM_TO_WAREHOUSE,
		///   WAREHOUSE_TO_ROOM,
		///   ROOM_TO_ROOM,
		///   NONE,
		/// </summary>
		public string SystemDefinedActionIdentifierKey { get; set; }
	}
}
