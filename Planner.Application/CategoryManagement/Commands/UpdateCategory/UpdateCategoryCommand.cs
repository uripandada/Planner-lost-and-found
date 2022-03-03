﻿using FluentValidation;
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

namespace Planner.Application.CategoryManagement.Commands.UpdateCategory
{
	public class UpdateCategoryCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateCategoryCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.Categorys.FindAsync(request.Id);

			if (category == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find category to update."
				};
			}

			category.ModifiedAt = DateTime.UtcNow;
			category.ModifiedById = this._userId;
			category.Name = request.Name;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Category updated."
			};
		}
	}

	public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public UpdateCategoryCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var category = await this._databaseContext.Categorys.Where(t => t.Name.ToLower() == key.ToLower() && t.Id != command.Id).FirstOrDefaultAsync();
				return category == null;
			}).WithMessage("CATEGORY_ALREADY_EXISTS");
		}
	}
}
