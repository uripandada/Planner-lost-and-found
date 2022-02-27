using System;

namespace Planner.Domain.Entities
{
	public class RoomAssetModel
	{
		public Guid RoomId { get; set; }
		public Room Room { get; set; }
		public Guid AssetModelId { get; set; }
		public AssetModel AssetModel { get; set; }
		public int Quantity { get; set; }
	}
}
