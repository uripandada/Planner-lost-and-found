using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planner.Common.Infrastructure
{
	public class CleaningProviderRequest
	{
		public class Reservation
		{
			public string Id { get; set; }
			public string ExternalId { get; set; }
			public string GuestName { get; set; }

			/// <summary>
			/// If reservation.ActualCheckIn != null then use that value
			/// If reservation.ActualCheckIn == null and reservation.CheckIn != null
			///		then use that value and if the time is "00:00" set it to default check in time
			///		from the hotel settings.
			/// </summary>
			public DateTime CheckIn { get; set; }

			/// <summary>
			/// If reservation.ActualCheckOut != null then use that value
			/// If reservation.ActualCheckOut == null and reservation.CheckOut != null
			///		then use that value and if the time is "00:00" set it to default check out time
			///		from the hotel settings.
			/// </summary>
			public DateTime CheckOut { get; set; }

			public bool IsCheckedIn { get; set; }
			public bool IsCheckedOut { get; set; }

			public Dictionary<string, string> OtherProperties { get; set; }

			public bool IsActive { get; set; }
		}

		public class Room
		{
			public Guid RoomId { get; set; }
			public Guid? BedId { get; set; }
			public bool IsBed { get; set; }
			public string HotelId { get; set; }
			public string ExternalId { get; set; }
			public string Name { get; set; }

			public Guid FloorId { get; set; }
			public string Section { get; set; }
			public string SubSection { get; set; }


			public bool IsOutOfService { get; set; }
			public bool IsDoNotDisturb { get; set; }
			public bool IsPriority { get; set; }
			public bool IsClean { get; set; }

			public DateTime? PreviousCleaningDate { get; set; }


			public RoomCategory Category { get; set; }
			public Reservation[] Reservations { get; set; }
		}

		public class RoomTest: Room
		{
			public string Description { get; set; }
		}

		public class RoomCategory
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			//public int Credits { get; set; }
		}

		public class Cleaning
		{
			public Guid RoomId { get; set; }
			public Guid? BedId { get; set; }
			public Room Room { get; set; }
			public DateTime? ShouldNotStartBefore { get; set; }
			public DateTime? ShouldNotEndAfter { get; set; }
			public int Credits { get; set; }
			public string Description { get; set; }
			public string PluginName { get; set; }
			public bool IsChangeSheets { get; set; }
			public bool IsPriority { get; set; }
			public Guid PluginId { get; set; }

			public bool HasPriority { get; set; }
			public CleaningType Type { get; set; }
		}


		public enum CleaningType
		{
			Stay,
			Vacant,
			OutOfService,
			Departure,
			//Clean
		}

	}

	public interface ICleaningProvider
	{
		CleaningGeneratorResponse CalculateCleanings(DateTime cleaningDate, IEnumerable<CleaningProviderRequest.Room> rooms, IEnumerable<CleaningProviderPlugin> plugins);
	}

	public interface ICleaningProviderCustomPlugin
	{
		bool ShouldTheRoomBeCleaned(CleaningProviderRequest.Room room);
	}

	public class CleaningProviderPlugin
	{


		public string HotelId { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; }
		public bool IsTopRule { get; set; }
		public int OrdinalNumber { get; set; }


		public IEnumerable<CleaningPluginPeriodicalInterval> PeriodicalIntervals { get; set; }
		public bool? PeriodicalPostponeSundayCleaningsToMonday { get; set; }
		public bool ChangeSheets { get; set; }
		public bool IsNightlyCleaningPlugin { get; set; }
		public bool CleanOnHolidays { get; set; }
		public bool CleanOnSaturday { get; set; }
		public bool CleanOnSunday { get; set; }
		public string Color { get; set; }
		public IEnumerable<string> DailyCleaningTypeTimes { get; set; }
		public string DailyCleaningTimeTypeKey { get; set; }
		public string DisplayStyleKey { get; set; }
		public string Instructions { get; set; }
		public string MonthlyCleaningTypeTimeOfMonthKey { get; set; }
		public bool PostponeUntilVacant { get; set; }
		public int? StartsCleaningAfter { get; set; }
		public string TypeKey { get; set; }
		public IEnumerable<int> WeekBasedCleaningTypeWeeks { get; set; }
		public bool? WeeklyCleanOnMonday { get; set; }
		public bool? WeeklyCleanOnTuesday { get; set; }
		public bool? WeeklyCleanOnWednesday { get; set; }
		public bool? WeeklyCleanOnThursday { get; set; }
		public bool? WeeklyCleanOnFriday { get; set; }
		public bool? WeeklyCleanOnSaturday { get; set; }
		public bool? WeeklyCleanOnSunday { get; set; }

		public IEnumerable<string> WeeklyCleaningTypeMondayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeTuesdayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeWednesdayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeThursdayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeFridayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeSaturdayTimes { get; set; }
		public IEnumerable<string> WeeklyCleaningTypeSundayTimes { get; set; }

		public string WeeklyTimeMondayTypeKey { get; set; }
		public string WeeklyTimeTuesdayTypeKey { get; set; }
		public string WeeklyTimeWednesdayTypeKey { get; set; }
		public string WeeklyTimeThursdayTypeKey { get; set; }
		public string WeeklyTimeFridayTypeKey { get; set; }
		public string WeeklyTimeSaturdayTypeKey { get; set; }
		public string WeeklyTimeSundayTypeKey { get; set; }

		public string WeekBasedCleaningDayOfTheWeekKey { get; set; } // MONDAY; TUESDAY; WEDNESDAY,...

		public IEnumerable<CleaningPluginBasedOn> BasedOns { get; set; }
	}

	public class CleaningPluginBasedOnTest: CleaningPluginBasedOn
	{
		public string Description { get; set; }
	}

	public enum CleaningPluginBaseOnType
	{
		OTHER_PROPERTIES,
		ALL,
		OCCUPATION,
		ROOM,
		ROOM_CATEGORY,
		NIGHTS,
		RESERVATION_SPACE_CATEGORY,
		PRODUCT_TAG,
		FLOOR,
		SECTION,
		SUB_SECTION,
		CLEANLINESS
	}

	public class CleaningPluginPeriodicalInterval
	{
		public int NumberOfCleanings { get; set; }
		public int EveryNumberOfDays { get; set; }
		public int FromNights { get; set; }
		public int ToNights { get; set; }
		public string FromDayKey { get; set; } // CHECK_IN, FIRST_MONDAY, FIRST_TUESDAY, FIRST_WEDNESDAY, FIRST_THURSDAY, FIRST_FRIDAY, FIRST_SATURDAY, FIRST_SUNDAY
		public string PeriodTypeKey { get; set; } // BALANCE_OVER_RESERVATION_DURATION, BALANCE_OVER_PERIOD, ONCE_EVERY_N_DAYS
		public string IntervalTypeKey { get; set; } // FROM, MORE_THAN
	}
	public class CleaningPluginBasedOn
	{
		public string Id { get; set; }
		public CleaningPluginBaseOnType Type { get; set; }
		public bool CleanDeparture { get; set; }
		public bool CleanStay { get; set; }
		public bool CleanVacant { get; set; }
		public int? CleanVacantEveryNumberOfDays { get; set; }
		public bool CleanOutOfService { get; set; }


		public bool? ProductsTagsMustBeConsumedOnTime { get; set; }
		public DateTime? ProductsTagsConsumationIntervalFrom { get; set; }
		public DateTime? ProductsTagsConsumationIntervalTo { get; set; }

		public string CleanlinessKey { get; set; }
		public string NightsTypeKey { get; set; }
		public int? NightsEveryNumberOfDays { get; set; }
		public string NightsFromKey { get; set; }


		//public IEnumerable<Guid> RoomIds { get; set; }
		public IEnumerable<Guid> FoorIds { get; set; }
		public IEnumerable<string> Sections { get; set; }
		public IEnumerable<string> SubSections { get; set; }
		public IEnumerable<int> Nights { get; set; }
		public IEnumerable<string> ReservationSpaceCategories { get; set; }
		//public IEnumerable<string> ProductsTags { get; set; }

		public IEnumerable<CleaningPluginRoomCredits> Rooms { get; set; }
		//public IEnumerable<CleaningPluginKeyValue> OtherProperties { get; set; }
		public IEnumerable<CleaningPluginBasedOnRoomCategory> RoomCategories { get; set; }


		public IEnumerable<CleaningPluginBasedOnOtherProperties> OtherPropertiesExtended { get; set; }
		public IEnumerable<CleaningPluginBasedOnProductsTags> ProductsTagsExtended { get; set; }

	}
	public class CleaningPluginBasedOnRoomCategory
	{
		public Guid CategoryId { get; set; }
		public int Credits { get; set; }
	}
	public class CleaningPluginBasedOnOtherProperties
	{
		/// <summary>
		/// BasedOnOtherPropertiesType enum
		/// </summary>
		public string BasedOnOtherPropertiesTypeKey { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
	public class CleaningPluginBasedOnProductsTags
	{       
		/// <summary>
		/// BasedOnProductsTagsType enum
		/// </summary>
		public string BasedOnProductsTagsTypeKey { get; set; }
		public bool IsCaseSensitive { get; set; }
		public string ComparisonValue { get; set; }
		public string ProductId { get; set; }
	}
	public class CleaningPluginRoomCredits
	{
		public Guid RoomId { get; set; }
		public int? Credits { get; set; }
	}
	public class CleaningPluginKeyValue
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}


	public enum LogMessageSeverity
	{
		EXCEPTION = 1,
		ERROR = 2,
		INFO = 3,
		WARNING = 4,
	}

	public enum CleaningGeneratorMessageType
	{


	}

	public class CleaningGeneratorLogMessage
	{
		public Guid Id { get; set; }
		public DateTime At { get; set; }
		public string Message { get; set; }
		public string RoomDescription { get; set; }
		public string ReservationsDescription { get; set; }
		public string ReservationsEventsDescription { get; set; }
		public string PluginEventsDescription { get; set; }
		public string OrderedPluginsDescription { get; set; }
		public string CleaningEventsDescription { get; set; }
		public string CleaningsDescription { get; set; }
		public Guid GenerationId { get; set; }
	}

	public class CleaningGeneratorResponse
	{
		public List<ProcessResponse<CleaningProviderRequest.Cleaning[]>> Results { get; set; }
		public List<CleaningGeneratorLogMessage> LogMessages { get; set; }
	}

	public class CleaningProvider : ICleaningProvider
	{
		//private IEnumerable<CleaningProviderRequest.Room> _rooms { get; set; }
		//private IEnumerable<CleaningProviderPlugin> _plugins { get; set; }
		//private IEnumerable<ICleaningProviderCustomPlugin> _customPlugins { get; set; }

		/// <summary>
		/// PREREQUISITE: ROOM SHOULD HAVE ONLY CURRENTLY ACTIVE RESERVATIONS SET
		/// </summary>
		/// <param name="rooms"></param>
		/// <param name="plugins"></param>
		/// <param name="customPlugins"></param>
		/// <returns></returns>
		/// 
		public CleaningGeneratorResponse CalculateCleanings(DateTime cleaningDate, IEnumerable<CleaningProviderRequest.Room> rooms, IEnumerable<CleaningProviderPlugin> plugins)
		{
			var generationId = Guid.NewGuid();

			var cleaningFrom = cleaningDate.Date;
			var cleaningTo = cleaningFrom.AddDays(1);
			var response = new CleaningGeneratorResponse
			{
				LogMessages = new List<CleaningGeneratorLogMessage>(),
				Results = new List<ProcessResponse<CleaningProviderRequest.Cleaning[]>>(),
			};

			// plugins that should be applied on the date selected
			var activePlugins = new List<CleaningProviderPlugin>();
			var inactivePlugins = new List<CleaningProviderPlugin>();

			// First check which plugins should be active on the day of the cleaning.
			foreach (var plugin in plugins)
			{
				// If there are no based ons, the plugin is skipped before the checks.
				if(plugin.BasedOns.Count() == 0)
				{
					inactivePlugins.Add(plugin);
					continue;
				}

				switch (plugin.TypeKey)
				{
					case "DAILY":
						activePlugins.Add(plugin);
						break;
					case "WEEKLY":
						if (this._isWeeklyPluginActiveOn(cleaningDate, plugin))
						{
							activePlugins.Add(plugin);
						}
						else
						{
							inactivePlugins.Add(plugin);
						}
						break;
					case "MONTHLY":
						if (this._isMonthlyPluginActiveOn(cleaningDate, plugin))
						{
							activePlugins.Add(plugin);
						}
						else
						{
							inactivePlugins.Add(plugin);
						}
						break;
					case "PERIODICAL":
						// Periodical plugins are based on reservation checkin and duration of the stay so we can't exclude them at this point
						activePlugins.Add(plugin);
						break;
					case "WEEK_BASED":
						if (this._isWeekBasedPluginActiveOn(cleaningDate, plugin))
						{
							activePlugins.Add(plugin);
						}
						else
						{
							inactivePlugins.Add(plugin);
						}
						break;
					case "NO_CLEANING":
						break;
				}
			}

			response.LogMessages.Add(new CleaningGeneratorLogMessage
			{
				At = DateTime.UtcNow,
				GenerationId = generationId,
				Id = Guid.NewGuid(),
				Message = $"Generation started. Active plugins: {string.Join(", ", activePlugins.Select(ap => ap.Name).ToArray())}. Inactive plugins: {string.Join(", ", inactivePlugins.Select(ip => ip.Name).ToArray())}.",
			});

			// If there are no active plugins, there are no cleanings
			if (activePlugins.Count == 0)
			{
				response.LogMessages.Add(new CleaningGeneratorLogMessage
				{
					At = DateTime.UtcNow,
					GenerationId = generationId,
					Id = Guid.NewGuid(),
					Message = $"Generation ended. No active plugins.",
				});

				return response;
			}

			foreach (var room in rooms)
			{
				var logMessage = new CleaningGeneratorLogMessage
				{
					At = DateTime.UtcNow,
					GenerationId = generationId,
					Id = Guid.NewGuid(),
					Message = $"",
					RoomDescription = this._GenerateRoomLogDescription(room),
					ReservationsDescription = this._GenerateReservationsLogDescription(room),
				};

				// 1. Generate the list of reservation events based on currently active reservations: check ins, check outs, out of services,...
				// 2. Generate the list of plugin events for each active plugin based on reservation events and plugin configuration.
				// 3. Order active plugins so the top rule ones are first and then sort by ordinal number from zero up.
				// 4. For each active plugin, go through the plugin events and create a cleaning for the plugin event ID if the cleaning is not yet generated.

				// Cleaning events for a room are based on a cleaning date and active reservations
				// Cleaning events format | CHECK_IN, CHECK_OUT, CHECK_IN, CHECK_IN, CHECK_OUT |
				// Required to calculate when the room is vacant or has arrivals, stays, and/or departures.
				var reservationEventsResult = this._CreateReservationEvents(room, cleaningDate);
				if (reservationEventsResult.HasError)
				{
					response.Results.Add(new ProcessResponse<CleaningProviderRequest.Cleaning[]>
					{
						Data = new CleaningProviderRequest.Cleaning[0],
						HasError = true,
						IsSuccess = false,
						Message = reservationEventsResult.Message
					});

					logMessage.Message = $"Error while creating reservation events. No cleanings generated. " + (reservationEventsResult.Message ?? "");
					response.LogMessages.Add(logMessage);

					continue;
				}

				logMessage.ReservationsEventsDescription = this._GenerateReservationsEventsDescription(reservationEventsResult.Data);

				var pluginEventsMap = new Dictionary<Guid, IEnumerable<CleaningEvent>>();

				// For each of the active plugins, create a list of sequential events.
				foreach (var plugin in activePlugins)
				{
					// Check the start cleaning treshold date.
					var shouldStartCleaning = _ShouldStartCleaning(plugin.StartsCleaningAfter.HasValue ? plugin.StartsCleaningAfter.Value : 0, cleaningDate, room.Reservations);
					if (!shouldStartCleaning)
					{
						// Treshold date is not reached yet - skip the plugin.
						continue;
					}

					// This check is required only for periodical plugins since they can't be checked before the rooms and plugins loop
					if(plugin.TypeKey == "PERIODICAL")
					{
						// Periodical plugins must be checked on a per room + reservation level
						var isPeriodicalPluginActive = this._isPeriodicalPluginActiveOn(cleaningDate, plugin, room.Reservations);

						// Plugin is not active on the cleaning date - skip the plugin.
						if (!isPeriodicalPluginActive)
						{
							continue;
						}
					}

					var pluginEvents = JsonSerializer.Deserialize<IEnumerable<CleaningEvent>>(JsonSerializer.Serialize(reservationEventsResult.Data));
					pluginEvents = this._CalculateBasedOnCleaningEvents(plugin.BasedOns, cleaningDate, room, pluginEvents);
					if (pluginEvents.Any())
					{
						pluginEventsMap.Add(plugin.Id, pluginEvents);
					}
				}

				logMessage.PluginEventsDescription = this._GeneratePluginsEventsDescription(pluginEventsMap, activePlugins);

				// pluginEventsMap at this point contains a list of applicable events per cleaning plugin.
				// Those applicable events are defined by the cleaning plugin.
				if (pluginEventsMap.Any())
				{
					var cleaningEventsMap = new Dictionary<int, CleaningEvent>();
					var cleaningEventsPluginsMap = new Dictionary<int, CleaningProviderPlugin>();
					var pluginsMap = activePlugins.ToDictionary(p => p.Id, p => p);
					var orderedPlugins = activePlugins.OrderBy(p => !p.IsTopRule).ThenBy(p => p.OrdinalNumber).ToArray();

					logMessage.OrderedPluginsDescription = this._GenerateOrderedPluginsDescription(orderedPlugins);

					// Reservation events are generated from reservations. Reservation events are: CheckIn, CheckOut, DND,...
					// Plugin events are generated from reservation events. Plugin events are a sub set of reservation events per plugin. Each plugin has it's own sub set of reservation events.
					// Cleaning events are generated from plugin events. Cleaning events is a distinct list of plugin events, matched with "most powerful" plugin and with filtered out reservation events that have no matching plugin.
					// Final cleaning events list is merged from plugin events back by the ID which is unique for each reservation/plugin event.

					// This loop will pick cleaning events by priority
					foreach (var plugin in orderedPlugins)
					{
						if (!pluginEventsMap.ContainsKey(plugin.Id)) 
							continue;

						var eventIndex = 0;
						foreach(var pluginEvent in pluginEventsMap[plugin.Id])
						{
							// WARNING: Tricky part of the algorithm.
							// If the plugin event is already in the cleaning events map - practically it means that there was already a "more important" plugin that handled the cleaning.
							if (!cleaningEventsMap.ContainsKey(pluginEvent.Id))
							{
								cleaningEventsMap.Add(pluginEvent.Id, pluginEvent);
								cleaningEventsPluginsMap.Add(pluginEvent.Id, plugin);
							}
							
							// Only non departure cleanings are scheduled at specific times
							// TODO: Ask Jonathan about it. If the plugin has any "Specific time" defined, does that apply to departure
							// cleanings also since the departure cleanings are defined by the guest checkout.
							if(pluginEvent.Type != CleaningProviderRequest.CleaningType.Departure)
							{
								this._UpdatePluginEventSpecificTimes(plugin, cleaningDate, pluginEvent, eventIndex);
							}

							eventIndex++;
						}
					}

					logMessage.CleaningEventsDescription = this._GenerateCleaningEventsDescription(cleaningEventsMap, cleaningEventsPluginsMap);

					var cleaningProcessResponse = new ProcessResponse<CleaningProviderRequest.Cleaning[]>
					{
						Data = cleaningEventsMap.Select(kvp =>
						{
							var cleaningPlugin = cleaningEventsPluginsMap[kvp.Key];
							var cleaning = new CleaningProviderRequest.Cleaning
							{
								Credits = 0,
								Description = "",
								PluginName = cleaningPlugin.Name,
								PluginId = cleaningPlugin.Id,
								HasPriority = false,
								Room = room,
								RoomId = room.RoomId,
								BedId = room.BedId,
								ShouldNotEndAfter = kvp.Value.To,
								ShouldNotStartBefore = kvp.Value.From,
								Type = kvp.Value.Type,
								IsChangeSheets = cleaningPlugin.ChangeSheets,
								IsPriority = room.IsPriority,
							};

							this._SetCleaningCredits(cleaning, cleaningPlugin);

							return cleaning;
						}).ToArray(),
						HasError = false,
						IsSuccess = true,
						Message = reservationEventsResult.Message
					};

					logMessage.CleaningsDescription = this._GenerateCleaningsDescription(cleaningProcessResponse.Data);

					// The room should be cleaned according to the cleaning events in the list
					response.Results.Add(cleaningProcessResponse);

					logMessage.Message = "Cleanings generated.";
					response.LogMessages.Add(logMessage);
				}
				else
				{
					logMessage.Message = "Nothing generated. No applicable plugins for available reservations.";
					response.LogMessages.Add(logMessage);
				}
			}

			response.LogMessages.Add(new CleaningGeneratorLogMessage
			{
				At = DateTime.UtcNow,
				GenerationId = generationId,
				Id = Guid.NewGuid(),
				Message = $"Generation finished.",
			});


			return response;
		}

		/// <summary>
		/// Warning: Unpredictable behavior in case of multiple basedOn -> Room/RoomCategory.
		/// The algorithm just takes the first eligible, ordered as they were sent into the method.
		/// </summary>
		/// <param name="cleaning"></param>
		/// <param name="cleaningPlugin"></param>
		private void _SetCleaningCredits(CleaningProviderRequest.Cleaning cleaning, CleaningProviderPlugin cleaningPlugin)
		{
			// First check if the based on room exists and set credits from there.
			// WARNING: Unpredictable behavior. The algorithm will take first basedOn.Room that matches the condition. Possible unpredicted behavior in case of multiple.
			var basedOnRoom = cleaningPlugin.BasedOns.FirstOrDefault(bo => bo.Type == CleaningPluginBaseOnType.ROOM && bo.Rooms.Any(r => r.RoomId == cleaning.RoomId));
			if(basedOnRoom != null)
			{
				cleaning.Credits = basedOnRoom.Rooms.First(r => r.RoomId == cleaning.RoomId).Credits ?? 0;
				return;
			}

			// At this point the basedOnRoom doesn't exist so check for the basedOnRoomCategory.
			// WARNING: Unpredictable behavior. The algorithm will take first basedOn.RoomCategory that matches the condition. Possible unpredicted behavior in case of multiple.
			var basedOnRoomCategory = cleaningPlugin.BasedOns.FirstOrDefault(bo => bo.Type == CleaningPluginBaseOnType.ROOM_CATEGORY && bo.RoomCategories.Any(rc => rc.CategoryId == cleaning.Room?.Category?.Id));
			if (basedOnRoomCategory != null)
			{
				cleaning.Credits = basedOnRoomCategory.RoomCategories.First(rc => rc.CategoryId == cleaning.Room?.Category?.Id).Credits;
				return;
			}
		}

		private void _UpdateEventTime(bool? cleanOnWeekday, string timeTypeKey, IEnumerable<string> cleaningTimes, CleaningEvent pluginEvent, int eventIndex)
		{
			if ((cleanOnWeekday ?? false) && timeTypeKey == "SPECIFIC_TIMES")
			{
				var mondayCleaningTimeString = cleaningTimes.ElementAtOrDefault(eventIndex);
				if (mondayCleaningTimeString.IsNotNull())
				{
					var fromDate = new DateTime(pluginEvent.From.Year, pluginEvent.From.Month, pluginEvent.From.Day).Date.Add(TimeSpan.Parse($"0:{mondayCleaningTimeString}:00"));
					pluginEvent.From = fromDate;

					if (fromDate > pluginEvent.To)
					{
						pluginEvent.To = pluginEvent.From;
					}
				}
			}
		}

		private void _UpdatePluginEventSpecificTimes(CleaningProviderPlugin plugin, DateTime cleaningDate, CleaningEvent pluginEvent, int eventIndex)
		{
			var specificTimes = (IEnumerable<string>)new string[0];
			if (plugin.TypeKey == "DAILY")
			{
				this._UpdateEventTime(true, plugin.DailyCleaningTimeTypeKey, plugin.DailyCleaningTypeTimes, pluginEvent, eventIndex);
			}
			else if (plugin.TypeKey == "WEEKLY")
			{
				var dayOfWeek = cleaningDate.DayOfWeek;
				switch (dayOfWeek)
				{
					case DayOfWeek.Monday:
						this._UpdateEventTime(plugin.WeeklyCleanOnMonday, plugin.WeeklyTimeMondayTypeKey, plugin.WeeklyCleaningTypeMondayTimes, pluginEvent, eventIndex);
						break;
					case DayOfWeek.Tuesday:
						this._UpdateEventTime(plugin.WeeklyCleanOnTuesday, plugin.WeeklyTimeTuesdayTypeKey, plugin.WeeklyCleaningTypeTuesdayTimes, pluginEvent, eventIndex);
						break;
					case DayOfWeek.Wednesday:
						this._UpdateEventTime(plugin.WeeklyCleanOnWednesday, plugin.WeeklyTimeWednesdayTypeKey, plugin.WeeklyCleaningTypeWednesdayTimes, pluginEvent, eventIndex);
						break;
					case DayOfWeek.Thursday:
						this._UpdateEventTime(plugin.WeeklyCleanOnThursday, plugin.WeeklyTimeThursdayTypeKey, plugin.WeeklyCleaningTypeThursdayTimes, pluginEvent, eventIndex);
						break;
					case DayOfWeek.Friday:
						this._UpdateEventTime(plugin.WeeklyCleanOnFriday, plugin.WeeklyTimeFridayTypeKey, plugin.WeeklyCleaningTypeFridayTimes, pluginEvent, eventIndex);
						break;
					case DayOfWeek.Saturday:
						this._UpdateEventTime(plugin.WeeklyCleanOnSaturday, plugin.WeeklyTimeSaturdayTypeKey, plugin.WeeklyCleaningTypeSaturdayTimes, pluginEvent, eventIndex);
						break;
					case DayOfWeek.Sunday:
						this._UpdateEventTime(plugin.WeeklyCleanOnSunday, plugin.WeeklyTimeSundayTypeKey, plugin.WeeklyCleaningTypeSundayTimes, pluginEvent, eventIndex);
						break;
				}
			}

		}

		private IEnumerable<CleaningEvent> _CalculateBasedOnCleaningEvents(IEnumerable<CleaningPluginBasedOn> basedOns, DateTime cleaningDate, CleaningProviderRequest.Room room, IEnumerable<CleaningEvent> allEvents)
		{
			var events = new List<CleaningEvent>(allEvents);

			// ALL based ons must match for a plugin to apply.
			foreach (var basedOn in basedOns)
			{
				switch (basedOn.Type)
				{
					case CleaningPluginBaseOnType.CLEANLINESS:
						if(!this._ShouldCleanBasedOnCleanliness(room, basedOn.CleanlinessKey))
						{
							return new CleaningEvent[0];
						}
						break;

					case CleaningPluginBaseOnType.OTHER_PROPERTIES:
						if (!this._ShouldCleanBasedOnOtherProperties(room.Reservations, basedOn.OtherPropertiesExtended))
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.NIGHTS:
						if (!this._ShouldCleanBasedOnNights(cleaningDate, room.Reservations, basedOn.NightsTypeKey, basedOn.Nights, basedOn.NightsEveryNumberOfDays ?? 0, basedOn.NightsFromKey))
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.OCCUPATION:
						// Cleaning based on room occupation just filters out the "default" cleaning events
						events = this._GetCleaningEventsBasedOnOccupancy(basedOn, events, cleaningDate, room.PreviousCleaningDate.HasValue ? room.PreviousCleaningDate.Value : DateTime.MinValue);
						if(events.Count() == 0)
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.PRODUCT_TAG:
						if(!this._ShouldCleanBasedOnProductOrTag(room.Reservations, basedOn.ProductsTagsExtended, basedOn.ProductsTagsMustBeConsumedOnTime ?? false, basedOn.ProductsTagsConsumationIntervalFrom ?? DateTime.MinValue, basedOn.ProductsTagsConsumationIntervalTo ?? DateTime.MinValue, cleaningDate))
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.RESERVATION_SPACE_CATEGORY:
						if(!this._ShouldCleanBasedOnReservationSpaceCategory(room.Reservations, basedOn.ReservationSpaceCategories))
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.ROOM:
						var cleanBasedOnRoom = basedOn.Rooms.FirstOrDefault(r => r.RoomId == room.RoomId);
						if (cleanBasedOnRoom == null) 
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.ROOM_CATEGORY:
						var cleanBasedOnCategory = basedOn.RoomCategories.FirstOrDefault(rc => room.Category != null && rc.CategoryId == room.Category.Id);
						if (cleanBasedOnCategory == null) 
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.FLOOR:
						if(!this._ShouldCleanBasedOnFloor(room, basedOn.FoorIds))
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.SECTION:
						if(!this._ShouldCleanBasedOnSection(room, basedOn.Sections))
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.SUB_SECTION:
						if(!this._ShouldCleanBasedOnSubSection(room, basedOn.SubSections))
						{
							return new CleaningEvent[0];
						}
						break;
					case CleaningPluginBaseOnType.ALL:
						break;
					default:
						break;
				}
			}

			// Any plugin based on specific times should have a check here to set those specific times of cleaning.
			// All BasedOns conditions are met if the algorithm came to this point.
			// Otherwise it would return earlier in the switch statement above.
			return events;
		}

		private List<CleaningEvent> _GetCleaningEventsBasedOnOccupancy(CleaningPluginBasedOn basedOn, IEnumerable<CleaningEvent> cleaningEvents, DateTime currentDate, DateTime lastRoomCleaningDate)
		{
			var events = new List<CleaningEvent>();

			if (basedOn.CleanOutOfService)
			{
				events.AddRange(cleaningEvents.Where(e => e.Type == CleaningProviderRequest.CleaningType.OutOfService));
			}

			if (basedOn.CleanVacant)
			{
				var numberOfNightsFromLastCleaning = this._CalculateNumberOfNights(lastRoomCleaningDate, currentDate);
				var isCleaningDay = (numberOfNightsFromLastCleaning > 0) && ((numberOfNightsFromLastCleaning % basedOn.CleanVacantEveryNumberOfDays) == 0);
				if (isCleaningDay)
				{
					events.AddRange(cleaningEvents.Where(e => e.Type == CleaningProviderRequest.CleaningType.Vacant));
				}
			}

			if (basedOn.CleanDeparture)
			{
				events.AddRange(cleaningEvents.Where(e => e.Type == CleaningProviderRequest.CleaningType.Departure));

			}

			if (basedOn.CleanStay)
			{
				events.AddRange(cleaningEvents.Where(e => e.Type == CleaningProviderRequest.CleaningType.Stay));
			}

			return events;
		}

		private bool _ShouldCleanBasedOnRoom(Dictionary<Guid, bool> basedOnRoomMap, CleaningProviderRequest.Room room)
		{
			return basedOnRoomMap.ContainsKey(room.RoomId);
		}

		private CleaningPluginRoomCredits _GetCleanBasedOnRoom(IEnumerable<CleaningPluginRoomCredits> basedOnRooms, CleaningProviderRequest.Room room)
		{
			return basedOnRooms.FirstOrDefault(r => r.RoomId == room.RoomId);

			//if (basedOnRooms.Any(r => r.RoomId == room.Id))
			//{
			//	return basedOnRoomCategoryMap[room.Category.Id];
			//}
			//else
			//{
			//	return null;
			//}
		}
		
		private CleaningPluginBasedOnRoomCategory _GetCleanBasedOnRoomCategory(Dictionary<Guid, CleaningPluginBasedOnRoomCategory> basedOnRoomCategoryMap, CleaningProviderRequest.Room room)
		{
			if (basedOnRoomCategoryMap.ContainsKey(room.Category.Id))
			{
				return basedOnRoomCategoryMap[room.Category.Id];
			}
			else
			{
				return null;
			}
		}

		private bool _CheckWeeklyPeriod(CleaningProviderRequest.Reservation oldestReservation, int numberOfNights, int everyNumberOfDays, DateTime currentDate, DayOfWeek dayOfWeek)
		{
			if (everyNumberOfDays == 0)
				return false;

			if (oldestReservation.CheckIn.DayOfWeek == dayOfWeek)
			{
				return (numberOfNights > 0) && ((numberOfNights % everyNumberOfDays) == 0);
			}
			else
			{
				var firstMondayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, dayOfWeek);
				var nightsFromFirstMonday = this._CalculateNumberOfNights(firstMondayDate, currentDate);
				return (nightsFromFirstMonday > 0) && ((nightsFromFirstMonday) % everyNumberOfDays) == 0;
			}
		}

		private bool _ShouldCleanBasedOnNights(DateTime cleaningDate, CleaningProviderRequest.Reservation[] reservations, string nightsTypeKey, IEnumerable<int> nights, int nightsEveryNumberOfDays, string nightsFromKey)
		{
			if (reservations.Length == 0)
				return false;

			if(nightsTypeKey == "SPECIFIC_NIGHTS")
			{
				var oldestReservation = reservations.OrderBy(r => r.CheckIn).First();
				var oldestReservationDate = oldestReservation.CheckIn.Date;
				var daysAfterCheckIn = _GetNumberOfDaysBetweenDates(cleaningDate, oldestReservationDate); // (int)Math.Round((cleaningDate - oldestReservationDate).TotalDays, 0, MidpointRounding.AwayFromZero);

				if (daysAfterCheckIn < 0)
				{
					return false;
				}

				return nights.Contains(daysAfterCheckIn);
			}
			else if (nightsTypeKey == "PERIODICAL")
			{

				// TODO: REVIEW THE CHOICE OF RESERVATIONS FOR THE BALANCING CALCULATION!!!
				var oldestReservation = reservations.OrderBy(r => r.CheckIn).FirstOrDefault();
				if (oldestReservation == null)
					return false;

				//var currentDate = DateTime.UtcNow.Date;
				var numberOfNights = this._CalculateNumberOfNights(oldestReservation.CheckIn, cleaningDate);

				switch (nightsFromKey)
				{
					case "CHECK_IN":
						return (numberOfNights > 0) && ((numberOfNights % nightsEveryNumberOfDays) == 0);
					case "FIRST_MONDAY":
						return this._CheckWeeklyPeriod(oldestReservation, numberOfNights, nightsEveryNumberOfDays, cleaningDate, DayOfWeek.Monday);
						//if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Monday)
						//{
						//	return (numberOfNights > 0) && ((numberOfNights % nightsEveryNumberOfDays) == 0);
						//}
						//else
						//{
						//	var firstMondayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Monday);
						//	var nightsFromFirstMonday = this._CalculateNumberOfNights(firstMondayDate, currentDate);
						//	return (nightsFromFirstMonday > 0) && ((nightsFromFirstMonday) % nightsEveryNumberOfDays) == 0;
						//}
					case "FIRST_TUESDAY":
						return this._CheckWeeklyPeriod(oldestReservation, numberOfNights, nightsEveryNumberOfDays, cleaningDate, DayOfWeek.Tuesday);
						//if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Tuesday)
						//{
						//	return (numberOfNights > 0) && ((numberOfNights % nightsEveryNumberOfDays) == 0);
						//}
						//else
						//{
						//	var firstTuesdayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Tuesday);
						//	var nightsFromFirstTuesday = this._CalculateNumberOfNights(firstTuesdayDate, currentDate);
						//	return (nightsFromFirstTuesday > 0) && ((nightsFromFirstTuesday) % nightsEveryNumberOfDays) == 0;
						//}
					case "FIRST_WEDNESDAY":
						return this._CheckWeeklyPeriod(oldestReservation, numberOfNights, nightsEveryNumberOfDays, cleaningDate, DayOfWeek.Wednesday);
						//if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Wednesday)
						//{
						//	return (numberOfNights > 0) && ((numberOfNights % nightsEveryNumberOfDays) == 0);
						//}
						//else
						//{
						//	var firstWednesdayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Wednesday);
						//	var nightsFromFirstWednesday = this._CalculateNumberOfNights(firstWednesdayDate, currentDate);
						//	return (nightsFromFirstWednesday > 0) && ((nightsFromFirstWednesday) % nightsEveryNumberOfDays) == 0;
						//}
					case "FIRST_THURSDAY":

						return this._CheckWeeklyPeriod(oldestReservation, numberOfNights, nightsEveryNumberOfDays, cleaningDate, DayOfWeek.Thursday);
						//if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Thursday)
						//{
						//	return (numberOfNights > 0) && ((numberOfNights % nightsEveryNumberOfDays) == 0);
						//}
						//else
						//{
						//	var firstThursdayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Thursday);
						//	var nightsFromFirstThursday = this._CalculateNumberOfNights(firstThursdayDate, currentDate);
						//	return (nightsFromFirstThursday > 0) && ((nightsFromFirstThursday) % nightsEveryNumberOfDays) == 0;
						//}
					case "FIRST_FRIDAY":

						return this._CheckWeeklyPeriod(oldestReservation, numberOfNights, nightsEveryNumberOfDays, cleaningDate, DayOfWeek.Friday);
						//if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Friday)
						//{
						//	return (numberOfNights > 0) && ((numberOfNights % nightsEveryNumberOfDays) == 0);
						//}
						//else
						//{
						//	var firstFridayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Friday);
						//	var nightsFromFirstFriday = this._CalculateNumberOfNights(firstFridayDate, currentDate);
						//	return (nightsFromFirstFriday > 0) && ((nightsFromFirstFriday) % nightsEveryNumberOfDays) == 0;
						//}
					case "FIRST_SATURDAY":

						return this._CheckWeeklyPeriod(oldestReservation, numberOfNights, nightsEveryNumberOfDays, cleaningDate, DayOfWeek.Saturday);
						//if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Saturday)
						//{
						//	return (numberOfNights > 0) && ((numberOfNights % nightsEveryNumberOfDays) == 0);
						//}
						//else
						//{
						//	var firstSaturdayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Saturday);
						//	var nightsFromFirstSaturday = this._CalculateNumberOfNights(firstSaturdayDate, currentDate);
						//	return (nightsFromFirstSaturday > 0) && ((nightsFromFirstSaturday) % nightsEveryNumberOfDays) == 0;
						//}
					case "FIRST_SUNDAY":
						return this._CheckWeeklyPeriod(oldestReservation, numberOfNights, nightsEveryNumberOfDays, cleaningDate, DayOfWeek.Sunday);
						//if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Sunday)
						//{
						//	return (numberOfNights > 0) && ((numberOfNights % nightsEveryNumberOfDays) == 0);
						//}
						//else
						//{
						//	var firstSundayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Sunday);
						//	var nightsFromFirstSunday = this._CalculateNumberOfNights(firstSundayDate, currentDate);
						//	return (nightsFromFirstSunday > 0) && ((nightsFromFirstSunday) % nightsEveryNumberOfDays) == 0;
						//}
				}
			}

			return false;

		}

		private int _GetNumberOfDaysBetweenDates(DateTime newerDate, DateTime olderDate)
		{
			return (int)Math.Round((newerDate - olderDate).TotalDays, 0, MidpointRounding.AwayFromZero);
		}

		private bool _ShouldCleanBasedOnReservationSpaceCategory(CleaningProviderRequest.Reservation[] reservations, IEnumerable<string> reservationSpaceCategories)
		{
			// if any reservation has an OtherProperty with a key "ReservationSpaceCategory" and if under that key is a value from the list "reservationSpaceCategories"
			return reservations.Any(r => r.OtherProperties.ContainsKey("ReservationSpaceCategory") && reservationSpaceCategories.Contains(r.OtherProperties["ReservationSpaceCategory"]));
		}

		private bool _ShouldCleanBasedOnOtherProperties(CleaningProviderRequest.Reservation[] reservations, IEnumerable<CleaningPluginBasedOnOtherProperties> otherProperties)
		{
			if (!otherProperties.Any())
				return false;

			/// A reservation must match ALL conditions in order to return true.

			var reservationFound = false;
			foreach(var reservation in reservations)
			{
				if(reservation.OtherProperties == null || reservation.OtherProperties.Count == 0)
				{
					continue;
				}

				var reservationMatchesAllProperties = true;

				foreach(var property in otherProperties)
				{
					if (!reservation.OtherProperties.ContainsKey(property.Key))
					{
						reservationMatchesAllProperties = false;
						break;
					}
					
					if (string.IsNullOrWhiteSpace(property.Value))
					{
						reservationMatchesAllProperties = false;
						break;
					}

					var foundProperty = reservation.OtherProperties[property.Key];

					if (string.IsNullOrWhiteSpace(foundProperty))
					{
						reservationMatchesAllProperties = false;
						break;
					}

					switch (property.BasedOnOtherPropertiesTypeKey)
					{
						case "EQUALS":
							if(property.Value.ToLower() != foundProperty.ToLower())
							{
								reservationMatchesAllProperties = false;
							}
							break;
						case "CONTAINS":
							if (!foundProperty.ToLower().Contains(property.Value.ToLower()) )
							{
								reservationMatchesAllProperties = false;
							}
							break;
						case "BEGINS_WITH":
							if (!foundProperty.ToLower().StartsWith(property.Value.ToLower()))
							{
								reservationMatchesAllProperties = false;
							}
							break;
						case "ENDS_WITH":
							if (!foundProperty.ToLower().EndsWith(property.Value.ToLower()))
							{
								reservationMatchesAllProperties = false;
							}
							break;
						case "REGEX":
							try
							{
								var regex = new System.Text.RegularExpressions.Regex(property.Value, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
								var regexMatchResult = regex.Match(foundProperty);
								if (!regexMatchResult.Success)
								{
									reservationMatchesAllProperties = false;
								}
							}
							catch(Exception e)
							{
								reservationMatchesAllProperties = false;
							}
							break;
						default:
							reservationMatchesAllProperties = false;
							break;
					}
				}

				if (reservationMatchesAllProperties)
				{
					reservationFound = true;
					break;
				}
			}

			return reservationFound;
		}

		private bool _ShouldCleanBasedOnProductOrTag(CleaningProviderRequest.Reservation[] reservations, IEnumerable<CleaningPluginBasedOnProductsTags> productOrTags, bool mustBeConsumedInGivenInterval, DateTime from, DateTime to, DateTime currentDate)
		{
			if (!productOrTags.Any())
			{
				return false;
			}

			var reservationFound = false;
			foreach(var reservation in reservations)
			{
				if(reservation.OtherProperties == null || !reservation.OtherProperties.Any())
				{
					continue;
				}

				var productAndTagProperties = reservation.OtherProperties.Where(op => op.Key.ToLower() == "product" || op.Key.ToLower() == "tag").ToArray();
				if (!productAndTagProperties.Any())
				{
					continue;
				}

				var productsMatch = true;

				foreach(var reservationProduct in productAndTagProperties)
				{
					if (string.IsNullOrWhiteSpace(reservationProduct.Value))
					{
						productsMatch = false;
						break;
					}

					foreach(var product in productOrTags)
					{
						switch (product.BasedOnProductsTagsTypeKey)
						{
							case "EQUALS":
								if (string.IsNullOrWhiteSpace(product.ComparisonValue))
								{
									productsMatch = false;
								}
								else
								{
									if (product.IsCaseSensitive)
									{
										if (product.ComparisonValue != reservationProduct.Value)
										{
											productsMatch = false;
										}
									}
									else
									{
										if (product.ComparisonValue.ToLower() != reservationProduct.Value.ToLower())
										{
											productsMatch = false;
										}
									}
								}
								break;
							case "CONTAINS":
								if (string.IsNullOrWhiteSpace(product.ComparisonValue))
								{
									productsMatch = false;
								}
								else
								{
									if (product.IsCaseSensitive)
									{
										if (!reservationProduct.Value.Contains(product.ComparisonValue))
										{
											productsMatch = false;
										}
									}
									else
									{
										if (!reservationProduct.Value.ToLower().Contains(product.ComparisonValue.ToLower()))
										{
											productsMatch = false;
										}
									}
								}
								break;
							case "BEGINS_WITH":
								if (string.IsNullOrWhiteSpace(product.ComparisonValue))
								{
									productsMatch = false;
								}
								else
								{
									if (product.IsCaseSensitive)
									{
										if (!reservationProduct.Value.StartsWith(product.ComparisonValue))
										{
											productsMatch = false;
										}
									}
									else
									{
										if (!reservationProduct.Value.ToLower().StartsWith(product.ComparisonValue.ToLower()))
										{
											productsMatch = false;
										}
									}
								}
								break;
							case "ENDS_WITH":
								if (string.IsNullOrWhiteSpace(product.ComparisonValue))
								{
									productsMatch = false;
								}
								else
								{
									if (product.IsCaseSensitive)
									{
										if (!reservationProduct.Value.EndsWith(product.ComparisonValue))
										{
											productsMatch = false;
										}
									}
									else
									{
										if (!reservationProduct.Value.ToLower().EndsWith(product.ComparisonValue.ToLower()))
										{
											productsMatch = false;
										}
									}
								}
								break;
							case "REGEX":
								if (string.IsNullOrWhiteSpace(product.ComparisonValue))
								{
									productsMatch = false;
								}
								else
								{
									try
									{
										var regex = new System.Text.RegularExpressions.Regex(product.ComparisonValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
										var regexMatchResult = regex.Match(reservationProduct.Value);
										if (!regexMatchResult.Success)
										{
											productsMatch = false;
										}
									}
									catch (Exception e)
									{
										productsMatch = false;
									}
								}
								break;
							case "EXISTING_PRODUCT":
								if (string.IsNullOrWhiteSpace(product.ProductId))
								{
									productsMatch = false;
								}
								else
								{
									if (reservationProduct.Value.ToLower() != product.ProductId.ToLower())
									{
										productsMatch = false;
									}
								}
								break;
							default:
								productsMatch = false;
								break;
						}
					}
				}

				if (productsMatch)
				{
					reservationFound = true;
					break;
				}
			}

			if(reservationFound)
			{
				if (mustBeConsumedInGivenInterval)
				{
					return from <= currentDate && to >= currentDate;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		private bool _ShouldCleanBasedOnFloor(CleaningProviderRequest.Room room, IEnumerable<Guid> floorIds)
		{
			return floorIds.Contains(room.FloorId);
		}

		private bool _ShouldCleanBasedOnCleanliness(CleaningProviderRequest.Room room, string cleanlinessKey)
		{
			if(cleanlinessKey == "ONLY_CLEAN")
			{
				return room.IsClean;
			}
			else if(cleanlinessKey == "ONLY_DIRTY")
			{
				return !room.IsClean;
			}
			else
			{
				return false;
			}
		}

		private bool _ShouldCleanBasedOnSection(CleaningProviderRequest.Room room, IEnumerable<string> sections)
		{
			return sections.Contains(room.Section);
		}

		private bool _ShouldCleanBasedOnSubSection(CleaningProviderRequest.Room room, IEnumerable<string> subSections)
		{
			return subSections.Contains(room.SubSection);
		}

		private bool _ShouldStartCleaning(int startCleaningAfterDays, DateTime cleaningDate, CleaningProviderRequest.Reservation[] reservations)
		{
			if (startCleaningAfterDays <= 0)
				return true;

			var oldestReservation = reservations.OrderBy(r => r.CheckIn).First();
			var oldestReservationDate = oldestReservation.CheckIn.Date;
			var daysAfterCheckIn = _GetNumberOfDaysBetweenDates(cleaningDate, oldestReservationDate);

			return daysAfterCheckIn > startCleaningAfterDays;
		}

		private bool _isWeeklyPluginActiveOn(DateTime date, CleaningProviderPlugin plugin)
		{
			switch (date.DayOfWeek)
			{
				case DayOfWeek.Monday:
					return plugin.WeeklyCleanOnMonday.HasValue && plugin.WeeklyCleanOnMonday.Value;
				case DayOfWeek.Tuesday:
					return plugin.WeeklyCleanOnTuesday.HasValue && plugin.WeeklyCleanOnTuesday.Value;
				case DayOfWeek.Wednesday:
					return plugin.WeeklyCleanOnWednesday.HasValue && plugin.WeeklyCleanOnWednesday.Value;
				case DayOfWeek.Thursday:
					return plugin.WeeklyCleanOnThursday.HasValue && plugin.WeeklyCleanOnThursday.Value;
				case DayOfWeek.Friday:
					return plugin.WeeklyCleanOnFriday.HasValue && plugin.WeeklyCleanOnFriday.Value;
				case DayOfWeek.Saturday:
					return plugin.WeeklyCleanOnSaturday.HasValue && plugin.WeeklyCleanOnSaturday.Value;
				case DayOfWeek.Sunday:
					return plugin.WeeklyCleanOnSunday.HasValue && plugin.WeeklyCleanOnSunday.Value;
			}

			return false;
		}

		private bool _isMonthlyPluginActiveOn(DateTime date, CleaningProviderPlugin plugin)
		{
			var dateOfMonth = new DateTime(date.Year, date.Month, 1);

			switch (plugin.MonthlyCleaningTypeTimeOfMonthKey)
			{
				case "BEGINNING_OF_MONTH":
					// first workday after (including) 1st of month
					dateOfMonth = new DateTime(date.Year, date.Month, 1);
					break;
				case "MIDDLE_OF_MONTH":
					// first workday after (including) the 15th of month
					dateOfMonth = new DateTime(date.Year, date.Month, 15);
					break;
				case "END_OF_MONTH":
					// last workday before (including) the last day of month
					dateOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
					break;
			}

			if (dateOfMonth.DayOfWeek == DayOfWeek.Saturday)
			{
				dateOfMonth.AddDays(2);
			}
			else if (dateOfMonth.DayOfWeek == DayOfWeek.Sunday)
			{
				dateOfMonth.AddDays(1);
			}

			return date.Date == dateOfMonth.Date;
		}

		private bool _isPeriodicalPluginActiveOn(DateTime date, CleaningProviderPlugin plugin, CleaningProviderRequest.Reservation[] reservations)
		{
			if(plugin.PeriodicalIntervals == null || !plugin.PeriodicalIntervals.Any())
			{
				return false;
			}

			if(reservations.Length == 0)
			{
				// TODO: Ask Jonathan about this case.
				return false;
			}

			// TODO: REVIEW THE CHOICE OF RESERVATIONS FOR THE BALANCING CALCULATION!!!
			var oldestReservation = reservations.OrderByDescending(r => r.CheckIn).FirstOrDefault();
			if (oldestReservation == null)
				return false;

			var latestReservation = reservations.Where(r => !r.IsCheckedIn).OrderByDescending(r => r.CheckOut).FirstOrDefault() ?? oldestReservation;
			var reservationDurationNights = this._CalculateNumberOfNights(oldestReservation.CheckIn, latestReservation.CheckOut);
			var nightsFromCheckInUntilToday = this._CalculateNumberOfNights(oldestReservation.CheckIn, date);

			foreach(var interval in plugin.PeriodicalIntervals)
			{
				if(interval.IntervalTypeKey == "FROM")
				{
					if(interval.FromNights > reservationDurationNights || interval.ToNights < reservationDurationNights)
					{
						continue;
					}
				}
				else if(interval.IntervalTypeKey == "MORE_THAN")
				{
					if(interval.FromNights > reservationDurationNights)
					{
						continue;
					}
				}
				else
				{
					// UNKNOWN INTERVAL TYPE KEY!
					continue;
				}

		//		// at this point the interval applies to the day
				if(interval.PeriodTypeKey == "BALANCE_OVER_RESERVATION")
				{
					var checkInDate = oldestReservation.CheckIn.Date;
					var checkOutDate = latestReservation.CheckOut.Date;
					var balancedDates = CleaningBalancer.BalanceCleaningsOverDateInterval(checkInDate, checkOutDate, interval.NumberOfCleanings, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);
					return balancedDates.Contains(date);
				}
				else if (interval.PeriodTypeKey == "BALANCE_OVER_PERIOD")
				{
					var checkInDate = oldestReservation.CheckIn.Date;
					var checkOutDate = latestReservation.CheckOut.Date;
					var balancedDates = CleaningBalancer.BalanceCleaningsOverDateInterval(checkInDate, checkOutDate, interval.NumberOfCleanings, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);
					return balancedDates.Contains(date);

					//var currentDate = DateTime.UtcNow.Date;
					//switch (interval.FromDayKey)
					//{
					//	case "CHECK_IN":
					//		return this._shouldBalanceClean(reservationDurationNights, interval.EveryNumberOfDays, interval.NumberOfCleanings, oldestReservation, date, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);

					//	case "FIRST_MONDAY":
					//		var difference2 = reservationDurationNights;
					//		if (oldestReservation.CheckIn.DayOfWeek != DayOfWeek.Monday)
					//		{
					//			var firstDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Monday);
					//			difference2 = this._CalculateNumberOfNights(firstDate, latestReservation.CheckOut);
					//		}
					//		return this._shouldBalanceClean(difference2, interval.EveryNumberOfDays, interval.NumberOfCleanings, oldestReservation, date, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);

					//	case "FIRST_TUESDAY":
					//		var difference3 = reservationDurationNights;
					//		if (oldestReservation.CheckIn.DayOfWeek != DayOfWeek.Tuesday)
					//		{
					//			var firstDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Tuesday);
					//			difference3 = this._CalculateNumberOfNights(firstDate, latestReservation.CheckOut);
					//		}
					//		return this._shouldBalanceClean(difference3, interval.EveryNumberOfDays, interval.NumberOfCleanings, oldestReservation, date, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);

					//	case "FIRST_WEDNESDAY":
					//		var difference4 = reservationDurationNights;
					//		if (oldestReservation.CheckIn.DayOfWeek != DayOfWeek.Wednesday)
					//		{
					//			var firstDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Wednesday);
					//			difference4 = this._CalculateNumberOfNights(firstDate, latestReservation.CheckOut);
					//		}
					//		return this._shouldBalanceClean(difference4, interval.EveryNumberOfDays, interval.NumberOfCleanings, oldestReservation, date, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);

					//	case "FIRST_THURSDAY":
					//		var difference5 = reservationDurationNights;
					//		if (oldestReservation.CheckIn.DayOfWeek != DayOfWeek.Thursday)
					//		{
					//			var firstDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Thursday);
					//			difference5 = this._CalculateNumberOfNights(firstDate, latestReservation.CheckOut);
					//		}
					//		return this._shouldBalanceClean(difference5, interval.EveryNumberOfDays, interval.NumberOfCleanings, oldestReservation, date, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);

					//	case "FIRST_FRIDAY":
					//		var difference6 = reservationDurationNights;
					//		if (oldestReservation.CheckIn.DayOfWeek != DayOfWeek.Friday)
					//		{
					//			var firstDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Friday);
					//			difference6 = this._CalculateNumberOfNights(firstDate, latestReservation.CheckOut);
					//		}
					//		return this._shouldBalanceClean(difference6, interval.EveryNumberOfDays, interval.NumberOfCleanings, oldestReservation, date, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);

					//	case "FIRST_SATURDAY":
					//		var difference7 = reservationDurationNights;
					//		if (oldestReservation.CheckIn.DayOfWeek != DayOfWeek.Saturday)
					//		{
					//			var firstDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Saturday);
					//			difference7 = this._CalculateNumberOfNights(firstDate, latestReservation.CheckOut);
					//		}
					//		return this._shouldBalanceClean(difference7, interval.EveryNumberOfDays, interval.NumberOfCleanings, oldestReservation, date, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);

					//	case "FIRST_SUNDAY":
					//		var difference8 = reservationDurationNights;
					//		if (oldestReservation.CheckIn.DayOfWeek != DayOfWeek.Sunday)
					//		{
					//			var firstDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Sunday);
					//			difference8 = this._CalculateNumberOfNights(firstDate, latestReservation.CheckOut);
					//		}
					//		return this._shouldBalanceClean(difference8, interval.EveryNumberOfDays, interval.NumberOfCleanings, oldestReservation, date, plugin.PeriodicalPostponeSundayCleaningsToMonday ?? false);

					//}
				}
				else if (interval.PeriodTypeKey == "ONCE_EVERY_N_DAYS")
				{
					switch (interval.FromDayKey)
					{
						case "CHECK_IN":
							return nightsFromCheckInUntilToday > 0 && (nightsFromCheckInUntilToday % interval.EveryNumberOfDays) == 0;
						case "FIRST_MONDAY":
							if(oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Monday)
							{
								return nightsFromCheckInUntilToday > 0 && (nightsFromCheckInUntilToday % interval.EveryNumberOfDays) == 0;
							}
							else
							{
								var firstMondayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Monday);
								var nightsFromFirstMonday = this._CalculateNumberOfNights(firstMondayDate, date);
								return nightsFromFirstMonday > 0 && (nightsFromFirstMonday % interval.EveryNumberOfDays) == 0;
							}
						case "FIRST_TUESDAY":
							if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Tuesday)
							{
								return nightsFromCheckInUntilToday > 0 && (nightsFromCheckInUntilToday % interval.EveryNumberOfDays) == 0;
							}
							else
							{
								var firstTuesdayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Tuesday);
								var nightsFromFirstTuesday = this._CalculateNumberOfNights(firstTuesdayDate, date);
								return nightsFromFirstTuesday > 0 && (nightsFromFirstTuesday % interval.EveryNumberOfDays) == 0;
							}
						case "FIRST_WEDNESDAY":
							if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Wednesday)
							{
								return nightsFromCheckInUntilToday > 0 && (nightsFromCheckInUntilToday % interval.EveryNumberOfDays) == 0;
							}
							else
							{
								var firstWednesdayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Wednesday);
								var nightsFromFirstWednesday = this._CalculateNumberOfNights(firstWednesdayDate, date);
								return nightsFromFirstWednesday > 0 && (nightsFromFirstWednesday % interval.EveryNumberOfDays) == 0;
							}
						case "FIRST_THURSDAY":

							if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Thursday)
							{
								return nightsFromCheckInUntilToday > 0 && (nightsFromCheckInUntilToday % interval.EveryNumberOfDays) == 0;
							}
							else
							{
								var firstThursdayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Thursday);
								var nightsFromFirstThursday = this._CalculateNumberOfNights(firstThursdayDate, date);
								return nightsFromFirstThursday > 0 && (nightsFromFirstThursday % interval.EveryNumberOfDays) == 0;
							}
						case "FIRST_FRIDAY":

							if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Friday)
							{
								return nightsFromCheckInUntilToday > 0 && (nightsFromCheckInUntilToday % interval.EveryNumberOfDays) == 0;
							}
							else
							{
								var firstFridayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Friday);
								var nightsFromFirstFriday = this._CalculateNumberOfNights(firstFridayDate, date);
								return nightsFromFirstFriday > 0 && (nightsFromFirstFriday % interval.EveryNumberOfDays) == 0;
							}
						case "FIRST_SATURDAY":

							if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Saturday)
							{
								return nightsFromCheckInUntilToday > 0 && (nightsFromCheckInUntilToday % interval.EveryNumberOfDays) == 0;
							}
							else
							{
								var firstSaturdayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Saturday);
								var nightsFromFirstSaturday = this._CalculateNumberOfNights(firstSaturdayDate, date);
								return nightsFromFirstSaturday > 0 && (nightsFromFirstSaturday % interval.EveryNumberOfDays) == 0;
							}
						case "FIRST_SUNDAY":
							if (oldestReservation.CheckIn.DayOfWeek == DayOfWeek.Sunday)
							{
								return nightsFromCheckInUntilToday > 0 && (nightsFromCheckInUntilToday % interval.EveryNumberOfDays) == 0;
							}
							else
							{
								var firstSundayDate = this._GetNextWeekday(oldestReservation.CheckIn.Date, DayOfWeek.Sunday);
								var nightsFromFirstSunday = this._CalculateNumberOfNights(firstSundayDate, date);
								return nightsFromFirstSunday > 0 && (nightsFromFirstSunday % interval.EveryNumberOfDays) == 0;
							}
					}
				}
			}

			return false;
		}

		private bool _shouldBalanceClean(int nights, int everyNumberOfDays, int numberOfCleanings, CleaningProviderRequest.Reservation oldestReservation, DateTime cleaningDate, bool periodicalPostponeSundayCleaningsToMonday)
		{
			var numberOfPeriods = nights / everyNumberOfDays;
			var daysToAdd = numberOfPeriods * everyNumberOfDays;
			var intervalStart = oldestReservation.CheckIn.AddDays(daysToAdd);
			var intervalEnd = intervalStart.AddDays(everyNumberOfDays);
			var balancedDates = CleaningBalancer.BalanceCleaningsOverDateInterval(intervalStart, intervalEnd, numberOfCleanings, periodicalPostponeSundayCleaningsToMonday);
			return balancedDates.Contains(cleaningDate);
		}

		//private bool _shouldBalanceCleanOverPeriod(DateTime fromDate, DateTime toDate, DateTime cleaningDate, int nightsFromCheckInUntilToday, int reservationDurationNights, int nights, int intervalDurationNights, int everyNumberOfDays, int numberOfCleanings, CleaningProviderRequest.Reservation oldestReservation, DateTime cleaningDate, bool periodicalPostponeSundayCleaningsToMonday)
		//{
		//	var intervalIndex = nightsFromCheckInUntilToday / intervalDurationNights;
		//	var daysToShiftForward = intervalIndex * intervalDurationNights;



		//	var numberOfPeriods = nights / everyNumberOfDays;
		//	var daysToAdd = numberOfPeriods * everyNumberOfDays;
		//	var intervalStart = oldestReservation.CheckIn.AddDays(daysToAdd);
		//	var intervalEnd = intervalStart.AddDays(everyNumberOfDays);
		//	var balancedDates = CleaningBalancer.BalanceCleaningsOverDateInterval(intervalStart, intervalEnd, numberOfCleanings, periodicalPostponeSundayCleaningsToMonday);
		//	return balancedDates.Contains(cleaningDate);
		//}

		private int _CalculateNumberOfNights(DateTime checkIn, DateTime currentTime)
		{
			var checkInDate = checkIn.Date;
			var currentDate = currentTime.Date;

			return (currentDate - checkInDate).Days;
		}

		//private bool _isBalancedPluginActiveOn(DateTime date, CleaningProviderPlugin plugin)
		//{
		//	return false;
		//}
		//private bool _isBalancedWeeklyPluginActiveOn(DateTime date, CleaningProviderPlugin plugin)
		//{
		//	return false;
		//}
		private bool _isWeekBasedPluginActiveOn(DateTime date, CleaningProviderPlugin plugin)
		{
			var week = GetIso8601WeekOfYear(date);
			var dayOfWeek = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
			var dayOfWeekKey = "N/A";

			switch (dayOfWeek)
			{
				case DayOfWeek.Monday: dayOfWeekKey = "MONDAY"; break;
				case DayOfWeek.Tuesday: dayOfWeekKey = "TUESDAY"; break;
				case DayOfWeek.Wednesday: dayOfWeekKey = "WEDNESDAY"; break;
				case DayOfWeek.Thursday: dayOfWeekKey = "THURSDAY"; break;
				case DayOfWeek.Friday: dayOfWeekKey = "FRIDAY"; break;
				case DayOfWeek.Saturday: dayOfWeekKey = "SATURDAY"; break;
				case DayOfWeek.Sunday: dayOfWeekKey = "SUNDAY"; break;
			}

			return plugin.WeekBasedCleaningTypeWeeks.Contains(week) && dayOfWeekKey == plugin.WeekBasedCleaningDayOfTheWeekKey;
		}

		// This presumes that weeks start with Monday.
		// Week 1 is the 1st week of the year with a Thursday in it.
		private static int GetIso8601WeekOfYear(DateTime time)
		{
			// Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
			// be the same week# as whatever Thursday, Friday or Saturday are,
			// and we always get those right
			DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				time = time.AddDays(3);
			}

			// Return the week of our adjusted day
			return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}

		private static DateTime GetDateOfWeekISO8601(int year, int weekOfYear, string dayOfWeekKey)
		{
			var firstDayDate = GetFirstDateOfWeekISO8601(year, weekOfYear);
			var offsetDays = 0;
			switch (dayOfWeekKey)
			{
				case "MONDAY":
					offsetDays = 0;
					break;
				case "TUESDAY":
					offsetDays = 1;
					break;
				case "WEDNESDAY":
					offsetDays = 2;
					break;
				case "THURSDAY":
					offsetDays = 3;
					break;
				case "FRIDAY":
					offsetDays = 4;
					break;
				case "SATURDAY":
					offsetDays = 5;
					break;
				case "SUNDAY":
					offsetDays = 6;
					break;
				default:
					throw new NotSupportedException("Unknown dayOfWeekKey CleaningProvider.GetDateOfWeekISO8601");
			}

			return firstDayDate.AddDays(offsetDays);
		}

		private static DateTime GetFirstDateOfWeekISO8601(int year, int weekOfYear)
		{
			DateTime jan1 = new DateTime(year, 1, 1);
			int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

			// Use first Thursday in January to get first week of the year as
			// it will never be in Week 52/53
			DateTime firstThursday = jan1.AddDays(daysOffset);
			var cal = CultureInfo.CurrentCulture.Calendar;
			int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

			var weekNum = weekOfYear;
			// As we're adding days to a date in Week 1,
			// we need to subtract 1 in order to get the right date for week #1
			if (firstWeek == 1)
			{
				weekNum -= 1;
			}

			// Using the first Thursday as starting week ensures that we are starting in the right year
			// then we add number of weeks multiplied with days
			var result = firstThursday.AddDays(weekNum * 7);

			// Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
			return result.AddDays(-3);
		}

		private DateTime _GetNextWeekday(DateTime start, DayOfWeek day)
		{
			// The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
			int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
			return start.AddDays(daysToAdd);
		}

		private enum RoomEventType
		{
			CHECK_IN,
			CHECK_OUT,
			NEW_DAY
		}
		private class RoomEvent
		{
			public RoomEventType Type { get; set; }
			public DateTime At { get; set; }
			public string ReservationId { get; set; }
		}
		private class CleaningEvent
		{
			public DateTime From { get; set; }
			public DateTime To { get; set; }
			//public int Credits { get; set; }
			public CleaningProviderRequest.CleaningType Type { get; set; }
			public int Id { get; set; }
		}

		/// <summary>
		/// Sometimes when the PMS enables late CHECK OUTs the system will not move the CHECK IN to be after the checkout but will stay at the default check in time.
		/// In that case the cleaning generated will be a STAY cleaning instead of DEPARTURE cleaning.
		/// When you want to fix that case on the Hopr side you must set the fixOverlappingCheckinsWithLateCheckouts to true.
		/// WARNING!! This only applies when there is 1 check in and 1 check out in the day.
		/// </summary>
		/// <param name="roomEvents"></param>
		/// <param name="fixOverlappingCheckinsWithLateCheckouts"></param>
		/// <returns></returns>
		private IEnumerable<RoomEvent> _GetOrderedRoomEvents(IEnumerable<RoomEvent> roomEvents, bool fixOverlappingCheckinsWithLateCheckouts)
		{
			// Only fix the case when there are 2 room events out of which 1 is CHECK IN and 1 is CHECK OUT and the CHECK OUT is AFTER the CHECK IN FOR DIFFERENT RESERVATIONS!
			if (fixOverlappingCheckinsWithLateCheckouts && roomEvents.Count() == 2)
			{
				var checkInEvent = roomEvents.FirstOrDefault(re => re.Type == RoomEventType.CHECK_IN);
				var checkOutEvent = roomEvents.FirstOrDefault(re => re.Type == RoomEventType.CHECK_OUT);

				if(checkInEvent != null && checkOutEvent != null && checkInEvent.ReservationId != checkOutEvent.ReservationId && checkInEvent.At <= checkOutEvent.At)
				{
					// // Order them so the CHECK OUT is BEFORE the CHECK IN - no matter the real 
					// return new RoomEvent[] { checkOutEvent, checkInEvent };

					// Better solution for now is to move the CHECK IN event to be AFTER the CHECK OUT event.
					// This matters because the cleaning must be in some time frame and if we just reorder them the times of the cleanings will not be correct.

					var timeShiftMinutes = 30;
					checkInEvent.At = checkOutEvent.At.AddMinutes(timeShiftMinutes);

					// There is one edge case when the cleaning can possibly go into the next day.
					// In that case, both check in and check out times must be moved to fall into the same day (current day of the calculation)!
					if(checkInEvent.At.Date > checkInEvent.At.Date)
					{
						// Just move the times one hour earlier than they really are.
						// This is a SUPER EDGE CASE and will produce incorrect cleaning times but something must be done to fit both in the same day.
						checkOutEvent.At.AddMinutes(-1 * (timeShiftMinutes + 1));
						checkInEvent.At.AddMinutes(-1 * (timeShiftMinutes + 1));
					}
				}
			}
			
			return roomEvents.OrderBy(ae => ae.At).ToArray();
		}

		private ProcessResponse<IEnumerable<CleaningEvent>> _CreateReservationEvents(CleaningProviderRequest.Room room, DateTime cleaningDate)
		{
			var eventIdSeed = 1;
			var cleaningFrom = cleaningDate.Date;
			var cleaningTo = cleaningFrom.AddDays(1);
			var cleaningEvents = new List<CleaningEvent>();

			if (room.IsOutOfService)
			{
				cleaningEvents.Add(new CleaningEvent { Id = eventIdSeed, From = cleaningFrom, To = cleaningTo, Type = CleaningProviderRequest.CleaningType.OutOfService });

				return new ProcessResponse<IEnumerable<CleaningEvent>>
				{
					Data = cleaningEvents,
					HasError = false,
					IsSuccess = true,
					Message = "Cleaning events calculated."
				};
			}

			var numberOfStartingActiveReservations = 0;

			var activityEvents = new List<RoomEvent>();
			foreach (var reservation in room.Reservations)
			{
				var isCheckInToday = reservation.CheckIn >= cleaningFrom && reservation.CheckIn < cleaningTo;
				var isCheckOutToday = reservation.CheckOut >= cleaningFrom && reservation.CheckOut < cleaningTo;

				if (reservation.CheckIn < cleaningFrom)
				{
					numberOfStartingActiveReservations++;
				}

				if (isCheckInToday)
				{
					activityEvents.Add(new RoomEvent { Type = RoomEventType.CHECK_IN, At = reservation.CheckIn, ReservationId = reservation.Id });
				}
				if (isCheckOutToday)
				{
					activityEvents.Add(new RoomEvent { Type = RoomEventType.CHECK_OUT, At = reservation.CheckOut, ReservationId = reservation.Id });
				}
			}

			var events = new List<RoomEvent>();
			events.Add(new RoomEvent { At = cleaningFrom, Type = RoomEventType.NEW_DAY, ReservationId = "-" });
			//events.AddRange(activityEvents.OrderBy(ae => ae.At).ToArray());
			events.AddRange(this._GetOrderedRoomEvents(activityEvents, true));
			events.Add(new RoomEvent { At = cleaningTo, Type = RoomEventType.NEW_DAY, ReservationId = "-" });

			//-----------------------

			var numberOfEvents = activityEvents.Count;
			var numberOfReservations = numberOfStartingActiveReservations;
			var isRoomInitiallyDirty = numberOfStartingActiveReservations > 0;
			var hasCheckoutInconsistencyError = false;

			if (numberOfEvents == 0)
			{
				// There are no check in or check out events.

				if (numberOfReservations == 0)
				{
					// VACANT CLEANING
					cleaningEvents.Add(new CleaningEvent { Id = eventIdSeed, Type = CleaningProviderRequest.CleaningType.Vacant, From = cleaningFrom, To = cleaningTo });
					eventIdSeed++;
				}
				else
				{
					// STAY CLEANING
					cleaningEvents.Add(new CleaningEvent { Id = eventIdSeed, Type = CleaningProviderRequest.CleaningType.Stay, From = cleaningFrom, To = cleaningTo });
					eventIdSeed++;
				}
			}
			else
			{
				// There is at least one check in or check out event.

				for (int i = 1; i < events.Count - 1; i++)
				{
					var e = events[i];

					if (e.Type == RoomEventType.CHECK_IN)
					{
						numberOfReservations++;

						// This means that there is a check in to a vacant room. So there should be a vacant cleaning this day really before the check in. IF any.
						if(numberOfReservations == 1 && i == 1)
						{
							// VACANT CLEANING
							cleaningEvents.Add(new CleaningEvent { Id = eventIdSeed, Type = CleaningProviderRequest.CleaningType.Vacant, From = cleaningFrom, To = e.At });
							eventIdSeed++;
						}
					}
					else if (e.Type == RoomEventType.CHECK_OUT)
					{
						numberOfReservations--;
						if (numberOfReservations == 0)
						{
							var nextEvent = events[i + 1];
							// DEPARTURE CLEANING
							cleaningEvents.Add(new CleaningEvent { Id = eventIdSeed, Type = CleaningProviderRequest.CleaningType.Departure, From = e.At, To = nextEvent.At });
							eventIdSeed++;
						}
						else if (numberOfReservations < 0)
						{
							// ERROR!
							hasCheckoutInconsistencyError = true;
							break;
						}
					}
				}

				if (!hasCheckoutInconsistencyError)
				{
					// Cleanings.Count will be 0 at this point only if there are no check out events / departure cleanings.
					if (isRoomInitiallyDirty && cleaningEvents.Count == 0)
					{
						// REGULAR DIRTY CLEANING only if the room is initially occupied (dirty) and there are no departure cleanings
						cleaningEvents.Add(new CleaningEvent { Id = eventIdSeed, Type = CleaningProviderRequest.CleaningType.Stay, From = cleaningFrom, To = cleaningTo });
						eventIdSeed++;
					}
				}
			}

			if (hasCheckoutInconsistencyError)
			{
				return new ProcessResponse<IEnumerable<CleaningEvent>>
				{
					Data = new CleaningEvent[0],
					HasError = true,
					IsSuccess = false,
					Message = $"There are more checkouts than reservations for the room {room.Name} at date {cleaningDate}"
				};
			}
			else
			{
				return new ProcessResponse<IEnumerable<CleaningEvent>>
				{
					Data = cleaningEvents,
					HasError = false,
					IsSuccess = true,
					Message = "Cleaning events calculated."
				};
			}
		}






		private string _GenerateRoomLogDescription(CleaningProviderRequest.Room room)
		{
			var data = new
			{
				RoomName = room.Name,
				RoomId = room.RoomId,
				BedId = room.BedId,
				IsBed = room.IsBed,
				IsClean = room.IsClean,
				IsDoNotDisturb = room.IsDoNotDisturb,
				IsOutOfService = room.IsOutOfService,
				IsPriority = room.IsPriority,
			};

			return JsonSerializer.Serialize(data);
		}
		private string _GenerateReservationsLogDescription(CleaningProviderRequest.Room room)
		{
			if (room.Reservations == null || !room.Reservations.Any())
			{
				return "[]";
			}
			else
			{
				return JsonSerializer.Serialize(room.Reservations.Select(r => new
				{
					ReservationId = r.Id,
					CheckIn = r.CheckIn.ToString("yyyy.MM.dd. HH:mm"),
					CheckOut = r.CheckOut.ToString("yyyy.MM.dd. HH:mm"),
					r.ExternalId,
					r.GuestName,
					r.IsActive,
				}).ToArray());
			}
		}

		private string _GenerateReservationsEventsDescription(IEnumerable<CleaningEvent> events)
		{
			return JsonSerializer.Serialize(events.Select(e => new 
			{
				From = e.From.ToString("yyyy.MM.dd. HH:mm"),
				To = e.To.ToString("yyyy.MM.dd. HH:mm"),
				Type = e.Type.ToString(),
			}));
		}

		private string _GeneratePluginsEventsDescription(Dictionary<Guid, IEnumerable<CleaningEvent>>  pluginEventsMap, List<CleaningProviderPlugin>  activePlugins)
		{
			return JsonSerializer.Serialize(pluginEventsMap.Select(kvp => 
			{
				var plugin = activePlugins.FirstOrDefault(p => p.Id == kvp.Key);
				return new
				{
					CleaningEvents = this._GenerateReservationsEventsDescription(kvp.Value),
					PluginName = plugin?.Name ?? "N/A",
					PluginId = plugin?.Id.ToString() ?? "N/A",
				};
			}).ToArray());
		}

		private string _GenerateOrderedPluginsDescription(IEnumerable<CleaningProviderPlugin> plugins)
		{
			return JsonSerializer.Serialize(plugins.Select(p => new 
			{ 
				PluginId = p.Id,
				PluginName = p.Name,
				Order = p.OrdinalNumber,
				TopRule = p.IsTopRule,
			}).ToArray());
		}

		private string _GenerateCleaningEventsDescription(Dictionary<int, CleaningEvent> cleaningEventsMap, Dictionary<int, CleaningProviderPlugin> cleaningEventsPluginsMap)
		{
			var cleaningEvents = cleaningEventsMap.Select(kvp =>
			{
				var cleaningPlugin = cleaningEventsPluginsMap[kvp.Key];
				return new 
				{
					PluginId = cleaningPlugin.Id,
					PluginName = cleaningPlugin.Name,
					EventId = kvp.Value.Id,
					EventType = kvp.Value.Type.ToString(),
					From = kvp.Value.From.ToString("yyyy.MM.dd. HH:mm"),
					To = kvp.Value.To.ToString("yyyy.MM.dd. HH:mm"),
				};
			}).ToArray();

			return JsonSerializer.Serialize(cleaningEvents);
		}

		private string _GenerateCleaningsDescription(CleaningProviderRequest.Cleaning[] cleanings)
		{
			return JsonSerializer.Serialize(cleanings.Select(c => new
			{
				EventType = c.Type.ToString(),
				Plugin = c.PluginName,
				Credits = c.Credits,
				ChangeSheets = c.IsChangeSheets,
				Priority = c.IsPriority,
			}).ToArray());
		}
	}
}
