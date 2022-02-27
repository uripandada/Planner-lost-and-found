using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Enums;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomMessages
{
	public class RoomMessagesGenerator
	{
		public class RoomMessageDiff
		{
			public bool DidMessageTextChanged { get; set; }
			public List<RoomMessageRoom> FoundRooms { get; set; }
			public List<RoomMessageRoom> NewRooms { get; set; }
			public List<RoomMessageRoom> RemovedRooms { get; set; }
			public List<RoomMessageReservation> FoundReservation { get; set; }
			public List<RoomMessageReservation> NewReservations { get; set; }
			public List<RoomMessageReservation> RemovedReservations { get; set; }
		}

		private IDatabaseContext _databaseContext;

		public RoomMessagesGenerator(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<RoomMessage> GenerateSimpleRoomMessage(Guid roomMessageId, Guid? userId, SaveSimpleRoomMessage request, DateTime dateTime, CancellationToken cancellationToken)
		{
			var message = new Domain.Entities.RoomMessage
			{
				Id = roomMessageId,
				CreatedAt = dateTime,
				CreatedById = userId,
				ModifiedAt = dateTime,
				ModifiedById = userId,
				DateType = RoomMessageDateType.SPECIFIC_DATES,
				ForType = request.ForType,
				IntervalEndDate = null,
				IntervalNumberOfDays = null,
				IntervalStartDate = null,
				IsDeleted = false,
				HotelId = request.HotelId,
				Message = request.Message,
				ReservationOnArrivalDate = null,
				ReservationOnDepartureDate = null,
				ReservationOnStayDates = null,
				Type = RoomMessageType.SIMPLE,
			};

			var roomMessageDates = new List<Domain.Entities.RoomMessageDate> { new RoomMessageDate { Id = Guid.NewGuid(), Date = dateTime.Date, RoomMessageId = message.Id } };
			var messageFilters = new List<Domain.Entities.RoomMessageFilter>();
			var messageRooms = new List<RoomMessageRoom>();
			var messageReservations = new List<RoomMessageReservation>();

			// Message is created for reservations
			if (request.ForType == RoomMessageForType.RESERVATIONS)
			{
				var reservations = await this._databaseContext.Reservations.Where(r => request.ReservationIds.Contains(r.Id)).ToListAsync();

				foreach (var reservation in reservations)
				{
					messageFilters.Add(new Domain.Entities.RoomMessageFilter
					{
						Id = Guid.NewGuid(),
						ReferenceDescription = "Reservation " + reservation.Id,
						ReferenceId = reservation.Id,
						ReferenceName = reservation.GuestName,
						ReferenceType = RoomMessageFilterReferenceType.RESERVATIONS,
						RoomMessageId = message.Id,
					});

					messageReservations.Add(new RoomMessageReservation
					{
						Id = Guid.NewGuid(),
						Date = dateTime.Date,
						ReservationId = reservation.Id,
						RoomMessageId = message.Id,
					});
				}
			}
			// Message is created for the room
			else if (request.ForType == RoomMessageForType.TODAY)
			{
				var room = await this._databaseContext.Rooms.Where(r => r.Id == request.RoomId.Value).FirstOrDefaultAsync();

				messageFilters.Add(new Domain.Entities.RoomMessageFilter
				{
					Id = Guid.NewGuid(),
					ReferenceDescription = "Room",
					ReferenceId = room.Id.ToString(),
					ReferenceName = room.Name,
					ReferenceType = RoomMessageFilterReferenceType.ROOMS,
					RoomMessageId = message.Id,
				});

				messageRooms.Add(new RoomMessageRoom
				{
					Id = Guid.NewGuid(),
					Date = dateTime.Date,
					RoomBedId = null,
					RoomId = room.Id,
					RoomMessageId = message.Id,
				});
			}

			message.RoomMessageDates = roomMessageDates;
			message.RoomMessageFilters = messageFilters;
			message.RoomMessageReservations = messageReservations;
			message.RoomMessageRooms = messageRooms;

			return message;
		}

		public async Task<RoomMessage> GenerateRoomMessage(Guid roomMessageId, Guid? userId, SaveComplexRoomMessage request, DateTime dateTime, CancellationToken cancellationToken)
		{
			//var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			//var timeZoneInfo = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			//var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

			var message = new Domain.Entities.RoomMessage
			{
				Id = roomMessageId,
				CreatedAt = dateTime,
				CreatedById = userId,
				ModifiedAt = dateTime,
				ModifiedById = userId,
				DateType = request.DateType,
				ForType = request.ForType,
				IntervalEndDate = null,
				IntervalNumberOfDays = null,
				IntervalStartDate = null,
				IsDeleted = false,
				HotelId = request.HotelId,
				Message = request.Message,
				ReservationOnArrivalDate = null,
				ReservationOnDepartureDate = null,
				ReservationOnStayDates = null,
				Type = request.Type,
			};

			var messageDates = new List<Domain.Entities.RoomMessageDate>();
			if (request.Dates != null && request.Dates.Any())
			{
				foreach (var d in request.Dates)
				{
					var date = DateTime.ParseExact(d, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
					messageDates.Add(new Domain.Entities.RoomMessageDate
					{
						Date = date,
						Id = Guid.NewGuid(),
						RoomMessageId = message.Id,
					});
				}
			}

			var messageFilters = new List<Domain.Entities.RoomMessageFilter>();
			if (request.Filters != null && request.Filters.Any())
			{
				foreach (var f in request.Filters)
				{
					messageFilters.Add(new Domain.Entities.RoomMessageFilter
					{
						Id = Guid.NewGuid(),
						ReferenceDescription = f.ReferenceDescription,
						ReferenceId = f.ReferenceId,
						ReferenceName = f.ReferenceName,
						ReferenceType = f.ReferenceType,
						RoomMessageId = message.Id,
					});
				}
			}

			var messageRooms = new List<RoomMessageRoom>();
			var messageReservations = new List<RoomMessageReservation>();
			if (request.ForType == Common.Enums.RoomMessageForType.RESERVATIONS)
			{
				message.ReservationOnArrivalDate = request.ReservationOnArrivalDate ?? false;
				message.ReservationOnStayDates = request.ReservationOnStayDates ?? false;
				message.ReservationOnDepartureDate = request.ReservationOnDepartureDate ?? false;

				// ALL REFERENCES ARE RESERVATIONS
				var reservations = await this._LoadMessageReservations(request.Filters, message.Id, dateTime);

				foreach (var reservation in reservations)
				{
					if (!reservation.CheckIn.HasValue || !reservation.CheckOut.HasValue)
					{
						continue;
					}

					if (request.ReservationOnArrivalDate ?? false)
					{
						messageReservations.Add(new RoomMessageReservation
						{
							Date = reservation.CheckIn.Value.Date,
							Id = Guid.NewGuid(),
							ReservationId = reservation.Id,
							RoomMessageId = message.Id,
						});
					}

					if (request.ReservationOnStayDates ?? false)
					{
						// First date is the arrival / check in date
						var fromDate = reservation.CheckIn.Value.Date.AddDays(1);
						// Last date is the departure / check out date
						var toDate = reservation.CheckOut.Value.Date.AddDays(-1);

						var date = fromDate;

						while (date <= toDate)
						{
							messageReservations.Add(new RoomMessageReservation
							{
								Date = date,
								Id = Guid.NewGuid(),
								ReservationId = reservation.Id,
								RoomMessageId = message.Id,
							});

							date.AddDays(1);
						}
					}

					if (request.ReservationOnDepartureDate ?? false)
					{
						messageReservations.Add(new RoomMessageReservation
						{
							Date = reservation.CheckOut.Value.Date,
							Id = Guid.NewGuid(),
							ReservationId = reservation.Id,
							RoomMessageId = message.Id,
						});
					}
				}
			}
			else
			{
				//var rooms = (IEnumerable<Domain.Entities.Room>)null;
				if (request.ForType == Common.Enums.RoomMessageForType.PLACES)
				{
					// ONLY BUILDINGS, FLOORS, SECTIONS, SUBSECTIONS, ROOMS
					var rooms = await this._LoadPlacesRooms(request.Filters, request.HotelId);

					if (request.DateType == Common.Enums.RoomMessageDateType.SPECIFIC_DATES)
					{

						foreach (var dateString in request.Dates)
						{
							var date = DateTime.ParseExact(dateString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

							foreach (var room in rooms)
							{
								messageRooms.Add(new RoomMessageRoom
								{
									Id = Guid.NewGuid(),
									Date = date,
									RoomId = room.Id,
									RoomMessageId = message.Id,
								});
							}

							//dates.Add(date);
						}

					}
					else if (request.DateType == Common.Enums.RoomMessageDateType.INTERVAL)
					{
						var fromDate = DateTime.ParseExact(request.IntervalStartDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
						var toDate = DateTime.ParseExact(request.IntervalEndDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
						var period = !request.IntervalEveryNumberOfDays.HasValue || request.IntervalEveryNumberOfDays < 1 ? 1 : request.IntervalEveryNumberOfDays.Value;

						message.IntervalStartDate = fromDate;
						message.IntervalEndDate = toDate;
						message.IntervalNumberOfDays = period;

						var date = fromDate;
						while (date <= toDate)
						{
							foreach (var room in rooms)
							{
								messageRooms.Add(new RoomMessageRoom
								{
									Id = Guid.NewGuid(),
									Date = date,
									RoomId = room.Id,
									RoomMessageId = message.Id,
								});
							}

							date = date.AddDays(period);
						}
					}
				}
				else if (request.ForType == Common.Enums.RoomMessageForType.TODAY)
				{
					// EVERYTHING BUT RESERVATIONS
					var rooms = await this._LoadTodayMessageRooms(request.Filters, request.HotelId);
					var date = dateTime.Date;

					foreach (var room in rooms)
					{
						messageRooms.Add(new RoomMessageRoom
						{
							Id = Guid.NewGuid(),
							Date = date,
							RoomId = room.Id,
							RoomMessageId = message.Id,
						});
					}
				}
			}


			message.RoomMessageDates = messageDates;
			message.RoomMessageFilters = messageFilters;
			message.RoomMessageReservations = messageReservations;
			message.RoomMessageRooms = messageRooms;
			return message;
		}

		public RoomMessageDiff FindDifferenceBetweenMessages(RoomMessage message, RoomMessage updatedMessage)
		{
			var foundRooms = new List<RoomMessageRoom>();
			var newRooms = new List<RoomMessageRoom>();
			var removedRooms = new List<RoomMessageRoom>();

			// RoomId -> Date:RoomMessageRoom[]
			var updatedMessageRoomsMap = updatedMessage.RoomMessageRooms.GroupBy(r => r.RoomId).ToDictionary(group => group.Key, group => group.Where(group => group.RoomBedId == null).GroupBy(g => g.Date).ToDictionary(g => g.Key, g => g.ToArray()));
			// RoomId -> RoomBedId -> Date:RoomMessageRoom[]
			var updatedMessageRoomBedsMap = updatedMessage.RoomMessageRooms.GroupBy(r => r.RoomId).ToDictionary(group => group.Key, group => group.Where(group => group.RoomBedId != null).GroupBy(g => g.RoomBedId.Value).ToDictionary(ig => ig.Key, ig => ig.GroupBy(iig => iig.Date).ToDictionary(iig => iig.Key, iig => iig.ToArray())));

			var checkedMessageRoomsMap = new Dictionary<Guid, HashSet<DateTime>>();
			var checkedMessageRoomBedsMap = new Dictionary<Guid, Dictionary<Guid, HashSet<DateTime>>>();

			foreach(var messageRoom in message.RoomMessageRooms)
			{
				if(messageRoom.RoomBedId == null)
				{
					if (!checkedMessageRoomsMap.ContainsKey(messageRoom.RoomId)) checkedMessageRoomsMap.Add(messageRoom.RoomId, new HashSet<DateTime>());
					if (!checkedMessageRoomsMap[messageRoom.RoomId].Contains(messageRoom.Date)) checkedMessageRoomsMap[messageRoom.RoomId].Add(messageRoom.Date);

					if (!updatedMessageRoomsMap.ContainsKey(messageRoom.RoomId))
					{
						// DELETE MESSAGE ROOM
						removedRooms.Add(messageRoom);
					}
					else
					{
						if (!updatedMessageRoomsMap[messageRoom.RoomId].ContainsKey(messageRoom.Date))
						{
							// DELETE MESSAGE ROOM
							removedRooms.Add(messageRoom);
						}
						else
						{
							// UPDATE MESSAGE ROOM
							foundRooms.Add(messageRoom);
						}
					}
				}
				else
				{
					if (!checkedMessageRoomBedsMap.ContainsKey(messageRoom.RoomId)) checkedMessageRoomBedsMap.Add(messageRoom.RoomId, new Dictionary<Guid, HashSet<DateTime>>());
					if (!checkedMessageRoomBedsMap[messageRoom.RoomId].ContainsKey(messageRoom.RoomBedId.Value)) checkedMessageRoomBedsMap[messageRoom.RoomId].Add(messageRoom.RoomBedId.Value, new HashSet<DateTime>());
					if (!checkedMessageRoomBedsMap[messageRoom.RoomId][messageRoom.RoomBedId.Value].Contains(messageRoom.Date)) checkedMessageRoomBedsMap[messageRoom.RoomId][messageRoom.RoomBedId.Value].Add(messageRoom.Date);

					if (!updatedMessageRoomBedsMap.ContainsKey(messageRoom.RoomId))
					{
						// DELETE MESSAGE ROOM
						removedRooms.Add(messageRoom);
					}
					else
					{
						if (!updatedMessageRoomBedsMap[messageRoom.RoomId].ContainsKey(messageRoom.RoomBedId.Value))
						{
							// DELETE MESSAGE ROOM
							removedRooms.Add(messageRoom);
						}
						else
						{

							if (!updatedMessageRoomBedsMap[messageRoom.RoomId][messageRoom.RoomBedId.Value].ContainsKey(messageRoom.Date))
							{
								// DELETE MESSAGE ROOM
								removedRooms.Add(messageRoom);
							}
							else
							{
								// UPDATE MESSAGE ROOM
								foundRooms.Add(messageRoom);
							}
						}
					}
				}
			}

			foreach(var updatedMessageRoom in updatedMessage.RoomMessageRooms)
			{
				if(updatedMessageRoom.RoomBedId == null)
				{
					if (checkedMessageRoomsMap.ContainsKey(updatedMessageRoom.RoomId) && checkedMessageRoomsMap[updatedMessageRoom.RoomId].Contains(updatedMessageRoom.Date))
						continue;

					if (!checkedMessageRoomsMap.ContainsKey(updatedMessageRoom.RoomId)) checkedMessageRoomsMap.Add(updatedMessageRoom.RoomId, new HashSet<DateTime>());
					if (!checkedMessageRoomsMap[updatedMessageRoom.RoomId].Contains(updatedMessageRoom.Date)) checkedMessageRoomsMap[updatedMessageRoom.RoomId].Add(updatedMessageRoom.Date);

					// INSERT MESSAGE ROOM
					newRooms.Add(updatedMessageRoom);
				}
				else
				{
					if (checkedMessageRoomBedsMap.ContainsKey(updatedMessageRoom.RoomId) && checkedMessageRoomBedsMap[updatedMessageRoom.RoomId].ContainsKey(updatedMessageRoom.RoomBedId.Value) && checkedMessageRoomBedsMap[updatedMessageRoom.RoomId][updatedMessageRoom.RoomBedId.Value].Contains(updatedMessageRoom.Date))
						continue;

					if (!checkedMessageRoomBedsMap.ContainsKey(updatedMessageRoom.RoomId)) checkedMessageRoomBedsMap.Add(updatedMessageRoom.RoomId, new Dictionary<Guid, HashSet<DateTime>>());
					if (!checkedMessageRoomBedsMap[updatedMessageRoom.RoomId].ContainsKey(updatedMessageRoom.RoomBedId.Value)) checkedMessageRoomBedsMap[updatedMessageRoom.RoomId].Add(updatedMessageRoom.RoomBedId.Value, new HashSet<DateTime>());
					if (!checkedMessageRoomBedsMap[updatedMessageRoom.RoomId][updatedMessageRoom.RoomBedId.Value].Contains(updatedMessageRoom.Date)) checkedMessageRoomBedsMap[updatedMessageRoom.RoomId][updatedMessageRoom.RoomBedId.Value].Add(updatedMessageRoom.Date);

					// INSERT MESSAGE ROOM
					newRooms.Add(updatedMessageRoom);
				}
			}


			var foundReservations = new List<RoomMessageReservation>();
			var newReservations = new List<RoomMessageReservation>();
			var removedReservations = new List<RoomMessageReservation>();

			var updatedMessageReservationsMap = updatedMessage.RoomMessageReservations.GroupBy(r => r.ReservationId).ToDictionary(group => group.Key, group => group.GroupBy(g => g.Date).ToDictionary(g => g.Key, g => g.ToArray()));
			var checkedMessageReservationsMap = new Dictionary<string, HashSet<DateTime>>();

			foreach (var messageReservation in message.RoomMessageReservations)
			{
				if (!checkedMessageReservationsMap.ContainsKey(messageReservation.ReservationId)) checkedMessageReservationsMap.Add(messageReservation.ReservationId, new HashSet<DateTime>());
				if (!checkedMessageReservationsMap[messageReservation.ReservationId].Contains(messageReservation.Date)) checkedMessageReservationsMap[messageReservation.ReservationId].Add(messageReservation.Date);

				if (!updatedMessageReservationsMap.ContainsKey(messageReservation.ReservationId))
				{
					// DELETE MESSAGE RESERVATION
					removedReservations.Add(messageReservation);
				}
				else
				{
					if (!updatedMessageReservationsMap[messageReservation.ReservationId].ContainsKey(messageReservation.Date))
					{
						// DELETE MESSAGE RESERVATION
						removedReservations.Add(messageReservation);
					}
					else
					{
						// UPDATE MESSAGE RESERVATION
						foundReservations.Add(messageReservation);
					}
				}
			}


			foreach (var updatedMessageReservation in updatedMessage.RoomMessageReservations)
			{
				if (checkedMessageReservationsMap.ContainsKey(updatedMessageReservation.ReservationId) && checkedMessageReservationsMap[updatedMessageReservation.ReservationId].Contains(updatedMessageReservation.Date))
					continue;

				if (!checkedMessageReservationsMap.ContainsKey(updatedMessageReservation.ReservationId)) checkedMessageReservationsMap.Add(updatedMessageReservation.ReservationId, new HashSet<DateTime>());
				if (!checkedMessageReservationsMap[updatedMessageReservation.ReservationId].Contains(updatedMessageReservation.Date)) checkedMessageReservationsMap[updatedMessageReservation.ReservationId].Add(updatedMessageReservation.Date);

				// INSERT MESSAGE RESERVATION
				newReservations.Add(updatedMessageReservation);
			}

			var diff = new RoomMessageDiff();
			diff.DidMessageTextChanged = message.Message != updatedMessage.Message;
			diff.NewReservations = newReservations;
			diff.FoundReservation = foundReservations;
			diff.RemovedReservations = removedReservations;
			diff.NewRooms = newRooms;
			diff.FoundRooms = foundRooms;
			diff.RemovedRooms = removedRooms;

			return diff;
		}

		private async Task<IEnumerable<Room>> _LoadTodayMessageRooms(IEnumerable<SaveRoomMessageFilter> filters, string hotelId)
		{
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


			var roomsQuery = this._databaseContext.Rooms
				.Where(r => r.HotelId == hotelId)
				.AsQueryable();

			foreach (var filter in filters)
			{
				if (filter.ReferenceType == Common.Enums.RoomMessageFilterReferenceType.RESERVATIONS)
				{
					continue;
				}

				switch (filter.ReferenceType)
				{
					case Common.Enums.RoomMessageFilterReferenceType.CLENLINESS:
						switch (filter.ReferenceId)
						{
							case "CLEAN":
								filterOnlyClean = true;
								break;
							case "DIRTY":
								filterOnlyDirty = true;
								break;
						}
						break;
					case Common.Enums.RoomMessageFilterReferenceType.BUILDINGS:
						var buildingId = new Guid(filter.ReferenceId);
						if (!filterBuildingIds.Contains(buildingId)) filterBuildingIds.Add(buildingId);
						break;
					case Common.Enums.RoomMessageFilterReferenceType.FLOORS:
						var floorId = new Guid(filter.ReferenceId);
						if (!filterFloorIds.Contains(floorId)) filterFloorIds.Add(floorId);
						break;
					case Common.Enums.RoomMessageFilterReferenceType.FLOOR_SECTIONS:
						var sectionKey = filter.ReferenceId;
						var sectionKeyParts = sectionKey.Split('|');
						if (sectionKeyParts.Length != 2) break;

						var sFloorId = new Guid(sectionKeyParts[0]);
						var sSection = sectionKeyParts[1];

						if (!filterFloorIds.Contains(sFloorId)) filterFloorIds.Add(sFloorId);
						if (!filterFloorSections.Contains(sSection)) filterFloorSections.Add(sSection);

						break;
					case Common.Enums.RoomMessageFilterReferenceType.FLOOR_SUB_SECTIONS:
						var subSectionKey = filter.ReferenceId;
						var subSectionKeyParts = subSectionKey.Split('|');
						if (subSectionKeyParts.Length != 2) break;

						var ssFloorId = new Guid(subSectionKeyParts[0]);
						var ssSection = subSectionKeyParts[1];
						var ssSubSection = subSectionKeyParts[2];

						if (!filterFloorIds.Contains(ssFloorId)) filterFloorIds.Add(ssFloorId);
						if (!filterFloorSections.Contains(ssSection)) filterFloorSections.Add(ssSection);
						if (!filterFloorSubSections.Contains(ssSubSection)) filterFloorSubSections.Add(ssSubSection);

						break;
					case Common.Enums.RoomMessageFilterReferenceType.GUEST_STATUSES:
						switch (filter.ReferenceId)
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
					case Common.Enums.RoomMessageFilterReferenceType.HOUSEKEEPING_STATUSES:
						switch (filter.ReferenceId)
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
					case Common.Enums.RoomMessageFilterReferenceType.OTHERS:
						switch (filter.ReferenceId)
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
					case Common.Enums.RoomMessageFilterReferenceType.PMS:
						switch (filter.ReferenceId)
						{
							case "VIP":
								filterVips = true;
								break;
							case "PMS_NOTE":
								filterPmsNotes = true;
								break;
						}
						break;
					case Common.Enums.RoomMessageFilterReferenceType.ROOMS:
						var roomId = new Guid(filter.ReferenceId);
						if (!filterRoomIds.Contains(roomId)) filterRoomIds.Add(roomId);
						break;
					case Common.Enums.RoomMessageFilterReferenceType.ROOM_CATEGORIES:
						var roomCategoryId = new Guid(filter.ReferenceId);
						if (!filterRoomCategoryIds.Contains(roomCategoryId)) filterRoomCategoryIds.Add(roomCategoryId);
						break;
					case Common.Enums.RoomMessageFilterReferenceType.RESERVATIONS:
					default:
						continue;
				}

				if (filterVips) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && rr.Vip != null && rr.Vip != ""));
				if (filterPmsNotes) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && rr.PmsNote != null && rr.PmsNote != ""));

				if (filterBuildingIds.Any()) roomsQuery = roomsQuery.Where(r => r.BuildingId != null && filterBuildingIds.Contains(r.BuildingId.Value));
				if (filterFloorIds.Any()) roomsQuery = roomsQuery.Where(r => r.FloorId != null && filterFloorIds.Contains(r.FloorId.Value));
				if (filterFloorSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSections.Contains(r.FloorSectionName));
				if (filterFloorSubSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSubSections.Contains(r.FloorSubSectionName));
				if (filterRoomIds.Any()) roomsQuery = roomsQuery.Where(r => filterRoomIds.Contains(r.Id));
				if (filterRoomCategoryIds.Any()) roomsQuery = roomsQuery.Where(r => r.CategoryId != null && filterRoomCategoryIds.Contains(r.CategoryId.Value));
				//if (filterGuestNames.Any()) roomsQuery = roomsQuery.Where(r => r.Reservations.Any(rr => rr.IsActive && rr.IsActiveToday && filterGuestNames.Contains(rr.GuestName)));

				if (filterOnlyClean != filterOnlyDirty)
				{
					if (filterOnlyClean) roomsQuery = roomsQuery.Where(r => r.IsClean);
					if (filterOnlyDirty) roomsQuery = roomsQuery.Where(r => !r.IsClean);
				}

				var filterByIsOccupied = false;
				var isOccupied = false;
				if (filterVacant != filterOccupied)
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

					if (hkStatuses.Count > 0 && filterHkInspected)
					{
						roomsQuery = roomsQuery.Where(r => r.IsInspected || r.Cleanings.Any(c => c.IsActive && hkStatuses.Contains(c.Status)));
					}
					else if (hkStatuses.Count > 0 && !filterHkInspected)
					{
						roomsQuery = roomsQuery.Where(r => r.Cleanings.Any(c => c.IsActive && hkStatuses.Contains(c.Status)));

					}
					else if (filterHkInspected)
					{
						roomsQuery = roomsQuery.Where(r => r.IsInspected);
					}
				}
			}

			return await roomsQuery.ToListAsync();
		}

		private async Task<IEnumerable<Room>> _LoadPlacesRooms(IEnumerable<SaveRoomMessageFilter> filters, string hotelId)
		{
			var filterRoomIds = new HashSet<Guid>();
			var filterRoomCategoryIds = new HashSet<Guid>();
			var filterFloorIds = new HashSet<Guid>();
			var filterBuildingIds = new HashSet<Guid>();
			var filterFloorSections = new HashSet<string>();
			var filterFloorSubSections = new HashSet<string>();

			var roomsQuery = this._databaseContext.Rooms
				.Where(r => r.HotelId == hotelId)
				.AsQueryable();

			var validReferenceTypes = new HashSet<RoomMessageFilterReferenceType>
			{
				RoomMessageFilterReferenceType.ROOM_CATEGORIES,
				RoomMessageFilterReferenceType.BUILDINGS,
				RoomMessageFilterReferenceType.FLOORS,
				RoomMessageFilterReferenceType.FLOOR_SECTIONS,
				RoomMessageFilterReferenceType.FLOOR_SUB_SECTIONS,
				RoomMessageFilterReferenceType.ROOMS,
			};

			var appliedFilters = filters.Where(f => validReferenceTypes.Contains(f.ReferenceType)).ToArray();

			if (appliedFilters.Length == 0)
				return new Room[0];

			foreach (var filter in appliedFilters)
			{
				switch (filter.ReferenceType)
				{
					case Common.Enums.RoomMessageFilterReferenceType.BUILDINGS:
						var buildingId = new Guid(filter.ReferenceId);
						if (!filterBuildingIds.Contains(buildingId)) filterBuildingIds.Add(buildingId);
						break;
					case Common.Enums.RoomMessageFilterReferenceType.FLOORS:
						var floorId = new Guid(filter.ReferenceId);
						if (!filterFloorIds.Contains(floorId)) filterFloorIds.Add(floorId);
						break;
					case Common.Enums.RoomMessageFilterReferenceType.FLOOR_SECTIONS:
						var sectionKey = filter.ReferenceId;
						var sectionKeyParts = sectionKey.Split('|');
						if (sectionKeyParts.Length != 2) break;

						var sFloorId = new Guid(sectionKeyParts[0]);
						var sSection = sectionKeyParts[1];

						if (!filterFloorIds.Contains(sFloorId)) filterFloorIds.Add(sFloorId);
						if (!filterFloorSections.Contains(sSection)) filterFloorSections.Add(sSection);

						break;
					case Common.Enums.RoomMessageFilterReferenceType.FLOOR_SUB_SECTIONS:
						var subSectionKey = filter.ReferenceId;
						var subSectionKeyParts = subSectionKey.Split('|');
						if (subSectionKeyParts.Length != 2) break;

						var ssFloorId = new Guid(subSectionKeyParts[0]);
						var ssSection = subSectionKeyParts[1];
						var ssSubSection = subSectionKeyParts[2];

						if (!filterFloorIds.Contains(ssFloorId)) filterFloorIds.Add(ssFloorId);
						if (!filterFloorSections.Contains(ssSection)) filterFloorSections.Add(ssSection);
						if (!filterFloorSubSections.Contains(ssSubSection)) filterFloorSubSections.Add(ssSubSection);

						break;
					case Common.Enums.RoomMessageFilterReferenceType.ROOMS:
						var roomId = new Guid(filter.ReferenceId);
						if (!filterRoomIds.Contains(roomId)) filterRoomIds.Add(roomId);
						break;
					case Common.Enums.RoomMessageFilterReferenceType.ROOM_CATEGORIES:
						var roomCategoryId = new Guid(filter.ReferenceId);
						if (!filterRoomCategoryIds.Contains(roomCategoryId)) filterRoomCategoryIds.Add(roomCategoryId);
						break;
					default:
						continue;
				}

				if (filterBuildingIds.Any()) roomsQuery = roomsQuery.Where(r => r.BuildingId != null && filterBuildingIds.Contains(r.BuildingId.Value));
				if (filterFloorIds.Any()) roomsQuery = roomsQuery.Where(r => r.FloorId != null && filterFloorIds.Contains(r.FloorId.Value));
				if (filterFloorSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSections.Contains(r.FloorSectionName));
				if (filterFloorSubSections.Any()) roomsQuery = roomsQuery.Where(r => filterFloorSubSections.Contains(r.FloorSubSectionName));
				if (filterRoomIds.Any()) roomsQuery = roomsQuery.Where(r => filterRoomIds.Contains(r.Id));
				if (filterRoomCategoryIds.Any()) roomsQuery = roomsQuery.Where(r => r.CategoryId != null && filterRoomCategoryIds.Contains(r.CategoryId.Value));
			}

			return await roomsQuery.ToListAsync();
		}

		private async Task<IEnumerable<Reservation>> _LoadMessageReservations(IEnumerable<SaveRoomMessageFilter> filters, Guid roomMessageId, DateTime date)
		{
			var reservationIds = new List<string>();

			var appliedFilters = filters.Where(f => f.ReferenceType == RoomMessageFilterReferenceType.RESERVATIONS).ToArray();

			if (appliedFilters.Length == 0)
			{
				return new Reservation[0];
			}

			foreach (var filter in filters)
			{
				reservationIds.Add(filter.ReferenceId);
			}

			return await this._databaseContext.Reservations.Where(r => reservationIds.Contains(r.Id)).ToListAsync();
		}
	}
}
