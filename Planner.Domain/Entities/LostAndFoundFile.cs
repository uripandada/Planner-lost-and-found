using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
    public class LostAndFoundFile
    {
        public Guid FileId { get; set; }
        public File File { get; set; }
        public Guid LostAndFoundId { get; set; }
        public LostAndFound LostAndFound { get; set; }
    }
}
