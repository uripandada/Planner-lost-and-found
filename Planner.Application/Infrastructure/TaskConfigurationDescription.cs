using System.Collections.Generic;

namespace Planner.Application.Infrastructure
{
	public class TaskConfigurationDescription
	{
		public IEnumerable<TaskConfigurationActionDescription> Actions { get; set; }
		public string Description { get; set; }
	}
	//public class CleaningProvider : ICleaningProvider
	//{
	//	private IEnumerable<CleaningProviderRequest.Room> _rooms { get; set; }
	//	private IEnumerable<ICleaningProviderPlugin> _plugins { get; set; }
	//	private IEnumerable<ICleaningProviderCustomPlugin> _customPlugins { get; set; }

	//	/// <summary>
	//	/// PREREQUISITE: ROOM SHOULD HAVE ONLY CURRENTLY ACTIVE RESERVATIONS SET
	//	/// </summary>
	//	/// <param name="rooms"></param>
	//	/// <param name="plugins"></param>
	//	/// <param name="customPlugins"></param>
	//	/// <returns></returns>
	//	public IEnumerable<ProcessResponse<CleaningProviderRequest.Cleaning[]>> CalculateCleanings(DateTime cleaningDate, IEnumerable<CleaningProviderRequest.Room> rooms, IEnumerable<ICleaningProviderPlugin> plugins, IEnumerable<ICleaningProviderCustomPlugin> customPlugins)
	//	{
	//		this._rooms = rooms;
	//		this._plugins = plugins;
	//		this._customPlugins = customPlugins;

	//		var cleaningFrom = cleaningDate.Date;
	//		var cleaningTo = cleaningFrom.AddDays(1);
	//		var results = new List<ProcessResponse<CleaningProviderRequest.Cleaning[]>>();

	//		// plugins that should be applied on the date selected
	//		var activePlugins = new List<ICleaningProviderPlugin>();
	//		var inactivePlugins = new List<ICleaningProviderPlugin>();

	//		foreach (var plugin in plugins)
	//		{
	//			switch (plugin.CleaningPluginTypeKey)
	//			{
	//				case "DAILY":
	//					activePlugins.Add(plugin);
	//					break;
	//				case "WEEKLY":
	//					if(this._isWeeklyPluginActiveOn(cleaningDate, plugin))
	//					{
	//						activePlugins.Add(plugin);
	//					}
	//					else
	//					{
	//						inactivePlugins.Add(plugin);
	//					}
	//					break;
	//				case "MONTHLY":
	//					if (this._isMonthlyPluginActiveOn(cleaningDate, plugin))
	//					{
	//						activePlugins.Add(plugin);
	//					}
	//					else
	//					{
	//						inactivePlugins.Add(plugin);
	//					}
	//					break;
	//				case "BALANCED":
	//					break;
	//				case "BALANCED_PERIODICAL":
	//					break;
	//				case "WEEK_BASED":
	//					if(this._isWeekBasedPluginActiveOn(cleaningDate, plugin))
	//					{
	//						activePlugins.Add(plugin);
	//					}
	//					else
	//					{
	//						inactivePlugins.Add(plugin);
	//					}
	//					break;
	//				case "NO_CLEANING":
	//					break;
	//			}
	//		}


	//		// If there are no active plugins, there are no cleanings either for the room, or any other room
	//		if(activePlugins.Count == 0)
	//		{
	//			return results;
	//		}

	//		foreach(var room in rooms)
	//		{
	//			var cleaningEventsResult = this._CreateCleaningEvents(room, cleaningDate);
	//			if (cleaningEventsResult.HasError)
	//			{
	//				results.Add(new ProcessResponse<CleaningProviderRequest.Cleaning[]>
	//				{
	//					Data = new CleaningProviderRequest.Cleaning[0],
	//					HasError = true,
	//					IsSuccess = false,
	//					Message = cleaningEventsResult.Message
	//				});
	//				continue;
	//			}

	//			// Events contain cleaning events that are removed from the list
	//			// with application of each cleaning plugin.
	//			var events = cleaningEventsResult.Data;
	//			// Only if this flag is set the cleanings are generated from the cleaning events
	//			var shouldBeCleaned = false;
				
