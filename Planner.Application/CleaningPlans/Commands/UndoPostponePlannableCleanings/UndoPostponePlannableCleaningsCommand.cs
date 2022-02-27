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

namespace Planner.Application.CleaningPlans.Commands.UndoPostponePlannableCleanings
{
	public class UndoPostponePlannableCleaningsCommand : IRequest<ProcessResponse>
	{
		public IEnumerable<Guid> Ids { get; set; }
	}

	public class UndoPostponePlannableCleaningsCommandHandler : IRequestHandler<UndoPostponePlannableCleaningsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UndoPostponePlannableCleaningsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(UndoPostponePlannableCleaningsCommand request, CancellationToken cancellationToken)
		{
			var postponerCleanings = await this._databaseContext
				.CleaningPlanItems
				.Include(i => i.PostponeeCleaningPlanItem)
				.Where(i => i.IsPostponed && i.IsPostponer && request.Ids.Contains(i.Id))
				.ToArrayAsync();

			if (postponerCleanings.Any())
			{
				foreach(var postponerCleaning in postponerCleanings)
				{

					postponerCleaning.PostponeeCleaningPlanItem = null;
					postponerCleaning.PostponeeCleaningPlanItemId = null;
					postponerCleaning.IsPostponed = false;
					postponerCleaning.IsPostponer = false;

					var postponee = postponerCleaning.PostponeeCleaningPlanItem;
					if(postponee != null)
					{
						postponee.IsActive = false;
					}

					// TODO: Ask Jonathan what to do on undo postpone of already planned cleanings.
					// If postponed to cleaning is already planned, for now just skip it.
					// TODO: Ask Jonathan what to do on undo postpone of already postponed cleanings.
					// If postponed to cleaning is already postponed, for now just skip it.
				}

				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Cleaning postpones undone."
			};
		}
	}
}
