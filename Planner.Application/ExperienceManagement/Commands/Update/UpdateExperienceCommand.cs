using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceManagement.Commands.Update
{
	public class UpdateExperienceCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
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
	}

	public class UpdateExperienceCommandHandler : IRequestHandler<UpdateExperienceCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateExperienceCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
		{
			var experience = await this._databaseContext.Experiences.FindAsync(request.Id);

			if (experience == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find experience to update."
				};
			}

			experience.ModifiedAt = DateTime.UtcNow;
			experience.ModifiedById = this._userId;
			experience.RoomName = request.RoomName;
			experience.GuestName = request.GuestName;
			experience.CheckIn = request.CheckIn;
			experience.CheckOut = request.CheckOut;
			experience.ReservationId = request.ReservationId;
			experience.VIP = request.VIP;
			experience.Email = request.Email;
			experience.PhoneNumber = request.PhoneNumber;
			experience.Type = request.Type;
			experience.Description = request.Description;
			experience.Actions = request.Actions;
			experience.InternalFollowUp = request.InternalFollowUp;
			experience.ExperienceCategoryId = request.ExperienceCategoryId;
			experience.ExperienceCompensationId = request.ExperienceCompensationId;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Experience updated."
			};
		}
	}
}
