using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.AddPlanItems
{
	public class AddCleaningPlanItemsCommand : IRequest<ProcessResponse>
	{
		public Guid CleaningPlanId { get; set; }
		public IEnumerable<AddCleaningPlanItem> Cleanings { get; set; }
	}

	public class AddCleaningPlanItem
	{
		public Guid Id { get; set; }
		public Guid CleaningPlanGroupId { get; set; }
		public string StartString { get; set; }
		public string EndString { get; set; }
	}

	public class AddCleaningPlanItemsCommandHandler : IRequestHandler<AddCleaningPlanItemsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AddCleaningPlanItemsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(AddCleaningPlanItemsCommand request, CancellationToken cancellationToken)
		{
			var planItemIds = request.Cleanings.Select(c => c.Id).ToArray();

			var planItemsMap = (await this._databaseContext
				.CleaningPlanItems
				.Where(pci => pci.CleaningPlanId == request.CleaningPlanId && planItemIds.Contains(pci.Id))
				.ToArrayAsync())
				.ToDictionary(pci => pci.Id);

			if (planItemsMap.Any())
			{
				foreach(var cleaning in request.Cleanings)
				{
					var planItem = planItemsMap[cleaning.Id];
					var startsAt = DateTimeHelper.ParseIsoDate(cleaning.StartString);
					var endsAt = DateTimeHelper.ParseIsoDate(cleaning.EndString);
					var durationSec = (int)(startsAt.Subtract(startsAt).TotalSeconds);

					planItem.IsPlanned = true;
					planItem.StartsAt = startsAt;
					planItem.EndsAt = endsAt;
					planItem.DurationSec = durationSec;
					planItem.CleaningPlanGroupId = cleaning.CleaningPlanGroupId;
				}

				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Cleanings added."
			};
		}
	}
}
