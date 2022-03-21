using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceCompensationManagement.Queries.GetList
{
	public class ExperienceCompensationItemViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public string Currency { get; set; }
	}

	public class GetListExperienceCompensationsQuery : GetPageRequest, IRequest<PageOf<ExperienceCompensationItemViewModel>>
	{
	}

	public class GetListExperienceCompensationsQueryHandler : GetPageRequest, IRequestHandler<GetListExperienceCompensationsQuery, PageOf<ExperienceCompensationItemViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListExperienceCompensationsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<ExperienceCompensationItemViewModel>> Handle(GetListExperienceCompensationsQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.ExperienceCompensations
				.AsQueryable();

			var compensations = await query.ToArrayAsync();

			var count = compensations.Length;

			var response = new PageOf<ExperienceCompensationItemViewModel>
			{
				TotalNumberOfItems = count,
				Items = compensations.Select(d => new ExperienceCompensationItemViewModel
				{
					Id = d.Id,
					Name = d.Name,
					Price = d.Price,
					Currency = d.Currency
				}).ToArray()
			};

			return response;
		}
	}
}
