using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Rcc.RoomStatuses.Queries.SendDifferentialRoomStatusChanges
{
	public class SendDifferentialRoomStatusChangesQuery : IRequest<ProcessResponse>
	{
		public string HotelGroupKey { get; set; }
		public string HotelId { get; set; }
		public IEnumerable<Guid> RoomIds { get; set; }
	}

	public class SendDifferentialRoomStatusChangesQueryHandler : IRequestHandler<SendDifferentialRoomStatusChangesQuery, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public SendDifferentialRoomStatusChangesQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(SendDifferentialRoomStatusChangesQuery request, CancellationToken cancellationToken)
		{
			var rccRoomStatusProvider = new RccRoomStatusProvider();
			var rooms = await this._databaseContext.Rooms.Where(r => r.HotelId == request.HotelId && request.RoomIds.Contains(r.Id)).ToListAsync();

			var hotelRoomStatuses = await rccRoomStatusProvider.LoadStatuses(this._databaseContext, request.HotelId, rooms);

			// TODO FINISH THIS METHOD!

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Room status differences sent.",
			};
		}
	}
}
