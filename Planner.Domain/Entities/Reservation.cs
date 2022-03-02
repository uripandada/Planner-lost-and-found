using System;
using System.Collections.Generic;
using System.Linq;

namespace Planner.Domain.Entities
{
	public class Reservation
	{
		public string Id { get; set; }
		public string RoomName { get; set; }
		public string PMSRoomName { get; set; }
		public string BedName { get; set; }
		public string PMSBedName { get; set; }
		public string GuestName { get; set; }
		public DateTime? CheckIn { get; set; }
		public DateTime? ActualCheckIn { get; set; }
		public DateTime? CheckOut { get; set; }
		public DateTime? ActualCheckOut { get; set; }
		public string RccReservationStatusKey { get; set; }
		public int NumberOfAdults { get; set; }
		public int NumberOfChildren { get; set; }
		public int NumberOfInfants { get; set; }
		public string PmsNote { get; set; } 
		public string Vip { get; set; }
		public string Group { get; set; }
		public ReservationOtherProperty[] OtherProperties { get; set; }

		public string HotelId { get; set; }
		public Hotel Hotel { get; set; }
		public Guid? RoomId { get; set; }
		public Room Room { get; set; }
		public Guid? RoomBedId { get; set; }
		public RoomBed RoomBed { get; set; }

		public bool IsActive { get; set; }
		public bool IsSynchronizedFromRcc { get; set; }
		public DateTimeOffset? LastTimeModifiedBySynchronization { get; set; }
		public DateTimeOffset? SynchronizedAt { get; set; }


		public bool IsActiveToday { get; set; }
		/// <summary>
		/// CI, CO, ARR, DEP, STAY, null when it is in the future or in the past, for day stays - ARR|DEP, CI|DEP, CI|CO, ARR|CO (impossible one)
		/// </summary>
		public string ReservationStatusKey { get; set; }
		/// <summary>
		/// CI hh:mm, CO hh:mm, ARR, DEP, STAY, null when it is in the future or in the past, any combination {ARR or CI hh:mm}|{DEP or CO hh:mm} for day stays
		/// </summary>
		public string ReservationStatusDescription { get; set; }
	}

	public static class ReservationEntityExtensions
	{
		public class ReservationStatusChanges
		{
			//public bool HasChanged { get; set; }
			public bool IsToday { get; set; }
			public List<ReservationStatusValue> Statuses { get; set; } = new List<ReservationStatusValue>();
		}

		public class ReservationStatusValue
		{
			/// <summary>
			/// ARR, CI, DEP, CO, STAY,
			/// </summary>
			public string StatusKey { get; set; }

			/// <summary>
			/// ARR, CI hh:mm, DEP, CO hh:mm, STAY, 
			/// </summary>
			public string StatusDescription { get; set; }
		}

		public class ReservationCiCoTimesForCleaningCalculation
		{
			public DateTime CheckIn { get; set; }
			public DateTime CheckOut { get; set; }
		}

		/// <summary>
		/// </summary>
		/// <param name="r"></param>
		/// <param name="defaultCheckInTime">In format "HH:mm"</param>
		/// <returns></returns>
		public static DateTime GetReservationCheckInTimeForCleaningCalculation(this Reservation r, TimeSpan defaultCheckInTime)
		{
			if (r.CheckIn.HasValue)
			{
				if (r.CheckIn.Value.Hour == 0 && r.CheckIn.Value.Minute == 0)
				{
					return r.CheckIn.Value.Add(defaultCheckInTime);
				}

				return r.CheckIn.Value;
			}
			else if (r.ActualCheckIn.HasValue)
			{
				return r.ActualCheckIn.Value;
			}
			else
			{
				return default(DateTime);
			}
		}

