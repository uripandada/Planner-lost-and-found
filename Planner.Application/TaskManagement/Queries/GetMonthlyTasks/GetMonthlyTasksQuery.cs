using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetPageOfTasks;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetMonthlyTasks
{
	public class GetMonthlyTasksQuery : IRequest<TaskGridItemData[]>
	{
		public string Keywords { get; set; }
		public DateTime MonthDate { get; set; }
		public string StatusKey { get; set; }
		public string ActionName { get; set; }
		public Guid? AssetId { get; set; }
		public Guid? AssetGroupId { get; set; }
		public TaskWhoData[] Whos { get; set; }
		public TaskWhereData[] Wheres { get; set; }
	}

	public class GetMonthlyTasksQueryHandler : IRequestHandler<GetMonthlyTasksQuery, TaskGridItemData[]>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetMonthlyTasksQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<TaskGridItemData[]> Handle(GetMonthlyTasksQuery request, CancellationToken cancellationToken)
		{
			var fromDateValue = request.MonthDate;
			var toDateValue = fromDateValue.AddDays(1);

			var query = this._databaseContext.SystemTasks
				.Where(t => t.StartsAt >= fromDateValue && t.StartsAt < toDateValue)
				.AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
			}

			if (request.StatusKey.IsNotNull() && request.StatusKey != "ANY")
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
					query = query.Where(t => (t.FromRoomId != null && roomIds.Contains(t.FromRoomId.Value)) || (t.ToRoomId != null && roomIds.Contains(t.ToRoomId.Value)));
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

			var data = await query.Select(q => new
			{
				Actions = q.Actions.Select(a => new 
				{ 
					ActionName = a.ActionName,
					AssetName = a.AssetName,
					AssetQuantity = a.AssetQuantity,
				}).ToArray(),
				Id = q.Id,
				//StartsAt = q.StartsAt,
				//StatusKey = q.StatusKey,
				//UserFullName = $"{q.User.FirstName} {q.User.LastName}",
				//UserId = q.User.Id,
				//UserUsername = q.User.UserName,
				//BuildingName = q.Building == null ? "" : q.Building.Name,
				//FloorName = q.Floor == null ? "" : q.Floor.Name,
				//HotelName = q.Hotel == null ? "" : q.Hotel.Name,
				//q.RecurringTypeKey,
				//q.RepeatsForKey,
				//RoomName = q.Room == null ? "" : q.Room.Name,
				//q.TypeKey,
				//q.WhereTypeKey,
				//q.EventKey,
				//q.EventModifierKey,
				//q.EventTimeKey,
				//q.IsGuestRequest,
				//IsRoomOccupied = q.Room.IsOccupied,
				//q.PriorityKey,
				//UserFirstName = q.User.FirstName,
				//UserLastName = q.User.LastName,
				//UserAvatarImageUrl = q.User.Avatar.FileUrl
			}).ToListAsync();

			return data.Select(d =>
			{
				var item = new TaskGridItemData
				{
					Actions = d.Actions.Select(a => new TaskGridItemActionData {
						ActionName = a.ActionName,
						AssetName = a.AssetName,
						AssetQuantity = a.AssetQuantity,
					}),
					Id = d.Id,
				//	StartsAt = d.StartsAt,
				//	StatusKey = d.StatusKey,
				//	TypeKey = d.TypeKey,
				//	RecurringTypeKey = d.RecurringTypeKey,
				//	UserFullName = d.UserFullName,
				//	UserId = d.UserId,
				//	UserUsername = d.UserUsername,
				//	StatusDescription = TaskDescriptions.GetTaskStatusDescription(d.StatusKey),
				//	TypeDescription = TaskDescriptions.GetTaskTypeDescription(d.TypeKey, d.RecurringTypeKey, d.EventModifierKey, d.EventKey),
				//	When = TaskDescriptions.GetWhen(d.StartsAt, d.TypeKey, d.EventTimeKey, d.EventModifierKey, d.EventKey),
				//	WhenDescription = TaskDescriptions.GetWhenDescription(d.StartsAt, d.TypeKey, d.EventTimeKey),
				//	Where = "TO-SET",
				//	WhereDescription = "TO-SET",
				//	IsGuestRequest = d.IsGuestRequest,
				//	IsRoomOccupied = d.IsRoomOccupied,
				//	PriorityKey = d.PriorityKey,
				//	UserAvatarImageUrl = d.UserAvatarImageUrl,
				//	UserInitials = $"{d.UserFirstName[0]}{d.UserLastName[0]}"
				};

				//var where = TaskDescriptions.GetWhere(d.RoomName, d.FloorName, d.BuildingName, d.HotelName);
				//item.Where = where.Where;
				//item.WhereDescription = where.Description;

				return item;
			}).ToArray();
		}
	}

}
