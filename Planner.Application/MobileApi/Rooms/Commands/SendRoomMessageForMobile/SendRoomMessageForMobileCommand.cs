using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.RoomMessages;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Rooms.Commands.SendRoomMessageForMobile
{
	public class SendRoomMessageForMobileCommand: IRequest<ProcessResponse<Guid>>
	{
		/// <summary>
		/// ROOM, ROOM_BED, RESERVATION
		/// </summary>
		public string ForKey { get; set; }
		public string Message { get; set; }
		public Guid? RoomId { get; set; }
		public Guid? RoomBedId { get; set; }
		public string[] ReservationIds { get; set; }
		public DateTime CreatedAt { get; set; }
		public string HotelId { get; set; }

	}

	public class SendRoomMessageForMobileCommandHandler: IRequestHandler<SendRoomMessageForMobileCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public SendRoomMessageForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}

		public async Task<ProcessResponse<Guid>> Handle(SendRoomMessageForMobileCommand request, CancellationToken cancellationToken)
		{
			var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			var timeZoneInfo = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
			var date = dateTime.Date;

			var generator = new RoomMessagesGenerator(this._databaseContext);
			var message = await generator.GenerateSimpleRoomMessage(Guid.NewGuid(), this._userId, new SaveSimpleRoomMessage 
			{
				ForType = request.ForKey == "RESERVATION" ? Common.Enums.RoomMessageForType.RESERVATIONS : Common.Enums.RoomMessageForType.TODAY,
				HotelId = hotel.Id,
				Message = request.Message,
				ReservationIds = request.ForKey == "RESERVATION" ? request.ReservationIds : new string[0],
				RoomId = request.ForKey == "RESERVATION" ? null : request.RoomId.Value,
			}, dateTime, cancellationToken);

			await this._databaseContext.RoomMessages.AddAsync(message);

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			var roomIds = message.RoomMessageRooms.Where(rmr => rmr.Date == date).Select(rmr => rmr.RoomId).Distinct().ToArray();
			var reservationIds = message.RoomMessageReservations.Where(rmr => rmr.Date == date).Select(rmr => rmr.ReservationId).Distinct().ToArray();
			var userIds = new List<Guid>();

			if (request.RoomId.HasValue)
			{
				userIds = await this._databaseContext
					.Cleanings
					.Where(c => c.IsActive && c.CleaningPlan != null && c.CleaningPlan.Date == date && c.RoomId == request.RoomId.Value)
					.Select(c => c.CleanerId)
					.Distinct()
					.ToListAsync();
			}
			else if (request.ReservationIds != null && request.ReservationIds.Length > 0)
			{
				// TODO: MISSING ROOM BEDS
				userIds = await this._databaseContext
					.Cleanings
					.Where(c => c.IsActive && c.CleaningPlan != null && c.CleaningPlan.Date == date && c.Room.Reservations.Any(r => request.ReservationIds.Contains(r.Id)))
					.Select(c => c.CleanerId)
					.Distinct()
					.ToListAsync();
			}

			await this._systemEventsService.RoomMessagesChanged(this._hotelGroupId, roomIds, reservationIds, userIds);

			return new ProcessResponse<Guid>
			{
				Data = message.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Message sent.",
			};
		}
	}
}
