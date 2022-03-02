using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.ExternalApi.Infrastructure;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExternalApi.Tasks.Commands.ExternalInsertCompactTask
{
	public class ExternalInsertCompactTaskAssetAction
	{
		/// <summary>
		/// If not set defaults to 1
		/// </summary>
		public int? AssetQuantity { get; set; }
		/// <summary>
		/// You can choose to set either asset id or asset name
		/// </summary>
		public Guid? AssetId { get; set; }
		/// <summary>
		/// You can choose to set either asset id or asset name
		/// </summary>
		public string AssetName { get; set; }
		/// <summary>
		/// You can choose to set either action id or action name
		/// </summary>
		public Guid? ActionId { get; set; }
		/// <summary>
		/// You can choose to set either action id or action name
		/// </summary>
		public string ActionName { get; set; }
	}

	public class ExternalInsertCompactTaskCommand: IRequest<ProcessResponseSimple<Guid>>
	{
		/// <summary>
		/// You can choose to set either hotelGroupId or hotelGroupKey
		/// </summary>
		public Guid? HotelGroupId { get; set; }
		/// <summary>
		/// You can choose to set either hotelGroupId or hotelGroupKey
		/// </summary>
		public string HotelGroupKey { get; set; }
		public string HotelId { get; set; }
		/// <summary>
		/// You can choose to set either roomId or roomName
		/// </summary>
		public Guid? RoomId { get; set; }
		/// <summary>
		/// You can choose to set either roomId or roomName
		/// </summary>
		public string RoomName { get; set; }
		/// <summary>
		/// You can choose to set either roomBedId or roomBedName
		/// </summary>
		public Guid? RoomBedId { get; set; }
		/// <summary>
		/// You can choose to set either roomBedId or roomBedName
		/// </summary>
		public string RoomBedName { get; set; }
		public string ReservationId { get; set; }

		/// <summary>
		/// ROOM, RESERVATION
		/// </summary>
		public string LocationTypeKey { get; set; }

		public Guid? UserId { get; set; }

		public IEnumerable<ExternalInsertCompactTaskAssetAction> AssetActions { get; set; }
		
		/// <summary>
		/// A string description to identify who made the request
		/// </summary>
		public string RequestedBy { get; set; }
		public string Comment { get; set; }

		public int? Credits { get; set; }

		public bool? IsForPlannedAttendant { get; set; }
		public bool? IsGuestRequest { get; set; }
		public decimal? Price { get; set; }
		public string PriorityKey { get; set; }
		/// <summary>
		/// You can choose to set the date and time for the task. If not set, current local time is used.
		/// </summary>
		public DateTime? StartsAt { get; set; }
	}

	public class ExternalInsertCompactTaskCommandHandler : ExternalApiBaseHandler, IRequestHandler<ExternalInsertCompactTaskCommand, ProcessResponseSimple<Guid>>, IAmWebApplicationHandler
	{
		private readonly ISystemEventsService _systemEventsService;

		public ExternalInsertCompactTaskCommandHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService systemEventsService)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._databaseContext = databaseContext;
			this._systemEventsService = systemEventsService;
			this._contextAccessor = contextAccessor;
		}

		public async Task<ProcessResponseSimple<Guid>> Handle(ExternalInsertCompactTaskCommand request, CancellationToken cancellationToken)
		{
			var initResult = await this._Initialize(request.HotelGroupId, request.HotelGroupKey);
			if (initResult != null)
			{
				return initResult;
			}

			var isForPlannedAttendant = request.IsForPlannedAttendant ?? false;

			var hotel = (Domain.Entities.Hotel)null;

			if (request.HotelId.IsNotNull())
			{
				hotel = await this._databaseContext.Hotels
					.Where(aa => aa.Id == request.HotelId)
					.FirstOrDefaultAsync();

				if (hotel == null)
				{
					return new ProcessResponseSimple<Guid>
					{
						Data = Guid.Empty,
						IsSuccess = false,
						Message = $"Unable to find hotel with id: {request.HotelId}",
					};
				}
			}
			else
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Hotel id is not set.",
				};
			}

			var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			var localHotelDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

			var insertData = await this._LoadDataFromRequest(request);

			if (insertData.HasErrors)
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = String.Join(" ", insertData.ErrorMessages)
				};
			}

			var systemTaskConfigurationWho = new SystemTaskConfigurationWho
			{
				ReferenceId = "",
				ReferenceName = "",
				TypeDescription = "",
				TypeKey = "",
			};

			if (isForPlannedAttendant)
			{
				systemTaskConfigurationWho.ReferenceId = Guid.Empty.ToString();
				systemTaskConfigurationWho.ReferenceName = $"Planned attendant";
				systemTaskConfigurationWho.TypeDescription = "Anyone who will do the cleaning";
				systemTaskConfigurationWho.TypeKey = TaskWhoType.PLANNED_ATTENDANT.ToString();
			}
			else
			{
				systemTaskConfigurationWho.ReferenceId = insertData.User.Id.ToString();
				systemTaskConfigurationWho.ReferenceName = $"{insertData.User.FirstName} {insertData.User.LastName}";
				systemTaskConfigurationWho.TypeDescription = "User";
				systemTaskConfigurationWho.TypeKey = TaskWhoType.USER.ToString();
			}

			var systemTaskConfigurationWhere = new SystemTaskConfigurationWhere
			{
				ReferenceId = "",
				ReferenceName = "",
				TypeDescription = "",
				TypeKey = "",
			};

			if (request.LocationTypeKey == "RESERVATION")
			{
				systemTaskConfigurationWhere.ReferenceId = insertData.Reservation.Id;
				systemTaskConfigurationWhere.ReferenceName = insertData.Reservation.GuestName;
				systemTaskConfigurationWhere.TypeKey = TaskWhereType.RESERVATION.ToString();
				systemTaskConfigurationWhere.TypeDescription = "Reservation";
			}
			else if(request.LocationTypeKey == "ROOM")
			{
				if(insertData.RoomBed == null)
				{
					systemTaskConfigurationWhere.ReferenceId = insertData.Room.Id.ToString();
					systemTaskConfigurationWhere.ReferenceName = insertData.Room.Name;
					systemTaskConfigurationWhere.TypeKey = TaskWhereType.ROOM.ToString();
					systemTaskConfigurationWhere.TypeDescription = "Room";
				}
				else
				{
					systemTaskConfigurationWhere.ReferenceId = insertData.RoomBed.Id.ToString();
					systemTaskConfigurationWhere.ReferenceName = insertData.RoomBed.Name;
					systemTaskConfigurationWhere.TypeKey = TaskWhereType.BED.ToString();
					systemTaskConfigurationWhere.TypeDescription = "Bed";
				}
			}

			var systemTaskConfiguration = new Domain.Entities.SystemTaskConfiguration
			{
				CreatedAt = localHotelDateTime,
				CreatedById = null,
				Id = Guid.NewGuid(),
				ModifiedAt = localHotelDateTime,
				ModifiedById = null,
				Data = new SystemTaskConfigurationData
				{
					Whos = new SystemTaskConfigurationWho[] {
						systemTaskConfigurationWho
					},
					Files = new SystemTaskConfigurationFile[0],
					Price = request.Price ?? 0,
					Whats = insertData.SystemTaskActions.Select(ta => new SystemTaskConfigurationWhat 
					{ 
						ActionName = ta.ActionName,
						AssetGroupId = ta.AssetGroupId,
						AssetGroupName = ta.AssetGroupName,
						AssetId = ta.AssetId,
						AssetName = ta.AssetName,
						AssetQuantity = ta.AssetQuantity,	
						IsActionSystemDefined = false,
						SystemDefinedActionIdentifierKey = "NONE",
						SystemDefinedActionTypeKey = "NONE",
					}).ToArray(),
					Wheres = new SystemTaskConfigurationWhere[]
					{
						systemTaskConfigurationWhere,
					},
					StartsAtTimes = new DateTime[] { request.StartsAt ?? localHotelDateTime },
					EventModifierKey = null,
					EventTimeKey = null,
					ExcludeHolidays = null,
					ExcludeWeekends = null,
					IsBlockingCleaningUntilFinished = false,
					IsGuestRequest = request.IsGuestRequest ?? false,
					Comment = request.Comment,
					Credits = request.Credits ?? 0,
					EndsAtTime = null,
					EventKey = null,
					IsMajorNotificationRaisedWhenFinished = false,
					IsRescheduledEveryDayUntilFinished = false,
					IsShownInNewsFeed = false,
					MustBeFinishedByAllWhos = true,
					PostponeWhenRoomIsOccupied = null,
					PriorityKey = request.PriorityKey,
					RecurringEveryNumberOfDays = 0,
					RecurringTaskRepeatTimes = null,
					RecurringTaskTypeKey = "DAILY",
					RepeatsForKey = null,
					RepeatsForNrDays = null,
					RepeatsForNrOccurences = null,
					RepeatsUntilTime = null,
					TaskTypeKey = "SINGLE",
					WhatsTypeKey = "LIST",
					WhereFrom = null,
					WhereTo = null,
				},

			};

			var systemTaskId = Guid.NewGuid();
			var systemTask = new SystemTask
			{
				Id = systemTaskId,
				Comment = request.Comment,
				CreatedAt = localHotelDateTime,
				CreatedById = null,
				ModifiedAt = localHotelDateTime,
				ModifiedById = null,
				Actions = insertData.SystemTaskActions.Select(ta => new SystemTaskAction 
				{ 
					SystemTaskId = systemTaskId,
					ActionName = ta.ActionName,
					AssetGroupId = ta.AssetGroupId,
					AssetGroupName = ta.AssetGroupName,
					AssetId = ta.AssetId,
					AssetName = ta.AssetName,
					AssetQuantity = ta.AssetQuantity,
					Id = Guid.NewGuid(),
				}).ToArray(),
				Credits = systemTaskConfiguration.Data.Credits,
				Price = systemTaskConfiguration.Data.Price,
				IsGuestRequest = systemTaskConfiguration.Data.IsGuestRequest,
				PriorityKey = systemTaskConfiguration.Data.PriorityKey,
				StartsAt = systemTaskConfiguration.Data.StartsAtTimes.First(),
				IsForPlannedAttendant = request.IsForPlannedAttendant ?? false,
				UserId = request.UserId,

				SystemTaskConfigurationId = systemTaskConfiguration.Id,
				MustBeFinishedByAllWhos = true,
				
				EventKey = null,
				EventModifierKey = null,
				EventTimeKey = null,
				RecurringTypeKey = Common.Enums.RecurringTaskType.DAILY.ToString(),
				RepeatsForKey = null,
				StatusKey = Common.Enums.TaskStatusType.PENDING.ToString(),
				TypeKey = Common.Enums.TaskType.SINGLE.ToString(),
				
				WhereTypeKey = "TO",

				FromHotelId = null,
				FromName = null,
				FromReservationId = null,
				FromRoomId = null,
				FromWarehouseId = null,
				
				ToHotelId = request.HotelId,
				ToName = systemTaskConfiguration.Data.Wheres.First().ReferenceName,
				ToReservationId = request.ReservationId,
				ToRoomId = insertData.Room == null ? null : insertData.Room.Id,
				ToWarehouseId = null,

				IsBlockingCleaningUntilFinished = false,
				IsMajorNotificationRaisedWhenFinished = false,
				IsManuallyModified = true,
				IsRescheduledEveryDayUntilFinished = false,
				IsShownInNewsFeed = false,
			};

			var systemTaskHistory = new SystemTaskHistory
			{
				Id = Guid.NewGuid(),
				ChangedByKey = "EXTERNAL_API",
				CreatedAt = localHotelDateTime,
				CreatedById = null,
				SystemTaskId = systemTask.Id,
				NewData = new SystemTaskHistoryData(),
				OldData = new SystemTaskHistoryData(),
				Message = "Task created by an external API.",
			};

			if (isForPlannedAttendant)
			{
				await this._UpdateForPlannedAttendant(systemTask, localHotelDateTime);
			}

			await this._databaseContext.SystemTaskConfigurations.AddAsync(systemTaskConfiguration);
			await this._databaseContext.SystemTasks.AddAsync(systemTask);
			await this._databaseContext.SystemTaskHistorys.AddAsync(systemTaskHistory);

			var userIds = new List<Guid>();
			if (systemTask.UserId.HasValue)
			{
				userIds.Add(systemTask.UserId.Value);
			}

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, new Guid[] { systemTask.Id }, "You have new tasks.");

			return new ProcessResponseSimple<Guid>
			{
				Data = systemTask.Id,
				IsSuccess = true,
				Message = "Task created.",
			};
		}

		private async Task<InsertExternalTaskData> _LoadDataFromRequest(ExternalInsertCompactTaskCommand command)
		{
			var data = new InsertExternalTaskData
			{
				ErrorMessages = new List<string>(),
				SystemTaskActions = new List<SystemTaskAction>(),
			};

			if (command.PriorityKey.IsNull())
			{
				data.ErrorMessages.Add("Priority key is required. Accepted values: 'NORMAL' or 'HIGH'.");
				data.HasErrors = true;
			}
			else if (!new string[] { "NORMAL", "HIGH" }.Contains(command.PriorityKey))
			{
				data.ErrorMessages.Add($"Priority key {command.PriorityKey} is invalid. Accepted values: 'NORMAL' or 'HIGH'.");
				data.HasErrors = true;
			}


			if (!(command.IsForPlannedAttendant ?? false) && !command.UserId.HasValue)
			{
				data.ErrorMessages.Add("User ID is required when the task is not for a planned attendant.");
				data.HasErrors = true; 
			}
			else if(!(command.IsForPlannedAttendant ?? false) && command.UserId.HasValue)
			{
				var userId = command.UserId.Value;
				var user = await this._databaseContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
				if(user == null)
				{
					data.ErrorMessages.Add($"Unable to find user with ID: {userId.ToString()}.");
					data.HasErrors = true;
				}
				else
				{
					data.User = user;
				}
			}

			if(command.LocationTypeKey == "ROOM")
			{
				if (command.HotelId.IsNull())
				{
					data.ErrorMessages.Add("'ROOM' LocationTypeKey must have HotelId set.");
					data.HasErrors = true;
				}

				// 
				if (!command.RoomBedId.HasValue && command.RoomBedName.IsNull() && !command.RoomId.HasValue && command.RoomName.IsNull())
				{
					data.ErrorMessages.Add("'ROOM' LocationTypeKey must have RoomId, RoomName, RoomBedId or RoomBedName set.");
					data.HasErrors = true;
				}
				else
				{
					if (command.RoomBedId.HasValue)
					{
						var bedId = command.RoomBedId.Value;
						var bed = await this._databaseContext.RoomBeds.Include(rb => rb.Room).FirstOrDefaultAsync(rb => rb.Id == bedId && rb.Room.HotelId == command.HotelId);
						if(bed == null)
						{
							data.ErrorMessages.Add($"Unable to find bed with ID {bedId}.");
							data.HasErrors = true;
						}
						else
						{
							data.RoomBed = bed;
							data.Room = bed.Room;
						}
					}
					else if (command.RoomBedName.IsNotNull())
					{
						var bed = await this._databaseContext.RoomBeds.Include(rb => rb.Room).FirstOrDefaultAsync(rb => rb.Name == command.RoomBedName && rb.Room.HotelId == command.HotelId);
						if (bed == null)
						{
							data.ErrorMessages.Add($"Unable to find bed with name {command.RoomBedName}.");
							data.HasErrors = true;
						}
						else
						{
							data.RoomBed = bed;
							data.Room = bed.Room;
						}
					}
					else if (command.RoomId.HasValue)
					{
						var roomId = command.RoomId.Value;
						var room = await this._databaseContext.Rooms.FirstOrDefaultAsync(rb => rb.Id == roomId && rb.HotelId == command.HotelId);
						if (room == null)
						{
							data.ErrorMessages.Add($"Unable to find room with ID {roomId}.");
							data.HasErrors = true;
						}
						else
						{
							data.Room = room;
						}
					}
					else if (command.RoomName.IsNotNull())
					{
						var room = await this._databaseContext.Rooms.FirstOrDefaultAsync(r => r.Name == command.RoomName && r.HotelId == command.HotelId);
						if (room == null)
						{
							data.ErrorMessages.Add($"Unable to find room with name {command.RoomName}.");
							data.HasErrors = true;
						}
						else
						{
							data.Room = room;
						}
					}
				}
			}
			else if(command.LocationTypeKey == "RESERVATION")
			{
				var hasError = false;
				if (command.ReservationId.IsNull())
				{
					data.ErrorMessages.Add("'RESERVATION' LocationTypeKey must have ReservationId set.");
					data.HasErrors = true;
					hasError = true;
				}

				if (command.HotelId.IsNull())
				{
					data.ErrorMessages.Add("'RESERVATION' LocationTypeKey must have HotelId set.");
					data.HasErrors = true;
					hasError = true;
				}

				if (!hasError)
				{
					var reservation = await this._databaseContext.Reservations.FirstOrDefaultAsync(r => r.Id == command.ReservationId && r.HotelId == command.HotelId);

					if(reservation == null)
					{
						data.ErrorMessages.Add($"Reservation with ID {command.ReservationId} doesn't exist.");
					}
					else
					{
						if(!reservation.RoomId.HasValue && !reservation.RoomBedId.HasValue)
						{
							data.ErrorMessages.Add("Invalid LocationTypeKey. Accepted values: 'ROOM' or 'RESERVATION'.");
							data.HasErrors = true;
						}
						else
						{
							if (reservation.RoomBedId.HasValue)
							{
								var roomBedId = reservation.RoomBedId.Value;
								var roomBed = await this._databaseContext.RoomBeds.Include(rb => rb.Room).FirstOrDefaultAsync(rb => rb.Id == roomBedId && rb.Room.HotelId == command.HotelId);
								
								if(roomBed == null)
								{
									data.ErrorMessages.Add($"Unable to find reservation's bed with ID {roomBedId}.");
									data.HasErrors = true;
								}
								else
								{
									if (roomBed.Room == null)
									{
										data.ErrorMessages.Add($"Room bed with ID {roomBed.Id} is not assigned to a room.");
										data.HasErrors = true;
									}
									else
									{
										data.RoomBed = roomBed;
										data.Room = roomBed.Room;
										data.Reservation = reservation;
									}
								}
							}
							else if (reservation.RoomId.HasValue)
							{
								var roomId = reservation.RoomId.Value;
								var room = await this._databaseContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == command.HotelId);

								if(room == null)
								{
									data.ErrorMessages.Add($"Unable to find reservation's room with ID {roomId}.");
									data.HasErrors = true;
								}
								else
								{
									data.Room = room;
									data.Reservation = reservation;
								}
							}
							else
							{
								data.ErrorMessages.Add($"Reservation with ID {reservation.Id} is not assigned to a room or a bed.");
								data.HasErrors = true;
							}
						}
					}
				}
			}
			else
			{
				data.ErrorMessages.Add("Invalid LocationTypeKey. Accepted values: 'ROOM' or 'RESERVATION'.");
				data.HasErrors = true;
			}


			var systemTaskActions = new List<SystemTaskAction>();
			if(command.AssetActions == null || !command.AssetActions.Any())
			{
				data.ErrorMessages.Add("There must be at least one element in AssetActions array.");
				data.HasErrors = true;
			}
			else
			{

				foreach (var assetAction in command.AssetActions)
				{
					var asset = (Domain.Entities.Asset)null;
					var action = (Domain.Entities.AssetAction)null;
					var hasError = false;

					if (assetAction.AssetId.HasValue)
					{
						asset = await this._databaseContext.Assets
							.Where(aa => aa.Id == assetAction.AssetId.Value)
							.FirstOrDefaultAsync();

						if (asset == null)
						{
							data.ErrorMessages.Add($"Asset ID {assetAction.AssetId.Value} doesn't exist.");
							data.HasErrors = true;
							hasError = true;
						}
					}
					else if (assetAction.AssetName.IsNotNull())
					{
						var assetNameValue = assetAction.AssetName.Trim().ToLower();
						asset = await this._databaseContext.Assets
							.Where(aa => aa.Name.ToLower() == assetNameValue)
							.FirstOrDefaultAsync();
						
						if (asset == null)
						{
							data.ErrorMessages.Add($"Asset {assetAction.AssetName} doesn't exist.");
							data.HasErrors = true;
							hasError = true;
						}
					}
					else
					{
						data.ErrorMessages.Add($"Either AssetId or AssetName must be set for each asset action.");
						data.HasErrors = true;
						hasError = true;
					}

					if (!hasError && !asset.AssetGroupId.HasValue)
					{
						data.ErrorMessages.Add($"Asset {asset.Name} is not a part of a group.");
						data.HasErrors = true;
						hasError = true;
					}

					var assetGroup = (Domain.Entities.AssetGroup)null;
					if (!hasError)
					{
						assetGroup = await this._databaseContext.AssetGroups.Include(ag => ag.AssetActions).FirstOrDefaultAsync(ag => ag.Id == asset.AssetGroupId);

						if(assetGroup == null)
						{
							data.ErrorMessages.Add($"Asset {asset.Name} is not a part of a group.");
							data.HasErrors = true;
							hasError = true;
						}
					}

					//if (!hasError)
					//{
					//	data.ErrorMessages.Add($"Asset {asset.Name} is not a part of a group.");
					//	data.HasErrors = true;
					//	hasError = true;
					//}

					if (!hasError && (assetGroup.AssetActions == null || !assetGroup.AssetActions.Any()))
					{
						data.ErrorMessages.Add($"Asset group {assetGroup.Name} has no actions.");
						data.HasErrors = true;
						hasError = true;
					}

					if (!hasError && assetAction.ActionId.HasValue)
					{
						action = assetGroup.AssetActions.FirstOrDefault(aa => aa.Id == assetAction.ActionId.Value);

						if (action == null)
						{
							data.ErrorMessages.Add($"Asset action ID {assetAction.ActionId.Value} doesn't exist.");
							data.HasErrors = true;
							hasError = true;
						}
					}
					else if (!hasError && assetAction.ActionName.IsNotNull())
					{
						var actionNameValue = assetAction.ActionName.Trim().ToLower();

						action = assetGroup.AssetActions.FirstOrDefault(aa => aa.Name.ToLower() == actionNameValue);

						if (action == null)
						{
							data.ErrorMessages.Add($"Asset action {assetAction.ActionName} doesn't exist.");
							data.HasErrors = true;
							hasError = true;
						}
					}
					else if(!hasError)
					{
						data.ErrorMessages.Add($"Either ActionId or Action name must be set for each asset action.");
						data.HasErrors = true;
						hasError = true;
					}

					if (!hasError)
					{
						data.SystemTaskActions.Add(new SystemTaskAction
						{
							AssetGroupId = assetGroup.Id,
							ActionName = action.Name,
							AssetGroupName = assetGroup.Name,
							AssetId = asset.Id,
							AssetName = asset.Name,
							AssetQuantity = assetAction.AssetQuantity ?? 1,
							Id = Guid.NewGuid(),
							SystemTaskId = Guid.Empty,
						});
					}
				}
			}

			return data;
		}

		private class InsertExternalTaskData
		{
			public Reservation Reservation { get; set; }
			public Room Room { get; set; }
			public RoomBed RoomBed { get; set; }
			public List<SystemTaskAction> SystemTaskActions { get; set; }
			public User User { get; set; }

			public List<string> ErrorMessages { get; set; }
			public bool HasErrors { get; set; }
		}


		/// <summary>
		/// TODO: FIX THIS METHOD TO INCLUDE BEDS
		/// </summary>
		/// <param name="tasksForPlannedAttendants"></param>
		/// <returns></returns>
		private async Task _UpdateForPlannedAttendant(SystemTask t, DateTime localHotelDate)
		{
			// if it is the todays task.
			if (t.StartsAt.Date == localHotelDate.Date)
			{
				if (!t.ToRoomId.HasValue && !t.FromRoomId.HasValue)
					return;

				var roomId = t.ToRoomId.HasValue ? t.ToRoomId.Value : t.FromRoomId.Value;
				var date = localHotelDate.Date;

				// Find out if there is anyone cleaning this room today!
				var activeCleaning = await this._databaseContext.Cleanings.Where(c => c.IsActive && c.RoomId == roomId && c.StartsAt.Date == date).FirstOrDefaultAsync();

				// There are no active cleanings
				if (activeCleaning == null)
					return;

				// Find active cleaning cleaning group
				var cleaningGroup = await this._databaseContext.CleaningPlanGroups.Include(cpg => cpg.Cleaner).Where(g => g.Items.Any(gi => gi.CleaningId == activeCleaning.Id)).FirstOrDefaultAsync();

				// Cleaning group doesn't exist (for some reason)
				if (cleaningGroup == null || cleaningGroup.Cleaner == null)
					return;

				// Assign a task to the cleaner
				t.UserId = cleaningGroup.Cleaner.Id;
			}
		}


		private async Task<ProcessResponseSimple<Guid>> _Initialize(Guid? hotelGroupId, string hotelGroupKey)
		{
			var authResult = await this.AuthorizeExternalClient();
			if (!authResult.IsSuccess)
				return new ProcessResponseSimple<Guid> { IsSuccess = false, Message = authResult.Message, Data = Guid.Empty };

			var initResult = this.InitializeHotelGroupContext(hotelGroupId, hotelGroupKey);
			if (!initResult.IsSuccess)
				return new ProcessResponseSimple<Guid> { IsSuccess = false, Message = initResult.Message, Data = Guid.Empty };

			return null;
		}
	}
}

