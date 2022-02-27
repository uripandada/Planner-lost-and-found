using System;

namespace Planner.Domain.Entities
{
	public class AssetTag
	{
		public Guid AssetId { get; set; }
		public Asset Asset { get; set; }
		public string TagKey { get; set; }
		public Tag Tag { get; set; }
	}
}
