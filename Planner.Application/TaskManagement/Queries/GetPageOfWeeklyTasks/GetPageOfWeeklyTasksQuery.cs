using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetPageOfWeeklyTasks
{
	public class WeeklyTasksViewModel
	{
		public WeeklyUserTaskGroupViewModel[] UserGroups { get; set; }
		public int TotalNumberOfUserGroups { get; set; }
		public WeekDayViewModel[] WeekDays { get; internal set; }
	}

	public class WeeklyUserTaskGroupViewModel
	{
		public Guid UserId { get; set; }
		public string UserFullName { get; set; }
		public string UserAvatarUrl { get; set; }
		public bool HasAvatarImage { get; set; }
		public string DefaultUserAvatarColorHex { get; set; }
		public string UserFullNameInitials { get; set; }
		public string UserGroupName { get; set; }
		public string UserSubGroupName { get; set; }
		public List<WeeklyUserDayTaskGroupViewModel> DayGroups { get; set; }
	}

	public class WeeklyUserDayTaskGroupViewModel
	{
		public string DateString { get; set; }
		public string DateKey { get; set; }
		public string DayName { get; set; }
		public List<WeeklyTaskViewModel> Tasks { get; set; }
		public bool ShowMoreTasks { get; set; }
		public int NumberOfHiddenTasks { get; set; }
	}

	public class WeekDayViewModel
	{
		public string DateString { get; set; }
		public string DateKey { get; set; }
		public string DayName { get; set; }
	}

	public class WeeklyTaskActionViewModel
	{
		public string ActionName { get; set; }
		public string AssetName { get; set; }
		public int AssetQuantity { get; set; }
	}

	public class WeeklyTaskViewModel
	{
		public Guid Id { get; set; }
		public IEnumerable<WeeklyTaskActionViewModel> Actions { get; set; }
		public string StatusKey { get; set; }
		public string StatusDescription { get; set; }
		public string WhereDescription { get; set; }
		public bool IsGuestRequest { get; set; }
		public bool IsBelowTreshold { get; set; }
	}

	public class GetPageOfWeeklyTasksQuery : IRequest<WeeklyTasksViewModel>
	{
		public string Keywords { get; set; }
		public string FromDateString { get; set; }
		public string ToDateString { get; set; }
		public string SortKey { get; set; }
		public string StatusKey { get; set; }
		public string ActionName { get; set; }
		public Guid? AssetId { get; set; }
		public Guid? AssetGroupId { get; set; }
		public TaskWhoData[] Whos { get; set; }
		public TaskWhereData[] Wheres { get; set; }
		public bool OnlyMyTasks { get; set; }

		public int Skip { get; set; }
		public int? Take { get; set; }
	}

	public class GetPageOfWeeklyTasksQueryHandler : IRequestHandler<GetPageOfWeeklyTasksQuery, WeeklyTasksViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfWeeklyTasksQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<WeeklyTasksViewModel> Handle(GetPageOfWeeklyTasksQuery request, CancellationToken cancellationToken)
		{
			var usersQuery = this._databaseContext.Users
				.Include(u => u.Avatar)
				.Include(u => u.UserGroup)
				.Include(u => u.UserSubGroup)
				.AsQueryable();

			if (request.Whos != null && request.Whos.Any())
			{
				var userIds = new List<Guid>();
				//var roleIds = new List<Guid>();
				var userGroupIds = new List<Guid>();
				var userSubGroupIds = new List<Guid>();
				foreach (var who in request.Whos)
				{
					switch (who.TypeKey)
					{
						case nameof(TaskWhoType.GROUP):
							userGroupIds.Add(new Guid(who.ReferenceId));
							break;
						//case nameof(TaskWhoType.ROLE):
						//	roleIds.Add(new Guid(who.ReferenceId));
						//	break;
						case nameof(TaskWhoType.SUBGROUP):
							userSubGroupIds.Add(new Guid(who.ReferenceId));
							break;
						case nameof(TaskWhoType.USER):
							userIds.Add(new Guid(who.ReferenceId));
							break;
					}
				}

				if (userIds.Any())
				{
					usersQuery = usersQuery.Where(t => userIds.Contains(t.Id));
				}

				//if (roleIds.Any())
				//{
				//	usersQuery = usersQuery.Where(t => t.UserRoles.Any(ur => roleIds.Contains(ur.RoleId)));
				//}

				if (userGroupIds.Any())
				{
					usersQuery = usersQuery.Where(t => userGroupIds.Contains(t.UserSubGroup.UserGroupId));
				}

				if (userSubGroupIds.Any())
				{
					usersQuery = usersQuery.Where(t => userSubGroupIds.Contains(t.UserSubGroupId.Value));
				}
			}

			var usersCount = await usersQuery.CountAsync();

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					case "USER_ASC":
						usersQuery = usersQuery.OrderBy(q => q.FirstName).OrderBy(q => q.LastName);
						break;
					case "USER_DESC":
						usersQuery = usersQuery.OrderByDescending(q => q.FirstName).OrderByDescending(q => q.LastName);
						break;
					default:
						break;
				}
			}

			if (request.Skip > 0)
			{
				usersQuery = usersQuery.Skip(request.Skip);
			}
			
			if (request.Take.HasValue && request.Take.Value > 0)
			{
				usersQuery = usersQuery.Take(request.Take.Value);
			}

			var users = await usersQuery.ToArrayAsync();
			var visibleUserIds = users.Select(u => u.Id).ToArray();
			var weekStartDate = DateTimeHelper.ParseIsoDate(request.FromDateString).Date;
			var weekEndDate = weekStartDate.AddDays(8); // includes the hours of the 7th day

			var weekDays = new WeekDayViewModel[]
			{
				new WeekDayViewModel { DateKey = "OTHER", DateString = "", DayName = "Event & Other" },
				new WeekDayViewModel { DateKey = "MONDAY", DateString = (weekStartDate).ToString("d"), DayName = "Monday" },
				new WeekDayViewModel { DateKey = "TUESDAY", DateString = (weekStartDate.AddDays(1)).ToString("d"), DayName = "Tuesday" },
				new WeekDayViewModel { DateKey = "WEDNESDAY", DateString = (weekStartDate.AddDays(2)).ToString("d"), DayName = "Wednesday" },
				new WeekDayViewModel { DateKey = "THURSDAY", DateString = (weekStartDate.AddDays(3)).ToString("d"), DayName = "Thursday" },
				new WeekDayViewModel { DateKey = "FRIDAY", DateString = (weekStartDate.AddDays(4)).ToString("d"), DayName = "Friday" },
				new WeekDayViewModel { DateKey = "SATURDAY", DateString = (weekStartDate.AddDays(5)).ToString("d"), DayName = "Saturday" },
				new WeekDayViewModel { DateKey = "SUNDAY", DateString = (weekStartDate.AddDays(6)).ToString("d"), DayName = "Sunday" },
			};

			var data = users.Select(u => new WeeklyUserTaskGroupViewModel
			{
				UserGroupName = u.UserGroup?.Name,
				UserSubGroupName = u.UserSubGroup?.Name,
				UserFullName = $"{u.FirstName} {u.LastName}",
				UserId = u.Id,
				UserFullNameInitials = $"{(u.FirstName.IsNotNull() ? u.FirstName[0].ToString() : "")}{(u.LastName.IsNotNull() ? u.LastName[0].ToString() : "")}",
				UserAvatarUrl = u.Avatar?.FileUrl,
				DefaultUserAvatarColorHex = u.DefaultAvatarColorHex,
				HasAvatarImage = false,
				DayGroups = new List<WeeklyUserDayTaskGroupViewModel>()
				{
					new WeeklyUserDayTaskGroupViewModel
					{
						DateKey = "OTHER",
						DateString = "OTHER",
						DayName = "Event & Other",
						NumberOfHiddenTasks = 0,
						ShowMoreTasks = false,
						Tasks = new List<WeeklyTaskViewModel>()
					},
					new WeeklyUserDayTaskGroupViewModel
					{
						DateKey = "MONDAY",
						DateString = (weekStartDate).ToString("d"),
						DayName = "Monday",
						NumberOfHiddenTasks = 0,
						ShowMoreTasks = false,
						Tasks = new List<WeeklyTaskViewModel>()
					},
					new WeeklyUserDayTaskGroupViewModel
					{
						DateKey = "TUESDAY",
						DateString = (weekStartDate.AddDays(1)).ToString("d"),
						DayName = "Tuesday",
						NumberOfHiddenTasks = 0,
						ShowMoreTasks = false,
						Tasks = new List<WeeklyTaskViewModel>()
					},
					new WeeklyUserDayTaskGroupViewModel
					{
						DateKey = "WEDNESDAY",
						DateString = (weekStartDate.AddDays(2)).ToString("d"),
						DayName = "Wednesday",
						NumberOfHiddenTasks = 0,
						ShowMoreTasks = false,
						Tasks = new List<WeeklyTaskViewModel>()
					},
					new WeeklyUserDayTaskGroupViewModel
					{
						DateKey = "THURSDAY",
						DateString = (weekStartDate.AddDays(3)).ToString("d"),
						DayName = "Thursday",
						NumberOfHiddenTasks = 0,
						ShowMoreTasks = false,
						Tasks = new List<WeeklyTaskViewModel>()
					},
					new WeeklyUserDayTaskGroupViewModel
					{
						DateKey = "FRIDAY",
						DateString = (weekStartDate.AddDays(4)).ToString("d"),
						DayName = "Friday",
						NumberOfHiddenTasks = 0,
						ShowMoreTasks = false,
						Tasks = new List<WeeklyTaskViewModel>()
					},
					new WeeklyUserDayTaskGroupViewModel
					{
						DateKey = "SATURDAY",
						DateString = (weekStartDate.AddDays(5)).ToString("d"),
						DayName = "Saturday",
						NumberOfHiddenTasks = 0,
						ShowMoreTasks = false,
						Tasks = new List<WeeklyTaskViewModel>()
					},
					new WeeklyUserDayTaskGroupViewModel
					{
						DateKey = "SUNDAY",
						DateString = (weekStartDate.AddDays(6)).ToString("d"),
						DayName = "Sunday",
						NumberOfHiddenTasks = 0,
						ShowMoreTasks = false,
						Tasks = new List<WeeklyTaskViewModel>()
					},
				}
			}).ToArray();


			var tasksQuery = this._databaseContext.SystemTasks
				.Where(t => ((t.StartsAt >= weekStartDate && t.StartsAt < weekEndDate) || t.TypeKey == "EVENT") && t.UserId != null && visibleUserIds.Contains(t.UserId.Value))
				.AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				// TODO: FIX KEYWORDS SEARCH
				//tasksQuery = tasksQuery.Where(t => t.ActionName.ToLower().Contains(keywordsValue) || t.AssetName.ToLower().Contains(keywordsValue));
				tasksQuery = tasksQuery.Where(t => t.Actions.Any(a => a.ActionName.ToLower().Contains(keywordsValue) && a.AssetName.ToLower().Contains(keywordsValue)));
			}

			if (request.StatusKey.IsNotNull() && request.StatusKey != "ANY")
			{
				tasksQuery = tasksQuery.Where(t => t.StatusKey == request.StatusKey);
			}

			if (request.AssetId.HasValue && request.AssetGroupId.HasValue)
			{
				tasksQuery = tasksQuery.Where(t => t.Actions.Any(a => a.AssetId == request.AssetId.Value && a.ActionName == request.ActionName && a.AssetGroupId == request.AssetGroupId.Value));
			}
			else if (request.AssetGroupId.HasValue && !request.AssetId.HasValue)
			{
				tasksQuery = tasksQuery.Where(t => t.Actions.Any(a => a.AssetGroupId == request.AssetGroupId.Value && a.ActionName == request.ActionName));
			}
			else if (request.AssetId.HasValue && !request.AssetGroupId.HasValue)
			{
				tasksQuery = tasksQuery.Where(t => t.Actions.Any(a => a.AssetId == request.AssetId.Value && a.ActionName == request.ActionName));
			}

			if (request.Wheres != null && request.Wheres.Any())
			{
				var roomIds = new List<Guid>();
				var floorIds = new List<Guid>();
				var buildingIds = new List<Guid>();
				var warehouseIds = new List<Guid>();
				var hotelIds = new List<string>();
				var reservationIds = new List<string>();
				foreach (var where in request.Wheres)
				{
					switch (where.TypeKey)
					{
						case nameof(TaskWhereType.BUILDING):
							buildingIds.Add(new Guid(where.ReferenceId));
							break;
						case nameof(TaskWhereType.FLOOR):
							floorIds.Add(new Guid(where.ReferenceId));
							break;
						case nameof(TaskWhereType.HOTEL):
							hotelIds.Add(where.ReferenceId);
							break;
						case nameof(TaskWhereType.RESERVATION):
							reservationIds.Add(where.ReferenceId);
							break;
						case nameof(TaskWhereType.ROOM):
							roomIds.Add(new Guid(where.ReferenceId));
							break;
						case nameof(TaskWhereType.WAREHOUSE):
							warehouseIds.Add(new Guid(where.ReferenceId));
							break;
					}
				}

				if (roomIds.Any())
				{
					tasksQuery = tasksQuery.Where(t => (t.FromRoomId != null && roomIds.Contains(t.FromRoomId.Value)) || (t.ToRoomId != null && roomIds.Contains(t.ToRoomId.Value)));
				}

				//if (floorIds.Any())
				//{
				//	query = query.Where(t => floorIds.Contains(t.FloorId.Value));
				//}

				//if (buildingIds.Any())
				//{
				//	query = query.Where(t => buildingIds.Contains(t.BuildingId.Value));
				//}

				if (hotelIds.Any())
				{
					tasksQuery = tasksQuery.Where(t => (t.FromHotelId != null && hotelIds.Contains(t.FromHotelId)) || (t.ToHotelId != null && hotelIds.Contains(t.ToHotelId)));
				}

				if (reservationIds.Any())
				{
					tasksQuery = tasksQuery.Where(t => (t.FromReservationId != null && reservationIds.Contains(t.FromReservationId)) || (t.ToReservationId != null && reservationIds.Contains(t.ToReservationId)));
				}

				if (warehouseIds.Any())
				{
					tasksQuery = tasksQuery.Where(t => (t.FromWarehouseId != null && warehouseIds.Contains(t.FromWarehouseId.Value)) || (t.ToWarehouseId != null && warehouseIds.Contains(t.ToWarehouseId.Value)));
				}
			}

			var tasksCount = await tasksQuery.CountAsync();

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					////case "ASSET_ASC":
					////	tasksQuery = tasksQuery.OrderBy(q => q.AssetName);
					////	break;
					////case "ASSET_DESC":
					////	tasksQuery = tasksQuery.OrderByDescending(q => q.AssetName);
					////	break;
					////case "ACTION_ASC":
					////	tasksQuery = tasksQuery.OrderBy(q => q.ActionName);
					////	break;
					////case "ACTION_DESC":
					////	tasksQuery = tasksQuery.OrderByDescending(q => q.ActionName);
					////	break;
					//case "ROOM_ASC":
					//	tasksQuery = tasksQuery.OrderBy(q => q.Room.Name);
					//	break;
					//case "ROOM_DESC":
					//	tasksQuery = tasksQuery.OrderByDescending(q => q.Room.Name);
					//	break;
					//case "FLOOR_ASC":
					//	tasksQuery = tasksQuery.OrderBy(q => q.Floor.Name);
					//	break;
					//case "FLOOR_DESC":
					//	tasksQuery = tasksQuery.OrderByDescending(q => q.Floor.Name);
					//	break;
					//case "BUILDING_ASC":
					//	tasksQuery = tasksQuery.OrderBy(q => q.Building.Name);
					//	break;
					//case "BUILDING_DESC":
					//	tasksQuery = tasksQuery.OrderByDescending(q => q.Building.Name);
					//	break;
					//case "HOTEL_ASC":
					//	tasksQuery = tasksQuery.OrderBy(q => q.Hotel.Name);
					//	break;
					//case "HOTEL_DESC":
					//	tasksQuery = tasksQuery.OrderByDescending(q => q.Hotel.Name);
					//	break;
					case "CREATED_AT_ASC":
						tasksQuery = tasksQuery.OrderBy(q => q.CreatedAt);
						break;
					case "CREATED_AT_DESC":
						tasksQuery = tasksQuery.OrderByDescending(q => q.CreatedAt);
						break;
					case "STARTS_AT_ASC":
						tasksQuery = tasksQuery.OrderBy(q => q.StartsAt);
						break;
					case "STARTS_AT_DESC":
						tasksQuery = tasksQuery.OrderByDescending(q => q.StartsAt);
						break;
					default:
						break;
				}
			}

			var numberOfTasksTreshold = 2;
			var tasks = await tasksQuery.Select(t => new 
			{
				Actions = t.Actions.Select(a => new 
				{ 
					ActionName = a.ActionName,
					AssetName = a.AssetName,
					AssetQuantity = a.AssetQuantity,
				}).ToArray(),
				t.Id,
				t.StatusKey,
				t.StartsAt,
				t.UserId,
				t.IsForPlannedAttendant,
				t.FromName,
				t.ToName,
				t.TypeKey,
				//RoomName = t.Room.Name,
				//HotelName = t.Hotel.Name,
				//FloorName =  t.Floor.Name,
				//BuildingName = t.Building.Name,
				t.IsGuestRequest,
				//IsRoomOccupied = t.Room.IsOccupied,
				t.PriorityKey,
			}).ToArrayAsync();

			var userDaysMap = data.ToDictionary(d => d.UserId, d => d.DayGroups.ToDictionary(dg => dg.DateString));

			foreach (var task in tasks)
			{
				var dayKey = "OTHER";
				if (task.TypeKey != TaskType.EVENT.ToString())
				{
					dayKey = task.StartsAt.ToString("d");
				}
				
				var day = userDaysMap[task.UserId.Value][dayKey];

				var t = new WeeklyTaskViewModel
				{
					Actions = task.Actions.Select(a => new WeeklyTaskActionViewModel
					{
						ActionName = a.ActionName,
						AssetName = a.AssetName,
						AssetQuantity = a.AssetQuantity,
					}).ToArray(),
					Id = task.Id,
					StatusDescription = task.StatusKey,
					StatusKey = task.StatusKey,
					WhereDescription = $"{(task.FromName.IsNull() ? "" : $"{task.FromName} -> ")}{task.ToName}",
					IsGuestRequest = task.IsGuestRequest,
					IsBelowTreshold = day.Tasks.Count < numberOfTasksTreshold,
				};

				day.Tasks.Add(t);

				if (day.Tasks.Count > numberOfTasksTreshold)
				{
					day.ShowMoreTasks = true;
					day.NumberOfHiddenTasks++;
				}
			}

			return new WeeklyTasksViewModel
			{
				TotalNumberOfUserGroups = usersCount,
				UserGroups = data,
				WeekDays = weekDays
			};
		}
	}
}
