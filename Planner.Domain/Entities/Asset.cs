using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
	public class Asset: ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public Guid? AssetGroupId { get; set; }
		public Guid? AssetSubGroupId { get; set; }

		public AssetGroup AssetGroup { get; set; }
		public AssetGroup AssetSubGroup { get; set; }
		public IEnumerable<AssetTag> AssetTags { get; set; }
		public IEnumerable<AssetFile> AssetFiles { get; set; }
		//public IEnumerable<RoomAsset> RoomAssets { get; set; }
		//public IEnumerable<AssetAction> Actions { get; set; }

		public IEnumerable<WarehouseAssetAvailability> WarehouseAvailabilities { get; set; }
		public IEnumerable<RoomAssetUsage> RoomUsages { get; set; }
		public IEnumerable<InventoryAssetStatus> InventoryStatuses { get; set; }
		public IEnumerable<WarehouseDocument> WarehouseDocuments { get; set; }
		public IEnumerable<WarehouseDocumentArchive> WarehouseDocumentArchives { get; set; }
	}
}
