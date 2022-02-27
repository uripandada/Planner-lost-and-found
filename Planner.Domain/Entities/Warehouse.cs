using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Domain.Entities
{
	public class Warehouse: ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsCentral { get; set; }
		public Guid? FloorId { get; set; }
		public Floor Floor { get; set; }
		public string HotelId { get; set; }
		public Hotel Hotel { get; set; }

		public IEnumerable<WarehouseAssetAvailability> AssetAvailabilities { get; set; }
		public IEnumerable<Inventory> Inventories { get; set; }
		public IEnumerable<WarehouseDocument> WarehouseDocuments { get; set; }
		public IEnumerable<WarehouseDocumentArchive> WarehouseDocumentArchives { get; set; }
	}
}
