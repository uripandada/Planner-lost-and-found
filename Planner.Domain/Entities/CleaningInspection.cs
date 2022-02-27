using System;

namespace Planner.Domain.Entities
{
	public class CleaningInspection
	{
		public Guid Id { get; set; }
		public Guid CreatedById { get; set; }
		public User CreatedBy { get; set; }
		public DateTime StartedAt { get; set; }
		public DateTime? EndedAt { get; set; }
		public bool IsFinished { get; set; }
		public bool IsSuccess { get; set; }
		public string Note { get; set; }

		public Guid CleaningId { get; set; }
		public Cleaning Cleaning { get; set; }

	}
}
