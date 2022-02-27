using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Queries.GetFullUserHierarchy
{
	public class GetFullUserGroupsHierarchyQuery : IRequest<FullGroupHierarchyData>
    {
        public string Keywords { get; set; }
        public string SortKey { get; set; }
        public string ActiveStatusKey { get; set; }
        public bool ShowEmptyGroupsAndSubGroups { get; set; }
    }

    public class GetFullUserGroupsHierarchyQueryHandler : IRequestHandler<GetFullUserGroupsHierarchyQuery, FullGroupHierarchyData>, IAmWebApplicationHandler
    {
        private struct GroupKey
		{
            public Guid Id { get; set; }
            public string Name { get; set; }
		}

        private readonly IDatabaseContext databaseContext;

        public GetFullUserGroupsHierarchyQueryHandler(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<FullGroupHierarchyData> Handle(GetFullUserGroupsHierarchyQuery request, CancellationToken cancellationToken)
        {
            //var users = await this.databaseContext.Users.Select(u => new
            //{
            //    Id = u.Id,
            //    FirstName = u.FirstName,
            //    LastName = u.LastName,
            //    UserName = u.UserName,
            //    AvatarUrl = u.Avatar.FileUrl,
            //    UserSubGroupId = u.UserSubGroupID,
            //    UserSubGroupName = u.UserSubGroup.Name,
            //    UserGroupId = u.UserSubGroup.UserGroupId,
            //    UserGroupName = u.UserSubGroup.UserGroup.Name
            //}).ToArrayAsync();

            var userRolesMap = (await this.databaseContext.UserRoles.ToArrayAsync()).GroupBy(g => g.UserId).ToDictionary(g => g.Key, g => g.ToArray());
            var rolesMap = (await this.databaseContext.Roles.ToArrayAsync()).ToDictionary(g => g.Id);

            var result = new FullGroupHierarchyData();

            var usersQuery = this.databaseContext
                .Users
                .Include(u => u.UserSubGroup)
                .Include(u => u.UserGroup)
                .AsQueryable();

            if (request.Keywords.IsNotNull())
			{
                var keywordsValue = request.Keywords.Trim().ToLower();
                usersQuery = usersQuery.Where(u => u.FirstName.ToLower().Contains(keywordsValue) || u.LastName.ToLower().Contains(keywordsValue) || u.UserGroup.Name.ToLower().Contains(keywordsValue) || u.UserSubGroup.Name.ToLower().Contains(keywordsValue));
            }

            if (request.ActiveStatusKey == "ACTIVE")
            {
                usersQuery = usersQuery.Where(u => u.IsActive);
            } 
            else if (request.ActiveStatusKey == "INACTIVE")
            {
                usersQuery = usersQuery.Where(u => !u.IsActive);
            }

            switch (request.SortKey)
            {
                case "NAME_DESC":
                    usersQuery = usersQuery.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName);
                    break;
                case "NAME_ASC":
                default:
                    usersQuery = usersQuery.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
                    break;
            }

            var users = await usersQuery.ToListAsync();
            var ungroupedUsers = new List<UserHierarchyData>();
            var usersMap = new Dictionary<Guid, UserHierarchyData>();
            var groupsMap = new Dictionary<Guid, GroupHierarchyData>();
            var subGroupsMap = new Dictionary<Guid, SubGroupHierarchyData>();

            var avatarImagesMap = (await this.databaseContext.ApplicationUserAvatar.Select(a => new { a.Id, a.FileUrl }).ToArrayAsync()).ToDictionary(a => a.Id);

            //result.Groups = GroupHierarchyData.CreateLists(data);

            foreach(var user in users)
			{
                var userData = new UserHierarchyData
                {
                    AvatarImageUrl = avatarImagesMap.ContainsKey(user.Id) ? avatarImagesMap[user.Id].FileUrl : null,
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.UserName,
                    FullNameInitials = $"{user.FirstName[0]}{user.LastName[0]}",
                    UserRole = "N/A",
                    IsSubGroupLeader = user.IsSubGroupLeader,
                    IsActive = user.IsActive,
                    DefaultAvatarColorHex = user.DefaultAvatarColorHex,
                };

                if (userRolesMap.ContainsKey(user.Id))
                {
                    var userSpecificRoles = userRolesMap[user.Id];
                    if (userSpecificRoles.Length > 0)
                    {
                        var roleId = userSpecificRoles[0].RoleId;
                        userData.UserRole = rolesMap[roleId].Name;
                    }
                }

				if (!user.UserGroupId.HasValue)
				{
                    ungroupedUsers.Add(userData);
                    continue;
				}

                var group = (GroupHierarchyData)null;
                var groupId = user.UserGroupId.Value;
                if (groupsMap.ContainsKey(groupId))
                {
                    group = groupsMap[groupId];
                }
                else
                {
                    group = new GroupHierarchyData
                    {
                        Id = groupId,
                        CreatedAt = user.UserGroup.CreatedAt,
                        Name = user.UserGroup.Name,
                        SubGroups = new List<SubGroupHierarchyData>(),
                        Users = new List<UserHierarchyData>()

                    };
                    groupsMap.Add(groupId, group);
                }
                
                if (!user.UserSubGroupId.HasValue)
				{
                    group.Users.Add(userData);
                    continue;
				}

                var subGroup = default(SubGroupHierarchyData);
                var subGroupId = user.UserSubGroupId.Value;
				if (subGroupsMap.ContainsKey(subGroupId))
				{
                    subGroup = subGroupsMap[subGroupId];
				}
				else
				{
                    subGroup = new SubGroupHierarchyData
                    {
                        Id = subGroupId,
                        CreatedAt = user.UserSubGroup.CreatedAt,
                        Name = user.UserSubGroup.Name,
                        Users = new List<UserHierarchyData>(),
                    };
                    subGroupsMap.Add(subGroupId, subGroup);
                    group.SubGroups.Add(subGroup);
                }
                subGroup.Users.Add(userData);
            }

            if (request.ShowEmptyGroupsAndSubGroups)
            {
                var userGroups = await this.databaseContext
                    .UserGroups
                    .Include(g => g.UserSubGroups)
                    .ToArrayAsync();

                foreach (var group in userGroups)
                {
					if (!groupsMap.ContainsKey(group.Id))
					{
                        groupsMap.Add(group.Id, new GroupHierarchyData
                        {
                            Id = group.Id,
                            CreatedAt = group.CreatedAt,
                            Name = group.Name,
                            SubGroups = new List<SubGroupHierarchyData>(),
                            Users = new List<UserHierarchyData>()
                        });
					}

                    var groupData = groupsMap[group.Id];

                    foreach(var subGroup in group.UserSubGroups)
					{
						if (!subGroupsMap.ContainsKey(subGroup.Id))
						{
                            subGroupsMap.Add(subGroup.Id, new SubGroupHierarchyData
                            {
                                Id = subGroup.Id,
                                CreatedAt = subGroup.CreatedAt,
                                Name = subGroup.Name,
                                Users = new List<UserHierarchyData>()
                            });

                            groupData.SubGroups.Add(subGroupsMap[subGroup.Id]);
						}
					}
                }
            }

            return new FullGroupHierarchyData
            {
                Groups = groupsMap.Values.ToList(),
                UngroupedUsers = ungroupedUsers
            };



   //         var groupsData = new List<GroupHierarchyData>();
   //         foreach (var group in data)
			//{
   //             var groupData = new GroupHierarchyData
   //             {
   //                 CreatedAt = group.CreatedAt,
   //                 Id = group.Id,
   //                 Name = group.Name,
   //                 Users = new List<UserHierarchyData>(),
   //                 SubGroups = new List<SubGroupHierarchyData>()
   //             };
   //             groupsData.Add(groupData);


   //             foreach (var user in group.Users.Where(u => !u.UserSubGroupId.HasValue).ToArray())
   //             {
   //                 var userData = new UserHierarchyData
   //                 {
   //                     AvatarImageUrl = avatarImagesMap.ContainsKey(user.Id) ? avatarImagesMap[user.Id].FileUrl : null,
   //                     Id = user.Id,
   //                     FirstName = user.FirstName,
   //                     LastName = user.LastName,
   //                     Username = user.UserName,
   //                     FullNameInitials = $"{user.FirstName[0]}{user.LastName[0]}",
   //                     UserRole = "N/A",
   //                     IsSubGroupLeader = user.IsSubGroupLeader,
   //                     IsActive = user.IsActive,
   //                 };
   //                 groupData.Users.Add(userData);

   //                 if (userRolesMap.ContainsKey(user.Id))
   //                 {
   //                     var userSpecificRoles = userRolesMap[user.Id];
   //                     if (userSpecificRoles.Length > 0)
   //                     {
   //                         var roleId = userSpecificRoles[0].RoleId;
   //                         userData.UserRole = rolesMap[roleId].Name;
   //                     }
   //                 }
   //             }

   //             foreach (var subGroup in group.UserSubGroups)
			//	{
   //                 var subGroupData = new SubGroupHierarchyData
   //                 {
   //                     CreatedAt = subGroup.CreatedAt,
   //                     Id = subGroup.Id,
   //                     Name = subGroup.Name,
   //                     Users = new List<UserHierarchyData>()
   //                 };
   //                 groupData.SubGroups.Add(subGroupData);

   //                 foreach(var user in subGroup.Users)
			//		{
   //                     var userData = new UserHierarchyData
   //                     {
   //                         AvatarImageUrl = avatarImagesMap.ContainsKey(user.Id) ? avatarImagesMap[user.Id].FileUrl : null,
   //                         Id = user.Id,
   //                         FirstName = user.FirstName,
   //                         LastName = user.LastName,
   //                         Username = user.UserName,
   //                         FullNameInitials = $"{user.FirstName[0]}{user.LastName[0]}",
   //                         UserRole = "N/A",
   //                         IsSubGroupLeader = user.IsSubGroupLeader,
   //                         IsActive = user.IsActive
   //                     };
   //                     subGroupData.Users.Add(userData);

			//			if (userRolesMap.ContainsKey(user.Id))
			//			{
   //                         var userSpecificRoles = userRolesMap[user.Id];
   //                         if(userSpecificRoles.Length > 0)
			//				{
   //                             var roleId = userSpecificRoles[0].RoleId;
   //                             userData.UserRole = rolesMap[roleId].Name;
   //                         }
			//			}
			//		}
			//	}
			//}

   //         result.Groups = groupsData;
   //         result.UngroupedUsers = await this.databaseContext.Users.Where(u => !u.UserSubGroupId.HasValue && !u.UserGroupId.HasValue).Select(u => new UserHierarchyData 
   //         {
   //             AvatarImageUrl = u.Avatar.FileUrl,
   //             FirstName = u.FirstName,
   //             FullNameInitials = "",
   //             Id = u.Id,
   //             IsSubGroupLeader = u.IsSubGroupLeader,
   //             LastName = u.LastName,
   //             Username = u.UserName,
   //             UserRole = "N/A",
   //             IsActive = u.IsActive
   //         }).ToListAsync();
            
   //         foreach(var user in result.UngroupedUsers)
			//{
   //             user.FullNameInitials = $"{user.FirstName[0]}{user.LastName[0]}";
   //             if (userRolesMap.ContainsKey(user.Id))
   //             {
   //                 var userSpecificRoles = userRolesMap[user.Id];
   //                 if (userSpecificRoles.Length > 0)
   //                 {
   //                     var roleId = userSpecificRoles[0].RoleId;
   //                     user.UserRole = rolesMap[roleId].Name;
   //                 }
   //             }
   //         }

   //         return new ProcessResponse<FullGroupHierarchyData>
   //         {
   //             Data = result,
   //             IsSuccess = true
   //         };
        }
    }
}
