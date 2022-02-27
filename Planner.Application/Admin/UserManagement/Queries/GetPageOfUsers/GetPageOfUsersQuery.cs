using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.UserManagement.Queries.GetPageOfUsers
{
	public class UserGridData
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
	}

	public class GetPageOfUsersQuery : GetPageRequest, IRequest<PageOf<UserGridData>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
	}

	public class GetPageOfUsersQueryHandler : IRequestHandler<GetPageOfUsersQuery, PageOf<UserGridData>>, IAmAdminApplicationHandler
	{
		private readonly IMasterDatabaseContext _databaseContext;

		public GetPageOfUsersQueryHandler(IMasterDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<PageOf<UserGridData>> Handle(GetPageOfUsersQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Users
				.AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				query = query.Where(c => c.UserName.ToLower().Contains(keywordsValue) || c.Email.ToLower().Contains(keywordsValue));
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
					case "NAME_ASC":
						query = query.OrderBy(q => q.UserName);
						break;
					case "NAME_DESC":
						query = query.OrderByDescending(q => q.UserName);
						break;
					default:
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

			var categories = await query.ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = categories.Length;
			}

			var response = new PageOf<UserGridData>
			{
				TotalNumberOfItems = count,
				Items = categories.Select(d => new UserGridData
				{
					Id = d.Id,
					Email = d.Email,
					Username = d.UserName
				}).ToArray()
			};

			return response;
		}
	}
}
