using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Infrastructure;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure
{
	public class CleaningGeneratorService : ICleaningGeneratorService
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICleaningProvider _cleaningProvider;
		public CleaningGeneratorService(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, ICleaningProvider cleaningProvider)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._cleaningProvider = cleaningProvider;
		}

		private Dictionary<Guid, RoomWithHotelStructureView> _roomsMap = new Dictionary<Guid, RoomWithHotelStructureView>();
		private IEnumerable<CleaningPlugin> _cleaningPlugins = new CleaningPlugin[0];
		private IEnumerable<ProcessResponse<CleaningProviderRequest.Cleaning[]>> _cleaningResults = new ProcessResponse<CleaningProviderRequest.Cleaning[]>[0];
		private Dictionary<Guid, List<CleaningTimelineItemTaskData>> _cleaningTasksMap = new Dictionary<Guid, List<CleaningTimelineItemTaskData>>();

		public async Task<IEnumerable<CleaningTimelineItemData>> GenerateCleanings(string hotelId, bool isToday, DateTime cleaningDate)
		{
			var planFromDate = cleaningDate.Date;
			var planToDate = planFromDate.AddDays(1);

			var hotel = await this._databaseContext
				.Hotels
				.Include(h => h.Settings)
				.FirstOrDefaultAsync(h => h.Id == hotelId);

			var defaultCheckOutTime = new TimeSpan(10, 0, 0);
			var defaultCheckInTime = new TimeSpan(14, 0, 0);

			if(hotel.Settings != null)
			{
				if(!TimeSpan.TryParse((hotel.Settings.DefaultCheckOutTime ?? "") + ":00", out defaultCheckOutTime))
				{
					defaultCheckOutTime = new TimeSpan(10, 0, 0);
				}
				if (!TimeSpan.TryParse((hotel.Settings.DefaultCheckInTime ?? "") + ":00", out defaultCheckInTime))
				{
					defaultCheckInTime = new TimeSpan(14, 0, 0);
				}
			}


			this._roomsMap = await this._LoadRoomsWithActiveReservations(hotelId, planFromDate);
			this._cleaningPlugins = await this._LoadCleaningPlugins(hotelId);
			this._cleaningTasksMap = await this._LoadCleaningTasks(hotelId, planFromDate, planToDate);

			var cleaningGeneratorResult = this._CalculateCleanings(hotelId, isToday, planFromDate, defaultCheckInTime, defaultCheckOutTime);
			this._cleaningResults = cleaningGeneratorResult.Results;

			await this._SaveCleaningGeneratorLogs(cleaningGeneratorResult.LogMessages, cleaningDate.Date, hotelId);

			var timeZoneId = Infrastructure.HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

			return this._GenerateTimelineCleanings(planFromDate, timeZoneId);
		}

		private async Task _SaveCleaningGeneratorLogs(List<CleaningGeneratorLogMessage> logMessages, DateTime cleaningPlanDate, string hotelId)
		{
			if (logMessages == null || !logMessages.Any())
				return;

			var logs = logMessages.Select(lm => new Domain.Entities.CleaningGeneratorLog 
			{ 
				CleaningPlanDate = cleaningPlanDate,
				At = lm.At,
				Id = Guid.NewGuid(),
				GenerationId = lm.GenerationId,
				Message = lm.Message,
				CleaningEventsDescription = lm.CleaningEventsDescription,
				CleaningsDescription = lm.CleaningsDescription,
				OrderedPluginsDescription = lm.OrderedPluginsDescription,
				PluginEventsDescription = lm.PluginEventsDescription,
				ReservationsDescription = lm.ReservationsDescription,
				ReservationsEventsDescription = lm.ReservationsEventsDescription,
				RoomDescription = lm.RoomDescription,
				HotelId = hotelId,
			}).ToArray();

			await this._databaseContext.CleaningGeneratorLogs.AddRangeAsync(logs);
			await this._databaseContext.SaveChangesAsync(System.Threading.CancellationToken.None);
		}

		private async Task<Dictionary<Guid, RoomWithHotelStructureView>> _LoadRoomsWithActiveReservations(string hotelId, DateTime cleaningDate)
		{
			var roomsMap = (await this._databaseContext.Rooms.GetRoomsWithStructureAndActiveReservationsQuery(hotelId, cleaningDate, null, null, null, null, null, false, false, true).ToArrayAsync())
					.ToDictionary(r => r.Id, r => r);

			var activeBedReservations = (await this._databaseContext.Reservations.GetActiveReservationsForBedsQuery(hotelId, cleaningDate).ToArrayAsync()).GroupBy(r => r.RoomId.Value).ToDictionary(r => r.Key, r => r.ToArray());

			foreach (var roomReservationsPair in activeBedReservations)
			{
				var roomId = roomReservationsPair.Key;
				if (!roomsMap.ContainsKey(roomId)) continue;

				var room = roomsMap[roomId];

				if (room.RoomBeds == null) continue;

				var bedReservations = roomReservationsPair.Value.GroupBy(r => r.RoomBedId.Value).ToDictionary(r => r.Key, r => r.ToArray());


				foreach (var bedReservationsPair in bedReservations)
				{
					var bedId = bedReservationsPair.Key;
					var bed = room.RoomBeds.FirstOrDefault(b => b.Id == bedId);

					if (bed == null) continue;

					bed.Reservations = bedReservationsPair.Value;
				}
			}

			/////////////////////////// REMOVE FROM HERE
			/////////////////////////// REMOVE FROM HERE
			/////////////////////////// REMOVE FROM HERE
			/////////////////////////// REMOVE FROM HERE
			//var firstRoomId = roomsMap.Keys.First();
			//var firstRoom = roomsMap.First().Value;
			//firstRoom.Reservations = new List<Reservation>()
			//{
			//	new Reservation
			//	{
			//		CheckIn = new DateTime(2022, 2, 13, 14, 0, 0),
			//		CheckOut = new DateTime(2022, 2, 14, 16, 0, 0),
			//		RoomId = firstRoom.Id,
			//		RoomName = firstRoom.Name,
			//		PMSRoomName = firstRoom.ExternalId,

			//		ActualCheckIn = null,
			//		ActualCheckOut = null,
			//		BedName = null,
			//		Group = null,
			//		GuestName = "Late checkout today",
			//		HotelId = firstRoom.HotelId,
			//		Id = Guid.NewGuid().ToString(),
			//		IsActive = true,
			//		IsActiveToday = true,
			//		IsSynchronizedFromRcc = true,
			//		LastTimeModifiedBySynchronization = DateTime.Now,
			//		NumberOfAdults = 1,
			//		NumberOfChildren = 1,
			//		NumberOfInfants = 0,
			//		OtherProperties = new ReservationOtherProperty[0],
			//		PMSBedName = null,
			//		PmsNote = null,
			//		RccReservationStatusKey = null,
			//		ReservationStatusDescription = null,
			//		ReservationStatusKey = null,
			//		RoomBedId = null,
			//		Vip = null,
			//		SynchronizedAt = DateTime.Now,
			//	},
			//	new Reservation
			//	{
			//		CheckIn = new DateTime(2022, 2, 14, 0, 0, 0),
			//		CheckOut = new DateTime(2022, 2, 18, 0, 0, 0),
			//		RoomId = firstRoom.Id,
			//		RoomName = firstRoom.Name,
			//		PMSRoomName = firstRoom.ExternalId,

			//		ActualCheckIn = null,
			//		ActualCheckOut = null,
			//		BedName = null,
			//		Group = null,
			//		GuestName = "Default arrival today",
			//		HotelId = firstRoom.HotelId,
			//		Id = Guid.NewGuid().ToString(),
			//		IsActive = true,
			//		IsActiveToday = true,
			//		IsSynchronizedFromRcc = true,
			//		LastTimeModifiedBySynchronization = DateTime.Now,
			//		NumberOfAdults = 1,
			//		NumberOfChildren = 1,
			//		NumberOfInfants = 0,
			//		OtherProperties = new ReservationOtherProperty[0],
			//		PMSBedName = null,
			//		PmsNote = null,
			//		RccReservationStatusKey = null,
			//		ReservationStatusDescription = null,
			//		ReservationStatusKey = null,
			//		RoomBedId = null,
			//		Vip = null,
			//		SynchronizedAt = DateTime.Now,
			//	},
			//};

			//roomsMap = new Dictionary<Guid, RoomWithHotelStructureView>();
			//roomsMap.Add(firstRoom.Id, firstRoom);

			/////////////////////////// REMOVE UNTIL HERE
			/////////////////////////// REMOVE UNTIL HERE
			/////////////////////////// REMOVE UNTIL HERE
			/////////////////////////// REMOVE UNTIL HERE

			return roomsMap;
		}

		private async Task<Dictionary<Guid, List<CleaningTimelineItemTaskData>>> _LoadCleaningTasks(string hotelId, DateTime planFromDate, DateTime planToDate)
		{
			// TODO: THIS QUERY MUST CHANGE!!!
			// TODO: THIS QUERY MUST CHANGE!!!
			// TODO: THIS QUERY MUST CHANGE!!!
			// TODO: THIS QUERY MUST CHANGE!!!
			// TODO: THIS QUERY MUST CHANGE!!!
			var tasks = await this._databaseContext
				.SystemTasks
				.Include(st => st.User)
				.Include(t => t.Actions)
				.Where(t =>

					(t.FromHotelId == hotelId || t.ToHotelId == hotelId) &&
					t.StatusKey != TaskStatusType.CANCELLED.ToString() &&
					(
						(
							t.TypeKey == TaskType.EVENT.ToString() &&
							t.EventKey == EventTaskType.CLEANING.ToString() &&
							(
								(t.EventTimeKey == TaskEventTimeType.ON_NEXT.ToString()) ||
								(t.EventTimeKey == TaskEventTimeType.ON_DATE.ToString() && planFromDate <= t.StartsAt && planToDate >= t.StartsAt) ||
								(t.EventTimeKey == TaskEventTimeType.EVERY_TIME.ToString() && t.StartsAt <= planToDate)
							)
						)
						||
						(
							t.TypeKey != TaskType.EVENT.ToString() &&
							planFromDate <= t.StartsAt &&
							planToDate >= t.StartsAt
						)
					)
			//(t.FromHotelId == hotelId || t.ToHotelId == hotelId) &&
			//t.EventKey == EventTaskType.CLEANING.ToString() &&
			//(
			//	(t.EventTimeKey == TaskEventTimeType.ON_NEXT.ToString()) ||
			//	(t.EventTimeKey == TaskEventTimeType.ON_DATE.ToString() && planFromDate <= t.StartsAt && planToDate >= t.StartsAt) ||
			//	(t.EventTimeKey == TaskEventTimeType.EVERY_TIME.ToString() && t.StartsAt <= planToDate)
			//)
			).ToArrayAsync();

			var tasksMap = new Dictionary<Guid, List<CleaningTimelineItemTaskData>>();

			foreach (var task in tasks)
			{
				var taskItem = this._CreateTimelineItemTaskData(task);

				if (task.FromRoomId.HasValue)
				{
					if (!tasksMap.ContainsKey(task.FromRoomId.Value))
					{
						tasksMap.Add(task.FromRoomId.Value, new List<CleaningTimelineItemTaskData>());
					}

					tasksMap[task.FromRoomId.Value].Add(taskItem);
				}

				if (task.ToRoomId.HasValue)
				{
					if (!tasksMap.ContainsKey(task.ToRoomId.Value))
					{
						tasksMap.Add(task.ToRoomId.Value, new List<CleaningTimelineItemTaskData>());
					}

					if (!tasksMap[task.ToRoomId.Value].Any(t => t.TaskId == taskItem.TaskId))
					{
						tasksMap[task.ToRoomId.Value].Add(taskItem);
					}
				}
			}

			return tasksMap;
		}

		private async Task<CleaningPlugin[]> _LoadCleaningPlugins(string hotelId)
		{
			/////////////////////////// REMOVE FROM HERE
			/////////////////////////// REMOVE FROM HERE
			/////////////////////////// REMOVE FROM HERE
			/////////////////////////// REMOVE FROM HERE

			//var plugins = new List<CleaningPlugin>();
			//plugins.Add(new CleaningPlugin
			//{
			//	Id = Guid.NewGuid(),
			//	Description = "Stay cleaning plugin",
			//	HotelId = hotelId,
			//	IsActive = true,
			//	IsTopRule = false,
			//	Name = "Stay cleaning plugin",
			//	OrdinalNumber = 1,
			//	Data = new CleaningPluginJson
			//	{
			//		Name = "Stay cleaning plugin",
			//		Color = null,
			//		TypeKey = "DAILY",
			//		BasedOns = new List<CleaningPluginBasedOnJson>
			//		{
			//			new CleaningPluginBasedOnJson
			//			{
			//				Id = "58ea0211-140e-4d29-88be-f7797a315fc0",
			//				Key = "OCCUPATION",
			//				Name = "Occupancies",
			//				Rooms = new HotelRoomCreditsDataJson[0],
			//				Nights = new int[0],
			//				FoorIds = new Guid[0],
			//				Sections = new string[0],
			//				CleanStay = true,
			//				Categories = new CleaningPluginBasedOnRoomCategoryJson[0],
			//				CleanVacant = false,
			//				Description = "Departure",
			//				SubSections = new string[0],
			//				ProductsTags = new string[0],
			//				NightsFromKey = null,
			//				NightsTypeKey = null,
			//				CleanDeparture = false,
			//				CleanlinessKey = null,
			//				OtherProperties = new CleaningPluginKeyValueJson[0],
			//				CleanOutOfService = false,
			//				ProductsTagsExtended = new CleaningPluginBasedOnProductsTagsExtendedJson[0],
			//				NightsEveryNumberOfDays = 0,
			//				OtherPropertiesExtended = new BasedOnOtherPropertiesExtendedJson[0],
			//				ReservationSpaceCategories = new string[0],
			//				CleanVacantEveryNumberOfDays = 7,
			//				ProductsTagsMustBeConsumedOnTime = null,
			//				ProductsTagsConsumationIntervalTo = null,
			//				ProductsTagsConsumationIntervalFrom = null
			//			},
			//		},
			//		ChangeSheets = false,
			//		Instructions = null,
			//		CleanOnSunday = true,
			//		CleanOnHolidays = true,
			//		CleanOnSaturday = true,
			//		DisplayStyleKey = null,
			//		PeriodicalIntervals = new CleaningPluginPeriodicalIntervalJson[0],
			//		PostponeUntilVacant = false,
			//		StartsCleaningAfter = 0,
			//		WeeklyCleanOnFriday = false,
			//		WeeklyCleanOnMonday = false,
			//		WeeklyCleanOnSaturday = false,
			//		WeeklyCleanOnSunday = false,
			//		WeeklyCleanOnThursday = false,
			//		WeeklyCleanOnTuesday = false,
			//		WeeklyCleanOnWednesday = false,
			//		DailyCleaningTypeTimes = new string[0],
			//		IsNightlyCleaningPlugin = false,
			//		WeeklyTimeFridayTypeKey = "ANY_TIME",
			//		WeeklyTimeMondayTypeKey = "ANY_TIME",
			//		WeeklyTimeSaturdayTypeKey = "ANY_TIME",
			//		WeeklyTimeSundayTypeKey = "ANY_TIME",
			//		WeeklyTimeThursdayTypeKey = "ANY_TIME",
			//		WeeklyTimeTuesdayTypeKey = "ANY_TIME",
			//		WeeklyTimeWednesdayTypeKey = "ANY_TIME",
			//		DailyCleaningTimeTypeKey = "ANY_TIME",
			//		WeekBasedCleaningTypeWeeks = new int[0],
			//		WeeklyCleaningTypeFridayTimes = new string[0],
			//		WeeklyCleaningTypeMondayTimes = new string[0],
			//		WeeklyCleaningTypeSaturdayTimes = new string[0],
			//		WeeklyCleaningTypeSundayTimes = new string[0],
			//		WeeklyCleaningTypeThursdayTimes = new string[0],
			//		WeeklyCleaningTypeTuesdayTimes = new string[0],
			//		WeeklyCleaningTypeWednesdayTimes = new string[0],
			//		MonthlyCleaningTypeTimeOfMonthKey = null,
			//		WeekBasedCleaningDayOfTheWeekKey = "MONDAY",
			//		PeriodicalPostponeSundayCleaningsToMonday = false,
			//	},
			//});
			//plugins.Add(new CleaningPlugin
			//{
			//	Id = Guid.NewGuid(),
			//	Description = "Departure cleaning plugin",
			//	HotelId = hotelId,
			//	IsActive = true,
			//	IsTopRule = false,
			//	Name = "Departure cleaning plugin",
			//	OrdinalNumber = 2,
			//	Data = new CleaningPluginJson
			//	{
			//		Name = "Departure cleaning plugin",
			//		Color = null,
			//		TypeKey = "DAILY",
			//		BasedOns = new List<CleaningPluginBasedOnJson>
			//		{
			//			new CleaningPluginBasedOnJson
			//			{
			//				Id = "aaaa0211-140e-4d29-88be-f7797a315fc0",
			//				Key = "OCCUPATION",
			//				Name = "Occupancies",
			//				Rooms = new HotelRoomCreditsDataJson[0],
			//				Nights = new int[0],
			//				FoorIds = new Guid[0],
			//				Sections = new string[0],
			//				CleanStay = false,
			//				Categories = new CleaningPluginBasedOnRoomCategoryJson[0],
			//				CleanVacant = false,
			//				Description = "Departure",
			//				SubSections = new string[0],
			//				ProductsTags = new string[0],
			//				NightsFromKey = null,
			//				NightsTypeKey = null,
			//				CleanDeparture = true,
			//				CleanlinessKey = null,
			//				OtherProperties = new CleaningPluginKeyValueJson[0],
			//				CleanOutOfService = false,
			//				ProductsTagsExtended = new CleaningPluginBasedOnProductsTagsExtendedJson[0],
			//				NightsEveryNumberOfDays = 0,
			//				OtherPropertiesExtended = new BasedOnOtherPropertiesExtendedJson[0],
			//				ReservationSpaceCategories = new string[0],
			//				CleanVacantEveryNumberOfDays = 7,
			//				ProductsTagsMustBeConsumedOnTime = null,
			//				ProductsTagsConsumationIntervalTo = null,
			//				ProductsTagsConsumationIntervalFrom = null
			//			},
			//		},
			//		ChangeSheets = false,
			//		Instructions = null,
			//		CleanOnSunday = true,
			//		CleanOnHolidays = true,
			//		CleanOnSaturday = true,
			//		DisplayStyleKey = null,
			//		PeriodicalIntervals = new CleaningPluginPeriodicalIntervalJson[0],
			//		PostponeUntilVacant = false,
			//		StartsCleaningAfter = 0,
			//		WeeklyCleanOnFriday = false,
			//		WeeklyCleanOnMonday = false,
			//		WeeklyCleanOnSaturday = false,
			//		WeeklyCleanOnSunday = false,
			//		WeeklyCleanOnThursday = false,
			//		WeeklyCleanOnTuesday = false,
			//		WeeklyCleanOnWednesday = false,
			//		DailyCleaningTypeTimes = new string[0],
			//		IsNightlyCleaningPlugin = false,
			//		WeeklyTimeFridayTypeKey = "ANY_TIME",
			//		WeeklyTimeMondayTypeKey = "ANY_TIME",
			//		WeeklyTimeSaturdayTypeKey = "ANY_TIME",
			//		WeeklyTimeSundayTypeKey = "ANY_TIME",
			//		WeeklyTimeThursdayTypeKey = "ANY_TIME",
			//		WeeklyTimeTuesdayTypeKey = "ANY_TIME",
			//		WeeklyTimeWednesdayTypeKey = "ANY_TIME",
			//		DailyCleaningTimeTypeKey = "ANY_TIME",
			//		WeekBasedCleaningTypeWeeks = new int[0],
			//		WeeklyCleaningTypeFridayTimes = new string[0],
			//		WeeklyCleaningTypeMondayTimes = new string[0],
			//		WeeklyCleaningTypeSaturdayTimes = new string[0],
			//		WeeklyCleaningTypeSundayTimes = new string[0],
			//		WeeklyCleaningTypeThursdayTimes = new string[0],
			//		WeeklyCleaningTypeTuesdayTimes = new string[0],
			//		WeeklyCleaningTypeWednesdayTimes = new string[0],
			//		MonthlyCleaningTypeTimeOfMonthKey = null,
			//		WeekBasedCleaningDayOfTheWeekKey = "MONDAY",
			//		PeriodicalPostponeSundayCleaningsToMonday = false,
			//	}
			//});
			//return plugins.ToArray();

			/////////////////////////// REMOVE UNTIL HERE
			/////////////////////////// REMOVE UNTIL HERE
			/////////////////////////// REMOVE UNTIL HERE
			/////////////////////////// REMOVE UNTIL HERE

			return await this._databaseContext.CleaningPlugins.Where(cp => cp.HotelId == hotelId).ToArrayAsync();
		}

		private IEnumerable<CleaningTimelineItemData> _GenerateTimelineCleanings(DateTime localCleaningDate, string timeZoneId)
		{
			var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			var cleaningDateUtc = TimeZoneInfo.ConvertTimeToUtc(localCleaningDate, timeZoneInfo);

			var cleanings = new List<CleaningTimelineItemData>();

			var cleaningDateKey = localCleaningDate.ToString("yyyy-MM-dd");
			var roomCleaningIndexMap = new Dictionary<Guid, int>();

			var cleaningsPerRoom = new Dictionary<Guid, int>();
			var cleaningsPerBed = new Dictionary<Guid, int>();

			var pluginsMap = this._cleaningPlugins == null ? new Dictionary<Guid, CleaningPlugin>() : this._cleaningPlugins.ToDictionary(cp => cp.Id);

			foreach (var results in this._cleaningResults)
			{
				if (results.IsSuccess)
				{
					foreach (var result in results.Data)
					{
						var room = this._roomsMap[result.RoomId];
						var bed = result.BedId.HasValue ? room.RoomBeds.FirstOrDefault(b => b.Id == result.BedId.Value) : null;
						var roomCleaningNumber = 0;
						var bedCleaningNumber = 0;

						if (cleaningsPerRoom.ContainsKey(room.Id))
						{
							cleaningsPerRoom[room.Id]++;
						}
						else
						{
							cleaningsPerRoom.Add(room.Id, 1);
						}
						roomCleaningNumber = cleaningsPerRoom[room.Id];

						if(bed != null)
						{
							if (cleaningsPerBed.ContainsKey(bed.Id))
							{
								cleaningsPerBed[bed.Id]++;
							}
							else
							{
								cleaningsPerBed.Add(bed.Id, 1);
							}
							bedCleaningNumber = cleaningsPerBed[bed.Id];
						}

						var plannedAttendantTasks = new List<CleaningTimelineItemTaskData>();
						var cleaningEventTasks = new List<CleaningTimelineItemTaskData>();

						if (this._cleaningTasksMap.ContainsKey(result.RoomId))
						{
							foreach(var task in this._cleaningTasksMap[result.RoomId])
							{
								if (task.IsForPlannedAttendant)
								{
									plannedAttendantTasks.Add(task);
								}
								else
								{
									cleaningEventTasks.Add(task);
								}
							}
						}

						var cleaning = new CleaningTimelineItemData
						{
							Id = Guid.Empty.ToString(),
							RoomId = result.RoomId,
							BedId = result.BedId,

							//IsClean = cleaningStatus.IsClean, // Loaded from the room if the cleanings are generated for today, predicted from reservations if the cleanings are in the future
							//IsOccupied = cleaningStatus.IsOccupied, // Loaded from the room if the cleanings are generated for today, predicted from reservations if the cleanings are in the future

							IsDoNotDisturb = room.IsDoNotDisturb, // Loaded from the room, can't be predicted
							IsOutOfOrder = room.IsOutOfOrder, // Loaded from the room, can't be predicted

							IsPostponed = false, // Newly generated cleaning is never postponed

							IsActive = true,
							IsCustom = false,
							IsTaskGuestRequest = false,
							IsTaskHighPriority = false,
							IsTaskLowPriority = false,
							TaskDescription = null,

							Title = bed == null ? room.Name : $"{room.Name} - {bed.Name}", // Cleaning timeline item title is always the room name (for now)
							IsRoomAssigned = room.BuildingId.HasValue && room.FloorId.HasValue, // Better naming would be is room assigned to a floor
							//Reservations = this._roomsMap[result.RoomId].Reservations.Select(r => this._reservationsMap[r.Id]).ToArray(), // Reservations linked to a room that needs to be cleaned
							Tasks = cleaningEventTasks, // Cleaning tasks
							PlannedAttendantTasks = plannedAttendantTasks,

							ItemTypeKey = "CLEANING",
							Credits = result.Credits,
							Price = 0,
							CleaningPluginId = result.PluginId,
							CleaningPluginName = result.PluginName,
							CleaningDescription = result.PluginName,

							IsChangeSheets = result.IsChangeSheets,

							IsPriority = result.IsPriority,

							BorderColorHex = result.PluginId != Guid.Empty && pluginsMap.ContainsKey(result.PluginId) ? pluginsMap[result.PluginId].Data?.Color : null,
						};
						cleaning.RefreshCleaningStatus(cleaningDateUtc, timeZoneId, room);

						cleanings.Add(cleaning);
					}
				}
			}

			return cleanings.OrderBy(c => c.Title).ToArray();
		}

		public IEnumerable<CleaningTimelineItemData> CreateTimelineCleanings(IEnumerable<CleaningPlanItem> items, DateTime cleaningDateUtc, string timeZoneId)
		{
			var cleanings = new List<CleaningTimelineItemData>();
			var pluginsMap = this._cleaningPlugins == null ? new Dictionary<Guid, CleaningPlugin>() : this._cleaningPlugins.ToDictionary(cp => cp.Id);

			foreach (var result in items)
			{
				var room = this._roomsMap[result.RoomId];
				var bed = result.RoomBedId.HasValue ? room.RoomBeds.FirstOrDefault(b => b.Id == result.RoomBedId.Value) : null;

				var plannedAttendantTasks = new List<CleaningTimelineItemTaskData>();
				var cleaningEventTasks = new List<CleaningTimelineItemTaskData>();

				if (this._cleaningTasksMap.ContainsKey(result.RoomId))
				{
					foreach (var task in this._cleaningTasksMap[result.RoomId])
					{
						if (task.IsForPlannedAttendant)
						{
							plannedAttendantTasks.Add(task);
						}
						else
						{
							cleaningEventTasks.Add(task);
						}
					}
				}

				var cleaning = new CleaningTimelineItemData
				{
					Id = result.Id.ToString(),
					RoomId = result.RoomId,
					BedId = result.RoomBedId,

					//IsClean = cleaningStatus.IsClean, // Loaded from the room if the cleanings are generated for today, predicted from reservations if the cleanings are in the future
					//IsOccupied = cleaningStatus.IsOccupied, // Loaded from the room if the cleanings are generated for today, predicted from reservations if the cleanings are in the future

					IsDoNotDisturb = room.IsDoNotDisturb, // Loaded from the room, can't be predicted
					IsOutOfOrder = room.IsOutOfOrder, // Loaded from the room, can't be predicted

					IsPostponed = result.IsPostponed, // Newly generated cleaning is never postponed

					IsActive = result.IsActive,
					IsCustom = result.IsCustom,
					IsTaskGuestRequest = false,
					IsTaskHighPriority = false,
					IsTaskLowPriority = false,
					TaskDescription = null,

					Title = bed == null ? room.Name : $"{room.Name} - {bed.Name}", // Cleaning timeline item title is always the room name (for now)
					IsRoomAssigned = room.BuildingId.HasValue && room.FloorId.HasValue, // Better naming would be is room assigned to a floor
																						//Reservations = this._roomsMap[result.RoomId].Reservations.Select(r => this._reservationsMap[r.Id]).ToArray(), // Reservations linked to a room that needs to be cleaned
					PlannedAttendantTasks = plannedAttendantTasks,
					Tasks = cleaningEventTasks, // Cleaning tasks
					ItemTypeKey = "CLEANING",
					Credits = result.Credits,
					Price = 0,
					CleaningPluginId = result.CleaningPluginId,
					CleaningPluginName = result.Description,
					CleaningDescription = result.Description,

					IsChangeSheets = result.IsChangeSheets,
					IsPriority = result.IsPriority,

					IsSent = result.CleaningId.HasValue,

					BorderColorHex = result.CleaningPluginId.HasValue && pluginsMap.ContainsKey(result.CleaningPluginId.Value) ? pluginsMap[result.CleaningPluginId.Value].Data?.Color : null,
				};
				cleaning.RefreshCleaningStatus(cleaningDateUtc, timeZoneId, room);

				cleanings.Add(cleaning);
			}

			return cleanings;
		}

		public IEnumerable<PlannedCleaningTimelineItemData> CreatePlannedTimelineCleanings(IEnumerable<CleaningPlanItem> items, DateTime cleaningDateUtc, string timeZoneId)
		{
			var cleanings = new List<PlannedCleaningTimelineItemData>();
			var pluginsMap = this._cleaningPlugins == null ? new Dictionary<Guid, CleaningPlugin>() : this._cleaningPlugins.ToDictionary(cp => cp.Id);

			foreach (var result in items)
			{
				var room = this._roomsMap[result.RoomId];
				var bed = result.RoomBedId.HasValue ? room.RoomBeds.FirstOrDefault(b => b.Id == result.RoomBedId.Value) : null;

				var plannedAttendantTasks = new List<CleaningTimelineItemTaskData>();
				var cleaningEventTasks = new List<CleaningTimelineItemTaskData>();

				if (this._cleaningTasksMap.ContainsKey(result.RoomId))
				{
					foreach (var task in this._cleaningTasksMap[result.RoomId])
					{
						if (task.IsForPlannedAttendant)
						{
							plannedAttendantTasks.Add(task);
						}
						else
						{
							cleaningEventTasks.Add(task);
						}
					}
				}

				var cleaning = new PlannedCleaningTimelineItemData
				{
					Id = result.Id.ToString(),
					RoomId = result.RoomId,
					BedId = result.RoomBedId,

					//IsClean = cleaningStatus.IsClean, // Loaded from the room if the cleanings are generated for today, predicted from reservations if the cleanings are in the future
					//IsOccupied = cleaningStatus.IsOccupied, // Loaded from the room if the cleanings are generated for today, predicted from reservations if the cleanings are in the future

					IsDoNotDisturb = room.IsDoNotDisturb, // Loaded from the room, can't be predicted
					IsOutOfOrder = room.IsOutOfOrder, // Loaded from the room, can't be predicted

					IsPostponed = result.IsPostponed, // Newly generated cleaning is never postponed

					IsActive = result.IsActive,
					IsCustom = result.IsCustom,
					IsTaskGuestRequest = false,
					IsTaskHighPriority = false,
					IsTaskLowPriority = false,
					TaskDescription = null,

					Title = bed == null ? room.Name : $"{room.Name} - {bed.Name}", // Cleaning timeline item title is always the room name (for now)
					IsRoomAssigned = room.BuildingId.HasValue && room.FloorId.HasValue, // Better naming would be is room assigned to a floor
																						//Reservations = this._roomsMap[result.RoomId].Reservations.Select(r => this._reservationsMap[r.Id]).ToArray(), // Reservations linked to a room that needs to be cleaned
					PlannedAttendantTasks = plannedAttendantTasks.ToArray(),
					Tasks = cleaningEventTasks, // Cleaning tasks
					ItemTypeKey = "CLEANING",
					Credits = result.Credits,
					Price = 0,
					CleaningPluginId = result.CleaningPluginId,
					CleaningPluginName = result.Description,
					CleaningDescription = result.Description,

					IsChangeSheets = result.IsChangeSheets,
					IsPriority = result.IsPriority,

					IsSent = result.CleaningId.HasValue,

					BorderColorHex = result.CleaningPluginId.HasValue && pluginsMap.ContainsKey(result.CleaningPluginId.Value) ? pluginsMap[result.CleaningPluginId.Value].Data?.Color : null,

					End = result.EndsAt.Value,
					Start = result.StartsAt.Value,
					CleaningPlanGroupId = result.CleaningPlanGroupId.Value.ToString(),
				};
				cleaning.RefreshCleaningStatus(cleaningDateUtc, timeZoneId, room);

				cleanings.Add(cleaning);
			}

			return cleanings;
		}

		private CleaningTimelineItemTaskData _CreateTimelineItemTaskData(SystemTask t)
		{
			var userFullName = "N/A";
			if (t.User != null)
			{
				userFullName = $"{t.User.FirstName} {t.User.LastName}";
			}
			else if (t.IsForPlannedAttendant)
			{
				userFullName = "Planned attendant";
			}

			var item = new CleaningTimelineItemTaskData
			{
				Actions = t.Actions.Select(a => new CleaningTimelineItemTaskActionData
				{
					ActionName = a.ActionName,
					AssetName = a.AssetName,
					AssetQuantity = a.AssetQuantity.ToString(),
				}).ToArray(),
				DurationMinutes = 0,
				TaskId = t.Id.ToString(),
				IsCompleted = t.StatusKey == TaskStatusType.FINISHED.ToString() || t.StatusKey == TaskStatusType.VERIFIED.ToString(),
				StatusKey = t.StatusKey,
				UserId = t.UserId,
				UserFullName = userFullName,
				IsForPlannedAttendant = t.IsForPlannedAttendant,
				FromReferenceId = "N/A",
				FromReferenceName = "N/A",
				FromReferenceTypeKey = "UNKNOWN",
				ToReferenceId = "N/A",
				ToReferenceName = "N/A",
				ToReferenceTypeKey = "UNKNOWN",
				//Actions = t.Actions.Select(a => new CleaningTimelineItemTaskActionData
				//{
				//	ActionName = a.ActionName,
				//	AssetName = a.AssetName,
				//	AssetQuantity = a.AssetQuantity.ToString(),
				//}).ToArray(),
				//DurationMinutes = 0,
				//TaskId = t.Id.ToString(),
				//IsCompleted = t.StatusKey == TaskStatusType.FINISHED.ToString() || t.StatusKey == TaskStatusType.VERIFIED.ToString(),
				//StatusKey = t.StatusKey,
				//UserId = t.UserId,
				//UserFullName = t.User == null ? "N/A" : $"{t.User.FirstName} {t.User.LastName}",
				//FromReferenceId = null,
				//FromReferenceName = "",
				//FromReferenceTypeKey = "NONE",
				//ToReferenceId = null,
				//ToReferenceName = "",
				//ToReferenceTypeKey = "NONE",
				//IsForPlannedAttendant = t.IsForPlannedAttendant,
			};

			if (t.FromWarehouseId.HasValue)
			{
				item.FromReferenceId = t.FromWarehouseId.Value.ToString();
				item.FromReferenceTypeKey = "WAREHOUSE";
				item.FromReferenceName = t.FromName;
			}
			else if (t.FromReservationId.IsNotNull())
			{
				item.FromReferenceId = t.FromReservationId;
				item.FromReferenceTypeKey = "RESERVATION";
				item.FromReferenceName = t.FromName;
			}
			else if (t.FromRoomId.HasValue)
			{
				item.FromReferenceId = t.FromRoomId.Value.ToString();
				item.FromReferenceTypeKey = "ROOM";
				item.FromReferenceName = t.FromName;
			}

			if (t.ToWarehouseId.HasValue)
			{
				item.ToReferenceId = t.ToWarehouseId.Value.ToString();
				item.ToReferenceTypeKey = "WAREHOUSE";
				item.ToReferenceName = t.ToName;
			}
			else if (t.ToReservationId.IsNotNull())
			{
				item.ToReferenceId = t.ToReservationId;
				item.ToReferenceTypeKey = "RESERVATION";
				item.ToReferenceName = t.ToName;
			}
			else if (t.ToRoomId.HasValue)
			{
				item.ToReferenceId = t.ToRoomId.Value.ToString();
				item.ToReferenceTypeKey = "ROOM";
				item.ToReferenceName = t.ToName;
			}

			return item;
		}

		private CleaningGeneratorResponse _CalculateCleanings(string hotelId, bool isToday, DateTime cleaningDate, TimeSpan defaultCheckInTime, TimeSpan defaultCheckOutTime)
		{
			var cleaningRequestRooms = new List<CleaningProviderRequest.Room>();
			
			foreach (var room in this._roomsMap.Values)
			{
				if(!room.FloorId.HasValue || !room.BuildingId.HasValue)
				{
					continue;
				}

				if(room.TypeKey == RoomTypeEnum.HOSTEL.ToString())
				{
					foreach(var bed in room.RoomBeds)
					{
						var roomRequest = new CleaningProviderRequest.Room
						{
							RoomId = room.Id,
							BedId = bed.Id,
							IsBed = true,
							ExternalId = bed.ExternalId,
							FloorId = room.FloorId.HasValue ? room.FloorId.Value : Guid.Empty,
							HotelId = room.HotelId,
							IsDoNotDisturb = room.IsDoNotDisturb,
							IsOutOfService = room.IsOutOfOrder,
							Name = $"{bed.Name} [{room.Name}]",
							Section = room.FloorSectionName,
							SubSection = room.FloorSubSectionName,
							IsPriority = isToday ? bed.IsCleaningPriority : false,
							Category = room.Category == null ? null : new CleaningProviderRequest.RoomCategory
							{
								Id = room.Category.Id,
								//Credits = room.Category.Credits,
								Name = room.Category.Name
							},
							Reservations = bed.Reservations == null ? new CleaningProviderRequest.Reservation[0] : room.Reservations.Select(r =>
							{
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

									// THESE TWO PROPETIES ARE NOT USED AND SHOULD BE REMOVED IN THE FUTURE
									// THESE TWO PROPETIES ARE NOT USED AND SHOULD BE REMOVED IN THE FUTURE
									IsCheckedIn = false, // checkInDate.HasValue && checkInDate.Value >= cleaningDate,
									IsCheckedOut = false, // checkOutDate.HasValue && checkOutDate.Value > cleaningDate,
								};
							}).ToArray(),
						};
						cleaningRequestRooms.Add(roomRequest);
					}
				}
				else
				{
					var roomRequest = new CleaningProviderRequest.Room
					{
						RoomId = room.Id,
						BedId = null,
						IsBed = false,
						ExternalId = room.ExternalId,
						FloorId = room.FloorId.HasValue ? room.FloorId.Value : Guid.Empty,
						HotelId = room.HotelId,
						IsDoNotDisturb = room.IsDoNotDisturb,
						IsOutOfService = room.IsOutOfOrder,
						Name = room.Name,
						Section = room.FloorSectionName,
						SubSection = room.FloorSubSectionName,
						IsPriority = isToday ? room.IsCleaningPriority : false,
						Category = room.Category == null ? null : new CleaningProviderRequest.RoomCategory
						{
							Id = room.Category.Id,
							//Credits = room.Category.Credits,
							Name = room.Category.Name
						},
						Reservations = room.Reservations == null ? new CleaningProviderRequest.Reservation[0] : room.Reservations.Select(r =>
						{
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

								// THESE TWO PROPETIES ARE NOT USED AND SHOULD BE REMOVED IN THE FUTURE
								// THESE TWO PROPETIES ARE NOT USED AND SHOULD BE REMOVED IN THE FUTURE
								IsCheckedIn = false, // checkInDate.HasValue && checkInDate.Value >= cleaningDate,
								IsCheckedOut = false, // checkOutDate.HasValue && checkOutDate.Value > cleaningDate,
							};
						}).ToArray(),
					};
					cleaningRequestRooms.Add(roomRequest);
				}
			}

			var cleaningPlugins = this._cleaningPlugins.Select(p => new CleaningProviderPlugin
			{
				BasedOns = p.Data.BasedOns == null ? new CleaningPluginBasedOn[0] : p.Data.BasedOns.Select(bo => new CleaningPluginBasedOn
				{
					CleanDeparture = bo.CleanDeparture ?? false,
					CleanOutOfService = bo.CleanOutOfService ?? false,
					CleanStay = bo.CleanStay ?? false,
					CleanVacant = bo.CleanVacant ?? false,
					FoorIds = bo.FoorIds,
					Id = bo.Id,
					Nights = bo.Nights,
					//ProductsTags = bo.ProductsTags,
					CleanlinessKey = bo.CleanlinessKey,
					ReservationSpaceCategories = bo.ReservationSpaceCategories,
					RoomCategories = bo.Categories.Where(c => c.IsSelected).Select(c => new CleaningPluginBasedOnRoomCategory
					{
						CategoryId = c.CategoryId,
						Credits = c.Credits,
					}).ToArray(),
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
				}).ToArray(),
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
				HotelId = hotelId,
				Id = p.Id,
				Instructions = p.Data.Instructions,
				IsActive = p.IsActive,
				IsTopRule = p.IsTopRule,
				MonthlyCleaningTypeTimeOfMonthKey = p.Data.MonthlyCleaningTypeTimeOfMonthKey,
				Name = p.Data.Name,
				OrdinalNumber = p.OrdinalNumber,
				PeriodicalIntervals = p.Data.PeriodicalIntervals == null ? new CleaningPluginPeriodicalInterval[0] : p.Data.PeriodicalIntervals.Select(pi => new CleaningPluginPeriodicalInterval
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

			return this._cleaningProvider.CalculateCleanings(cleaningDate, cleaningRequestRooms, cleaningPlugins);
		}
	}
}
