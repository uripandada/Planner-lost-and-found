using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Domain.Entities
{
	/// <summary>
	/// Represents the current state of the asset on the warehouse
	/// </summary>
	public class WarehouseAssetAvailability
	{
		public Guid Id { get; set; }
		public int Quantity { get; set; }
		public int ReservedQuantity { get; set; }
		public Guid WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }
		public Guid AssetId { get; set; }
		public Asset Asset { get; set; }
		//public byte[] ConcurrencyToken { get; set; }

		public uint xmin { get; set; }
	}
}
