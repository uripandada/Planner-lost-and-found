using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupUsers
{
	public class HotelGroupUserData
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Username { get; set; }
		public string UserSubGroupName { get; set; }
		public string UserGroupName { get; set; }
		public bool IsSubGroupLeader { get; set; }
		public bool IsActive { get; set; }
	}

	public class GetPageOfHotelGroupUsersQuery : GetPageRequest, IRequest<PageOf<HotelGroupUserData>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
		public string ActiveStatusKey { get; set; }
	}

	public class GetPageOfHotelGroupUsersQueryHandler : IRequestHandler<GetPageOfHotelGroupUsersQuery, PageOf<HotelGroupUserData>>, IAmAdminApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		public GetPageOfHotelGroupUsersQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<PageOf<HotelGroupUserData>> Handle(GetPageOfHotelGroupUsersQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Users
				   .AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				query = query.Where(user => user.FirstName.ToLower().Contains(keywordsValue) || user.LastName.ToLower().Contains(keywordsValue) || user.UserName.ToLower().Contains(keywordsValue));
			}

			if (request.ActiveStatusKey.IsNotNull())
			{
				if(request.ActiveStatusKey == "ACTIVE")
				{
					query = query.Where(user => user.IsActive);
				}
				else if (request.ActiveStatusKey == "INACTIVE")
				{
					query = query.Where(user => !user.IsActive);
				}
			}

			var count = 0;
			if (request.Skip > 0 || request.Take > 0)
			{
				count = await query.CountAsync();
			}

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					case "FIRST_NAME_DESC":
						query = query.OrderByDescending(group => group.FirstName);
						break;
					case "LAST_NAME_ASC":
						query = query.OrderBy(user => user.LastName);
						break;
					case "LAST_NAME_DESC":
						query = query.OrderByDescending(group => group.LastName);
						break;
					case "USER_NAME_ASC":
						query = query.OrderBy(user => user.UserName);
						break;
					case "USER_NAME_DESC":
						query = query.OrderByDescending(group => group.UserName);
						break;
					case "FIRST_NAME_ASC":
					default:
						query = query.OrderBy(group => group.FirstName);
						break;
				}
			}

			if (request.Skip > 0)
			{
				query = query.Skip(request.Skip);
			}

			if (request.Take > 0)
			{
				query = query.Take(request.Take);
			}

			var users = await query.Select(user => new HotelGroupUserData
			{
				Id = user.Id,
				IsActive = user.IsActive,
				FirstName = user.FirstName,
				LastName = user.LastName,
				IsSubGroupLeader = user.IsSubGroupLeader,
				UserGroupName = user.UserSubGroup.UserGroup.Name,
				Username = user.UserName,
				UserSubGroupName = user.UserSubGroup.Name,
			}).ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = users.Length;
			}

			return new PageOf<HotelGroupUserData>
			{
				TotalNumberOfItems = count,
				Items = users
			};
		}
	}
}
