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

namespace Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomMessagesForMobile
{
	public class MobileRoomMessage
	{
		public Guid Id { get; set; }
		public string Message { get; set; }
		public string CreatedByName { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class GetListOfRoomMessagesForMobileQuery: IRequest<IEnumerable<MobileRoomMessage>>
	{
		public string HotelId { get; set; }
		public Guid RoomId { get; set; }
		public Guid? RoomBedId { get; set; }
	}

	public class GetListOfRoomMessagesForMobileQueryHandler : IRequestHandler<GetListOfRoomMessagesForMobileQuery, IEnumerable<MobileRoomMessage>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfRoomMessagesForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileRoomMessage>> Handle(GetListOfRoomMessagesForMobileQuery request, CancellationToken cancellationToken)
		{
			var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			var timeZone = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
			
			var query = this._databaseContext
				.RoomMessages
				.Include(rm => rm.CreatedBy)
				.Where(rm =>
					rm.HotelId == request.HotelId &&
					rm.RoomMessageDates.Any(rmd => rmd.Date == dateTime.Date) &&
					!rm.IsDeleted
					)
				.AsQueryable();

			if (request.RoomBedId == null)
			{
				query = query.Where(rm =>
					rm.RoomMessageRooms.Any(rmr => rmr.RoomId == request.RoomId)
					||
					rm.RoomMessageReservations.Any(rmr => rmr.Reservation.RoomId == request.RoomId && rmr.Reservation.RoomBedId == request.RoomBedId)
				);
			}
			else
			{
				query = query.Where(rm =>
					rm.RoomMessageRooms.Any(rmr => rmr.RoomId == request.RoomId && rmr.RoomBedId == request.RoomBedId)
					||
					rm.RoomMessageReservations.Any(rmr => rmr.Reservation.RoomId == request.RoomId && rmr.Reservation.RoomBedId == request.RoomBedId)
				);
			}


			var messages = await query.ToArrayAsync();

			var response = new List<MobileRoomMessage>();
			foreach (var message in messages)
			{
				response.Add(new MobileRoomMessage
				{
					Id = message.Id,
					CreatedAt = message.CreatedAt.Date,
					CreatedByName = $"{(message.CreatedBy?.FirstName ?? "")} {(message.CreatedBy?.LastName ?? "")}",
					Message = message.Message,
				});
			}

			return response;
		}
	}
}
