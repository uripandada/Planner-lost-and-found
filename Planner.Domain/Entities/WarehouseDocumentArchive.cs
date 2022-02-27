using System;

namespace Planner.Domain.Entities
{
	/// <summary>
	/// Represents a warehouse document (an event) - inputs and outputs to the warehouse
	/// </summary>
	public class WarehouseDocumentArchive : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public Guid WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }
		public string TypeKey { get; set; }
		public string Note { get; set; }
		public Guid AssetId { get; set; }
		public Asset Asset { get; set; }
		public int AvailableQuantityChange { get; set; }
		public int AvailableQuantityBeforeChange { get; set; }
		public int ReservedQuantityChange { get; set; }
		public int ReservedQuantityBeforeChange { get; set; }
	}
}
