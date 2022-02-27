using System;

namespace Planner.Domain.Entities
{
	/// <summary>
	/// Quantity of an asset at the inventory
	/// </summary>
	public class InventoryAssetStatus
	{
		public Guid Id { get; set; }
		public int Quantity { get; set; }
		public Guid InventoryId { get; set; }
		public Inventory Inventory { get; set; }
		public Guid AssetId { get; set; }
		public Asset Asset { get; set; }
	}
}
