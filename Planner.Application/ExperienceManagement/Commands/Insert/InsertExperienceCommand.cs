using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceManagement.Commands.Insert
{
	public class InsertExperienceCommand : IRequest<ProcessResponse<Guid>>
	{
		public string RoomName { get; set; }
		public string GuestName { get; set; }
		public DateTime? CheckIn { get; set; }
		public DateTime? CheckOut { get; set; }
		public string ReservationId { get; set; }
		public string VIP { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public int Type { get; set; }
		public string Description { get; set; }
		public string Actions { get; set; }
		public string InternalFollowUp { get; set; }
		public Guid ExperienceCategoryId { get; set; }
		public Guid ExperienceCompensationId { get; set; }
		public string Group { get; set; }
		public ExperienceTicketStatus ExperienceTicketStatus { get; set; }
		public ExperienceClientRelationStatus ExperienceClientRelationStatus { get; set; }
		public ExperienceResolutionStatus ExperienceResolutionStatus { get; set; }
	}

	public class InsertExperienceCommandHandler : IRequestHandler<InsertExperienceCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertExperienceCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertExperienceCommand request, CancellationToken cancellationToken)
		{
			var experience = new Experience
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Id = Guid.NewGuid(),
				RoomName = request.RoomName,
				GuestName = request.GuestName,
				CheckIn = request.CheckIn,
				CheckOut = request.CheckOut,
				ReservationId = request.ReservationId,
				VIP = request.VIP,
				Email = request.Email,
				PhoneNumber = request.PhoneNumber,
				Type = request.Type,
				Description = request.Description,
				Actions = request.Actions,
				InternalFollowUp = request.InternalFollowUp,
				ExperienceCategoryId = request.ExperienceCategoryId,
				ExperienceCompensationId = request.ExperienceCompensationId,
				ExperienceTicketStatus = request.ExperienceTicketStatus,
				ExperienceClientRelationStatus = request.ExperienceClientRelationStatus,
				ExperienceResolutionStatus = request.ExperienceResolutionStatus
			};

			await this._databaseContext.Experiences.AddAsync(experience);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = experience.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Experience inserted."
			};
		}
	}

	public class InsertExperienceCommandValidator : AbstractValidator<InsertExperienceCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public InsertExperienceCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;
			RuleFor(command => command.GuestName).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var experience = await this._databaseContext.Experiences.Where(t => t.GuestName.ToLower() == key.ToLower()).FirstOrDefaultAsync();
				return experience == null;
			}).WithMessage("EXPERIENCE_ALREADY_EXISTS");
		}
	}
}