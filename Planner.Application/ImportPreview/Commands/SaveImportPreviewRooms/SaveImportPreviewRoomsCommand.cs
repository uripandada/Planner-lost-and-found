using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.ImportPreview.Commands.UploadImportPreviewRooms;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ImportPreview.Commands.SaveImportPreviewRooms
{
	public class SaveRoomImportResult
	{
		public Guid? Id { get; set; }
		public string RoomName { get; set; }
		public string RoomType { get; set; }
		public string RoomCategory { get; set; }
		public string Beds { get; set; }
		public string Order { get; set; }
		public string FloorSubSection { get; set; }
		public string FloorSection { get; set; }
		public string Floor { get; set; }
		public string Building { get; set; }
		public string Hotel { get; set; }
		public string Area { get; set; }
		public bool HasErrors { get; set; }
		public string Message { get; set; }
	}

	public class SaveImportPreviewRoomsCommand : IRequest<ProcessResponse<IEnumerable<SaveRoomImportResult>>>
	{
		public IEnumerable<ImportRoomPreview> Rooms { get; set; }
	}

	public class SaveImportPreviewRoomsCommandHandler : IRequestHandler<SaveImportPreviewRoomsCommand, ProcessResponse<IEnumerable<SaveRoomImportResult>>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public SaveImportPreviewRoomsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<IEnumerable<SaveRoomImportResult>>> Handle(SaveImportPreviewRoomsCommand request, CancellationToken cancellationToken)
		{
			var results = new List<SaveRoomImportResult>();
			var hotelsMap = new Dictionary<string, Domain.Entities.Hotel>(); // hotelKey -> hotel
			var areasMap = new Dictionary<string, Domain.Entities.Area>(); // areaKey -> area
			var buildingsMap = new Dictionary<string, Dictionary<string, Domain.Entities.Building>>(); // hotelKey -> buildingKey -> building : Building is unique per hotel
			var floorsMap = new Dictionary<string, Dictionary<string, Dictionary<string, Domain.Entities.Floor>>>(); // hotelKey -> buildingKey -> floorKey -> floor : Floor is unique per building which is unique per hotel
			var roomsMap = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Domain.Entities.Room>>>>(); // hotelKey -> buildingKey -> floorKey -> roomKey -> room
			var roomsFullMap = new Dictionary<string, Domain.Entities.Room>(); // hotelKey -> buildingKey -> floorKey -> roomKey -> room
			var unassignedRoomsMap = new Dictionary<string, Dictionary<string, Domain.Entities.Room>>(); // hotelKey -> roomKey -> room
			var unassignedRoomsFullMap = new Dictionary<string, Domain.Entities.Room>(); // hotelKey -> roomKey -> room
			var roomCategoriesMap = new Dictionary<string, Domain.Entities.RoomCategory>();

			var bedsMap = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Domain.Entities.RoomBed>>>>>(); // hotelKey -> buildingKey -> floorKey -> roomKey -> bedKey -> bed
			var bedsFullMap = new Dictionary<string, Domain.Entities.RoomBed>(); // hotelKey -> buildingKey -> floorKey -> roomKey -> bedKey -> bed
			var unassignedBedsMap = new Dictionary<string, Dictionary<string, Dictionary<string, Domain.Entities.RoomBed>>>(); // hotelKey -> roomKey -> bedKey -> bed
			var unassignedBedsFullMap = new Dictionary<string, Domain.Entities.RoomBed>(); // hotelKey -> roomKey -> bedKey -> bed

			var hotels = await this._databaseContext
				.Hotels
				.ToArrayAsync();

			foreach(var hotel in hotels)
			{
				var hotelKey = hotel.Name.Trim().ToLower();
				if (!hotelsMap.ContainsKey(hotelKey))
				{
					hotelsMap.Add(hotelKey, hotel);
					buildingsMap.Add(hotelKey, new Dictionary<string, Building>());
					floorsMap.Add(hotelKey, new Dictionary<string, Dictionary<string, Floor>>());
					roomsMap.Add(hotelKey, new Dictionary<string, Dictionary<string, Dictionary<string, Room>>>());
					unassignedRoomsMap.Add(hotelKey, new Dictionary<string, Room>());
					unassignedBedsMap.Add(hotelKey, new Dictionary<string, Dictionary<string, RoomBed>>());
					bedsMap.Add(hotelKey, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, RoomBed>>>>());
				}
			}

			var areas = await this._databaseContext.Areas.ToArrayAsync();
			
			foreach(var area in areas)
			{
				var areaKey = area.Name.Trim().ToLower();
				if (!areasMap.ContainsKey(areaKey)) areasMap.Add(areaKey, area);
			}

			var roomCategories = await this._databaseContext
				.RoomCategorys
				.ToArrayAsync();

			foreach(var category in roomCategories)
			{
				var categoryKey = category.Name.Trim().ToLower();
				if (!roomCategoriesMap.ContainsKey(categoryKey)) roomCategoriesMap.Add(categoryKey, category);
			}

			var buildings = await this._databaseContext
				.Buildings
				.Include(b => b.Hotel)
				.Include(b => b.Area)
				.Include(b => b.Floors)
				.ThenInclude(f => f.Rooms)
				.ThenInclude(r => r.RoomBeds)
				.ToListAsync();

			foreach(var building in buildings)
			{
				var hotel = hotels.First(h => h.Id == building.HotelId);
				var hotelKey = hotel.Name.Trim().ToLower();

				var buildingKey = building.Name.Trim().ToLower();
				if (!buildingsMap[hotelKey].ContainsKey(buildingKey))
				{
					buildingsMap[hotelKey].Add(buildingKey, building);
					floorsMap[hotelKey].Add(buildingKey, new Dictionary<string, Floor>());
					roomsMap[hotelKey].Add(buildingKey, new Dictionary<string, Dictionary<string, Room>>());
					bedsMap[hotelKey].Add(buildingKey, new Dictionary<string, Dictionary<string, Dictionary<string, RoomBed>>>());
				}

				foreach(var floor in building.Floors)
				{
					var floorKey = floor.Name.Trim().ToLower();
					if (!floorsMap[hotelKey][buildingKey].ContainsKey(floorKey))
					{
						floorsMap[hotelKey][buildingKey].Add(floorKey, floor);
						roomsMap[hotelKey][buildingKey].Add(floorKey, new Dictionary<string, Room>());
						bedsMap[hotelKey][buildingKey].Add(floorKey, new Dictionary<string, Dictionary<string, RoomBed>>());
					}

					foreach (var room in floor.Rooms)
					{
						var roomKey = room.Name.Trim().ToLower();
						if (!roomsMap[hotelKey][buildingKey][floorKey].ContainsKey(roomKey))
						{
							roomsMap[hotelKey][buildingKey][floorKey].Add(roomKey, room);
							roomsFullMap.Add(this._CreateRoomKey(hotelKey, buildingKey, floorKey, roomKey), room);
							bedsMap[hotelKey][buildingKey][floorKey].Add(roomKey, new Dictionary<string, RoomBed>());

							if(room.RoomBeds != null)
							{
								foreach(var bed in room.RoomBeds)
								{
									var bedKey = bed.Name.Trim().ToLower();
									bedsMap[hotelKey][buildingKey][floorKey][roomKey].Add(bedKey, bed);
									bedsFullMap.Add(this._CreateBedKey(hotelKey, buildingKey, floorKey, roomKey, bedKey), bed);
								}
							}
						}
					}
				}
			}

			var unassignedRooms = await this._databaseContext
				.Rooms
				.Where(r => r.BuildingId == null || r.FloorId == null)
				.ToListAsync();

			foreach(var room in unassignedRooms)
			{
				var hotel = hotels.First(h => h.Id == room.HotelId);
				var hotelKey = hotel.Name.Trim().ToLower();
				var roomKey = room.Name.Trim().ToLower();

				if (!unassignedRoomsMap[hotelKey].ContainsKey(roomKey))
				{
					unassignedRoomsMap[hotelKey].Add(roomKey, room);
					unassignedRoomsFullMap.Add(this._CreateTemporaryRoomKey(hotelKey, roomKey), room);
					unassignedBedsMap[hotelKey].Add(roomKey, new Dictionary<string, RoomBed>());

					if (room.RoomBeds != null)
					{
						foreach (var bed in room.RoomBeds)
						{
							var bedKey = bed.Name.Trim().ToLower();
							unassignedBedsMap[hotelKey][roomKey].Add(bedKey, bed);
							unassignedBedsFullMap.Add(this._CreateTemporaryBedKey(hotelKey, roomKey, bedKey), bed);
						}
					}
				}
			}

			var buildingsToInsert = new List<Building>();
			var floorsToInsert = new List<Floor>();
			var areasToInsert = new List<Area>();
			var roomsToInsert = new List<Room>();
			var roomsToUpdate = new List<Room>();
			var bedsToInsert = new List<RoomBed>();
			var roomCategoriesToInsert = new List<RoomCategory>();

			foreach (var roomPreview in request.Rooms)
			{
				// If the building or the floor are not set, add the room to the list of unassigned rooms.
				var hotelKey = roomPreview.Hotel.Trim().ToLower();
				var hotel = hotels.First(h => h.Name.Trim().ToLower() == hotelKey);

				// Find/Create hotel's area
				var area = (Area)null;
				if (roomPreview.Area.IsNotNull())
				{
					var areaKey = roomPreview.Area.Trim().ToLower();
					if (areasMap.ContainsKey(areaKey))
					{
						area = areasMap[areaKey];
					}
					else
					{
						area = new Area
						{
							CreatedAt = DateTime.UtcNow,
							CreatedById = this._userId,
							HotelId = hotel.Id,
							Id = Guid.NewGuid(),
							ModifiedAt = DateTime.UtcNow,
							ModifiedById = this._userId,
							Name = roomPreview.Area.Trim(),
						};

						areasToInsert.Add(area);
						areasMap.Add(areaKey, area);
					}
				}

				var roomCategory = (RoomCategory)null;
				var roomCategoryKey = roomPreview.RoomCategory.Trim().ToLower();
				if (roomCategoriesMap.ContainsKey(roomCategoryKey))
				{
					roomCategory = roomCategoriesMap[roomCategoryKey];
				}
				else
				{
					roomCategory = new RoomCategory
					{
						Id = Guid.NewGuid(),
						CreatedAt = DateTime.UtcNow,
						CreatedById = this._userId,
						ModifiedAt = DateTime.UtcNow,
						ModifiedById = this._userId,
						Name = roomPreview.RoomCategory.Trim(),
						IsPrivate = false,
						IsPublic = true,
					};

					roomCategoriesToInsert.Add(roomCategory);
					roomCategoriesMap.Add(roomCategoryKey, roomCategory);
				}

				// Find/Create room's building and floor.
				var isTemporaryRoom = true;
				var building = (Building)null;
				var floor = (Floor)null;
				var buildingKey = "";
				var floorKey = "";
				if (roomPreview.Building.IsNotNull())
				{
					buildingKey = roomPreview.Building.Trim().ToLower();
					if (buildingsMap[hotelKey].ContainsKey(buildingKey))
					{
						building = buildingsMap[hotelKey][buildingKey];
					}
					else
					{
						building = new Building
						{
							Id = Guid.NewGuid(),
							Address = null,
							AreaId = area == null ? null : area.Id,
							CreatedAt = DateTime.UtcNow,
							CreatedById = this._userId,
							HotelId = hotel.Id,
							ModifiedAt = DateTime.UtcNow,
							ModifiedById = this._userId,
							Latitude = null,
							Longitude = null,
							Name = roomPreview.Building.Trim(),
							OrdinalNumber = 1,
							TypeKey = roomPreview.RoomType.Trim(),
							Floors = new List<Floor>(),
							Hotel = hotel,
						};

						buildingsToInsert.Add(building);

						buildingsMap[hotelKey].Add(buildingKey, building);
						floorsMap[hotelKey].Add(buildingKey, new Dictionary<string, Floor>());
						roomsMap[hotelKey].Add(buildingKey, new Dictionary<string, Dictionary<string, Room>>());
					}

					// CAUTION: FLOOR EXISTS ONLY IF THE BUILDING EXISTS ALSO.
					if (roomPreview.Floor.IsNotNull())
					{
						floorKey = roomPreview.Floor.Trim().ToLower();
						if (floorsMap[hotelKey][buildingKey].ContainsKey(floorKey))
						{
							floor = floorsMap[hotelKey][buildingKey][floorKey];
						}
						else
						{
							int floorNumber;
							if(!int.TryParse(roomPreview.Floor.Trim(), out floorNumber))
							{
								floorNumber = 999;
							}

							floor = new Floor
							{
								Id = Guid.NewGuid(),
								CreatedAt = DateTime.UtcNow,
								CreatedById = this._userId,
								HotelId = hotel.Id,
								ModifiedAt = DateTime.UtcNow,
								ModifiedById = this._userId,
								Name = roomPreview.Floor.Trim(),
								OrdinalNumber = floorNumber,
								Number = floorNumber,
								BuildingId = building.Id,
								Rooms = new List<Room>(),
								Hotel = hotel,
							};

							floorsToInsert.Add(floor);

							floorsMap[hotelKey][buildingKey].Add(floorKey, floor);
							roomsMap[hotelKey][buildingKey].Add(floorKey, new Dictionary<string, Room>());
						}

						isTemporaryRoom = false;
					}
				}

				var room = (Domain.Entities.Room)null;
				var roomKey = roomPreview.RoomName.Trim().ToLower();
				var fullRoomKey = "";
				if (isTemporaryRoom)
				{
					fullRoomKey = this._CreateTemporaryRoomKey(hotelKey, roomKey);

					if (unassignedRoomsFullMap.ContainsKey(fullRoomKey))
					{
						room = unassignedRoomsFullMap[fullRoomKey];
					}
				}
				else
				{
					fullRoomKey = this._CreateRoomKey(hotelKey, buildingKey, floorKey, roomKey);
					
					if (roomsFullMap.ContainsKey(fullRoomKey))
					{
						room = roomsFullMap[fullRoomKey];
					}
				}

				var order = 0;
				if (roomPreview.Order.IsNotNull())
				{
					int.TryParse(roomPreview.Order, out order);
				}

				var bedNames = new string[0];
				if(roomPreview.RoomType.Trim().ToLower() == Planner.Common.Enums.RoomTypeEnum.HOSTEL.ToString().ToLower() && roomPreview.Beds.IsNotNull())
				{
					bedNames = roomPreview.Beds.Split(",", StringSplitOptions.RemoveEmptyEntries);
				}

				var ordinalNumber = 0;
				if(!int.TryParse(roomPreview.Order, out ordinalNumber))
				{
					ordinalNumber = 0;
				}

				// ROOM CHECK!!! - EXTERNAL ID CHECK
				var hasExternalIdDuplicate = false;
				var roomDuplicate = (Domain.Entities.Room)null;
				foreach (var x in roomsFullMap.Values)
				{
					if (x.ExternalId.Trim().ToLower() == roomPreview.RoomName.Trim().ToLower())
					{
						roomDuplicate = x;
					}
				}

				if(roomDuplicate == null)
				{
					foreach (var x in unassignedRoomsFullMap.Values)
					{
						if (x.ExternalId.Trim().ToLower() == roomPreview.RoomName.Trim().ToLower())
						{
							roomDuplicate = x;
						}
					}
				}

				if (roomDuplicate != null)
				{
					if (room == null)
					{
						// The room is really found by the external ID. I don't like this case.
						room = roomDuplicate;
					}
					else
					{
						if (room.Id != roomDuplicate.Id)
						{
							hasExternalIdDuplicate = true;
							// DIFFERENT ROOMS - this really must be the case since the algo would match rooms in earlier steps
							room = roomDuplicate;
						}
					}
				}

				if (room == null)
				{
					// INSERT ROOM!!

					room = new Domain.Entities.Room
					{
						BuildingId = building == null ? null : building.Id,
						FloorId = floor == null ? null : floor.Id,
						AreaId = area == null ? null : area.Id,
						CreatedById = this._userId,
						ModifiedById = this._userId,
						CreatedAt = DateTime.UtcNow,
						ModifiedAt = DateTime.UtcNow,
						Id = Guid.NewGuid(),
						HotelId = hotel.Id,
						ExternalId = roomPreview.RoomName.Trim(),
						Name = roomPreview.RoomName.Trim(),
						FloorSectionName = roomPreview.FloorSection,
						FloorSubSectionName = roomPreview.FloorSubSection,
						IsAutogeneratedFromReservationSync = false,
						IsClean = false,
						IsDoNotDisturb = false,
						IsOccupied = false,
						IsOutOfOrder = false,
						OrdinalNumber = ordinalNumber,
						TypeKey = roomPreview.RoomType.Trim(),
						CategoryId = roomCategory.Id,
						RoomBeds = new RoomBed[0],
						IsInspected = false,
						IsGuestCurrentlyIn = false,
						IsCleaningInProgress = false,
						IsOutOfService = false,
						RccHousekeepingStatus = Common.Enums.RccHousekeepingStatusCode.HC,
						RccRoomStatus = Common.Enums.RccRoomStatusCode.VAC,
					};

					roomsToInsert.Add(room);

					if(isTemporaryRoom)
					{
						unassignedRoomsMap[hotelKey].Add(roomKey, room);
						unassignedRoomsFullMap.Add(fullRoomKey, room);
					}
					else
					{
						roomsMap[hotelKey][buildingKey][floorKey].Add(roomKey, room);
						roomsFullMap.Add(fullRoomKey, room);
					}
				}
				else
				{
					// UPDATE ROOM!!

					// Existing room has a floor but new floor is not set.
					if(room.FloorId.HasValue && floor == null)
					{
						results.Add(new SaveRoomImportResult 
						{ 
							Id = room.Id,
							Area = roomPreview.Area,
							Building = roomPreview.Building,
							Floor = roomPreview.Floor,
							Hotel = roomPreview.Hotel,
							RoomName = roomPreview.RoomName,
							HasErrors = true,
							Message = $"Room was previously assigned to a building and a floor. Room can't become temporary.",
						});

						continue;
					}

					// Existing room has a floor but new floor is different.
					if(room.FloorId.HasValue && floor.Id != room.FloorId.Value)
					{
						results.Add(new SaveRoomImportResult
						{
							Id = room.Id,
							Area = roomPreview.Area,
							Building = roomPreview.Building,
							Floor = roomPreview.Floor,
							Hotel = roomPreview.Hotel,
							RoomName = roomPreview.RoomName,
							HasErrors = true,
							Message = $"Room can't be moved between buildings and floors.",
						});

						continue;
					}

					room.ModifiedAt = DateTime.UtcNow;
					room.ModifiedById = this._userId;
					room.AreaId = area == null ? null : area.Id;
					room.FloorSectionName = roomPreview.FloorSection?.Trim();
					room.FloorSubSectionName = roomPreview.FloorSubSection?.Trim();
					room.TypeKey = roomPreview.RoomType.Trim();
					room.CategoryId = roomCategory.Id;
					room.OrdinalNumber = ordinalNumber;
					
					if(!isTemporaryRoom)
					{
						room.FloorId = floor.Id;
						room.BuildingId = building.Id;
					}

					roomsToUpdate.Add(room);
				}


				// Now check if beds are created!
				foreach (var bedName in bedNames)
				{

					var bed = (Domain.Entities.RoomBed)null;
					var bedKey = bedName.Trim().ToLower();
					var fullBedKey = ""; // isTemporaryRoom ? : this._CreateBedKey(hotelKey, buildingKey, floorKey, roomKey, bedKey);

					if (isTemporaryRoom)
					{
						fullBedKey = this._CreateTemporaryBedKey(hotelKey, roomKey, bedKey);

						if (unassignedBedsFullMap.ContainsKey(fullBedKey))
						{
							bed = unassignedBedsFullMap[fullBedKey];
						}
					}
					else
					{
						fullBedKey = this._CreateBedKey(hotelKey, buildingKey, floorKey, roomKey, bedKey);

						if (bedsFullMap.ContainsKey(fullBedKey))
						{
							bed = bedsFullMap[fullBedKey];
						}
					}

					if(bed == null)
					{
						// INSERT THE BED!
						bedsToInsert.Add(new RoomBed
						{
							Id = Guid.NewGuid(),
							IsAutogeneratedFromReservationSync = false,
							ExternalId = null,
							Name = bedName.Trim(),
							RoomId = room.Id,
							IsOutOfService = false,
							IsGuestCurrentlyIn = false,
							IsCleaningInProgress = false,
							IsClean = false,
							IsDoNotDisturb = false,
							IsInspected = false,
							IsOccupied = false,
							IsOutOfOrder = false,
							RccHousekeepingStatus = Common.Enums.RccHousekeepingStatusCode.HC,
							RccRoomStatus = Common.Enums.RccRoomStatusCode.VAC,
						});
					}
					else
					{
						// UPDATE THE BED - There is nothing to update since there are only bed names.
					}
				}
			}




			try
			{
				using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
				{
					if (roomCategoriesToInsert.Any())
					{
						await this._databaseContext.RoomCategorys.AddRangeAsync(roomCategoriesToInsert);
					}

					if (areasToInsert.Any())
					{
						await this._databaseContext.Areas.AddRangeAsync(areasToInsert);
					}

					if (buildingsToInsert.Any())
					{
						await this._databaseContext.Buildings.AddRangeAsync(buildingsToInsert);
					}

					if (floorsToInsert.Any())
					{
						await this._databaseContext.Floors.AddRangeAsync(floorsToInsert);
					}

					if (roomsToInsert.Any())
					{
						await this._databaseContext.Rooms.AddRangeAsync(roomsToInsert);
					}

					if (bedsToInsert.Any())
					{
						await this._databaseContext.RoomBeds.AddRangeAsync(bedsToInsert);
					}

					await this._databaseContext.SaveChangesAsync(cancellationToken);
					await transaction.CommitAsync(cancellationToken);
				}
			}
			catch (Exception e)
			{
				return new ProcessResponse<IEnumerable<SaveRoomImportResult>>()
				{
					Data = results,
					HasError = true,
					IsSuccess = false,
					Message = "Importing rooms fatal error."
				};
			}

			if (results.Any(r => r.HasErrors))
			{
				return new ProcessResponse<IEnumerable<SaveRoomImportResult>>()
				{
					Data = results,
					HasError = true,
					IsSuccess = false,
					Message = "Importing rooms failed."
				};
			}
			else
			{
				return new ProcessResponse<IEnumerable<SaveRoomImportResult>>()
				{
					Data = results,
					HasError = false,
					IsSuccess = true,
					Message = "Rooms imported."
				};
			}

		}

		private const string SEPARATOR = "|";
		private string _CreateRoomKey(string hotelKey, string buildingKey, string floorKey, string roomKey)
		{
			return $"{hotelKey}{SEPARATOR}{buildingKey}{SEPARATOR}{floorKey}{SEPARATOR}{roomKey}";
		}
		private string _CreateTemporaryRoomKey(string hotelKey, string roomKey)
		{
			return $"{hotelKey}{SEPARATOR}{roomKey}";
		}
		private string _CreateBedKey(string hotelKey, string buildingKey, string floorKey, string roomKey, string bedKey)
		{
			return $"{hotelKey}{SEPARATOR}{buildingKey}{SEPARATOR}{floorKey}{SEPARATOR}{roomKey}{SEPARATOR}{bedKey}";
		}
		private string _CreateTemporaryBedKey(string hotelKey, string roomKey, string bedKey)
		{
			return $"{hotelKey}{SEPARATOR}{roomKey}{SEPARATOR}{bedKey}";
		}
	}
}

