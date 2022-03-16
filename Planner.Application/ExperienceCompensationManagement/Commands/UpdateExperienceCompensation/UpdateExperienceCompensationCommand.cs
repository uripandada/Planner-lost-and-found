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

namespace Planner.Application.ExperienceCompensationManagement.Commands.UpdateExperienceCompensation
{
	public class UpdateExperienceCompensationCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
	}

	public class UpdateExperienceCompensationCommandHandler : IRequestHandler<UpdateExperienceCompensationCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateExperienceCompensationCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateExperienceCompensationCommand request, CancellationToken cancellationToken)
		{
			var compensation = await this._databaseContext.ExperienceCompensations.FindAsync(request.Id);

			if (compensation == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find experience compensation to update."
				};
			}

			compensation.ModifiedAt = DateTime.UtcNow;
			compensation.ModifiedById = this._userId;
			compensation.Name = request.Name;
			compensation.Price = request.Price;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Experience compensation updated."
			};
		}
	}

	public class UpdateExperienceCompensationCommandValidator : AbstractValidator<UpdateExperienceCompensationCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public UpdateExperienceCompensationCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var compensation = await this._databaseContext.ExperienceCompensations.Where(t => t.Name.ToLower() == key.ToLower() && t.Id != command.Id).FirstOrDefaultAsync();
				return compensation == null;
			}).WithMessage("EXPERIENCE_COMPENSATION_ALREADY_EXISTS");
		}
	}
}
