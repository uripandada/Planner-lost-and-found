using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupHotels
{
	public class HotelGroupHotelData
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string WindowsTimeZoneId { get; set; }
		public string IanaTimeZoneId { get; set; }
	}

	public class GetPageOfHotelGroupHotelsQuery : GetPageRequest, IRequest<PageOf<HotelGroupHotelData>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
		public string ActiveStatusKey { get; set; }
	}

	public class GetPageOfHotelGroupHotelsQueryHandler : IRequestHandler<GetPageOfHotelGroupHotelsQuery, PageOf<HotelGroupHotelData>>, IAmAdminApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public GetPageOfHotelGroupHotelsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<PageOf<HotelGroupHotelData>> Handle(GetPageOfHotelGroupHotelsQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Hotels
				.AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				query = query.Where(hotel => hotel.Name.ToLower().Contains(keywordsValue));
			}

			if (request.ActiveStatusKey.IsNotNull())
			{
				if (request.ActiveStatusKey == "ACTIVE")
				{
					// TODO: IMPLEMENT HOTEL.IS_ACTIVE FLAG!
				}
				else if (request.ActiveStatusKey == "INACTIVE")
				{
					// TODO: IMPLEMENT HOTEL.IS_ACTIVE FLAG!
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
					case "NAME_DESC":
						query = query.OrderByDescending(hotel => hotel.Name);
						break;
					default:
					case "NAME_ASC":
						query = query.OrderBy(hotel => hotel.Name);
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

			var hotels = await query.Select(hotel => new HotelGroupHotelData
			{
				Id = hotel.Id,
				Name = hotel.Name,
				IanaTimeZoneId = hotel.IanaTimeZoneId,
				WindowsTimeZoneId = hotel.WindowsTimeZoneId
			}).ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = hotels.Length;
			}

			return new PageOf<HotelGroupHotelData>
			{
				TotalNumberOfItems = count,
				Items = hotels
			};
		}
	}
}