		public static ReservationCiCoTimesForCleaningCalculation GetReservationCiCoTimesForCleaningCalculation(this Reservation r, TimeSpan defaultCheckInTime, TimeSpan defaultCheckOutTime)
		{
			var times = new ReservationCiCoTimesForCleaningCalculation
			{
				CheckIn = default(DateTime),
				CheckOut = default(DateTime),
			};

			if (r.CheckIn.HasValue)
			{
				if (r.CheckIn.Value.Hour == 0 && r.CheckIn.Value.Minute == 0)
				{
					times.CheckIn = r.CheckIn.Value.Add(defaultCheckInTime);
				}

				times.CheckIn = r.CheckIn.Value;
			}
			else if (r.ActualCheckIn.HasValue)
			{
				times.CheckIn = r.ActualCheckIn.Value;
			}

			if (r.CheckOut.HasValue)
			{
				if (r.CheckOut.Value.Hour == 0 && r.CheckOut.Value.Minute == 0)
				{
					times.CheckOut = r.CheckOut.Value.Add(defaultCheckOutTime);
				}

				times.CheckOut = r.CheckOut.Value;
			}
			else if (r.ActualCheckOut.HasValue)
			{
				times.CheckOut = r.ActualCheckOut.Value;
			}

			// This means it is the DayStay
			if (times.CheckIn.Date == times.CheckOut.Date && times.CheckOut.Date < times.CheckIn.Date)
			{
				// TODO: WARNING !!! THIS IS A FIX JUST TO PROPERLY CALCULATE CLEANINGS.
				// TODO: WARNING !!! Make a proper resolution of this case - where the reservation is day stay but the actual times are not set.
				times.CheckOut = times.CheckIn.AddDays(0);
				times.CheckIn = times.CheckIn.AddSeconds(-1);
			}

			return times;
		}

		/// <summary>
		/// </summary>
		/// <param name="r"></param>
		/// <param name="defaultCheckOutTime">In format "HH:mm"</param>
		/// <returns></returns>
		public static DateTime GetReservationCheckOutTimeForCleaningCalculation(this Reservation r, TimeSpan defaultCheckOutTime)
		{
			if (r.CheckOut.HasValue)
			{
				if (r.CheckOut.Value.Hour == 0 && r.CheckOut.Value.Minute == 0)
				{
					return r.CheckOut.Value.Add(defaultCheckOutTime);
				}

				return r.CheckOut.Value;
			}
			else if (r.ActualCheckOut.HasValue)
			{
				return r.ActualCheckOut.Value;
			}
			else
			{
				return default(DateTime);
			}
		}

