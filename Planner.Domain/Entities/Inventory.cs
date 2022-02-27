using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	/// <summary>
	/// Describes a process of creating an inventory.
	/// </summary>
	public class Inventory: ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public IEnumerable<InventoryAssetStatus> AssetStatuses { get; set; }
		public Guid WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }
		public DateTime Date { get; set; }
	}
}