	//			foreach(var plugin in activePlugins)
	//			{
	//				// First check if the plugin start cleaning after days is passed.
	//				// If not - the plugin is not active yet - just skip the plugin.
	//				var startCleaning = _ShouldStartCleaning(plugin.StartsCleaningAfterDays, cleaningDate, room.Reservations);
	//				if (!startCleaning)
	//				{
	//					continue;
	//				}

	//				// If there are no based ons, it applies same as ALL based on.
	//				if(plugin.BasedOns.Count() == 0)
	//				{
	//					// TODO: Needs feedback
	//					// The other option is to apply it as NO cleaning.
	//					shouldBeCleaned = true;
	//					continue;
	//				}

	//				foreach(var basedOn in plugin.BasedOns)
	//				{
	//					var shouldBeCleanedBasedOn = false;
	//					switch (basedOn.Type)
	//					{
	//						case CleaningPluginBaseOnType.OTHER_PROPERTIES:
	//							// TODO: Handle other properties
	//							break;
	//						case CleaningPluginBaseOnType.NIGHTS:
	//							shouldBeCleanedBasedOn = this._ShouldCleanBasedOnNights(cleaningDate, room.Reservations, basedOn.Nights);
	//							break;
	//						case CleaningPluginBaseOnType.OCCUPATION:
	//							var occEvents = this._GetCleaningEventsBasedOnOccupancy(basedOn, events);
	//							shouldBeCleanedBasedOn = occEvents.Count() > 0;
	//							events = occEvents;
	//							break;
	//						case CleaningPluginBaseOnType.PRODUCT_TAG:
	//							shouldBeCleanedBasedOn = this._ShouldCleanBasedOnProductOrTag(room.Reservations, basedOn.ProductsTags);
	//							break;
	//						case CleaningPluginBaseOnType.RESERVATION_SPACE_CATEGORY:
	//							shouldBeCleanedBasedOn = this._ShouldCleanBasedOnReservationSpaceCategory(room.Reservations, basedOn.ReservationSpaceCategories);
	//							break;
	//						case CleaningPluginBaseOnType.ROOM:
	//							shouldBeCleanedBasedOn = this._ShouldCleanBasedOnRoom(basedOn.RoomIds.ToDictionary(rid => rid, rid => true), room);
	//							break;
	//						case CleaningPluginBaseOnType.ROOM_CATEGORY:
	//							var cleanBasedOnCategory = this._GetCleanBasedOnRoomCategory(basedOn.RoomCategories.ToDictionary(rc => rc.CategoryId, rc => rc), room);
	//							if(cleanBasedOnCategory != null)
	//							{
	//								shouldBeCleanedBasedOn = true;
	//								foreach(var e in events)
	//								{
	//									// TODO: FIX BUG WITH IsTopRule AND CREDIT OVERRIDES! CURRENTLY LAST ONE WINS!!!
	//									// TODO: FIX BUG WITH IsTopRule AND CREDIT OVERRIDES! CURRENTLY LAST ONE WINS!!!
	//									// TODO: FIX BUG WITH IsTopRule AND CREDIT OVERRIDES! CURRENTLY LAST ONE WINS!!!
	//									// TODO: FIX BUG WITH IsTopRule AND CREDIT OVERRIDES! CURRENTLY LAST ONE WINS!!!
	//									// TODO: FIX BUG WITH IsTopRule AND CREDIT OVERRIDES! CURRENTLY LAST ONE WINS!!!
	//									// TODO: FIX BUG WITH IsTopRule AND CREDIT OVERRIDES! CURRENTLY LAST ONE WINS!!!
	//									e.Credits = cleanBasedOnCategory.Credits;
	//								}
	//							}
	//							break;
	//						case CleaningPluginBaseOnType.FLOOR:
	//							shouldBeCleanedBasedOn = this._ShouldCleanBasedOnFloor(room, basedOn.FoorIds);
	//							break;
	//						case CleaningPluginBaseOnType.SECTION:
	//							shouldBeCleanedBasedOn = this._ShouldCleanBasedOnSection(room, basedOn.Sections);
	//							break;
	//						case CleaningPluginBaseOnType.SUB_SECTION:
	//							shouldBeCleanedBasedOn = this._ShouldCleanBasedOnSubSection(room, basedOn.SubSections);
	//							break;
	//						case CleaningPluginBaseOnType.ALL:
	//							shouldBeCleanedBasedOn = true;
	//							break;
	//						default:
	//							break;
	//					}

	//					if (shouldBeCleanedBasedOn)
	//					{
	//						shouldBeCleaned = true;
	//					}
	//				}
	//			}

