using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.UpdateIsGuestCurrentlyIn
{
	public class UpdateIsGuestCurrentlyInCommand: IRequest<ProcessResponse>
	{
		public Guid? RoomId { get; set; }
		public Guid? BedId { get; set; }
		public bool IsGuestCurrentlyIn { get; set; }
	}

	public class UpdateIsGuestCurrentlyInCommandHandler : IRequestHandler<UpdateIsGuestCurrentlyInCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;
		private readonly ISystemEventsService _systemEventsService;

		public UpdateIsGuestCurrentlyInCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
			this._systemEventsService = systemEventsService;
		}

		public async Task<ProcessResponse> Handle(UpdateIsGuestCurrentlyInCommand request, CancellationToken cancellationToken)
		{
			var hotelId = "";
			var roomId = request.RoomId.HasValue ? request.RoomId.Value : Guid.Empty;

			if (request.BedId.HasValue)
			{
				var bed = await this._databaseContext.RoomBeds.Include(rb => rb.Room).Where(rb => rb.Id == request.BedId.Value).FirstOrDefaultAsync();
				bed.IsGuestCurrentlyIn = request.IsGuestCurrentlyIn;
				hotelId = bed.Room.HotelId;
				roomId = bed.Room.Id;
			}
			else
			{
				var room = await this._databaseContext.Rooms.FindAsync(request.RoomId.Value);
				room.IsGuestCurrentlyIn = request.IsGuestCurrentlyIn;
				hotelId = room.HotelId;
			}

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, hotelId, true);

			await this._systemEventsService.GuestCurrentlyInRoomChanged(this._hotelGroupId, roomId, request.BedId, dateTime, this._userId, request.IsGuestCurrentlyIn);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Guest status updated.",
			};
		}
	}

}
