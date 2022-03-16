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

namespace Planner.Application.CategoryManagement.Queries.GetPageOfLostAndFoundCategories
{
	public class LostAndFoundCategoryGridItemViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int ExpirationDays { get; set; }
	}

	public class GetPageOfLostAndFoundCategoriesQuery : GetPageRequest, IRequest<PageOf<LostAndFoundCategoryGridItemViewModel>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
	}

	public class GetPageOfLostAndFoundCategoriesQueryHandler : GetPageRequest, IRequestHandler<GetPageOfLostAndFoundCategoriesQuery, PageOf<LostAndFoundCategoryGridItemViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfLostAndFoundCategoriesQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<LostAndFoundCategoryGridItemViewModel>> Handle(GetPageOfLostAndFoundCategoriesQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.LostAndFoundCategories
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
					case "EXPIRATION_DAYS_ASC":
						query = query.OrderByDescending(q => q.ExpirationDays);
						break;
					case "EXPIRATION_DAYS_DESC":
						query = query.OrderBy(q => q.ExpirationDays);
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

			var response = new PageOf<LostAndFoundCategoryGridItemViewModel>
			{
				TotalNumberOfItems = count,
				Items = categories.Select(d => new LostAndFoundCategoryGridItemViewModel
				{
					Id = d.Id,
					Name = d.Name,
					ExpirationDays = d.ExpirationDays
				}).ToArray()
			};

			return response;
		}
	}
}
