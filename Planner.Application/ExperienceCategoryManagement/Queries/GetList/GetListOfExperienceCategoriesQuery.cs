using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceCategoryManagement.Queries.GetList
{
	public class ExperienceCategoryItemData
	{
		public string Name { get; set; }
	}

	public class GetListOfExperienceCategoriesQuery : IRequest<ExperienceCategoryItemData[]>
	{
		public string Name { get; set; }
	}

	public class GetListOfExperienceCategoriesQueryHandler : IRequestHandler<GetListOfExperienceCategoriesQuery, ExperienceCategoryItemData[]>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GetListOfExperienceCategoriesQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ExperienceCategoryItemData[]> Handle(GetListOfExperienceCategoriesQuery request, CancellationToken cancellationToken)
		{
			var user = this._httpContextAccessor.HttpContext.User;
			var query = this._databaseContext.ExperienceCategories.AsQueryable();

			return await query.Where(x => x.Name.ToLower().Contains(request.Name.ToLower())).Select(h => new ExperienceCategoryItemData { Name = h.Name }).Distinct().ToArrayAsync();
		}
	}
}
