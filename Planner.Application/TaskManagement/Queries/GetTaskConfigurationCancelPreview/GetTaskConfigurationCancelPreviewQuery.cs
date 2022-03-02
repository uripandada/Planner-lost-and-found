using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Application.TaskManagement.Queries.GetTaskConfigurationSavePreview;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetTaskConfigurationCancelPreview
{
	public class TaskConfigurationCancelPreview
	{
		public TaskConfigurationCancelPreview()
		{
			this.TasksToCancel = new List<TaskPreviewData>();
			this.AlreadyCancelledTasks = new List<TaskPreviewData>();
			this.AlreadyFinishedTasks = new List<TaskPreviewData>();
		}

		public List<TaskPreviewData> TasksToCancel { get; set; }
		public List<TaskPreviewData> AlreadyCancelledTasks { get; set; }
		public List<TaskPreviewData> AlreadyFinishedTasks { get; set; }
	}
	public class GetTaskConfigurationCancelPreviewQuery : IRequest<TaskConfigurationCancelPreview>
	{
		public Guid TaskConfigurationId { get; set; }
	}
	public class GetTaskConfigurationCancelPreviewQueryHandler : IRequestHandler<GetTaskConfigurationCancelPreviewQuery, TaskConfigurationCancelPreview>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly Guid _userId;

		public GetTaskConfigurationCancelPreviewQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator)
		{
			this._databaseContext = databaseContext;
			this._systemTaskGenerator = systemTaskGenerator;
			this._userId = contextAccessor.UserId();
		}

		public async Task<TaskConfigurationCancelPreview> Handle(GetTaskConfigurationCancelPreviewQuery request, CancellationToken cancellationToken)
		{
			var preview = new TaskConfigurationCancelPreview();

			var existingTasksData = (await this._databaseContext.SystemTasks
			.Where(t => t.SystemTaskConfigurationId == request.TaskConfigurationId)
			.Select(q => new
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
				UserFullName = $"{q.User.FirstName} {q.User.LastName}",
				UserUsername = q.User.UserName,
				//BuildingName = q.Building == null ? "" : q.Building.Name,
				//FloorName = q.Floor == null ? "" : q.Floor.Name,
				//HotelName = q.Hotel == null ? "" : q.Hotel.Name,
				//RoomName = q.Room == null ? "" : q.Room.Name,
				q.IsManuallyModified,
				q.TypeKey,
				q.EventTimeKey,
				q.EventModifierKey,
				q.EventKey,
				q.FromName,
				q.FromHotelId,
				FromHotelName = q.FromHotel.Name,
				q.FromRoomId,
				q.FromWarehouseId,
				q.FromReservationId,
				q.ToName,
				q.ToHotelId,
				ToHotelName = q.ToHotel.Name,
				q.ToRoomId,
				q.ToWarehouseId,
				q.ToReservationId,
			})
			.ToListAsync());

			// C: Extract hotel ids from the tasks in order to load proper time zones
			var hotelIdsSet = new HashSet<string>();
			foreach (var task in existingTasksData)
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


			var existingTasks = existingTasksData
				.Select(d =>
				{
					var currentHotelTime = d.FromHotelId.IsNotNull() ? hotelTimeZonesMap[d.FromHotelId].CurrentHotelTime : hotelTimeZonesMap[d.ToHotelId].CurrentHotelTime;

					var item = new TaskPreviewData
					{
						Actions = d.Actions.Select(a => new TaskPreviewActionData 
						{
							ActionName = a.ActionName,
							AssetName = a.AssetName,
							AssetQuantity = a.AssetQuantity,
						}).ToArray(),
						StartsAt = d.StartsAt,
						UserFullName = d.UserFullName,
						UserUsername = d.UserUsername,
						When = TaskDescriptions.GetWhen(d.StartsAt, d.TypeKey, d.EventTimeKey, d.EventModifierKey, d.EventKey, currentHotelTime),
						WhenDescription = TaskDescriptions.GetWhenDescription(d.StartsAt, d.TypeKey, d.EventTimeKey, currentHotelTime),
						Where = "TO-SET",
						WhereDescription = "TO-SET"
					};

					var fromWhere = TaskDescriptions.GetWhere2(d.FromHotelId, d.FromHotelName, d.FromWarehouseId, d.FromReservationId, d.FromRoomId, d.FromName);
					var toWhere = TaskDescriptions.GetWhere2(d.ToHotelId, d.ToHotelName, d.ToWarehouseId, d.ToReservationId, d.ToRoomId, d.ToName);
					item.Where = $"{fromWhere.Where} -> {toWhere.Where}";
					item.WhereDescription = $"{fromWhere.Description} -> {toWhere.Description}";

					return item;
				}
				).ToArray();

			var taskIndex = -1;
			foreach (var existingTask in existingTasksData)
			{
				taskIndex++;
				var existingTaskPreview = existingTasks[taskIndex];

				if (existingTask.StatusKey == TaskStatusType.FINISHED.ToString())
				{
					preview.AlreadyFinishedTasks.Add(existingTaskPreview);
					continue;
				}

				if (existingTask.StatusKey == TaskStatusType.CANCELLED.ToString())
				{
					preview.AlreadyCancelledTasks.Add(existingTaskPreview);
					continue;
				}

				preview.TasksToCancel.Add(existingTaskPreview);
			}
			
			return preview;
		}
	}

}