	//			if(shouldBeCleaned && events.Any())
	//			{
	//				// The room should be cleaned according to the cleaning events in the list
	//				results.Add(new ProcessResponse<CleaningProviderRequest.Cleaning[]>
	//				{
	//					Data = events.Select(e => new CleaningProviderRequest.Cleaning
	//					{ 
	//						Credits = e.Credits, // Defaults are loaded from the room category, plugins can change the credits value! TODO: FINISH CREDITS
	//						Description = "",
	//						HasPriority = false,
	//						Room = room,
	//						RoomId = room.Id,
	//						ShouldNotEndAfter = e.To,
	//						ShouldNotStartBefore = e.From,
	//						Type = e.Type
	//					}).ToArray(),
	//					HasError = true,
	//					IsSuccess = false,
	//					Message = cleaningEventsResult.Message
	//				});
	//			}
	//		}

	//		return results;
	//	}

	//	private IEnumerable<CleaningEvent> _GetCleaningEventsBasedOnOccupancy(ICleaningPluginBasedOn basedOnOccupancy, IEnumerable<CleaningEvent> cleaningEvents)
	//	{
	//		var events = new List<CleaningEvent>();

	//		if (basedOnOccupancy.CleanOutOfService)
	//		{
	//			events.AddRange(cleaningEvents.Where(e => e.Type == CleaningProviderRequest.CleaningType.OutOfService));
	//		}

	//		if (basedOnOccupancy.CleanVacant)
	//		{
	//			events.AddRange(cleaningEvents.Where(e => e.Type == CleaningProviderRequest.CleaningType.Vacant));
	//		}

	//		if (basedOnOccupancy.CleanDeparture)
	//		{
	//			events.AddRange(cleaningEvents.Where(e => e.Type == CleaningProviderRequest.CleaningType.Departure));

	//		}

	//		if (basedOnOccupancy.CleanStay)
	//		{
	//			events.AddRange(cleaningEvents.Where(e => e.Type == CleaningProviderRequest.CleaningType.Stay));
	//		}

	//		return events;
	//	}

	//	private bool _ShouldCleanBasedOnRoom(Dictionary<Guid, bool> basedOnRoomMap, CleaningProviderRequest.Room room)
	//	{
	//		return basedOnRoomMap.ContainsKey(room.Id);
	//	}

	//	private ICleaningPluginBasedOnRoomCategory _GetCleanBasedOnRoomCategory(Dictionary<Guid, ICleaningPluginBasedOnRoomCategory> basedOnRoomCategoryMap, CleaningProviderRequest.Room room)
	//	{
	//		if (basedOnRoomCategoryMap.ContainsKey(room.Category.Id))
	//		{
	//			return basedOnRoomCategoryMap[room.Category.Id];
	//			//var c = basedOnRoomCategoryMap[room.Category.Id];
	//			//foreach(var e in cleaningEvents)
	//			//{
	//			//	// TODO: BUG!!!!!! TOP RULES CAN OVERRIDE EACH OTHER BUT NON TOP RULE CAN'T!!!!
	//			//	// TODO: BUG!!!!!! TOP RULES CAN OVERRIDE EACH OTHER BUT NON TOP RULE CAN'T!!!!
	//			//	// TODO: BUG!!!!!! TOP RULES CAN OVERRIDE EACH OTHER BUT NON TOP RULE CAN'T!!!!
	//			//	// TODO: BUG!!!!!! TOP RULES CAN OVERRIDE EACH OTHER BUT NON TOP RULE CAN'T!!!!
	//			//	e.Credits = c.Credits;
	//			//}
	//			//return true;
	//		}
	//		else
	//		{
	//			return null;
	//			//return false;
	//		}
	//	}

	//	private bool _ShouldCleanBasedOnNights(DateTime cleaningDate, CleaningProviderRequest.Reservation[] reservations, IEnumerable<int> nights)
	//	{
	//		if (reservations.Length == 0)
	//			return false;

	//		var oldestReservation = reservations.OrderBy(r => r.CheckIn).First();
	//		var oldestReservationDate = oldestReservation.CheckIn.Date;
	//		var daysAfterCheckIn = _GetNumberOfDaysBetweenDates(cleaningDate, oldestReservationDate); // (int)Math.Round((cleaningDate - oldestReservationDate).TotalDays, 0, MidpointRounding.AwayFromZero);

