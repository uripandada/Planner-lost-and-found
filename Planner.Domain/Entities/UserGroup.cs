using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
    public class UserGroup : ChangeTrackingBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserSubGroup> UserSubGroups { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
