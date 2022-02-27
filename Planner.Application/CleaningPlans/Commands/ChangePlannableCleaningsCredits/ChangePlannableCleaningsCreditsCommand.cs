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

namespace Planner.Application.CleaningPlans.Commands.ChangePlannableCleaningsCredits
{
	public class ChangePlannableCleaningsCreditsCommand : IRequest<ProcessResponse>
	{
		public IEnumerable<Guid> Ids { get; set; }
		public int Credits { get; set; }
	}

	public class ChangePlannableCleaningsCreditsCommandHandler : IRequestHandler<ChangePlannableCleaningsCreditsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ChangePlannableCleaningsCreditsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(ChangePlannableCleaningsCreditsCommand request, CancellationToken cancellationToken)
		{
			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var cleanings = await this._databaseContext.CleaningPlanItems.Where(c => request.Ids.Contains(c.Id)).ToArrayAsync();
				foreach(var c in cleanings)
				{
					c.Credits = request.Credits;
				}

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Credits changed."
			};
		}
	}
}
