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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetPageOfTasksForGrid
{
	public class TaskGridItemAction
	{
		public int AssetQuantity { get; set; }
		public string ActionName { get; set; }
		public string AssetName { get; set; }

	}
	public class TaskGridItem
	{
		public Guid Id { get; set; }
		public Guid TaskConfigurationId { get; set; }
		public Guid? UserId { get; set; }
		public bool IsForPlannedAttendant { get; set; }
		public string UserFullName { get; set; }
		public string UserUsername { get; set; }
		public string UserInitials { get; set; }
		public string UserAvatarImageUrl { get; set; }
		public string DefaultUserAvatarColorHex { get; set; }
		public Guid? UserGroupId { get; set; }
		public Guid? UserSubGroupId { get; set; }

		public IEnumerable<TaskGridItemAction> Actions { get; set; }

		public string StatusKey { get; set; }
		public string StatusDescription { get; set; }

		public string WhereId { get; set; }
		public string Where { get; set; }
		public string WhereDescription { get; set; }
		public string WhereTypeKey { get; set; }

		public string When { get; set; }
		public string WhenDescription { get; set; }
		public DateTime StartsAt { get; set; }
		public string StartsAtString { get; set; }

		public string TypeKey { get; set; }
		public string RecurringTypeKey { get; set; }
		public string TypeDescription { get; set; }

		public bool IsRoomOccupied { get; set; }
		public bool IsGuestRequest { get; set; }
		public string PriorityKey { get; set; }

		public string CreatedByUserName { get; set; }
		public DateTime CreatedAt { get; set; }
		public string CreatedAtString { get; set; }
		public DateTime ModifiedAt { get; set; }
		public string ModifiedAtString { get; set; }
	}

	public class GetPageOfTasksForGridQuery : IRequest<PageOf<TaskGridItem>>
	{
		public Guid? UserGroupId { get; set; }
		public Guid? UserSubGroupId { get; set; }
		public bool OnlyMyTasks { get; set; }
		public Guid? TaskConfigurationId { get; set; }
		public string SortKey { get; set; }
		public int? Skip { get; set; }
		public int? Take { get; set; }

		/// <summary>
		/// Set to true when you want to load only currently active tasks.
		/// Combine with LoadMissedUnfinishedTasks and CurrentDate.
		/// </summary>
		public bool LoadOnlyCurrentTasks { get; set; }
		/// <summary>
		/// Taken into consideration only if LoadOnlyCurrentTasks is true.
		/// </summary>
		public bool LoadMissedUnfinishedTasks { get; set; }
		/// <summary>
		/// Taken into consideration only if LoadOnlyCurrentTasks is true.
		/// 
		/// Used to send the current date from the client because tasks can be done across multiple hotels and multiple time zones.
		/// Each task is in the time zone of the hotel.
		/// </summary>
		public string CurrentDateString { get; set; }
	}

	public class GetPageOfTasksForGridQueryHandler : IRequestHandler<GetPageOfTasksForGridQuery, PageOf<TaskGridItem>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfTasksForGridQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<TaskGridItem>> Handle(GetPageOfTasksForGridQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.SystemTasks
				.AsQueryable();

			if (request.TaskConfigurationId.HasValue)
			{
				query = query.Where(t => t.SystemTaskConfigurationId == request.TaskConfigurationId.Value);
			}

			if (request.OnlyMyTasks)
			{
				query = query.Where(t => t.UserId == this._userId);
			}

			if (request.UserGroupId.HasValue)
			{
				query = query.Where(t => t.User.UserGroupId != null && t.User.UserGroupId == request.UserGroupId.Value);
			}

			if (request.UserSubGroupId.HasValue)
			{
				query = query.Where(t => t.User.UserSubGroupId != null && t.User.UserSubGroupId.Value == request.UserSubGroupId.Value);
			}

			var currentDate = DateTime.UtcNow;
			if (request.LoadOnlyCurrentTasks)
			{
				currentDate = request.CurrentDateString.IsNotNull() ? DateTime.Parse(request.CurrentDateString, CultureInfo.InvariantCulture) : DateTime.UtcNow.Date;
				var unfinishedTaskStatuses = new string[] { nameof(TaskStatusType.CLAIMED), nameof(TaskStatusType.PAUSED), nameof(TaskStatusType.PENDING), nameof(TaskStatusType.STARTED), nameof(TaskStatusType.WAITING) };
				if (request.LoadMissedUnfinishedTasks)
				{
					query = query.Where(t => t.StartsAt.Date == currentDate || (t.StartsAt.Date < currentDate && unfinishedTaskStatuses.Contains(t.StatusKey)));
				}
				else
				{
					query = query.Where(t => t.StartsAt.Date == currentDate);
				}
			}
			var yesterdayDate = currentDate.AddDays(-1);

			var loadCountFromDatabase = (request.Skip.HasValue && request.Skip.Value > 0) || (request.Take.HasValue && request.Take.Value > 0);
			var count = 0;

			if (loadCountFromDatabase)
			{
				count = await query.CountAsync();
			}

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

			if (request.Skip.HasValue && request.Skip.Value > 0)
			{
				query = query.Skip(request.Skip.Value);
			}
			if (request.Take.HasValue && request.Take.Value > 0)
			{
				query = query.Take(request.Take.Value);
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
				UserGroupId = q.User.UserGroupId,
				UserSubGroupId = q.User.UserSubGroupId,
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
				q.CreatedAt,
				q.ModifiedAt,
				CreatedBy = q.CreatedBy,
				TaskConfigurationId = q.SystemTaskConfigurationId,
			}).OrderByDescending(q => q.StartsAt).ThenBy(q => q.ToRoomName).ToListAsync();

			if (!loadCountFromDatabase)
			{
				count = data.Count;
			}


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


			var response = new PageOf<TaskGridItem>
			{
				TotalNumberOfItems = count,
				Items = data.Select(d => {
					var currentHotelTime = d.FromHotelId.IsNotNull() ? hotelTimeZonesMap[d.FromHotelId].CurrentHotelTime : hotelTimeZonesMap[d.ToHotelId].CurrentHotelTime;

					var userFullName = "N/A";
					var userName = "N/A";
					var userInitials = "N/A";
					if (d.UserId.HasValue)
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

					var item = new TaskGridItem
					{
						Actions = d.Actions.Select(a => new TaskGridItemAction
						{
							ActionName = a.ActionName,
							AssetName = a.AssetName,
							AssetQuantity = a.AssetQuantity,
						}).ToArray(),
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
						UserAvatarImageUrl = d.UserAvatarImageUrl,
						DefaultUserAvatarColorHex = d.DefaultUserAvatarColorHex,
						UserInitials = userInitials,
						CreatedAt = d.CreatedAt,
						ModifiedAt = d.ModifiedAt,
						CreatedByUserName = d.CreatedBy == null ? "" : $"{d.CreatedBy.FirstName} {d.CreatedBy.LastName}",
						UserGroupId = d.UserGroupId,
						UserSubGroupId = d.UserSubGroupId,
						TaskConfigurationId = d.TaskConfigurationId,

						StartsAtString = this._GenerateTimeDescription(d.StartsAt, currentDate, yesterdayDate),
						CreatedAtString = this._GenerateTimeDescription(d.CreatedAt, currentDate, yesterdayDate),
						ModifiedAtString = this._GenerateTimeDescription(d.ModifiedAt, currentDate, yesterdayDate),
					};

					

					if (d.WhereTypeKey == "FROM_TO")
					{
						var fromWhere = TaskDescriptions.GetWhere2(d.FromHotelId, d.FromHotelName, d.FromWarehouseId, d.FromReservationId, d.FromRoomId, d.FromName, d.FromRoomName);
						var toWhere = TaskDescriptions.GetWhere2(d.ToHotelId, d.ToHotelName, d.ToWarehouseId, d.ToReservationId, d.ToRoomId, d.ToName, d.ToRoomName);
						item.Where = $"{fromWhere.Where} -> {toWhere.Where}";
						item.WhereDescription = $"{fromWhere.Description} -> {toWhere.Description}";
						item.WhereId = toWhere.WhereId;
						item.WhereTypeKey = toWhere.WhereTypeKey;
					}
					else if (d.WhereTypeKey == "TO")
					{
						var toWhere = TaskDescriptions.GetWhere2(d.ToHotelId, d.ToHotelName, d.ToWarehouseId, d.ToReservationId, d.ToRoomId, d.ToName, d.ToRoomName);
						item.Where = toWhere.Where;
						item.WhereDescription = toWhere.Description;
						item.WhereId = toWhere.WhereId;
						item.WhereTypeKey = toWhere.WhereTypeKey;
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

		private string _GenerateTimeDescription(DateTime date, DateTime referenceDate, DateTime dateBeforeReferenceDate)
        {
			if (date.Date == referenceDate.Date)
			{
				return date.ToString("HH:mm");
			}
			else if (date.Date == dateBeforeReferenceDate.Date)
			{
				return "Yesterday";
			}
			else
			{
				return date.ToString("MMM d yyyy");
			}
		}
	}
}
