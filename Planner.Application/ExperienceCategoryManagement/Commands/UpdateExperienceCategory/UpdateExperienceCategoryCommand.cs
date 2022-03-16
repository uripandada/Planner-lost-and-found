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

namespace Planner.Application.ExperienceCategoryManagement.Commands.UpdateExperienceCategory
{
	public class UpdateExperienceCategoryCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ExperienceName { get; set; }
	}

	public class UpdateExperienceCategoryCommandHandler : IRequestHandler<UpdateExperienceCategoryCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateExperienceCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateExperienceCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.ExperienceCategories.FindAsync(request.Id);

			if (category == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find Experience category to update."
				};
			}

			category.ModifiedAt = DateTime.UtcNow;
			category.ModifiedById = this._userId;
			category.Name = request.Name;
			category.ExperienceName = request.ExperienceName;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Experience category updated."
			};
		}
	}

	public class UpdateExperienceCategoryCommandValidator : AbstractValidator<UpdateExperienceCategoryCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public UpdateExperienceCategoryCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var category = await this._databaseContext.ExperienceCategories.Where(t => t.Name.ToLower() == key.ToLower() && t.Id != command.Id).FirstOrDefaultAsync();
				return category == null;
			}).WithMessage("EXPERIENCE_CATEGORY_ALREADY_EXISTS");
		}
	}
}
