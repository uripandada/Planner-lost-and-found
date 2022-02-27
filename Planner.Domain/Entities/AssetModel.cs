using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class AssetModel : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public Guid AssetId { get; set; }
		public Asset Asset { get; set; }
		public string Name { get; set; }
		public int AvailableQuantity { get; set; }
		//public bool IsAvailableToMaintenance { get; set; }
		//public bool IsAvailableToHousekeeping { get; set; }
		public IEnumerable<RoomAssetModel> RoomAssetModels { get; set; }
		//public IEnumerable<AssetAction> Actions { get; set; }
	}
}
