using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Commands.ReassignTask
{
	public class ReassignTaskCommand : IRequest<ProcessResponse<Guid>>
	{
		public Guid TaskId { get; set; }
		public string StartsAtString { get; set; }
		public Guid? UserId { get; set; }
		public bool IsForPlannedAttendant { get; set; }
	}

	public class ReassignTaskCommandHandler : IRequestHandler<ReassignTaskCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public ReassignTaskCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
			this._systemTaskGenerator = systemTaskGenerator;
		}

		public async Task<ProcessResponse<Guid>> Handle(ReassignTaskCommand request, CancellationToken cancellationToken)
		{
			var task = (Domain.Entities.SystemTask)null;
			var newTask = (Domain.Entities.SystemTask)null;

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				task = await this._databaseContext.SystemTasks
					.Include(t => t.FromHotel)
					.Include(t => t.ToHotel)
					.Include(t => t.Actions)
					.Include(t => t.Messages)
					.Where(t => t.Id == request.TaskId).FirstOrDefaultAsync();

				if (task == null)
				{
					return new ProcessResponse<Guid>
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to find task."
					};
				}

				if (task.StatusKey == TaskStatusType.CANCELLED.ToString())
				{
					return new ProcessResponse<Guid>
					{
						HasError = true,
						IsSuccess = false,
						Message = "Task is cancelled."
					};
				}
				else if (task.StatusKey == TaskStatusType.FINISHED.ToString() || task.StatusKey == TaskStatusType.VERIFIED.ToString())
				{
					return new ProcessResponse<Guid>
					{
						HasError = true,
						IsSuccess = false,
						Message = "Task is finished."
					};

				}
				else if (task.StatusKey == TaskStatusType.STARTED.ToString() || task.StatusKey == TaskStatusType.PAUSED.ToString())
				{
					return new ProcessResponse<Guid>
					{
						HasError = true,
						IsSuccess = false,
						Message = "Task is in progress."
					};
				}

				//var oldValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

				//// TODO: ADD THE PROPERTIES THAT SHOULD BE UPDATED HERE!

				//task.StartsAt = Common.Helpers.DateTimeHelper.ParseIsoDate(request.StartsAtString);
				//task.UserId = request.UserId;
				//task.IsManuallyModified = true;
				//task.ModifiedAt = DateTime.UtcNow;
				//task.ModifiedById = this._userId;

				//var newValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

				//var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task updated.", task, oldValue, newValue);

				var oldValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

				task.StatusKey = TaskStatusType.CANCELLED.ToString();
				task.IsManuallyModified = true;
				task.ModifiedAt = DateTime.UtcNow;
				task.ModifiedById = this._userId;

				var newValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

				var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task cancelled.", task, oldValue, newValue);
				await this._databaseContext.SystemTaskHistorys.AddAsync(taskHistory);

				newTask = new Domain.Entities.SystemTask
				{
					Id = Guid.NewGuid(),
					CreatedAt = DateTime.UtcNow,
					CreatedById = this._userId,
					ModifiedAt = DateTime.UtcNow,
					ModifiedById = this._userId,
					Credits = task.Credits,
					EventKey = task.EventKey,
					EventModifierKey = task.EventModifierKey,
					EventTimeKey = task.EventTimeKey,
					FromHotelId = task.FromHotelId,
					FromReservationId = task.FromReservationId,
					FromRoomId = task.FromRoomId,
					FromWarehouseId = task.FromWarehouseId,
					IsBlockingCleaningUntilFinished = task.IsBlockingCleaningUntilFinished,
					IsGuestRequest = task.IsGuestRequest,
					IsMajorNotificationRaisedWhenFinished = task.IsMajorNotificationRaisedWhenFinished,
					IsManuallyModified = true,
					IsRescheduledEveryDayUntilFinished = task.IsRescheduledEveryDayUntilFinished,
					IsShownInNewsFeed = task.IsShownInNewsFeed,
					MustBeFinishedByAllWhos = task.MustBeFinishedByAllWhos,
					Price = task.Price,
					PriorityKey = task.PriorityKey,
					RecurringTypeKey = task.RecurringTypeKey,
					RepeatsForKey = task.RepeatsForKey,
					StartsAt = Common.Helpers.DateTimeHelper.ParseIsoDate(request.StartsAtString),
					StatusKey = TaskStatusType.PENDING.ToString(),
					SystemTaskConfigurationId = task.SystemTaskConfigurationId,
					ToHotelId = task.ToHotelId,
					FromName = task.FromName,
					ToName = task.ToName,
					ToReservationId = task.ToReservationId,
					ToRoomId = task.ToRoomId,
					ToWarehouseId = task.ToWarehouseId,
					TypeKey = task.TypeKey,
					UserId = request.UserId,
					WhereTypeKey = task.WhereTypeKey,
					IsForPlannedAttendant = task.IsForPlannedAttendant,
				};

				newTask.Actions = task.Actions.Select(a => new Domain.Entities.SystemTaskAction
				{
					Id = Guid.NewGuid(),
					SystemTaskId = newTask.Id,
					ActionName = a.ActionName,
					AssetGroupId = a.AssetGroupId,
					AssetGroupName = a.AssetGroupName,
					AssetId = a.AssetId,
					AssetName = a.AssetName,
					AssetQuantity = a.AssetQuantity,
				}).ToArray();

				newTask.Messages = task.Messages.Select(m => new Domain.Entities.SystemTaskMessage
				{
					Id = Guid.NewGuid(),
					SystemTaskId = newTask.Id,
					CreatedAt = m.CreatedAt,
					CreatedById = m.CreatedById,
					Message = m.Message,
					ModifiedAt = m.ModifiedAt,
					ModifiedById = m.ModifiedById,
				})
				.ToArray();

				var newTaskHistoryValue = this._systemTaskGenerator.GenerateTaskHistoryData(newTask);
				var newTaskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task created by reassignment.", newTask, newTaskHistoryValue, newTaskHistoryValue);

				await this._databaseContext.SystemTasks.AddAsync(newTask);
				if (newTask.Actions.Any())
				{
					await this._databaseContext.SystemTaskActions.AddRangeAsync(newTask.Actions);
				}

				if (newTask.Messages.Any())
				{
					await this._databaseContext.SystemTaskMessages.AddRangeAsync(newTask.Messages);
				}

				await this._databaseContext.SystemTaskHistorys.AddAsync(newTaskHistory);
				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			var taskIds = new Guid[] { task.Id, newTask.Id };
			var userIds = new List<Guid>();
			if (task.UserId.HasValue) userIds.Add(task.UserId.Value);
			if (newTask.UserId.HasValue && !userIds.Contains(newTask.UserId.Value)) userIds.Add(newTask.UserId.Value);

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "Some of your tasks has been reassigned");

			return new ProcessResponse<Guid>
			{
				Data = newTask.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Task updated."
			};
		}
	}
}
