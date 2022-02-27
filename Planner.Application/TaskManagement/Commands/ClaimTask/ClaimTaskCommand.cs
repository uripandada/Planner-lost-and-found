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

namespace Planner.Application.TaskManagement.Commands.ClaimTask
{
	public class ClaimTaskCommand: IRequest<ProcessResponse>
	{
		public Guid TaskId { get; set; }
	}

	public class ClaimTaskCommandHandler : IRequestHandler<ClaimTaskCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public ClaimTaskCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemTaskGenerator = systemTaskGenerator;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}

		public async Task<ProcessResponse> Handle(ClaimTaskCommand request, CancellationToken cancellationToken)
		{
			var task = await this._databaseContext.SystemTasks.Include(t => t.Actions).FirstOrDefaultAsync(t => t.Id == request.TaskId);

			if(task == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unknown task."
				};
			}

			if(task.StatusKey != TaskStatusType.PENDING.ToString())
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Only PENDING tasks can be claimed."
				};
			}
			
			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, task.ToHotelId, true);
			var tasksClaimedBySomeoneElse = new List<Domain.Entities.SystemTask>();

			if (!task.MustBeFinishedByAllWhos)
			{
				// multiple status updates
				var tasksFromTheSameConfiguration = await this._databaseContext.SystemTasks.Include(t => t.Actions).Where(st => st.SystemTaskConfigurationId == task.SystemTaskConfigurationId).ToArrayAsync();

				foreach(var t in tasksFromTheSameConfiguration)
				{
					if (t.Id == task.Id) continue;
					if (t.StatusKey == TaskStatusType.REJECTED.ToString()) continue;

					if (t.ToWarehouseId.HasValue)
					{
						if (!task.ToWarehouseId.HasValue) continue;
						if (t.ToWarehouseId.Value != task.ToWarehouseId.Value) continue;

						tasksClaimedBySomeoneElse.Add(t);
					}
					else if (t.ToRoomId.HasValue)
					{
						if (!task.ToRoomId.HasValue) continue;
						if (t.ToRoomId.Value != task.ToRoomId.Value) continue;

						tasksClaimedBySomeoneElse.Add(t);
					}
					else if(t.ToReservationId != null)
					{
						if (task.ToReservationId == null) continue;
						if (t.ToReservationId != task.ToReservationId) continue;

						tasksClaimedBySomeoneElse.Add(t);
					}
				}
			}

			var taskHistories = new List<Domain.Entities.SystemTaskHistory>();
			
			var oldValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

			task.StatusKey = TaskStatusType.WAITING.ToString();
			task.ModifiedAt = dateTime;
			task.ModifiedById = this._userId;

			var newValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

			taskHistories.Add(this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Status changed. Claimed.", task, oldValue, newValue));

			foreach(var t in tasksClaimedBySomeoneElse)
			{
				var toldValue = this._systemTaskGenerator.GenerateTaskHistoryData(t);

				t.StatusKey = TaskStatusType.CLAIMED_BY_SOMEONE_ELSE.ToString();
				t.ModifiedAt = dateTime;
				t.ModifiedById = this._userId;

				var tnewValue = this._systemTaskGenerator.GenerateTaskHistoryData(t);

				taskHistories.Add(this._systemTaskGenerator.GenerateTaskHistory("ADMIN", "Status changed. Claimed by someone else.", t, toldValue, tnewValue));
			}

			await this._databaseContext.SystemTaskHistorys.AddRangeAsync(taskHistories);
			await this._databaseContext.SaveChangesAsync(cancellationToken);


			var taskIds = new List<Guid> { task.Id };
			var userIds = new HashSet<Guid>();
			if (task.UserId.HasValue) userIds.Add(task.UserId.Value);

			foreach (var t in tasksClaimedBySomeoneElse)
			{
				taskIds.Add(t.Id);
				if (t.UserId.HasValue && !userIds.Contains(t.UserId.Value)) userIds.Add(t.UserId.Value);
			}

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "Your tasks have changed");

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Task claimed.",
			};
		}
	}
}
