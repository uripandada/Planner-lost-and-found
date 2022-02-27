using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Rcc.RoomStatuses.Queries.GetHotelRoomStatuses
{
	public class GetHotelRoomStatusesQuery : IRequest<RccHotelRoomStatusChanges>
	{
		public string HotelGroupKey { get; set; }
		public string HotelId { get; set; }
		public bool? IncludeTemporaryRooms { get; set; }
		public bool? OnlyBedSpaces { get; set; }
	}

	public class GetHotelRoomStatusesQueryHandler : IRequestHandler<GetHotelRoomStatusesQuery, RccHotelRoomStatusChanges>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetHotelRoomStatusesQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<RccHotelRoomStatusChanges> Handle(GetHotelRoomStatusesQuery request, CancellationToken cancellationToken)
		{
			if (!this._databaseContext.DoesHotelGroupExist(request.HotelGroupKey))
			{
				return new RccHotelRoomStatusChanges
				{
					HotelId = request.HotelId,
					RoomStatuses = new RccRoomStatusData[0],
				};
			}

			this._databaseContext.SetTenantKey(request.HotelGroupKey);

			var roomsQuery = this._databaseContext.Rooms
				.Where(r => r.HotelId == request.HotelId)
				.AsQueryable();

			var includeTemporaryRooms = request.IncludeTemporaryRooms ?? false;
			if (!includeTemporaryRooms)
			{
				roomsQuery = roomsQuery.Where(r => r.FloorId != null && r.BuildingId != null);
			}

			var onlyBedSpaces = request.OnlyBedSpaces ?? true;
			if (onlyBedSpaces)
			{
				roomsQuery = roomsQuery.Where(r => r.Category != null && r.Category.IsPrivate);
			}

			var rooms = await roomsQuery.ToArrayAsync();

			var rccRoomStatusProvider = new RccRoomStatusProvider();
			return await rccRoomStatusProvider.LoadStatuses(this._databaseContext, request.HotelId, rooms);
		}
	}
}
