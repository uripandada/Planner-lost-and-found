using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Products.Queries.GetListOfProducts
{
	public class ProductListItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class GetListOfProductsQuery: IRequest<IEnumerable<ProductListItem>>
	{

	}

	public class GetListOfProductsQueryHandler : IRequestHandler<GetListOfProductsQuery, IEnumerable<ProductListItem>>, IAmAdminApplicationHandler
	{
		private readonly IMasterDatabaseContext _masterDatabaseContext;
		private readonly IDatabaseContext _databaseContext;

		public GetListOfProductsQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<ProductListItem>> Handle(GetListOfProductsQuery request, CancellationToken cancellationToken)
		{
			return await this._databaseContext.RccProducts.Select(p => new ProductListItem 
			{ 
				Id = p.Id,
				Name = p.ExternalName,
			}).ToArrayAsync();
		}
	}
}
