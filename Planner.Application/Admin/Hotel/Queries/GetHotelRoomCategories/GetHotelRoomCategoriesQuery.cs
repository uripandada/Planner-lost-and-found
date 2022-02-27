using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.Hotel.Queries.GetHotelRoomCategories
{
	public class HotelRoomCategoryData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		//public int DefaultCredits { get; set; }
	}

	public class GetHotelRoomCategoriesQuery : IRequest<IEnumerable<HotelRoomCategoryData>>
	{
	}

	public class GetHotelRoomCategoriesQueryHandler : IRequestHandler<GetHotelRoomCategoriesQuery, IEnumerable<HotelRoomCategoryData>>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetHotelRoomCategoriesQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<HotelRoomCategoryData>> Handle(GetHotelRoomCategoriesQuery request, CancellationToken cancellationToken)
		{
			return await this._databaseContext
				.RoomCategorys
				.Select(rc => new HotelRoomCategoryData 
				{ 
					Id = rc.Id,
					//DefaultCredits = rc.Credits,
					Name = rc.Name
				})
				.ToArrayAsync();
		}
	}
}
