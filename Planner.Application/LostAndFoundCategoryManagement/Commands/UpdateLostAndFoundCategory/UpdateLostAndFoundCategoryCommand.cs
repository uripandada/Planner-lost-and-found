using FluentValidation;
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

namespace Planner.Application.CategoryManagement.Commands.UpdateLostAndFoundCategory
{
	public class UpdateLostAndFoundCategoryCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int ExpirationDays { get; set; }
	}

	public class UpdateLostAndFoundCategoryCommandHandler : IRequestHandler<UpdateLostAndFoundCategoryCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateLostAndFoundCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateLostAndFoundCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.LostAndFoundCategories.FindAsync(request.Id);

			if (category == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find lost and found category to update."
				};
			}

			category.ModifiedAt = DateTime.UtcNow;
			category.ModifiedById = this._userId;
			category.Name = request.Name;
			category.ExpirationDays = request.ExpirationDays;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Lost and Found Category updated."
			};
		}
	}

	public class UpdateLostAndFoundCategoryCommandValidator : AbstractValidator<UpdateLostAndFoundCategoryCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public UpdateLostAndFoundCategoryCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var category = await this._databaseContext.LostAndFoundCategories.Where(t => t.Name.ToLower() == key.ToLower() && t.Id != command.Id).FirstOrDefaultAsync();
				return category == null;
			}).WithMessage("LOST_AND_FOUND_CATEGORY_ALREADY_EXISTS");
		}
	}
}
