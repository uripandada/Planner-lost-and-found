using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.FloorAffinities.Queries.GetListOfFloorAffinities;
using Planner.Application.Interfaces;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Helpers;
using Planner.Common.Infrastructure;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Queries.GetCleaningPlanDetails
{
	// Cleaning plan elements:
	//	- Cleaners - list of all available cleaners
	//	- Planned Groups - each group represents a cleaner(s)
	//	- Planned Items - specific cleanings for group, room, date and time
	//	- Plannable Items - cleanings that are not yet assigned to a group
	//
	// Cleaning plan loads differently for different dates.
	// If the plan is loaded for today the room statuses are the ones current in the system - stored with the room itself, current transactional data
	// If the plan is loaded for a future date the room statuses are assumed based on reservations on that date
	//
	// Planned and calculated cleanings are matched by the key that has a format "{ROOM-NAME}_{CLEANING-DATE}_{CLEANING-INDEX}"
	// Cleaning index is just an index of a cleaning for a room on a specific date. If the room has 2 cleanings for a date, index has 1 and 2 as possible values.
	// Used to uniquely identify cleanings in order to match and update.

	public class CleaningPlanData
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public bool IsSent { get; set; }
		public DateTime? SentAt { get; set; }
		public Guid? SentById { get; set; }
		public string SentByFullName { get; set; }
		public CleaningPlanOptions Options { get; set; }
		public CpsatPlannerConfigurationData CpsatConfiguration { get; set; }
		public IEnumerable<PlannedCleaningTimelineItemData> PlannedNonEventTasks { get; set; }
		public IEnumerable<PlannedCleaningTimelineItemData> PlannedCleanings { get; set; }
		public IEnumerable<CleaningTimelineGroupData> PlannedGroups { get; set; }

		public IEnumerable<CleaningTimelineItemData> PlannableCleanings { get; set; }


		public bool IsNewPlan { get; set; }
	}

	public class CpsatPlannerConfigurationData
	{
		public Guid? Id { get; set; }
		public string PlanningStrategyTypeKey { get; set; } // BALANCE_BY_ROOMS, BALANCE_BY_CREDITS_STRICT, BALANCE_BY_CREDITS_WITH_AFFINITIES, TARGET_BY_ROOMS, TARGET_BY_CREDITS
		public int BalanceByRoomsMinRooms { get; set; }
		public int BalanceByRoomsMaxRooms { get; set; }
		public int BalanceByCreditsStrictMinCredits { get; set; }
		public int BalanceByCreditsStrictMaxCredits { get; set; }
		public int BalanceByCreditsWithAffinitiesMinCredits { get; set; }
		public int BalanceByCreditsWithAffinitiesMaxCredits { get; set; }
		public string TargetByRoomsValue { get; set; } // value is set if PlanningStrategyTypeKey = TARGET_BY_ROOMS
		public string TargetByCreditsValue { get; set; } // value is set if PlanningStrategyTypeKey = TARGET_BY_CREDITS

		public int MaxTravelTime { get; set; }
		public int MaxBuildingTravelTime { get; set; }
		public int MaxNumberOfBuildingsPerAttendant { get; set; }
		public int MaxNumberOfLevelsPerAttendant { get; set; }

		public int RoomAward { get; set; }
		public int LevelAward { get; set; }
		public int BuildingAward { get; set; }

		public int WeightTravelTime { get; set; }
		public int WeightCredits { get; set; }
		public int WeightRoomsCleaned { get; set; }
		public int WeightLevelChange { get; set; } = -1;
		public bool LimitAttendantsPerLevel { get; set; } = false;


		public int SolverRunTime { get; set; }

		public bool DoesLevelMovementReduceCredits { get; set; }
		public int ApplyLevelMovementCreditReductionAfterNumberOfLevels { get; set; }
		public int LevelMovementCreditsReduction { get; set; }

		public bool DoUsePrePlan { get; set; }
		public bool DoUsePreAffinity { get; set; }
		public bool DoCompleteProposedPlanOnUsePreplan { get; set; }

		public bool DoesBuildingMovementReduceCredits { get; set; }
		public int BuildingMovementCreditsReduction { get; set; }

		public bool ArePreferredLevelsExclusive { get; set; }

		public string CleaningPriorityKey { get; set; }

		public string BuildingsDistanceMatrix { get; set; }
		public string LevelsDistanceMatrix { get; set; }




		public bool DoBalanceStaysAndDepartures { get; set; }
		public int WeightEpsilonStayDeparture { get; set; }
		public int MaxStay { get; set; }
		public int MaxDeparture { get; set; }
		public bool MaxDeparturesReducesCredits { get; set; }
		public int MaxDeparturesEquivalentCredits { get; set; }
		public int MaxDeparturesReductionThreshold { get; set; }

		public bool MaxStaysIncreasesCredits { get; set; }
		public int MaxStaysEquivalentCredits { get; set; }
		public int MaxStaysIncreaseThreshold { get; set; }

		public decimal MinutesPerCredit { get; set; }
		public int MinCreditsForMultipleCleanersCleaning { get; set; }
	}

	public class CleanerData
	{
		public CleanerData()
		{
			this.Affinities = new AffinityData[0];
			this.AvailabilityIntervals = new TimeIntervalData[0];
		}
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Username { get; set; }
		public string GroupName { get; set; }
		public string SubGroupName { get; set; }
		public decimal? WeekHours { get; set; }
		public AffinityData[] Affinities { get; set; }
		public int? MaxCredits { get; set; }
		public int? MaxTwins { get; set; }
		public int? MaxDepartures { get; set; }
		public bool MustFillAllCredits { get; set; }
		public TimeIntervalData[] AvailabilityIntervals { get; set; }
		public string AvatarUrl { get; set; }
		public string FullNameInitials { get; set; }
	}

	public class AffinityData
	{
		public string ReferenceId { get; set; }
		public string ReferenceName { get; set; }
		public string ReferenceDescription { get; set; }
		public CleaningPlanGroupAffinityType Type { get; set; }
	}

	public class CleaningTimelineGroupData
	{
		public Guid Id { get; set; }
		public CleanerData Cleaner { get; set; }
		public bool HasSecondaryCleaner { get; set; }
		public Guid? SecondaryCleanerId { get; set; }
		public CleanerData SecondaryCleaner { get; set; }
	}

	public class TimeIntervalData
	{
		public Guid Id { get; set; }
		public string FromTimeString { get; set; }
		public string ToTimeString { get; set; }
	}

	/// <summary>
	/// Better name would be PlannableCleaningItem.
	/// This class is used to describe plannable cleanings.
	/// Plannable cleanings are all cleanings without assigned cleaner and specific time of cleaning.
	/// </summary>
	public class CleaningTimelineItemData
	{
		public string Id { get; set; }

		///// <summary>
		///// Cleaning timeline item keys are now deprecated.
		///// Plannable items are now generated and stored upon creation of the plan so the Key is not required any more.
		///// </summary>
		//public string Key { get; set; }

		public string Title { get; set; }

		public bool HasArrival { get; set; }
		public bool HasDeparture { get; set; }
		public bool HasStay { get; set; }
		public bool HasVipReservation { get; set; }
		public Guid? RoomCategoryId { get; set; }
		public string RoomCategoryName { get; set; }

		public IEnumerable<string> VipValues { get; set; }


		public bool IsPostponed { get; set; }
		public bool IsCustom { get; set; }
		public bool IsActive { get; set; }
		public Guid RoomId { get; set; }
		public Guid? BedId { get; set; }
		//public string ReservationId { get; set; }


		public string ItemTypeKey { get; set; }
		public string TaskDescription { get; set; }

		public bool IsOccupied { get; set; }
		public bool IsClean { get; set; }
		public bool IsRoomAssigned { get; set; }
		public bool IsOutOfOrder { get; set; }
		public bool IsDoNotDisturb { get; set; }

		public bool IsChangeSheets { get; set; }
		public bool IsPriority { get; set; }

		public bool IsTaskGuestRequest { get; set; }
		public bool IsTaskHighPriority { get; set; }
		public bool IsTaskLowPriority { get; set; }


		public decimal Price { get; set; }
		public int? Credits { get; set; }

		public Guid? CleaningPluginId { get; set; }
		public string CleaningPluginName { get; set; }

		public IEnumerable<CleaningTimelineItemReservationData> Reservations { get; set; }
		public IEnumerable<CleaningTimelineItemTaskData> Tasks { get; set; }

		/// <summary>
		/// Planned attendant tasks are the tasks that don't have a cleaner assigned yet.
		/// Cleaner is assigned at the point of sending the plan to the cleaners.
		/// Chosen cleaner is the one that gets this cleaning.
		/// </summary>
		public IEnumerable<CleaningTimelineItemTaskData> PlannedAttendantTasks { get; set; }

		//public IEnumerable<string> ReservationStatuses { get; set; } // Just a list of ARR/DEP/STAY strings.
		public string CleaningDescription { get; set; }


		public Guid? BuildingId { get; set; }
		public string BuildingName { get; set; }
		public Guid? FloorId { get; set; }
		public string FloorName { get; set; }
		public int? FloorNumber { get; set; }
		public string FloorSectionName { get; set; }
		public string FloorSubSectionName { get; set; }
		public string HotelId { get; set; }
		public string HotelName { get; set; }
		public Guid? AreaId { get; set; }
		public string AreaName { get; set; }



		/// <summary>
		/// After the cleaning is sent to the cleaner. Maybe a better name would be IsLocked or something. This flag is important one.
		/// </summary>
		public bool IsSent { get; set; }

		/// <summary>
		/// Describes the lifecycle status of the cleaning.
		/// </summary>
		public CleaningProcessStatus CleaningStatus { get; set; }

		/// <summary>
		/// If the inspection is required, the cleaning process is not really finished until the inspection is finished also.
		/// </summary>
		public bool IsInspectionRequired { get; set; }

		/// <summary>
		/// After the cleaner is done cleaning, this flag is set to true only if the inspection is required.
		/// </summary>
		public bool IsReadyForInspection { get; set; }

		/// <summary>
		/// Set to true after the inspector finishes the inspection.
		/// </summary>
		public bool IsInspected { get; set; }

		/// <summary>
		/// Id of the inspector.
		/// </summary>
		public Guid? InspectedById { get; set; }
		public string InspectedByFullName { get; set; }

		/// <summary>
		/// Set to true only if the inspaction passed successfully.
		/// </summary>
		public bool IsInspectionSuccess { get; set; }


		/// <summary>
		/// This property is used only by the frontend. The backend will always set this as false.
		/// </summary>
		public bool IsFilteredOut { get; set; }

		public string BorderColorHex { get; set; }
	}

	public class CleaningTimelineItemReservationData
	{
		public string ReservationId { get; set; }
		public string ReservationStatus { get; set; }
		public string ReservationStatusKey { get; set; }
		public string TimeString { get; set; }
		public string StyleCode { get; set; }
		public string GuestName { get; set; }
		public string TypeAndTimeTag { get; set; }
		public bool IsVip { get; set; }
		public string VipTag { get; set; }
		public bool IsDayUse { get; set; }
	}

	public class ExtendedCleaningTimelineItemReservationData : CleaningTimelineItemReservationData
	{
		public Guid RoomId { get; set; }
	}

	public class CleaningTimelineItemTaskActionData
	{
		public string ActionName { get; set; }
		public string AssetName { get; set; }
		public string AssetQuantity { get; set; }
	}

	public class CleaningTimelineItemTaskData
	{
		public IEnumerable<CleaningTimelineItemTaskActionData> Actions { get; set; }

		public string TaskId { get; set; }
		public int DurationMinutes { get; set; }

		public bool IsCompleted { get; set; }
		public string StatusKey { get; set; }

		public bool IsForPlannedAttendant { get; set; }
		public Guid? UserId { get; set; }
		public string UserFullName { get; set; }
		public string FromReferenceId { get; set; }
		public string FromReferenceName { get; set; }
		public string FromReferenceTypeKey { get; set; }
		public string ToReferenceId { get; set; }
		public string ToReferenceName { get; set; }
		public string ToReferenceTypeKey { get; set; }
	}

	public class PlannedCleaningTimelineItemData : CleaningTimelineItemData
	{
		public string CleaningPlanGroupId { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }

	}

	public class CleaningPlanOptions
	{
		public string DefaultAttendantStartTime { get; set; }
		public string DefaultAttendantEndTime { get; set; }
	}

	public class GetCleaningPlanDetailsQuery : IRequest<CleaningPlanData>
	{
		public string HotelId { get; set; }
		public Guid? Id { get; set; }
		public string CleaningDateString { get; set; }
		public bool IsTodaysCleaningPlan { get; set; }
	}

	public class GetCleaningPlanDetailsQueryHandler : IRequestHandler<GetCleaningPlanDetailsQuery, CleaningPlanData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ICleaningProvider _cleaningProvider;
		private readonly ICleaningGeneratorService _cleaningGeneratorService;

		public GetCleaningPlanDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, ICleaningProvider cleaningProvider, ICleaningGeneratorService cleaningGeneratorService)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._cleaningProvider = cleaningProvider;
			this._cleaningGeneratorService = cleaningGeneratorService;
		}

		public async Task<CleaningPlanData> Handle(GetCleaningPlanDetailsQuery request, CancellationToken cancellationToken)
		{
			var from = DateTimeHelper.ParseIsoDate(request.CleaningDateString);
			var planFromDate = from.Date;
			var planToDate = planFromDate.AddDays(1);

			// Cleaning plan exists?
			// - NO - Create, initilize and save cleaning plan
			// - YES - Load cleaning plan
			//
			// Warning - cleaning plan can be requested by the cleaning plan date or by the cleaning plan ID
			// If the ID is set and the plan doesn't exist, the plan SHOULD NOT be created!
			// If the ID is not set and the plan doesn't exist for the date, the plan SHOULD be created.

			var cleaningPlanResult = await this._GetInitializedCleaningPlanHeader(request.HotelId, request.Id, planFromDate);
			var cleaningPlan = cleaningPlanResult.Plan;

			var cleaningPlanResponse = new CleaningPlanData
			{
				Id = cleaningPlan.Id,
				IsNewPlan = cleaningPlanResult.IsNew,
				Date = cleaningPlan.Date,
				IsSent = cleaningPlan.IsSent,
				SentAt = cleaningPlan.SentAt,
				SentById = cleaningPlan.SentById,
				SentByFullName = $"{cleaningPlan.SentBy?.FirstName} {cleaningPlan.SentBy?.LastName}",
				Options = new CleaningPlanOptions
				{
					DefaultAttendantStartTime = "08:00",
					DefaultAttendantEndTime = "16:00"
				},
				PlannedGroups = new CleaningTimelineGroupData[0],
				PlannedCleanings = new PlannedCleaningTimelineItemData[0],
				PlannedNonEventTasks = new PlannedCleaningTimelineItemData[0],
				PlannableCleanings = new CleaningTimelineItemData[0],
				CpsatConfiguration = null
			};

			cleaningPlanResponse.CpsatConfiguration = new CpsatPlannerConfigurationData
			{
				Id = cleaningPlan.CleaningPlanCpsatConfiguration.Id,
				ApplyLevelMovementCreditReductionAfterNumberOfLevels = cleaningPlan.CleaningPlanCpsatConfiguration.ApplyLevelMovementCreditReductionAfterNumberOfLevels,
				ArePreferredLevelsExclusive = cleaningPlan.CleaningPlanCpsatConfiguration.ArePreferredLevelsExclusive,
				BalanceByCreditsStrictMaxCredits = cleaningPlan.CleaningPlanCpsatConfiguration.BalanceByCreditsStrictMaxCredits,
				BalanceByCreditsStrictMinCredits = cleaningPlan.CleaningPlanCpsatConfiguration.BalanceByCreditsStrictMinCredits,
				BalanceByCreditsWithAffinitiesMaxCredits = cleaningPlan.CleaningPlanCpsatConfiguration.BalanceByCreditsWithAffinitiesMaxCredits,
				BalanceByCreditsWithAffinitiesMinCredits = cleaningPlan.CleaningPlanCpsatConfiguration.BalanceByCreditsWithAffinitiesMinCredits,
				BalanceByRoomsMaxRooms = cleaningPlan.CleaningPlanCpsatConfiguration.BalanceByRoomsMaxRooms,
				BalanceByRoomsMinRooms = cleaningPlan.CleaningPlanCpsatConfiguration.BalanceByRoomsMinRooms,
				BuildingAward = cleaningPlan.CleaningPlanCpsatConfiguration.BuildingAward,
				BuildingMovementCreditsReduction = cleaningPlan.CleaningPlanCpsatConfiguration.BuildingMovementCreditsReduction,
				BuildingsDistanceMatrix = cleaningPlan.CleaningPlanCpsatConfiguration.BuildingsDistanceMatrix,
				CleaningPriorityKey = cleaningPlan.CleaningPlanCpsatConfiguration.CleaningPriorityKey,
				WeightCredits = cleaningPlan.CleaningPlanCpsatConfiguration.WeightCredits,
				WeightRoomsCleaned = cleaningPlan.CleaningPlanCpsatConfiguration.WeightRoomsCleaned,
				WeightLevelChange = cleaningPlan.CleaningPlanCpsatConfiguration.WeightLevelChange,
				DoBalanceStaysAndDepartures = cleaningPlan.CleaningPlanCpsatConfiguration.DoBalanceStaysAndDepartures,
				DoCompleteProposedPlanOnUsePreplan = cleaningPlan.CleaningPlanCpsatConfiguration.DoCompleteProposedPlanOnUsePreplan,
				DoesBuildingMovementReduceCredits = cleaningPlan.CleaningPlanCpsatConfiguration.DoesBuildingMovementReduceCredits,
				DoesLevelMovementReduceCredits = cleaningPlan.CleaningPlanCpsatConfiguration.DoesLevelMovementReduceCredits,
				DoUsePreAffinity = cleaningPlan.CleaningPlanCpsatConfiguration.DoUsePreAffinity,
				DoUsePrePlan = cleaningPlan.CleaningPlanCpsatConfiguration.DoUsePrePlan,
				LevelAward = cleaningPlan.CleaningPlanCpsatConfiguration.LevelAward,
				LevelMovementCreditsReduction = cleaningPlan.CleaningPlanCpsatConfiguration.LevelMovementCreditsReduction,
				LevelsDistanceMatrix = cleaningPlan.CleaningPlanCpsatConfiguration.LevelsDistanceMatrix,
				MaxNumberOfBuildingsPerAttendant = cleaningPlan.CleaningPlanCpsatConfiguration.MaxNumberOfBuildingsPerAttendant,
				MaxBuildingTravelTime = cleaningPlan.CleaningPlanCpsatConfiguration.MaxBuildingTravelTime,
				MaxDeparture = cleaningPlan.CleaningPlanCpsatConfiguration.MaxDeparture,
				MaxNumberOfLevelsPerAttendant = cleaningPlan.CleaningPlanCpsatConfiguration.MaxNumberOfLevelsPerAttendant,
				MaxStay = cleaningPlan.CleaningPlanCpsatConfiguration.MaxStay,
				MaxTravelTime = cleaningPlan.CleaningPlanCpsatConfiguration.MaxTravelTime,
				PlanningStrategyTypeKey = cleaningPlan.CleaningPlanCpsatConfiguration.PlanningStrategyTypeKey,
				RoomAward = cleaningPlan.CleaningPlanCpsatConfiguration.RoomAward,
				SolverRunTime = cleaningPlan.CleaningPlanCpsatConfiguration.SolverRunTime,
				TargetByCreditsValue = cleaningPlan.CleaningPlanCpsatConfiguration.TargetByCreditsValue,
				TargetByRoomsValue = cleaningPlan.CleaningPlanCpsatConfiguration.TargetByRoomsValue,
				WeightTravelTime = cleaningPlan.CleaningPlanCpsatConfiguration.WeightTravelTime,
				WeightEpsilonStayDeparture = cleaningPlan.CleaningPlanCpsatConfiguration.WeightEpsilonStayDeparture,

				MinutesPerCredit = cleaningPlan.CleaningPlanCpsatConfiguration.MinutesPerCredit,
				MinCreditsForMultipleCleanersCleaning = cleaningPlan.CleaningPlanCpsatConfiguration.MinCreditsForMultipleCleanersCleaning,
				MaxDeparturesEquivalentCredits = cleaningPlan.CleaningPlanCpsatConfiguration.MaxDeparturesEquivalentCredits,
				MaxDeparturesReducesCredits = cleaningPlan.CleaningPlanCpsatConfiguration.MaxDeparturesReducesCredits,
				MaxStaysIncreasesCredits = cleaningPlan.CleaningPlanCpsatConfiguration.MaxStaysIncreasesCredits,
				MaxDeparturesReductionThreshold = cleaningPlan.CleaningPlanCpsatConfiguration.MaxDeparturesReductionThreshold,
				MaxStaysEquivalentCredits = cleaningPlan.CleaningPlanCpsatConfiguration.MaxStaysEquivalentCredits,
				LimitAttendantsPerLevel = cleaningPlan.CleaningPlanCpsatConfiguration.LimitAttendantsPerLevel,
				MaxStaysIncreaseThreshold = cleaningPlan.CleaningPlanCpsatConfiguration.MaxStaysIncreaseThreshold,
			};

			// Load the plan data - different for todays plan than future/past dates.
			// Loading plan data also depends whether the plan is newly created or not.

			//
			//	If NEW plan
			//		Create cleanings based on hotel plugins
			//		Read room HK statuses from current state if the plan is for today
			//		Predict room HK statuses from Reservations if the plan is not for today
			//
			//	If EXISTING plan
			//		Load data from the DB
			//		Read room HK statuses from current state if the plan is for today
			//		Predict room HK statuses from Reservations if the plan is not for today
			//

			// New cleaning plan was inserted
			if (cleaningPlanResult.IsNew)
			{
				var cleanings = await this._cleaningGeneratorService.GenerateCleanings(request.HotelId, request.IsTodaysCleaningPlan, planFromDate);

				cleaningPlanResponse.PlannableCleanings = cleanings;
				var plannableCleanings = this._GenerateCleaningPlanItems(cleaningPlanResponse.Id, cleaningPlanResponse.PlannableCleanings);

				await this._databaseContext.CleaningPlanItems.AddRangeAsync(plannableCleanings, cancellationToken);
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}
			// A cleaning plan already existed
			else
			{
				cleaningPlanResponse.PlannedGroups = await this._LoadCleaningPlanGroups(cleaningPlan.Id);

				var userIds = cleaningPlanResponse.PlannedGroups.SelectMany(g =>
				{
					var cleanerIds = new List<Guid>();
					cleanerIds.Add(g.Cleaner.Id);
					if (g.HasSecondaryCleaner)
					{
						cleanerIds.Add(g.SecondaryCleanerId.Value);
					}
					return cleanerIds;
				}).ToArray();

				var roomsMap = await this._databaseContext.Rooms.GetRoomsWithStructureAndActiveReservationsQuery(request.HotelId, planFromDate, null, null, null, null, null, false, false, true).ToDictionaryAsync(r => r.Id, r => r);

				var allTasks = await this._LoadTasks(request.HotelId, planFromDate, planToDate);

				var cleaningTasksMap = new Dictionary<Guid, List<CleaningTimelineItemTaskData>>();
				var plannedAttendantTasksMap = new Dictionary<Guid, List<CleaningTimelineItemTaskData>>();
				var nonCleaningTasks = new List<PlannedCleaningTimelineItemData>();
				foreach (var task in allTasks)
				{
					// If the task is the cleaning event task. ***Tasks are prefiltered in the query to contain only cleaning event tasks.
					if (task.TypeKey == TaskType.EVENT.ToString())
					{
						var timelineItemTaskData = this._CreateTimelineItemTaskData(task);

						if (task.FromRoomId.HasValue)
						{
							if (!cleaningTasksMap.ContainsKey(task.FromRoomId.Value))
							{
								cleaningTasksMap.Add(task.FromRoomId.Value, new List<CleaningTimelineItemTaskData>());
							}
							cleaningTasksMap[task.FromRoomId.Value].Add(timelineItemTaskData);
						}

						if (task.ToRoomId.HasValue)
						{
							if (!cleaningTasksMap.ContainsKey(task.ToRoomId.Value))
							{
								cleaningTasksMap.Add(task.ToRoomId.Value, new List<CleaningTimelineItemTaskData>());
							}

							if (cleaningTasksMap[task.ToRoomId.Value].Any(t => t.TaskId == timelineItemTaskData.TaskId))
							{
								// The task is already added
								continue;
							}

							cleaningTasksMap[task.ToRoomId.Value].Add(timelineItemTaskData);
						}
					}
					else // Oterwise it is just a task for the same date as the cleaning plan for the hotel.
					{
						if (task.IsForPlannedAttendant)
						{
							var timelineItemTaskData = this._CreateTimelineItemTaskData(task);

							if (task.FromRoomId.HasValue)
							{
								if (!plannedAttendantTasksMap.ContainsKey(task.FromRoomId.Value))
								{
									plannedAttendantTasksMap.Add(task.FromRoomId.Value, new List<CleaningTimelineItemTaskData>());
								}
								plannedAttendantTasksMap[task.FromRoomId.Value].Add(timelineItemTaskData);
							}

							if (task.ToRoomId.HasValue)
							{
								if (!plannedAttendantTasksMap.ContainsKey(task.ToRoomId.Value))
								{
									plannedAttendantTasksMap.Add(task.ToRoomId.Value, new List<CleaningTimelineItemTaskData>());
								}

								if (plannedAttendantTasksMap[task.ToRoomId.Value].Any(t => t.TaskId == timelineItemTaskData.TaskId))
								{
									// The task is already added
									continue;
								}

								plannedAttendantTasksMap[task.ToRoomId.Value].Add(timelineItemTaskData);
							}
						}
						else
						{
							if (!task.UserId.HasValue)
							{
								// Task is not assigned to a user. It must be assigned to a user first in order to display it on the cleaning plan.
								continue;
							}

							var cleaningGroup = cleaningPlanResponse.PlannedGroups.Where(g => g.Cleaner.Id == task.UserId.Value).FirstOrDefault();

							if (cleaningGroup == null)
							{
								// Task is not for any of the planned cleaners. This is filtered in code just to make the query faster.
								continue;
							}

							var fromRoomName = task.FromRoomId.HasValue && roomsMap.ContainsKey(task.FromRoomId.Value) ? roomsMap[task.FromRoomId.Value].Name : "No room";
							var toRoomName = task.ToRoomId.HasValue && roomsMap.ContainsKey(task.ToRoomId.Value) ? roomsMap[task.ToRoomId.Value].Name : "No room";
							var roomName = (fromRoomName.IsNotNull() ? $"From {fromRoomName} " : "") + (toRoomName.IsNotNull() ? $"To {toRoomName} " : "");

							nonCleaningTasks.Add(this._CreatePlannedNonEventTaskData(task, cleaningGroup == null ? Guid.Empty : cleaningGroup.Id, roomName));
						}
					}
				}

				cleaningPlanResponse.PlannedNonEventTasks = nonCleaningTasks;

				var planItems = await this._LoadCleaningPlanItems(cleaningPlan.Id, cleaningPlan.HotelId, plannedAttendantTasksMap);

				cleaningPlanResponse.PlannedCleanings = planItems.plannedItems;
				cleaningPlanResponse.PlannableCleanings = planItems.plannableItems;

				var hotel = await this._databaseContext.Hotels.FindAsync(cleaningPlan.HotelId);
				var timeZoneId = Infrastructure.HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
				var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
				var cleaningDateUtc = TimeZoneInfo.ConvertTimeToUtc(planFromDate, timeZoneInfo);

				foreach (var cleaning in cleaningPlanResponse.PlannedCleanings)
				{
					var room = roomsMap[cleaning.RoomId];
					var bed = cleaning.BedId.HasValue ? room.RoomBeds.FirstOrDefault(b => b.Id == cleaning.BedId.Value) : null;

					cleaning.Title = bed == null ? room.Name : $"{room.Name} - {bed.Name}";
					cleaning.RefreshCleaningStatus(cleaningDateUtc, timeZoneId, room);
				}

				foreach (var cleaning in cleaningPlanResponse.PlannableCleanings)
				{
					var room = roomsMap[cleaning.RoomId];
					var bed = cleaning.BedId.HasValue ? room.RoomBeds.FirstOrDefault(b => b.Id == cleaning.BedId.Value) : null;

					cleaning.Title = bed == null ? room.Name : $"{room.Name} - {bed.Name}";
					cleaning.RefreshCleaningStatus(cleaningDateUtc, timeZoneId, room);
				}

				cleaningPlanResponse.PlannableCleanings = cleaningPlanResponse.PlannableCleanings.OrderBy(c => c.Title).ToArray();
			}

			return cleaningPlanResponse;
		}

		private class GetInsertCleaningPlanHeaderResult
		{
			public CleaningPlan Plan { get; set; }
			public bool IsNew { get; set; }
		}

		private async Task<GetInsertCleaningPlanHeaderResult> _GetInitializedCleaningPlanHeader(string hotelId, Guid? cleaningPlanId, DateTime cleaningDate)
		{
			var cleaningPlan = (CleaningPlan)null;
			var isNewPlan = true;

			if (cleaningPlanId.HasValue)
			{
				cleaningPlan = await this._LoadCleaningPlanById(hotelId, cleaningPlanId.Value);
				isNewPlan = false;

				// if the cleaning plan doesn't exist and is requested by ID the exception is thrown
				if (cleaningPlan == null)
				{
					throw new Exception($"CleaningPlanDetailsQueryHandler: Unable to find cleaning plan by id. HotelID: {hotelId} ## ID: {cleaningPlanId.Value}");
				}
			}
			else
			{
				// The transaction is required in order to prevent chance for double cleaning plan per date per hotel.
				using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
				{
					cleaningPlan = await this._LoadCleaningPlanByDate(hotelId, cleaningDate);

					if (cleaningPlan == null)
					{
						cleaningPlan = await this._InsertNewCleaningPlan(hotelId, cleaningDate, this._httpContextAccessor.UserId());
						await this._databaseContext.SaveChangesAsync(CancellationToken.None);
						await transaction.CommitAsync();
						isNewPlan = true;
					}
					else
					{
						isNewPlan = false;
					}
				}
			}

			if(cleaningPlan.CleaningPlanCpsatConfiguration == null)
			{
				var config = await this._LoadDefaultCpsatConfiguration(hotelId, cleaningPlan.Id);
				await this._databaseContext.CleaningPlanCpsatConfigurations.AddAsync(config);
				await this._databaseContext.SaveChangesAsync(CancellationToken.None);

				cleaningPlan.CleaningPlanCpsatConfiguration = config;
			}

			var hotelSettings = await this._databaseContext
				   .Settings
				   .Where(s => s.HotelId == hotelId)
				   .FirstOrDefaultAsync();

			cleaningPlan.CleaningPlanCpsatConfiguration.BuildingsDistanceMatrix = hotelSettings.BuildingsDistanceMatrix;
			cleaningPlan.CleaningPlanCpsatConfiguration.LevelsDistanceMatrix = hotelSettings.LevelsDistanceMatrix;
			cleaningPlan.CleaningPlanCpsatConfiguration.WeightCredits = hotelSettings.WeightCredits;
			cleaningPlan.CleaningPlanCpsatConfiguration.WeightLevelChange = hotelSettings.WeightLevelChange;
			cleaningPlan.CleaningPlanCpsatConfiguration.MinCreditsForMultipleCleanersCleaning = hotelSettings.MinCreditsForMultipleCleanersCleaning;
			cleaningPlan.CleaningPlanCpsatConfiguration.MinutesPerCredit = hotelSettings.MinutesPerCredit;

			return new GetInsertCleaningPlanHeaderResult
			{
				Plan = cleaningPlan,
				IsNew = isNewPlan
			};
		}

		private async Task<CleaningPlanCpsatConfiguration> _LoadDefaultCpsatConfiguration(string hotelId, Guid cleaningPlanId)
		{
			var c = CpsatConfigurationProvider.GetDefaultCpsatPlannerConfiguration();
			var hotelSettings = await this._databaseContext
				   .Settings
				   .Where(s => s.HotelId == hotelId)
				   .FirstOrDefaultAsync();

			var config = new Domain.Entities.CleaningPlanCpsatConfiguration
			{
				Id = cleaningPlanId,
				ApplyLevelMovementCreditReductionAfterNumberOfLevels = c.ApplyLevelMovementCreditReductionAfterNumberOfLevels,
				ArePreferredLevelsExclusive = c.ArePreferredLevelsExclusive,
				BalanceByCreditsStrictMaxCredits = c.BalanceByCreditsStrictMaxCredits,
				BalanceByCreditsStrictMinCredits = c.BalanceByCreditsStrictMinCredits,
				BalanceByCreditsWithAffinitiesMaxCredits = c.BalanceByCreditsWithAffinitiesMaxCredits,
				BalanceByCreditsWithAffinitiesMinCredits = c.BalanceByCreditsWithAffinitiesMinCredits,
				BalanceByRoomsMaxRooms = c.BalanceByRoomsMaxRooms,
				BalanceByRoomsMinRooms = c.BalanceByRoomsMinRooms,
				BuildingAward = c.BuildingAward,
				BuildingMovementCreditsReduction = c.BuildingMovementCreditsReduction,
				CleaningPriorityKey = c.CleaningPriorityKey,
				WeightRoomsCleaned = c.WeightRoomsCleaned,
				DoBalanceStaysAndDepartures = c.DoBalanceStaysAndDepartures,
				DoCompleteProposedPlanOnUsePreplan = c.DoCompleteProposedPlanOnUsePreplan,
				DoesBuildingMovementReduceCredits = c.DoesBuildingMovementReduceCredits,
				DoesLevelMovementReduceCredits = c.DoesLevelMovementReduceCredits,
				DoUsePreAffinity = c.DoUsePreAffinity,
				DoUsePrePlan = c.DoUsePrePlan,
				LevelAward = c.LevelAward,
				LevelMovementCreditsReduction = c.LevelMovementCreditsReduction,
				MaxNumberOfBuildingsPerAttendant = c.MaxNumberOfBuildingsPerAttendant,
				MaxBuildingTravelTime = c.MaxBuildingTravelTime,
				MaxDeparture = c.MaxDeparture,
				MaxNumberOfLevelsPerAttendant = c.MaxNumberOfLevelsPerAttendant,
				MaxStay = c.MaxStay,
				MaxTravelTime = c.MaxTravelTime,
				PlanningStrategyTypeKey = c.PlanningStrategyTypeKey,
				RoomAward = c.RoomAward,
				SolverRunTime = c.SolverRunTime,
				TargetByCreditsValue = c.TargetByCreditsValue,
				TargetByRoomsValue = c.TargetByRoomsValue,
				WeightTravelTime = c.WeightTravelTime,
				WeightEpsilonStayDeparture = c.WeightEpsilonStayDeparture,
				MaxStaysIncreaseThreshold = c.MaxStaysIncreaseThreshold,
				MaxStaysEquivalentCredits = c.MaxStaysEquivalentCredits,
				MaxStaysIncreasesCredits = c.MaxStaysIncreasesCredits,
				MaxDeparturesReductionThreshold = c.MaxDeparturesReductionThreshold,
				MaxDeparturesReducesCredits = c.MaxDeparturesReducesCredits,
				MaxDeparturesEquivalentCredits = c.MaxDeparturesEquivalentCredits,
				LimitAttendantsPerLevel = c.LimitAttendantsPerLevel,

				BuildingsDistanceMatrix = hotelSettings.BuildingsDistanceMatrix,
				LevelsDistanceMatrix = hotelSettings.LevelsDistanceMatrix,
				WeightCredits = hotelSettings.WeightCredits,
				WeightLevelChange = hotelSettings.WeightLevelChange,
				MinCreditsForMultipleCleanersCleaning = hotelSettings.MinCreditsForMultipleCleanersCleaning,
				MinutesPerCredit = hotelSettings.MinutesPerCredit,
			};

			return config;
		}

		private IEnumerable<CleaningPlanItem> _GenerateCleaningPlanItems(Guid cleaningPlanId, IEnumerable<CleaningTimelineItemData> cleanings)
		{
			return cleanings.Select(c => this._GenerateCleaningPlanItem(cleaningPlanId, c)).ToArray();
		}

		private CleaningPlanItem _GenerateCleaningPlanItem(Guid cleaningPlanId, CleaningTimelineItemData c)
		{
			var planItem = new CleaningPlanItem
			{
				Id = Guid.NewGuid(),
				CleaningPlanId = cleaningPlanId,
				CleaningPluginId = c.CleaningPluginId,
				Description = c.CleaningPluginName,
				Credits = c.Credits,
				IsActive = true,
				IsCustom = false,
				IsPostponed = false,
				RoomId = c.RoomId,
				IsChangeSheets = c.IsChangeSheets,
				CleaningPlanGroupId = null,
				DurationSec = null,
				StartsAt = null,
				EndsAt = null,
				RoomBedId = c.BedId,
			};

			c.Id = planItem.Id.ToString();

			return planItem;
		}

		private CleaningTimelineItemTaskData _CreateTimelineItemTaskData(SystemTask t)
		{
			var userFullName = "N/A";
			if (t.User != null)
			{
				userFullName = $"{t.User.FirstName} {t.User.LastName}";
			}
			else if (t.IsForPlannedAttendant)
			{
				userFullName = "Planned attendant";
			}

			var data = new CleaningTimelineItemTaskData
			{
				Actions = t.Actions.Select(a => new CleaningTimelineItemTaskActionData
				{
					ActionName = a.ActionName,
					AssetName = a.AssetName,
					AssetQuantity = a.AssetQuantity.ToString(),
				}).ToArray(),
				DurationMinutes = 0,
				TaskId = t.Id.ToString(),
				IsCompleted = t.StatusKey == TaskStatusType.FINISHED.ToString() || t.StatusKey == TaskStatusType.VERIFIED.ToString(),
				StatusKey = t.StatusKey,
				UserId = t.UserId,
				UserFullName = userFullName,
				IsForPlannedAttendant = t.IsForPlannedAttendant,
				FromReferenceId = "N/A",
				FromReferenceName = "N/A",
				FromReferenceTypeKey = "UNKNOWN",
				ToReferenceId = "N/A",
				ToReferenceName = "N/A",
				ToReferenceTypeKey = "UNKNOWN",
			};

			if (t.FromReservationId.IsNotNull())
			{
				data.FromReferenceId = t.FromReservationId;
				data.FromReferenceName = t.FromReservationId;
				data.FromReferenceTypeKey = "RESERVATION";
			}
			else if (t.FromWarehouseId.HasValue)
			{
				data.FromReferenceId = t.FromWarehouseId.Value.ToString();
				data.FromReferenceName = t.FromWarehouseId.Value.ToString();
				data.FromReferenceTypeKey = "WAREHOUSE";

			}
			else if (t.FromRoomId.HasValue)
			{
				data.FromReferenceId = t.FromRoomId.Value.ToString();
				data.FromReferenceName = t.FromRoomId.Value.ToString();
				data.FromReferenceTypeKey = "ROOM";
			}

			if (t.ToReservationId.IsNotNull())
			{
				data.ToReferenceId = t.ToReservationId;
				data.ToReferenceName = t.ToReservationId;
				data.ToReferenceTypeKey = "RESERVATION";
			}
			else if (t.ToWarehouseId.HasValue)
			{
				data.ToReferenceId = t.ToWarehouseId.Value.ToString();
				data.ToReferenceName = t.ToWarehouseId.Value.ToString();
				data.ToReferenceTypeKey = "WAREHOUSE";

			}
			else if (t.ToRoomId.HasValue)
			{
				data.ToReferenceId = t.ToRoomId.Value.ToString();
				data.ToReferenceName = t.ToRoomId.Value.ToString();
				data.ToReferenceTypeKey = "ROOM";
			}

			return data;
		}

		private PlannedCleaningTimelineItemData _CreatePlannedNonEventTaskData(SystemTask i, Guid groupId, string roomName)
		{
			return new PlannedCleaningTimelineItemData
			{
				Id = i.Id.ToString(),
				IsClean = true,
				IsDoNotDisturb = false,
				IsOccupied = false,
				IsOutOfOrder = false,
				IsPostponed = false,
				IsRoomAssigned = false,
				IsChangeSheets = false,
				Reservations = null,
				RoomId = Guid.Empty,
				Tasks = null,
				TaskDescription = string.Join(", ", i.Actions.Select(a => $"{a.ActionName} {a.AssetQuantity}x{a.AssetName}").ToArray()),
				ItemTypeKey = "TASK",
				Title = roomName,
				CleaningPlanGroupId = groupId.ToString(),
				End = i.StartsAt,
				Start = i.StartsAt,
				IsTaskGuestRequest = i.IsGuestRequest,
				IsTaskHighPriority = i.PriorityKey == "HIGH",
				IsTaskLowPriority = i.PriorityKey == "LOW",
				CleaningPluginId = null,
				IsActive = true,
				CleaningDescription = null,
				CleaningPluginName = null,
				Credits = 0,
				IsCustom = false,
				Price = 0,

				InspectedById = null,
				IsInspected = false,
				InspectedByFullName = null,
				IsInspectionRequired = false,
				IsInspectionSuccess = false,
				IsReadyForInspection = false,
				IsSent = false,
				CleaningStatus = CleaningProcessStatus.UNKNOWN,
			};
		}

		private async Task<CleaningPlan> _LoadCleaningPlanById(string hotelId, Guid id)
		{
			return await this._LoadCleaningPlan(hotelId, id, null);
		}

		private async Task<CleaningPlan> _LoadCleaningPlanByDate(string hotelId, DateTime date)
		{
			return await this._LoadCleaningPlan(hotelId, null, date);
		}

		private async Task<CleaningPlan> _LoadCleaningPlan(string hotelId, Guid? id, DateTime? date)
		{
			var query = this._databaseContext.CleaningPlans
				.Include(cp => cp.SentBy)
				.Include(cp => cp.CleaningPlanCpsatConfiguration)
				.Where(cp => cp.HotelId == hotelId).AsQueryable();

			if (id.HasValue)
			{
				query = query.Where(cp => cp.Id == id.Value);
			}
			else if (date.HasValue)
			{
				query = query.Where(cp => cp.Date == date);
			}

			return await query.FirstOrDefaultAsync();
		}

		private async Task<IEnumerable<CleaningTimelineGroupData>> _LoadCleaningPlanGroups(Guid? cleaningPlanId)
		{
			var groups = await this._databaseContext.CleaningPlanGroups
				.Where(g => g.CleaningPlanId == cleaningPlanId)
				.Select(g => new CleaningTimelineGroupData
				{
					Id = g.Id,
					Cleaner = new CleanerData
					{
						Id = g.CleanerId,
						Affinities = g.Affinities.Select(f => new AffinityData
						{
							ReferenceDescription = f.ReferenceId,
							ReferenceId = f.ReferenceId,
							Type = f.AffinityType,
							ReferenceName = f.ReferenceId,
						}).ToArray(),
						MaxCredits = g.MaxCredits,
						MaxDepartures = g.MaxDepartures,
						MaxTwins = g.MaxTwins,
						MustFillAllCredits = g.MustFillAllCredits,
						Name = g.Cleaner.FirstName + " " + g.Cleaner.LastName,
						AvailabilityIntervals = g.AvailabilityIntervals.Select(i => new TimeIntervalData
						{
							Id = i.Id,
							FromTimeString = i.From.ToString(@"hh\:mm"),
							ToTimeString = i.To.ToString(@"hh\:mm"),
						}).ToArray(),
						Username = g.Cleaner.UserName,
						WeekHours = g.WeeklyHours,
						AvatarUrl = g.Cleaner.Avatar.FileUrl,
						FullNameInitials = "",
						GroupName = g.Cleaner.UserSubGroup.UserGroup.Name,
						SubGroupName = g.Cleaner.UserSubGroup.Name
					},
					HasSecondaryCleaner = g.SecondaryCleanerId.HasValue,
					SecondaryCleaner = !g.SecondaryCleanerId.HasValue ? null : new CleanerData
					{
						Id = g.SecondaryCleaner.Id,
						Name = (g.SecondaryCleaner.FirstName.IsNotNull() ? g.SecondaryCleaner.FirstName + " " : "") + (g.SecondaryCleaner.LastName.IsNotNull() ? g.SecondaryCleaner.LastName : ""),
						Username = g.SecondaryCleaner.UserName
					},
					SecondaryCleanerId = g.SecondaryCleanerId
				})
				.ToArrayAsync();


			//var buildingIds = new HashSet<string>();
			//var floorIds = new HashSet<string>();
			//var floorSections = new HashSet<string>();
			//var floorSubSections = new HashSet<string>();

			//foreach (var g in groups)
			//{
			//	foreach(var a in g.Cleaner.Affinities)
			//	{
			//		switch (a.Type)
			//		{
			//			case Common.Enums.CleaningPlanGroupAffinityType.BUILDING:
			//				if(!buildingIds.Contains(a.ReferenceId)) buildingIds.Add(a.ReferenceId);
			//				break;
			//			case Common.Enums.CleaningPlanGroupAffinityType.FLOOR:
			//				if (!floorIds.Contains(a.ReferenceId)) floorIds.Add(a.ReferenceId);
			//				break;
			//			case Common.Enums.CleaningPlanGroupAffinityType.FLOOR_SECTION:
			//				if (!floorSections.Contains(a.ReferenceId)) floorSections.Add(a.ReferenceId);
			//				break;
			//			case Common.Enums.CleaningPlanGroupAffinityType.FLOOR_SUB_SECTION:
			//				if (!floorSubSections.Contains(a.ReferenceId)) floorSubSections.Add(a.ReferenceId);
			//				break;
			//		}
			//	}
			//}

			return groups;
		}

		private async Task<(IEnumerable<PlannedCleaningTimelineItemData> plannedItems, IEnumerable<CleaningTimelineItemData> plannableItems)> _LoadCleaningPlanItems(Guid cleaningPlanId, string hotelId, Dictionary<Guid, List<CleaningTimelineItemTaskData>> plannedAttendantTasksMap)
		{
			var items = await this._databaseContext
				.CleaningPlanItems
				.Where(i => i.CleaningPlanId == cleaningPlanId)
				.Select(i =>
					 new
					 {
						 CleaningPlanGroupId = i.CleaningPlanGroupId,
						 End = i.EndsAt,
						 Start = i.StartsAt,
						 IsPlanned = i.IsPlanned,
						 CleaningDescription = i.Description,
						 CleaningPluginId = i.CleaningPluginId,
						 CleaningPluginName = i.Description,
						 Credits = i.Credits,
						 Id = i.Id.ToString(),
						 IsActive = i.IsActive,
						 IsChangeSheets = i.IsChangeSheets,
						 IsCustom = i.IsCustom,
						 IsPostponed = i.IsPostponed,
						 RoomId = i.RoomId,
						 //IsSent = i.CleaningId != null,
						 CleaningId = i.CleaningId,
						 Cleaning = i.Cleaning,
						 RoomBedId = i.RoomBedId,
						 IsPriority = i.IsPriority,
						 //InspectedById = i.Cleaning.InspectedById,
						 //IsInspected = i.Cleaning.IsInspected,
						 //InspectedByFullName = i.Cleaning.InspectedBy.FirstName + " " + i.Cleaning.InspectedBy.LastName,
						 //IsInspectionRequired = i.Cleaning.IsInspectionRequired,
						 //IsInspectionSuccess = i.Cleaning.IsInspectionSuccess,
						 //IsReadyForInspection = i.Cleaning.IsReadyForInspection,
						 //CleaningStatus = i.Cleaning.Status,
					 }
				)
				.ToArrayAsync();

			var pluginsMap = await this._databaseContext.CleaningPlugins.Where(cp => cp.HotelId == hotelId).ToDictionaryAsync(cp => cp.Id);

			var userIds = items.Where(i => i.Cleaning != null && i.Cleaning.InspectedById != null).Select(i => i.Cleaning.InspectedById.Value).Distinct().ToArray();
			var usersMap = await this._databaseContext.Users.Where(u => userIds.Contains(u.Id)).Select(u => new { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName }).ToDictionaryAsync(u => u.Id);

			var plannableItems = new List<CleaningTimelineItemData>();
			var plannedItems = new List<PlannedCleaningTimelineItemData>();
			foreach (var i in items)
			{
				var plannedAttendantTasks = plannedAttendantTasksMap.ContainsKey(i.RoomId) ? plannedAttendantTasksMap[i.RoomId] : new List<CleaningTimelineItemTaskData>();

				if (i.IsPlanned)
				{
					var plannedItem = new PlannedCleaningTimelineItemData
					{
						CleaningPlanGroupId = i.CleaningPlanGroupId?.ToString(),
						End = i.End.Value,
						Start = i.Start.Value,
						CleaningDescription = i.CleaningDescription,
						CleaningPluginId = i.CleaningPluginId,
						CleaningPluginName = i.CleaningPluginName,
						Credits = i.Credits,
						Id = i.Id.ToString(),
						IsActive = i.IsActive,
						IsChangeSheets = i.IsChangeSheets,
						IsCustom = i.IsCustom,
						IsPostponed = i.IsPostponed,
						RoomId = i.RoomId,
						IsSent = false,
						InspectedById = null,
						InspectedByFullName = null,
						IsInspected = false,
						IsInspectionRequired = false,
						IsInspectionSuccess = false,
						IsReadyForInspection = false,
						CleaningStatus = CleaningProcessStatus.UNKNOWN,
						PlannedAttendantTasks = plannedAttendantTasks,
						BedId = i.RoomBedId,
						IsPriority = i.IsPriority,
						BorderColorHex = i.CleaningPluginId.HasValue && pluginsMap.ContainsKey(i.CleaningPluginId.Value) ? pluginsMap[i.CleaningPluginId.Value].Data?.Color : null,
						Tasks = new List<CleaningTimelineItemTaskData>()
					};
					plannedItems.Add(plannedItem);

					if (i.Cleaning != null)
					{
						plannedItem.IsSent = i.Cleaning != null;
						plannedItem.InspectedById = i.Cleaning.InspectedById;
						plannedItem.IsInspected = i.Cleaning.IsInspected;
						plannedItem.IsInspectionRequired = i.Cleaning.IsInspectionRequired;
						plannedItem.IsInspectionSuccess = i.Cleaning.IsInspectionSuccess;
						plannedItem.IsReadyForInspection = i.Cleaning.IsReadyForInspection;
						plannedItem.CleaningStatus = i.Cleaning.Status;
						plannedItem.IsPriority = i.Cleaning.IsPriority;

						if (i.Cleaning.InspectedById.HasValue && usersMap.ContainsKey(i.Cleaning.InspectedById.Value))
						{
							var user = usersMap[i.Cleaning.InspectedById.Value];
							plannedItem.InspectedByFullName = $"{user.FirstName} {user.LastName}";
						}
					}
				}
				else
				{
					var plannableItem = new CleaningTimelineItemData
					{
						CleaningDescription = i.CleaningDescription,
						CleaningPluginId = i.CleaningPluginId,
						CleaningPluginName = i.CleaningPluginName,
						Credits = i.Credits,
						Id = i.Id.ToString(),
						IsActive = i.IsActive,
						IsChangeSheets = i.IsChangeSheets,
						IsCustom = i.IsCustom,
						IsPostponed = i.IsPostponed,
						IsPriority = i.IsPriority,
						RoomId = i.RoomId,
						BedId = i.RoomBedId,
						IsSent = false,
						InspectedById = null,
						InspectedByFullName = null,
						IsInspected = false,
						IsInspectionRequired = false,
						IsInspectionSuccess = false,
						IsReadyForInspection = false,
						BorderColorHex = i.CleaningPluginId.HasValue && pluginsMap.ContainsKey(i.CleaningPluginId.Value) ? pluginsMap[i.CleaningPluginId.Value].Data?.Color : null,

						CleaningStatus = CleaningProcessStatus.UNKNOWN,

						PlannedAttendantTasks = plannedAttendantTasks,
					};
					plannableItems.Add(plannableItem);

					if (i.Cleaning != null)
					{
						plannableItem.IsSent = i.Cleaning != null;
						plannableItem.InspectedById = i.Cleaning.InspectedById;
						plannableItem.IsInspected = i.Cleaning.IsInspected;
						plannableItem.IsInspectionRequired = i.Cleaning.IsInspectionRequired;
						plannableItem.IsInspectionSuccess = i.Cleaning.IsInspectionSuccess;
						plannableItem.IsReadyForInspection = i.Cleaning.IsReadyForInspection;
						plannableItem.CleaningStatus = i.Cleaning.Status;
						plannableItem.IsPriority = i.Cleaning.IsPriority;

						if (i.Cleaning.InspectedById.HasValue && usersMap.ContainsKey(i.Cleaning.InspectedById.Value))
						{
							var user = usersMap[i.Cleaning.InspectedById.Value];
							plannableItem.InspectedByFullName = $"{user.FirstName} {user.LastName}";
						}
					}
				}
			}
			return new(plannedItems, plannableItems);
		}

		private async Task<CleaningPlan> _InsertNewCleaningPlan(string hotelId, DateTime date, Guid userId)
		{
			var cleaningPlan = await this._CreateNewCleaningPlan(hotelId, date, userId);
			await this._databaseContext.CleaningPlans.AddAsync(cleaningPlan);
			return cleaningPlan;
		}

		private async Task<CleaningPlan> _CreateNewCleaningPlan(string hotelId, DateTime date, Guid userId)
		{
			var cleaningPlanId = Guid.NewGuid();
			var c = CpsatConfigurationProvider.GetDefaultCpsatPlannerConfiguration();

			var hotelSettings = await this._databaseContext
				.Settings
				.Where(s => s.HotelId == hotelId)
				.FirstOrDefaultAsync();

			var previousCleaningPlan = await this._databaseContext
				.CleaningPlans
				.Include(cp => cp.CleaningPlanCpsatConfiguration)
				.Where(cp => cp.Date < date.Date)
				.OrderByDescending(cp => cp.ModifiedAt)
				.FirstOrDefaultAsync();

			var config = new Domain.Entities.CleaningPlanCpsatConfiguration
			{
				Id = cleaningPlanId,
				ApplyLevelMovementCreditReductionAfterNumberOfLevels = c.ApplyLevelMovementCreditReductionAfterNumberOfLevels,
				ArePreferredLevelsExclusive = c.ArePreferredLevelsExclusive,
				BalanceByCreditsStrictMaxCredits = c.BalanceByCreditsStrictMaxCredits,
				BalanceByCreditsStrictMinCredits = c.BalanceByCreditsStrictMinCredits,
				BalanceByCreditsWithAffinitiesMaxCredits = c.BalanceByCreditsWithAffinitiesMaxCredits,
				BalanceByCreditsWithAffinitiesMinCredits = c.BalanceByCreditsWithAffinitiesMinCredits,
				BalanceByRoomsMaxRooms = c.BalanceByRoomsMaxRooms,
				BalanceByRoomsMinRooms = c.BalanceByRoomsMinRooms,
				BuildingAward = c.BuildingAward,
				BuildingMovementCreditsReduction = c.BuildingMovementCreditsReduction,
				CleaningPriorityKey = c.CleaningPriorityKey,
				WeightRoomsCleaned = c.WeightRoomsCleaned,
				DoBalanceStaysAndDepartures = c.DoBalanceStaysAndDepartures,
				DoCompleteProposedPlanOnUsePreplan = c.DoCompleteProposedPlanOnUsePreplan,
				DoesBuildingMovementReduceCredits = c.DoesBuildingMovementReduceCredits,
				DoesLevelMovementReduceCredits = c.DoesLevelMovementReduceCredits,
				DoUsePreAffinity = c.DoUsePreAffinity,
				DoUsePrePlan = c.DoUsePrePlan,
				LevelAward = c.LevelAward,
				LevelMovementCreditsReduction = c.LevelMovementCreditsReduction,
				MaxNumberOfBuildingsPerAttendant = c.MaxNumberOfBuildingsPerAttendant,
				MaxBuildingTravelTime = c.MaxBuildingTravelTime,
				MaxDeparture = c.MaxDeparture,
				MaxNumberOfLevelsPerAttendant = c.MaxNumberOfLevelsPerAttendant,
				MaxStay = c.MaxStay,
				MaxTravelTime = c.MaxTravelTime,
				PlanningStrategyTypeKey = c.PlanningStrategyTypeKey,
				RoomAward = c.RoomAward,
				SolverRunTime = c.SolverRunTime,
				TargetByCreditsValue = c.TargetByCreditsValue,
				TargetByRoomsValue = c.TargetByRoomsValue,
				WeightTravelTime = c.WeightTravelTime,
				WeightEpsilonStayDeparture = c.WeightEpsilonStayDeparture,
				MaxStaysIncreaseThreshold = c.MaxStaysIncreaseThreshold,
				MaxStaysEquivalentCredits = c.MaxStaysEquivalentCredits,
				MaxStaysIncreasesCredits = c.MaxStaysIncreasesCredits,
				MaxDeparturesReductionThreshold = c.MaxDeparturesReductionThreshold,
				MaxDeparturesReducesCredits = c.MaxDeparturesReducesCredits,
				MaxDeparturesEquivalentCredits = c.MaxDeparturesEquivalentCredits,
				LimitAttendantsPerLevel = c.LimitAttendantsPerLevel,

				BuildingsDistanceMatrix = hotelSettings.BuildingsDistanceMatrix,
				LevelsDistanceMatrix = hotelSettings.LevelsDistanceMatrix,
				WeightCredits = hotelSettings.WeightCredits,
				WeightLevelChange = hotelSettings.WeightLevelChange,
				MinCreditsForMultipleCleanersCleaning = hotelSettings.MinCreditsForMultipleCleanersCleaning,
				MinutesPerCredit = hotelSettings.MinutesPerCredit,
			};

			if (previousCleaningPlan != null && previousCleaningPlan.CleaningPlanCpsatConfiguration != null) 
			{ 
				config = new CleaningPlanCpsatConfiguration
				{
					ApplyLevelMovementCreditReductionAfterNumberOfLevels = previousCleaningPlan.CleaningPlanCpsatConfiguration.ApplyLevelMovementCreditReductionAfterNumberOfLevels,
					ArePreferredLevelsExclusive = previousCleaningPlan.CleaningPlanCpsatConfiguration.ArePreferredLevelsExclusive,
					BalanceByCreditsStrictMaxCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.BalanceByCreditsStrictMaxCredits,
					BalanceByCreditsStrictMinCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.BalanceByCreditsStrictMinCredits,
					BalanceByCreditsWithAffinitiesMaxCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.BalanceByCreditsWithAffinitiesMaxCredits,
					BalanceByCreditsWithAffinitiesMinCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.BalanceByCreditsWithAffinitiesMinCredits,
					BalanceByRoomsMaxRooms = previousCleaningPlan.CleaningPlanCpsatConfiguration.BalanceByRoomsMaxRooms,
					BalanceByRoomsMinRooms = previousCleaningPlan.CleaningPlanCpsatConfiguration.BalanceByRoomsMinRooms,
					BuildingAward = previousCleaningPlan.CleaningPlanCpsatConfiguration.BuildingAward,
					BuildingMovementCreditsReduction = previousCleaningPlan.CleaningPlanCpsatConfiguration.BuildingMovementCreditsReduction,
					CleaningPriorityKey = previousCleaningPlan.CleaningPlanCpsatConfiguration.CleaningPriorityKey,
					DoBalanceStaysAndDepartures = previousCleaningPlan.CleaningPlanCpsatConfiguration.DoBalanceStaysAndDepartures,
					DoCompleteProposedPlanOnUsePreplan = previousCleaningPlan.CleaningPlanCpsatConfiguration.DoCompleteProposedPlanOnUsePreplan,
					DoesBuildingMovementReduceCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.DoesBuildingMovementReduceCredits,
					DoesLevelMovementReduceCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.DoesLevelMovementReduceCredits,
					DoUsePreAffinity = previousCleaningPlan.CleaningPlanCpsatConfiguration.DoUsePreAffinity,
					DoUsePrePlan = previousCleaningPlan.CleaningPlanCpsatConfiguration.DoUsePrePlan,
					Id = cleaningPlanId,
					LevelAward = previousCleaningPlan.CleaningPlanCpsatConfiguration.LevelAward,
					LevelMovementCreditsReduction = previousCleaningPlan.CleaningPlanCpsatConfiguration.LevelMovementCreditsReduction,
					LimitAttendantsPerLevel = previousCleaningPlan.CleaningPlanCpsatConfiguration.LimitAttendantsPerLevel,
					MaxBuildingTravelTime = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxBuildingTravelTime,
					MaxDeparture = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxDeparture,
					MaxDeparturesEquivalentCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxDeparturesEquivalentCredits,
					MaxDeparturesReducesCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxDeparturesReducesCredits,
					MaxDeparturesReductionThreshold = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxDeparturesReductionThreshold,
					MaxNumberOfBuildingsPerAttendant = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxNumberOfBuildingsPerAttendant,
					MaxNumberOfLevelsPerAttendant = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxNumberOfLevelsPerAttendant,
					MaxStay = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxStay,
					MaxStaysEquivalentCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxStaysEquivalentCredits,
					MaxStaysIncreasesCredits = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxStaysIncreasesCredits,
					MaxStaysIncreaseThreshold = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxStaysIncreaseThreshold,
					MaxTravelTime = previousCleaningPlan.CleaningPlanCpsatConfiguration.MaxTravelTime,
					PlanningStrategyTypeKey = previousCleaningPlan.CleaningPlanCpsatConfiguration.PlanningStrategyTypeKey,
					RoomAward = previousCleaningPlan.CleaningPlanCpsatConfiguration.RoomAward,
					SolverRunTime = previousCleaningPlan.CleaningPlanCpsatConfiguration.SolverRunTime,
					TargetByCreditsValue = previousCleaningPlan.CleaningPlanCpsatConfiguration.TargetByCreditsValue,
					TargetByRoomsValue = previousCleaningPlan.CleaningPlanCpsatConfiguration.TargetByRoomsValue,
					WeightEpsilonStayDeparture = previousCleaningPlan.CleaningPlanCpsatConfiguration.WeightEpsilonStayDeparture,
					WeightRoomsCleaned = previousCleaningPlan.CleaningPlanCpsatConfiguration.WeightRoomsCleaned,
					WeightTravelTime = previousCleaningPlan.CleaningPlanCpsatConfiguration.WeightTravelTime,
					
					MinCreditsForMultipleCleanersCleaning = hotelSettings.MinCreditsForMultipleCleanersCleaning,
					WeightCredits = hotelSettings.WeightCredits,
					WeightLevelChange = hotelSettings.WeightLevelChange,
					MinutesPerCredit = hotelSettings.MinutesPerCredit,
					LevelsDistanceMatrix = hotelSettings.LevelsDistanceMatrix,
					BuildingsDistanceMatrix = hotelSettings.BuildingsDistanceMatrix,
				};
			}

			

			return new CleaningPlan
			{
				CreatedAt = DateTime.UtcNow,
				CreatedById = userId,
				Date = date,
				HotelId = hotelId,
				Id = cleaningPlanId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = userId,
				Groups = new CleaningPlanGroup[0],
				CleaningPlanCpsatConfiguration = config,
				IsSent = false,
				SentAt = null,
				SentById = null,
			};
		}

		private async Task<Dictionary<Guid, List<CleaningTimelineItemTaskData>>> _LoadPlannedTasks(IEnumerable<Guid> userIds, string hotelId, DateTime planFromDate, DateTime planToDate)
		{
			var tasks = await this._databaseContext
				.SystemTasks
				.Include(st => st.User)
				.Where(t =>
					(t.FromHotelId == hotelId || t.ToHotelId == hotelId) &&
					t.UserId != null &&
					userIds.Contains(t.UserId.Value) &&
					(
						(
							t.TypeKey == TaskType.EVENT.ToString() &&
							t.EventKey == EventTaskType.CLEANING.ToString() &&
							(
								(t.EventTimeKey == TaskEventTimeType.ON_NEXT.ToString()) ||
								(t.EventTimeKey == TaskEventTimeType.ON_DATE.ToString() && planFromDate <= t.StartsAt && planToDate >= t.StartsAt) ||
								(t.EventTimeKey == TaskEventTimeType.EVERY_TIME.ToString() && t.StartsAt <= planToDate)
							)
						)
						||
						(
							t.TypeKey != TaskType.EVENT.ToString() &&
							planFromDate <= t.StartsAt &&
							planToDate >= t.StartsAt
						)
					)
				).ToArrayAsync();

			var tasksMap = new Dictionary<Guid, List<CleaningTimelineItemTaskData>>();

			foreach (var task in tasks)
			{
				var taskItem = this._CreateTimelineItemTaskData(task);

				if (task.FromRoomId.HasValue)
				{
					if (!tasksMap.ContainsKey(task.FromRoomId.Value))
					{
						tasksMap.Add(task.FromRoomId.Value, new List<CleaningTimelineItemTaskData>());
					}

					tasksMap[task.FromRoomId.Value].Add(taskItem);
				}

				if (task.ToRoomId.HasValue)
				{
					if (!tasksMap.ContainsKey(task.ToRoomId.Value))
					{
						tasksMap.Add(task.ToRoomId.Value, new List<CleaningTimelineItemTaskData>());
					}

					if (!tasksMap[task.FromRoomId.Value].Any(t => t.TaskId == taskItem.TaskId))
					{
						tasksMap[task.FromRoomId.Value].Add(taskItem);
					}
				}
			}

			return tasksMap;

			//return ()
			//		.Select(st => { return _CreateTimelineItemTaskData(st); })
			//		.GroupBy(st => st.RoomId)
			//		.ToDictionary(st => st.Key, st => st.ToArray());
		}

		private async Task<SystemTask[]> _LoadPlannedCleaningAndNonEventTasks(IEnumerable<Guid> userIds, string hotelId, DateTime planFromDate, DateTime planToDate)
		{
			return (await this._databaseContext
				.SystemTasks
				.Include(st => st.User)
				.Include(st => st.Actions)
				.Where(t =>
					(t.FromHotelId == hotelId || t.ToHotelId == hotelId) &&
					userIds.Contains(t.UserId.Value) &&
					(
						(
							t.TypeKey == TaskType.EVENT.ToString() &&
							t.EventKey == EventTaskType.CLEANING.ToString() &&
							(
								(t.EventTimeKey == TaskEventTimeType.ON_NEXT.ToString()) ||
								(t.EventTimeKey == TaskEventTimeType.ON_DATE.ToString() && planFromDate <= t.StartsAt && planToDate >= t.StartsAt) ||
								(t.EventTimeKey == TaskEventTimeType.EVERY_TIME.ToString() && t.StartsAt <= planToDate)
							)
						)
						||
						(
							t.TypeKey != TaskType.EVENT.ToString() &&
							planFromDate <= t.StartsAt &&
							planToDate >= t.StartsAt
						)
					)
				).ToArrayAsync());
		}

		private async Task<SystemTask[]> _LoadTasks(string hotelId, DateTime planFromDate, DateTime planToDate)
		{
			return (await this._databaseContext
				.SystemTasks
				.Include(st => st.User)
				.Include(st => st.Actions)
				.Where(t =>
					(t.FromHotelId == hotelId || t.ToHotelId == hotelId) &&
					t.StatusKey != TaskStatusType.CANCELLED.ToString() &&
					(
						(
							t.TypeKey == TaskType.EVENT.ToString() &&
							t.EventKey == EventTaskType.CLEANING.ToString() &&
							(
								(t.EventTimeKey == TaskEventTimeType.ON_NEXT.ToString()) ||
								(t.EventTimeKey == TaskEventTimeType.ON_DATE.ToString() && planFromDate <= t.StartsAt && planToDate >= t.StartsAt) ||
								(t.EventTimeKey == TaskEventTimeType.EVERY_TIME.ToString() && t.StartsAt <= planToDate)
							)
						)
						||
						(
							t.TypeKey != TaskType.EVENT.ToString() &&
							planFromDate <= t.StartsAt &&
							planToDate >= t.StartsAt
						)
					)
				).ToArrayAsync());
		}
	}

	public class CpsatConfigurationProvider
	{
		public static CpsatPlannerConfigurationData GetDefaultCpsatPlannerConfiguration()
		{
			return new CpsatPlannerConfigurationData
			{
				Id = null,
				PlanningStrategyTypeKey = "BALANCE_BY_ROOMS", // BALANCE_BY_ROOMS, BALANCE_BY_CREDITS_STRICT, BALANCE_BY_CREDITS_WITH_AFFINITIES, TARGET_BY_ROOMS, TARGET_BY_CREDITS
				BalanceByRoomsMinRooms = 1,
				BalanceByRoomsMaxRooms = 10,
				BalanceByCreditsStrictMinCredits = 0,
				BalanceByCreditsStrictMaxCredits = 0,
				BalanceByCreditsWithAffinitiesMinCredits = 0,
				BalanceByCreditsWithAffinitiesMaxCredits = 0,
				TargetByRoomsValue = "", // value is set if PlanningStrategyTypeKey = TARGET_BY_ROOMS
				TargetByCreditsValue = "", // value is set if PlanningStrategyTypeKey = TARGET_BY_CREDITS
				DoBalanceStaysAndDepartures = false,
				WeightEpsilonStayDeparture = 0,
				MaxStay = 0,
				MaxDeparture = 0,
				MaxTravelTime = 1000,
				MaxBuildingTravelTime = 100,
				MaxNumberOfBuildingsPerAttendant = 10,
				MaxNumberOfLevelsPerAttendant = 10,
				RoomAward = 10,
				LevelAward = 5,
				BuildingAward = 1,
				WeightTravelTime = -1,
				WeightCredits = 1,
				WeightRoomsCleaned = 1,
				WeightLevelChange = -1,
				SolverRunTime = 60,
				DoesLevelMovementReduceCredits = false,
				ApplyLevelMovementCreditReductionAfterNumberOfLevels = 0,
				LevelMovementCreditsReduction = 0,
				DoUsePrePlan = true,
				DoUsePreAffinity = false,
				DoCompleteProposedPlanOnUsePreplan = false,
				DoesBuildingMovementReduceCredits = false,
				BuildingMovementCreditsReduction = 0,
				ArePreferredLevelsExclusive = false,
				CleaningPriorityKey = "Departure",
				BuildingsDistanceMatrix = null,
				LevelsDistanceMatrix = null,

				LimitAttendantsPerLevel = false,
				MaxDeparturesEquivalentCredits = 0,
				MaxDeparturesReducesCredits = false,
				MaxDeparturesReductionThreshold = 0,
				MaxStaysEquivalentCredits = 0,
				MaxStaysIncreasesCredits = false,
				MaxStaysIncreaseThreshold = 0,

				MinutesPerCredit = 0,
				MinCreditsForMultipleCleanersCleaning = 0,
			};
		}
	}
}
