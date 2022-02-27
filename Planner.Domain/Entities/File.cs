using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
	public class File : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		public string FileTypeKey { get; set; }
		public string FileName { get; set; }
		public byte[] FileData { get; set; }
	}
}
