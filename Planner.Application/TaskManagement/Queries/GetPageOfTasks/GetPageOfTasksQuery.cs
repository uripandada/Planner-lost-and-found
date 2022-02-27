using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
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

namespace Planner.Application.TaskManagement.Queries.GetPageOfTasks
{
	public class TaskGridItemActionData
	{
		public int AssetQuantity { get; set; }
		public string ActionName { get; set; }
		public string AssetName { get; set; }

	}

	public class TaskGridItemData
	{
		public Guid Id { get; set; }
		public Guid? UserId { get; set; }
		public bool IsForPlannedAttendant { get; set; }
		public string UserFullName { get; set; }
		public string UserUsername { get; set; }
		public string UserInitials { get; set; }
		public string UserAvatarImageUrl { get; set; }
		public string DefaultUserAvatarColorHex { get; set; }

		public IEnumerable<TaskGridItemActionData> Actions { get; set; }
		
		public string StatusKey { get; set; }
		public string StatusDescription { get; set; }

		public string Where { get; set; }
		public string WhereDescription { get; set; }


		public string When { get; set; }
		public string WhenDescription { get; set; }
		public DateTime StartsAt { get; set; }

		public string TypeKey { get; set; }
		public string RecurringTypeKey { get; set; }
		public string TypeDescription { get; set; }

		public bool IsRoomOccupied { get; set; }
		public bool IsGuestRequest { get; set; }
		public string PriorityKey { get; set; }
	}

