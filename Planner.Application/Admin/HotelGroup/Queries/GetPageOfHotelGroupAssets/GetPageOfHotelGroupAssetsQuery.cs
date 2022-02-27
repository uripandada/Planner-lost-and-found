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

namespace Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupAssets
{
	public class HotelGroupAssetData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		//public bool IsAvailableToMaintenance { get; set; }
		//public bool IsAvailableToHousekeeping { get; set; }
		public int AvailableQuantity { get; set; }
		public bool UsesModels { get; set; }
		public IEnumerable<HotelGroupAssetModelData> Models { get; set; }
	}

	public class HotelGroupAssetModelData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class GetPageOfHotelGroupAssetsQuery : GetPageRequest, IRequest<PageOf<HotelGroupAssetData>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
		public string ActiveStatusKey { get; set; }
	}

	public class GetPageOfHotelGroupAssetsQueryHandler : IRequestHandler<GetPageOfHotelGroupAssetsQuery, PageOf<HotelGroupAssetData>>, IAmAdminApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		public GetPageOfHotelGroupAssetsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}
		public async Task<PageOf<HotelGroupAssetData>> Handle(GetPageOfHotelGroupAssetsQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Assets
				   .AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				query = query.Where(asset => asset.Name.ToLower().Contains(keywordsValue));
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
					case "NAME_DESC":
						query = query.OrderByDescending(asset => asset.Name);
						break;
					default:
					case "NAME_ASC":
						query = query.OrderBy(asset => asset.Name);
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

			var assets = await query.Select(asset => new HotelGroupAssetData
			{
				Id = asset.Id,
				Name = asset.Name,
				AvailableQuantity = 0,
				UsesModels = false,
				Models = new HotelGroupAssetModelData[0],
			}).ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = assets.Length;
			}

			return new PageOf<HotelGroupAssetData>
			{
				TotalNumberOfItems = count,
				Items = assets
			};
		}
	}
}
