using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Domain.Entities
{
	/// <summary>
	/// Has 1:0...1 relationship with CleaningPlan
	/// </summary>
	public class CleaningPlanCpsatConfiguration
	{
		public Guid Id { get; set; }
		public CleaningPlan CleaningPlan { get; set; }

		public string PlanningStrategyTypeKey { get; set; } // BALANCE_BY_ROOMS, BALANCE_BY_CREDITS_STRICT, BALANCE_BY_CREDITS_WITH_AFFINITIES, TARGET_BY_ROOMS, TARGET_BY_CREDITS
		public int BalanceByRoomsMinRooms { get; set; }
		public int BalanceByRoomsMaxRooms { get; set; }
		public int BalanceByCreditsStrictMinCredits { get; set; }
		public int BalanceByCreditsStrictMaxCredits { get; set; }
		public int BalanceByCreditsWithAffinitiesMinCredits { get; set; }
		public int BalanceByCreditsWithAffinitiesMaxCredits { get; set; }
		public string TargetByRoomsValue { get; set; } // value is set if PlanningStrategyTypeKey = TARGET_BY_ROOMS
		public string TargetByCreditsValue { get; set; } // value is set if PlanningStrategyTypeKey = TARGET_BY_CREDITS

		public bool DoBalanceStaysAndDepartures { get; set; }
		public int WeightEpsilonStayDeparture { get; set; }
		public int MaxStay { get; set; }
		public int MaxDeparture { get; set; }

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
		public int WeightLevelChange { get; set; }
		//public int WeightFloorsCompleted { get; set; }
		public bool LimitAttendantsPerLevel  { get; set; }


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

		public decimal MinutesPerCredit { get; set; }
		public int MinCreditsForMultipleCleanersCleaning { get; set; }

		public bool MaxDeparturesReducesCredits { get; set; }
		public int MaxDeparturesEquivalentCredits { get; set; }
		public int MaxDeparturesReductionThreshold { get; set; }

		public bool MaxStaysIncreasesCredits { get; set; }
		public int MaxStaysEquivalentCredits { get; set; }
		public int MaxStaysIncreaseThreshold { get; set; }
	}

}
