using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Queries.GetListOfUsers
{
	public class GetListOfUsersQuery : IRequest<IEnumerable<UserListItemData>>
	{
	}

	public class GetListOfUsersQueryHandler : IRequestHandler<GetListOfUsersQuery, IEnumerable<UserListItemData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetListOfUsersQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<UserListItemData>> Handle(GetListOfUsersQuery request, CancellationToken cancellationToken)
		{
			var users = await this._databaseContext.Users.Select(u => new { 
				Id = u.Id,
				FirstName = u.FirstName,
				LastName = u.LastName,
				UserName = u.UserName,
				AvatarUrl = u.Avatar.FileUrl,
				IsSubGroupLeader = u.IsSubGroupLeader,
				IsActive = u.IsActive,
				DefaultAvatarColorHex = u.DefaultAvatarColorHex,
			}).ToArrayAsync();
			var userRolesMap = (await this._databaseContext.UserRoles.ToArrayAsync()).GroupBy(g => g.UserId).ToDictionary(g => g.Key, g => g.ToArray());
			var rolesMap = (await this._databaseContext.Roles.ToArrayAsync()).ToDictionary(g => g.Id);

			var response = new List<UserListItemData>();
			foreach(var user in users)
			{
				var u = new UserListItemData
				{
					Id = user.Id,
					FullName = $"{user.FirstName} {user.LastName}",
					UserName = user.UserName,
					RoleName = "N/A",
					AvatarImageUrl = user.AvatarUrl,
					FullNameInitials = $"{user.FirstName[0]}{user.LastName[0]}",
					IsSubGroupLeader = user.IsSubGroupLeader,
					IsActive = user.IsActive,
					DefaultAvatarColorHex = user.DefaultAvatarColorHex,
				};

				if (userRolesMap.ContainsKey(user.Id))
				{
					var userRoles = userRolesMap[user.Id];
					var roleNames = new List<string>();
					foreach(var userRole in userRoles)
					{
						roleNames.Add(rolesMap[userRole.RoleId].Name);
					}

					u.RoleName = string.Join(", ", roleNames);
				}

				response.Add(u);
			}

			return response;
		}
	}
}
