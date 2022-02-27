using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetTaskMessages
{
	public class TaskMessagesViewModel
	{
		public string CreatedByUserFullName { get; set; }
		public string CreatedByInitials { get; set; }
		public PageOf<TaskMessageViewModel> PageOfMessages { get; set; }
	}

	public class TaskMessageViewModel
	{
		public Guid Id { get; set; }
		public string Message { get; set; }
		public Guid CreatedByUserId { get; set; }
		public string CreatedByUserFullName { get; set; }
		public string CreatedByInitials { get; set; }
		public bool HasAvatar { get; set; }
		public string AvatarUrl { get; set; }
		public string CreatedAtString { get; set; }
		public bool IsMyMessage { get; set; }
	}

	public class GetTaskMessagesQuery : IRequest<TaskMessagesViewModel>
	{
		public Guid TaskId { get; set; }
	}

	public class GetTaskMessagesQueryHandler : IRequestHandler<GetTaskMessagesQuery, TaskMessagesViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetTaskMessagesQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<TaskMessagesViewModel> Handle(GetTaskMessagesQuery request, CancellationToken cancellationToken)
		{
			var messages = await this._databaseContext
				.SystemTaskMessages
				.Include(h => h.CreatedBy)
				.Where(h => h.SystemTaskId == request.TaskId)
				.ToListAsync();

			var viewModels = messages.OrderBy(h => h.CreatedAt).Select(h => new TaskMessageViewModel
			{
				CreatedAtString = h.CreatedAt.ToString("f"),
				CreatedByUserFullName = $"{h.CreatedBy.FirstName} {h.CreatedBy.LastName}",
				Id = h.Id,
				Message = h.Message,
				AvatarUrl = null,
				HasAvatar = false,
				CreatedByInitials = $"{(h.CreatedBy.FirstName.IsNull() ? "" : h.CreatedBy.FirstName[0].ToString())}{(h.CreatedBy.LastName.IsNull() ? "" : h.CreatedBy.LastName[0].ToString())}",
				CreatedByUserId = h.CreatedById.Value,
				IsMyMessage = h.CreatedById == this._userId
			}).ToArray();

			var user = await this._databaseContext.Users.FindAsync(this._userId);

			var response = new TaskMessagesViewModel
			{
				CreatedByInitials= $"{(user.FirstName.IsNull() ? "" : user.FirstName[0].ToString())}{(user.LastName.IsNull() ? "" : user.LastName[0].ToString())}",
				CreatedByUserFullName = $"{user.FirstName} {user.LastName}",
				PageOfMessages = new PageOf<TaskMessageViewModel>
				{
					Items = viewModels,
					TotalNumberOfItems = viewModels.Length
				}
			};
			return response;
		}
	}
}
