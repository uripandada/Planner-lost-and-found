using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common;
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

namespace Planner.Application.MobileApi.Tasks.Commands.UpdateMultipleTaskStatusesForMobile
{
	public class UpdateMultipleTaskStatusesForMobileCommand : IRequest<SimpleProcessResponse>
	{
		public string HotelId { get; set; }
		public IEnumerable<UpdateMultipleTaksStatusesItem> Tasks { get; set; }
	}

	public class UpdateMultipleTaksStatusesItem
	{
		public Guid TaskId { get; set; }
		/// <summary>
		/// resume,claimed,rejected,started,paused,completed,cancelled
		/// </summary>
		public string Status { get; set; }
	}


	public class UpdateMultipleTaskStatusesForMobileCommandHandler : IRequestHandler<UpdateMultipleTaskStatusesForMobileCommand, SimpleProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public UpdateMultipleTaskStatusesForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemTaskGenerator = systemTaskGenerator;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}


		public async Task<SimpleProcessResponse> Handle(UpdateMultipleTaskStatusesForMobileCommand request, CancellationToken cancellationToken)
		{
			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);
			var tasks = new Dictionary<Guid, Domain.Entities.SystemTask>();
			var tasksClaimedBySomeoneElse = new List<Domain.Entities.SystemTask>();

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				var taskHistories = new List<SystemTaskHistory>();
				var taskIds = request.Tasks.Select(t => t.TaskId).ToArray();
				tasks = await this._databaseContext.SystemTasks.Include(t => t.Actions).Where(st => taskIds.Contains(st.Id)).ToDictionaryAsync(st => st.Id);


				foreach (var taskData in request.Tasks)
				{
					var task = tasks[taskData.TaskId];
					var taskStatus = this._GetTaskStatus(taskData.Status);
					if(taskStatus == TaskStatusType.WAITING && !task.MustBeFinishedByAllWhos)
					{
						// multiple status updates
						var tasksFromTheSameConfiguration = await this._databaseContext.SystemTasks.Include(t => t.Actions).Where(st => st.SystemTaskConfigurationId == task.SystemTaskConfigurationId).ToArrayAsync();

						foreach (var t in tasksFromTheSameConfiguration)
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
							else if (t.ToReservationId != null)
							{
								if (task.ToReservationId == null) continue;
								if (t.ToReservationId != task.ToReservationId) continue;

								tasksClaimedBySomeoneElse.Add(t);
							}
						}
					}

					var oldValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);

					task.StatusKey = taskStatus.ToString();
					task.ModifiedAt = dateTime;
					task.ModifiedById = this._userId;

					var newValue = this._systemTaskGenerator.GenerateTaskHistoryData(task);
					taskHistories.Add(this._systemTaskGenerator.GenerateTaskHistory("MOBILE_USER", "Status changed.", task, oldValue, newValue));
				}

				foreach (var t in tasksClaimedBySomeoneElse)
				{
					var toldValue = this._systemTaskGenerator.GenerateTaskHistoryData(t);

					t.StatusKey = TaskStatusType.CLAIMED_BY_SOMEONE_ELSE.ToString();
					t.ModifiedAt = dateTime;
					t.ModifiedById = this._userId;

					var tnewValue = this._systemTaskGenerator.GenerateTaskHistoryData(t);

					taskHistories.Add(this._systemTaskGenerator.GenerateTaskHistory("MOBILE_USER", "Status changed. Claimed by someone else.", t, toldValue, tnewValue));
				}

				await this._databaseContext.SystemTaskHistorys.AddRangeAsync(taskHistories);

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			var tIds = new List<Guid>();
			var userIds = new HashSet<Guid>();

			foreach (var t in tasks.Values)
			{
				tIds.Add(t.Id);
				if (t.UserId.HasValue && !userIds.Contains(t.UserId.Value)) userIds.Add(t.UserId.Value);
			}
			foreach (var t in tasksClaimedBySomeoneElse)
			{
				tIds.Add(t.Id);
				if (t.UserId.HasValue && !userIds.Contains(t.UserId.Value)) userIds.Add(t.UserId.Value);
			}

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, tIds, "Your task status changed");

			return new SimpleProcessResponse
			{
				Success = true
			};
		}

		private Common.Enums.TaskStatusType _GetTaskStatus(string status)
		{
			var statusValue = status.ToLower();

			if (this._TaskStatusesMap.ContainsKey(statusValue))
				return this._TaskStatusesMap[statusValue];

			return Common.Enums.TaskStatusType.UNKNOWN;
		}

		private Dictionary<string, Common.Enums.TaskStatusType> _TaskStatusesMap = new Dictionary<string, Common.Enums.TaskStatusType>
		{
			{ "resume", Common.Enums.TaskStatusType.STARTED},
			{ "resumed", Common.Enums.TaskStatusType.STARTED},
			{ "started", Common.Enums.TaskStatusType.STARTED },
			{ "claimed", Common.Enums.TaskStatusType.WAITING },
			{ "rejected", Common.Enums.TaskStatusType.REJECTED },
			{ "paused", Common.Enums.TaskStatusType.PAUSED },
			{ "completed", Common.Enums.TaskStatusType.FINISHED },
			{ "cancelled", Common.Enums.TaskStatusType.CANCELLED },
		};
	}
}
