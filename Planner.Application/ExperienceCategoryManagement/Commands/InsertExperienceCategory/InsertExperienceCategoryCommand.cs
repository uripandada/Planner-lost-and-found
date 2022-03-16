using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceCategoryManagement.Commands.InsertExperienceCategory
{
	public class InsertExperienceCategoryCommand : IRequest<ProcessResponse<Guid>>
	{
		public string Name { get; set; }
		public string ExperienceName { get; set; }
	}

	public class InsertExperienceCategoryCommandHandler : IRequestHandler<InsertExperienceCategoryCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertExperienceCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertExperienceCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = new ExperienceCategory
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Id = Guid.NewGuid(),
				Name = request.Name,
				ExperienceName = request.ExperienceName
			};

			await this._databaseContext.ExperienceCategories.AddAsync(category);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = category.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Experience category inserted."
			};
		}
	}

	public class InsertExperienceCategoryCommandValidator : AbstractValidator<InsertExperienceCategoryCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public InsertExperienceCategoryCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var category = await this._databaseContext.ExperienceCategories.Where(t => t.Name.ToLower() == key.ToLower()).FirstOrDefaultAsync();
				return category == null;
			}).WithMessage("EXPERIENCE_CATEGORY_ALREADY_EXISTS");
		}
	}
}
