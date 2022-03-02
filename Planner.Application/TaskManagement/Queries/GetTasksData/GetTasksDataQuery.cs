using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetTasksData
{
	public static class TaskDescriptions
	{
		private static Dictionary<string, string> EventTimeTypeDescriptions { get; set; } = new Dictionary<string, string>
		{
			{ TaskEventTimeType.EVERY_TIME.ToString(), "Every time" },
			{ TaskEventTimeType.ON_DATE.ToString(), "On date" },
			{ TaskEventTimeType.ON_NEXT.ToString(), "On next" },
			{ "", "" },
		};
		public static readonly Dictionary<string, string> EventTypeDescriptions = new Dictionary<string, string>
		{
			{ EventTaskType.CHECK_IN.ToString(), "Check in" },
			{ EventTaskType.CHECK_OUT.ToString(), "Check out" },
			{ EventTaskType.CLEANING.ToString(), "Cleaning" },
			{ EventTaskType.CLEANING_REFUSED.ToString(), "Cleaning refused" },
			{ EventTaskType.DO_NOT_DISTURB.ToString(), "Do not disturb" },
			{ EventTaskType.HOSTING.ToString(), "Hosting" },
			{ EventTaskType.LATER.ToString(), "Later" },
			{ EventTaskType.STAY.ToString(), "Stay" },
			{ "", "" },
		};
		public static readonly Dictionary<string, string> EventModifierDescriptions = new Dictionary<string, string>
		{
			{ EventTaskModifierType.BEFORE.ToString(), "Before" },
			{ EventTaskModifierType.ON.ToString(), "During" },
			{ EventTaskModifierType.AFTER.ToString(), "After" },
			{ EventTaskModifierType.DURING.ToString(), "During" },
			{ "", "" },
		};

		public static string GetEventTypeDescription(string key)
		{
			if (EventTypeDescriptions.ContainsKey(key))
			{
				return EventTypeDescriptions[key];
			}

			return "UNKNOWN_EVENT_TYPE";
		}
		public static string GetEventModifierDescription(string key)
		{
			if (EventModifierDescriptions.ContainsKey(key))
			{
				return EventModifierDescriptions[key];
			}

			return "UNKNOWN_EVENT_MODIFIER";
		}

		public static string GetTaskStatusDescription(string statusKey)
		{
			switch (statusKey)
			{
				case nameof(TaskStatusType.PENDING):
					return "Pending";
				case nameof(TaskStatusType.WAITING):
					return "Waiting";
				case nameof(TaskStatusType.PAUSED):
					return "Paused";
				case nameof(TaskStatusType.FINISHED):
					return "Finished";
				case nameof(TaskStatusType.STARTED):
					return "Started";
				case nameof(TaskStatusType.CANCELLED):
					return "Cancelled";
				case nameof(TaskStatusType.VERIFIED):
					return "Verified";
				case nameof(TaskStatusType.CLAIMED):
					return "Claimed";
				case nameof(TaskStatusType.CLAIMED_BY_SOMEONE_ELSE):
					return "Claimed by someone else";
				case nameof(TaskStatusType.REJECTED):
					return "Rejected";
				default:
					return "Unknown status";
			}
		}

		public static string GetTaskTypeDescription(string typeKey, string recurringTypeKey, string eventModifierKey, string eventKey, bool showSinlgeDescription = false)
		{
			switch (typeKey)
			{
				case nameof(TaskType.SINGLE):
					return showSinlgeDescription ? "Single task" : "";
				case nameof(TaskType.RECURRING):
					switch (recurringTypeKey)
					{
						case nameof(RecurringTaskType.DAILY):
							return "Recurring daily";
						case nameof(RecurringTaskType.MONTHLY):
							return "Recurring monthly";
						case nameof(RecurringTaskType.SPECIFIC_TIME):
							return "Recurring at specific times";
						case nameof(RecurringTaskType.WEEKLY):
							return "Recurring weekly";
						default:
							return "";
					}
				case nameof(TaskType.EVENT):
					return $"{EventModifierDescriptions[eventModifierKey]} {EventTypeDescriptions[eventKey]}";
				default:
					return "";
			}
		}



		public static WhereDescription GetWhere(string roomName, string floorName, string buildingName, string hotelName)
		{
			if (roomName.IsNotNull())
			{
				return new WhereDescription
				{
					Where = $"Room {roomName}",
					Description = $"{hotelName} | {buildingName} | {floorName}"
				};
			}
			else if (floorName.IsNotNull())
			{
				return new WhereDescription
				{
					Where = $"Floor {floorName}",
					Description = $"{hotelName} | {buildingName}"
				};
			}
			else if (buildingName.IsNotNull())
			{
				return new WhereDescription
				{
					Where = $"Building {buildingName}",
					Description = $"{hotelName}"
				};
			}
			else
			{
				return new WhereDescription
				{
					Where = $"Hotel {hotelName}",
					Description = ""
				};
			}
		}

		public static WhereDescription GetWhere2(string hotelId, string hotelName, Guid? warehouseId, string reservationId, Guid? roomId, string name, string additionalDescription = "Reservation")
		{
			if (reservationId.IsNotNull())
			{
				return new WhereDescription
				{
					WhereId = reservationId,
					Where = $"{name}",
					WhereTypeKey = "RESERVATION",
					Description = $"{additionalDescription} at {hotelName}"
				};
			}
			else if (warehouseId.HasValue)
			{
				return new WhereDescription
				{
					WhereId = warehouseId.Value.ToString(),
					Where = $"{name}",
					WhereTypeKey = "WAREHOUSE",
					Description = $"Warehouse at {hotelName}"
				};
			}
			else if (roomId.HasValue)
			{
				return new WhereDescription
				{
					WhereId = roomId.Value.ToString(),
					Where = $"{name}",
					WhereTypeKey = "ROOM",
					Description = $"Room at {hotelName}"
				};
			}
			else
			{
				return new WhereDescription
				{
					WhereId = hotelId,
					Where = $"{hotelName}",
					WhereTypeKey = "HOTEL",
					Description = "Hotel"
				};
			}
		}

		public static string GetWhen(DateTime startsAt, string taskType, string taskTimeType, string eventModifierKey, string eventKey, DateTime currentHotelTime)
		{
			if(taskType == TaskType.EVENT.ToString() && taskTimeType == TaskEventTimeType.ON_NEXT.ToString())
			{
				return $"On next {EventModifierDescriptions[eventModifierKey]} {EventTypeDescriptions[eventKey]}";
			}

			var activeTo = currentHotelTime;
			var activeFrom = currentHotelTime.AddHours(-1);
			if (startsAt < activeFrom)
			{
				return "Passed";
			}
			else if (startsAt >= activeFrom && startsAt <= activeTo)
			{
				return "Now";
			}
			else
			{
				var difference = startsAt.Subtract(currentHotelTime);
				if (difference.TotalHours < 1d)
				{
					return $"In {(int)difference.TotalMinutes} min";
				}
				else if (difference.TotalHours < 24d)
				{
					return $"In {(int)difference.TotalHours} hr";
				}
				else if (difference.TotalDays < 366)
				{
					return $"In {(int)difference.TotalDays} days";
				}
				else
				{
					return $"In {(int)(((int)difference.TotalDays) / 365d)} years";
				}
			}

		}

		public static string GetWhenDescription(DateTime startsAt, string taskType, string taskTimeType, DateTime currentHotelTime)
		{
			if (taskType == TaskType.EVENT.ToString() && taskTimeType == TaskEventTimeType.ON_NEXT.ToString())
			{
				return $"";
			}

			var now = currentHotelTime;
			var tomorrow = now.AddDays(1);

			var startsAtShortDate = startsAt.ToShortDateString();
			if (startsAtShortDate == now.ToShortDateString())
			{
				return "Today at " + startsAt.TimeOfDay.ToString(@"hh\:mm");
			}
			else if (startsAtShortDate == tomorrow.ToShortDateString())
			{
				return "Tomorrow at " + startsAt.TimeOfDay.ToString(@"hh\:mm");
			}
			else
			{
				return startsAtShortDate + " " + startsAt.TimeOfDay.ToString(@"hh\:mm");
			}

		}
	}

	public class WhereDescription
	{
		public string WhereId { get; set; }
		public string Where { get; set; }
		/// <summary>
		/// RESERVATION, ROOM, WAREHOUSE, HOTEL
		/// </summary>
		public string WhereTypeKey { get; set; }
		public string Description { get; set; }
	}
	

	public class EnumData
	{
		public string Key { get; set; }
		public string Name { get; set; }
	}

	public class TasksData
	{
		public IEnumerable<ExtendedTaskActionData> AllTaskActions { get; set; }
		public IEnumerable<TaskWhoData> AllWhos { get; set; }
		public IEnumerable<TaskWhereData> AllWheres { get; set; }

		public IEnumerable<EnumData> AllTaskTypes { get; set; }
		public IEnumerable<EnumData> AllRecurringTaskTypes { get; set; }

		public IEnumerable<EnumData> AllEventTaskTypes { get; set; }
		public IEnumerable<EnumData> AllEventTaskModifierTypes { get; set; }

		/// <summary>
		/// Used for generating tab headers on the tasks management page.
		/// </summary>
		public IEnumerable<AvailableUserGroup> AvailableUserGroups { get; set; }
		/// <summary>
		/// Used for generating filter options.
		/// </summary>
		public IEnumerable<AvailableUserGroup> AvailableUserSubGroups { get; set; }
	}
	public class TaskActionData
	{
		public int AvailableQuantity { get; set; }
		public string ActionName { get; set; }
		public string AssetName { get; set; }
		public string AssetGroupName { get; set; }
		public Guid AssetGroupId { get; set; }
		public Guid AssetId { get; set; }

		public bool IsActionSystemDefined { get; set; }

		/// <summary>
		/// Described by enum: SystemActionType
		///   LOCATION_CHANGE
		///   NONE
		/// </summary>
		public string SystemDefinedActionTypeKey { get; set; }

		/// <summary>
		/// Described by enum: SystemDefinedActionIdentifier
		///   WAREHOUSE_TO_WAREHOUSE,
		///   ROOM_TO_WAREHOUSE,
		///   WAREHOUSE_TO_ROOM,
		///   ROOM_TO_ROOM,
		///   NONE,
		/// </summary>
		public string SystemDefinedActionIdentifierKey { get; set; }

		public Guid? DefaultAssingedUserId { get; set; }
		public Guid? DefaultAssingedUserGroupId { get; set; }
		public Guid? DefaultAssingedUserSubGroupId { get; set; }
	}

	public class ExtendedTaskActionData : TaskActionData
	{
		public string AltAssetName { get; set; }

		public int? DefaultCredits { get; set; }
		public decimal? DefaultPrice { get; set; }
		public string DefaultPriorityKey { get; set; }

		public string Description { get; set; }

		//public List<TaskActionAssetAvailability> Availability { get; set; }
	}

	public class TaskActionAssetAvailability
	{
		public Guid WarehouseId { get; set; }
		public string WarehouseName { get; set; }
		public int Available { get; set; }
		public int Reserved { get; set; }
	}

	public class TaskWhoData
	{
		public string TypeKey { get; set; }
		public string TypeDescription { get; set; }
		public string ReferenceId { get; set; }
		public string ReferenceName { get; set; }
		public string ImageUrl { get; set; }
	}

	public class TaskWhereData
	{
		public string TypeKey { get; set; }
		public string TypeDescription { get; set; }
		public string ReferenceId { get; set; }
		public string ReferenceName { get; set; }
	}

	public class AvailableUserGroup
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsSubGroup { get; set; }
	}

	public class GetTasksDataQuery : IRequest<TasksData>
	{
	}

	public class GetTasksDataQueryHandler : IRequestHandler<GetTasksDataQuery, TasksData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly RoleManager<Role> _roleManager;
		private readonly UserManager<User> _userManager;
		private readonly Guid _userId;

		public GetTasksDataQueryHandler(IDatabaseContext databaseContext, RoleManager<Role> roleManager, UserManager<User> userManager, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userManager = userManager;
			this._roleManager = roleManager;
			this._userId = contextAccessor.UserId();
		}

		public async Task<TasksData> Handle(GetTasksDataQuery request, CancellationToken cancellationToken)
		{
			var assets = await this._databaseContext
				.Assets
				.Select(a => new 
				{ 
					a.Id,
					a.Name,
					a.IsBulk,
					a.AssetGroupId,
					a.SerialNumber,
					AssetTags = a.AssetTags.ToArray(),
				})
				.OrderBy(a => a.Name)
				.ToArrayAsync();

			var assetGroupsMap = (await this._databaseContext.AssetGroups
				.Select(ag => new
				{
					ag.Id,
					ag.Name,
					AssetActions = ag.AssetActions.Select(aa => new
					{
						aa.Id,
						aa.Name,
						aa.Credits,
						aa.DefaultAssignedToUserId,
						aa.DefaultAssignedToUserGroupId,
						aa.IsSystemDefined,
						aa.DefaultAssignedToUserSubGroupId,
						aa.Price,
						aa.PriorityKey,
						aa.QuickOrTimedKey,
						aa.SystemDefinedActionIdentifierKey,
						aa.SystemActionTypeKey,
					})
					.OrderBy(aa => aa.Name)
					.ToArray()
				})
				.OrderBy(ag => ag.Name)
				.ToArrayAsync())
				.ToDictionary(ag => ag.Id, ag => ag);

			var warehousesMap = (await this._databaseContext.Warehouses
				.Select(w => new
				{
					w.Id,
					w.Name,
				})
				.ToArrayAsync())
				.ToDictionary(w => w.Id, w => w);

			var taskActions = new List<ExtendedTaskActionData>();

			foreach(var asset in assets)
			{
				if (!asset.AssetGroupId.HasValue)
					continue;

				var assetGroup = assetGroupsMap[asset.AssetGroupId.Value];

				foreach(var action in assetGroup.AssetActions)
				{
					taskActions.Add(new ExtendedTaskActionData
					{
						AvailableQuantity = 0,
						ActionName = action.Name,
						AssetGroupId = assetGroup.Id,
						AssetGroupName = assetGroup.Name,
						AssetId = asset.Id,
						AssetName = asset.Name,
						DefaultCredits = action.Credits,
						DefaultPrice = action.Price,
						DefaultPriorityKey = action.PriorityKey,
						IsActionSystemDefined = action.IsSystemDefined,
						SystemDefinedActionIdentifierKey = action.SystemDefinedActionIdentifierKey,
						SystemDefinedActionTypeKey = action.SystemActionTypeKey,
						DefaultAssingedUserId = action.DefaultAssignedToUserId,
						DefaultAssingedUserGroupId = action.DefaultAssignedToUserGroupId,
						DefaultAssingedUserSubGroupId = action.DefaultAssignedToUserSubGroupId,
						Description = action.SystemActionTypeKey == SystemActionType.LOCATION_CHANGE.ToString() ? "Asset location change" : "",
						AltAssetName = asset.AssetTags == null ? "" : string.Join(", ", asset.AssetTags.Select(at => at.TagKey.ToLower()).ToArray()),
					});
				}
			}

			var users = await this._userManager.Users.Include(u => u.UserGroup).Include(u => u.UserSubGroup).ToListAsync();
			var userGroups = await this._databaseContext.UserGroups.Include(ug => ug.UserSubGroups).ToListAsync();

			var whos = new List<TaskWhoData>();

			whos.Add(new TaskWhoData
			{
				ImageUrl = null,
				ReferenceId = Guid.Empty.ToString(),
				ReferenceName = $"Planned attendant",
				TypeDescription = "Anyone who will do the cleaning",
				TypeKey = TaskWhoType.PLANNED_ATTENDANT.ToString()
			});

			var userGroupId = (Guid?)null;
			var userSubGroupId = (Guid?)null;
			var availableUserGroups = new List<AvailableUserGroup>();
			var availableUserSubGroups = new List<AvailableUserGroup>();

			foreach (var user in users)
			{
				if(user.Id == this._userId)
				{
					userGroupId = user.UserGroupId;
					userSubGroupId = user.UserSubGroupId;
				}
					
				var typeDescription = "User";
				if (user.UserSubGroup != null)
				{
					typeDescription += $", {user.UserSubGroup.Name}";
				}
				if (user.UserGroup != null)
				{
					typeDescription += $", {user.UserGroup.Name}";
				}

				whos.Add(new TaskWhoData
				{
					ImageUrl = null, // TODO: IMPLEMENT USER AVATAR IMAGES
					ReferenceId = user.Id.ToString(),
					ReferenceName = $"{user.FirstName} {user.LastName}",
					TypeDescription = typeDescription,
					TypeKey = TaskWhoType.USER.ToString()
				});
			}

			foreach (var userGroup in userGroups)
			{
				if(userGroupId.HasValue && userGroup.Id == userGroupId.Value)
				{
                    if (userSubGroupId.HasValue)
                    {
                        var subGroup = userGroup.UserSubGroups.FirstOrDefault(sg => sg.Id == userSubGroupId.Value);
                        if (subGroup != null)
                        {
                            availableUserGroups.Add(new AvailableUserGroup { Id = subGroup.Id, IsSubGroup = true, Name = subGroup.Name + " tasks"});
                        }
					}
					else
                    {
						// current user's group
						availableUserGroups.Add(new AvailableUserGroup { Id = userGroup.Id, IsSubGroup = false, Name = userGroup.Name + " tasks" });
					}

                    if (!userSubGroupId.HasValue)
                    {
                        // then the user can see all subgroups!
                        availableUserSubGroups.AddRange(userGroup.UserSubGroups.Select(sg => new AvailableUserGroup { Id = sg.Id, IsSubGroup = true, Name = sg.Name }));
                    }
                }

				whos.Add(new TaskWhoData
				{
					ImageUrl = null, // TODO: IMPLEMENT USER AVATAR IMAGES
					ReferenceId = userGroup.Id.ToString(),
					ReferenceName = $"{userGroup.Name}",
					TypeDescription = "User group",
					TypeKey = TaskWhoType.GROUP.ToString()
				});
				foreach (var subGroup in userGroup.UserSubGroups)
				{
					whos.Add(new TaskWhoData
					{
						ImageUrl = null, // TODO: IMPLEMENT USER AVATAR IMAGES
						ReferenceId = subGroup.Id.ToString(),
						ReferenceName = $"{subGroup.Name}",
						TypeDescription = $"Subgroup of {userGroup.Name}",
						TypeKey = TaskWhoType.SUBGROUP.ToString()
					});
				}
			}

			var rooms = await this._databaseContext.Rooms.OrderBy(r => r.Name).ToListAsync();
			var buildingsMap = (await this._databaseContext.Buildings.Include(b => b.Floors).ToListAsync()).ToDictionary(b => b.Id, b => b);
			var hotelsMap = (await this._databaseContext.Hotels.ToListAsync()).ToDictionary(h => h.Id, h => h);
			var reservations = await this._databaseContext.Reservations.Where(r => r.IsActive && r.RoomId != null).Select(r => new 
			{  
				ReservationId = r.Id,
				GuestName = r.GuestName, 
				StatusKey = r.RccReservationStatusKey, 
				RoomName = r.Room.Name, 
				HotelId = r.HotelId,
				CheckIn = r.CheckIn,
				CheckOut = r.CheckOut,
			}).ToArrayAsync();
			var warehouses = await this._databaseContext.Warehouses.ToListAsync();
			//var warehousesMap = new Dictionary<Guid, List<Domain.Entities.Warehouse>>();
			//var centralWarehouses = new List<Domain.Entities.Warehouse>();
			//foreach(var warehouse in warehouses)
			//{
			//	if (warehouse.IsCentral)
			//	{
			//		centralWarehouses.Add(warehouse);
			//	}
			//	else
			//	{

			//	}
			//}

			var wheres = new List<TaskWhereData>();

			foreach (var room in rooms)
			{
				var hotelName = hotelsMap.ContainsKey(room.HotelId) ? hotelsMap[room.HotelId].Name : "Unknown hotel";
				var buildingName = room.BuildingId.HasValue && buildingsMap.ContainsKey(room.BuildingId.Value) ? buildingsMap[room.BuildingId.Value].Name : "Unknown building";

				wheres.Add(new TaskWhereData
				{
					ReferenceId = room.Id.ToString(),
					ReferenceName = $"{room.Name}",
					TypeDescription = $"Room - {buildingName}, {hotelName}",
					TypeKey = TaskWhereType.ROOM.ToString()
				});
			}

			foreach (var warehouse in warehouses)
			{
				wheres.Add(new TaskWhereData
				{
					ReferenceId = warehouse.Id.ToString(),
					ReferenceName = $"{warehouse.Name}",
					TypeDescription = $"Warehouse",
					TypeKey = TaskWhereType.WAREHOUSE.ToString(),
				});
			}

			var addedHotelsIdSet = new HashSet<string>();
			foreach (var building in buildingsMap.Values)
			{
				var hotelName = hotelsMap.ContainsKey(building.HotelId) ? hotelsMap[building.HotelId].Name : "Unknown hotel";

				if (!addedHotelsIdSet.Contains(building.HotelId))
				{
					wheres.Add(new TaskWhereData
					{
						ReferenceId = building.HotelId,
						ReferenceName = hotelName,
						TypeDescription = $"Hotel",
						TypeKey = TaskWhereType.HOTEL.ToString()
					});
				}

				wheres.Add(new TaskWhereData
				{
					ReferenceId = building.Id.ToString(),
					ReferenceName = building.Name,
					TypeDescription = $"Building - {hotelName}",
					TypeKey = TaskWhereType.BUILDING.ToString()
				});

				foreach (var floor in building.Floors)
				{
					wheres.Add(new TaskWhereData
					{
						ReferenceId = floor.Id.ToString(),
						ReferenceName = $"{floor.Name} {floor.Number}",
						TypeDescription = $"Floor - {building.Name}, {hotelName}",
						TypeKey = TaskWhereType.FLOOR.ToString()
					});
				}
			}

			foreach (var reservation in reservations)
			{
				var hotelName = hotelsMap.ContainsKey(reservation.HotelId) ? hotelsMap[reservation.HotelId].Name : "Unknown hotel";
				wheres.Add(new TaskWhereData
				{
					ReferenceId = reservation.ReservationId,
					ReferenceName = $"{reservation.GuestName}{(reservation.RoomName.IsNull() ? "" : $" [{reservation.RoomName}]")}",
					TypeDescription = $"{(reservation.CheckIn.HasValue ? reservation.CheckIn.Value.ToString("dddd dd MMM") : "?")} - {(reservation.CheckOut.HasValue ? reservation.CheckOut.Value.ToString("dddd dd MMM") : "?")} at {hotelName}",
					TypeKey = TaskWhereType.RESERVATION.ToString(),
				});
			}

			return new TasksData
			{
				AllTaskActions = taskActions,
				AllWhos = whos,
				AllWheres = wheres,
				AvailableUserGroups = availableUserGroups,
				AvailableUserSubGroups = availableUserSubGroups,
				AllTaskTypes = new EnumData[] {
					new EnumData { Key = TaskType.SINGLE.ToString(), Name = "Single task" },
					new EnumData { Key = TaskType.RECURRING.ToString(), Name = "Recurring task" },
					new EnumData { Key = TaskType.EVENT.ToString(), Name = "On event task" },
					new EnumData { Key = TaskType.BALANCED.ToString(), Name = "Balanced" },
				},
				AllRecurringTaskTypes = new EnumData[] {
					new EnumData { Key = RecurringTaskType.DAILY.ToString(), Name = "Daily" },
					new EnumData { Key = RecurringTaskType.WEEKLY.ToString(), Name = "Weekly" },
					new EnumData { Key = RecurringTaskType.MONTHLY.ToString(), Name = "Monthly" },
					new EnumData { Key = RecurringTaskType.SPECIFIC_TIME.ToString(), Name = "Specific times" },
					new EnumData { Key = RecurringTaskType.EVERY.ToString(), Name = "Every" },
				},
				AllEventTaskModifierTypes = new EnumData[]
				{
					new EnumData { Key = EventTaskModifierType.BEFORE.ToString(), Name = "Before" },
					new EnumData { Key = EventTaskModifierType.DURING.ToString(), Name = "During" },
					new EnumData { Key = EventTaskModifierType.AFTER.ToString(), Name = "After" },
				},
				AllEventTaskTypes = new EnumData[]
				{
					new EnumData { Key = EventTaskType.CHECK_IN.ToString(), Name = "Check in" },
					new EnumData { Key = EventTaskType.CHECK_OUT.ToString(), Name = "Check out" },
					new EnumData { Key = EventTaskType.CLEANING.ToString(), Name = "Cleaning" },
					new EnumData { Key = EventTaskType.HOSTING.ToString(), Name = "Hosting" },
					new EnumData { Key = EventTaskType.STAY.ToString(), Name = "Stay" },
					new EnumData { Key = EventTaskType.DO_NOT_DISTURB.ToString(), Name = "Do not disturb" },
					new EnumData { Key = EventTaskType.CLEANING_REFUSED.ToString(), Name = "Cleaning refused" },
					new EnumData { Key = EventTaskType.LATER.ToString(), Name = "Later" },
				}
			};
		}
	}
}