	//		if(daysAfterCheckIn < 0)
	//		{
	//			return false;
	//		}

	//		return nights.Contains(daysAfterCheckIn);
	//	}

	//	private int _GetNumberOfDaysBetweenDates(DateTime newerDate, DateTime olderDate)
	//	{
	//		return (int)Math.Round((newerDate - olderDate).TotalDays, 0, MidpointRounding.AwayFromZero);
	//	}

	//	private bool _ShouldCleanBasedOnReservationSpaceCategory(CleaningProviderRequest.Reservation[] reservations, IEnumerable<string> reservationSpaceCategories)
	//	{
	//		// if any reservation has an OtherProperty with a key "ReservationSpaceCategory" and if under that key is a value from the list "reservationSpaceCategories"
	//		return reservations.Any(r => r.OtherProperties.ContainsKey("ReservationSpaceCategory") && reservationSpaceCategories.Contains(r.OtherProperties["ReservationSpaceCategory"]));
	//	}

	//	private bool _ShouldCleanBasedOnProductOrTag(CleaningProviderRequest.Reservation[] reservations, IEnumerable<string> productOrTags)
	//	{
	//		return reservations.Any(r => 
	//			(
	//				r.OtherProperties.ContainsKey("Product") && 
	//				productOrTags.Contains(r.OtherProperties["Product"])
	//			) || (
	//				r.OtherProperties.ContainsKey("Tag") && 
	//				productOrTags.Contains(r.OtherProperties["Tag"])
	//			)
	//		);
	//	}

	//	private bool _ShouldCleanBasedOnFloor(CleaningProviderRequest.Room room, IEnumerable<Guid> floorIds)
	//	{
	//		return floorIds.Contains(room.FloorId);
	//	}

	//	private bool _ShouldCleanBasedOnSection(CleaningProviderRequest.Room room, IEnumerable<string> sections)
	//	{
	//		return sections.Contains(room.Section);
	//	}

	//	private bool _ShouldCleanBasedOnSubSection(CleaningProviderRequest.Room room, IEnumerable<string> subSections)
	//	{
	//		return subSections.Contains(room.SubSection);
	//	}



	//	private bool _ShouldStartCleaning(int startCleaningAfterDays, DateTime cleaningDate, CleaningProviderRequest.Reservation[] reservations)
	//	{
	//		if (startCleaningAfterDays <= 0)
	//			return true;

	//		var oldestReservation = reservations.OrderBy(r => r.CheckIn).First();
	//		var oldestReservationDate = oldestReservation.CheckIn.Date;
	//		var daysAfterCheckIn = _GetNumberOfDaysBetweenDates(cleaningDate, oldestReservationDate);

	//		return daysAfterCheckIn > startCleaningAfterDays;
	//	}

	//	private bool _isWeeklyPluginActiveOn(DateTime date, ICleaningProviderPlugin plugin)
	//	{
	//		switch (date.DayOfWeek)
	//		{
	//			case DayOfWeek.Monday:
	//				return plugin.WeeklyCleanOnMonday.HasValue && plugin.WeeklyCleanOnMonday.Value;
	//			case DayOfWeek.Tuesday:
	//				return plugin.WeeklyCleanOnTuesday.HasValue && plugin.WeeklyCleanOnTuesday.Value;
	//			case DayOfWeek.Wednesday:
	//				return plugin.WeeklyCleanOnWednesday.HasValue && plugin.WeeklyCleanOnWednesday.Value;
	//			case DayOfWeek.Thursday:
	//				return plugin.WeeklyCleanOnThursday.HasValue && plugin.WeeklyCleanOnThursday.Value;
	//			case DayOfWeek.Friday:
	//				return plugin.WeeklyCleanOnFriday.HasValue && plugin.WeeklyCleanOnFriday.Value;
	//			case DayOfWeek.Saturday:
	//				return plugin.WeeklyCleanOnSaturday.HasValue && plugin.WeeklyCleanOnSaturday.Value;
	//			case DayOfWeek.Sunday:
	//				return plugin.WeeklyCleanOnSunday.HasValue && plugin.WeeklyCleanOnSunday.Value;
	//		}

	//		return false;
	//	}

	//	private bool _isMonthlyPluginActiveOn(DateTime date, ICleaningProviderPlugin plugin)
	//	{
	//		var dateOfMonth = new DateTime(date.Year, date.Month, 1);
			
