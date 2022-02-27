using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Rooms.Queries.GetListOfAllRoomMessagesForMobile
{
	public class MobileRoomMessageDetails
	{
		public Guid Id { get; set; }
		public string Message { get; set; }
		public string CreatedByName { get; set; }
		public DateTime CreatedAt { get; set; }

		public bool IsMessageForReservation { get; set; }

		public IEnumerable<MobileMessageRoom> MessageRooms { get; set; } = Enumerable.Empty<MobileMessageRoom>();
		public IEnumerable<string> MessageReservationIds { get; set; } = Enumerable.Empty<string>();
	}

	public class MobileMessageRoom
	{
		public Guid RoomId { get; set; }
		public Guid? RoomBedId { get; set; }
	}

	public class GetListOfAllRoomMessagesForMobileQuery : IRequest<IEnumerable<MobileRoomMessageDetails>>
	{
		public string HotelId { get; set; }

	}

	public class GetListOfAllRoomMessagesForMobileQueryHandler : IRequestHandler<GetListOfAllRoomMessagesForMobileQuery, IEnumerable<MobileRoomMessageDetails>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfAllRoomMessagesForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileRoomMessageDetails>> Handle(GetListOfAllRoomMessagesForMobileQuery request, CancellationToken cancellationToken)
		{
			var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			var timeZone = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

			var messages = await this._databaseContext
				.RoomMessages
				.Include(rm => rm.CreatedBy)
				.Where(rm =>
					rm.HotelId == request.HotelId &&
					rm.RoomMessageDates.Any(rmd => rmd.Date == dateTime.Date) &&
					!rm.IsDeleted
					)
				.ToListAsync();

			var roomMessageRooms = (await this._databaseContext
				.RoomMessageRooms
				.Where(rm =>
					rm.RoomMessage.HotelId == request.HotelId &&
					rm.RoomMessage.RoomMessageDates.Any(rmd => rmd.Date == dateTime.Date) &&
					!rm.RoomMessage.IsDeleted
					)
				.ToListAsync()).GroupBy(rm => rm.RoomMessageId).ToDictionary(group => group.Key, group => group.ToArray());

			var roomMessageReservations = (await this._databaseContext
				.RoomMessageReservations
				.Include(rmr => rmr.Reservation)
				.Where(rm =>
					rm.RoomMessage.HotelId == request.HotelId &&
					rm.RoomMessage.RoomMessageDates.Any(rmd => rmd.Date == dateTime.Date) &&
					!rm.RoomMessage.IsDeleted
					)
				.ToListAsync()).GroupBy(rm => rm.RoomMessageId).ToDictionary(group => group.Key, group => group.ToArray());

			var response = new List<MobileRoomMessageDetails>();

			foreach (var message in messages)
			{
				var messageRooms = roomMessageRooms.ContainsKey(message.Id) ? roomMessageRooms[message.Id].Select(rmr => new MobileMessageRoom
				{
					RoomId = rmr.RoomId,
					RoomBedId = rmr.RoomBedId,
				}).ToList() : new List<MobileMessageRoom>();
				var reservationIds = new List<string>();

				if (roomMessageReservations.ContainsKey(message.Id))
				{
					foreach(var reservation in roomMessageReservations[message.Id])
					{
						if(reservation.Reservation == null || !reservation.Reservation.RoomId.HasValue)
						{
							continue;
						}

						reservationIds.Add(reservation.ReservationId);
						messageRooms.Add(new MobileMessageRoom
						{
							RoomBedId = reservation.Reservation.RoomBedId,
							RoomId = reservation.Reservation.RoomId.Value,
						});

					}
				}

				response.Add(new MobileRoomMessageDetails
				{
					Id = message.Id,
					CreatedAt = message.CreatedAt.Date,
					CreatedByName = $"{(message.CreatedBy?.FirstName ?? "")} {(message.CreatedBy?.LastName ?? "")}",
					Message = message.Message,
					IsMessageForReservation = message.ForType == Common.Enums.RoomMessageForType.RESERVATIONS,
					MessageRooms = messageRooms,
					MessageReservationIds = reservationIds,
				});
			}

			return response;
		}
	}
}
