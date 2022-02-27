using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.PostponePlannableCleanings
{
	public class PostponePlannableCleaningsCommand : IRequest<ProcessResponse>
	{
		public Guid CleaningPlanId { get; set; }
		public IEnumerable<Guid> Ids { get; set; }
	}

	public class PostponePlannableCleaningsCommandHandler : IRequestHandler<PostponePlannableCleaningsCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public PostponePlannableCleaningsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(PostponePlannableCleaningsCommand request, CancellationToken cancellationToken)
		{
			var tomorrowPlan = (Domain.Entities.CleaningPlan)null;
			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var cleaningPlan = await this._databaseContext.CleaningPlans.FindAsync(request.CleaningPlanId);
				var tomorrowDate = cleaningPlan.Date.AddDays(1);
				var hotelId = cleaningPlan.HotelId;
				var userId = this._httpContextAccessor.UserId();

				tomorrowPlan = await this._databaseContext.CleaningPlans
					.Where(cp => cp.HotelId == hotelId && cp.Date == tomorrowDate)
					.FirstOrDefaultAsync();

				if(tomorrowPlan == null)
				{
					tomorrowPlan = new CleaningPlan
					{
						Id = Guid.NewGuid(),
						CreatedAt = DateTime.UtcNow,
						CreatedById = userId,
						Date = tomorrowDate,
						HotelId = hotelId,
						IsSent = false,
						ModifiedAt = DateTime.UtcNow,
						ModifiedById = userId,
						SentAt = null,
						SentById = null,
					};

					await this._databaseContext.CleaningPlans.AddAsync(tomorrowPlan, cancellationToken);
					await this._databaseContext.SaveChangesAsync(cancellationToken);
					await transaction.CommitAsync(cancellationToken);
				}
			}

			var cleaningIds = request.Ids;
			var postponedCleanings = await this._databaseContext
				.CleaningPlanItems
				.Where(pci => cleaningIds.Contains(pci.Id))
				.ToArrayAsync();

			var newCleanings = new List<CleaningPlanItem>();
			
			foreach(var c in postponedCleanings)
			{
				var newCleaning = new CleaningPlanItem
				{
					Id = Guid.NewGuid(),
					CleaningPlanId = tomorrowPlan.Id,
					CleaningPluginId = c.CleaningPluginId,
					Description = c.Description,
					Credits = c.Credits,
					IsActive = c.IsActive,
					IsCustom = c.IsCustom,
					IsPlanned = false,
					RoomId = c.RoomId,
					CleaningPlanGroupId = c.CleaningPlanGroupId,
					IsChangeSheets = c.IsChangeSheets,
					IsPriority = false,
					RoomBedId = c.RoomBedId,
					StartsAt = c.StartsAt,
					EndsAt = c.EndsAt,
					DurationSec = c.DurationSec,
					CleaningId = c.CleaningId,

					IsPostponed = false,
					IsPostponer = false,
					IsPostponee = true,
					PostponerCleaningPlanItemId = c.Id,
					PostponeeCleaningPlanItemId = null,
				};
				newCleanings.Add(newCleaning);

				c.IsPostponed = true;
				c.IsPostponer = true;
				c.PostponeeCleaningPlanItemId = newCleaning.Id;
			}

			await this._databaseContext.CleaningPlanItems.AddRangeAsync(newCleanings);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Cleanings postponed.",
			};
		}
	}
}
