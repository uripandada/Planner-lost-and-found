using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Planner.Application.UserManagement.Models
{
    public class SubGroupHierarchyData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<UserHierarchyData> Users { get; set; }

        public static Expression<Func<Domain.Entities.UserSubGroup, SubGroupHierarchyData>> Projection
        {
            get
            {
                return x => new SubGroupHierarchyData
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    Users = UserHierarchyData.CreateList(x.Users)
                };
            }
        }

        public static SubGroupHierarchyData Create(Domain.Entities.UserSubGroup subGroup)
        {
            return Projection.Compile().Invoke(subGroup);
        }

        public static List<SubGroupHierarchyData> CreateList(IEnumerable<Domain.Entities.UserSubGroup> subGroups)
        {
            if (subGroups == null)
                return new List<SubGroupHierarchyData>();

            return subGroups.OrderBy(x=>x.Name).Select(x => Create(x)).ToList();
        }
    }
}
