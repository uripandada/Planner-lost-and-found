using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.RemovePlanItems
{
	public class RemoveCleaningPlanItemsCommand: IRequest<ProcessResponse>
	{
		public IEnumerable<Guid> Ids { get; set; }
	}

	public class RemoveCleaningPlanItemsCommandHandler : IRequestHandler<RemoveCleaningPlanItemsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public RemoveCleaningPlanItemsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(RemoveCleaningPlanItemsCommand request, CancellationToken cancellationToken)
		{
			if (!request.Ids.Any())
			{
				return new ProcessResponse
				{
					HasError = false,
					IsSuccess = true,
					Message = "Nothing to remove."
				};
			}

			try
			{
				using (var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
				{
					var planItems = await this._databaseContext.CleaningPlanItems.Where(cpi => request.Ids.Contains(cpi.Id)).ToArrayAsync();
					
					//var idsString = string.Join(",", request.Ids.Select(id => $"'{id.ToString()}'").ToArray());

					//var deleteCommandText = @$"
					//	DELETE FROM public.cleaning_plan_items 
					//	WHERE id IN ({idsString});
					//";

					//var updateCommandText = @$"
					//	UPDATE public.plannable_cleaning_plan_items 
					//	SET is_planned = false 
					//	WHERE id IN ({idsString});
					//";

					foreach(var planItem in planItems)
					{
						planItem.IsPlanned = false;
						planItem.StartsAt = null;
						planItem.EndsAt = null;
						planItem.CleaningPlanGroupId = null;
						planItem.DurationSec = null;
					}

					//await this._databaseContext.Database.ExecuteSqlRawAsync(deleteCommandText);
					//await this._databaseContext.Database.ExecuteSqlRawAsync(updateCommandText);
					await this._databaseContext.SaveChangesAsync(cancellationToken);

					await transaction.CommitAsync(cancellationToken);
				}
			}
			catch(Exception e)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to remove sent cleaning."
				};
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Cleanings removed."
			};
		}
	}
}
