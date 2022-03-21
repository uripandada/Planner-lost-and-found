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

namespace Planner.Application.ExperienceCompensationManagement.Commands.InsertExperienceCompensation
{
	public class InsertExperienceCompensationCommand : IRequest<ProcessResponse<Guid>>
	{
		public string Name { get; set; }
		public int Price { get; set; }
		public string Currency { get; set; }
	}

	public class InsertExperienceCompensationCommandHandler : IRequestHandler<InsertExperienceCompensationCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertExperienceCompensationCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertExperienceCompensationCommand request, CancellationToken cancellationToken)
		{
			var compensation = new ExperienceCompensation
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Id = Guid.NewGuid(),
				Name = request.Name,
				Price = request.Price,
				Currency = request.Currency
			};

			await this._databaseContext.ExperienceCompensations.AddAsync(compensation);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = compensation.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Room compensation inserted."
			};
		}
	}

	public class InsertExperienceCompensationCommandValidator : AbstractValidator<InsertExperienceCompensationCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public InsertExperienceCompensationCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.Name).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var compensation = await this._databaseContext.ExperienceCompensations.Where(t => t.Name.ToLower() == key.ToLower()).FirstOrDefaultAsync();
				return compensation == null;
			}).WithMessage("EXPERIENCE_COMPENSATION_ALREADY_EXISTS");
		}
	}
}
