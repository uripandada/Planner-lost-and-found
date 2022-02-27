using Planner.Common.Enums;
using System;

namespace Planner.Domain.Entities
{
	public class CleaningHistoryEvent
	{
		public Guid Id { get; set; }
		public Guid CleaningId { get; set; }
		public Cleaning Cleaning { get; set; }
		public Guid RoomId { get; set; }
		public Room Room { get; set; }
		public Guid? UserId { get; set; }
		public User User { get; set; }
		public CleaningEventType Type { get; set; }
		public CleaningProcessStatus Status { get; set; }
		public DateTime At { get; set; }
		public string Message { get; set; }
		public string OldData { get; set; }
		public string NewData { get; set; }
	}
}
