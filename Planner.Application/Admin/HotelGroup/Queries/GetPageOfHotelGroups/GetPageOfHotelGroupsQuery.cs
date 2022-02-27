using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroups
{
	public class HotelGroupData
	{
		public Guid Id { get; set; } // 32 bit guid
		public string Key { get; set; } // "hotel-royal"
		public string Name { get; set; } // "Hotel Royal"
		public bool IsActive { get; set; }
	}

	public class GetPageOfHotelGroupsQuery : GetPageRequest, IRequest<PageOf<HotelGroupData>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
		public string ActiveStatusKey { get; set; }
	}

	public class GetPageOfHotelGroupsQueryHandler : IRequestHandler<GetPageOfHotelGroupsQuery, PageOf<HotelGroupData>>, IAmAdminApplicationHandler
	{
		private IMasterDatabaseContext _masterDatabaseContext;

		public GetPageOfHotelGroupsQueryHandler(IMasterDatabaseContext masterDatabaseContext)
		{
			this._masterDatabaseContext = masterDatabaseContext;
		}

		public async Task<PageOf<HotelGroupData>> Handle(GetPageOfHotelGroupsQuery request, CancellationToken cancellationToken)
		{
			var query = this._masterDatabaseContext.HotelGroupTenants
				.AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				query = query.Where(group => group.Key.ToLower().Contains(keywordsValue) || group.Name.ToLower().Contains(keywordsValue));
			}

			if (request.ActiveStatusKey.IsNotNull())
			{
				if (request.ActiveStatusKey == "ACTIVE")
				{
					query = query.Where(group => group.IsActive);
				}
				else if (request.ActiveStatusKey == "INACTIVE")
				{
					query = query.Where(group => !group.IsActive);
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
					case "KEY_ASC":
						query = query.OrderBy(group => group.Key);
						break;
					case "KEY_DESC":
						query = query.OrderByDescending(group => group.Key);
						break;
					case "NAME_DESC":
						query = query.OrderByDescending(group => group.Name);
						break;
					default:
					case "NAME_ASC":
						query = query.OrderBy(group => group.Name);
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

			var groups = await query.Select(g => new HotelGroupData 
			{
				Id = g.Id,
				IsActive = g.IsActive,
				Key = g.Key,
				Name = g.Name
			}).ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = groups.Length;
			}

			return new PageOf<HotelGroupData>
			{
				TotalNumberOfItems = count,
				Items = groups
			};
		}
	}
}
