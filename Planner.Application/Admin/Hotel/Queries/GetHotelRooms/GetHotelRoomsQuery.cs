using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.Hotel.Queries.GetHotelRooms
{
	public class HotelRoomData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Section { get; set; }
		public string SubSection { get; set; }
		public string Floor { get; set; }
		public string Building { get; set; }
	}

	public class GetHotelRoomsQuery : IRequest<IEnumerable<HotelRoomData>>
	{
		public string HotelId { get; set; }
	}
	public class GetHotelRoomsQueryHandler : IRequestHandler<GetHotelRoomsQuery, IEnumerable<HotelRoomData>>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetHotelRoomsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<HotelRoomData>> Handle(GetHotelRoomsQuery request, CancellationToken cancellationToken)
		{
			return await this._databaseContext
				.Rooms
				.Where(r => r.HotelId == request.HotelId)
				.Select(r => new HotelRoomData
				{
					Id = r.Id,
					Name = r.Name,
					Building = r.Building.Name,
					Floor = r.Floor.Name,
					Section = r.FloorSectionName,
					SubSection = r.FloorSubSectionName
				})
				.ToArrayAsync();
		}
	}
}
