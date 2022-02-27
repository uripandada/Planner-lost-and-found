using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
	public class RoomCategory : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		
		/// <summary>
		/// Whether rooms with this category are public.
		/// </summary>
		public bool IsPublic { get; set; }

		/// <summary>
		/// Whether rooms with this category are private.
		/// </summary>
		public bool IsPrivate { get; set; }

		/// <summary>
		/// System default category's main purpose is to be the fallback category on reservation sync if there is no other defined default room categories.
		/// System default category is seeded/inserted per hotel group once the hotel group is on boarded.
		/// Can't be deleted but it can be renamed.
		/// There should be ONLY ONE. Otherwise the system will choose at random per reservation sync cycle.
		/// </summary>
		public bool IsSystemDefaultForReservationSync { get; set; }

		/// <summary>
		/// A flag to override the SystemDefault value.
		/// </summary>
		public bool IsDefaultForReservationSync { get; set; }

		public IEnumerable<Room> Rooms { get; set; }
	}
}
