using MediatR;
using Planner.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Rcc.RoomStatuses.Queries.GetHotelGroupRoomStatuses
{
	public class GetHotelGroupRoomStatusesQuery: IRequest<RccHotelGroupRoomStatusChanges>
	{
		public string HotelGroupKey { get; set; }
		public bool? IncludeTemporaryRooms { get; set; }
		public bool? OnlyBedSpaces { get; set; }
	}

	public class GetHotelGroupRoomStatusesQueryHandler : IRequestHandler<GetHotelGroupRoomStatusesQuery, RccHotelGroupRoomStatusChanges>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetHotelGroupRoomStatusesQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<RccHotelGroupRoomStatusChanges> Handle(GetHotelGroupRoomStatusesQuery request, CancellationToken cancellationToken)
		{
			var rccRoomStatusProvider = new RccRoomStatusProvider();
			return await rccRoomStatusProvider.LoadHotelGroupStatuses(this._databaseContext, request.HotelGroupKey, request.IncludeTemporaryRooms ?? false, request.OnlyBedSpaces ?? true);
		}
	}
}
