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

namespace Planner.Application.CategoryManagement.Commands.InsertLostAndFoundCategory
{
	public class InsertLostAndFoundCategoryCommand : IRequest<ProcessResponse<Guid>>
	{
		public string Name { get; set; }
		public int ExpirationDays { get; set; }
	}

	public class InsertLostAndFoundCategoryCommandHandler : IRequestHandler<InsertLostAndFoundCategoryCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertLostAndFoundCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertLostAndFoundCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = new LostAndFoundCategory
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Id = Guid.NewGuid(),
				Name = request.Name,
				ExpirationDays = request.ExpirationDays
			};

			await this._databaseContext.LostAndFoundCategories.AddAsync(category);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = category.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Lost and Found Category inserted."
			};
		}
	}

	public class InsertLostAndFoundCategoryCommandValidator : AbstractValidator<InsertLostAndFoundCategoryCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public InsertLostAndFoundCategoryCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var category = await this._databaseContext.LostAndFoundCategories.Where(t => t.Name.ToLower() == key.ToLower()).FirstOrDefaultAsync();
				return category == null;
			}).WithMessage("LOST_AND_FOUND_CATEGORY_ALREADY_EXISTS");
		}
	}
}
