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

namespace Planner.Application.CategoryManagement.Commands.DeleteLostAndFoundCategory
{
	public class DeleteLostAndFoundCategoryCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}
	public class DeleteLostAndFoundCategoryCommandHandler : IRequestHandler<DeleteLostAndFoundCategoryCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public DeleteLostAndFoundCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(DeleteLostAndFoundCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.LostAndFoundCategories.FindAsync(request.Id);

			if(category == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find lost and found category to delete."
				};
			}

			this._databaseContext.LostAndFoundCategories.Remove(category);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "lost and found category deleted"
			};
		}
	}
}
