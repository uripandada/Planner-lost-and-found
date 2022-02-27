using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Planner.Application.UserManagement.Models
{
    public class GroupHierarchyData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<UserHierarchyData> Users { get; set; }

        public List<SubGroupHierarchyData> SubGroups { get; set; }


        public static Expression<Func<Domain.Entities.UserGroup, GroupHierarchyData>> Projection
        {
            get
            {
                return x => new GroupHierarchyData
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    Users = UserHierarchyData.CreateList(x.Users),
                    SubGroups = SubGroupHierarchyData.CreateList(x.UserSubGroups)
                };
            }
        }

        public static GroupHierarchyData Create(Domain.Entities.UserGroup group)
        {
            return Projection.Compile().Invoke(group);
        }

        public static List<GroupHierarchyData> CreateLists(IEnumerable<Domain.Entities.UserGroup> groups)
        {
            if (groups == null)
                return new List<GroupHierarchyData>();

            return groups.OrderBy(x=>x.Name).Select(x => Create(x)).ToList();
        }
    }
}
