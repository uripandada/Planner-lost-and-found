using System;

namespace Planner.Domain.Entities
{
	public class SystemTaskConfigurationFile
	{
		public string FileName { get; set; }
		public string FileUrl { get; set; }
		public Guid FileId { get; set; }
	}
}
