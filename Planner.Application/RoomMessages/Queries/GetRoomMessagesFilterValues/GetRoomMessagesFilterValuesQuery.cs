using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Planner.Application.RoomMessages.Queries.GetRoomMessagesFilterValues
{
	public class GetRoomMessagesFilterValuesQuery : IRequest<RoomMessageFilterValues>
	{
		public string HotelId { get; set; }
	}

	public class GetRoomMessagesFilterValuesQueryHandler : IRequestHandler<GetRoomMessagesFilterValuesQuery, RoomMessageFilterValues>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetRoomMessagesFilterValuesQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<RoomMessageFilterValues> Handle(GetRoomMessagesFilterValuesQuery request, CancellationToken cancellationToken)
		{
			var reservations = await this._databaseContext
				.Reservations
				.Where(r => r.IsActive)
				.Select(r => new 
				{ 
					r.Id,
					r.GuestName,
					r.CheckIn,
					r.CheckOut,
					r.RccReservationStatusKey,
					r.ReservationStatusKey,
					r.ReservationStatusDescription,
				})
				.Distinct()
				.ToArrayAsync();

			var rooms = await this._databaseContext
				.Rooms
				.Where(r => r.HotelId == request.HotelId)
				.OrderBy(r => r.OrdinalNumber)
				.Select(r => new
				{
					r.Id,
					r.Name,
					r.BuildingId,
					r.FloorId,
					r.FloorSectionName,
					r.FloorSubSectionName,
					r.OrdinalNumber,
				})
				.ToArrayAsync();

			var floorsMap = await this._databaseContext
				.Floors
				.Where(f => f.HotelId == request.HotelId)
				.Select(f => new
				{
					f.Id,
					f.Name,
					f.Number,
					f.BuildingId,
				})
				.OrderBy(f => f.Number)
				.ToDictionaryAsync(f => f.Id, f => f);

			var buildingsMap = await this._databaseContext
				.Buildings
				.Where(b => b.HotelId == request.HotelId)
				.Select(b => new
				{
					b.Id,
					b.Name,
					b.TypeKey,
				})
				.OrderBy(b => b.Name)
				.ToDictionaryAsync(b => b.Id, b => b);

			var roomCategories = await this._databaseContext
				.RoomCategorys
				.Select(rc => new
				{
					rc.Id,
					rc.Name,
				})
				.ToArrayAsync();

			var reservationFilterValues = reservations.OrderBy(r => r.GuestName).Select(r => new RoomMessageFilterGroupItem
			{
				Id = r.Id,
				Name = r.GuestName,
				Description = "Reservation " + r.Id,
			}).ToList();
			var buildingFilterValues = new List<RoomMessageFilterGroupItem>();
			var floorFilterValues = new List<RoomMessageFilterGroupItem>();
			var floorSectionFilterValues = new List<RoomMessageFilterGroupItem>();
			var floorSubSectionFilterValues = new List<RoomMessageFilterGroupItem>();
			var roomFilterValues = new List<RoomMessageFilterGroupItem>();
			var roomCategoryFilterValues = new List<RoomMessageFilterGroupItem>();
			var guestStatusFilterValues = new List<RoomMessageFilterGroupItem>()
			{
				new RoomMessageFilterGroupItem
				{
					Id = "VACANT",
					Name = "Vacant",
					Description = "Guest status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "OCCUPIED",
					Name = "Occupied",
					Description = "Guest status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "STAY",
					Name = "Stay",
					Description = "Guest status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "ARRIVAL",
					Name = "Arrival",
					Description = "Guest status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "ARRIVED",
					Name = "Arrived",
					Description = "Guest status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "DEPARTURE",
					Name = "Departure",
					Description = "Guest status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "DEPARTED",
					Name = "Departed",
					Description = "Guest status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "ALL_ARRIVALS",
					Name = "All arrivals",
					Description = "Guest status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "ALL_DEPARTURES",
					Name = "All departures",
					Description = "Guest status",
				},
			};
			var clenlinessStatusFilterValues = new List<RoomMessageFilterGroupItem>()
			{
				new RoomMessageFilterGroupItem
				{
					Id = "DIRTY",
					Name = "Dirty",
					Description = "Clenliness",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "CLEAN",
					Name = "Clean",
					Description = "Clenliness",
				},
			};
			var housekeepingStatusFilterValues = new List<RoomMessageFilterGroupItem>()
			{
				new RoomMessageFilterGroupItem
				{
					Id = "ANY",
					Name = "Any",
					Description = "Housekeeping status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "NEW",
					Name = "New",
					Description = "Housekeeping status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "IN_PROGRESS",
					Name = "In progress",
					Description = "Housekeeping status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "PAUSED",
					Name = "Paused",
					Description = "Housekeeping status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "FINISHED",
					Name = "Finished",
					Description = "Housekeeping status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "INSPECTED",
					Name = "Inspected",
					Description = "Housekeeping status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "DND",
					Name = "Do not disturb",
					Description = "Housekeeping status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "REFUSED",
					Name = "Refused",
					Description = "Housekeeping status",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "DELAYED",
					Name = "Delayed",
					Description = "Housekeeping status",
				},
			};
			var pmsFilterValues = new List<RoomMessageFilterGroupItem>()
			{
				new RoomMessageFilterGroupItem
				{
					Id = "VIP",
					Name = "VIP",
					Description = "PMS",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "NOTE",
					Name = "Note",
					Description = "PMS",
				},
			};
			var otherFilterValues = new List<RoomMessageFilterGroupItem>()
			{
				new RoomMessageFilterGroupItem
				{
					Id = "CHANGE_SHEETS",
					Name = "Change sheets",
					Description = "Others",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "PRIORITY",
					Name = "Priority",
					Description = "Others",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "OUT_OF_SERVICE",
					Name = "Out of service",
					Description = "Others",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "OUT_OF_ORDER",
					Name = "Out of order",
					Description = "Others",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "GUEST_IS_IN_THE_ROOM",
					Name = "Guest is in the room",
					Description = "Others",
				},
				new RoomMessageFilterGroupItem
				{
					Id = "GUEST_IS_NOT_IN_THE_ROOM",
					Name = "Guest is not in the room",
					Description = "Others",
				},
			};

			foreach (var category in roomCategories.OrderBy(rc => rc.Name).ToArray())
			{
				roomCategoryFilterValues.Add(new RoomMessageFilterGroupItem
				{
					Id = category.Id.ToString(),
					Description = "Room category",
					Name = category.Name,
				});
			}

			foreach (var building in buildingsMap.Values.OrderBy(b => b.Name).ToArray())
			{
				buildingFilterValues.Add(new RoomMessageFilterGroupItem
				{
					Id = building.Id.ToString(),
					Description = "Building",
					Name = building.Name,
				});
			}

			var floorSectionsMap = new Dictionary<string, HashSet<string>>();
			foreach (var floor in floorsMap.Values.OrderBy(f => f.Number).ToArray())
			{
				floorFilterValues.Add(new RoomMessageFilterGroupItem
				{
					Id = floor.Id.ToString(),
					Description = $"Floor in {buildingsMap[floor.BuildingId].Name}",
					Name = floor.Name,
				});
			}

			foreach (var room in rooms.OrderBy(r => r.OrdinalNumber).ToArray())
			{
				var isTemporaryRoom = false;
				var roomFilterValue = new RoomMessageFilterGroupItem
				{
					Name = room.Name,
					Id = room.Id.ToString(),
				};

				if (!room.BuildingId.HasValue || !room.FloorId.HasValue)
				{
					isTemporaryRoom = true;
					roomFilterValue.Description = "Temporary room";
				}
				else
				{
					roomFilterValue.Description = $"Room on {floorsMap[room.FloorId.Value].Name} in {buildingsMap[room.BuildingId.Value].Name}";
				}
				roomFilterValues.Add(roomFilterValue);

				if (room.FloorSectionName.IsNotNull())
				{
					if (!floorSectionsMap.ContainsKey(room.FloorSectionName))
					{
						floorSectionsMap.Add(room.FloorSectionName, new HashSet<string>());

						floorSectionFilterValues.Add(new RoomMessageFilterGroupItem
						{
							Id = room.FloorSectionName,
							Name = room.FloorSectionName,
							Description = isTemporaryRoom ? "Section" : $"Section of {floorsMap[room.FloorId.Value].Name} in {buildingsMap[room.BuildingId.Value].Name}",
						});
					}

					if (room.FloorSubSectionName.IsNotNull())
					{
						if (!floorSectionsMap[room.FloorSectionName].Contains(room.FloorSubSectionName))
						{
							floorSectionsMap[room.FloorSectionName].Add(room.FloorSubSectionName);

							floorSubSectionFilterValues.Add(new RoomMessageFilterGroupItem
							{
								Id = $"{room.FloorSectionName}|{room.FloorSubSectionName}",
								Name = room.FloorSubSectionName,
								Description = $"Subsection of {room.FloorSectionName}{(isTemporaryRoom ? "" : $" of {floorsMap[room.FloorId.Value].Name} in {buildingsMap[room.BuildingId.Value].Name}")}"
							});
						}
					}
				}
			}

			floorSectionFilterValues = floorSectionFilterValues.OrderBy(fs => fs.Name).ToList();
			floorSubSectionFilterValues = floorSubSectionFilterValues.OrderBy(fss => fss.Name).ToList();

			return new RoomMessageFilterValues
			{
				TodayFilterValues = new List<RoomMessageFilterGroup>
				{
					new RoomMessageFilterGroup { Name = "Guest statuses", Items = guestStatusFilterValues, ReferenceType = RoomMessageFilterReferenceType.GUEST_STATUSES },
					new RoomMessageFilterGroup { Name = "Cleanliness ", Items = clenlinessStatusFilterValues, ReferenceType = RoomMessageFilterReferenceType.CLENLINESS },
					new RoomMessageFilterGroup { Name = "Housekeeping statuses", Items = housekeepingStatusFilterValues, ReferenceType = RoomMessageFilterReferenceType.HOUSEKEEPING_STATUSES },
					new RoomMessageFilterGroup { Name = "Others", Items = otherFilterValues, ReferenceType = RoomMessageFilterReferenceType.OTHERS },
					new RoomMessageFilterGroup { Name = "PMS", Items = pmsFilterValues, ReferenceType = RoomMessageFilterReferenceType.PMS },
				},
				PlacesFilterValues = new List<RoomMessageFilterGroup>
				{
					new RoomMessageFilterGroup { Name = "Room categories", Items = roomCategoryFilterValues, ReferenceType = RoomMessageFilterReferenceType.ROOM_CATEGORIES },
					new RoomMessageFilterGroup { Name = "Buildings", Items = buildingFilterValues, ReferenceType = RoomMessageFilterReferenceType.BUILDINGS },
					new RoomMessageFilterGroup { Name = "Floors", Items = floorFilterValues, ReferenceType = RoomMessageFilterReferenceType.FLOORS },
					new RoomMessageFilterGroup { Name = "Floor sections", Items = floorSectionFilterValues, ReferenceType = RoomMessageFilterReferenceType.FLOOR_SECTIONS },
					new RoomMessageFilterGroup { Name = "Floor subsections", Items = floorSubSectionFilterValues, ReferenceType = RoomMessageFilterReferenceType.FLOOR_SUB_SECTIONS },
					new RoomMessageFilterGroup { Name = "Rooms", Items = roomFilterValues, ReferenceType = RoomMessageFilterReferenceType.ROOMS },
				},
				ReservationsFilterValues = new List<RoomMessageFilterGroup>
				{
					new RoomMessageFilterGroup { Name = "Reservations", Items = reservationFilterValues, ReferenceType = RoomMessageFilterReferenceType.RESERVATIONS },
				}
			};
		}
	}
}
