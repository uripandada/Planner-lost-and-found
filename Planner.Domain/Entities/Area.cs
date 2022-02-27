using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
	public class Area : BaseEntity
	{
		public Guid Id{ get; set; }
		public string Name { get; set; }

		public IEnumerable<Building> Buildings { get; set; }
		public IEnumerable<Room> Rooms { get; set; }
	}
}
