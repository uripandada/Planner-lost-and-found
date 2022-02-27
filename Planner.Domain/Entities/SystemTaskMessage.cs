using System;

namespace Planner.Domain.Entities
{
	public class SystemTaskMessage: ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public Guid SystemTaskId { get; set; }
		public SystemTask SystemTask { get; set; }
		public string Message { get; set; }
	}
}
