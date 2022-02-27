using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.WarehouseManagement.Queries.GetListOfWarehouses;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Queries.GetFullRoomHierarchy
{
	public class HierarchyWarehouseData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsCentralWarehouse { get; set; }
		public Guid? FloorId { get; set; }
		public string HotelId { get; set; }
	}

	public class FullRoomHierarchyData
	{
		public List<FullRoomHierarchyAreaData> AllAreas { get; set; }
		public List<FullRoomHierarchyBuildingData> Buildings { get; set; }
		public List<HierarchyWarehouseData> CentralWarehouses { get; set; }
	}
	
	public class FullRoomHierarchyAreaData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedAt { get; set; }
	}
	
	public class FullRoomHierarchyBuildingData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Guid? AreaId { get; set; }
		public string AreaName { get; set; }
		public string Address { get; set; }
		public long? Latitude { get; set; }
		public long? Longitude { get; set; }
		public DateTime CreatedAt { get; set; }
		public int OrdinalNumber { get; set; }
		public string TypeKey { get; set; }
		public List<FullRoomHierarchyFloorData> Floors { get; set; }
	}
	
	public class FullRoomHierarchyFloorData
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Number { get; set; }
		public DateTime CreatedAt { get; set; }
		public int OrdinalNumber { get; set; }
		public List<FullRoomHierarchyRoomData> Rooms { get; set; }
		public List<HierarchyWarehouseData> Warehouses { get; set; }

	}
	
	public class FullRoomHierarchyRoomData
	{
		public Guid Id { get; set; }
		public string ExternalId { get; set; }
		public string Label { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; }
		public string Description { get; set; }
		public DateTime CreatedAt { get; set; }
		public int OrdinalNumber { get; set; }

		public string FloorSectionName { get; set; }
		public string FloorSubSectionName { get; set; }
		public Guid? CategoryId { get; set; }
		public string CategoryName { get; set; }

		public IEnumerable<FullRoomHierarchyBedData> Beds { get; set; }
	}
	
	public class FullRoomHierarchyBedData
	{
		public Guid Id { get; set; }
		public string ExternalId { get; set; }
		public string Name { get; set; }
	}

	public class GetFullRoomHierarchyQuery : IRequest<FullRoomHierarchyData>
	{
		public string HotelId { get; set; }
		public string Keywords { get; set; }

		public bool IncludeHotelWarehouses { get; set; }
		public bool IncludeCentralWarehouses { get; set; }
	}
	
	public class GetFullRoomHierarchyQueryHandler : IRequestHandler<GetFullRoomHierarchyQuery, FullRoomHierarchyData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetFullRoomHierarchyQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<FullRoomHierarchyData> Handle(GetFullRoomHierarchyQuery request, CancellationToken cancellationToken)
		{
			// Areas are shared between hotels so load all areas
			var areas = await this._databaseContext.Areas
				.ToListAsync();

			var buildingsQuery = this._databaseContext.Buildings
				.Include(b => b.Area)
				.Include(b => b.Floors)
				.AsQueryable();

			var roomsQuery = this._databaseContext.Rooms
				.Include(r => r.RoomBeds)
				.AsQueryable();

			var warehousesQuery = this._databaseContext.Warehouses
				.AsQueryable();

			if (request.HotelId.IsNotNull())
			{
				buildingsQuery = buildingsQuery.Where(h => h.HotelId == request.HotelId);
				roomsQuery = roomsQuery.Where(r => r.HotelId == request.HotelId);
				warehousesQuery = warehousesQuery.Where(w => w.HotelId == request.HotelId);
			}

			if (request.IncludeCentralWarehouses && !request.IncludeHotelWarehouses)
			{
				warehousesQuery = warehousesQuery.Where(w => w.IsCentral);
			} 
			else if (request.IncludeHotelWarehouses && !request.IncludeCentralWarehouses)
			{
				warehousesQuery = warehousesQuery.Where(w => !w.IsCentral);
			}

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.Trim().ToLower();
				roomsQuery = roomsQuery.Where(r => r.Name.ToLower().Contains(keywordsValue));
				warehousesQuery = warehousesQuery.Where(w => w.Name.ToLower().Contains(keywordsValue));
			}

			var buildings = await buildingsQuery
				.OrderBy(b => b.OrdinalNumber)
				.ToListAsync();

			var centralWarehouses = new List<HierarchyWarehouseData>();
			var warehousesMap = new Dictionary<Guid, List<HierarchyWarehouseData>>();
			if(request.IncludeCentralWarehouses || request.IncludeHotelWarehouses)
			{
				var warehouses = await warehousesQuery
					.OrderBy(w => w.Name)
					.Select(w => new HierarchyWarehouseData
					{
						Id = w.Id,
						Name = w.Name,
						FloorId = w.FloorId,
						HotelId = w.HotelId,
						IsCentralWarehouse = w.IsCentral
					})
					.ToArrayAsync();

				foreach(var warehouse in warehouses)
				{
					if (warehouse.IsCentralWarehouse)
					{
						centralWarehouses.Add(warehouse);
					}
					else
					{
						if (!warehouse.FloorId.HasValue)
						{
							// TODO: HANDLE THIS CASE PROPERLY - THERE SHOULD BE A FLOOR ID IN THIS CASE!!!
							continue;
						}

						if(!warehousesMap.ContainsKey(warehouse.FloorId.Value))
						{
							warehousesMap.Add(warehouse.FloorId.Value, new List<HierarchyWarehouseData>());
						}

						warehousesMap[warehouse.FloorId.Value].Add(warehouse);
					}
				}
			}

			var roomsLookup = (await roomsQuery.ToListAsync()).ToLookup(r => r.FloorId);
			var roomCategoriesMap = (await this._databaseContext.RoomCategorys.ToListAsync()).ToDictionary(rc => rc.Id);

			var areasMap = areas.ToDictionary(a => a.Id, a => new FullRoomHierarchyAreaData
			{
				Id = a.Id,
				Name = a.Name
			});

			var buildingsData = new List<FullRoomHierarchyBuildingData>();
			foreach(var building in buildings)
			{
				var buildingData = new FullRoomHierarchyBuildingData
				{
					Id = building.Id,
					Name = building.Name,
					Address = building.Address,
					CreatedAt = building.CreatedAt,
					Latitude = building.Latitude,
					Longitude = building.Longitude,
					OrdinalNumber = building.OrdinalNumber,
					TypeKey = building.TypeKey,
					Floors = new List<FullRoomHierarchyFloorData>()
				};

				if(building.Area != null)
				{
					buildingData.AreaId = building.Area.Id;
					buildingData.AreaName = building.Area.Name;
				}

				foreach(var floor in building.Floors.OrderBy(f => f.OrdinalNumber))
				{
					var floorData = new FullRoomHierarchyFloorData
					{
						Id = floor.Id,
						CreatedAt = floor.CreatedAt,
						Name = floor.Name,
						Number = floor.Number,
						OrdinalNumber = floor.OrdinalNumber,
						Rooms = new List<FullRoomHierarchyRoomData>()
					};

					var floorRooms = roomsLookup.Contains(floor.Id) ? roomsLookup[floor.Id].ToArray() : new Domain.Entities.Room[0];

					foreach(var floorRoom in floorRooms.OrderBy(r => r.OrdinalNumber))
					{
						floorData.Rooms.Add(new FullRoomHierarchyRoomData
						{
							Id = floorRoom.Id,
							CreatedAt = floorRoom.CreatedAt,
							Description = "",
							ExternalId = floorRoom.ExternalId,
							Label = "",
							Name = floorRoom.Name,
							OrdinalNumber = floorRoom.OrdinalNumber,
							TypeKey = floorRoom.TypeKey,
							CategoryId = floorRoom.CategoryId,
							CategoryName = floorRoom.CategoryId.HasValue && roomCategoriesMap.ContainsKey(floorRoom.CategoryId.Value) ? roomCategoriesMap[floorRoom.CategoryId.Value].Name : null,
							FloorSectionName = floorRoom.FloorSectionName,
							FloorSubSectionName = floorRoom.FloorSubSectionName,
							Beds = floorRoom.RoomBeds == null ? new List<FullRoomHierarchyBedData>() : floorRoom.RoomBeds.Select(b => new FullRoomHierarchyBedData { Id = b.Id, ExternalId = b.ExternalId, Name = b.Name }).ToList(),
						});
					}

					if (warehousesMap.ContainsKey(floor.Id))
					{
						floorData.Warehouses = warehousesMap[floor.Id];
					}
					else
					{
						floorData.Warehouses = new List<HierarchyWarehouseData>();
					}

					buildingData.Floors.Add(floorData);
				}

				buildingsData.Add(buildingData);
			}

			return new FullRoomHierarchyData
			{
				AllAreas = areasMap.Values.ToList(),
				Buildings = buildingsData,
				CentralWarehouses = centralWarehouses
			};
		}
	}
}
