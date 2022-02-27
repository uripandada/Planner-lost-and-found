using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class SystemTaskConfiguration: ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public SystemTaskConfigurationData Data { get; set; }

		public IEnumerable<SystemTask> Tasks { get; set; }
	}

}
