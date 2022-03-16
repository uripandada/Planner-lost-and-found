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

namespace Planner.Application.ExperienceCategoryManagement.Queries.GetPageOfExperienceCategories
{
	public class ExperienceCategoryGridItemViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ExperienceName { get; set; }
	}

	public class GetPageOfExperienceCategoriesQuery : GetPageRequest, IRequest<PageOf<ExperienceCategoryGridItemViewModel>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
	}

	public class GetPageOfExperienceCategoriesQueryHandler : GetPageRequest, IRequestHandler<GetPageOfExperienceCategoriesQuery, PageOf<ExperienceCategoryGridItemViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfExperienceCategoriesQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<ExperienceCategoryGridItemViewModel>> Handle(GetPageOfExperienceCategoriesQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.ExperienceCategories
				.AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				query = query.Where(c => c.Name.ToLower().Contains(keywordsValue));
			}

			var count = 0;
			if(request.Skip > 0 || request.Take > 0)
			{
				count = await query.CountAsync();
			}

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					case "NAME_ASC":
						query = query.OrderBy(q => q.Name);
						break;
					case "NAME_DESC":
						query = query.OrderByDescending(q => q.Name);
						break;
					case "CREATED_AT_DESC":
						query = query.OrderBy(q => q.CreatedAt);
						break;
					case "CREATED_AT_ASC":
						query = query.OrderByDescending(q => q.CreatedAt);
						break;
					default:
						break;
				}
			}

			if (request.Skip > 0)
			{
				query = query.Skip(request.Skip);
			}

			if (request.Take > 0)
			{
				query = query.Take(request.Take);
			}

			var categories = await query.ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = categories.Length;
			}

			var response = new PageOf<ExperienceCategoryGridItemViewModel>
			{
				TotalNumberOfItems = count,
				Items = categories.Select(d => new ExperienceCategoryGridItemViewModel
				{
					Id = d.Id,
					Name = d.Name,
					ExperienceName = d.ExperienceName
				}).ToArray()
			};

			return response;
		}
	}
}
