using System;

namespace Planner.Domain.Entities
{
	public class SystemTaskHistory
	{
		public Guid Id { get; set; }
		public Guid SystemTaskId { get; set; }
		public SystemTask SystemTask { get; set; }
		public Guid? CreatedById { get; set; }
		public User CreatedBy { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public string ChangedByKey { get; set; }
		public string Message { get; set; }
		public SystemTaskHistoryData OldData { get; set; }
		public SystemTaskHistoryData NewData { get; set; }
	}
}