	public class GetPageOfTasksQuery : GetPageRequest, IRequest<PageOf<TaskGridItemData>>
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
		public Guid? TaskConfigurationId { get; set; }
	}
	public class GetPageOfTasksQueryHandler : IRequestHandler<GetPageOfTasksQuery, PageOf<TaskGridItemData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfTasksQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<TaskGridItemData>> Handle(GetPageOfTasksQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.SystemTasks
				.AsQueryable();

			if (request.TaskConfigurationId.HasValue)
			{
				query = query.Where(t => t.SystemTaskConfigurationId == request.TaskConfigurationId.Value);
			}

			if (request.FromDateString.IsNotNull())
			{
				var fromDateValue = DateTimeHelper.ParseIsoDate(request.FromDateString).Date;
				query = query.Where(t => t.StartsAt >= fromDateValue);
			}

			if (request.ToDateString.IsNotNull())
			{
				var toDateValue = DateTimeHelper.ParseIsoDate(request.ToDateString).Date.AddDays(1);
				query = query.Where(t => t.StartsAt < toDateValue);
			}

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
			}

			if(request.StatusKey.IsNotNull() && request.StatusKey != "ANY")
			{
				query = query.Where(t => t.StatusKey == request.StatusKey);
			}

			if (request.AssetId.HasValue && request.AssetGroupId.HasValue)
			{
				query = query.Where(t => t.Actions.Any(a => a.AssetId == request.AssetId.Value && a.ActionName == request.ActionName && a.AssetGroupId == request.AssetGroupId.Value));
			}
			else if (request.AssetGroupId.HasValue && !request.AssetId.HasValue)
			{
				query = query.Where(t => t.Actions.Any(a => a.AssetGroupId == request.AssetGroupId.Value && a.ActionName == request.ActionName));
			}
			else if (request.AssetId.HasValue && !request.AssetGroupId.HasValue)
			{
				query = query.Where(t => t.Actions.Any(a => a.AssetId == request.AssetId.Value && a.ActionName == request.ActionName));
			}

			if (request.Whos != null && request.Whos.Any())
			{
				var userIds = new List<Guid?>();
				var userGroupIds = new List<Guid>();
				var userSubGroupIds = new List<Guid>();
				foreach (var who in request.Whos)
				{
					switch (who.TypeKey)
					{
						case nameof(TaskWhoType.GROUP):
							userGroupIds.Add(new Guid(who.ReferenceId));
							break;
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
					query = query.Where(t => t.UserId != null && userIds.Contains(t.UserId));
				}

				if (userGroupIds.Any())
				{
					query = query.Where(t => userGroupIds.Contains(t.User.UserSubGroup.UserGroupId));
				}

				if (userSubGroupIds.Any())
				{
					query = query.Where(t => userSubGroupIds.Contains(t.User.UserSubGroupId.Value));
				}
			}

			if (request.Wheres != null && request.Wheres.Any())
			{
				var roomIds = new List<Guid>();
				var floorIds = new List<Guid>();
				var buildingIds = new List<Guid>();
				var hotelIds = new List<string>();
				var reservationIds = new List<string>();
				var warehouseIds = new List<Guid>();
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
					query = query.Where(t => (t.FromRoomId != null && roomIds.Contains(t.FromRoomId.Value)) || (t.ToRoomId != null && roomIds.Contains(t.ToRoomId.Value)));
				}

				if (hotelIds.Any())
				{
					query = query.Where(t => (t.FromHotelId != null && hotelIds.Contains(t.FromHotelId)) || (t.ToHotelId != null && hotelIds.Contains(t.ToHotelId)));
				}

				if (reservationIds.Any())
				{
					query = query.Where(t => (t.FromReservationId != null && reservationIds.Contains(t.FromReservationId)) || (t.ToReservationId != null && reservationIds.Contains(t.ToReservationId)));
				}

				if (warehouseIds.Any())
				{
					query = query.Where(t => (t.FromWarehouseId != null && warehouseIds.Contains(t.FromWarehouseId.Value)) || (t.ToWarehouseId != null && warehouseIds.Contains(t.ToWarehouseId.Value)));
				}
			}

			var count = await query.CountAsync();

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{

					case "USER_ASC":
						query = query.OrderBy(q => q.User.FirstName).OrderBy(q => q.User.LastName);
						break;
					case "USER_DESC":
						query = query.OrderByDescending(q => q.User.FirstName).OrderByDescending(q => q.User.LastName);
						break;
					case "CREATED_AT_ASC":
						query = query.OrderBy(q => q.CreatedAt);
						break;
					case "CREATED_AT_DESC":
						query = query.OrderByDescending(q => q.CreatedAt);
						break;
					case "STARTS_AT_ASC":
						query = query.OrderBy(q => q.StartsAt);
						break;
					case "STARTS_AT_DESC":
						query = query.OrderByDescending(q => q.StartsAt);
						break;
					default:
						break;
				}
			}

			if (request.Skip > 0)
			{
				query = query.Skip(request.Skip);
			}
			if (request.Take > 0)
			{
				query = query.Take(request.Take);
			}

			var data = await query.Select(q => new 
			{
				Actions = q.Actions.Select(a => new 
				{ 
					ActionName = a.ActionName,
					AssetName = a.AssetName,
					AssetQuantity = a.AssetQuantity,
				}).ToArray(),
				Id = q.Id,
				StartsAt = q.StartsAt,
				StatusKey = q.StatusKey,
				//UserFullName = $"{q.User.FirstName} {q.User.LastName}",
				IsForPlannedAttendant = q.IsForPlannedAttendant,
				UserFirstName = q.User.FirstName,
				UserLastName = q.User.LastName,
				UserId = q.UserId,
				UserUsername = q.User.UserName,
				//BuildingName = q.Building == null ? "" : q.Building.Name,
				//FloorName = q.Floor == null ? "" : q.Floor.Name,
				//HotelName = q.Hotel == null ? "" : q.Hotel.Name,
				q.RecurringTypeKey,
				q.RepeatsForKey,
				//RoomName = q.Room == null ? "" : q.Room.Name,
				q.TypeKey,
				q.WhereTypeKey,
				q.EventKey,
				q.EventModifierKey,
				q.EventTimeKey,
				q.IsGuestRequest,
				//IsRoomOccupied = q.Room == null ? false : q.Room.IsOccupied, // q.Room.IsOccupied,
				q.PriorityKey,
				UserAvatarImageUrl = "", //q.User.Avatar.FileUrl
				q.FromName,
				q.FromHotelId,
				FromHotelName = q.FromHotel.Name,
				q.FromRoomId,
				FromRoomName = q.FromRoom == null ? "" : q.FromRoom.Name,
				q.FromWarehouseId,
				q.FromReservationId,
				q.ToName,
				q.ToHotelId,
				ToHotelName = q.ToHotel.Name,
				q.ToRoomId,
				ToRoomName = q.ToRoom == null ? "" : q.ToRoom.Name,
				q.ToWarehouseId,
				q.ToReservationId,
				DefaultUserAvatarColorHex = q.User.DefaultAvatarColorHex,
			}).ToListAsync();


			// C: Extract hotel ids from the tasks in order to load proper time zones
			var hotelIdsSet = new HashSet<string>();
			foreach (var task in data)
			{
				if (task.FromHotelId.IsNotNull())
				{
					// FROM HOTEL IS ONLY SET ON TASKS THAT HAVE FROM-TO
					// -> IN THAT CASE, THE TASK MUST BE IN THE FROM HOTEL TIME ZONE
					if (!hotelIdsSet.Contains(task.FromHotelId))
					{
						hotelIdsSet.Add(task.FromHotelId);
					}
				}
				else if (task.ToHotelId.IsNotNull())
				{
					// THIS SHOULD ALWAYS BE THE CASE
					if (!hotelIdsSet.Contains(task.ToHotelId))
					{
						hotelIdsSet.Add(task.ToHotelId);
					}
				}
			}
			// /C: Extract hotel ids


			// C: Load hotel time zones from the DB
			var taskHotelIds = hotelIdsSet.ToArray();
			var hotelTimeZonesMap = await this._databaseContext
				.Hotels
				.Where(h => taskHotelIds.Contains(h.Id))
				.Select(h => new HotelTimeZone
				{
					HotelId = h.Id,
					IanaTimeZoneId = h.IanaTimeZoneId,
					WindowsTimeZoneId = h.WindowsTimeZoneId
				})
				.ToDictionaryAsync(htz => htz.HotelId, htz => htz);

			foreach (var htz in hotelTimeZonesMap.Values)
			{
				var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(htz.WindowsTimeZoneId, htz.IanaTimeZoneId);
				htz.CurrentHotelTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneConverter.TZConvert.GetTimeZoneInfo(timeZoneId));
			}
			// /C: Load hotel time zones


			var response = new PageOf<TaskGridItemData>
			{
				TotalNumberOfItems = count,
				Items = data.Select(d => {
					var currentHotelTime = d.FromHotelId.IsNotNull() ? hotelTimeZonesMap[d.FromHotelId].CurrentHotelTime : hotelTimeZonesMap[d.ToHotelId].CurrentHotelTime;

					var userFullName = "N/A";
					var userName = "N/A";
					var userInitials = "N/A";
					if(d.UserId.HasValue)
					{
						userFullName = $"{d.UserFirstName} {d.UserLastName}";
						userName = d.UserUsername;
						userInitials = $"{d.UserFirstName[0]}{d.UserLastName[0]}";
					}
					else if (d.IsForPlannedAttendant)
					{
						userFullName = "Planned Attendant";
						userName = "For the cleaner who will do the cleaning";
						userInitials = "*";
					}

					var item = new TaskGridItemData
					{
						Actions = d.Actions.Select(a => new TaskGridItemActionData
						{
							ActionName = a.ActionName,
							AssetName = a.AssetName,
							AssetQuantity = a.AssetQuantity,
						}).ToArray(),
						//ActionName = d.ActionName,
						//AssetName = d.AssetName,
						//AssetQuantity = d.AssetQuantity,
						Id = d.Id,
						StartsAt = d.StartsAt,
						StatusKey = d.StatusKey,
						TypeKey = d.TypeKey,
						RecurringTypeKey = d.RecurringTypeKey,
						UserFullName = userFullName,
						UserId = d.UserId,
						IsForPlannedAttendant = d.IsForPlannedAttendant,
						UserUsername = userName,
						StatusDescription = TaskDescriptions.GetTaskStatusDescription(d.StatusKey),
						TypeDescription = TaskDescriptions.GetTaskTypeDescription(d.TypeKey, d.RecurringTypeKey, d.EventModifierKey, d.EventKey),
						When = TaskDescriptions.GetWhen(d.StartsAt, d.TypeKey, d.EventTimeKey, d.EventModifierKey, d.EventKey, currentHotelTime),
						WhenDescription = TaskDescriptions.GetWhenDescription(d.StartsAt, d.TypeKey, d.EventTimeKey, currentHotelTime),
						Where = "TO-SET",
						WhereDescription = "TO-SET",
						IsGuestRequest = d.IsGuestRequest,
						IsRoomOccupied = false,
						PriorityKey = d.PriorityKey,
						UserAvatarImageUrl =d.UserAvatarImageUrl,
						DefaultUserAvatarColorHex = d.DefaultUserAvatarColorHex,
						UserInitials = userInitials
					};

					if(d.WhereTypeKey == "FROM_TO")
					{
						var fromWhere = TaskDescriptions.GetWhere2(d.FromHotelName, d.FromWarehouseId, d.FromReservationId, d.FromRoomId, d.FromName, d.FromRoomName);
						var toWhere = TaskDescriptions.GetWhere2(d.ToHotelName, d.ToWarehouseId, d.ToReservationId, d.ToRoomId, d.ToName, d.ToRoomName);
						item.Where = $"{fromWhere.Where} -> {toWhere.Where}";
						item.WhereDescription = $"{fromWhere.Description} -> {toWhere.Description}";
					}
					else if(d.WhereTypeKey == "TO")
					{
						var toWhere = TaskDescriptions.GetWhere2(d.ToHotelName, d.ToWarehouseId, d.ToReservationId, d.ToRoomId, d.ToName, d.ToRoomName);
						item.Where = toWhere.Where;
						item.WhereDescription = toWhere.Description;
					}
					else
					{
						item.Where = "N/A";
						item.WhereDescription = "N/A";
					}

					return item;
				}).ToArray()
			};

			return response;
		}
	}
}
