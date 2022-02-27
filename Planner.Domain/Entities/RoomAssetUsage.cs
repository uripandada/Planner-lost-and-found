using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planner.Domain.Entities
{
	/// <summary>
	/// Represents the current usage of the asset in the room
	/// </summary>
	public class RoomAssetUsage
	{
		public Guid Id { get; set; }
		public int Quantity { get; set; }
		public Guid RoomId { get; set; }
		public Room Room { get; set; }
		public Guid AssetId { get; set; }
		public Asset Asset { get; set; }
		//public byte[] ConcurrencyToken { get; set; }

		public uint xmin { get; set; }
	}
}
