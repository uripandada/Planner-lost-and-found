using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Queries.GetPageOfHotelGroupHotelReservations
{
	public class HotelGroupHotelReservationData
	{
		public string Id { get; set; }
		public string RoomName { get; set; }
		public string PMSRoomName { get; set; }
		public string GuestName { get; set; }
		public DateTime? CheckIn { get; set; }
		public DateTime? CheckOut { get; set; }
		public string RccReservationStatusKey { get; set; }
		public int NumberOfAdults { get; set; }
		public int NumberOfChildren { get; set; }
		public int NumberOfInfants { get; set; }
		public string PmsNote { get; set; }
		public string Vip { get; set; }
		public IEnumerable<HotelGroupReservationOtherProperties> OtherProperties { get; set; }
		public Guid? RoomId { get; set; }
	}

	public class HotelGroupReservationOtherProperties
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class GetPageOfHotelGroupHotelReservationsQuery : GetPageRequest, IRequest<PageOf<HotelGroupHotelReservationData>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
		public string HotelId { get; set; }
	}

	public class GetPageOfHotelGroupHotelReservationsQueryHandler : IRequestHandler<GetPageOfHotelGroupHotelReservationsQuery, PageOf<HotelGroupHotelReservationData>>, IAmAdminApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		
		public GetPageOfHotelGroupHotelReservationsQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<PageOf<HotelGroupHotelReservationData>> Handle(GetPageOfHotelGroupHotelReservationsQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Reservations
				   .AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				query = query.Where(r => r.GuestName.ToLower().Contains(keywordsValue) || r.Id.ToLower().Contains(keywordsValue) || r.RoomName.ToLower().Contains(keywordsValue) || r.PMSRoomName.ToLower().Contains(keywordsValue));
			}

			if (request.HotelId.IsNotNull())
			{
				query = query.Where(r => r.HotelId == request.HotelId);
			}

			var count = 0;
			if (request.Skip > 0 || request.Take > 0)
			{
				count = await query.CountAsync();
			}

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					case "ID_DESC":
						query = query.OrderByDescending(r => r.Id);
						break;
					case "ID_ASC":
						query = query.OrderBy(r => r.Id);
						break;
					case "ROOM_NAME_DESC":
						query = query.OrderByDescending(r => r.RoomName);
						break;
					case "ROOM_NAME_ASC":
						query = query.OrderBy(r => r.RoomName);
						break;

					case "PMS_ROOM_NAME_DESC":
						query = query.OrderByDescending(r => r.PMSRoomName);
						break;
					case "PMS_ROOM_NAME_ASC":
						query = query.OrderBy(r => r.PMSRoomName);
						break;

					case "GUEST_NAME_DESC":
						query = query.OrderByDescending(r => r.GuestName);
						break;
					default:
					case "GUEST_NAME_ASC":
						query = query.OrderBy(r => r.GuestName);
						break;
				}
			}

			if (request.Skip > 0)
			{
				query = query.Skip(request.Skip);
			}

			if (request.Take > 0)
			{
				query = query.Take(request.Take);
			}

			var reservations = (await query.ToArrayAsync()).Select(r => {
				var checkInDate = r.CheckIn.HasValue ? r.CheckIn.Value : (r.ActualCheckIn.HasValue ? r.ActualCheckIn.Value : (DateTime?)null);
				var checkOutDate = r.CheckOut.HasValue ? r.CheckOut.Value : (r.ActualCheckOut.HasValue ? r.ActualCheckOut.Value : (DateTime?)null);

				return new HotelGroupHotelReservationData
				{
					Id = r.Id,
					CheckIn = checkInDate,
					CheckOut = checkOutDate,
					GuestName = r.GuestName,
					NumberOfAdults = r.NumberOfAdults,
					NumberOfChildren = r.NumberOfChildren,
					NumberOfInfants = r.NumberOfInfants,
					OtherProperties = r.OtherProperties.Select(p => new HotelGroupReservationOtherProperties 
					{ 
						Key = p.Key,
						Value = p.Value
					}).ToArray()
				};
			}).ToArray();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = reservations.Length;
			}

			return new PageOf<HotelGroupHotelReservationData>
			{
				TotalNumberOfItems = count,
				Items = reservations
			};
		}
	}
}
