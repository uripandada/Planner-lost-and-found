using System.Collections;

namespace Planner.Domain.Entities
{
	public class Tag: ChangeTrackingBaseEntity
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}
}
