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

namespace Planner.Application.CleaningPlans.Commands.CancelPlannableCleanings
{
	public class CancelPlannableCleaningsCommand : IRequest<ProcessResponse>
	{
		public IEnumerable<Guid> Ids { get; set; }
	}

	public class CancelPlannableCleaningsCommandHandler : IRequestHandler<CancelPlannableCleaningsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CancelPlannableCleaningsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(CancelPlannableCleaningsCommand request, CancellationToken cancellationToken)
		{
			var cleanings = await this._databaseContext
				.CleaningPlanItems
				.Where(i => request.Ids.Contains(i.Id))
				.ToArrayAsync();

			foreach (var cleaning in cleanings)
			{
				cleaning.IsActive = false;
			}

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Cleanings cancelled."
			};
		}
	}
}
