using System;

namespace Planner.Domain.Entities
{
	public abstract class ChangeTrackingBaseEntity
	{
		public DateTime CreatedAt { get; set; }
		public Guid? CreatedById { get; set; }
		public User CreatedBy { get; set; }
		public DateTime ModifiedAt { get; set; }
		public User ModifiedBy { get; set; }
		public Guid? ModifiedById { get; set; }

	}
}
