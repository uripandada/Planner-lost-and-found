using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.FloorAffinities.Queries.GetListOfFloorAffinities;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.UpdateCleaningPlanGroup
{
	public class UpdateCleaningPlanGroupResult
	{
		public Guid Id { get; set; }
		public IEnumerable<UpdateCleaningGroupAvailabilityIntervalResult> Intervals { get; set; }
		public CleanerData SecondaryCleaner { get; set; }
	}

	public class UpdateCleaningGroupAvailabilityIntervalResult
	{
		public Guid Id { get; set; }
		public string FromTimeString { get; set; }
		public string ToTimeString { get; set; }
	}
	public class UpdateCleaningGroupAffinity
	{
		public string ReferenceId { get; set; }
		public string ReferenceName { get; set; }
		public string ReferenceDescription { get; set; }
		public CleaningPlanGroupAffinityType Type { get; set; }
	}

	public class UpdateCleaningPlanGroupCommand : IRequest<ProcessResponse<UpdateCleaningPlanGroupResult>>
	{
		public Guid Id { get; set; }

		public int? MaxCredits { get; set; }
		public int? MaxDepartures { get; set; }
		public int? MaxTwins { get; set; }
		public int? WeeklyHours { get; set; }
		public bool MustFillAllCredits { get; set; }
		public Guid? SecondaryCleanerId { get; set; }

		public IEnumerable<UpdateCleaningGroupAffinity> Affinities { get; set; }
		public IEnumerable<UpdateCleaningPlanGroupAvailabilityInterval> AvailabilityIntervals { get; set; }
	}

	public class UpdateCleaningPlanGroupAvailabilityInterval
	{
		public Guid? Id { get; set; }
		public string FromTimeString { get; set; }
		public string ToTimeString { get; set; }
	}

	public class UpdateCleaningPlanGroupCommandHandler : IRequestHandler<UpdateCleaningPlanGroupCommand, ProcessResponse<UpdateCleaningPlanGroupResult>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UpdateCleaningPlanGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse<UpdateCleaningPlanGroupResult>> Handle(UpdateCleaningPlanGroupCommand request, CancellationToken cancellationToken)
		{
			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				var planGroup = await this._databaseContext.CleaningPlanGroups
					.Include(g => g.Affinities)
					.Include(g => g.AvailabilityIntervals)
					.Where(g => g.Id == request.Id)
					.FirstOrDefaultAsync();

				if(planGroup == null)
				{
					await transaction.RollbackAsync();
					return new ProcessResponse<UpdateCleaningPlanGroupResult>
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to find cleaner to update."
					};
				}

				var addedAvailabilityIntervals = new List<CleaningPlanGroupAvailabilityInterval>();
				var updatedAvailabilityIntervals = new List<CleaningPlanGroupAvailabilityInterval>();
				var removedAvailabilityIntervals = new List<CleaningPlanGroupAvailabilityInterval>();

				var existingBuildingIds = new HashSet<Guid>();
				var existingFloorIds = new HashSet<Guid>();
				var existingFloorSections = new HashSet<string>();
				var existingFloorSubSections = new HashSet<string>();

				var addedAffinities = new List<CleaningPlanGroupAffinity>();
				var removedAffinities = new List<CleaningPlanGroupAffinity>();

				foreach (var efa in planGroup.Affinities)
				{
					switch (efa.AffinityType)
					{
						case Common.Enums.CleaningPlanGroupAffinityType.BUILDING:
							existingBuildingIds.Add(new Guid(efa.ReferenceId));
							break;
						case Common.Enums.CleaningPlanGroupAffinityType.FLOOR:
							existingFloorIds.Add(new Guid(efa.ReferenceId));
							break;
						case Common.Enums.CleaningPlanGroupAffinityType.FLOOR_SECTION:
							existingFloorSections.Add(efa.ReferenceId);
							break;
						case Common.Enums.CleaningPlanGroupAffinityType.FLOOR_SUB_SECTION:
							existingFloorSubSections.Add(efa.ReferenceId);
							break;
					}

					if (request.Affinities.FirstOrDefault(a => a.ReferenceId == efa.ReferenceId && a.Type == efa.AffinityType) == null)
					{
						removedAffinities.Add(efa);
					}
				}

				foreach(var affinity in request.Affinities)
				{
					var addAffinity = false;
					switch (affinity.Type)
					{
						case Common.Enums.CleaningPlanGroupAffinityType.BUILDING:
							if (!existingBuildingIds.Contains(new Guid(affinity.ReferenceId)))
							{
								addAffinity = true;	
							}
							break;
						case Common.Enums.CleaningPlanGroupAffinityType.FLOOR:
							if (!existingFloorIds.Contains(new Guid(affinity.ReferenceId)))
							{
								addAffinity = true;
							}
							break;
						case Common.Enums.CleaningPlanGroupAffinityType.FLOOR_SECTION:
							if (!existingFloorSections.Contains(affinity.ReferenceId))
							{
								addAffinity = true;
							}
							break;
						case Common.Enums.CleaningPlanGroupAffinityType.FLOOR_SUB_SECTION:
							if (!existingFloorSubSections.Contains(affinity.ReferenceId))
							{
								addAffinity = true;
							}
							break;
					}

					if (addAffinity)
					{
						addedAffinities.Add(new CleaningPlanGroupAffinity
						{
							AffinityType = affinity.Type,
							ReferenceId = affinity.ReferenceId,
							CleaningPlanGroupId = planGroup.Id,
						});
					}
				}

				var existingIntervalIds = new HashSet<Guid>();
				//var allNewIntervals = new List<CleaningPlanGroupAvailabilityInterval>();
				var selectedExistingIntervals = request.AvailabilityIntervals.Where(i => i.Id.HasValue).ToList();
				foreach(var eai in planGroup.AvailabilityIntervals)
				{
					existingIntervalIds.Add(eai.Id);
					if(!selectedExistingIntervals.Any(i => i.Id.Value == eai.Id))
					{
						removedAvailabilityIntervals.Add(eai);
					}
					else
					{
					}
				}

				foreach(var interval in request.AvailabilityIntervals)
				{
					if (interval.Id.HasValue)// && existingIntervalIds.Contains(interval.Id.Value))
					{
						var existingInterval = planGroup.AvailabilityIntervals.First(i => i.Id == interval.Id.Value);
						existingInterval.From = interval.FromTimeString.FromSimpleTimeStringToTimeSpan();
						existingInterval.To = interval.ToTimeString.FromSimpleTimeStringToTimeSpan();
						updatedAvailabilityIntervals.Add(new CleaningPlanGroupAvailabilityInterval { 
							Id = interval.Id.Value,
							To = interval.ToTimeString.FromSimpleTimeStringToTimeSpan(),
							From = interval.FromTimeString.FromSimpleTimeStringToTimeSpan(),
							CleaningPlanGroupId = planGroup.Id
						});
						continue;
					}
					
					addedAvailabilityIntervals.Add(new CleaningPlanGroupAvailabilityInterval { Id = Guid.NewGuid(), CleaningPlanGroupId = planGroup.Id, From = interval.FromTimeString.FromSimpleTimeStringToTimeSpan(), To = interval.ToTimeString.FromSimpleTimeStringToTimeSpan() });
				}
				//allNewIntervals.AddRange(addedAvailabilityIntervals);

				if (removedAffinities.Any())
				{
					this._databaseContext.CleaningPlanGroupAffinities.RemoveRange(removedAffinities);
				}
				if (removedAvailabilityIntervals.Any())
				{
					this._databaseContext.CleaningPlanGroupAvailabilityIntervals.RemoveRange(removedAvailabilityIntervals);
				}
				if (addedAvailabilityIntervals.Any())
				{
					await this._databaseContext.CleaningPlanGroupAvailabilityIntervals.AddRangeAsync(addedAvailabilityIntervals);
				}
				if (addedAffinities.Any())
				{
					await this._databaseContext.CleaningPlanGroupAffinities.AddRangeAsync(addedAffinities);
				}

				planGroup.MaxCredits = request.MaxCredits;
				planGroup.MaxDepartures = request.MaxDepartures;
				planGroup.MaxTwins = request.MaxTwins;
				planGroup.MustFillAllCredits = request.MustFillAllCredits;
				planGroup.WeeklyHours = request.WeeklyHours;
				planGroup.SecondaryCleanerId = request.SecondaryCleanerId;

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);

				var allIntervals = new List<UpdateCleaningGroupAvailabilityIntervalResult>();
				allIntervals.AddRange(addedAvailabilityIntervals.Select(i => new UpdateCleaningGroupAvailabilityIntervalResult
				{
					Id = i.Id,
					FromTimeString = i.From.ToString(@"hh\:mm"),
					ToTimeString = i.To.ToString(@"hh\:mm")
				}).ToArray());
				allIntervals.AddRange(updatedAvailabilityIntervals.Select(i => new UpdateCleaningGroupAvailabilityIntervalResult
				{
					Id = i.Id,
					FromTimeString = i.From.ToString(@"hh\:mm"),
					ToTimeString = i.To.ToString(@"hh\:mm")
				}).ToArray());

				var result = new ProcessResponse<UpdateCleaningPlanGroupResult>
				{
					Data = new UpdateCleaningPlanGroupResult
					{
						Id = request.Id,
						Intervals = allIntervals,
						SecondaryCleaner = null
					},
					HasError = false,
					IsSuccess = true,
					Message = "Cleaner updated"
				};


				if (planGroup.SecondaryCleanerId.HasValue)
				{
					var secondaryUser = await this._databaseContext.Users.FindAsync(planGroup.SecondaryCleanerId.Value);
					result.Data.SecondaryCleaner = new CleanerData
					{
						AvailabilityIntervals = new TimeIntervalData[0],
						Affinities = new AffinityData[0],
						Id = secondaryUser.Id,
						MaxCredits = null,
						MaxDepartures = null,
						MaxTwins = null,
						MustFillAllCredits = false,
						Name = (secondaryUser.FirstName.IsNotNull() ? secondaryUser.FirstName + " " : "") + (secondaryUser.LastName.IsNotNull() ? secondaryUser.LastName : ""),
						Username = secondaryUser.UserName,
						WeekHours = null
					};
				}

				return result;
			}
		}
	}
}
