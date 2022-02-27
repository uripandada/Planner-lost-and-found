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


namespace Planner.Application.RoomMessages.Commands.UpdateSimpleMessage
{
	public class UpdateSimpleMessageCommand : SaveSimpleRoomMessage, IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class UpdateSimpleMessageCommandHandler : IRequestHandler<UpdateSimpleMessageCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public UpdateSimpleMessageCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}

		public async Task<ProcessResponse> Handle(UpdateSimpleMessageCommand request, CancellationToken cancellationToken)
		{
			var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			var timeZoneInfo = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
			var date = dateTime.Date;

			var message = await this._databaseContext.RoomMessages.FindAsync(request.Id);
			message.RoomMessageFilters = await this._databaseContext.RoomMessageFilters.Where(d => d.RoomMessageId == message.Id).ToListAsync();
			message.RoomMessageRooms = await this._databaseContext.RoomMessageRooms.Where(d => d.RoomMessageId == message.Id).ToListAsync();
			message.RoomMessageReservations = await this._databaseContext.RoomMessageReservations.Where(d => d.RoomMessageId == message.Id).ToListAsync();

			var generator = new RoomMessagesGenerator(this._databaseContext);
			var updatedMessage = await generator.GenerateSimpleRoomMessage(message.Id, this._userId, request, dateTime, cancellationToken);

			var diff = generator.FindDifferenceBetweenMessages(message, updatedMessage);

			this._databaseContext.RoomMessageFilters.RemoveRange(message.RoomMessageFilters);
			await this._databaseContext.RoomMessageFilters.AddRangeAsync(updatedMessage.RoomMessageFilters);
			
			//message.RoomMessageFilters = updatedMessage.RoomMessageFilters;
			
			//var roomMessageRooms = new List<Domain.Entities.RoomMessageRoom>();
			//roomMessageRooms.AddRange(diff.NewRooms);
			//roomMessageRooms.AddRange(diff.FoundRooms);
			//message.RoomMessageRooms = roomMessageRooms;

			this._databaseContext.RoomMessageRooms.RemoveRange(diff.RemovedRooms);
			await this._databaseContext.RoomMessageRooms.AddRangeAsync(diff.NewRooms);

			//var roomMessageReservations = new List<Domain.Entities.RoomMessageReservation>();
			//roomMessageReservations.AddRange(diff.NewReservations);
			//roomMessageReservations.AddRange(diff.FoundReservation);
			//message.RoomMessageReservations = roomMessageReservations;

			this._databaseContext.RoomMessageReservations.RemoveRange(diff.RemovedReservations);
			await this._databaseContext.RoomMessageReservations.AddRangeAsync(diff.NewReservations);

			message.ForType = updatedMessage.ForType;
			message.ModifiedAt = dateTime;
			message.ModifiedById = this._userId;
			message.Message = request.Message;

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
			else if (request.ReservationIds != null && request.ReservationIds.Any())
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

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Message updated."
			};
		}
	}
}
