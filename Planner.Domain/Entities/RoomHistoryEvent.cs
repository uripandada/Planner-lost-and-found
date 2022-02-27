using Planner.Common.Enums;
using System;

namespace Planner.Domain.Entities
{
	public class RoomHistoryEvent
	{
		public Guid Id { get; set; }
		public Guid RoomId { get; set; }
		public Room Room { get; set; }
		public Guid? RoomBedId { get; set; }
		public RoomBed RoomBed { get; set; }
		public Guid? UserId { get; set; }
		public User User { get; set; }
		public RoomEventType Type { get; set; }
		public DateTime At { get; set; }
		public string Message { get; set; }
		public string OldData { get; set; }
		public string NewData { get; set; }
	}
}
