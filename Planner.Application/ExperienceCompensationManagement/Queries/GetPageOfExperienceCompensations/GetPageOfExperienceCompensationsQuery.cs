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

namespace Planner.Application.ExperienceCompensationManagement.Queries.GetPageOfExperienceCompensations
{
	public class ExperienceCompensationGridItemViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		//public int Credits { get; set; }
	}

	public class GetPageOfExperienceCompensationsQuery : GetPageRequest, IRequest<PageOf<ExperienceCompensationGridItemViewModel>>
	{
		public string Keywords { get; set; }
		public string SortKey { get; set; }
	}

	public class GetPageOfExperienceCompensationsQueryHandler : GetPageRequest, IRequestHandler<GetPageOfExperienceCompensationsQuery, PageOf<ExperienceCompensationGridItemViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfExperienceCompensationsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<ExperienceCompensationGridItemViewModel>> Handle(GetPageOfExperienceCompensationsQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.ExperienceCompensations
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
					case "PRICE_ASC":
						query = query.OrderBy(q => q.Price);
						break;
					case "PRICE_DESC":
						query = query.OrderByDescending(q => q.Price);
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

			var compensations = await query.ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = compensations.Length;
			}

			var response = new PageOf<ExperienceCompensationGridItemViewModel>
			{
				TotalNumberOfItems = count,
				Items = compensations.Select(d => new ExperienceCompensationGridItemViewModel
				{
					Id = d.Id,
					Name = d.Name,
					Price = d.Price
				}).ToArray()
			};

			return response;
		}
	}
}
