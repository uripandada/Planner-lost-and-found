using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.CleaningPlans.Commands.SaveCpsatConfiguration
{
	public class SaveCpsatConfigurationCommand : IRequest<ProcessResponse>
	{
		public Guid CleaningPlanId { get; set; }
		public int SolverRunTime { get; set; }
		public string PlanningStrategyTypeKey { get; set; }
		public int BalanceByRoomsMinRooms { get; set; }
		public int BalanceByRoomsMaxRooms { get; set; }
		public int BalanceByCreditsStrictMinCredits { get; set; }
		public int BalanceByCreditsStrictMaxCredits { get; set; }
		public int BalanceByCreditsWithAffinitiesMinCredits { get; set; }
		public int BalanceByCreditsWithAffinitiesMaxCredits { get; set; }
		public string TargetByRoomsValue { get; set; }
		public string TargetByCreditsValue { get; set; }
		public int MaxNumberOfBuildingsPerAttendant { get; set; }
		public int MaxBuildingTravelTime { get; set; }
		public bool DoesBuildingMovementReduceCredits { get; set; }
		public int BuildingMovementCreditsReduction { get; set; }
		public int MaxNumberOfLevelsPerAttendant { get; set; }
		public bool DoesLevelMovementReduceCredits { get; set; }
		public int LevelMovementCreditsReduction { get; set; }
		public int ApplyLevelMovementCreditReductionAfterNumberOfLevels { get; set; }


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
	}

	public class SaveCpsatConfigurationCommandHandler : IRequestHandler<SaveCpsatConfigurationCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public SaveCpsatConfigurationCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<ProcessResponse> Handle(SaveCpsatConfigurationCommand request, CancellationToken cancellationToken)
		{
			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var config = await this._databaseContext.CleaningPlanCpsatConfigurations.FindAsync(request.CleaningPlanId);

				if (config == null)
				{
					var defaultConfig = Queries.GetCleaningPlanDetails.CpsatConfigurationProvider.GetDefaultCpsatPlannerConfiguration();

					config = new Domain.Entities.CleaningPlanCpsatConfiguration
					{
						Id = request.CleaningPlanId,
						SolverRunTime = request.SolverRunTime,
						PlanningStrategyTypeKey = request.PlanningStrategyTypeKey,
						BalanceByCreditsStrictMaxCredits = request.BalanceByCreditsStrictMaxCredits,
						BalanceByCreditsStrictMinCredits = request.BalanceByCreditsStrictMinCredits,
						BalanceByCreditsWithAffinitiesMaxCredits = request.BalanceByCreditsWithAffinitiesMaxCredits,
						BalanceByCreditsWithAffinitiesMinCredits = request.BalanceByCreditsWithAffinitiesMinCredits,
						BalanceByRoomsMaxRooms = request.BalanceByRoomsMaxRooms,
						BalanceByRoomsMinRooms = request.BalanceByRoomsMinRooms,
						TargetByRoomsValue = request.TargetByRoomsValue,
						TargetByCreditsValue = request.TargetByCreditsValue,
						MaxNumberOfBuildingsPerAttendant = request.MaxNumberOfBuildingsPerAttendant,
						MaxBuildingTravelTime = request.MaxBuildingTravelTime,
						DoesBuildingMovementReduceCredits = request.DoesBuildingMovementReduceCredits,
						BuildingMovementCreditsReduction = request.BuildingMovementCreditsReduction,
						MaxNumberOfLevelsPerAttendant = request.MaxNumberOfLevelsPerAttendant,
						DoesLevelMovementReduceCredits = request.DoesLevelMovementReduceCredits,
						LevelMovementCreditsReduction = request.LevelMovementCreditsReduction,
						ApplyLevelMovementCreditReductionAfterNumberOfLevels = request.ApplyLevelMovementCreditReductionAfterNumberOfLevels,

						MaxDeparture = request.MaxDeparture,
						MaxStay = request.MaxStay,
						WeightEpsilonStayDeparture = request.WeightEpsilonStayDeparture,
						DoBalanceStaysAndDepartures = request.DoBalanceStaysAndDepartures,

						MaxDeparturesEquivalentCredits = request.MaxDeparturesEquivalentCredits,
						MaxDeparturesReducesCredits = request.MaxDeparturesReducesCredits,
						MaxDeparturesReductionThreshold = request.MaxDeparturesReductionThreshold,
						MaxStaysEquivalentCredits = request.MaxStaysEquivalentCredits,
						MaxStaysIncreasesCredits = request.MaxStaysIncreasesCredits,
						MaxStaysIncreaseThreshold = request.MaxStaysIncreaseThreshold,

						MinCreditsForMultipleCleanersCleaning = defaultConfig.MinCreditsForMultipleCleanersCleaning,
						MinutesPerCredit = defaultConfig.MinutesPerCredit,
						ArePreferredLevelsExclusive = defaultConfig.ArePreferredLevelsExclusive,
						DoUsePreAffinity = defaultConfig.DoUsePreAffinity,
						DoUsePrePlan = defaultConfig.DoUsePrePlan,
						DoCompleteProposedPlanOnUsePreplan = defaultConfig.DoCompleteProposedPlanOnUsePreplan,
						CleaningPriorityKey = defaultConfig.CleaningPriorityKey,

						BuildingAward = defaultConfig.BuildingAward,
						BuildingsDistanceMatrix = defaultConfig.BuildingsDistanceMatrix,
						LevelAward = defaultConfig.LevelAward,
						LevelsDistanceMatrix = defaultConfig.LevelsDistanceMatrix,
						LimitAttendantsPerLevel = defaultConfig.LimitAttendantsPerLevel,
						MaxTravelTime = defaultConfig.MaxTravelTime,
						RoomAward = defaultConfig.RoomAward,
						WeightCredits = defaultConfig.WeightCredits,
						WeightLevelChange = defaultConfig.WeightLevelChange,
						WeightRoomsCleaned = defaultConfig.WeightRoomsCleaned,
						WeightTravelTime = defaultConfig.WeightTravelTime,
					};

					await this._databaseContext.CleaningPlanCpsatConfigurations.AddAsync(config);
				}
				else
				{
					config.SolverRunTime = request.SolverRunTime;
					config.PlanningStrategyTypeKey = request.PlanningStrategyTypeKey;
					config.BalanceByCreditsStrictMaxCredits = request.BalanceByCreditsStrictMaxCredits;
					config.BalanceByCreditsStrictMinCredits = request.BalanceByCreditsStrictMinCredits;
					config.BalanceByCreditsWithAffinitiesMaxCredits = request.BalanceByCreditsWithAffinitiesMaxCredits;
					config.BalanceByCreditsWithAffinitiesMinCredits = request.BalanceByCreditsWithAffinitiesMinCredits;
					config.BalanceByRoomsMaxRooms = request.BalanceByRoomsMaxRooms;
					config.BalanceByRoomsMinRooms = request.BalanceByRoomsMinRooms;
					config.TargetByRoomsValue = request.TargetByRoomsValue;
					config.TargetByCreditsValue = request.TargetByCreditsValue;
					config.MaxNumberOfBuildingsPerAttendant = request.MaxNumberOfBuildingsPerAttendant;
					config.MaxBuildingTravelTime = request.MaxBuildingTravelTime;
					config.DoesBuildingMovementReduceCredits = request.DoesBuildingMovementReduceCredits;
					config.BuildingMovementCreditsReduction = request.BuildingMovementCreditsReduction;
					config.MaxNumberOfLevelsPerAttendant = request.MaxNumberOfLevelsPerAttendant;
					config.DoesLevelMovementReduceCredits = request.DoesLevelMovementReduceCredits;
					config.LevelMovementCreditsReduction = request.LevelMovementCreditsReduction;
					config.ApplyLevelMovementCreditReductionAfterNumberOfLevels = request.ApplyLevelMovementCreditReductionAfterNumberOfLevels;

					config.MaxDeparture = request.MaxDeparture;
					config.MaxStay = request.MaxStay;
					config.WeightEpsilonStayDeparture = request.WeightEpsilonStayDeparture;
					config.DoBalanceStaysAndDepartures = request.DoBalanceStaysAndDepartures;
					config.MaxDeparturesEquivalentCredits = request.MaxDeparturesEquivalentCredits;
					config.MaxDeparturesReducesCredits = request.MaxDeparturesReducesCredits;
					config.MaxDeparturesReductionThreshold = request.MaxDeparturesReductionThreshold;
					config.MaxStaysEquivalentCredits = request.MaxStaysEquivalentCredits;
					config.MaxStaysIncreasesCredits = request.MaxStaysIncreasesCredits;
					config.MaxStaysIncreaseThreshold = request.MaxStaysIncreaseThreshold;
				}

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync(cancellationToken);

				return new ProcessResponse
				{
					HasError = false,
					IsSuccess = true,
					Message = "CPSAT configuration saved"
				};
			}
		}
	}
}
