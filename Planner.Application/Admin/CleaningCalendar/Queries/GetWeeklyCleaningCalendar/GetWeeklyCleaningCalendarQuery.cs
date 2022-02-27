using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Infrastructure;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningCalendar.Queries.GetWeeklyCleaningCalendar
{
	public class CleaningCalendarIntervalResult
	{
		public IEnumerable<CalendarDay> Days { get; set; }
		public IEnumerable<CleaningCalendarRoom> Rooms { get; set; }
	}

	public class CleaningCalendarRoom
	{
		public CleaningCalendarRoom()
		{
			this.Days = new List<CleaningCalendarDay>();
		}
		public string Name { get; set; }
		public string CategoryName { get; set; }
		public List<CleaningCalendarDay> Days { get; set; }
	}

	public class CalendarDay
	{
		public DateTime Date { get; set; }
		public string DayName { get; set; }
		public string DateString { get; set; }
	}

	public class CleaningCalendarDay: CalendarDay
	{
		public CleaningCalendarDay()
		{
			this.Reservations = new List<CleaningCalendarReservation>();
			this.Cleanings = new List<CleaningCalendarCleaning>();
		}

		public List<CleaningCalendarReservation> Reservations { get; set; }
		public List<CleaningCalendarCleaning> Cleanings { get; set; }
	}
	public class CleaningCalendarReservation
	{
		public string ReservationId { get; set; }
		public string GuestName { get; set; }
		public bool IsArrival { get; set; }
		public bool IsDeparture { get; set; }
	}
	public class CleaningCalendarCleaning
	{
		public string CleaningName { get; set; }
		public bool HasRecommendedInterval { get; set; }
		public string RecommendedIntervalFromTimeString { get; set; }
		public string RecommendedIntervalToTimeString { get; set; }
	}

	public class GetWeeklyCleaningCalendarQuery : IRequest<CleaningCalendarIntervalResult>
	{
		public string HotelId { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
	}

	public class GetWeeklyCleaningCalendarQueryHandler : IRequestHandler<GetWeeklyCleaningCalendarQuery, CleaningCalendarIntervalResult>, IAmAdminApplicationHandler, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ICleaningProvider _cleaningProvider;

		public GetWeeklyCleaningCalendarQueryHandler(IDatabaseContext databaseContext, ICleaningProvider cleaningProvider)
		{
			this._databaseContext = databaseContext;
			this._cleaningProvider = cleaningProvider;
		}

		private async Task<IEnumerable<Domain.Entities.Room>> _LoadRooms(string hotelId, DateTime fromDate, DateTime toDate)
		{
			var rooms = await this._databaseContext
				.Rooms
				.Include(r => r.Category)
				.Include(r => r.Reservations.Where(r =>
					// This query selects active reservations per room for a given time period
					((r.ActualCheckIn != null && r.ActualCheckIn <= toDate) || (r.CheckIn != null && r.CheckIn <= toDate))
					&&
					((r.ActualCheckOut != null && r.ActualCheckOut >= fromDate) || (r.CheckOut != null && r.CheckOut >= fromDate))
					)
				)
				.Where(r => r.HotelId == hotelId)
				.ToArrayAsync();

			foreach (var room in rooms)
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

			return rooms;
		}

		private IEnumerable<CleaningCalendarRoom> _GetCleaningCalendarRooms(IEnumerable<CalendarDay> calendarDays, IEnumerable<Domain.Entities.Room> rooms, IEnumerable<Domain.Entities.CleaningPlugin> plugins, TimeSpan defaultCheckInTime, TimeSpan defaultCheckOutTime) 
		{
			var calendarRooms = new List<CleaningCalendarRoom>();
			
			foreach (var room in rooms)
			{
				var calendarRoom = new CleaningCalendarRoom();
				calendarRoom.Name = room.Name;
				calendarRoom.CategoryName = room.Category.Name;
				calendarRooms.Add(calendarRoom);

				foreach (var calendarDay in calendarDays)
				{
					var day = new CleaningCalendarDay
					{
						Date = calendarDay.Date,
						DayName = calendarDay.DayName,
						DateString = calendarDay.DateString
					};
					calendarRoom.Days.Add(day);

					var reservations = new List<Domain.Entities.Reservation>();
					foreach (var reservation in room.Reservations)
					{
						if (reservation.CheckIn > day.Date || reservation.CheckOut < day.Date)
						{
							continue;
						}

						reservations.Add(new Domain.Entities.Reservation 
						{
							ActualCheckIn = reservation.ActualCheckIn,
							IsActive = reservation.IsActive,
							Id = reservation.Id,
							GuestName = reservation.GuestName,
							ActualCheckOut = reservation.ActualCheckOut,
							CheckIn = reservation.CheckIn,
							CheckOut = reservation.CheckOut,
							Hotel = reservation.Hotel,
							HotelId = reservation.HotelId,
							IsSynchronizedFromRcc = reservation.IsSynchronizedFromRcc,
							LastTimeModifiedBySynchronization = reservation.LastTimeModifiedBySynchronization,
							NumberOfAdults = reservation.NumberOfAdults,
							NumberOfChildren = reservation.NumberOfChildren,
							NumberOfInfants = reservation.NumberOfInfants,
							OtherProperties = reservation.OtherProperties,
							PmsNote = reservation.PmsNote,
							PMSRoomName = reservation.PMSRoomName,
							RccReservationStatusKey = reservation.RccReservationStatusKey,
							Room = reservation.Room,
							RoomId = reservation.RoomId,
							RoomName = reservation.RoomName,
							SynchronizedAt = reservation.SynchronizedAt,
							Vip = reservation.Vip
						});
					}

					day.Reservations.AddRange(reservations.Select(reservation => new CleaningCalendarReservation
					{
						GuestName = reservation.GuestName,
						IsArrival = reservation.CheckIn == day.Date,
						IsDeparture = reservation.CheckOut == day.Date,
						ReservationId = reservation.Id
					}));

					day.Cleanings = this._CalculateCleanings(day.Date, room, reservations, plugins, defaultCheckInTime, defaultCheckOutTime);
				}
			}

			return calendarRooms;
		}

		private List<CleaningCalendarCleaning> _CalculateCleanings(DateTime cleaningDate, Domain.Entities.Room room, IEnumerable<Domain.Entities.Reservation> reservations, IEnumerable<Domain.Entities.CleaningPlugin> plugins, TimeSpan defaultCheckInTime, TimeSpan defaultCheckOutTime)
		{
			var cleaningRequestRooms = new List<CleaningProviderRequest.Room>();
			cleaningRequestRooms.Add(new CleaningProviderRequest.Room
			{
				ExternalId = room.ExternalId,
				FloorId = room.FloorId.HasValue ? room.FloorId.Value : Guid.Empty,
				HotelId = room.HotelId,
				RoomId = room.Id,
				IsDoNotDisturb = room.IsDoNotDisturb,
				IsOutOfService = room.IsOutOfOrder,
				Name = room.Name,
				Section = room.FloorSectionName,
				SubSection = room.FloorSubSectionName,
				IsClean = false,
				IsPriority = room.IsCleaningPriority,
				Category = room.Category == null ? null : new CleaningProviderRequest.RoomCategory 
				{
					Id = room.Category.Id,
					//Credits = room.Category.Credits,
					Name = room.Category.Name
				},
				Reservations = reservations == null ? new CleaningProviderRequest.Reservation[0] : reservations.Select(r => {
					//var checkInDate = r.ActualCheckIn.HasValue ? r.ActualCheckIn.Value.Date : (r.CheckIn.HasValue ? r.CheckIn.Value.Date : (DateTime?)null);
					//var checkOutDate = r.ActualCheckOut.HasValue ? r.ActualCheckOut.Value.Date : (r.CheckOut.HasValue ? r.CheckOut.Value.Date : (DateTime?)null);

					return new CleaningProviderRequest.Reservation
					{
						CheckIn = r.GetReservationCheckInTimeForCleaningCalculation(defaultCheckInTime),
						CheckOut = r.GetReservationCheckOutTimeForCleaningCalculation(defaultCheckOutTime),
						ExternalId = room.ExternalId,
						GuestName = r.GuestName,
						Id = r.Id,
						IsActive = r.IsActive,
						OtherProperties = new Dictionary<string, string>(),

						// TODO: REMOVE BELOW PROPERTIES
						// TODO: REMOVE BELOW PROPERTIES
						// TODO: REMOVE BELOW PROPERTIES
						IsCheckedIn = false, //checkInDate.HasValue && checkInDate.Value >= cleaningDate,
						IsCheckedOut = false, //checkOutDate.HasValue && checkOutDate.Value > cleaningDate,
					}; 
				}).ToArray(),
			});

			var cleaningPlugins = plugins.Select(p => new CleaningProviderPlugin 
			{
				BasedOns = p.Data.BasedOns.Select(bo => new CleaningPluginBasedOn 
				{
					CleanDeparture = bo.CleanDeparture ?? false,
					CleanOutOfService = bo.CleanOutOfService ?? false,
					CleanStay = bo.CleanStay ?? false,
					CleanVacant = bo.CleanVacant ?? false,
					FoorIds = bo.FoorIds,
					Id = bo.Id,
					Nights = bo.Nights,
					CleanlinessKey = bo.CleanlinessKey,
					//ProductsTags = bo.ProductsTags,
					ReservationSpaceCategories = bo.ReservationSpaceCategories,
					RoomCategories = bo.Categories.Where(rc => rc.IsSelected).Select(c => new CleaningPluginBasedOnRoomCategory 
					{ 
						CategoryId = c.CategoryId,
						Credits = c.Credits,
					}).ToArray(),
					//RoomIds = bo.Rooms.Select(r => r.RoomId).ToArray(),
					Sections = bo.Sections,
					SubSections = bo.SubSections,
					Type = (CleaningPluginBaseOnType)Enum.Parse(typeof(CleaningPluginBaseOnType), bo.Key),
					
					CleanVacantEveryNumberOfDays = bo.CleanVacantEveryNumberOfDays,
					NightsEveryNumberOfDays = bo.NightsEveryNumberOfDays,
					NightsFromKey = bo.NightsFromKey,
					NightsTypeKey = bo.NightsTypeKey,
					ProductsTagsConsumationIntervalFrom = bo.ProductsTagsConsumationIntervalFrom,
					ProductsTagsConsumationIntervalTo = bo.ProductsTagsConsumationIntervalTo,
					ProductsTagsMustBeConsumedOnTime = bo.ProductsTagsMustBeConsumedOnTime,

					Rooms = bo.Rooms.Select(r => new CleaningPluginRoomCredits { Credits = r.Credits, RoomId = r.RoomId }).ToArray(),
					OtherPropertiesExtended = bo.OtherPropertiesExtended == null ? new CleaningPluginBasedOnOtherProperties[0] : bo.OtherPropertiesExtended.Select(ope => new CleaningPluginBasedOnOtherProperties 
					{ 
						BasedOnOtherPropertiesTypeKey = ope.BasedOnOtherPropertiesTypeKey,
						Key = ope.Key,
						Value = ope.Value,
					}).ToArray(),
					ProductsTagsExtended = bo.ProductsTagsExtended == null ? new CleaningPluginBasedOnProductsTags[0] : bo.ProductsTagsExtended.Select(pte => new CleaningPluginBasedOnProductsTags
					{
						BasedOnProductsTagsTypeKey = pte.BasedOnProductsTagsTypeKey,
						ComparisonValue = pte.ComparisonValue,
						IsCaseSensitive = pte.IsCaseSensitive,
						ProductId = pte.ProductId,
					}).ToArray(),
				}),
				IsNightlyCleaningPlugin = p.Data.IsNightlyCleaningPlugin,
				TypeKey = p.Data.TypeKey,
				ChangeSheets = p.Data.ChangeSheets,
				CleanOnHolidays = p.Data.CleanOnHolidays,
				CleanOnSaturday = p.Data.CleanOnSaturday,
				CleanOnSunday = p.Data.CleanOnSunday,
				Color = p.Data.Color,
				DailyCleaningTimeTypeKey = p.Data.DailyCleaningTimeTypeKey,
				DailyCleaningTypeTimes = p.Data.DailyCleaningTypeTimes,
				Description = "TO SET!!!!!",
				DisplayStyleKey = p.Data.DisplayStyleKey,
				HotelId = room.HotelId,
				Id = p.Id, // ID IS NOT IMPORTANT
				Instructions = p.Data.Instructions,
				IsActive = p.IsActive,
				IsTopRule = p.IsTopRule,
				MonthlyCleaningTypeTimeOfMonthKey = p.Data.MonthlyCleaningTypeTimeOfMonthKey,
				Name = p.Data.Name,
				OrdinalNumber = p.OrdinalNumber,
				PeriodicalIntervals = p.Data.PeriodicalIntervals.Select(pi => new CleaningPluginPeriodicalInterval 
				{ 
					EveryNumberOfDays = pi.EveryNumberOfDays,
					FromDayKey = pi.FromDayKey,
					FromNights = pi.FromNights,
					IntervalTypeKey = pi.IntervalTypeKey,
					NumberOfCleanings = pi.NumberOfCleanings,
					PeriodTypeKey = pi.PeriodTypeKey,
					ToNights = pi.ToNights
				}).ToArray(),
				PeriodicalPostponeSundayCleaningsToMonday = p.Data.PeriodicalPostponeSundayCleaningsToMonday,
				PostponeUntilVacant = p.Data.PostponeUntilVacant,
				StartsCleaningAfter = p.Data.StartsCleaningAfter,
				WeekBasedCleaningTypeWeeks = p.Data.WeekBasedCleaningTypeWeeks,
				WeeklyCleaningTypeFridayTimes = p.Data.WeeklyCleaningTypeFridayTimes,
				WeeklyCleaningTypeMondayTimes = p.Data.WeeklyCleaningTypeMondayTimes,
				WeeklyCleaningTypeSaturdayTimes = p.Data.WeeklyCleaningTypeSaturdayTimes,
				WeeklyCleaningTypeSundayTimes = p.Data.WeeklyCleaningTypeSundayTimes,
				WeeklyCleaningTypeThursdayTimes = p.Data.WeeklyCleaningTypeThursdayTimes,
				WeeklyCleaningTypeTuesdayTimes = p.Data.WeeklyCleaningTypeTuesdayTimes,
				WeeklyCleaningTypeWednesdayTimes = p.Data.WeeklyCleaningTypeWednesdayTimes,
				WeeklyCleanOnFriday = p.Data.WeeklyCleanOnFriday,
				WeeklyCleanOnMonday = p.Data.WeeklyCleanOnMonday,
				WeeklyCleanOnSaturday = p.Data.WeeklyCleanOnSaturday,
				WeeklyCleanOnSunday = p.Data.WeeklyCleanOnSunday,
				WeeklyCleanOnThursday = p.Data.WeeklyCleanOnThursday,
				WeeklyCleanOnTuesday = p.Data.WeeklyCleanOnTuesday,
				WeeklyCleanOnWednesday = p.Data.WeeklyCleanOnWednesday,
				WeeklyTimeFridayTypeKey = p.Data.WeeklyTimeFridayTypeKey,
				WeeklyTimeMondayTypeKey = p.Data.WeeklyTimeMondayTypeKey,
				WeeklyTimeSaturdayTypeKey = p.Data.WeeklyTimeSaturdayTypeKey,
				WeeklyTimeSundayTypeKey = p.Data.WeeklyTimeSundayTypeKey,
				WeeklyTimeThursdayTypeKey = p.Data.WeeklyTimeThursdayTypeKey,
				WeeklyTimeTuesdayTypeKey = p.Data.WeeklyTimeTuesdayTypeKey,
				WeeklyTimeWednesdayTypeKey = p.Data.WeeklyTimeWednesdayTypeKey,
				WeekBasedCleaningDayOfTheWeekKey = p.Data.WeekBasedCleaningDayOfTheWeekKey,
			}).ToArray();

			var cleanings = this._cleaningProvider.CalculateCleanings(cleaningDate, cleaningRequestRooms, cleaningPlugins).Results;
			if (!cleanings.Any())
			{
				return new List<CleaningCalendarCleaning>();
			}

			return cleanings.First().Data.Select(c => 
			{
				var hasRecommendedInterval = c.ShouldNotStartBefore.HasValue && c.ShouldNotEndAfter.HasValue;
				return new CleaningCalendarCleaning
				{
					CleaningName = c.PluginName,
					HasRecommendedInterval = hasRecommendedInterval,
					RecommendedIntervalFromTimeString = hasRecommendedInterval ? c.ShouldNotStartBefore.Value.ToString("HH:mm") : "",
					RecommendedIntervalToTimeString = hasRecommendedInterval ? c.ShouldNotEndAfter.Value.ToString("HH:mm"): "",
				}; 
			}).ToList();
		}

		private async Task<Domain.Entities.CleaningPlugin[]> _LoadCleaningPlugins(string hotelId)
		{
			return await this._databaseContext.CleaningPlugins.Where(cp => cp.HotelId == hotelId).ToArrayAsync();
		}

		public async Task<CleaningCalendarIntervalResult> Handle(GetWeeklyCleaningCalendarQuery request, CancellationToken cancellationToken)
		{
			var firstDateOfPeriod = request.FromDate.Date;
			var lastDateOfPeriod = request.ToDate.Date;

			if(firstDateOfPeriod > lastDateOfPeriod)
			{
				return new CleaningCalendarIntervalResult
				{
					Days = new CalendarDay[0],
					Rooms = new CleaningCalendarRoom[0]
				};
			}

			var hotel = await this._databaseContext
				.Hotels
				.Include(h => h.Settings)
				.FirstOrDefaultAsync(h => h.Id == request.HotelId);

			var defaultCheckOutTime = new TimeSpan(10, 0, 0);
			var defaultCheckInTime = new TimeSpan(14, 0, 0);

			if (hotel.Settings != null)
			{
				if (!TimeSpan.TryParse((hotel.Settings.DefaultCheckOutTime ?? "") + ":00", out defaultCheckOutTime))
				{
					defaultCheckOutTime = new TimeSpan(10, 0, 0);
				}
				if (!TimeSpan.TryParse((hotel.Settings.DefaultCheckInTime ?? "") + ":00", out defaultCheckInTime))
				{
					defaultCheckInTime = new TimeSpan(14, 0, 0);
				}
			}

			var rooms = await this._LoadRooms(request.HotelId, firstDateOfPeriod, lastDateOfPeriod);
			var cleaningPlugins = await this._LoadCleaningPlugins(request.HotelId);
			var calendarDays = this._GenerateCalendarDays(firstDateOfPeriod, lastDateOfPeriod);
			var calendarRooms = this._GetCleaningCalendarRooms(calendarDays, rooms, cleaningPlugins, defaultCheckInTime, defaultCheckOutTime);

			return new CleaningCalendarIntervalResult
			{
				Days = calendarDays,
				Rooms = calendarRooms.OrderBy(r => r.Name).ToArray()
			};
		}

		private IEnumerable<CalendarDay> _GenerateCalendarDays(DateTime firstDateOfPeriod, DateTime lastDateOfPeriod)
		{
			var calendarDays = new List<CalendarDay>();
			var dateOfPeriod = firstDateOfPeriod;
			while (dateOfPeriod <= lastDateOfPeriod)
			{
				var day = new CalendarDay
				{
					Date = dateOfPeriod,
					DayName = dateOfPeriod.DayOfWeek.ToString(),
					DateString = dateOfPeriod.ToString("yyyy-MM-dd")
				};
				calendarDays.Add(day);
				dateOfPeriod = dateOfPeriod.AddDays(1);
			}

			return calendarDays;
		}
	}
}
