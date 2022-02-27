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


namespace Planner.Application.RoomMessages.Queries.GetPageOfRoomMessages
{
	public class GetPageOfRoomMessagesQuery: GetPageRequest, IRequest<PageOf<RoomMessageListItem>>
	{
		public bool IsToday { get; set; }
		public string DateString { get; set; }
		public string HotelId { get; set; }
		public Guid RoomId { get; set; }
		public Guid? RoomBedId { get; set; }
	}

	public class GetPageOfRoomMessagesQueryHandler : IRequestHandler<GetPageOfRoomMessagesQuery, PageOf<RoomMessageListItem>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfRoomMessagesQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<RoomMessageListItem>> Handle(GetPageOfRoomMessagesQuery request, CancellationToken cancellationToken)
		{
			var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			var timeZone = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var dateTime = DateTime.UtcNow;
			if (request.IsToday)
			{
				dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
			}
			else if (request.DateString.IsNotNull())
			{
				dateTime = DateTime.ParseExact(request.DateString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
			}
			else
			{
				return new PageOf<RoomMessageListItem>
				{
					Items = new List<RoomMessageListItem>(),
					TotalNumberOfItems = 0,
				};
			}

			var query = this._databaseContext
				.RoomMessages
				.Include(rm => rm.CreatedBy)
				.Where(rm => 
					rm.HotelId == request.HotelId &&
					rm.RoomMessageDates.Any(rmd => rmd.Date == dateTime.Date) &&
					!rm.IsDeleted
					//rm.RoomMessageRooms.Any(rmr => rmr.RoomId == request.RoomId && (request.RoomBedId == null || (request.RoomBedId != null && request.RoomBedId.Value == rmr.RoomBedId)))
					)
				.AsQueryable();

			if(request.RoomBedId == null)
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

			var count = 0;
			if (request.Skip > 0 || request.Take > 0)
			{
				count = await query.CountAsync();
			}

			query = query.OrderByDescending(q => q.CreatedAt);
		

			if (request.Skip > 0)
			{
				query = query.Skip(request.Skip);
			}

			if (request.Take > 0)
			{
				query = query.Take(request.Take);
			}

			var messages = await query.ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = messages.Length;
			}

			var roomIds = new List<Guid>();
			var reservationIds= new List<Guid>();
			var messageListItems = new List<RoomMessageListItem>();
			var messageIds = new List<Guid>();

			foreach(var message in messages)
			{
				messageIds.Add(message.Id);
				messageListItems.Add(new RoomMessageListItem
				{
					Id = message.Id,
					CreatedAtString = request.DateString,
					CreatedByName = $"{(message.CreatedBy?.FirstName ?? "")} {(message.CreatedBy?.LastName ?? "")}",
					Description = "",
					Message = message.Message,
				});
			}

			if (messageIds.Any())
			{
				var messageRoomsMap = (
					await this._databaseContext
					.RoomMessageRooms
					.Where(r => 
						messageIds.Contains(r.RoomMessageId) &&
						r.RoomId == request.RoomId &&
						r.RoomBedId == request.RoomBedId &&
						r.Date == dateTime.Date
						)
					.Select(r => new
					{
						RoomMessageId = r.RoomMessageId,
						RoomId = r.RoomId,
						RoomName = r.Room == null ? "" : r.Room.Name,
						BedId = r.RoomBedId,
						BedName = r.RoomBed == null ? "" : r.RoomBed.Name,
					})
					.ToListAsync()
				)
				.GroupBy(r => r.RoomMessageId)
				.ToDictionary(group => group.Key, group => group.ToArray());
				
				var messageReservationsMap = (await this._databaseContext
					.RoomMessageReservations
					.Where(r => 
						messageIds.Contains(r.RoomMessageId) &&
						r.Reservation.RoomId == request.RoomId &&
						r.Reservation.RoomBedId == request.RoomBedId &&
						r.Date == dateTime.Date
					)
					.Select(r => new
					{
						RoomMessageId = r.RoomMessageId,
						ReservationId = r.ReservationId,
						GuestName = r.Reservation.GuestName,
						RoomId = r.Reservation.RoomId,
						RoomBedId = r.Reservation.RoomBedId,
						RoomName = r.Reservation.Room != null ? r.Reservation.Room.Name : "",
						BedName = r.Reservation.RoomBed != null ? r.Reservation.RoomBed.Name : "",
					})
					.ToListAsync())
					.GroupBy(r => r.RoomMessageId)
					.ToDictionary(group => group.Key, group => group.ToArray());

				foreach(var message in messageListItems)
				{
					var description = "";
					if (messageRoomsMap.ContainsKey(message.Id) && messageRoomsMap[message.Id].Length > 0)
					{
						var messageRooms = messageRoomsMap[message.Id];
						description += "for rooms: " + string.Join(", ", messageRooms.Select(mr => mr.RoomName + (mr.BedName.IsNotNull() ? (" - " + mr.BedName) : "")));

					}
					if (messageReservationsMap.ContainsKey(message.Id) && messageReservationsMap[message.Id].Length > 0)
					{
						var messageReservations = messageReservationsMap[message.Id];
						description += "for reservations: " + string.Join(", ", messageReservations.Select(mr => mr.GuestName + " at " + mr.RoomName + (mr.BedName.IsNotNull() ? (" - " + mr.BedName) : "")));
					}

					message.Description = description;
				}
			}

			var response = new PageOf<RoomMessageListItem>
			{
				TotalNumberOfItems = count,
				Items = messageListItems
			};

			return response;
		}
	}
}