	//		switch (plugin.MonthlyCleaningTypeTimeOfMonthKey)
	//		{
	//			case "BEGINNING_OF_MONTH":
	//				// first workday after (including) 1st of month
	//				dateOfMonth = new DateTime(date.Year, date.Month, 1);
	//				break;
	//			case "MIDDLE_OF_MONTH":
	//				// first workday after (including) the 15th of month
	//				dateOfMonth = new DateTime(date.Year, date.Month, 15);
	//				break;
	//			case "END_OF_MONTH":
	//				// last workday before (including) the last day of month
	//				dateOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
	//				break;
	//		}

	//		if(dateOfMonth.DayOfWeek == DayOfWeek.Saturday)
	//		{
	//			dateOfMonth.AddDays(2);
	//		}
	//		else if(dateOfMonth.DayOfWeek == DayOfWeek.Sunday)
	//		{
	//			dateOfMonth.AddDays(1);
	//		}

	//		return date.Date == dateOfMonth.Date;
	//	}
		
	//	private bool _isBalancedPluginActiveOn(DateTime date, ICleaningProviderPlugin plugin)
	//	{
	//		return false;
	//	}
	//	private bool _isBalancedWeeklyPluginActiveOn(DateTime date, ICleaningProviderPlugin plugin)
	//	{
	//		return false;
	//	}
	//	private bool _isWeekBasedPluginActiveOn(DateTime date, ICleaningProviderPlugin plugin)
	//	{
	//		var week = GetIso8601WeekOfYear(date);
	//		return plugin.Weeks.Contains(week);
	//	}

	//	// This presumes that weeks start with Monday.
	//	// Week 1 is the 1st week of the year with a Thursday in it.
	//	private static int GetIso8601WeekOfYear(DateTime time)
	//	{
	//		// Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
	//		// be the same week# as whatever Thursday, Friday or Saturday are,
	//		// and we always get those right
	//		DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
	//		if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
	//		{
	//			time = time.AddDays(3);
	//		}

	//		// Return the week of our adjusted day
	//		return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
	//	}

	//	private enum RoomEventType
	//	{
	//		CHECK_IN,
	//		CHECK_OUT,
	//		NEW_DAY
	//	}
	//	private class RoomEvent
	//	{
	//		public RoomEventType Type { get; set; }
	//		public DateTime At { get; set; }
	//	}
	//	private class CleaningEvent
	//	{
	//		public DateTime From { get; set; }
	//		public DateTime To { get; set; }
	//		public int Credits { get; set; }
	//		public CleaningProviderRequest.CleaningType Type { get; set; }
	//	}

	//	private ProcessResponse<IEnumerable<CleaningEvent>> _CreateCleaningEvents(CleaningProviderRequest.Room room, DateTime cleaningDate)
	//	{
	//		var cleaningFrom = cleaningDate.Date;
	//		var cleaningTo = cleaningFrom.AddDays(1);
	//		var cleaningEvents = new List<CleaningEvent>();

	//		if (room.IsOutOfService)
	//		{
	//			cleaningEvents.Add(new CleaningEvent { From = cleaningFrom, To = cleaningTo, Type = CleaningProviderRequest.CleaningType.OutOfService, Credits = room.Category.Credits });

	//			return new ProcessResponse<IEnumerable<CleaningEvent>>
	//			{
	//				Data = cleaningEvents,
	//				HasError = false,
	//				IsSuccess = true,
	//				Message = "Cleaning events calculated."
	//			};
	//		}

	//		var numberOfStartingActiveReservations = 0;

	//		var activityEvents = new List<RoomEvent>();
	//		foreach (var reservation in room.Reservations)
	//		{
	//			var isCheckInToday = reservation.CheckIn >= cleaningFrom && reservation.CheckIn < cleaningTo;
	//			var isCheckOutToday = reservation.CheckOut >= cleaningFrom && reservation.CheckOut < cleaningTo;

	//			if (reservation.CheckIn < cleaningFrom)
	//			{
	//				numberOfStartingActiveReservations++;
	//			}

	//			if (isCheckInToday)
	//			{
	//				activityEvents.Add(new RoomEvent { Type = RoomEventType.CHECK_IN, At = reservation.CheckIn });
	//			}
	//			if (isCheckOutToday)
	//			{
	//				activityEvents.Add(new RoomEvent { Type = RoomEventType.CHECK_OUT, At = reservation.CheckOut });
	//			}
	//		}

