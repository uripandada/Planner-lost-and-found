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

namespace Planner.Application.Dashboard.Queries.GetRoomViewDashboard
{
	public abstract class RoomViewDashboardItem
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<RoomViewDashboardReservationItem> Reservations { get; set; }
		public IEnumerable<RoomViewDashboardWorkerItem> Workers { get; set; }

		public bool IsOccupied { get; set; }
		public bool IsClean { get; set; }
		public bool IsOutOfService { get; set; }
		public bool IsOutOfOrder { get; set; }
		public bool IsDoNotDisturb { get; set; }
		public bool IsPriority { get; set; }
		public int NumberOfTasks { get; set; }
		public int NumberOfMessages { get; set; }
		public int NumberOfNotes { get; set; }

		public bool IsGuestCurrentlyIn { get; set; }
		public string HousekeepingStatusCode { get; set; }
		public string HousekeepingStatusColorHex { get; set; }
	}

	public class RoomViewDashboardBedItem : RoomViewDashboardItem
	{
		public bool ShowBeds { get; set; }
		public IEnumerable<RoomViewDashboardBedItem> Beds { get; set; }
	}

	public class RoomViewDashboardRoomItem : RoomViewDashboardItem
	{

		public bool ShowBeds { get; set; }
		public IEnumerable<RoomViewDashboardBedItem> Beds { get; set; }
	}

	public class RoomViewDashboardReservationItem
	{
		public string Id { get; set; }
		public string GuestName { get; set; }
		public int NumberOfTasks { get; set; }
		public int NumberOfMessages { get; set; }
		public int NumberOfNotes { get; set; }

		public string CheckInDescription { get; set; } // Arrives after, Checked in
		public string CheckInTimeString { get; set; } // e.g. 10:00, 14:32, 22:30
		public string CheckOutDescription { get; set; } // Departs before, Checked out
		public string CheckOutTimeString { get; set; } // e.g. 10:00, 14:32, 22:30
		public string StayDescription { get; set; } // Stay

		public string FullCheckInDescription { get; set; }
		public string FullCheckOutDescription { get; set; }

		public int NumberOfAdults { get; set; }
		public int NumberOfChildren { get; set; }
		public int NumberOfInfants { get; set; }

		public string Group { get; set; }
		public string Vip { get; set; }
		public bool IsDayUse { get; set; }
		public IEnumerable<RoomViewDashboardReservationNote> Notes { get; set; }
	}
	public class RoomViewDashboardReservationNote
	{
		public bool IsPmsNote { get; set; }
		public string Note { get; set; }
		public Guid? CreatedById { get; set; }
		public string CreatedByName { get; set; }
		public bool IsMyNote { get; set; }
	}
	public class RoomViewDashboardWorkerItem
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
		public string UserName { get; set; }
		public string Initials { get; set; }
		public string DefaultAvatarColorHex { get; set; }
		public string AvatarImageUrl { get; set; }

		public string WorkTypeKey { get; set; } // CLEANING, MAINTENANCE, INSPECTION
		public string WorkStatusDescription { get; set; }
		public string WorkDescription { get; set; } // For cleaners this is the cleaning plugin name / custom cleaning name / cleaning description
	}
	public class RoomViewDashboard
	{
		public IEnumerable<RoomViewDashboardRoomItem> Rooms { get; set; }
		public int TotalNumberOfRooms { get; set; }
	}

	public class GetRoomViewDashboardQuery : IRequest<RoomViewDashboard>
	{
		public string SortKey { get; set; }
		public string SpaceAccessTypeKey { get; set; }
		public string HotelId { get; set; }
		public IEnumerable<GetRoomViewDashboardFilterValue> FilterValues { get; set; }
		public int Skip { get; set; }
		public int Take { get; set; }
	}

	public class GetRoomViewDashboardFilterValue
	{
		public MasterFilterGroupType Type { get; set; }
		public string Id { get; set; }
	}

	public class GetRoomViewDashboardQueryHandler : IRequestHandler<GetRoomViewDashboardQuery, RoomViewDashboard>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public GetRoomViewDashboardQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}


		public async Task<RoomViewDashboard> Handle(GetRoomViewDashboardQuery request, CancellationToken cancellationToken)
		{
			//var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);

			var roomsQuery = this._databaseContext.Rooms
				.AsQueryable();

			//if (request.Keywords.IsNotNull())
			//{
			//	var keywordsValue = request.Keywords.Trim().ToLower();

			//	roomsQuery = roomsQuery.Where(rq => rq.Name.ToLower().Contains(keywordsValue) || rq.Reservations.Any(res => res.GuestName.ToLower().Contains(keywordsValue)));
			//}

			if (request.HotelId.IsNotNull())
			{
				roomsQuery = roomsQuery.Where(r => r.HotelId == request.HotelId);
			}

			//if (request.ReservationStatusKey == "ONLY_OCCUPIED")
			//{
			//	roomsQuery = roomsQuery.Where(r => r.IsOccupied);
			//}
			//else if (request.ReservationStatusKey == "ONLY_VACANT")
			//{
			//	roomsQuery = roomsQuery.Where(r => !r.IsOccupied);
			//}

			//if (request.CleaningStatusKey == "ONLY_CLEAN")
			//{
			//	roomsQuery = roomsQuery.Where(r => r.IsClean);
			//}
			//else if (request.CleaningStatusKey == "ONLY_DIRTY")
			//{
			//	roomsQuery = roomsQuery.Where(r => !r.IsClean);
			//}

			//if(request.SpaceAccessTypeKey == "ONLY_PRIVATE")
			//{
			//	roomsQuery = roomsQuery.Where(r => r.Category != null && !r.Category.IsPublic);
			//}
			//else if (request.SpaceAccessTypeKey == "ONLY_PUBLIC")
			//{
			//	roomsQuery = roomsQuery.Where(r => r.Category != null && r.Category.IsPublic);
			//}

			if (request.SpaceAccessTypeKey.IsNotNull())
			{
				if (request.SpaceAccessTypeKey == "ONLY_PRIVATE")
				{
					roomsQuery = roomsQuery.Where(r => r.Category != null && r.Category.IsPrivate && r.BuildingId != null && r.FloorId != null);
				}
				else if (request.SpaceAccessTypeKey == "ONLY_PUBLIC")
				{
					roomsQuery = roomsQuery.Where(r => r.Category != null && !r.Category.IsPrivate && r.BuildingId != null && r.FloorId != null);
				}
				else if (request.SpaceAccessTypeKey == "ONLY_TEMPORARY")
				{
					roomsQuery = roomsQuery.Where(r => r.BuildingId == null && r.FloorId == null);
				}
			}

			if (request.FilterValues != null)
			{
				var filterGuestNames = new HashSet<string>();
				var filterRoomIds = new HashSet<Guid>();
				var filterRoomCategoryIds = new HashSet<Guid>();
				var filterFloorIds = new HashSet<Guid>();
				var filterBuildingIds = new HashSet<Guid>();
				var filterFloorSections = new HashSet<string>();
				var filterFloorSubSections = new HashSet<string>();
				var filterVips = false;
				var filterPmsNotes = false;
				var filterOnlyClean = false;
				var filterOnlyDirty = false;

				var filterIsPriority = false;
				var filterChangeSheets = false;
				var filterOos = false;
				var filterOoo = false;
				var filterGuestInRoom = false;
				var filterGuestNotInRoom = false;

				var filterVacant = false;
				var filterOccupied = false;
				var filterStay = false;
				var filterArrival = false;
				var filterArrived = false;
				var filterDeparture = false;
				var filterDeparted = false;

				var filterHkAny = false;
				var filterHkNew = false;
				var filterHkFinished = false;
				var filterHkDelayed = false;
				var filterHkPaused = false;
				var filterHkDnd = false;
				var filterHkRefused = false;
				var filterHkInspected = false;
				
				
				foreach (var filterValue in request.FilterValues)
				{
					switch (filterValue.Type)
					{
						case MasterFilterGroupType.GUESTS:
							var guestName = filterValue.Id;
							if (!filterGuestNames.Contains(guestName)) filterGuestNames.Add(guestName);
							break;
						case MasterFilterGroupType.BUILDINGS:
							var buildingId = new Guid(filterValue.Id);
							if (!filterBuildingIds.Contains(buildingId)) filterBuildingIds.Add(buildingId);
							break;
						case MasterFilterGroupType.CLENLINESS:
							switch (filterValue.Id)
							{
								case "CLEAN":
									filterOnlyClean = true;
									break;
								case "DIRTY":
									filterOnlyDirty = true;
									break;
							}
							break;
						case MasterFilterGroupType.FLOORS:
							var floorId = new Guid(filterValue.Id);
							if (!filterFloorIds.Contains(floorId)) filterFloorIds.Add(floorId);
							break;
						case MasterFilterGroupType.FLOOR_SECTIONS:
							var sectionKey = filterValue.Id;
							var sectionKeyParts = sectionKey.Split('|');
							if (sectionKeyParts.Length != 2) break;

							var sFloorId = new Guid(sectionKeyParts[0]);
							var sSection = sectionKeyParts[1];

							if (!filterFloorIds.Contains(sFloorId)) filterFloorIds.Add(sFloorId);
							if (!filterFloorSections.Contains(sSection)) filterFloorSections.Add(sSection);

							break;
						case MasterFilterGroupType.FLOOR_SUB_SECTIONS:
							var subSectionKey = filterValue.Id;
							var subSectionKeyParts = subSectionKey.Split('|');
							if (subSectionKeyParts.Length != 2) break;

							var ssFloorId = new Guid(subSectionKeyParts[0]);
							var ssSection = subSectionKeyParts[1];
							var ssSubSection = subSectionKeyParts[2];

							if(!filterFloorIds.Contains(ssFloorId)) filterFloorIds.Add(ssFloorId);
							if(!filterFloorSections.Contains(ssSection)) filterFloorSections.Add(ssSection);
							if(!filterFloorSubSections.Contains(ssSubSection)) filterFloorSubSections.Add(ssSubSection);

							break;
						case MasterFilterGroupType.GUEST_STATUSES:
							switch (filterValue.Id)
							{
								case "VACANT":
									filterVacant = true;
									break;
								case "OCCUPIED":
									filterOccupied = true;
									break;
								case "STAY":
									filterStay = true;
									break;
								case "ARRIVAL":
									filterArrival = true;
									break;
								case "ARRIVED":
									filterArrived = true;
									break;
								case "DEPARTURE":
									filterDeparture = true;
									break;
								case "DEPARTED":
									filterDeparted = true;
									break;
								case "ALL_ARRIVALS":
									filterArrival = true;
									filterArrived = true;
									break;
								case "ALL_DEPARTURES":
									filterDeparted = true;
									filterDeparture = true;
									break;
							}
							break;
						case MasterFilterGroupType.HOUSEKEEPING_STATUSES:
							switch (filterValue.Id)
							{
								case "ANY":
									filterHkAny = true;
									break;
								case "NEW":
									filterHkNew = true;
									break;
								case "FINISHED":
									filterHkFinished = true;
									break;
								case "DELAYED":
									filterHkDelayed = true;
									break;
								case "PAUSED":
									filterHkPaused = true;
									break;
								case "DND":
									filterHkDnd = true;
									break;
								case "REFUSED":
									filterHkRefused = true;
									break;
								case "INSPECTED":
									filterHkInspected = true;
									break;
							}
							break;
						case MasterFilterGroupType.OTHERS:
							switch (filterValue.Id)
							{
								case "CHANGE_SHEETS":
									filterChangeSheets = true;
									break;
								case "PRIORITY":
									filterIsPriority = true;
									break;
								case "OUT_OF_SERVICE":
									filterOos = true;
									break;
								case "OUT_OF_ORDER":
									filterOoo = true;
									break;
								case "GUEST_IS_IN_THE_ROOM":
									filterGuestInRoom = true;
									break;
								case "GUEST_IS_NOT_IN_THE_ROOM":
									filterGuestNotInRoom = true;
									break;
							}
							break;
						case MasterFilterGroupType.PMS:
							switch (filterValue.Id)
							{
								case "VIP":
									filterVips = true;
									break;
								case "PMS_NOTE":
									filterPmsNotes = true;
									break;
							}
							break;
						case MasterFilterGroupType.ROOMS:
							var roomId = new Guid(filterValue.Id);
							if (!filterRoomIds.Contains(roomId)) filterRoomIds.Add(roomId);
							break;
						case MasterFilterGroupType.ROOM_CATEGORIES:
							var roomCategoryId = new Guid(filterValue.Id);
							if (!filterRoomCategoryIds.Contains(roomCategoryId)) filterRoomCategoryIds.Add(roomCategoryId);
							break;
					}
				}

				if (filterVips) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && rr.Vip != null && rr.Vip != ""));
				if (filterPmsNotes) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && rr.PmsNote != null && rr.PmsNote != ""));

				if (filterBuildingIds.Any()) roomsQuery = roomsQuery.Where(r => r.BuildingId != null && filterBuildingIds.Contains(r.BuildingId.Value));
				if (filterFloorIds.Any()) roomsQuery = roomsQuery.Where(r => r.FloorId != null && filterFloorIds.Contains(r.FloorId.Value));
				if (filterFloorSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSections.Contains(r.FloorSectionName));
				if (filterFloorSubSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSubSections.Contains(r.FloorSubSectionName));
				if (filterRoomIds.Any()) roomsQuery = roomsQuery.Where(r => filterRoomIds.Contains(r.Id));
				if (filterRoomCategoryIds.Any()) roomsQuery = roomsQuery.Where(r => r.CategoryId != null && filterRoomCategoryIds.Contains(r.CategoryId.Value));
				if (filterGuestNames.Any()) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && filterGuestNames.Contains(rr.GuestName)));

				if(filterOnlyClean != filterOnlyDirty)
				{
					if (filterOnlyClean) roomsQuery = roomsQuery.Where(r => r.IsClean);
					if (filterOnlyDirty) roomsQuery = roomsQuery.Where(r => !r.IsClean);
				}

				var filterByIsOccupied = false;
				var isOccupied = false;
				if(filterVacant != filterOccupied)
				{
					filterByIsOccupied = true;
					if (filterVacant)
					{
						isOccupied = false;
					}
					else if (filterOccupied)
					{
						isOccupied = true;
					}
				}

				var reservationStautuses = new List<string>();
				if (filterStay) reservationStautuses.Add("STAY");
				if (filterArrival) reservationStautuses.Add("ARR");
				if (filterArrived) reservationStautuses.Add("CI");
				if (filterDeparture) reservationStautuses.Add("DEP");
				if (filterDeparted) reservationStautuses.Add("CO");
				if (reservationStautuses.Count > 0 && filterByIsOccupied)
				{
					roomsQuery = roomsQuery.Where(r => r.IsOccupied == isOccupied || r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && reservationStautuses.Contains(rr.ReservationStatusKey)));
				}
				else if (reservationStautuses.Count > 0 && !filterByIsOccupied)
				{
					roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && reservationStautuses.Contains(rr.ReservationStatusKey)));

				}
				else if (filterByIsOccupied)
				{
					roomsQuery = roomsQuery.Where(r => r.IsOccupied == isOccupied);
				}

				if (filterIsPriority) roomsQuery = roomsQuery.Where(r => r.IsCleaningPriority);
				if (filterGuestInRoom) roomsQuery = roomsQuery.Where(r => r.IsGuestCurrentlyIn);
				if (filterGuestNotInRoom) roomsQuery = roomsQuery.Where(r => !r.IsGuestCurrentlyIn);
				if (filterOos) roomsQuery = roomsQuery.Where(r => !r.IsOutOfService);
				if (filterOoo) roomsQuery = roomsQuery.Where(r => !r.IsOutOfOrder);

				if (filterChangeSheets) roomsQuery = roomsQuery.Where(r => r.Cleanings.Any(c => c.IsChangeSheets && c.IsActive));

				if (filterHkAny)
				{
					roomsQuery = roomsQuery.Where(r => r.Cleanings.Any(c => c.IsActive) || r.IsInspected);
				}
				else
				{
					var hkStatuses = new List<CleaningProcessStatus>();

					if (filterHkDelayed) hkStatuses.Add(CleaningProcessStatus.DELAYED);
					if (filterHkDnd) hkStatuses.Add(CleaningProcessStatus.DO_NOT_DISTURB);
					if (filterHkFinished) hkStatuses.Add(CleaningProcessStatus.FINISHED);
					if (filterHkNew) hkStatuses.Add(CleaningProcessStatus.NEW);
					if (filterHkPaused) hkStatuses.Add(CleaningProcessStatus.PAUSED);
					if (filterHkRefused) hkStatuses.Add(CleaningProcessStatus.REFUSED);

					if(hkStatuses.Count > 0 && filterHkInspected)
					{
						roomsQuery = roomsQuery.Where(r => r.IsInspected || r.Cleanings.Any(c => c.IsActive && hkStatuses.Contains(c.Status)));
					}
					else if(hkStatuses.Count > 0 && !filterHkInspected)
					{
						roomsQuery = roomsQuery.Where(r => r.Cleanings.Any(c => c.IsActive && hkStatuses.Contains(c.Status)));

					}
					else if (filterHkInspected)
					{
						roomsQuery = roomsQuery.Where(r => r.IsInspected);
					}
				}
			}

			var roomsCount = await roomsQuery.CountAsync();

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					case "NAME_ASC":
						roomsQuery = roomsQuery.OrderBy(r => r.Name);
						break;
					case "NAME_DESC":
						roomsQuery = roomsQuery.OrderByDescending(r => r.Name);
						break;
					case "ORDINAL_NUMBER_DESC":
						roomsQuery = roomsQuery.OrderByDescending(r => r.OrdinalNumber);
						break;
					case "ORDINAL_NUMBER_ASC":
					default:
						roomsQuery = roomsQuery.OrderBy(r => r.OrdinalNumber);
						break;
				}
			}

			if (request.Skip > 0)
			{
				roomsQuery = roomsQuery.Skip(request.Skip);
			}

			if (request.Take > 0)
			{
				roomsQuery = roomsQuery.Take(request.Take);
			}

			// other properties are a jsonb field so the data must materialize before you can .Select them, exception otherwise.
			// TODO: Find out how to project JSONB fields in the select before data materialization
			var rooms = (await roomsQuery.Select(r => new
			{
				Id = r.Id,
				IsClean = r.IsClean,
				IsDoNotDisturb = r.IsDoNotDisturb,
				IsOccupied = r.IsOccupied,
				IsOutOfService = r.IsOutOfService,
				IsOutOfOrder = r.IsOutOfOrder,
				IsGuestCurrentlyIn = r.IsGuestCurrentlyIn,
				Name = r.Name,
				Reservations = r.Reservations.Where(res => res.IsActiveToday && res.IsActive).ToArray(),
				TypeKey = r.TypeKey,
				RccHousekeepingStatus = r.RccHousekeepingStatus,
				IsCleaningPriority = r.IsCleaningPriority,
				Beds = r.RoomBeds.Select(rb => new
				{
					Id = rb.Id,
					Name = rb.Name,
					Reservations = rb.Reservations.Where(res => res.IsActiveToday && res.IsActive).ToArray(),
					RccHousekeepingStatus = rb.RccHousekeepingStatus,
					IsGuestCurrentlyIn = rb.IsGuestCurrentlyIn,
					IsClean = rb.IsClean,
					IsDoNotDisturb = rb.IsDoNotDisturb,
					IsOccupied = rb.IsOccupied,
					IsOutOfService = rb.IsOutOfService,
					IsOutOfOrder = rb.IsOutOfOrder,
					IsCleaningPriority = rb.IsCleaningPriority,
				}).ToArray(),
			}).ToArrayAsync());

			var hotelLocalDateProvider = new HotelLocalDateProvider();
			var hotelCurrentTime = await hotelLocalDateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);
			var hotelCurrentDate = hotelCurrentTime.Date;
			//var counter = 0;

			var roomIds = rooms.Select(r => r.Id).ToHashSet();

			var activeCleanings = await this._databaseContext
				.Cleanings
				.Include(cpi => cpi.Cleaner)
				.Where(cpi => cpi.IsActive &&  cpi.IsPlanned && cpi.StartsAt.Date == hotelCurrentDate && roomIds.Contains(cpi.RoomId))
				.Select(cpi => new
				{
					Id = cpi.Id,
					CleanerId = cpi.Cleaner.Id,
					CleanerDefaultAvatarColorHex = cpi.Cleaner.DefaultAvatarColorHex,
					CleanerFirstName = cpi.Cleaner.FirstName,
					CleanerLastName = cpi.Cleaner.LastName,
					CleanerUsername = cpi.Cleaner.UserName,
					CleaningStatus = cpi.Status,
					CleanerAvatarImageUrl = cpi.Cleaner.Avatar.FileUrl,
					RoomId = cpi.RoomId,
					CleaningDescription = cpi.Description,
				})
				.ToArrayAsync();

			var cleanersMap = new Dictionary<Guid, List<RoomViewDashboardWorkerItem>>();
			foreach (var cleaning in activeCleanings)
			{
				if (!cleanersMap.ContainsKey(cleaning.RoomId))
				{
					cleanersMap.Add(cleaning.RoomId, new List<RoomViewDashboardWorkerItem>());
				}

				cleanersMap[cleaning.RoomId].Add(new RoomViewDashboardWorkerItem
				{
					Id = cleaning.CleanerId,
					AvatarImageUrl = cleaning.CleanerAvatarImageUrl,
					DefaultAvatarColorHex = cleaning.CleanerDefaultAvatarColorHex,
					FullName = $"{cleaning.CleanerFirstName} {cleaning.CleanerLastName}",
					Initials = $"{cleaning.CleanerFirstName?.Substring(0, 1)}{cleaning.CleanerLastName?.Substring(0, 1)}",
					UserName = cleaning.CleanerUsername,
					WorkStatusDescription = cleaning.CleaningStatus.ToString(),
					WorkTypeKey = "CLEANING",
					WorkDescription = cleaning.CleaningDescription
				});
			}

			var roomMessageCounts1 = await this._databaseContext.RoomMessageRooms
				.Where(rmr => rmr.Date == hotelCurrentDate && roomIds.Contains(rmr.RoomId))
				.Select(rmr => new { rmr.RoomId, rmr.RoomBedId, rmr.Date, rmr.RoomMessageId })
				.Distinct()
				.GroupBy(rmr => rmr.RoomId)
				.Select(group => new { Key = group.Key, Value = group.Count() })
				.ToDictionaryAsync(item => item.Key, item => item.Value);

			var roomMessageCounts2 = await this._databaseContext.RoomMessageReservations
				.Where(rmr => rmr.Date == hotelCurrentDate && rmr.Reservation.RoomId != null && roomIds.Contains(rmr.Reservation.Room.Id))
				.Select(rmr => new { rmr.Date, rmr.RoomMessageId, RoomId = rmr.Reservation.RoomId })
				.Distinct()
				.GroupBy(rmr => rmr.RoomId)
				.Select(group => new { Key = group.Key, Value = group.Count() })
				.ToDictionaryAsync(item => item.Key, item => item.Value);


			//await this._databaseContext.RoomMessages
			//	.Where(rm => 
			//		rm.RoomMessageRooms.Any(rmr => rmr.Date == hotelCurrentDate && roomIds.Contains(rmr.RoomId))
			//		||
			//		rm.RoomMessageReservations.Any(rmr => rmr.Date == hotelCurrentDate && rmr.Reservation.RoomId != null && roomIds.Contains(rmr.Reservation.Room.Id))
			//	)
			//	.GroupBy(rm => rm.).CountAsync();

			var colorsMap = await this._databaseContext.RccHousekeepingStatusColors.ToDictionaryAsync(c => c.RccCode);

			return new RoomViewDashboard
			{
				Rooms = rooms.Select(r =>
				{
					var showBeds = r.TypeKey == Common.Enums.RoomTypeEnum.HOSTEL.ToString();
					var roomReservations = new RoomViewDashboardReservationItem[0];
					var roomBeds = new List<RoomViewDashboardBedItem>();

					if (!showBeds)
					{
						roomReservations = r.Reservations
							 .OrderBy(r => r.CheckIn)
							 .Select(r => { 
								 var res = this._CreateReservation(r, hotelCurrentTime);
								 if (r.RoomId.HasValue)
								 {
									 if (roomMessageCounts1.ContainsKey(r.RoomId.Value))
									 {
										 res.NumberOfMessages += roomMessageCounts1[r.RoomId.Value];
									 }
									 if (roomMessageCounts2.ContainsKey(r.RoomId.Value))
									 {
										 res.NumberOfMessages += roomMessageCounts2[r.RoomId.Value];
									 }

								 }
								 return res;
							 })
							 .ToArray();
					}
					else
					{

						foreach (var bed in r.Beds.OrderBy(b => b.Name).ToArray())
						{
							var bedReservations = bed.Reservations
								.OrderBy(r => r.CheckIn)
								.Select(r => {
									var res = this._CreateReservation(r, hotelCurrentTime);
									if (r.RoomId.HasValue)
									{
										if (roomMessageCounts1.ContainsKey(r.RoomId.Value))
										{
											res.NumberOfMessages += roomMessageCounts1[r.RoomId.Value];
										}
										if (roomMessageCounts2.ContainsKey(r.RoomId.Value))
										{
											res.NumberOfMessages += roomMessageCounts2[r.RoomId.Value];
										}

									}
									return res;
								})
								.ToArray();

							roomBeds.Add(
							new RoomViewDashboardBedItem
							{
								Id = bed.Id,
								IsClean = bed.IsClean,
								IsDoNotDisturb = bed.IsDoNotDisturb,
								IsOccupied = bed.IsOccupied,
								IsOutOfService = bed.IsOutOfService,
								IsPriority = bed.IsCleaningPriority,
								Name = bed.Name,
								NumberOfMessages = 0,
								NumberOfNotes = 0,
								NumberOfTasks = 0,
								Reservations = bedReservations,
								IsGuestCurrentlyIn = bed.IsGuestCurrentlyIn,
								IsOutOfOrder = bed.IsOutOfOrder,
								HousekeepingStatusCode = bed.RccHousekeepingStatus.HasValue ? bed.RccHousekeepingStatus.Value.ToString() : "Unknown",
								HousekeepingStatusColorHex = bed.RccHousekeepingStatus.HasValue && colorsMap.ContainsKey(bed.RccHousekeepingStatus.Value) ? "#" + colorsMap[bed.RccHousekeepingStatus.Value].ColorHex : "#343434",
							});
						}
					}

					var item = new RoomViewDashboardRoomItem
					{
						Id = r.Id,
						IsClean = r.IsClean,
						IsDoNotDisturb = r.IsDoNotDisturb,
						IsOccupied = r.IsOccupied,
						IsOutOfService = r.IsOutOfService,
						IsOutOfOrder = r.IsOutOfOrder,
						IsGuestCurrentlyIn = r.IsGuestCurrentlyIn,
						IsPriority = r.IsCleaningPriority,
						Name = r.Name,
						NumberOfTasks = 0,
						NumberOfMessages = 0,
						NumberOfNotes = 0,
						ShowBeds = showBeds,
						Beds = roomBeds,
						Reservations = roomReservations,
						Workers = cleanersMap.ContainsKey(r.Id) ? cleanersMap[r.Id].ToArray() : new RoomViewDashboardWorkerItem[0],
						HousekeepingStatusCode = r.RccHousekeepingStatus.HasValue ? r.RccHousekeepingStatus.Value.ToString() : "Unknown",
						HousekeepingStatusColorHex = r.RccHousekeepingStatus.HasValue && colorsMap.ContainsKey(r.RccHousekeepingStatus.Value) ? "#" + colorsMap[r.RccHousekeepingStatus.Value].ColorHex : "#343434",
					};

					return item;
				}).ToArray(),
				TotalNumberOfRooms = roomsCount
			};
		}

		//private RoomViewDashboardRoomItem _CreateRoomItem()
		//{
		//	var showBeds = r.TypeKey == Common.Enums.RoomTypeEnum.HOSTEL.ToString();
		//	return new RoomViewDashboardRoomItem
		//	{
		//		Id = r.Id,
		//		IsClean = r.IsClean,
		//		IsDoNotDisturb = r.IsDoNotDisturb,
		//		IsOccupied = r.IsOccupied,
		//		IsOutOfService = r.IsOutOfService,
		//		Name = r.Name,
		//		NumberOfTasks = 0,
		//		NumberOfMessages = 0,
		//		NumberOfNotes = 0,
		//		ShowBeds = showBeds,
		//		Beds = showBeds ? (r.Beds == null ? new RoomViewDashboardBedItem[0] : r.Beds.Select(b => new RoomViewDashboardBedItem { Id = b.Id, Name = b.Name }).ToArray()) : new RoomViewDashboardBedItem[0],
		//		Reservations = r.Reservations
		//			 .Where(r => r.CheckIn.HasValue && r.CheckIn.Value.Date <= hotelCurrentTime.Date && r.CheckOut.HasValue && r.CheckOut.Value.Date >= hotelCurrentTime.Date)
		//			 .OrderBy(r => r.CheckIn)
		//			 .Select(r => this._CreateReservation(r, hotelCurrentTime))
		//			 .ToArray(),
		//		Workers = cleanersMap.ContainsKey(r.Id) ? cleanersMap[r.Id].ToArray() : new RoomViewDashboardWorkerItem[0],
		//	}
		//}

		private RoomViewDashboardReservationItem _CreateReservation(Domain.Entities.Reservation reservation, DateTime hotelsCurrentTime)
		{
			var notes = new List<RoomViewDashboardReservationNote>();

			if (reservation.PmsNote.IsNotNull())
			{
				notes.Add(new RoomViewDashboardReservationNote
				{
					IsPmsNote = true,
					Note = reservation.PmsNote
				});
			}

			var r = new RoomViewDashboardReservationItem
			{
				Id = reservation.Id,
				GuestName = $"{reservation.GuestName} ({reservation.NumberOfAdults},{reservation.NumberOfChildren},{reservation.NumberOfInfants})",
				NumberOfMessages = 0,
				NumberOfNotes = notes.Count,
				NumberOfTasks = 0,
				NumberOfAdults = reservation.NumberOfAdults,
				NumberOfChildren = reservation.NumberOfChildren,
				NumberOfInfants = reservation.NumberOfInfants,
				Group = reservation.Group,
				Vip = reservation.Vip,
				Notes = notes,
				IsDayUse = reservation.CheckIn?.Date == reservation.CheckOut?.Date,
			};

			if (reservation.ReservationStatusKey.IsNotNull())
			{
				var reservationStatuses = reservation.ReservationStatusKey.Split("|", StringSplitOptions.RemoveEmptyEntries);
				foreach(var status in reservationStatuses)
				{
					// CI, CO, ARR, DEP, STAY
					switch (status)
					{
						case "STAY":
							r.StayDescription = "STAY";

							if (r.FullCheckInDescription.IsNull())
							{
								if (reservation.ActualCheckIn.HasValue)
								{
									r.FullCheckInDescription = $"{reservation.ActualCheckIn.Value.ToString("dddd dd MMM [HH:mm]")}";
								}
								else if (reservation.CheckIn.HasValue)
								{
									r.FullCheckInDescription = $"{reservation.CheckIn.Value.ToString("dddd dd MMM")}";
								}
								else
								{
									r.FullCheckInDescription = "ETA ?";
								}
							}

							if (r.FullCheckOutDescription.IsNull())
							{
								if (reservation.ActualCheckOut.HasValue)
								{
									r.FullCheckOutDescription = $"{reservation.ActualCheckOut.Value.ToString("dddd dd MMM [HH:mm]")}";
								}
								else if (reservation.CheckOut.HasValue)
								{
									r.FullCheckOutDescription = $"{reservation.CheckOut.Value.ToString("dddd dd MMM")}";
								}
								else
								{
									r.FullCheckOutDescription = "ETD ?";
								}
							}
							break;
						case "ARR":
							var checkInTime = "00:00";
							var checkInDateTime = "Unknown";
							if (reservation.CheckIn.HasValue)
							{
								checkInTime = reservation.CheckIn.Value.ToString("HH:mm");
								checkInDateTime = reservation.CheckIn.Value.ToString("dddd dd MMM");
							}

							r.CheckInDescription = "ARRIVAL";
							r.CheckInTimeString = checkInTime == "00:00" ? "" : $" {checkInTime}";
							r.FullCheckInDescription = checkInDateTime;

							if (r.FullCheckOutDescription.IsNull())
							{
								if (reservation.ActualCheckOut.HasValue)
								{
									r.FullCheckOutDescription = $"{reservation.ActualCheckOut.Value.ToString("dddd dd MMM [HH:mm]")}";
								}
								else if (reservation.CheckOut.HasValue)
								{
									r.FullCheckOutDescription = $"{reservation.CheckOut.Value.ToString("dddd dd MMM")}";
								}
								else
								{
									r.FullCheckOutDescription = "?";
								}
							}

							break;
						case "DEP":
							if (r.FullCheckInDescription.IsNull())
							{
								if (reservation.ActualCheckIn.HasValue)
								{
									r.FullCheckInDescription = $"{reservation.ActualCheckIn.Value.ToString("dddd dd MMM [HH:mm]")}";
								}
								else if (reservation.CheckIn.HasValue)
								{
									r.FullCheckInDescription = $"{reservation.CheckIn.Value.ToString("dddd dd MMM")}";
								}
								else
								{
									r.FullCheckInDescription = "?";
								}
							}

							var checkOutTime = "00:00";
							var checkOutDateTime = "Unknown";
							if (reservation.CheckOut.HasValue)
							{
								checkOutTime = reservation.CheckOut.Value.ToString("HH:mm");
								checkOutDateTime = reservation.CheckOut.Value.ToString("dddd dd MMM");
							}

							r.CheckOutDescription = "DEPARTURE";
							r.CheckOutTimeString = checkOutTime == "00:00" ? "" : $" {checkOutTime}";
							r.FullCheckOutDescription = checkOutDateTime;

							break;
						case "CI":
							var ciCheckInTime = "00:00";
							var ciCheckInDateTime = "Unknown";
							if (reservation.ActualCheckIn.HasValue)
							{
								ciCheckInTime = reservation.ActualCheckIn.Value.ToString("HH:mm");
								ciCheckInDateTime = reservation.ActualCheckIn.Value.ToString("dddd dd MMM [HH:mm]");
							}
							
							if (reservation.CheckIn.HasValue)
							{
								ciCheckInTime = reservation.CheckIn.Value.ToString("HH:mm");
								if(ciCheckInDateTime == "Unknown")
								{
									ciCheckInDateTime = reservation.CheckIn.Value.ToString("dddd dd MMM");
								}
							}

							r.CheckInDescription = "ARRIVED";
							r.CheckInTimeString = ciCheckInTime == "00:00" ? "" : $" {ciCheckInTime}";
							r.FullCheckInDescription = ciCheckInDateTime;

							if (r.FullCheckOutDescription.IsNull())
							{
								if (reservation.ActualCheckOut.HasValue)
								{
									r.FullCheckOutDescription = $"{reservation.ActualCheckOut.Value.ToString("dddd dd MMM [HH:mm]")}";
								}
								else if(reservation.CheckOut.HasValue)
								{
									r.FullCheckOutDescription = $"{reservation.CheckOut.Value.ToString("dddd dd MMM")}";
								}
								else
								{
									r.FullCheckOutDescription = "?";
								}
							}
							break;
						case "CO":
							if (r.FullCheckInDescription.IsNull())
							{
								if (reservation.ActualCheckIn.HasValue)
								{
									r.FullCheckInDescription = $"{reservation.ActualCheckIn.Value.ToString("dddd dd MMM [HH:mm]")}";
								}
								else if (reservation.CheckIn.HasValue)
								{
									r.FullCheckInDescription = $"{reservation.CheckIn.Value.ToString("dddd dd MMM")}";
								}
								else
								{
									r.FullCheckInDescription = "?";
								}
							}

							var coCheckOutTime = "00:00";
							var coCheckOutDateTime = "Unknown";
							if (reservation.ActualCheckOut.HasValue)
							{
								coCheckOutTime = reservation.ActualCheckOut.Value.ToString("HH:mm");
								coCheckOutDateTime = reservation.ActualCheckOut.Value.ToString("dddd dd MMM [HH:mm]");
							}

							if (reservation.CheckOut.HasValue)
							{
								coCheckOutTime = reservation.CheckOut.Value.ToString("HH:mm");
								if (coCheckOutDateTime == "Unknown")
								{
									coCheckOutDateTime = reservation.CheckOut.Value.ToString("dddd dd MMM");
								}
							}

							r.CheckOutDescription = "DEPARTED";
							r.CheckOutTimeString = coCheckOutTime == "00:00" ? "" : $" {coCheckOutTime}";
							r.FullCheckOutDescription = coCheckOutDateTime;
							break;
					}
				}
			}

			//var isStay = reservation.ReservationStatusKey == "STAY";
			//	//reservation.CheckIn.HasValue &&
			//	//reservation.CheckIn.Value.Date < hotelsCurrentTime.Date &&
			//	//reservation.CheckOut.HasValue &&
			//	//reservation.CheckOut.Value.Date > hotelsCurrentTime.Date;

			//if (reservation.ActualCheckIn.HasValue)
			//{
			//	r.FullCheckInDescription = $"{r.CheckInDescription} {reservation.ActualCheckIn.Value.ToString("dd MMM HH:mm")}";
			//}
			//else if (reservation.CheckIn.HasValue)
			//{
			//	r.FullCheckInDescription = $"{r.CheckInDescription} {reservation.CheckIn.Value.ToString("dd MMM HH:mm")}";
			//}

			//if (reservation.ActualCheckOut.HasValue)
			//{
			//	r.FullCheckOutDescription = $"{r.CheckOutDescription} {reservation.ActualCheckOut.Value.ToString("dd MMM HH:mm")}";
			//}
			//else if (reservation.CheckOut.HasValue)
			//{
			//	r.FullCheckOutDescription = $"{r.CheckOutDescription} {reservation.CheckOut.Value.ToString("dd MMM HH:mm")}";
			//}

			//if (isStay)
			//{
			//	r.StayDescription = "Stay";
			//}
			//else
			//{
			//	// Check in should be today
			//	if (reservation.CheckIn.HasValue && reservation.CheckIn.Value.Date == hotelsCurrentTime.Date)
			//	{
			//		// Check in was today
			//		if (reservation.ActualCheckIn.HasValue)
			//		{
			//			r.CheckInTimeString = reservation.ActualCheckIn.Value.ToString("HH:mm");
			//			r.CheckInDescription = "CI"; // "Checked in";
			//		}
			//		// Check in should be today
			//		else if (reservation.CheckIn.HasValue)
			//		{
			//			r.CheckInTimeString = reservation.CheckIn.Value.ToString("HH:mm");
			//			r.CheckInDescription = "ARR"; //"Arrives after";
			//		}
			//	}

			//	// Check out should be today
			//	if (reservation.CheckOut.HasValue && reservation.CheckOut.Value.Date == hotelsCurrentTime.Date)
			//	{
			//		// Check out was today
			//		if (reservation.ActualCheckOut.HasValue)
			//		{
			//			r.CheckOutTimeString = reservation.ActualCheckOut.Value.ToString("HH:mm");
			//			r.CheckOutDescription = "CO"; // "Checked out";
			//		}
			//		// Check out should be today
			//		else if (reservation.CheckOut.HasValue)
			//		{
			//			r.CheckOutTimeString = reservation.CheckOut.Value.ToString("HH:mm");
			//			r.CheckOutDescription = "DEP"; // "Departs before";
			//		}
			//	}
			//}

			return r;
		}
	}
}
