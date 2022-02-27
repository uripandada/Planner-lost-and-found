using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomHousekeepingStatusesForMobile
{
	public class MobileRoomHousekeepingStatus
	{
		public Guid Id { get; set; }
		public Guid RoomId { get; set; }
		public string Label { get; set; } = null; // label: { type: String, required: true, validate: labelValidations },
		public string Code { get; set; } = null; // code: { type: String, required: false, validate: codeValidations },
		public string Color { get; set; } = "000000"; // color: { type: String, default: '000000' },
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
	}

	public class GetListOfRoomHousekeepingStatusesForMobileQuery: IRequest<IEnumerable<MobileRoomHousekeepingStatus>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfRoomHousekeepingsForMobileQueryHandler : IRequestHandler<GetListOfRoomHousekeepingStatusesForMobileQuery, IEnumerable<MobileRoomHousekeepingStatus>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfRoomHousekeepingsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileRoomHousekeepingStatus>> Handle(GetListOfRoomHousekeepingStatusesForMobileQuery request, CancellationToken cancellationToken)
		{
			var rooms = await this._databaseContext.Rooms
				.Include(r => r.Reservations)
				.Where(r => r.HotelId == request.HotelId)
				.ToArrayAsync();

			var colors = await this._databaseContext.RccHousekeepingStatusColors.ToDictionaryAsync(c => c.RccCode);
			var rccHousekeepingStatuses = Enum.GetValues(typeof(RccHousekeepingStatusCode)).Cast<RccHousekeepingStatusCode>();
			foreach (var status in rccHousekeepingStatuses)
			{
				if (colors.ContainsKey(status))
				{
					continue;
				}
				if (COLORS.ROOM_HOUSEKEEPING_STATUSES.ContainsKey(status))
				{
					colors.Add(status, new RccHousekeepingStatusColor { RccCode = status, ColorHex = COLORS.ROOM_HOUSEKEEPING_STATUSES[status] });
					continue;
				}
				colors.Add(status, new RccHousekeepingStatusColor { RccCode = status, ColorHex = "454545" });
			}

			return rooms.Select(room =>
			{
				var housekeepingStatus = room.CalculateCurrentHousekeepingStatus();
				return new MobileRoomHousekeepingStatus
				{
					Id = room.Id,
					RoomId = room.Id,
					HotelId = request.HotelId,
					Code = housekeepingStatus.RccHousekeepingStatusCode.ToString(),
					Color = colors[housekeepingStatus.RccHousekeepingStatusCode].ColorHex,
					Label = COLORS.ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS.ContainsKey(housekeepingStatus.RccHousekeepingStatusCode) ? COLORS.ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS[housekeepingStatus.RccHousekeepingStatusCode] : housekeepingStatus.Description,
				};
			}).ToArray();
		}
	}
}
