using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Planner.Application.RoomMessages.Commands.InsertComplexMessage
{
	public class InsertComplexMessageCommand: SaveComplexRoomMessage, IRequest<ProcessResponse<Guid>>
	{

	}

	public class InsertComplexMessageCommandHandler : IRequestHandler<InsertComplexMessageCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public InsertComplexMessageCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertComplexMessageCommand request, CancellationToken cancellationToken)
		{
			var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			var timeZoneInfo = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
			var date = dateTime.Date;

			var generator = new RoomMessagesGenerator(this._databaseContext);
			var message = await generator.GenerateRoomMessage(Guid.NewGuid(), this._userId, request, dateTime, cancellationToken);

			await this._databaseContext.RoomMessages.AddAsync(message);

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			var roomIds = message.RoomMessageRooms.Where(rmr => rmr.Date == date).Select(rmr => rmr.RoomId).Distinct().ToArray();
			var reservationIds = message.RoomMessageReservations.Where(rmr => rmr.Date == date).Select(rmr => rmr.ReservationId).Distinct().ToArray();
			var userIds = new List<Guid>();

			if (roomIds.Any())
			{
				userIds = await this._databaseContext
					.Cleanings
					.Where(c => c.IsActive && c.CleaningPlan != null && c.CleaningPlan.Date == date && roomIds.Contains(c.RoomId))
					.Select(c => c.CleanerId)
					.Distinct()
					.ToListAsync();
			}
			else if (reservationIds.Any())
			{
				// TODO: MISSING ROOM BEDS
				userIds = await this._databaseContext
					.Cleanings
					.Where(c => c.IsActive && c.CleaningPlan != null && c.CleaningPlan.Date == date && c.Room.Reservations.Any(r => reservationIds.Contains(r.Id)))
					.Select(c => c.CleanerId)
					.Distinct()
					.ToListAsync();
			}

			await this._systemEventsService.RoomMessagesChanged(this._hotelGroupId, roomIds, reservationIds, userIds);

			return new ProcessResponse<Guid>
			{
				Data = message.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Message sent.",
			};
		}
		//private async Task<IEnumerable<Room>> _LoadTodayMessageRooms(IEnumerable<SaveRoomMessageFilter> filters, string hotelId)
		//{
		//	var filterRoomIds = new HashSet<Guid>();
		//	var filterRoomCategoryIds = new HashSet<Guid>();
		//	var filterFloorIds = new HashSet<Guid>();
		//	var filterBuildingIds = new HashSet<Guid>();
		//	var filterFloorSections = new HashSet<string>();
		//	var filterFloorSubSections = new HashSet<string>();
		//	var filterVips = false;
		//	var filterPmsNotes = false;
		//	var filterOnlyClean = false;
		//	var filterOnlyDirty = false;

		//	var filterIsPriority = false;
		//	var filterChangeSheets = false;
		//	var filterOos = false;
		//	var filterOoo = false;
		//	var filterGuestInRoom = false;
		//	var filterGuestNotInRoom = false;

		//	var filterVacant = false;
		//	var filterOccupied = false;
		//	var filterStay = false;
		//	var filterArrival = false;
		//	var filterArrived = false;
		//	var filterDeparture = false;
		//	var filterDeparted = false;

		//	var filterHkAny = false;
		//	var filterHkNew = false;
		//	var filterHkFinished = false;
		//	var filterHkDelayed = false;
		//	var filterHkPaused = false;
		//	var filterHkDnd = false;
		//	var filterHkRefused = false;
		//	var filterHkInspected = false;


		//	var roomsQuery = this._databaseContext.Rooms
		//		.Where(r => r.HotelId == hotelId)
		//		.AsQueryable();

		//	foreach (var filter in filters)
		//	{
		//		if (filter.ReferenceType == Common.Enums.RoomMessageFilterReferenceType.RESERVATIONS)
		//		{
		//			continue;
		//		}

		//		switch (filter.ReferenceType)
		//		{
		//			case Common.Enums.RoomMessageFilterReferenceType.CLENLINESS:
		//				switch (filter.ReferenceId)
		//				{
		//					case "CLEAN":
		//						filterOnlyClean = true;
		//						break;
		//					case "DIRTY":
		//						filterOnlyDirty = true;
		//						break;
		//				}
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.BUILDINGS:
		//				var buildingId = new Guid(filter.ReferenceId);
		//				if (!filterBuildingIds.Contains(buildingId)) filterBuildingIds.Add(buildingId);
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.FLOORS:
		//				var floorId = new Guid(filter.ReferenceId);
		//				if (!filterFloorIds.Contains(floorId)) filterFloorIds.Add(floorId);
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.FLOOR_SECTIONS:
		//				var sectionKey = filter.ReferenceId;
		//				var sectionKeyParts = sectionKey.Split('|');
		//				if (sectionKeyParts.Length != 2) break;

		//				var sFloorId = new Guid(sectionKeyParts[0]);
		//				var sSection = sectionKeyParts[1];

		//				if (!filterFloorIds.Contains(sFloorId)) filterFloorIds.Add(sFloorId);
		//				if (!filterFloorSections.Contains(sSection)) filterFloorSections.Add(sSection);

		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.FLOOR_SUB_SECTIONS:
		//				var subSectionKey = filter.ReferenceId;
		//				var subSectionKeyParts = subSectionKey.Split('|');
		//				if (subSectionKeyParts.Length != 2) break;

		//				var ssFloorId = new Guid(subSectionKeyParts[0]);
		//				var ssSection = subSectionKeyParts[1];
		//				var ssSubSection = subSectionKeyParts[2];

		//				if (!filterFloorIds.Contains(ssFloorId)) filterFloorIds.Add(ssFloorId);
		//				if (!filterFloorSections.Contains(ssSection)) filterFloorSections.Add(ssSection);
		//				if (!filterFloorSubSections.Contains(ssSubSection)) filterFloorSubSections.Add(ssSubSection);

		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.GUEST_STATUSES:
		//				switch (filter.ReferenceId)
		//				{
		//					case "VACANT":
		//						filterVacant = true;
		//						break;
		//					case "OCCUPIED":
		//						filterOccupied = true;
		//						break;
		//					case "STAY":
		//						filterStay = true;
		//						break;
		//					case "ARRIVAL":
		//						filterArrival = true;
		//						break;
		//					case "ARRIVED":
		//						filterArrived = true;
		//						break;
		//					case "DEPARTURE":
		//						filterDeparture = true;
		//						break;
		//					case "DEPARTED":
		//						filterDeparted = true;
		//						break;
		//					case "ALL_ARRIVALS":
		//						filterArrival = true;
		//						filterArrived = true;
		//						break;
		//					case "ALL_DEPARTURES":
		//						filterDeparted = true;
		//						filterDeparture = true;
		//						break;
		//				}
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.HOUSEKEEPING_STATUSES:
		//				switch (filter.ReferenceId)
		//				{
		//					case "ANY":
		//						filterHkAny = true;
		//						break;
		//					case "NEW":
		//						filterHkNew = true;
		//						break;
		//					case "FINISHED":
		//						filterHkFinished = true;
		//						break;
		//					case "DELAYED":
		//						filterHkDelayed = true;
		//						break;
		//					case "PAUSED":
		//						filterHkPaused = true;
		//						break;
		//					case "DND":
		//						filterHkDnd = true;
		//						break;
		//					case "REFUSED":
		//						filterHkRefused = true;
		//						break;
		//					case "INSPECTED":
		//						filterHkInspected = true;
		//						break;
		//				}
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.OTHERS:
		//				switch (filter.ReferenceId)
		//				{
		//					case "CHANGE_SHEETS":
		//						filterChangeSheets = true;
		//						break;
		//					case "PRIORITY":
		//						filterIsPriority = true;
		//						break;
		//					case "OUT_OF_SERVICE":
		//						filterOos = true;
		//						break;
		//					case "OUT_OF_ORDER":
		//						filterOoo = true;
		//						break;
		//					case "GUEST_IS_IN_THE_ROOM":
		//						filterGuestInRoom = true;
		//						break;
		//					case "GUEST_IS_NOT_IN_THE_ROOM":
		//						filterGuestNotInRoom = true;
		//						break;
		//				}
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.PMS:
		//				switch (filter.ReferenceId)
		//				{
		//					case "VIP":
		//						filterVips = true;
		//						break;
		//					case "PMS_NOTE":
		//						filterPmsNotes = true;
		//						break;
		//				}
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.ROOMS:
		//				var roomId = new Guid(filter.ReferenceId);
		//				if (!filterRoomIds.Contains(roomId)) filterRoomIds.Add(roomId);
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.ROOM_CATEGORIES:
		//				var roomCategoryId = new Guid(filter.ReferenceId);
		//				if (!filterRoomCategoryIds.Contains(roomCategoryId)) filterRoomCategoryIds.Add(roomCategoryId);
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.RESERVATIONS:
		//			default:
		//				continue;
		//		}

		//		if (filterVips) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && rr.Vip != null && rr.Vip != ""));
		//		if (filterPmsNotes) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && rr.PmsNote != null && rr.PmsNote != ""));

		//		if (filterBuildingIds.Any()) roomsQuery = roomsQuery.Where(r => r.BuildingId != null && filterBuildingIds.Contains(r.BuildingId.Value));
		//		if (filterFloorIds.Any()) roomsQuery = roomsQuery.Where(r => r.FloorId != null && filterFloorIds.Contains(r.FloorId.Value));
		//		if (filterFloorSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSections.Contains(r.FloorSectionName));
		//		if (filterFloorSubSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSubSections.Contains(r.FloorSubSectionName));
		//		if (filterRoomIds.Any()) roomsQuery = roomsQuery.Where(r => filterRoomIds.Contains(r.Id));
		//		if (filterRoomCategoryIds.Any()) roomsQuery = roomsQuery.Where(r => r.CategoryId != null && filterRoomCategoryIds.Contains(r.CategoryId.Value));
		//		//if (filterGuestNames.Any()) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && filterGuestNames.Contains(rr.GuestName)));

		//		if (filterOnlyClean != filterOnlyDirty)
		//		{
		//			if (filterOnlyClean) roomsQuery = roomsQuery.Where(r => r.IsClean);
		//			if (filterOnlyDirty) roomsQuery = roomsQuery.Where(r => !r.IsClean);
		//		}

		//		var filterByIsOccupied = false;
		//		var isOccupied = false;
		//		if (filterVacant != filterOccupied)
		//		{
		//			filterByIsOccupied = true;
		//			if (filterVacant)
		//			{
		//				isOccupied = false;
		//			}
		//			else if (filterOccupied)
		//			{
		//				isOccupied = true;
		//			}
		//		}

		//		var reservationStautuses = new List<string>();
		//		if (filterStay) reservationStautuses.Add("STAY");
		//		if (filterArrival) reservationStautuses.Add("ARR");
		//		if (filterArrived) reservationStautuses.Add("CI");
		//		if (filterDeparture) reservationStautuses.Add("DEP");
		//		if (filterDeparted) reservationStautuses.Add("CO");
		//		if (reservationStautuses.Count > 0 && filterByIsOccupied)
		//		{
		//			roomsQuery = roomsQuery.Where(r => r.IsOccupied == isOccupied || r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && reservationStautuses.Contains(rr.ReservationStatusKey)));
		//		}
		//		else if (reservationStautuses.Count > 0 && !filterByIsOccupied)
		//		{
		//			roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && reservationStautuses.Contains(rr.ReservationStatusKey)));

		//		}
		//		else if (filterByIsOccupied)
		//		{
		//			roomsQuery = roomsQuery.Where(r => r.IsOccupied == isOccupied);
		//		}

		//		if (filterIsPriority) roomsQuery = roomsQuery.Where(r => r.IsCleaningPriority);
		//		if (filterGuestInRoom) roomsQuery = roomsQuery.Where(r => r.IsGuestCurrentlyIn);
		//		if (filterGuestNotInRoom) roomsQuery = roomsQuery.Where(r => !r.IsGuestCurrentlyIn);
		//		if (filterOos) roomsQuery = roomsQuery.Where(r => !r.IsOutOfService);
		//		if (filterOoo) roomsQuery = roomsQuery.Where(r => !r.IsOutOfOrder);

		//		if (filterChangeSheets) roomsQuery = roomsQuery.Where(r => r.Cleanings.Any(c => c.IsChangeSheets && c.IsActive));

		//		if (filterHkAny)
		//		{
		//			roomsQuery = roomsQuery.Where(r => r.Cleanings.Any(c => c.IsActive) || r.IsInspected);
		//		}
		//		else
		//		{
		//			var hkStatuses = new List<CleaningProcessStatus>();

		//			if (filterHkDelayed) hkStatuses.Add(CleaningProcessStatus.DELAYED);
		//			if (filterHkDnd) hkStatuses.Add(CleaningProcessStatus.DO_NOT_DISTURB);
		//			if (filterHkFinished) hkStatuses.Add(CleaningProcessStatus.FINISHED);
		//			if (filterHkNew) hkStatuses.Add(CleaningProcessStatus.NEW);
		//			if (filterHkPaused) hkStatuses.Add(CleaningProcessStatus.PAUSED);
		//			if (filterHkRefused) hkStatuses.Add(CleaningProcessStatus.REFUSED);

		//			if (hkStatuses.Count > 0 && filterHkInspected)
		//			{
		//				roomsQuery = roomsQuery.Where(r => r.IsInspected || r.Cleanings.Any(c => c.IsActive && hkStatuses.Contains(c.Status)));
		//			}
		//			else if (hkStatuses.Count > 0 && !filterHkInspected)
		//			{
		//				roomsQuery = roomsQuery.Where(r => r.Cleanings.Any(c => c.IsActive && hkStatuses.Contains(c.Status)));

		//			}
		//			else if (filterHkInspected)
		//			{
		//				roomsQuery = roomsQuery.Where(r => r.IsInspected);
		//			}
		//		}
		//	}

		//	return await roomsQuery.ToListAsync();
		//}

		//private async Task<IEnumerable<Room>> _LoadPlacesRooms(IEnumerable<SaveRoomMessageFilter> filters, string hotelId)
		//{
		//	var filterRoomIds = new HashSet<Guid>();
		//	var filterRoomCategoryIds = new HashSet<Guid>();
		//	var filterFloorIds = new HashSet<Guid>();
		//	var filterBuildingIds = new HashSet<Guid>();
		//	var filterFloorSections = new HashSet<string>();
		//	var filterFloorSubSections = new HashSet<string>();

		//	var roomsQuery = this._databaseContext.Rooms
		//		.Where(r => r.HotelId == hotelId)
		//		.AsQueryable();

		//	var validReferenceTypes = new HashSet<RoomMessageFilterReferenceType> 
		//	{
		//		RoomMessageFilterReferenceType.ROOM_CATEGORIES,
		//		RoomMessageFilterReferenceType.BUILDINGS,
		//		RoomMessageFilterReferenceType.FLOORS,
		//		RoomMessageFilterReferenceType.FLOOR_SECTIONS,
		//		RoomMessageFilterReferenceType.FLOOR_SUB_SECTIONS,
		//		RoomMessageFilterReferenceType.ROOMS,
		//	};

		//	var appliedFilters = filters.Where(f => validReferenceTypes.Contains(f.ReferenceType)).ToArray();

		//	if (appliedFilters.Length == 0)
		//		return new Room[0];

		//	foreach (var filter in appliedFilters)
		//	{
		//		switch (filter.ReferenceType)
		//		{
		//			case Common.Enums.RoomMessageFilterReferenceType.BUILDINGS:
		//				var buildingId = new Guid(filter.ReferenceId);
		//				if (!filterBuildingIds.Contains(buildingId)) filterBuildingIds.Add(buildingId);
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.FLOORS:
		//				var floorId = new Guid(filter.ReferenceId);
		//				if (!filterFloorIds.Contains(floorId)) filterFloorIds.Add(floorId);
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.FLOOR_SECTIONS:
		//				var sectionKey = filter.ReferenceId;
		//				var sectionKeyParts = sectionKey.Split('|');
		//				if (sectionKeyParts.Length != 2) break;

		//				var sFloorId = new Guid(sectionKeyParts[0]);
		//				var sSection = sectionKeyParts[1];

		//				if (!filterFloorIds.Contains(sFloorId)) filterFloorIds.Add(sFloorId);
		//				if (!filterFloorSections.Contains(sSection)) filterFloorSections.Add(sSection);

		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.FLOOR_SUB_SECTIONS:
		//				var subSectionKey = filter.ReferenceId;
		//				var subSectionKeyParts = subSectionKey.Split('|');
		//				if (subSectionKeyParts.Length != 2) break;

		//				var ssFloorId = new Guid(subSectionKeyParts[0]);
		//				var ssSection = subSectionKeyParts[1];
		//				var ssSubSection = subSectionKeyParts[2];

		//				if (!filterFloorIds.Contains(ssFloorId)) filterFloorIds.Add(ssFloorId);
		//				if (!filterFloorSections.Contains(ssSection)) filterFloorSections.Add(ssSection);
		//				if (!filterFloorSubSections.Contains(ssSubSection)) filterFloorSubSections.Add(ssSubSection);

		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.ROOMS:
		//				var roomId = new Guid(filter.ReferenceId);
		//				if (!filterRoomIds.Contains(roomId)) filterRoomIds.Add(roomId);
		//				break;
		//			case Common.Enums.RoomMessageFilterReferenceType.ROOM_CATEGORIES:
		//				var roomCategoryId = new Guid(filter.ReferenceId);
		//				if (!filterRoomCategoryIds.Contains(roomCategoryId)) filterRoomCategoryIds.Add(roomCategoryId);
		//				break;
		//			default:
		//				continue;
		//		}

		//		if (filterBuildingIds.Any()) roomsQuery = roomsQuery.Where(r => r.BuildingId != null && filterBuildingIds.Contains(r.BuildingId.Value));
		//		if (filterFloorIds.Any()) roomsQuery = roomsQuery.Where(r => r.FloorId != null && filterFloorIds.Contains(r.FloorId.Value));
		//		if (filterFloorSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSections.Contains(r.FloorSectionName));
		//		if (filterFloorSubSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSubSections.Contains(r.FloorSubSectionName));
		//		if (filterRoomIds.Any()) roomsQuery = roomsQuery.Where(r => filterRoomIds.Contains(r.Id));
		//		if (filterRoomCategoryIds.Any()) roomsQuery = roomsQuery.Where(r => r.CategoryId != null && filterRoomCategoryIds.Contains(r.CategoryId.Value));
		//	}

		//	return await roomsQuery.ToListAsync();
		//}

		//private async Task<IEnumerable<Reservation>> _LoadMessageReservations(IEnumerable<SaveRoomMessageFilter> filters, Guid roomMessageId, DateTime date)
		//{
		//	var reservationIds = new List<string>();

		//	var appliedFilters = filters.Where(f => f.ReferenceType == RoomMessageFilterReferenceType.RESERVATIONS).ToArray();

		//	if(appliedFilters.Length == 0)
		//	{
		//		return new Reservation[0];
		//	}

		//	foreach (var filter in filters)
		//	{
		//		reservationIds.Add(filter.ReferenceId);
		//	}

		//	return await this._databaseContext.Reservations.Where(r => reservationIds.Contains(r.Id)).ToListAsync();
		//}
	}
}
