using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails;
using Planner.Application.FloorAffinities.Queries.GetListOfFloorAffinities;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.AddRemovePlanGroups
{
	public class AddRemoveCleaningPlanGroupsResult
	{
		public bool HasAnyChanges { get; set; }
		public IEnumerable<CleaningTimelineGroupData> InsertedGroups { get; set; }
		public IEnumerable<PlannedCleaningTimelineItemData> PlannedNonEventTasks { get; set; }
	}

	public class AddRemoveCleaningPlanGroupsCommand : IRequest<ProcessResponse<AddRemoveCleaningPlanGroupsResult>>
	{
		public Guid PlanId { get; set; }
		public IEnumerable<Guid> CleanerIds { get; set; }
	}

	public class AddRemoveCleaningPlanGroupsCommandHandler : IRequestHandler<AddRemoveCleaningPlanGroupsCommand, ProcessResponse<AddRemoveCleaningPlanGroupsResult>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AddRemoveCleaningPlanGroupsCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse<AddRemoveCleaningPlanGroupsResult>> Handle(AddRemoveCleaningPlanGroupsCommand request, CancellationToken cancellationToken)
		{
			// TODO: ADD TRANSACTION!!!
			// TODO: ADD TRANSACTION!!!
			// TODO: ADD TRANSACTION!!!

			var plan = await this._databaseContext.CleaningPlans.FindAsync(request.PlanId);

			var existingGroups = (await this._databaseContext.CleaningPlanGroups
				//.Include(g => g.Items)
				//.Include(g => g.AvailabilityIntervals)
				//.Include(g => g.FloorAffinities)
				.Where(g => g.CleaningPlanId == plan.Id)
				.ToListAsync())
				.ToDictionary(g => g.CleanerId);

			var cleanerIdsSet = request.CleanerIds.ToHashSet();

			var groupsToDelete = existingGroups.Values.Where(g => !cleanerIdsSet.Contains(g.CleanerId)).ToArray();
			var groupsToInsert = new List<CleaningPlanGroup>();
			var timeIntervalsToInsert = new List<CleaningPlanGroupAvailabilityInterval>();

			foreach (var cleanerId in cleanerIdsSet)
			{
				// The plan already contains the group (cleaner) so there is nothing to do
				if (existingGroups.ContainsKey(cleanerId))
				{
					continue;
				}

				var groupId = Guid.NewGuid();
				var group = new CleaningPlanGroup
				{
					AvailabilityIntervals = new List<CleaningPlanGroupAvailabilityInterval> {
						new CleaningPlanGroupAvailabilityInterval
						{
							CleaningPlanGroupId = groupId,
							From = new TimeSpan(8, 0, 0),
							To = new TimeSpan(16, 0, 0),
							Id = Guid.NewGuid()
						}
					},
					Cleaner = null,
					CleanerId = cleanerId,
					CleaningPlan = null,
					CleaningPlanId = plan.Id,
					Affinities = new List<CleaningPlanGroupAffinity>(),
					Id = groupId,
					MaxCredits = null,
					MaxDepartures = null,
					MustFillAllCredits = false,
					WeeklyHours = null,
					MaxTwins = null,
					SecondaryCleanerId = null
				};
				groupsToInsert.Add(group);
			}

			var hasAnyChanges = false;
			if (groupsToDelete.Any())
			{
				this._databaseContext.CleaningPlanGroups.RemoveRange(groupsToDelete);
				hasAnyChanges = true;
			}

			var insertedGroupUsers = new Dictionary<Guid, User>();
			var cleanerIdsToInsert = groupsToInsert.Select(g => g.CleanerId).ToArray();

			if (groupsToInsert.Any())
			{
				await this._databaseContext.CleaningPlanGroups.AddRangeAsync(groupsToInsert);

				insertedGroupUsers = (await this._databaseContext.Users.Where(u => cleanerIdsToInsert.Contains(u.Id)).ToListAsync()).ToDictionary(u => u.Id);
				hasAnyChanges = true;
			}

			if (hasAnyChanges)
			{
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			var plannedNonEventTasks = new List<PlannedCleaningTimelineItemData>();
			if (cleanerIdsToInsert.Any())
			{
				var fromDate = plan.Date;
				var toDate = plan.Date.AddDays(1);
				var groupsMap = groupsToInsert.ToDictionary(g => g.CleanerId, g => g);

				var taskItems = 
				(
					await this._databaseContext
					.SystemTasks
					.Include(st => st.Actions)
					//.Include(st => st.FromRoom)
					.Where(st => st.UserId != null && cleanerIdsToInsert.Contains(st.UserId.Value) && st.StartsAt >= fromDate && st.StartsAt <= toDate)
					.ToArrayAsync()
				)
				.Select(st => new PlannedCleaningTimelineItemData
				{
					Id = st.Id.ToString(),
					IsClean = true,
					IsDoNotDisturb = false,
					IsOccupied = false,
					IsOutOfOrder = false,
					IsPostponed = false,
					IsRoomAssigned = false,
					Reservations = null,
					RoomId = Guid.Empty,
					Tasks = null,
					TaskDescription = String.Join(", ", st.Actions.Select(a => $"{a.ActionName} {a.AssetQuantity}x{a.AssetName}").ToArray()),
					ItemTypeKey = "TASK",
					Title = (st.FromName == null ? "" : st.FromName + " ") + (st.ToName == null ? "" : st.ToName),
					CleaningPlanGroupId = groupsMap[st.UserId.Value].Id.ToString(),
					End = st.StartsAt,
					Start = st.StartsAt,
					Price = st.Price,
					Credits = st.Credits,
					IsTaskHighPriority = st.PriorityKey == "HIGH",
					IsTaskLowPriority = st.PriorityKey == "LOW",
					IsTaskGuestRequest = st.IsGuestRequest,
					CleaningPluginId = null,
					CleaningPluginName = null,
				}).ToArray();

				if (taskItems.Any())
				{
					foreach (var ti in taskItems)
					{
						ti.Reservations = new CleaningTimelineItemReservationData[0];
						ti.Tasks = new CleaningTimelineItemTaskData[0];
					}

					plannedNonEventTasks.AddRange(taskItems);
				}
			}

			var response = new ProcessResponse<AddRemoveCleaningPlanGroupsResult>
			{
				Data = new AddRemoveCleaningPlanGroupsResult
				{
					HasAnyChanges = hasAnyChanges,
					PlannedNonEventTasks = plannedNonEventTasks,
					InsertedGroups = groupsToInsert.Select(g => new CleaningTimelineGroupData
					{
						Cleaner = new CleanerData
						{
							Id = g.CleanerId,
							Affinities = new AffinityData[0],
							MaxCredits = g.MaxCredits,
							MaxDepartures = g.MaxDepartures,
							MaxTwins = g.MaxTwins,
							MustFillAllCredits = g.MustFillAllCredits,
							Name = insertedGroupUsers[g.CleanerId].FirstName + " " + insertedGroupUsers[g.CleanerId].LastName,
							AvailabilityIntervals = g.AvailabilityIntervals.Select(a => new TimeIntervalData { Id = a.Id, FromTimeString = a.From.ToString(@"hh\:mm"), ToTimeString = a.To.ToString(@"hh\:mm") }).ToArray(),
							Username = insertedGroupUsers[g.CleanerId].UserName,
							WeekHours = g.WeeklyHours
						},
						HasSecondaryCleaner = g.SecondaryCleanerId.HasValue,
						SecondaryCleaner = null,
						Id = g.Id,
						SecondaryCleanerId = g.SecondaryCleanerId
					}).ToArray()
				},
				HasError = false,
				IsSuccess = true,
				Message = "Groups changed."
			};

			return response;
		}
	}
}
