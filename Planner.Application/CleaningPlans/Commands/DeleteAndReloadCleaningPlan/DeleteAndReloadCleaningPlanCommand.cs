using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Infrastructure;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.DeleteAndReloadCleaningPlan
{
	public class DeleteAndReloadCleaningPlanResult
	{
		public IEnumerable<CleaningTimelineItemData> PlannableItems { get; set; }
		public IEnumerable<PlannedCleaningTimelineItemData> PlannedItems { get; set; }
	}

	public class DeleteAndReloadCleaningPlanCommand : IRequest<ProcessResponse<DeleteAndReloadCleaningPlanResult>>
	{
		public Guid CleaningPlanId { get; set; }
		public bool IsTodaysCleaningPlan { get; set; }
	}

	public class DeleteAndReloadCleaningPlanCommandHandler : IRequestHandler<DeleteAndReloadCleaningPlanCommand, ProcessResponse<DeleteAndReloadCleaningPlanResult>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICleaningProvider _cleaningProvider;
		private readonly ICleaningGeneratorService _cleaningGeneratorService;

		public DeleteAndReloadCleaningPlanCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, ICleaningProvider cleaningProvider, ICleaningGeneratorService cleaningGeneratorService)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._cleaningProvider = cleaningProvider;
			this._cleaningGeneratorService = cleaningGeneratorService;
		}

		public async Task<ProcessResponse<DeleteAndReloadCleaningPlanResult>> Handle(DeleteAndReloadCleaningPlanCommand request, CancellationToken cancellationToken)
		{
			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var cleaningPlanHeader = await this._databaseContext.CleaningPlans.FindAsync(request.CleaningPlanId);
				var cleanings = await this._databaseContext.CleaningPlanItems
					.Where(cpi => cpi.CleaningPlanId == request.CleaningPlanId && cpi.IsActive)
					.ToListAsync();

				var generatedCleanings = await this._cleaningGeneratorService.GenerateCleanings(cleaningPlanHeader.HotelId, request.IsTodaysCleaningPlan, cleaningPlanHeader.Date.Date);

				var difference = await this._FindDifferencesBetweenExistingAndGeneratedCleanings(cleaningPlanHeader.Id, cleanings, generatedCleanings);
				
				if(difference.toInsert != null && difference.toInsert.Any())
				{
					await this._databaseContext.CleaningPlanItems.AddRangeAsync(difference.toInsert, cancellationToken);
				}

				if (difference.toDelete != null && difference.toDelete.Any())
				{
					this._databaseContext.CleaningPlanItems.RemoveRange(difference.toDelete);
				}

				cleaningPlanHeader.ModifiedAt = DateTime.UtcNow;
				cleaningPlanHeader.ModifiedById = this._httpContextAccessor.UserId();

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);

				//var updatedCleanings = new List<CleaningTimelineItemData>();
				//updatedCleanings.AddRange(this._cleaningGeneratorService.CreateTimelineCleanings(difference.toInsert, request.IsTodaysCleaningPlan, cleaningPlanHeader.Date.Date));
				//updatedCleanings.AddRange(this._cleaningGeneratorService.CreateTimelineCleanings(difference.toUpdate, request.IsTodaysCleaningPlan, cleaningPlanHeader.Date.Date));

				var plannedItems = new List<CleaningPlanItem>();
				var plannableItems = new List<CleaningPlanItem>();
				foreach(var item in difference.toInsert)
				{
					if (item.IsPlanned) plannedItems.Add(item);
					else plannableItems.Add(item);
				}
				foreach(var item in difference.toUpdate)
				{
					if (item.IsPlanned) plannedItems.Add(item);
					else plannableItems.Add(item);
				}
				foreach(var item in difference.toSkip)
				{
					if (item.IsPlanned) plannedItems.Add(item);
					else plannableItems.Add(item);
				}

				var hotel = await this._databaseContext.Hotels.FindAsync(cleaningPlanHeader.HotelId);
				var timeZoneId = Infrastructure.HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
				var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
				var cleaningDateUtc = TimeZoneInfo.ConvertTimeToUtc(cleaningPlanHeader.Date.Date, timeZoneInfo);

				return new ProcessResponse<DeleteAndReloadCleaningPlanResult>
				{
					Data = new DeleteAndReloadCleaningPlanResult
					{
						PlannableItems = this._cleaningGeneratorService.CreateTimelineCleanings(plannableItems, cleaningDateUtc, timeZoneId),
						PlannedItems = this._cleaningGeneratorService.CreatePlannedTimelineCleanings(plannedItems, cleaningDateUtc, timeZoneId)
					},
					HasError = false,
					IsSuccess = true,
					Message = "Cleanings refreshed."
				};
			}
		}



		private async Task<(List<CleaningPlanItem> toInsert, List<CleaningPlanItem> toUpdate, List<CleaningPlanItem> toDelete, List<CleaningPlanItem> toSkip)> _FindDifferencesBetweenExistingAndGeneratedCleanings(Guid cleaningPlanId, IEnumerable<CleaningPlanItem> cleanings, IEnumerable<CleaningTimelineItemData> generatedCleanings)
		{
			var cleaningsToInsert = new List<CleaningPlanItem>();
			var cleaningsToDelete = new List<CleaningPlanItem>();
			var cleaningsToUpdate = new List<CleaningPlanItem>();
			var cleaningsToSkip = new List<CleaningPlanItem>();

			var cleaningsMap = cleanings.GroupBy(c => c.RoomId).ToDictionary(c => c.Key, c => c.GroupBy(ci => ci.CleaningPluginId.HasValue ? ci.CleaningPluginId.Value : Guid.Empty).ToDictionary(ci => ci.Key, ci => ci.ToArray()));
			var generatedCleaningsMap = generatedCleanings.GroupBy(c => c.RoomId).ToDictionary(c => c.Key, c => c.GroupBy(ci => ci.CleaningPluginId.HasValue ? ci.CleaningPluginId.Value : Guid.Empty).ToDictionary(ci => ci.Key, ci => ci.ToArray()));

			var checkedIds = new HashSet<Guid>();

			foreach(var gRoomPair in generatedCleaningsMap)
			{
				var roomId = gRoomPair.Key;
				foreach(var gPluginPair in gRoomPair.Value)
				{
					var pluginId = gPluginPair.Key;

					var generatedRoomPluginCleanings = gPluginPair.Value;

					if (!cleaningsMap.ContainsKey(roomId) || !cleaningsMap[roomId].ContainsKey(pluginId))
					{
						cleaningsToInsert.AddRange(generatedRoomPluginCleanings.Select(c => new CleaningPlanItem 
						{ 
							Id = Guid.NewGuid(),
							CleaningId = null,
							CleaningPlanGroupId = null,
							CleaningPlanId = cleaningPlanId,
							CleaningPluginId = c.CleaningPluginId,
							Credits = c.Credits,
							Description = c.CleaningDescription,
							DurationSec = null,
							EndsAt = null,
							StartsAt = null,
							IsActive = true,
							IsChangeSheets = c.IsChangeSheets,
							IsCustom = c.IsCustom,
							IsPlanned = false,
							IsPostponed = false,
							IsPostponee = false,
							IsPostponer = false,
							PostponeeCleaningPlanItemId = null,
							PostponerCleaningPlanItemId = null,
							RoomBedId = c.BedId,
							RoomId = c.RoomId,
							IsPriority = c.IsPriority,
						}).ToArray());
					}
					else
					{
						// UPDATE!
						// remove postponed and custom cleanings because they shouldn't be updated/compared.
						var roomPluginCleanings = cleaningsMap[roomId][pluginId].Where(c => !c.IsCustom && !c.IsPostponee).ToArray();
						foreach (var cleaning in roomPluginCleanings)
						{
							if (!checkedIds.Contains(cleaning.Id)) checkedIds.Add(cleaning.Id);
						}

						if(roomPluginCleanings.Length == generatedRoomPluginCleanings.Length)
						{
							for(int i = 0; i <roomPluginCleanings.Length; i++)
							{
								var cleaning = roomPluginCleanings[i];
								var generatedCleaning = generatedRoomPluginCleanings[i];

								cleaning.Credits = generatedCleaning.Credits;
								cleaning.Description = generatedCleaning.CleaningDescription;
								cleaning.IsChangeSheets = generatedCleaning.IsChangeSheets;
								cleaning.IsPriority = generatedCleaning.IsPriority;

								cleaningsToUpdate.Add(cleaning);
							}
						}
						else if(roomPluginCleanings.Length > generatedRoomPluginCleanings.Length)
						{
							for (int i = 0; i < roomPluginCleanings.Length; i++)
							{
								var cleaning = roomPluginCleanings[i];
								var generatedCleaning = (CleaningTimelineItemData)null;
								
								if(i < generatedRoomPluginCleanings.Length)
								{
									generatedCleaning = generatedRoomPluginCleanings[i];

									cleaning.Credits = generatedCleaning.Credits;
									cleaning.Description = generatedCleaning.CleaningDescription;
									cleaning.IsChangeSheets = generatedCleaning.IsChangeSheets;
									cleaning.IsPriority = generatedCleaning.IsPriority;

									cleaningsToUpdate.Add(cleaning);
								}
								else
								{
									cleaningsToDelete.Add(cleaning);
								}
							}
						}
						else
						{
							for (int i = 0; i < generatedRoomPluginCleanings.Length; i++)
							{
								var cleaning = (CleaningPlanItem)null;
								var generatedCleaning = generatedRoomPluginCleanings[i];

								if (i < roomPluginCleanings.Length)
								{
									cleaning = roomPluginCleanings[i];

									cleaning.Credits = generatedCleaning.Credits;
									cleaning.Description = generatedCleaning.CleaningDescription;
									cleaning.IsChangeSheets = generatedCleaning.IsChangeSheets;
									cleaning.IsPriority = generatedCleaning.IsPriority;

									cleaningsToUpdate.Add(cleaning);
								}
								else
								{
									cleaningsToInsert.Add(new CleaningPlanItem
									{
										Id = Guid.NewGuid(),
										CleaningId = null,
										CleaningPlanGroupId = null,
										CleaningPlanId = cleaningPlanId,
										CleaningPluginId = generatedCleaning.CleaningPluginId,
										Credits = generatedCleaning.Credits,
										Description = generatedCleaning.CleaningDescription,
										DurationSec = null,
										EndsAt = null,
										StartsAt = null,
										IsActive = true,
										IsChangeSheets = generatedCleaning.IsChangeSheets,
										IsCustom = generatedCleaning.IsCustom,
										IsPlanned = false,
										IsPostponed = false,
										IsPostponee = false,
										IsPostponer = false,
										PostponeeCleaningPlanItemId = null,
										PostponerCleaningPlanItemId = null,
										RoomBedId = generatedCleaning.BedId,
										RoomId = generatedCleaning.RoomId,
										IsPriority = generatedCleaning.IsPriority,
									});
								}
							}
						}
					}

				}
			}

			foreach(var cleaning in cleanings)
			{
				// Don't delete postponees
				// Don't delete custom cleanings
				if (!checkedIds.Contains(cleaning.Id))
				{
					// remove postponed and custom cleanings because they shouldn't be updated/compared.
					if (cleaning.IsCustom || cleaning.IsPostponee)
					{
						cleaningsToSkip.Add(cleaning);
					}
					else
					{
						cleaningsToDelete.Add(cleaning);
					}
				}
			}

			return new(cleaningsToInsert, cleaningsToUpdate, cleaningsToDelete, cleaningsToSkip);
		}

		//private void _ChangeUpdatedCleaningFields()










		//private IEnumerable<CleaningPlanItem> _GeneratePlannableCleaningPlanItems(Guid cleaningPlanId, IEnumerable<CleaningTimelineItemData> cleanings)
		//{
		//	return cleanings.Select(c => this._GeneratePlannableCleaningPlanItem(cleaningPlanId, c)).ToArray();
		//}

		//private CleaningPlanItem _GeneratePlannableCleaningPlanItem(Guid cleaningPlanId, CleaningTimelineItemData c)
		//{
		//	var planItem = new CleaningPlanItem
		//	{
		//		Id = Guid.NewGuid(),
		//		CleaningPlanId = cleaningPlanId,
		//		CleaningPluginId = c.CleaningPluginId,
		//		Description = c.CleaningPluginName,
		//		Credits = c.Credits,
		//		IsActive = true,
		//		IsCustom = false,
		//		IsPostponed = false,
		//		RoomId = c.RoomId,
		//		IsChangeSheets = c.IsChangeSheets,
		//		IsPlanned = false,
		//		IsPriority = c.IsPriority,
		//	};

		//	// TODO: FIND OUT WHY THIS LINE EXISTS!
		//	c.Id = planItem.Id.ToString();

		//	return planItem;
		//}
	}
}
