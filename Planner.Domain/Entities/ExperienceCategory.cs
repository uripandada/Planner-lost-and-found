using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
	public class ExperienceCategory : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ExperienceName { get; set; }

		/// <summary>
		/// Whether rooms with this category are public.
		/// </summary>
		/// <summary>
		/// System default category's main purpose is to be the fallback category on reservation sync if there is no other defined default room categories.
		/// System default category is seeded/inserted per hotel group once the hotel group is on boarded.
		/// Can't be deleted but it can be renamed.
		/// There should be ONLY ONE. Otherwise the system will choose at random per reservation sync cycle.
		/// </summary>

	}
}
