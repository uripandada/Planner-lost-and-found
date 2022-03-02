using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities.Master
{
	public class HotelGroupTenant
	{
		public Guid Id { get; set; } // 32 bit guid
		public string Key { get; set; } // "hotel-royal"
		public string Name { get; set; } // "Hotel Royal"
		public string ConnectionString { get; set; } // PostgreSQL connection string to the tenant
		public bool IsActive { get; set; }
		public string DatabaseName { get; set; } // Database name server only for validation purposes. There must not be two databases in the system with the same name!
	}

	public class ExternalClientSecretKey
	{
		public string ClientId { get; set; }
		public string Key { get; set; }
		public bool IsActive { get; set; }
		public bool HasAccessToListOfHotelGroups { get; set; }
		public bool HasAccessToListOfHotels { get; set; }
	}
}
