﻿using FluentValidation;
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

namespace Planner.Application.CategoryManagement.Commands.InsertCategory
{
	public class InsertCategoryCommand : IRequest<ProcessResponse<Guid>>
	{
		public string Name { get; set; }
	}

	public class InsertCategoryCommandHandler : IRequestHandler<InsertCategoryCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = new Category
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Id = Guid.NewGuid(),
				Name = request.Name
			};

			await this._databaseContext.Categorys.AddAsync(category);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = category.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Category inserted."
			};
		}
	}

	public class InsertCategoryCommandValidator : AbstractValidator<InsertCategoryCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public InsertCategoryCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var category = await this._databaseContext.Categorys.Where(t => t.Name.ToLower() == key.ToLower()).FirstOrDefaultAsync();
				return category == null;
			}).WithMessage("CATEGORY_ALREADY_EXISTS");
		}
	}
}
