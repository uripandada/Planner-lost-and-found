using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomCategoryManagement.Commands.DeleteRoomCategory
{
	public class DeleteRoomCategoryCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}
	public class DeleteRoomCategoryCommandHandler : IRequestHandler<DeleteRoomCategoryCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public DeleteRoomCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(DeleteRoomCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.RoomCategorys.FindAsync(request.Id);

			if(category == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find room category to delete."
				};
			}

			if (category.IsSystemDefaultForReservationSync)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "System defined category can't be deleted."
				};
			}

			this._databaseContext.RoomCategorys.Remove(category);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Room category deleted"
			};
		}
	}
}
