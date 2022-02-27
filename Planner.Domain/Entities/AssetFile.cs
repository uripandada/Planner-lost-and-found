using System;

namespace Planner.Domain.Entities
{
	public class AssetFile
	{
		public Guid AssetId { get; set; }
		public Asset Asset { get; set; }
		public Guid FileId { get; set; }
		public File File { get; set; }
		public bool IsPrimaryImage { get; set; }
		public bool IsQrCodeImage { get; set; }
	}
}
