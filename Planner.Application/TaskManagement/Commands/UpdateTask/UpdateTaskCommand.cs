using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Commands.UpdateTask
{
	public class UpdateTaskCommand: IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
		public string StartsAtString { get; set; }
		public Guid UserId { get; set; }
	}

	public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public UpdateTaskCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemTaskGenerator = systemTaskGenerator;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}

		public async Task<ProcessResponse> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
		{
			var task = (Domain.Entities.SystemTask)null;
			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				task = await this._databaseContext.SystemTasks
					.Include(t => t.FromHotel)
					.Include(t => t.ToHotel)
					.Include(t => t.Actions)
					.Where(t => t.Id == request.Id).FirstOrDefaultAsync();

				if (task == null)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to find task."
					};
				}

				var oldValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

				// TODO: ADD THE PROPERTIES THAT SHOULD BE UPDATED HERE!
				
				task.StartsAt = Common.Helpers.DateTimeHelper.ParseIsoDate(request.StartsAtString);
				task.UserId = request.UserId;
				task.IsManuallyModified = true;
				task.ModifiedAt = DateTime.UtcNow;
				task.ModifiedById = this._userId;

				var newValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

				var taskHistory = this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Task updated.", task, oldValue, newValue);
				await this._databaseContext.SystemTaskHistorys.AddAsync(taskHistory);

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			var taskIds = new Guid[] { task.Id };
			var userIds = new List<Guid>();
			if (task.UserId.HasValue) userIds.Add(task.UserId.Value);

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "Your task has changed");

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Task updated."
			};
		}
	}
}
