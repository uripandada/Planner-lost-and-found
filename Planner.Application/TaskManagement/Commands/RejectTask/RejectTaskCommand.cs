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
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Commands.RejectTask
{
	public class RejectTaskCommand : IRequest<ProcessResponse>
	{
		public Guid TaskId { get; set; }
	}

	public class RejectTaskCommandHandler : IRequestHandler<RejectTaskCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public RejectTaskCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemTaskGenerator = systemTaskGenerator;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}


		public async Task<ProcessResponse> Handle(RejectTaskCommand request, CancellationToken cancellationToken)
		{
			var task = await this._databaseContext.SystemTasks.Include(t => t.Actions).FirstOrDefaultAsync(t => t.Id == request.TaskId);

			if (task == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unknown task."
				};
			}

			if (task.StatusKey == TaskStatusType.REJECTED.ToString())
			{
				return new ProcessResponse
				{
					HasError = false,
					IsSuccess = true,
					Message = "Task already rejected."
				};
			}

			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, task.ToHotelId, true);
			
			var oldValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

			task.StatusKey = TaskStatusType.CLAIMED.ToString();
			task.ModifiedAt = dateTime;
			task.ModifiedById = this._userId;

			var newValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

			var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Status changed. Rejected.", task, oldValue, newValue);

			await this._databaseContext.SystemTaskHistorys.AddAsync(taskHistory);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			var taskIds = new Guid[] { task.Id };
			var userIds = new List<Guid>();
			if (task.UserId.HasValue) userIds.Add(task.UserId.Value);

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "Your task was rejected");

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Task rejected."
			};
		}
	}
}
