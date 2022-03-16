using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CategoryManagement.Queries.GetLostAndFoundCategoryDetails
{
	public class LostAndFoundCategoryDetailsViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int ExpirationDays { get; set; }
	}

	public class GetLostAndFoundCategoryDetailsQuery : IRequest<LostAndFoundCategoryDetailsViewModel>
	{
		public Guid Id { get; set; }
	}

	public class GetLostAndFoundCategoryDetailsQueryHandler : IRequestHandler<GetLostAndFoundCategoryDetailsQuery, LostAndFoundCategoryDetailsViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetLostAndFoundCategoryDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<LostAndFoundCategoryDetailsViewModel> Handle(GetLostAndFoundCategoryDetailsQuery request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.LostAndFoundCategories.FindAsync(request.Id);

			return new LostAndFoundCategoryDetailsViewModel
			{
				Id = category.Id,
				Name = category.Name,
				ExpirationDays = category.ExpirationDays
			};
		}
	}
}
