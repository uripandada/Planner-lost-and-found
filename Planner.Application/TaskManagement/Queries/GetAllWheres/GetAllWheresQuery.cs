using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetAllWheres
{
	public class ExtendedWhereData: TaskWhereData
	{
		public string HotelId { get; set; }
		public string GuestName { get; set; }				
		public string RoomName { get; set; }
	}

	public class GetAllWheresQuery: IRequest<IEnumerable<ExtendedWhereData>>
	{
		public bool IncludeReservationsWithoutRooms { get; set; }
		public bool IgnoreUnAllocatedReservations { get; set; }

		public bool IgnoreBuildingsMap { get; set; }

		public bool IgnoreWarehouses { get; set; }
		public bool IgnoreTemporaryRooms { get; set; }
		public bool IgnoreFutureReservations { get; set; }
	}

	public class GetAllWheresQueryHandler: IRequestHandler<GetAllWheresQuery, IEnumerable<ExtendedWhereData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetAllWheresQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<ExtendedWhereData>> Handle(GetAllWheresQuery request, CancellationToken cancellationToken)
		{
			var roomsQuery = this._databaseContext.Rooms.Include(r => r.RoomBeds).AsQueryable();

			if (request.IgnoreTemporaryRooms)
			{
				roomsQuery = roomsQuery.Where(r => r.BuildingId.HasValue && r.FloorId.HasValue);
			}

			var rooms = await roomsQuery.ToListAsync();
			var buildingsMap = (await this._databaseContext.Buildings.Include(b => b.Floors).ToListAsync()).ToDictionary(b => b.Id, b => b);
			var hotelsMap = (await this._databaseContext.Hotels.ToListAsync()).ToDictionary(h => h.Id, h => h);
			var warehouses = await this._databaseContext.Warehouses.ToListAsync();

			var reservationsQuery = this._databaseContext.Reservations.Where(r => r.IsActive).AsQueryable();//.Select(r => new { ReservationId = r.Id, r.GuestName, StatusKey = r.RccReservationStatusKey, RoomName = r.Room.Name, HotelId = r.HotelId }).ToArrayAsync();
			if (!request.IncludeReservationsWithoutRooms)
			{
				reservationsQuery = reservationsQuery.Where(r => r.RoomId != null);
			}					
			
			if (request.IgnoreFutureReservations)            			
			{								
				reservationsQuery = reservationsQuery.Where(r => r.CheckIn < DateTime.Today && r.CheckOut >= DateTime.Today);				
				reservationsQuery = reservationsQuery.Where(r => r.Room.BuildingId.HasValue && r.Room.FloorId.HasValue);		
			}

			var reservations = await reservationsQuery.Select(r => new { ReservationId = r.Id, r.GuestName, StatusKey = r.RccReservationStatusKey, RoomName = r.Room.Name, HotelId = r.HotelId, r.CheckIn, r.CheckOut }).ToArrayAsync();

			var wheres = new List<ExtendedWhereData>();

			if (request.IgnoreUnAllocatedReservations)
			{
				reservations = reservations.Where(r => !string.IsNullOrEmpty(r.RoomName)).ToArray();
			}

			foreach (var room in rooms.OrderBy(b => b.HotelId).ThenBy(b => b.OrdinalNumber).ToArray())
			{
				var hotelName = hotelsMap.ContainsKey(room.HotelId) ? hotelsMap[room.HotelId].Name : "Unknown hotel";
				var buildingName = room.BuildingId.HasValue && buildingsMap.ContainsKey(room.BuildingId.Value) ? buildingsMap[room.BuildingId.Value].Name : "Unknown building";

				wheres.Add(new ExtendedWhereData
				{
					ReferenceId = room.Id.ToString(),
					ReferenceName = $"{room.Name}",
					TypeDescription = $"Room - {buildingName}, {hotelName}",
					TypeKey = TaskWhereType.ROOM.ToString(),
					HotelId = room.HotelId,
					GuestName = "",										
					RoomName = room.Name
				});

				if (room.TypeKey == RoomTypeEnum.HOSTEL.ToString() && room.RoomBeds != null)
				{
					foreach(var roomBed in room.RoomBeds)
					{
						wheres.Add(new ExtendedWhereData
						{
							ReferenceId = roomBed.Id.ToString(),
							ReferenceName = $"{roomBed.Name}",
							TypeDescription = $"Bed - {room.Name}, {hotelName}",
							TypeKey = TaskWhereType.BED.ToString(),
							HotelId = room.HotelId,
							GuestName = "",														
							RoomName = room.Name
						});
					}
				}
			}

			if (!request.IgnoreWarehouses)
			{
				foreach (var warehouse in warehouses.OrderBy(b => b.HotelId).ThenBy(b => b.Name).ToArray())
				{
					wheres.Add(new ExtendedWhereData
					{
						ReferenceId = warehouse.Id.ToString(),
						ReferenceName = $"{warehouse.Name}",
						TypeDescription = $"Warehouse",
						TypeKey = TaskWhereType.WAREHOUSE.ToString(),
						HotelId = warehouse.HotelId,
					});
				}
			}

			if (!request.IgnoreBuildingsMap)
			{
				var addedHotelsIdSet = new HashSet<string>();
				foreach (var building in buildingsMap.Values.OrderBy(b => b.HotelId).ThenBy(b => b.Name).ToArray())
				{
					var hotelName = hotelsMap.ContainsKey(building.HotelId) ? hotelsMap[building.HotelId].Name : "Unknown hotel";

					if (!addedHotelsIdSet.Contains(building.HotelId))
					{
						wheres.Add(new ExtendedWhereData
						{
							ReferenceId = building.HotelId,
							ReferenceName = hotelName,
							TypeDescription = $"Hotel",
							TypeKey = TaskWhereType.HOTEL.ToString(),
							HotelId = building.HotelId,
						});
					}

					wheres.Add(new ExtendedWhereData
					{
						ReferenceId = building.Id.ToString(),
						ReferenceName = building.Name,
						TypeDescription = $"Building - {hotelName}",
						TypeKey = TaskWhereType.BUILDING.ToString(),
						HotelId = building.HotelId,
					});

					foreach (var floor in building.Floors.OrderBy(f => f.Number).ToArray())
					{
						wheres.Add(new ExtendedWhereData
						{
							ReferenceId = floor.Id.ToString(),
							ReferenceName = $"{floor.Name} {floor.Number}",
							TypeDescription = $"Floor - {building.Name}, {hotelName}",
							TypeKey = TaskWhereType.FLOOR.ToString(),
							HotelId = floor.HotelId,
						});
					}
				}
			}

			foreach (var reservation in reservations)
			{
				var hotelName = hotelsMap.ContainsKey(reservation.HotelId) ? hotelsMap[reservation.HotelId].Name : "Unknown hotel";
				wheres.Add(new ExtendedWhereData
				{
					ReferenceId = reservation.ReservationId,
					ReferenceName = $"{reservation.GuestName} [{reservation.RoomName}]",
					TypeDescription = $"{(reservation.CheckIn.HasValue ? reservation.CheckIn.Value.ToString("dddd dd MMM") : "?")} - {(reservation.CheckOut.HasValue ? reservation.CheckOut.Value.ToString("dddd dd MMM") : "?")} at {hotelName}",
					TypeKey = TaskWhereType.RESERVATION.ToString(),
					HotelId = reservation.HotelId,
					GuestName = reservation.GuestName,										
					RoomName = reservation.RoomName
				});
			}

			return wheres;
		}
	}
}
