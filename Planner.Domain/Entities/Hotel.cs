using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class Hotel
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public string WindowsTimeZoneId { get; set; }
		public string IanaTimeZoneId { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset ModifiedAt { get; set; }

		public IEnumerable<Room> Rooms { get; set; }
		public IEnumerable<Building> Buildings { get; set; }
		public IEnumerable<CleaningPlugin> CleaningPlugins { get; set; }
		
		public Settings Settings { get; set; }
	}
}
