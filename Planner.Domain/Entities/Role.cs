using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class Role : IdentityRole<Guid>
	{
        public bool IsSystemRole { get; set; }
		public string HotelAccessTypeKey { get; set; }
	}

	public class ReservationOtherProperties
	{
		public IEnumerable<ReservationOtherProperty> Properties { get; set; }
	}

	public class ReservationOtherProperty
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}
}
