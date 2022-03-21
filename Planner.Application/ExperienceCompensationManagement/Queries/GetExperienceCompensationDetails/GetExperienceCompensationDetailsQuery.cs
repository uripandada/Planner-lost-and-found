using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceCompensationManagement.Queries.GetExperienceCompensationDetails
{
	public class ExperienceCompensationDetailsViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Price{ get; set; }
		public string Currency { get; set; }
	}

	public class GetExperienceCompensationDetailsQuery : IRequest<ExperienceCompensationDetailsViewModel>
	{
		public Guid Id { get; set; }
	}

	public class GetExperienceCompensationDetailsQueryHandler : IRequestHandler<GetExperienceCompensationDetailsQuery, ExperienceCompensationDetailsViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetExperienceCompensationDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ExperienceCompensationDetailsViewModel> Handle(GetExperienceCompensationDetailsQuery request, CancellationToken cancellationToken)
		{
			var compensation = await this._databaseContext.ExperienceCompensations.FindAsync(request.Id);

			return new ExperienceCompensationDetailsViewModel
			{
				Id = compensation.Id,
				Name = compensation.Name,
				Price = compensation.Price,
				Currency = compensation.Currency
			};
		}
	}
}
