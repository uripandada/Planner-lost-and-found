using System;

namespace Planner.Domain.Entities
{
	public class ApplicationUserAvatar
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public byte[] File { get; set; }

        public User User { get; set; }
    }
}
