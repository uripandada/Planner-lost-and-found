using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
    public class OnGuard : ChangeTrackingBaseEntity
    {
        public OnGuard()
        {
            Files = new List<OnGuardFile>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Identification { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public OnGuardStatus Status { get; set; }
        public ICollection<OnGuardFile> Files { get; set; }
    }
}