		public static ReservationStatusChanges CalculateReservationStatus(this Reservation reservation, DateTime currentHotelLocalDate, bool isRccCurrentReservation = false, bool isRccDepartedReservation = false)
		{
			var result = new ReservationStatusChanges();
			var isToday = true;

			if(reservation.RccReservationStatusKey == Common.Enums.RccReservationStatus.Arrival.ToString())
			{
				var checkinTime =  "00:00";
				var isCheckInToday = false;
				if (reservation.CheckIn.HasValue)
				{
					checkinTime = reservation.CheckIn.Value.ToString("HH:mm");
					isCheckInToday = reservation.CheckIn.Value.Date == currentHotelLocalDate.Date;
				}

				// This is because all future reservations are sent as arrivals
				if (!isCheckInToday)
				{
					isToday = false;
				}
				
				var statusDescription = "";
				if (checkinTime == "00:00")
				{
					statusDescription = $"ETA today";
				}
				else
				{
					statusDescription = $"ETA {checkinTime}";
				}
				result.Statuses.Add(new ReservationStatusValue { StatusKey = "ARR", StatusDescription = statusDescription });

				// Check for the day stay
				if(reservation.CheckIn.HasValue && reservation.CheckOut.HasValue && reservation.CheckIn.Value.Date == reservation.CheckOut.Value.Date)
				{
					result.Statuses.Add(new ReservationStatusValue { StatusKey = "DEP", StatusDescription = $"ETA {reservation.CheckOut.Value.ToString("HH:mm")}" });
				}
			}
			else if (reservation.RccReservationStatusKey == Common.Enums.RccReservationStatus.Arrived.ToString())
			{
				var checkinTime = "00:00";
				if (reservation.ActualCheckIn.HasValue)
				{
					checkinTime = reservation.ActualCheckIn.Value.ToString("HH:mm");
				}
				else if (reservation.CheckIn.HasValue)
				{
					checkinTime = reservation.CheckIn.Value.ToString("HH:mm");
				}

				result.Statuses.Add(new ReservationStatusValue { StatusKey = "CI", StatusDescription = $"CI {checkinTime}" });

				// Check for the day stay
				if (reservation.CheckIn.HasValue && reservation.CheckOut.HasValue && reservation.CheckIn.Value.Date == reservation.CheckOut.Value.Date)
				{
					result.Statuses.Add(new ReservationStatusValue { StatusKey = "DEP", StatusDescription = $"ETA {reservation.CheckOut.Value.ToString("HH:mm")}" });
				}
			}
			else if (reservation.RccReservationStatusKey == Common.Enums.RccReservationStatus.Current.ToString())
			{
				result.Statuses.Add(new ReservationStatusValue { StatusKey = "STAY", StatusDescription = $"Stay" });
			}
			else if (reservation.RccReservationStatusKey == Common.Enums.RccReservationStatus.Departure.ToString())
			{
				// Check for day stay
				var checkInDateTime = reservation.ActualCheckIn.HasValue ? reservation.ActualCheckIn : reservation.CheckIn;
				var checkOutDateTime = reservation.ActualCheckOut.HasValue ? reservation.ActualCheckOut : reservation.CheckOut;
				if (checkInDateTime.HasValue && checkOutDateTime.HasValue && checkOutDateTime.Value.Date == checkInDateTime.Value.Date)
				{
					result.Statuses.Add(new ReservationStatusValue { StatusKey = "CI", StatusDescription = $"CI {checkInDateTime.Value.ToString("HH:mm")}" });
				}

				var checkOutTime = reservation.CheckOut.HasValue ? reservation.CheckOut.Value.ToString("HH:mm") : "00:00";
				var coStatusDescription = "";
				if (checkOutTime == "00:00")
				{
					coStatusDescription = $"ETD today";
				}
				else
				{
					coStatusDescription = $"ETD {checkOutTime}";
				}
				result.Statuses.Add(new ReservationStatusValue { StatusKey = "DEP", StatusDescription = coStatusDescription });
			}
			else if (reservation.RccReservationStatusKey == Common.Enums.RccReservationStatus.Departed.ToString())
			{
				// Check for day stay
				var checkInDateTime = reservation.ActualCheckIn.HasValue ? reservation.ActualCheckIn : reservation.CheckIn;
				var checkOutDateTime = reservation.ActualCheckOut.HasValue ? reservation.ActualCheckOut : reservation.CheckOut;
				if (checkInDateTime.HasValue && checkOutDateTime.HasValue && checkOutDateTime.Value.Date == checkInDateTime.Value.Date)
				{
					result.Statuses.Add(new ReservationStatusValue { StatusKey = "CI", StatusDescription = $"CI {checkInDateTime.Value.ToString("HH:mm")}" });
				}

				var checkOutTime = "00:00";
				if (reservation.ActualCheckOut.HasValue)
				{
					checkOutTime = reservation.ActualCheckOut.Value.ToString("HH:mm");
				}
				else if (reservation.CheckOut.HasValue)
				{
					checkOutTime = reservation.CheckOut.Value.ToString("HH:mm");
				}

				result.Statuses.Add(new ReservationStatusValue { StatusKey = "CO", StatusDescription = $"CO {checkOutTime}" });
			}
			else
			{
				isToday = false;
			}

			result.IsToday = isToday;

			return result;
		}