	//		var events = new List<RoomEvent>();
	//		events.Add(new RoomEvent { At = cleaningFrom, Type = RoomEventType.NEW_DAY });
	//		events.AddRange(activityEvents.OrderBy(ae => ae.At).ToArray());
	//		events.Add(new RoomEvent { At = cleaningTo, Type = RoomEventType.NEW_DAY });

	//		//-----------------------

	//		var numberOfEvents = activityEvents.Count;
	//		var numberOfReservations = numberOfStartingActiveReservations;
	//		var isRoomInitiallyDirty = numberOfStartingActiveReservations > 0;
	//		var hasCheckoutInconsistencyError = false;

	//		if (numberOfEvents == 0)
	//		{
	//			if (numberOfReservations == 0)
	//			{
	//				// VACANT CLEANING
	//				cleaningEvents.Add(new CleaningEvent { Type = CleaningProviderRequest.CleaningType.Vacant, From = cleaningFrom, To = cleaningTo, Credits = room.Category.Credits });
	//			}
	//			else
	//			{
	//				// STAY CLEANING
	//				cleaningEvents.Add(new CleaningEvent { Type = CleaningProviderRequest.CleaningType.Stay, From = cleaningFrom, To = cleaningTo, Credits = room.Category.Credits });
	//			}
	//		}
	//		else
	//		{
	//			for (int i = 1; i < events.Count - 1; i++)
	//			{
	//				var e = events[i];

	//				if (e.Type == RoomEventType.CHECK_IN)
	//				{
	//					numberOfReservations++;
	//				}
	//				else if (e.Type == RoomEventType.CHECK_OUT)
	//				{
	//					numberOfReservations--;
	//					if (numberOfReservations == 0)
	//					{
	//						var nextEvent = events[i + 1];
	//						// DEPARTURE CLEANING
	//						cleaningEvents.Add(new CleaningEvent { Type = CleaningProviderRequest.CleaningType.Departure, From = e.At, To = nextEvent.At, Credits = room.Category.Credits });

	//						//this._CreateCleaning(CleaningType.Departure, room, e.Time, nextEvent.Time, false));
	//					}
	//					else if (numberOfReservations < 0)
	//					{
	//						// ERROR!
	//						hasCheckoutInconsistencyError = true;
	//						break;
	//					}
	//				}
	//			}

	//			if (!hasCheckoutInconsistencyError)
	//			{
	//				// Cleanings.Count will be 0 at this point only if there are no check out events / departure cleanings.
	//				if (isRoomInitiallyDirty && cleaningEvents.Count == 0)
	//				{
	//					// REGULAR DIRTY CLEANING only if the room is initially occupied (dirty) and there are no departure cleanings
	//					cleaningEvents.Add(new CleaningEvent { Type = CleaningProviderRequest.CleaningType.Departure, From = cleaningFrom, To = cleaningTo, Credits = room.Category.Credits });
	//					//cleaningEvents.Add(this._CreateCleaning(CleaningType.Stay, room, cleaningFrom, cleaningTo, false));
	//				}
	//				else
	//				{
	//					cleaningEvents.Add(new CleaningEvent { Type = CleaningProviderRequest.CleaningType.Clean, From = cleaningFrom, To = cleaningTo, Credits = room.Category.Credits });
	//					// IRREGULAR CLEANING OF THE CLEAN ROOM??
	//					// TODO: Ask Jonathan about this case!
	//					//cleanings.Add(this._CreateCleaning(CleaningType.Stay, room, cleaningFrom, cleaningTo, false));
	//				}
	//			}
	//		}

	//		if (hasCheckoutInconsistencyError)
	//		{
	//			return new ProcessResponse<IEnumerable<CleaningEvent>>
	//			{
	//				Data = new CleaningEvent[0],
	//				HasError = true,
	//				IsSuccess = false,
	//				Message = $"There are more checkouts than reservations for the room {room.Name} at date {cleaningDate}"
	//			};
	//		}
	//		else
	//		{
	//			return new ProcessResponse<IEnumerable<CleaningEvent>>
	//			{
	//				Data = cleaningEvents,
	//				HasError = false,
	//				IsSuccess = true,
	//				Message = "Cleaning events calculated."
	//			};
	//		}
	//	}
	//}
}
