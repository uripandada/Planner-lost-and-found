using System;

namespace Planner.Domain.Entities
{
	public class SystemTaskAction
	{
		public Guid Id { get; set; }
		public string ActionName { get; set; }
		public string AssetName { get; set; }
		public string AssetGroupName { get; set; }
		public Guid AssetId { get; set; }
		public Guid AssetGroupId { get; set; }
		public int AssetQuantity { get; set; }

		public Guid SystemTaskId { get; set; }
		public SystemTask SystemTask { get; set; }
	}
}
