using Planner.Common.Enums;
using System;

namespace Planner.Domain.Entities
{
	public class UserHistoryEvent
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; }
		public UserEventType Type { get; set; }
		public DateTime At { get; set; }
		public string Message { get; set; }
		public string OldData { get; set; }
		public string NewData { get; set; }
	}
}
