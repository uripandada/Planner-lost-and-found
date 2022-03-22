using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
	public class Experience : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public string RoomName { get; set; }
		public string GuestName { get; set; }
		public DateTime? CheckIn { get; set; }
		public DateTime? CheckOut { get; set; }
		public string ReservationId { get; set; }
		public string VIP { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public int Type { get; set; }
		public string Description { get; set; }
		public string Actions { get; set; }
		public string InternalFollowUp { get; set; }
		public Guid ExperienceCategoryId { get; set; }
		public ExperienceCategory ExperienceCategory { get; set; }
		public Guid ExperienceCompensationId { get; set; }
		public ExperienceCompensation ExperienceCompensation { get; set; }
		public string Group { get; set; }
		public ExperienceTicketStatus ExperienceTicketStatus { get; set; }
		public ExperienceClientRelationStatus ExperienceClientRelationStatus { get; set; }
		public ExperienceResolutionStatus ExperienceResolutionStatus { get; set; }

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
