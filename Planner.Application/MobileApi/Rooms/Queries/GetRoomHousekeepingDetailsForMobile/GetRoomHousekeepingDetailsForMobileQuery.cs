using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
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

namespace Planner.Application.MobileApi.Rooms.Queries.GetRoomHousekeepingDetailsForMobile
{
	public class MobileRoomHousekeepingStatusDetails
	{
		public Guid RoomId { get; set; }
		public string Label { get; set; } = null; // label: { type: String, required: true, validate: labelValidations },
		public string Code { get; set; } = null; // code: { type: String, required: false, validate: codeValidations },
		public string Color { get; set; } = "000000"; // color: { type: String, default: '000000' },
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
	}

	public class GetRoomHousekeepingStatusDetailsForMobileQuery: IRequest<MobileRoomHousekeepingStatusDetails>
	{
		public Guid Id { get; set; }
	}

	public class GetRoomHousekeepingStatusDetailsForMobileQueryHandler : IRequestHandler<GetRoomHousekeepingStatusDetailsForMobileQuery, MobileRoomHousekeepingStatusDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetRoomHousekeepingStatusDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileRoomHousekeepingStatusDetails> Handle(GetRoomHousekeepingStatusDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			var room = await this._databaseContext.Rooms
				.Include(r => r.Reservations)
				.FirstOrDefaultAsync(r => r.Id == request.Id);
			
			var housekeepingStatus = room.CalculateCurrentHousekeepingStatus();
			var color = await this._databaseContext.RccHousekeepingStatusColors.FirstOrDefaultAsync(c => c.RccCode == housekeepingStatus.RccHousekeepingStatusCode);
			if (color == null && COLORS.ROOM_HOUSEKEEPING_STATUSES.ContainsKey(housekeepingStatus.RccHousekeepingStatusCode))
			{
				color = new RccHousekeepingStatusColor 
				{ 
					RccCode = housekeepingStatus.RccHousekeepingStatusCode, 
					ColorHex = COLORS.ROOM_HOUSEKEEPING_STATUSES[housekeepingStatus.RccHousekeepingStatusCode] 
				};
			}
			if(color == null)
			{
				color = new RccHousekeepingStatusColor { RccCode = housekeepingStatus.RccHousekeepingStatusCode, ColorHex = "454545" };
			}

			return new MobileRoomHousekeepingStatusDetails
			{
				RoomId = room.Id,
				HotelId = room.HotelId,
				Code = housekeepingStatus.RccHousekeepingStatusCode.ToString(),
				Color = color.ColorHex,
				Label = COLORS.ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS.ContainsKey(housekeepingStatus.RccHousekeepingStatusCode) ? COLORS.ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS[housekeepingStatus.RccHousekeepingStatusCode] : housekeepingStatus.Description,
			};
			
		}
	}
}
