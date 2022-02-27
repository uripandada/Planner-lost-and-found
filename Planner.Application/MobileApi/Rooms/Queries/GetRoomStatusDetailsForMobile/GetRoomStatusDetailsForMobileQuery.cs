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

namespace Planner.Application.MobileApi.Rooms.Queries.GetRoomStatusDetailsForMobile
{
	public class MobileRoomStatusDetails
	{
		public Guid RoomId { get; set; }
		public string Label { get; set; } = "NULL status"; // label: { type: String, required: true, validate: labelValidations },
		public string Code { get; set; } = null; // code: { type: String, required: false, validate: codeValidations },
		public string Color { get; set; } = "000000"; // color: { type: String, default: '000000' },
		public string HotelId { get; set; } = Guid.Empty.ToString(); // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
	}

	public class GetRoomStatusDetailsForMobileQuery : IRequest<MobileRoomStatusDetails>
	{
		public Guid Id { get; set; }
	}

	public class GetRoomStatusDetailsForMobileQueryHandler : IRequestHandler<GetRoomStatusDetailsForMobileQuery, MobileRoomStatusDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetRoomStatusDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileRoomStatusDetails> Handle(GetRoomStatusDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			var room = await this._databaseContext.Rooms
				.Include(r => r.Reservations)
				.FirstOrDefaultAsync(r => r.Id == request.Id);
			
			var hotelLocalDateProvider = new HotelLocalDateProvider();
			var date = await hotelLocalDateProvider.GetHotelCurrentLocalDate(this._databaseContext, room.HotelId);

			var reservationStatus = room.CalculateReservationStatusForDate(date, room.Reservations);
			return new MobileRoomStatusDetails
			{
				RoomId = room.Id,
				HotelId = room.HotelId,
				Code = reservationStatus.RccRoomStatusCode.ToString(),
				Color = reservationStatus.RccRoomStatusCode.ToHexColor(),
				Label = reservationStatus.Description,
			};
		}
	}
}
