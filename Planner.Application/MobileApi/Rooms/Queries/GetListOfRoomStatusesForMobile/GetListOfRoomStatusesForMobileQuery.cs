using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomStatusesForMobile
{
	
	public class MobileRoomStatus
	{
		public Guid Id { get; set; }
		public Guid RoomId { get; set; }
		public string Label { get; set; } = "NULL status"; // label: { type: String, required: true, validate: labelValidations },
		public string Code { get; set; } = null; // code: { type: String, required: false, validate: codeValidations },
		public string Color { get; set; } = "000000"; // color: { type: String, default: '000000' },
		public string HotelId { get; set; } = Guid.Empty.ToString(); // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
	}

	/// <summary>
	/// Mobile room status really describes the status of the reservation in the room.
	/// </summary>
	public class GetListOfRoomStatusesForMobileQuery : IRequest<IEnumerable<MobileRoomStatus>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfRoomStatusesForMobileQueryHandler : IRequestHandler<GetListOfRoomStatusesForMobileQuery, IEnumerable<MobileRoomStatus>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfRoomStatusesForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileRoomStatus>> Handle(GetListOfRoomStatusesForMobileQuery request, CancellationToken cancellationToken)
		{
			var hotelLocalDateProvider = new HotelLocalDateProvider();
			var date = await hotelLocalDateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId);
			
			var rooms = await this._databaseContext.Rooms
				.Include(r => r.Reservations)
				.Where(r => r.HotelId == request.HotelId)
				.ToArrayAsync();

			return rooms.Select(r =>
			{
			 	var reservationStatus = r.CalculateReservationStatusForDate(date, r.Reservations);
				return new MobileRoomStatus
				{
					Id = r.Id,
					RoomId = r.Id,
					HotelId = request.HotelId,
					Code = reservationStatus.RccRoomStatusCode.ToString(),
					Color = reservationStatus.RccRoomStatusCode.ToHexColor(),
					Label = reservationStatus.Description,
				};
			}).ToArray();
		}
	}
}
