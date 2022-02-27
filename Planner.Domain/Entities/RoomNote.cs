using System;

namespace Planner.Domain.Entities
{
	public class RoomNote
	{
		public Guid Id { get; set; }
		public string Note { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? Expiration { get; set; }
		public bool IsArchived { get; set; }
		/// <summary>
		/// The application that the note was created from.
		/// </summary>
		public string Application { get; set; }

		public Guid RoomId { get; set; }
		public Room Room { get; set; }

		public Guid CreatedById { get; set; }
		public User CreatedBy { get; set; }
		
		public Guid? TaskId { get; set; }
		public SystemTask Task { get; set; }
	}
}
