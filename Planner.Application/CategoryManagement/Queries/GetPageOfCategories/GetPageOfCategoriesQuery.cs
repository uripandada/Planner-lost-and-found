﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CategoryManagement.Queries.GetPageOfCategories
{
	public class CategoryGridItemViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class GetPageOfCategoriesQuery : GetPageRequest, IRequest<PageOf<CategoryGridItemViewModel>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
	}

	public class GetPageOfCategoriesQueryHandler : GetPageRequest, IRequestHandler<GetPageOfCategoriesQuery, PageOf<CategoryGridItemViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfCategoriesQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<CategoryGridItemViewModel>> Handle(GetPageOfCategoriesQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Categorys
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

			var response = new PageOf<CategoryGridItemViewModel>
			{
				TotalNumberOfItems = count,
				Items = categories.Select(d => new CategoryGridItemViewModel
				{
					Id = d.Id,
					Name = d.Name,
				}).ToArray()
			};

			return response;
		}
	}
}
