using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Application.TaskManagement.Queries.GetTaskConfigurationSavePreview;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Commands.CancelTasksByConfiguration
{
	public class CancelTasksByConfigurationCommand : IRequest<ProcessResponse>
	{
		public Guid TaskConfigurationId { get; set; }
	}
	public class CancelTasksByConfigurationCommandHandler : IRequestHandler<CancelTasksByConfigurationCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public CancelTasksByConfigurationCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemTaskGenerator = systemTaskGenerator;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
			this._systemEventsService = systemEventsService;
		}

		public async Task<ProcessResponse> Handle(CancelTasksByConfigurationCommand request, CancellationToken cancellationToken)
		{
			var existingTasks = await this._databaseContext
				.SystemTasks
				.Include(t => t.Actions)
				.Where(t => t.SystemTaskConfigurationId == request.TaskConfigurationId)
				.ToArrayAsync();

			var taskHistories = new List<SystemTaskHistory>();
			var userIds = new HashSet<Guid>();
			var taskIds = new List<Guid>();

			foreach (var task in existingTasks)
			{
				if (task.StatusKey == TaskStatusType.FINISHED.ToString())
				{
					continue;
				}

				if (task.StatusKey == TaskStatusType.CANCELLED.ToString())
				{
					continue;
				}

				var oldTaskHistoryData = this._systemTaskGenerator.GenerateTaskHistoryData(task);

				task.StatusKey = TaskStatusType.CANCELLED.ToString();
				task.ModifiedAt = DateTime.UtcNow;
				task.ModifiedById = this._userId;

				var newTaskHistoryData = this._systemTaskGenerator.GenerateTaskHistoryData(task);
				var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task cancelled by task configuration cancel.", task, oldTaskHistoryData, newTaskHistoryData);
				taskHistories.Add(taskHistory);

				if (task.UserId.HasValue && !userIds.Contains(task.UserId.Value)) userIds.Add(task.UserId.Value);
				taskIds.Add(task.Id);
			}

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "Some of your tasks have been cancelled");

			if (taskHistories.Any())
			{
				await this._databaseContext.SystemTaskHistorys.AddRangeAsync(taskHistories);
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Task(s) cancelled"
			};
		}
	}
}