		public static ReservationStatusChanges CalculateReservationStatus_OLD(this Reservation reservation, DateTime currentHotelLocalDate, bool isRccCurrentReservation = false, bool isRccDepartedReservation = false)
		{
			var result = new ReservationStatusChanges();

			// If the reservation actualcheckin is set use it, otherwise if the rcc status is "Current" use the CheckIn date time if it is set
			var actualCheckIn = reservation.ActualCheckIn;
			if(actualCheckIn == null && isRccCurrentReservation)
			{
				actualCheckIn = reservation.CheckIn;
			}

			// If the reservation actualcheckout is set use it, otherwise if the rcc status is "Departed" use the CheckOut date time if it is set
			var actualCheckOut = reservation.ActualCheckOut;
			if(actualCheckOut == null && isRccDepartedReservation)
			{
				actualCheckOut = reservation.CheckOut;
			}

			if (actualCheckIn.HasValue)
			{
				var actualCheckInDate = actualCheckIn.Value.Date;

				if (actualCheckInDate == currentHotelLocalDate)
				{
					// CI hh:mm because the actual check in was today
					result.Statuses.Add(new ReservationStatusValue { StatusKey = "CI", StatusDescription = $"CI {actualCheckInDate.ToString("HH:mm")}" });
				}
				else if(actualCheckInDate < currentHotelLocalDate)
				{
					if ((!actualCheckOut.HasValue && !reservation.CheckOut.HasValue))
					{
						// STAY because checkout did not happen yet
						result.Statuses.Add(new ReservationStatusValue { StatusKey = "STAY", StatusDescription = $"STAY" });
					}
					else if (actualCheckOut.HasValue && actualCheckOut.Value.Date > currentHotelLocalDate)
					{
						// STAY because the check out date is in the future
						result.Statuses.Add(new ReservationStatusValue { StatusKey = "STAY", StatusDescription = $"STAY" });
					}
					else if ((!actualCheckOut.HasValue && reservation.CheckOut.HasValue && reservation.CheckOut.Value.Date > currentHotelLocalDate))
					{
						result.Statuses.Add(new ReservationStatusValue { StatusKey = "STAY", StatusDescription = $"STAY" });
					}
				}
			}
			else if (reservation.CheckIn.HasValue)
			{
				var checkInDate = reservation.CheckIn.Value.Date;

				if (checkInDate == currentHotelLocalDate)
				{
					// ARR because the check in should be today but the actual check in date did not register
					var checkoutTime = reservation.CheckIn.Value.ToString("HH:mm");
					var statusDescription = "";
					if (checkoutTime == "00:00")
					{
						statusDescription = $"ETA today";
					}
					else
					{
						statusDescription = $"ETA {checkoutTime}";
					}
					result.Statuses.Add(new ReservationStatusValue { StatusKey = "ARR", StatusDescription = statusDescription });
				}
			}

			if (actualCheckOut.HasValue)
			{
				var actualCheckOutDate = actualCheckOut.Value.Date;

				if (actualCheckOutDate == currentHotelLocalDate)
				{
					// CO hh:mm because the actual check out was today
					result.Statuses.Add(new ReservationStatusValue { StatusKey = "CO", StatusDescription = $"CO {actualCheckOutDate.ToString("HH:mm")}" });
				}
			}
			else if (reservation.CheckOut.HasValue)
			{
				var checkOutDate = reservation.CheckOut.Value.Date;

				if (checkOutDate == currentHotelLocalDate)
				{
					// DEP because the guset didn't yet check out
					var checkoutTime = reservation.CheckOut.Value.ToString("HH:mm");
					var statusDescription = "";
					if (checkoutTime == "00:00")
					{
						statusDescription = $"ETD today";
					}
					else
					{
						statusDescription = $"ETD {checkoutTime}";
					}
					result.Statuses.Add(new ReservationStatusValue { StatusKey = "DEP", StatusDescription = statusDescription });
				}
			}

			result.IsToday = result.Statuses.Count > 0;

			return result;
		}
	}
}
