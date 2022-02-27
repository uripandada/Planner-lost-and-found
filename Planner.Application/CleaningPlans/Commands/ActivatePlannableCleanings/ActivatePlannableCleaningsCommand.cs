using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.ActivatePlannableCleanings
{
	public class ActivatePlannableCleaningsCommand : IRequest<ProcessResponse>
	{
		public IEnumerable<Guid> Ids { get; set; }
	}

	public class ActivatePlannableCleaningsCommandHandler : IRequestHandler<ActivatePlannableCleaningsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ActivatePlannableCleaningsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(ActivatePlannableCleaningsCommand request, CancellationToken cancellationToken)
		{
			var cleanings = await this._databaseContext
				.CleaningPlanItems
				.Where(i => request.Ids.Contains(i.Id))
				.ToArrayAsync();

			foreach (var cleaning in cleanings)
			{
				cleaning.IsActive = true;
			}

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Cleanings activated."
			};
		}
	}
}
