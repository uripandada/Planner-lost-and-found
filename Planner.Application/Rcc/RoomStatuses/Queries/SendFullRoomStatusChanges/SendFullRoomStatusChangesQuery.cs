using MediatR;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Rcc.RoomStatuses.Queries.SendFullRoomStatusChanges
{
	public class SendFullRoomStatusChangesQuery : IRequest<ProcessResponse>
	{
		public string HotelGroupKey { get; set; }
	}

	public class SendFullRoomStatusChangesQueryHandler : IRequestHandler<SendFullRoomStatusChangesQuery, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public SendFullRoomStatusChangesQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(SendFullRoomStatusChangesQuery request, CancellationToken cancellationToken)
		{
			var rccRoomStatusProvider = new RccRoomStatusProvider();
			var hotelGroupStatuses = await rccRoomStatusProvider.LoadHotelGroupStatuses(this._databaseContext, request.HotelGroupKey, false, true);

			// TODO FINISH THIS METHOD!

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Room statuses sent.",
			};
		}
	}
}
