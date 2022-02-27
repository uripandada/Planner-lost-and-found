using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{
	public class UserSubGroup : ChangeTrackingBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
