using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceManagement.Queries.GetById
{
	public class ExperienceDetailsViewModel
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
		public ExperienceCategory ExperienceCategory { get; set; }
		public Guid ExperienceCompensationId { get; set; }
		public ExperienceCompensation ExperienceCompensation { get; set; }
	}

	public class GetExperienceDetailsQuery : IRequest<ExperienceDetailsViewModel>
	{
		public Guid Id { get; set; }
	}

	public class GetExperienceDetailsQueryHandler : IRequestHandler<GetExperienceDetailsQuery, ExperienceDetailsViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetExperienceDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ExperienceDetailsViewModel> Handle(GetExperienceDetailsQuery request, CancellationToken cancellationToken)
		{
			var experience = await this._databaseContext.Experiences.FindAsync(request.Id);

			return new ExperienceDetailsViewModel
			{
				Id = experience.Id,
				RoomName = experience.RoomName,
				GuestName = experience.GuestName,
				CheckIn = experience.CheckIn,
				CheckOut = experience.CheckOut,
				ReservationId = experience.ReservationId,
				VIP = experience.VIP,
				Email = experience.Email,
				PhoneNumber = experience.PhoneNumber,
				Type = experience.Type,
				Description = experience.Description,
				Actions = experience.Actions,
				InternalFollowUp = experience.InternalFollowUp,
				ExperienceCategoryId = experience.ExperienceCategoryId,
				ExperienceCompensationId = experience.ExperienceCompensationId,
				ExperienceCategory = experience.ExperienceCategory,
				ExperienceCompensation = experience.ExperienceCompensation
			};
		}
	}
}
