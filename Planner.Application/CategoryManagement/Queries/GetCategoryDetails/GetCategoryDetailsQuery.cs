using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CategoryManagement.Queries.GetCategoryDetails
{
	public class CategoryDetailsViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int ExpirationDays { get; set; }
	}

	public class GetCategoryDetailsQuery : IRequest<CategoryDetailsViewModel>
	{
		public Guid Id { get; set; }
	}

	public class GetCategoryDetailsQueryHandler : IRequestHandler<GetCategoryDetailsQuery, CategoryDetailsViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetCategoryDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<CategoryDetailsViewModel> Handle(GetCategoryDetailsQuery request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.Categorys.FindAsync(request.Id);

			return new CategoryDetailsViewModel
			{
				Id = category.Id,
				Name = category.Name,
				ExpirationDays = category.ExpirationDays
			};
		}
	}
}