//{
//  "Whos": [
//    {
//      "TypeKey": "GROUP",
//      "ReferenceId": "d8f96a04-0c3b-4aa2-be3c-80cb7fdeafe3",
//      "ReferenceName": "Cleaners",
//      "TypeDescription": "User group"
//    }
//  ],
//  "Files": [],
//  "Price": 0,
//  "Whats": [
//    {
//      "AssetId": "440a455a-07a1-4dd5-b8c3-cc3f1ea2cff4",
//      "AssetName": "Asset-1",
//      "ActionName": "Modify",
//      "AssetGroupId": "d9a54c7b-fcb0-4976-acdb-278d9f146e13",
//      "AssetQuantity": 1,
//      "AssetGroupName": "aaa-x",
//      "IsActionSystemDefined": false,
//      "SystemDefinedActionTypeKey": "NONE",
//      "SystemDefinedActionIdentifierKey": "NONE"
//    }
//  ],
//  "Wheres": [
//    {
//      "TypeKey": "ROOM",
//      "ReferenceId": "cf9a827e-d9fd-4139-9395-734a9f5fe17e",
//      "ReferenceName": "1011",
//      "TypeDescription": "Room - Building 1, Florence"
//    },
//    {
//      "TypeKey": "ROOM",
//      "ReferenceId": "4a742ddd-8a17-4685-8298-d7304ff69df8",
//      "ReferenceName": "1002",
//      "TypeDescription": "Room - Building 1, Florence"
//    },
//    {
//      "TypeKey": "ROOM",
//      "ReferenceId": "b3f21005-61c7-43e8-b316-7924fb2d3901",
//      "ReferenceName": "1012",
//      "TypeDescription": "Room - Building 1, Florence"
//    }
//  ],
//  "Comment": "",
//  "Credits": 0,
//  "WhereTo": null,
//  "EventKey": null,
//  "WhereFrom": null,
//  "EndsAtTime": null,
//  "PriorityKey": "NORMAL",
//  "TaskTypeKey": "SINGLE",
//  "EventTimeKey": null,
//  "WhatsTypeKey": "LIST",
//  "RepeatsForKey": null,
//  "StartsAtTimes": [
//    "2021-12-15T08:31:00"
//  ],
//  "IsGuestRequest": false,
//  "ExcludeHolidays": null,
//  "ExcludeWeekends": null,
//  "EventModifierKey": null,
//  "RepeatsForNrDays": null,
//  "RepeatsUntilTime": null,
//  "IsShownInNewsFeed": false,
//  "RecurringTaskTypeKey": "DAILY",
//  "RepeatsForNrOccurences": null,
//  "MustBeFinishedByAllWhos": false,
//  "RecurringTaskRepeatTimes": null,
//  "PostponeWhenRoomIsOccupied": null,
//  "RecurringEveryNumberOfDays": 0,
//  "IsBlockingCleaningUntilFinished": false,
//  "IsRescheduledEveryDayUntilFinished": false,
//  "IsMajorNotificationRaisedWhenFinished": false
//}



