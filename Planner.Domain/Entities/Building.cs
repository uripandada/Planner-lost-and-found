using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Planner.Domain.Entities
{
	public class Building : BaseEntity
	{
		public Guid Id { get; set; }
		
		public string Name { get; set; }
		
		public string TypeKey { get; set; }
		public string Address { get; set; }
		public long? Latitude { get; set; }
		public long? Longitude { get; set; }
		public int OrdinalNumber { get; set; }

		public Guid? AreaId { get; set; }
		public Area Area { get; set; }

		public IEnumerable<Floor> Floors { get; set; }
		public IEnumerable<Room> Rooms { get; set; }
	}
}
