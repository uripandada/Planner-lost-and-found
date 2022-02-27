using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.Reservations.Commands.SynchronizeReservations;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Reservations.Queries.GetPageOfReservations
{
	public class ReservationGridData : ReservationData
	{
		public string HotelName { get; set; }
       
    }

	public class GetPageOfReservationsQuery : GetPageRequest, IRequest<PageOf<ReservationGridData>>
	{
		public string HotelId { get; set; }
		public string Keywords { get; set; }
		public string SortKey { get; set; }
	}

	public class GetPageOfReservationsQueryHandler : IRequestHandler<GetPageOfReservationsQuery, PageOf<ReservationGridData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetPageOfReservationsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<PageOf<ReservationGridData>> Handle(GetPageOfReservationsQuery request, CancellationToken cancellationToken)
		{
			var reservationsQuery = this._databaseContext.Reservations.Include(x=>x.Room).AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.Trim().ToLower();

				reservationsQuery = reservationsQuery.Where(rq => rq.GuestName.ToLower().Contains(keywordsValue) || rq.PMSRoomName.ToLower().Contains(keywordsValue));
			}

			if (request.HotelId.IsNotNull())
			{
				reservationsQuery = reservationsQuery.Where(r => r.HotelId == request.HotelId);
			}

			var reservationsCount = 0;

			if (request.Skip > 0 || request.Take > 0)
			{
				reservationsCount = await reservationsQuery.CountAsync();
			}

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					case "CREATED_AT_ASC":
						reservationsQuery = reservationsQuery.OrderBy(r => r.SynchronizedAt);
						break;
					case "CREATED_AT_DESC":
						reservationsQuery = reservationsQuery.OrderByDescending(r => r.SynchronizedAt);
						break;
					case "GUEST_NAME_ASC":
						reservationsQuery = reservationsQuery.OrderBy(r => r.GuestName);
						break;
					case "GUEST_NAME_DESC":
						reservationsQuery = reservationsQuery.OrderByDescending(r => r.GuestName);
						break;
					case "RESERVATION_ID_ASC":
						reservationsQuery = reservationsQuery.OrderBy(r => r.Id);
						break;
					case "RESERVATION_ID_DESC":
						reservationsQuery = reservationsQuery.OrderByDescending(r => r.Id);
						break;
				}
			}

			if (request.Skip > 0)
			{
				reservationsQuery = reservationsQuery.Skip(request.Skip);
			}

			if (request.Take > 0)
			{
				reservationsQuery = reservationsQuery.Take(request.Take);
			}

			// other properties are a jsonb field so the data must materialize before you can .Select them, exception otherwise.
			// TODO: Find out how to project JSONB fields in the select before data materialization
			var reservations = (await reservationsQuery.Include(r => r.Hotel).ToArrayAsync())
				.Select(r => new ReservationGridData
				{
					ActualCheckIn = r.ActualCheckIn,
					ActualCheckOut = r.ActualCheckOut,
					CheckIn = r.CheckIn,
					CheckOut = r.CheckOut,
					GuestName = r.GuestName,
					HotelId = r.HotelId,
					HotelName = r.Hotel.Name,
					Id = r.Id,
					IsActive = r.IsActive,
					IsSynchronizedFromRcc = r.IsSynchronizedFromRcc,
					LastTimeModifiedBySynchronization = r.LastTimeModifiedBySynchronization,
					NumberOfAdults = r.NumberOfAdults,
					NumberOfChildren = r.NumberOfChildren,
					NumberOfInfants = r.NumberOfInfants,
					OtherProperties = r.OtherProperties.Select(op => new ReservationOtherPropertyData { Key = op.Key, Value = op.Value }).ToArray(),
					PmsNote = r.PmsNote,
					PMSRoomName = r.PMSRoomName,
					RccReservationStatusKey = r.RccReservationStatusKey,
					RoomName = r.RoomName,
					RoomIsClean =  r.Room?.IsClean ?? false,
					RoomIsDoNotDisturb = r.Room?.IsDoNotDisturb ?? false,
					RoomIsOccupied = r.Room?.IsOccupied ?? false,
					RoomIsOutOfOrder = r.Room?.IsOutOfOrder ?? true,
					SynchronizedAt = r.SynchronizedAt,
					Vip = r.Vip
				})
				.ToArray();

			if (request.Skip == 0 && request.Take == 0)
			{
				reservationsCount = reservations.Length;
			}

			return new PageOf<ReservationGridData>
			{
				Items = reservations,
				TotalNumberOfItems = reservationsCount
			};
		}
	}
}
