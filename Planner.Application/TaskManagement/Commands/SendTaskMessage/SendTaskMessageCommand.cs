using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTaskMessages;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Commands.SendTaskMessage
{
	public class SendTaskMessageResult
	{
		public Guid Id { get; set; }
		public string CreatedAtString { get; set; }
	}
	public class SendTaskMessageCommand: IRequest<ProcessResponse<TaskMessageViewModel>>
	{
		public Guid TaskId { get; set; }
		public string Message { get; set; }
	}

	public class SendTaskMessageCommandHandler : IRequestHandler<SendTaskMessageCommand, ProcessResponse<TaskMessageViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public SendTaskMessageCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._systemEventsService = systemEventsService;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
		}

		public async Task<ProcessResponse<TaskMessageViewModel>> Handle(SendTaskMessageCommand request, CancellationToken cancellationToken)
		{
			var message = new Domain.Entities.SystemTaskMessage
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Id = Guid.NewGuid(),
				Message = request.Message,
				SystemTaskId = request.TaskId
			};

			await this._databaseContext.SystemTaskMessages.AddAsync(message);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			var user = await this._databaseContext.Users.FindAsync(this._userId);

			var taskForUserId = await this._databaseContext.SystemTasks.Where(st => st.Id == request.TaskId).Select(st => st.UserId).FirstOrDefaultAsync();
			var taskIds = new Guid[] { request.TaskId };
			var userIds = new List<Guid>();
			if (taskForUserId.HasValue) userIds.Add(taskForUserId.Value);

			await this._systemEventsService.TasksChanged(this._hotelGroupId, userIds, taskIds, "Your task has a new message");

			return new ProcessResponse<TaskMessageViewModel>
			{
				Data = new TaskMessageViewModel
				{
					CreatedAtString = message.CreatedAt.ToString("f"),
					CreatedByUserFullName = $"{user.FirstName} {user.LastName}",
					Id = message.Id,
					Message = message.Message,
					AvatarUrl = null,
					HasAvatar = false,
					CreatedByInitials = $"{(user.FirstName.IsNull() ? "" : user.FirstName[0].ToString())}{(user.LastName.IsNull() ? "" : user.LastName[0].ToString())}",
					CreatedByUserId = this._userId,
					IsMyMessage = true
				},
				HasError = false,
				IsSuccess = true,
				Message = "Message sent."
			};
		}
	}

}
