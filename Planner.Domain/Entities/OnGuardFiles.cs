using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
    public class OnGuardFile
    {
        public Guid FileId { get; set; }
        public File File { get; set; }
        public Guid OnGuardId { get; set; }
        public OnGuard OnGuard { get; set; }
    }
}
