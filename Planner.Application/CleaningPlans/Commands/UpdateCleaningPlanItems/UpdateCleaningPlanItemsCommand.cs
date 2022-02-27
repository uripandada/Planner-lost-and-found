using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.UpdateCleaningPlanItems
{
	public class UpdateCleaningPlanItem
	{
		public Guid Id { get; set; }
		public Guid RoomId { get; set; }
		public Guid CleaningPlanGroupId { get; set; }
		public string StartString { get; set; }
		public string EndString { get; set; }
	}

	public class UpdateCleaningPlanItemsResult
	{
	}

	public class UpdateCleaningPlanItemsCommand : IRequest<ProcessResponse<UpdateCleaningPlanItemsResult>>
	{
		public Guid CleaningPlanId { get; set; }
		public IEnumerable<UpdateCleaningPlanItem> Items { get; set; }
	}

	public class UpdateCleaningPlanItemsCommandHandler : IRequestHandler<UpdateCleaningPlanItemsCommand, ProcessResponse<UpdateCleaningPlanItemsResult>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UpdateCleaningPlanItemsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse<UpdateCleaningPlanItemsResult>> Handle(UpdateCleaningPlanItemsCommand request, CancellationToken cancellationToken)
		{
			var itemsMap = request.Items.ToDictionary(i => i.Id);
			var planItemIds = request.Items.Select(i => i.Id).ToArray();

			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var planItems = await this._databaseContext.CleaningPlanItems.Where(i => planItemIds.Contains(i.Id)).ToArrayAsync();
				foreach(var planItem in planItems)
				{
					var item = itemsMap[planItem.Id];
					var startsAt = DateTimeHelper.ParseIsoDate(item.StartString);
					var endsAt = DateTimeHelper.ParseIsoDate(item.EndString);
					var durationSec = (int)(startsAt.Subtract(startsAt).TotalSeconds);

					planItem.IsPlanned = true;
					planItem.StartsAt = startsAt;
					planItem.EndsAt = endsAt;
					planItem.DurationSec = durationSec;
					planItem.CleaningPlanGroupId = item.CleaningPlanGroupId;
				}

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}

			return new ProcessResponse<UpdateCleaningPlanItemsResult>
			{
				Data = null,
				HasError = false,
				IsSuccess = true,
				Message = "Cleanings updated."
			};
		}
	}
}
