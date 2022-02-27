using System;

namespace Planner.Application.OnGuards.Models
{
    public class OnGuardFileModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsImage { get; set; }
        public string Extension { get; set; }
    }


}
