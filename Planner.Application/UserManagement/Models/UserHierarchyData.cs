using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Planner.Application.UserManagement.Models
{
    public class UserHierarchyData
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserRole { get; set; }
        public string AvatarImageUrl { get; set; }
        public string FullNameInitials { get; set; }
        public bool IsSubGroupLeader { get; set; }
        public bool IsActive { get; set; }
        public string DefaultAvatarColorHex { get; set; }

        public static Expression<Func<Domain.Entities.User, UserHierarchyData>> Projection
        {
            get
            {
                return x => new UserHierarchyData
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Username = x.UserName,
                    UserRole = "",
                    IsSubGroupLeader = x.IsSubGroupLeader,
                    DefaultAvatarColorHex = x.DefaultAvatarColorHex,
                    IsActive = x.IsActive,
                };
            }
        }

        public static UserHierarchyData Create(Domain.Entities.User user)
        {
            return Projection.Compile().Invoke(user);
        }

        public static List<UserHierarchyData> CreateList(IEnumerable<Domain.Entities.User> users)
        {
            if (users == null)
                return new List<UserHierarchyData>();

            return users.OrderBy(x=>x.LastName).ThenBy(x=>x.FirstName).Select(c => Create(c)).ToList();
        }
    }
}
