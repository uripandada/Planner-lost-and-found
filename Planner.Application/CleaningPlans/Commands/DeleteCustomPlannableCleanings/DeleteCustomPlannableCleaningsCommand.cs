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

namespace Planner.Application.CleaningPlans.Commands.DeleteCustomPlannableCleanings
{
	public class DeleteCustomPlannableCleaningsCommand : IRequest<ProcessResponse>
	{
		public IEnumerable<Guid> Ids { get; set; }
	}

	public class DeleteCustomPlannableCleaningsCommandHandler : IRequestHandler<DeleteCustomPlannableCleaningsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DeleteCustomPlannableCleaningsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(DeleteCustomPlannableCleaningsCommand request, CancellationToken cancellationToken)
		{
			var cleanings = await this._databaseContext
				.CleaningPlanItems
				.Where(i => request.Ids.Contains(i.Id) && i.IsCustom)
				.ToArrayAsync();

			try
			{
				this._databaseContext.CleaningPlanItems.RemoveRange(cleanings);
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}
			catch(Exception ex)
			{
				return new ProcessResponse
				{
					HasError = false,
					IsSuccess = true,
					Message = "Unable to delete custom cleanings. Cleanings already used in the system in some other place."
				};
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Custom cleanings deleted."
			};
		}
	}
}
