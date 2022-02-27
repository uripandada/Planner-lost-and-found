using System;

namespace Planner.Application.LostAndFounds.Models
{
    public class LostAndFoundFileModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsImage { get; set; }
        public string Extension { get; set; }
    }
}
