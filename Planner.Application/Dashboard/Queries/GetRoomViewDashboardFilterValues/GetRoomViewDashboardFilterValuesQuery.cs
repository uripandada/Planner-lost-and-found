using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Dashboard.Queries.GetRoomViewDashboardFilterValues
{
	public class MasterFilterGroup
	{
		public MasterFilterGroupType Type { get; set; }
		public string Name { get; set; }
		public IEnumerable<MasterFilterGroupItem> Items { get; set; }
	}

	public class MasterFilterGroupItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public class GetRoomViewDashboardFilterValuesQuery: IRequest<IEnumerable<MasterFilterGroup>>
	{
		public string HotelId { get; set; }
		public MasterFilterType Type { get; set; }
	}

	public class GetRoomViewDashboardFilterValuesQueryHandler : IRequestHandler<GetRoomViewDashboardFilterValuesQuery, IEnumerable<MasterFilterGroup>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetRoomViewDashboardFilterValuesQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<MasterFilterGroup>> Handle(GetRoomViewDashboardFilterValuesQuery request, CancellationToken cancellationToken)
		{
			var guestNames = await this._databaseContext.Reservations.Where(r => r.IsActive && r.IsActiveToday).Select(r => r.GuestName).Distinct().ToArrayAsync();

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

			var guestFilterValues = guestNames.Where(guestName => guestName.IsNotNull()).OrderBy(guestName => guestName).Select(guestName => new MasterFilterGroupItem 
			{ 
				Id = guestName,
				Name = guestName,
				Description = "Guest",
			}).ToList();
			var buildingFilterValues = new List<MasterFilterGroupItem>();
			var floorFilterValues = new List<MasterFilterGroupItem>();
			var floorSectionFilterValues = new List<MasterFilterGroupItem>();
			var floorSubSectionFilterValues = new List<MasterFilterGroupItem>();
			var roomFilterValues = new List<MasterFilterGroupItem>();
			var roomCategoryFilterValues = new List<MasterFilterGroupItem>();
			var guestStatusFilterValues = new List<MasterFilterGroupItem>() 
			{
				new MasterFilterGroupItem
				{
					Id = "VACANT",
					Name = "Vacant",
					Description = "Guest status",
				},
				new MasterFilterGroupItem
				{
					Id = "OCCUPIED",
					Name = "Occupied",
					Description = "Guest status",
				},
				new MasterFilterGroupItem
				{
					Id = "STAY",
					Name = "Stay",
					Description = "Guest status",
				},
				new MasterFilterGroupItem
				{
					Id = "ARRIVAL",
					Name = "Arrival",
					Description = "Guest status",
				},
				new MasterFilterGroupItem
				{
					Id = "ARRIVED",
					Name = "Arrived",
					Description = "Guest status",
				},
				new MasterFilterGroupItem
				{
					Id = "DEPARTURE",
					Name = "Departure",
					Description = "Guest status",
				},
				new MasterFilterGroupItem
				{
					Id = "DEPARTED",
					Name = "Departed",
					Description = "Guest status",
				},
				new MasterFilterGroupItem
				{
					Id = "ALL_ARRIVALS",
					Name = "All arrivals",
					Description = "Guest status",
				},
				new MasterFilterGroupItem
				{
					Id = "ALL_DEPARTURES",
					Name = "All departures",
					Description = "Guest status",
				},
			};
			var clenlinessStatusFilterValues = new List<MasterFilterGroupItem>()
			{
				new MasterFilterGroupItem
				{
					Id = "DIRTY",
					Name = "Dirty",
					Description = "Clenliness",
				},
				new MasterFilterGroupItem
				{
					Id = "CLEAN",
					Name = "Clean",
					Description = "Clenliness",
				},
			};
			var housekeepingStatusFilterValues = new List<MasterFilterGroupItem>()
			{
				new MasterFilterGroupItem
				{
					Id = "ANY",
					Name = "Any",
					Description = "Housekeeping status",
				},
				new MasterFilterGroupItem
				{
					Id = "NEW",
					Name = "New",
					Description = "Housekeeping status",
				},
				new MasterFilterGroupItem
				{
					Id = "IN_PROGRESS",
					Name = "In progress",
					Description = "Housekeeping status",
				},
				new MasterFilterGroupItem
				{
					Id = "PAUSED",
					Name = "Paused",
					Description = "Housekeeping status",
				},
				new MasterFilterGroupItem
				{
					Id = "FINISHED",
					Name = "Finished",
					Description = "Housekeeping status",
				},
				new MasterFilterGroupItem
				{
					Id = "INSPECTED",
					Name = "Inspected",
					Description = "Housekeeping status",
				},
				new MasterFilterGroupItem
				{
					Id = "DND",
					Name = "Do not disturb",
					Description = "Housekeeping status",
				},
				new MasterFilterGroupItem
				{
					Id = "REFUSED",
					Name = "Refused",
					Description = "Housekeeping status",
				},
				new MasterFilterGroupItem
				{
					Id = "DELAYED",
					Name = "Delayed",
					Description = "Housekeeping status",
				},
			};
			var pmsFilterValues = new List<MasterFilterGroupItem>()
			{
				new MasterFilterGroupItem
				{
					Id = "VIP",
					Name = "VIP",
					Description = "PMS",
				},
				new MasterFilterGroupItem
				{
					Id = "NOTE",
					Name = "Note",
					Description = "PMS",
				},
			};
			var otherFilterValues = new List<MasterFilterGroupItem>()
			{
				new MasterFilterGroupItem
				{
					Id = "CHANGE_SHEETS",
					Name = "Change sheets",
					Description = "Others",
				},
				new MasterFilterGroupItem
				{
					Id = "PRIORITY",
					Name = "Priority",
					Description = "Others",
				},
				new MasterFilterGroupItem
				{
					Id = "OUT_OF_SERVICE",
					Name = "Out of service",
					Description = "Others",
				},
				new MasterFilterGroupItem
				{
					Id = "OUT_OF_ORDER",
					Name = "Out of order",
					Description = "Others",
				},
				new MasterFilterGroupItem
				{
					Id = "GUEST_IS_IN_THE_ROOM",
					Name = "Guest is in the room",
					Description = "Others",
				},
				new MasterFilterGroupItem
				{
					Id = "GUEST_IS_NOT_IN_THE_ROOM",
					Name = "Guest is not in the room",
					Description = "Others",
				},
			};

			foreach (var category in roomCategories.OrderBy(rc => rc.Name).ToArray())
			{
				roomCategoryFilterValues.Add(new MasterFilterGroupItem
				{
					Id = category.Id.ToString(),
					Description = "Room category",
					Name = category.Name,
				});
			}

			foreach(var building in buildingsMap.Values.OrderBy(b => b.Name).ToArray())
			{
				buildingFilterValues.Add(new MasterFilterGroupItem 
				{ 
					Id = building.Id.ToString(),
					Description = "Building",
					Name = building.Name,
				});
			}

			var floorSectionsMap = new Dictionary<string, HashSet<string>>();
			foreach (var floor in floorsMap.Values.OrderBy(f => f.Number).ToArray())
			{
				floorFilterValues.Add(new MasterFilterGroupItem
				{
					Id = floor.Id.ToString(),
					Description = $"Floor in {buildingsMap[floor.BuildingId].Name}",
					Name = floor.Name,
				});
			}

			foreach (var room in rooms.OrderBy(r => r.OrdinalNumber).ToArray())
			{
				var isTemporaryRoom = false;
				var roomFilterValue = new MasterFilterGroupItem
				{
					Name = room.Name,
					Id = room.Id.ToString(),
				};

				if(!room.BuildingId.HasValue || !room.FloorId.HasValue)
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

						floorSectionFilterValues.Add(new MasterFilterGroupItem
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

							floorSubSectionFilterValues.Add(new MasterFilterGroupItem
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

			return new List<MasterFilterGroup>
			{
				new MasterFilterGroup { Name = "Guest statuses", Items = guestStatusFilterValues, Type = MasterFilterGroupType.GUEST_STATUSES },
				new MasterFilterGroup { Name = "Cleanliness ", Items = clenlinessStatusFilterValues, Type = MasterFilterGroupType.CLENLINESS },
				new MasterFilterGroup { Name = "Housekeeping statuses", Items = housekeepingStatusFilterValues, Type = MasterFilterGroupType.HOUSEKEEPING_STATUSES },
				new MasterFilterGroup { Name = "Others", Items = otherFilterValues, Type = MasterFilterGroupType.OTHERS },
				new MasterFilterGroup { Name = "PMS", Items = pmsFilterValues, Type = MasterFilterGroupType.PMS },
				new MasterFilterGroup { Name = "Room categories", Items = roomCategoryFilterValues, Type = MasterFilterGroupType.ROOM_CATEGORIES },
				new MasterFilterGroup { Name = "Buildings", Items = buildingFilterValues, Type = MasterFilterGroupType.BUILDINGS },
				new MasterFilterGroup { Name = "Floors", Items = floorFilterValues, Type = MasterFilterGroupType.FLOORS },
				new MasterFilterGroup { Name = "Floor sections", Items = floorSectionFilterValues, Type = MasterFilterGroupType.FLOOR_SECTIONS },
				new MasterFilterGroup { Name = "Floor subsections", Items = floorSubSectionFilterValues, Type = MasterFilterGroupType.FLOOR_SUB_SECTIONS },
				new MasterFilterGroup { Name = "Rooms", Items = roomFilterValues, Type = MasterFilterGroupType.ROOMS },
				new MasterFilterGroup { Name = "Guests", Items = guestFilterValues, Type = MasterFilterGroupType.GUESTS },
			};
		}
	}
}
