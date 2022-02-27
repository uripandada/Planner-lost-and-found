using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class Floor : BaseEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Number { get; set; }
		public int OrdinalNumber { get; set; }

		public Guid BuildingId { get; set; }
		public Building Building { get; set; }

		public IEnumerable<Room> Rooms { get; set; }
		public IEnumerable<Warehouse> Warehouses { get; set; }
	}
}
