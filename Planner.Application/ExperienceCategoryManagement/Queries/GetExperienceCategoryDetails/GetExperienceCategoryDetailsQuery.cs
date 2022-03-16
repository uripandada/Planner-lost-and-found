using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceCategoryManagement.Queries.GetExperienceCategoryDetails
{
	public class ExperienceCategoryDetailsViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ExperienceName { get; set; }
	}

	public class GetExperienceCategoryDetailsQuery : IRequest<ExperienceCategoryDetailsViewModel>
	{
		public Guid Id { get; set; }
	}

	public class GetExperienceCategoryDetailsQueryHandler : IRequestHandler<GetExperienceCategoryDetailsQuery, ExperienceCategoryDetailsViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetExperienceCategoryDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ExperienceCategoryDetailsViewModel> Handle(GetExperienceCategoryDetailsQuery request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.ExperienceCategories.FindAsync(request.Id);

			return new ExperienceCategoryDetailsViewModel
			{
				Id = category.Id,
				Name = category.Name,
				ExperienceName= category.ExperienceName,
			};
		}
	}
}
