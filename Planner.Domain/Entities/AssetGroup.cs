using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class AssetGroup : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public Guid? ParentAssetGroupId { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; } // SIMPLE, GROUP

		public AssetGroup ParentAssetGroup { get; set; }
		public IEnumerable<AssetGroup> ChildAssetGroups { get; set; }

		public IEnumerable<Asset> GroupAssets { get; set; }
		public IEnumerable<Asset> SubGroupAssets { get; set; }
		
		public IEnumerable<AssetAction> AssetActions { get; set; }
	}
}
