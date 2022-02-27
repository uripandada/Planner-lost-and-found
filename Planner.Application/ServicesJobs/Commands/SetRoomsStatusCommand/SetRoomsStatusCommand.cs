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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ServicesJobs.Commands.SetRoomsStatusCommand
{
	public class AutomaticHkHotelChanges
	{
		public string HotelId { get; set; }
		public string HotelName { get; set; }
		public DateTime ChangedAt { get; set; }
		public List<AutomaticHkRoomChanges> RoomChanges { get; set; } = new List<AutomaticHkRoomChanges>();
	}

	public class AutomaticHkRoomChanges
	{
		public Guid RoomId { get; set; }
		public string RoomName { get; set; }
		public Guid? BedId { get; set; }
		public string BedName { get; set; }
		public bool OldIsClean { get; set; }
		public bool NewIsClean { get; set; }
		public bool OldDnd { get; set; }
		public bool NewDnd { get; set; }
		public bool OldIsInspected { get; set; }
		public bool NewIsInspected { get; set; }
		public bool OldIsCleaningPriority { get; set; }
		public bool NewIsCleaningPriority { get; set; }
		public bool OldIsCleaningInProgress { get; set; }
		public bool NewIsCleaningInProgress { get; set; }
	}

	public class SetRoomsStatusCommand : IRequest<bool>
	{
		/// <summary>
		/// If this flag is set, do the update out of the first hour after the midnight
		/// </summary>
		public bool? OverrideMidnightTime { get; set; }
	}

	public class SetRoomsStatusCommandHandler : IRequestHandler<SetRoomsStatusCommand, bool>, IAmServiceJobsApplicationHandler, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public SetRoomsStatusCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<bool> Handle(SetRoomsStatusCommand request, CancellationToken cancellationToken)
		{
			var nowUtc = DateTime.UtcNow;
			var previousCycle = (Domain.Entities.AutomaticHousekeepingUpdateCycle)null;
			var cycle = (Domain.Entities.AutomaticHousekeepingUpdateCycle)null;
			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				previousCycle = await this._databaseContext.HousekeepingNightlyUpdateCycles.OrderByDescending(c => c.StartedAt).FirstOrDefaultAsync();
				var hoursPassedSinceTheLastCycle = previousCycle == null ? 0 : (nowUtc - previousCycle.StartedAt).TotalHours;

				if (previousCycle != null && previousCycle.InProgress && hoursPassedSinceTheLastCycle >= 12)
				{
					previousCycle.EndedAt = nowUtc;
					previousCycle.InProgress = false;
					previousCycle.StateChanges = (previousCycle.StateChanges.IsNotNull() ? previousCycle.StateChanges + " " : "") + "ALERT! CYCLE DIDN'T FINISH IN TIME. FORCIBLY FINISHED BY THE NEXT CYCLE AFTER 12 HOURS.";
				}

				if (previousCycle == null || !previousCycle.InProgress)
				{
					cycle = new Domain.Entities.AutomaticHousekeepingUpdateCycle
					{
						InProgress = true,
						EndedAt = null,
						Id = Guid.NewGuid(),
						StartedAt = nowUtc,
						StateChanges = null,
					};

					await this._databaseContext.HousekeepingNightlyUpdateCycles.AddAsync(cycle);
					await this._databaseContext.SaveChangesAsync(cancellationToken);
					await transaction.CommitAsync(cancellationToken);
				}
				else if (previousCycle != null && previousCycle.InProgress)
				{
					return false;
				}
			}

			var hotels = await this._databaseContext.Hotels.ToListAsync();
			var hotelIdsToProcess = new HashSet<string>();

			if (request.OverrideMidnightTime.HasValue && request.OverrideMidnightTime.Value)
			{
				hotelIdsToProcess = hotels.Select(h => h.Id).ToHashSet();
			}
			else
			{
				foreach (var hotel in hotels)
				{
					var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
					var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
					var localHotelDate = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, timeZoneInfo);

					// If it is the first cycle then the previous one will be null. If it is null it is important
					// just to the the preivous cycle date to a date that is before local hotel date.
					var previousCycleLocalDate = TimeZoneInfo.ConvertTimeFromUtc(previousCycle == null ? localHotelDate.Date.AddDays(-10) : previousCycle.StartedAt, timeZoneInfo);
					var isFirstCycleOfTheDay = previousCycleLocalDate.Date < localHotelDate.Date;

					// If the sync is in the first hour of the day for the hotel and is the first cycle of the day - from midnight to 1am and is the first cycle of the day.
					// CONDITION: It must happen only once and in the the first 60 minutes of the day.
					// CONDITION: It must happen only once and in the the first 60 minutes of the day.
					//if (localHotelDate.TimeOfDay.TotalMinutes < 60 && isFirstCycleOfTheDay)
					//{
					//	hotelIdsToProcess.Add(hotel.Id);
					//}

					// ALLOW IT ONLY ONCE IN THE DAY
					if (isFirstCycleOfTheDay)
					{
						hotelIdsToProcess.Add(hotel.Id);
					}
				}
			}
			if (hotelIdsToProcess.Any())
			{
				var changes = new List<AutomaticHkHotelChanges>();
				try
				{
					var settingsMap = (await this._databaseContext.AutomaticHousekeepingUpdateSettingss.ToListAsync())
						.GroupBy(r => r.HotelId)
						.ToDictionary(group => group.Key, group => group.ToArray());

					var roomsMap = (await this._databaseContext.Rooms.Include(r => r.RoomBeds).ToListAsync())
						.GroupBy(r => r.HotelId)
						.ToDictionary(group => group.Key, group => group.ToArray());

					foreach (var hotelId in hotelIdsToProcess)
					{
						var hotel = hotels.First(h => h.Id == hotelId);
						var dateProvider = new HotelLocalDateProvider();
						var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, hotel.Id, true);

						var hotelChanges = new AutomaticHkHotelChanges
						{
							HotelId = hotel.Id,
							HotelName = hotel.Name,
							ChangedAt = dateTime,
						};
						changes.Add(hotelChanges);

						if (!settingsMap.ContainsKey(hotel.Id)) continue;
						if (!roomsMap.ContainsKey(hotel.Id)) continue;

						var hotelRooms = roomsMap[hotel.Id];
						var hotelSettings = settingsMap[hotel.Id];

						foreach (var room in hotelRooms)
						{
							if (room.TypeKey == "HOSTEL")
							{
								if (room.RoomBeds == null || !room.RoomBeds.Any())
									continue;

								foreach (var bed in room.RoomBeds)
								{
									var bedChanges = new AutomaticHkRoomChanges
									{
										BedId = bed.Id,
										BedName = bed.Name,
										RoomId = room.Id,
										RoomName = room.Name,
										OldDnd = bed.IsDoNotDisturb,
										NewDnd = bed.IsDoNotDisturb,
										OldIsClean = bed.IsClean,
										NewIsClean = bed.IsClean,
										OldIsInspected = bed.IsInspected,
										NewIsInspected = bed.IsInspected,
										OldIsCleaningPriority = bed.IsCleaningPriority,
										NewIsCleaningPriority = bed.IsCleaningPriority,
										NewIsCleaningInProgress = bed.IsCleaningInProgress,
										OldIsCleaningInProgress = bed.IsCleaningInProgress,
									};

									var setting = hotelSettings.FirstOrDefault(s =>
										(!s.Dirty || (s.Dirty && !bed.IsClean)) &&
										(!s.Clean || (s.Clean && bed.IsClean)) &&
										(!s.Inspected || (s.Inspected && bed.IsInspected)) &&
										(!s.Vacant || (s.Vacant && !bed.IsOccupied)) &&
										(!s.Occupied || (s.Occupied && bed.IsOccupied)) &&
										(!s.DoNotDisturb || (s.DoNotDisturb && bed.IsDoNotDisturb)) &&
										(!s.DoDisturb || (s.DoDisturb && !bed.IsDoNotDisturb)) &&
										(!s.OutOfService || (s.OutOfService && bed.IsOutOfService)) &&
										(!s.InService || (s.InService && !bed.IsOutOfService))
									);

									if (setting != null)
									{
										bed.IsClean = setting.UpdateStatusTo == AutomaticHousekeepingUpdateCleaningStatusTo.CLEAN;
										bedChanges.NewIsClean = bed.IsClean;
									}

									bed.IsDoNotDisturb = false;
									bed.IsInspected = false;
									bed.IsCleaningPriority = false;
									bed.IsCleaningInProgress = false;

									bedChanges.NewDnd = false;
									bedChanges.NewIsInspected = false;
									bedChanges.NewIsCleaningPriority = false;
									bedChanges.NewIsCleaningInProgress = false;

									if (bedChanges.OldDnd != bedChanges.NewDnd || 
										bedChanges.OldIsClean != bedChanges.NewIsClean || 
										bedChanges.OldIsInspected != bedChanges.NewIsInspected || 
										bedChanges.OldIsCleaningPriority != bedChanges.NewIsCleaningPriority || 
										bedChanges.OldIsCleaningInProgress != bedChanges.NewIsCleaningInProgress)
									{
										var newHousekeepingDetails = bed.CalculateCurrentHousekeepingStatus();
										bed.RccHousekeepingStatus = newHousekeepingDetails.RccHousekeepingStatusCode;
									}

									hotelChanges.RoomChanges.Add(bedChanges);
								}
							}
							else
							{
								var roomChanges = new AutomaticHkRoomChanges
								{
									BedId = null,
									BedName = null,
									RoomId = room.Id,
									RoomName = room.Name,
									OldDnd = room.IsDoNotDisturb,
									NewDnd = room.IsDoNotDisturb,
									OldIsClean = room.IsClean,
									NewIsClean = room.IsClean,
									NewIsInspected = room.IsInspected,
									OldIsInspected = room.IsInspected,
									OldIsCleaningPriority = room.IsCleaningPriority,
									NewIsCleaningPriority = room.IsCleaningPriority,
									NewIsCleaningInProgress = room.IsCleaningInProgress,
									OldIsCleaningInProgress = room.IsCleaningInProgress,
								};

								var setting = hotelSettings.FirstOrDefault(s =>
									(!s.Dirty || (s.Dirty && !room.IsClean)) ||
									(!s.Clean || (s.Clean && room.IsClean)) ||
									(!s.Inspected || (s.Inspected && room.IsInspected)) ||
									(!s.Vacant || (s.Vacant && !room.IsOccupied)) ||
									(!s.Occupied || (s.Occupied && room.IsOccupied)) ||
									(!s.DoNotDisturb || (s.DoNotDisturb && room.IsDoNotDisturb)) ||
									(!s.DoDisturb || (s.DoDisturb && !room.IsDoNotDisturb)) ||
									(!s.OutOfService || (s.OutOfService && room.IsOutOfService)) ||
									(!s.InService || (s.InService && !room.IsOutOfService) ||
									(s.RoomNameRegex.IsNull() || (s.RoomNameRegex.IsNotNull() && Regex.IsMatch(room.Name, s.RoomNameRegex, RegexOptions.IgnoreCase))))
								);

								if (setting != null)
								{
									room.IsClean = setting.UpdateStatusTo == AutomaticHousekeepingUpdateCleaningStatusTo.CLEAN;
									roomChanges.NewIsClean = room.IsClean;
								}

								room.IsDoNotDisturb = false;
								room.IsInspected = false;
								room.IsCleaningPriority = false;
								room.IsCleaningInProgress = false;

								roomChanges.NewDnd = false;
								roomChanges.NewIsInspected = false;
								roomChanges.NewIsCleaningPriority = false;
								roomChanges.NewIsCleaningInProgress = false;

								if (roomChanges.OldDnd != roomChanges.NewDnd ||
									roomChanges.OldIsClean != roomChanges.NewIsClean ||
									roomChanges.OldIsInspected != roomChanges.NewIsInspected ||
									roomChanges.OldIsCleaningPriority != roomChanges.NewIsCleaningPriority ||
									roomChanges.OldIsCleaningInProgress != roomChanges.NewIsCleaningInProgress)
								{
									var newHousekeepingDetails = room.CalculateCurrentHousekeepingStatus();
									room.RccHousekeepingStatus = newHousekeepingDetails.RccHousekeepingStatusCode;
								}

								hotelChanges.RoomChanges.Add(roomChanges);
							}
						}


					}

					cycle.StateChanges = Newtonsoft.Json.JsonConvert.SerializeObject(changes, new Newtonsoft.Json.JsonSerializerSettings { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

					await this._databaseContext.SaveChangesAsync(cancellationToken);
				}
				catch (Exception ex)
				{
					cycle.StateChanges = (cycle.StateChanges.IsNotNull() ? cycle.StateChanges + " " : "") + "Error happened while updating HK statuses. " + ex.Message;
				}
			}
			else
			{
				cycle.StateChanges = (cycle.StateChanges.IsNotNull() ? cycle.StateChanges + " " : "") + "No changes";
			}
			
			cycle.EndedAt = DateTime.UtcNow;
			cycle.InProgress = false;

			//this._databaseContext.HousekeepingNightlyUpdateCycles.Update(cycle);

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return true;
		}
	}
}
