using System.Collections.Generic;

namespace Planner.Application.UserManagement.Models
{
    public class FullGroupHierarchyData
    {
        public List<GroupHierarchyData> Groups { get; set; }
        public List<UserHierarchyData> UngroupedUsers { get; set; }
    }
}
