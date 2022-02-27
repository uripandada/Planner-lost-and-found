using Microsoft.EntityFrameworkCore;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Planner.Application.Interfaces
{
	public class RoomStatusDescription
	{
		//public bool IsClean { get; set; }
		//public bool IsOccupied { get; set; }
		public bool HasArrival { get; set; }
		public bool HasDeparture { get; set; }
		public bool HasStay { get; set; }
		public bool HasDayStay { get; set; }
		public bool HasVipReservation { get; set; }
		public IEnumerable<ReservationStatusDescription> ReservationStatuses { get; set; }
	}
	public class ReservationStatusDescription
	{
		public string ReservationId { get; set; }
		public ReservationStatusType Type { get; set; }
		public bool IsVip { get; set; }
		public bool IsArrival { get; set; }
		public bool HasArrived { get; set; }
		public bool IsDeparture { get; set; }
		public bool HasDeparted { get; set; }
		public bool IsStay { get; set; }
		public bool IsDayStay { get; set; }

		public bool IsInTheFuture { get; set; }
		public bool IsInThePast { get; set; }
		public bool IsCurrent { get; set; }

		public DateTime CheckIn { get; set; }
		public DateTime CheckInDate { get; set; }
		public DateTime CheckOut { get; set; }
		public DateTime CheckOutDate { get; set; }

		public string VipValue { get; set; }
	}

	public class RoomStatusDescriptionResult
	{
		public RccRoomStatus RccRoomStatus { get; set; }
		public RccRoomStatusCode RccRoomStatusCode { get; set; }
		public string Description { get; set; }
	}

	public class RoomHousekeepingDescriptionResult
	{
		public RccHousekeepingStatus RccHousekeepingStatus { get; set; }
		public RccHousekeepingStatusCode RccHousekeepingStatusCode { get; set; }
		public string Description { get; set; }
	}

	public static class RoomStatusClaculator
	{
		public static RccHousekeepingStatusCode CalculateRccRoomHousekeepingStatusCode(this Room room)
		{
			if (room.IsOutOfOrder)
			{
				return RccHousekeepingStatusCode.OOO;
			}
			else if (room.IsOutOfService)
			{
				return RccHousekeepingStatusCode.OOS;
			}
			else if (room.IsDoNotDisturb)
			{
				return RccHousekeepingStatusCode.DNN;
			}


			if (room.IsOccupied) // occupied
			{
				if (room.IsCleaningInProgress) // occupied, cleaning in progress
				{
					return RccHousekeepingStatusCode.OHP;
				}

				if (room.IsClean) // occupied, clean
				{
					if (room.IsInspected) // occupied, clean, inspected
					{
						return RccHousekeepingStatusCode.OHCI;
					}
					else // occupied, clean, NOT inspected
					{
						return RccHousekeepingStatusCode.OHC;
					}
				}
				else // occupied, dirty
				{
					return RccHousekeepingStatusCode.OHD;
				}
			}
			else // vacant
			{
				if (room.IsCleaningInProgress) // vacant, cleaning in progress
				{
					return RccHousekeepingStatusCode.VHP;
				}

				if (room.IsClean) // vacant, clean
				{
					if (room.IsInspected) // vacant, clean, inspected
					{
						return RccHousekeepingStatusCode.VHCI;
					}
					else // vacant, clean, NOT INSPECTED
					{
						return RccHousekeepingStatusCode.VHC;
					}
				}
				else // vacant, dirty
				{
					return RccHousekeepingStatusCode.VHD;
				}
			}
		}

		public static RoomHousekeepingDescriptionResult CalculateCurrentHousekeepingStatus(this Room room)
		{
			return _CalculateCurrentHousekeepingStatus(room.IsOutOfOrder, room.IsOutOfService, room.IsDoNotDisturb, room.IsOccupied, room.IsCleaningInProgress, room.IsClean, room.IsInspected);
		}
		
		public static RoomHousekeepingDescriptionResult CalculateCurrentHousekeepingStatus(this RoomBed bed)
		{
			return _CalculateCurrentHousekeepingStatus(bed.IsOutOfOrder, bed.IsOutOfService, bed.IsDoNotDisturb, bed.IsOccupied, bed.IsCleaningInProgress, bed.IsClean, bed.IsInspected);
		}
		
		private static RoomHousekeepingDescriptionResult _CalculateCurrentHousekeepingStatus(bool isOutOfOrder, bool isOutOfService, bool isDoNotDisturb, bool isOccupied, bool isCleaningInProgress, bool isClean, bool isInspected)
		{
			if (isOutOfOrder)
			{
				return new RoomHousekeepingDescriptionResult
				{
					RccHousekeepingStatus = RccHousekeepingStatus.OutOfOrder,
					RccHousekeepingStatusCode = RccHousekeepingStatus.OutOfOrder.ToCode(),
					Description = "Out of order",
				};
			}

			if (isOutOfService)
			{
				return new RoomHousekeepingDescriptionResult
				{
					RccHousekeepingStatus = RccHousekeepingStatus.OutOfService,
					RccHousekeepingStatusCode = RccHousekeepingStatus.OutOfService.ToCode(),
					Description = "Out of service",
				};
			}

			if (isDoNotDisturb)
			{
				return new RoomHousekeepingDescriptionResult
				{
					RccHousekeepingStatus = RccHousekeepingStatus.DoNotDisturb,
					RccHousekeepingStatusCode = RccHousekeepingStatus.DoNotDisturb.ToCode(),
					Description = "Do not disturb",
				};
			}

			if (isOccupied)
			{
				if (isCleaningInProgress)
				{
					return new RoomHousekeepingDescriptionResult
					{
						RccHousekeepingStatus = RccHousekeepingStatus.OccupiedHkInProgress,
						RccHousekeepingStatusCode = RccHousekeepingStatus.OccupiedHkInProgress.ToCode(),
						Description = "Occupied housekeeping in progress",
					};
				}
				else if (isClean)
				{
					if (isInspected)
					{
						return new RoomHousekeepingDescriptionResult
						{
							RccHousekeepingStatus = RccHousekeepingStatus.OccupiedCleanInspected,
							RccHousekeepingStatusCode = RccHousekeepingStatus.OccupiedCleanInspected.ToCode(),
							Description = "Occupied clean inspected",
						};
					}
					else
					{
						return new RoomHousekeepingDescriptionResult
						{
							RccHousekeepingStatus = RccHousekeepingStatus.OccupiedClean,
							RccHousekeepingStatusCode = RccHousekeepingStatus.OccupiedClean.ToCode(),
							Description = "Occupied clean",
						};
					}
				}
				else
				{
					return new RoomHousekeepingDescriptionResult
					{
						RccHousekeepingStatus = RccHousekeepingStatus.OccupiedDirty,
						RccHousekeepingStatusCode = RccHousekeepingStatus.OccupiedDirty.ToCode(),
						Description = "Occupied dirty",
					};
				}
			}
			else
			{
				if (isCleaningInProgress)
				{
					return new RoomHousekeepingDescriptionResult
					{
						RccHousekeepingStatus = RccHousekeepingStatus.VacantHkInProgress,
						RccHousekeepingStatusCode = RccHousekeepingStatus.VacantHkInProgress.ToCode(),
						Description = "Vacant housekeeping in progress",
					};
				}
				else if (isClean)
				{
					if (isInspected)
					{
						return new RoomHousekeepingDescriptionResult
						{
							RccHousekeepingStatus = RccHousekeepingStatus.VacantCleanInspected,
							RccHousekeepingStatusCode = RccHousekeepingStatus.VacantCleanInspected.ToCode(),
							Description = "Vacant clean inspected",
						};
					}
					else
					{
						return new RoomHousekeepingDescriptionResult
						{
							RccHousekeepingStatus = RccHousekeepingStatus.VacantClean,
							RccHousekeepingStatusCode = RccHousekeepingStatus.VacantClean.ToCode(),
							Description = "Vacant clean",
						};

					}
				}
				else
				{
					return new RoomHousekeepingDescriptionResult
					{
						RccHousekeepingStatus = RccHousekeepingStatus.VacantDirty,
						RccHousekeepingStatusCode = RccHousekeepingStatus.VacantDirty.ToCode(),
						Description = "Vacant dirty",
					};
				}
			}
		}

		public static RoomStatusDescriptionResult CalculateReservationStatusForDate(this RoomBed bed, DateTime localHotelDateTime, IEnumerable<Domain.Entities.Reservation> activeReservations)
		{
			return CalculateReservationStatusForDate(bed.IsOutOfOrder, bed.IsOutOfService, localHotelDateTime, activeReservations);
		}
		
		public static RoomStatusDescriptionResult CalculateReservationStatusForDate(this Room room, DateTime localHotelDateTime, IEnumerable<Domain.Entities.Reservation> activeReservations)
		{
			return CalculateReservationStatusForDate(room.IsOutOfOrder, room.IsOutOfService, localHotelDateTime, activeReservations);
		}

		public static RoomStatusDescriptionResult CalculateReservationStatusForDate(bool isOutOfOrder, bool isOutOfService, DateTime localHotelDateTime, IEnumerable<Domain.Entities.Reservation> activeReservations)
		{
			if (isOutOfOrder)
			{
				return new RoomStatusDescriptionResult
				{
					RccRoomStatus = RccRoomStatus.OutOfOrder,
					RccRoomStatusCode = RccRoomStatusCode.OOO,
					Description = "Out of order",
				};
			}
			else if (isOutOfService)
			{
				return new RoomStatusDescriptionResult
				{
					RccRoomStatus = RccRoomStatus.OutOfService,
					RccRoomStatusCode = RccRoomStatusCode.OOS,
					Description = "Out of service",
				};
			}

			var roomStatusDescription = ReservationsExtensions.GenerateReservationStatusesForTheDate(localHotelDateTime, activeReservations);

			// TODO: FIX THIS AFTER THE MOBILE APP IS UPGRADED! CURRENTLY ONLY FIRST CURRENT RESERVATION IS TAKEN INTO ACCOUNT AND THERE CAN BE MULTIPLE!!!
			// TODO: FIX THIS AFTER THE MOBILE APP IS UPGRADED! CURRENTLY ONLY FIRST CURRENT RESERVATION IS TAKEN INTO ACCOUNT AND THERE CAN BE MULTIPLE!!!
			// TODO: FIX THIS AFTER THE MOBILE APP IS UPGRADED! CURRENTLY ONLY FIRST CURRENT RESERVATION IS TAKEN INTO ACCOUNT AND THERE CAN BE MULTIPLE!!!
			var reservationStatus = roomStatusDescription.ReservationStatuses.FirstOrDefault(rs => rs.IsCurrent || rs.HasDeparted || rs.IsDayStay || rs.IsArrival || rs.IsDeparture || rs.IsStay);
			if (reservationStatus == null)
			{
				return new RoomStatusDescriptionResult
				{
					RccRoomStatus = RccRoomStatus.Vacant,
					RccRoomStatusCode = RccRoomStatusCode.VAC,
					Description = "Vacant",
				};
			}
			else
			{
				if (reservationStatus.IsDayStay)
				{
					if (reservationStatus.HasDeparted)
					{
						return new RoomStatusDescriptionResult
						{
							RccRoomStatus = RccRoomStatus.DayUseToday,
							RccRoomStatusCode = RccRoomStatusCode.DU,
							Description = "Departed, day stay",
						};
					}
					else if (reservationStatus.HasArrived)
					{
						return new RoomStatusDescriptionResult
						{
							RccRoomStatus = RccRoomStatus.DayUseToday,
							RccRoomStatusCode = RccRoomStatusCode.DU,
							Description = "Arrived, day stay",
						};
					}
					else
					{
						return new RoomStatusDescriptionResult
						{
							RccRoomStatus = RccRoomStatus.DayUseToday,
							RccRoomStatusCode = RccRoomStatusCode.DU,
							Description = "Not yet arrived, day stay",
						};
					}
				}
				else if (reservationStatus.IsDeparture)
				{
					if (reservationStatus.HasDeparted)
					{
						return new RoomStatusDescriptionResult
						{
							RccRoomStatus = RccRoomStatus.DepartsTodayCheckedOut,
							RccRoomStatusCode = RccRoomStatusCode.DPD,
							Description = "Departed",
						};
					}
					else
					{
						return new RoomStatusDescriptionResult
						{
							RccRoomStatus = RccRoomStatus.DepartsTodayNotCheckedOut,
							RccRoomStatusCode = RccRoomStatusCode.DPE,
							Description = "Departure",
						};
					}
				}
				else if (reservationStatus.IsArrival)
				{
					if (reservationStatus.HasArrived)
					{
						return new RoomStatusDescriptionResult
						{
							RccRoomStatus = RccRoomStatus.ArrivedTodayCheckedIn,
							RccRoomStatusCode = RccRoomStatusCode.ARD,
							Description = "Arrived",
						};
					}
					else
					{
						return new RoomStatusDescriptionResult
						{
							RccRoomStatus = RccRoomStatus.ArrivesTodayNotCheckedIn,
							RccRoomStatusCode = RccRoomStatusCode.ARD,
							Description = "Arrival",
						};
					}
				}
				else if (reservationStatus.IsStay)
				{
					return new RoomStatusDescriptionResult
					{
						RccRoomStatus = RccRoomStatus.StayToday,
						RccRoomStatusCode = RccRoomStatusCode.STAY,
						Description = "Stay",
					};
				}
				else
				{
					return new RoomStatusDescriptionResult
					{
						RccRoomStatus = RccRoomStatus.OutOfOrder,
						RccRoomStatusCode = RccRoomStatusCode.OOO,
						Description = "ERROR - UKNOWN ROOM STATUS",
					};
				}
			}
		}
	}

	public static class ReservationsExtensions
	{
		public static void SetReservationCheckInTimesToActualCheckInTimes(IEnumerable<RoomWithHotelStructureView> rooms)
		{
			foreach (var room in rooms)
			{
				room.SetReservationCheckInTimesToActualCheckInTimes();
			}
		}
		public static void SetReservationCheckInTimesToActualCheckInTimes(this RoomWithHotelStructureView room)
		{
			var alteredReservations = new List<Domain.Entities.Reservation>();
			foreach (var reservation in room.Reservations)
			{
				if (reservation.ActualCheckIn.HasValue)
				{
					reservation.CheckIn = reservation.ActualCheckIn;
				}
				if (reservation.ActualCheckOut.HasValue)
				{
					reservation.CheckOut = reservation.ActualCheckOut;
				}

				if (reservation.CheckIn.HasValue && reservation.CheckOut.HasValue)
				{
					alteredReservations.Add(reservation);
					reservation.CheckIn = reservation.CheckIn.Value.Date;
					reservation.CheckOut = reservation.CheckOut.Value.Date;
				}
			}
			room.Reservations = alteredReservations;
		}
		public static RoomStatusDescription GenerateReservationStatusesForTheDate(this RoomBed room, DateTime comparisonDateTime, IEnumerable<Domain.Entities.Reservation> activeReservations)
		{
			return GenerateReservationStatusesForTheDate(comparisonDateTime, activeReservations);
		}
		public static RoomStatusDescription GenerateReservationStatusesForTheDate(this Room room, DateTime comparisonDateTime, IEnumerable<Domain.Entities.Reservation> activeReservations)
		{
			return GenerateReservationStatusesForTheDate(comparisonDateTime, activeReservations);
		}
		public static RoomStatusDescription GenerateReservationStatusesForTheDate(DateTime comparisonDateTime, IEnumerable<Domain.Entities.Reservation> activeReservations)
		{
			// TODO: If the calculation is today, isClean and isOccupied are loaded from the room, otherwise are predicted from the reservations.

			if (activeReservations == null || !activeReservations.Any())
			{
				return new RoomStatusDescription
				{
					HasArrival = false,
					HasDeparture = false,
					HasStay = false,
					HasDayStay = false,
					HasVipReservation = false,
					ReservationStatuses = new ReservationStatusDescription[0],
				};
			}

			var dateTime = comparisonDateTime;
			var date = comparisonDateTime.Date;

			var hasArrival = false;
			var hasDeparture = false;
			var hasStay = false;
			var hasVip = false;
			var hasDayStay = false;
			var reservationStatusDescriptions = new List<ReservationStatusDescription>();

			foreach (var reservation in activeReservations)
			{
				var isVip = reservation.Vip.IsNotNull();
				var vipValue = (string)null;
				if (isVip) { 
					hasVip = true;
					vipValue = reservation.Vip;
				}

				if (!reservation.CheckIn.HasValue && !reservation.ActualCheckIn.HasValue && !reservation.ActualCheckOut.HasValue && !reservation.CheckOut.HasValue)
				{
					reservationStatusDescriptions.Add(new ReservationStatusDescription
					{
						ReservationId = reservation.Id,
						IsVip = isVip,
						Type = ReservationStatusType.UNKNOWN,
						VipValue = vipValue,
					});
					continue;
				}

				// If the check in date is not set we can assume it is infinity in the past
				// and since we are checking only for the date value we can safely assume 
				// the +-10 days as default value. The value should be at least more than 2
				// to take into account all time zones and checks.
				var checkIn = date.AddDays(-10);
				var checkInDate = checkIn.Date;
				var checkOut = date.AddDays(10);
				var checkOutDate = checkOut.Date;

				if (reservation.ActualCheckIn.HasValue) checkIn = reservation.ActualCheckIn.Value;
				else if (reservation.CheckIn.HasValue) checkIn = reservation.CheckIn.Value;
				checkInDate = checkIn.Date;

				if (reservation.ActualCheckOut.HasValue) checkOut = reservation.ActualCheckOut.Value;
				else if (reservation.CheckOut.HasValue) checkOut = reservation.CheckOut.Value;
				checkOutDate = checkOut.Date;

				var isInTheFuture = checkInDate > date;
				if (isInTheFuture)
				{
					reservationStatusDescriptions.Add(new ReservationStatusDescription
					{
						IsInTheFuture = isInTheFuture,
						ReservationId = reservation.Id,
						IsVip = isVip,
						Type = ReservationStatusType.IN_THE_FUTURE,
						CheckIn = checkIn,
						CheckInDate = checkInDate,
						CheckOut = checkOut,
						CheckOutDate = checkOutDate,
						VipValue = vipValue,
					});
					continue;
				}

				var isInThePast = checkOutDate < date;
				if (isInThePast)
				{
					reservationStatusDescriptions.Add(new ReservationStatusDescription
					{
						IsInThePast = isInThePast,
						ReservationId = reservation.Id,
						IsVip = isVip,
						Type = ReservationStatusType.IN_THE_PAST,
						CheckIn = checkIn,
						CheckInDate = checkInDate,
						CheckOut = checkOut,
						CheckOutDate = checkOutDate,
						VipValue = vipValue,
					});
					continue;
				}

				var isArrival = checkInDate == date;
				var isDeparture = checkOutDate == date;

				if (!isArrival && !isDeparture)
				{
					hasStay = true;
					reservationStatusDescriptions.Add(new ReservationStatusDescription
					{
						IsCurrent = true,
						IsStay = true,
						ReservationId = reservation.Id,
						IsVip = isVip,
						Type = ReservationStatusType.STAY,
						CheckIn = checkIn,
						CheckInDate = checkInDate,
						CheckOut = checkOut,
						CheckOutDate = checkOutDate,
						VipValue = vipValue,
					});
					continue;
				}
				else
				{
					if (isArrival && isDeparture)
					{
						hasArrival = true;
						hasDeparture = true;
						hasDayStay = true;

						var hasArrived = checkIn < dateTime;
						var hasDeparted = checkOut < dateTime;

						reservationStatusDescriptions.Add(new ReservationStatusDescription
						{
							IsCurrent = true,
							IsArrival = true,
							HasArrived = checkIn < dateTime,
							IsDeparture = true,
							HasDeparted = checkOut < dateTime,
							IsDayStay = true,
							ReservationId = reservation.Id,
							IsVip = isVip,
							Type = ReservationStatusType.DAYSTAY,
							CheckIn = checkIn,
							CheckInDate = checkInDate,
							CheckOut = checkOut,
							CheckOutDate = checkOutDate,
							VipValue = vipValue,
						});
						continue;
					}
					else if (isArrival)
					{
						hasArrival = true;
						reservationStatusDescriptions.Add(new ReservationStatusDescription
						{
							IsCurrent = true,
							IsArrival = true,
							HasArrived = checkIn < dateTime,
							ReservationId = reservation.Id,
							IsVip = isVip,
							Type = ReservationStatusType.ARRIVAL,
							CheckIn = checkIn,
							CheckInDate = checkInDate,
							CheckOut = checkOut,
							CheckOutDate = checkOutDate,
							VipValue = vipValue,
						});
						continue;
					}
					else if (isDeparture)
					{
						hasDeparture = true;
						reservationStatusDescriptions.Add(new ReservationStatusDescription
						{
							IsCurrent = true,
							IsDeparture = true,
							HasDeparted = checkOut < dateTime,
							ReservationId = reservation.Id,
							IsVip = isVip,
							Type = ReservationStatusType.DEPARTURE,
							CheckIn = checkIn,
							CheckInDate = checkInDate,
							CheckOut = checkOut,
							CheckOutDate = checkOutDate,
							VipValue = vipValue,
						});
						continue;
					}
				}

				reservationStatusDescriptions.Add(new ReservationStatusDescription
				{
					ReservationId = reservation.Id,
					IsVip = isVip,
					Type = ReservationStatusType.UNKNOWN,
					CheckIn = checkIn,
					CheckInDate = checkInDate,
					CheckOut = checkOut,
					CheckOutDate = checkOutDate,
					VipValue = vipValue,
				});
			}

			return new RoomStatusDescription
			{
				HasArrival = hasArrival,
				HasDeparture = hasDeparture,
				HasStay = hasStay,
				HasDayStay = hasDayStay,
				HasVipReservation = hasVip,
				ReservationStatuses = reservationStatusDescriptions,
			};
		}
		
		public static CleaningPlans.Commands.GenerateCpsatCleaningPlan.RoomCleaningStatus GenerateCleaningStatus(this RoomWithHotelStructureView room, DateTime cleaningDate, bool isTodaysCleaningPlan)
		{
			// TODO: If the calculation is today, isClean and isOccupied are loaded from the room, otherwise are predicted from the reservations.

			//var hasArrival = false;
			//var hasDeparture = false;
			//var hasStay = false;
			//var hasVip = false;
			////var reservationStatusKeys = new List<string>();

			//foreach (var reservation in room.Reservations)
			//{
			//	var rHasArrival = reservation.CheckIn.HasValue && reservation.CheckIn.Value.Date == cleaningDate;
			//	var rHasDeparture = reservation.CheckOut.HasValue && reservation.CheckOut.Value.Date == cleaningDate;

			//	if (reservation.Vip.IsNotNull())
			//	{
			//		hasVip = true;
			//	}

			//	if (!rHasArrival && !rHasDeparture)
			//	{
			//		hasStay = true;
			//		//reservationStatusKeys.Add(ReservationOccupationType.STAY.ToString());
			//	}
			//	else
			//	{
			//		if (rHasArrival)
			//		{
			//			hasArrival = true;
			//			//reservationStatusKeys.Add(ReservationOccupationType.ARR.ToString());
			//		}
			//		if (rHasDeparture)
			//		{
			//			hasDeparture = true;
			//			//reservationStatusKeys.Add(ReservationOccupationType.DEP.ToString());
			//		}
			//	}
			//}

			var roomStatusDescription = room.GenerateReservationStatusesForTheDate(cleaningDate, room.Reservations);

			var isClean = isTodaysCleaningPlan ? room.IsClean : (room.IsClean && !roomStatusDescription.HasStay && !roomStatusDescription.HasDeparture);
			var isOccupied = isTodaysCleaningPlan ? room.IsOccupied : (roomStatusDescription.HasStay || roomStatusDescription.HasDeparture || roomStatusDescription.HasDayStay);

			return new CleaningPlans.Commands.GenerateCpsatCleaningPlan.RoomCleaningStatus
			{
				HasArrival = roomStatusDescription.HasArrival,
				HasDeparture = roomStatusDescription.HasDeparture,
				HasStay = roomStatusDescription.HasStay,
				IsClean = isClean,
				IsOccupied = isOccupied,
				Room = room,
				HasVipReservation = roomStatusDescription.HasVipReservation,
				VipValues = roomStatusDescription.ReservationStatuses.Where(rs => rs.IsVip).Select(rs => rs.VipValue).ToArray()
				//StatusKeys = reservationStatusKeys.ToArray(),
			};
		}

		/// <summary>
		/// TODO: Fix naming. It is not really refresh cleaning status. It is much more.
		/// </summary>
		/// <param name="cleaning"></param>
		/// <param name="isTodaysCleaningPlan"></param>
		/// <param name="cleaningDate"></param>
		/// <param name="room"></param>
		public static void RefreshCleaningStatus(this CleaningTimelineItemData cleaning, DateTime cleaningDateUtc, string timeZoneId, RoomWithHotelStructureView room)
		{
			var nowUtc = DateTime.UtcNow;
			var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			var localHotelDateTime = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZoneInfo);
			var localCleaningDate = TimeZoneInfo.ConvertTimeFromUtc(cleaningDateUtc, timeZoneInfo);
			var isTodaysCleaningPlan = localCleaningDate.Date == localHotelDateTime.Date;

			var cleaningStatus = room.GenerateCleaningStatus(localCleaningDate, isTodaysCleaningPlan);

			cleaning.ItemTypeKey = "CLEANING";

			cleaning.IsClean = cleaningStatus.IsClean;
			cleaning.IsOccupied = cleaningStatus.IsOccupied;
			cleaning.HasArrival = cleaningStatus.HasArrival;
			cleaning.HasDeparture = cleaningStatus.HasDeparture;
			cleaning.HasStay = cleaningStatus.HasStay;
			cleaning.HasVipReservation = cleaningStatus.HasVipReservation;
			cleaning.VipValues = cleaningStatus.VipValues;

			cleaning.IsRoomAssigned = room.FloorId.HasValue;
			cleaning.IsDoNotDisturb = room.IsDoNotDisturb;
			cleaning.IsOutOfOrder = room.IsOutOfOrder;

			cleaning.HotelId = room.HotelId;
			cleaning.HotelName = room.HotelName;
			cleaning.AreaId = room.AreaId;
			cleaning.AreaName = room.AreaName;
			cleaning.BuildingId = room.BuildingId;
			cleaning.BuildingName = room.BuildingName;
			cleaning.FloorId = room.FloorId;
			cleaning.FloorName = room.FloorName;
			cleaning.FloorNumber = room.FloorNumber;
			cleaning.FloorSectionName = room.FloorSectionName;
			cleaning.FloorSubSectionName = room.FloorSubSectionName;

			cleaning.RoomCategoryId = room.CategoryId;
			cleaning.RoomCategoryName = room.CategoryName;

			cleaning.Reservations = room.Reservations.CreateTimelineReservations(localHotelDateTime, localCleaningDate);
		}

		public static IEnumerable<ExtendedCleaningTimelineItemReservationData> CreateTimelineReservations(this IEnumerable<Reservation> reservations, DateTime localHotelDateTime, DateTime localCleaningDate)
		{
			var statusEnumsMap = new Dictionary<string, RccReservationStatus>
			{
				{ RccReservationStatus.Arrival.ToString(), RccReservationStatus.Arrival },
				{ RccReservationStatus.Arrived.ToString(), RccReservationStatus.Arrived },
				{ RccReservationStatus.Departed.ToString(), RccReservationStatus.Departed },
				{ RccReservationStatus.Canceled.ToString(), RccReservationStatus.Canceled },
				{ RccReservationStatus.Current.ToString(), RccReservationStatus.Current },
				{ RccReservationStatus.Departure.ToString(), RccReservationStatus.Departure },
				{ RccReservationStatus.Unknown.ToString(), RccReservationStatus.Unknown },
			};

			return reservations.Select(r => _CreateTimelineReservation(r, localHotelDateTime, localCleaningDate, statusEnumsMap)).ToArray();
		}

		private static ExtendedCleaningTimelineItemReservationData _CreateTimelineReservation(Reservation r, DateTime localHotelDateTime, DateTime localCleaningDate, Dictionary<string, RccReservationStatus> reservationStatusesMap)
		{
			var rccReservationStatusKey = "";

			// PRESENT
			if(localCleaningDate.Date == localHotelDateTime.Date)
			{
				rccReservationStatusKey = r.RccReservationStatusKey;
			}
			// FUTURE
			else if (localCleaningDate.Date > localHotelDateTime.Date)
			{
				if (r.CheckIn.HasValue)
				{
					// If the check in is in the future from the requested cleaning date
					if(localCleaningDate < r.CheckIn.Value.Date)
					{
						rccReservationStatusKey = RccReservationStatus.Arrival.ToString();
					}
					// If the check in is at the requested cleaning date
					else if (localCleaningDate == r.CheckIn.Value.Date)
					{
						rccReservationStatusKey = RccReservationStatus.Arrival.ToString();
					}
					// If the check in is in the past from the requested cleaning date
					else
					{
						if (r.CheckOut.HasValue)
						{
							if(r.CheckOut.Value.Date < localCleaningDate)
							{
								rccReservationStatusKey = RccReservationStatus.Departed.ToString();
							}
							else if(r.CheckOut.Value.Date == localCleaningDate)
							{
								rccReservationStatusKey = RccReservationStatus.Departure.ToString();
							}
							else
							{
								rccReservationStatusKey = RccReservationStatus.Current.ToString();
							}
						}
						else
						{
							rccReservationStatusKey = RccReservationStatus.Current.ToString();
						}
					}
				}
				else
				{
					rccReservationStatusKey = RccReservationStatus.Unknown.ToString();
				}
			}
			else // HISTORY
			{
				rccReservationStatusKey = r.RccReservationStatusKey;
			}

			var reservation = new ExtendedCleaningTimelineItemReservationData
			{
				ReservationId = r.Id,
				ReservationStatusKey = rccReservationStatusKey,
				RoomId = r.RoomId.HasValue ? r.RoomId.Value : Guid.Empty,
				GuestName = r.GuestName,
				IsVip = r.Vip.IsNotNull(),
				VipTag = r.Vip,
				TypeAndTimeTag = "N/A",
				IsDayUse = r.CheckIn?.Date == r.CheckOut?.Date,
			};

			var reservationStatus = r.RccReservationStatusKey.IsNotNull() && reservationStatusesMap.ContainsKey(r.RccReservationStatusKey) ? reservationStatusesMap[r.RccReservationStatusKey] : RccReservationStatus.Unknown;
			switch (reservationStatus)
			{
				case RccReservationStatus.Arrival:
					reservation.ReservationStatus = "Arrival";
					reservation.TimeString = r.CheckIn.HasValue ? r.CheckIn.Value.ToString("HH:mm") : "";
					reservation.StyleCode = "icofont-login";
					reservation.TypeAndTimeTag = $"{Common.Enums.ReservationOccupationType.ARR.ToString()} {reservation.TimeString}";
					break;
				case RccReservationStatus.Arrived:
					reservation.ReservationStatus = "Arrived";
					reservation.TimeString = r.CheckIn.HasValue ? r.CheckIn.Value.ToString("HH:mm") : "";
					reservation.StyleCode = "icofont-login";
					reservation.TypeAndTimeTag = $"{Common.Enums.ReservationOccupationType.ARR.ToString()} {reservation.TimeString}";
					break;
				case RccReservationStatus.Current:
					reservation.ReservationStatus = "Current";
					reservation.TimeString = "";
					reservation.StyleCode = "icofont-bed";
					reservation.TypeAndTimeTag = $"{Common.Enums.ReservationOccupationType.STAY.ToString()}";
					break;
				case RccReservationStatus.Departure:
					reservation.ReservationStatus = "Departure";
					reservation.TimeString = r.CheckOut.HasValue ? r.CheckOut.Value.ToString("HH:mm") : "";
					reservation.StyleCode = "icofont-logout";
					reservation.TypeAndTimeTag = $"{Common.Enums.ReservationOccupationType.DEP.ToString()} {reservation.TimeString}";
					break;
				case RccReservationStatus.Departed:
					reservation.ReservationStatus = "Departed";
					reservation.TimeString = r.CheckOut.HasValue ? r.CheckOut.Value.ToString("HH:mm") : "";
					reservation.StyleCode = "icofont-logout";
					reservation.TypeAndTimeTag = $"{Common.Enums.ReservationOccupationType.DEP.ToString()} {reservation.TimeString}";
					break;
				case RccReservationStatus.Canceled:
					reservation.ReservationStatus = "Cancelled";
					reservation.TimeString = "";
					reservation.StyleCode = "icofont-close-line-circled";
					break;
				case RccReservationStatus.Unknown:
					reservation.ReservationStatus = "Unknown";
					reservation.TimeString = "";
					reservation.StyleCode = "icofont-question";
					break;
			}

			return reservation;
		}
	}
}
