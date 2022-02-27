using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.CpsatCleaningCalculator
{
    public class CPSat
    {
        public CPSat()
        {
            SolutionResult = new CPSatSolutionInformation();
        }
        public CPSatSolutionInformation SolutionResult { get; set; }
        public bool UsePrePlan { get; set; }
        public bool CompletePrePlan { get; set; }
        // public bool usePreAffinity { get; set; }
        public List<List<Room>> CommunicatingRooms { get; set; }

        public bool preferredLevelsAreExclusive { get; set; }

        public int minCreditsForMultipleCleanersCleaning { get; set; } = 100;
        public int minRooms { get; set; }
        public int maxRooms { get; set; }
        public int minCredits { get; set; }
        public int maxCredits { get; set; }
        public int maxTravelTime { get; set; }
        public int maxBuildingTravelTime { get; set; }
        // public int maxLevelCountChange { get; set; }//Count
        public int maxShiftFloorAllowed { get; set; } //Distance: 1
        public int maxBuildingToBuildingDistanceAllowed { get; set; }
        public int maxNumberOfBuildingsPerAttendant { get; set; }
        public int maxNumberOfLevelsPerAttendant { get; set; }

        public int epsilonCredits { get; set; }
        public int epsilonRooms { get; set; }

        public int weightTravelTime { get; set; }
        public int weightLevelChange { get; set; } = -70;// Should in be UI
        public bool limitAttendantsPerLevel  { get; set; } = false; //it is a tweak :

        public int weightCredits { get; set; }
        public int weightRoomsCleaned { get; set; }
        public int weightFloorsCompleted { get; set; } = 4;

        public int awardLevel { get; set; }
        public int awardRoom { get; set; }
        public int awardBuilding { get; set; }

        public int weightLevelAward { get; set; }
        public int weightRoomAward { get; set; }
        public int weightBuildingAward { get; set; }

        public int weightMinimizeAttendants { get; set; }
        public int solverRunTime { get; set; }

        public bool useTargetMode { get; set; }

        public bool targetModeMinimizeAttendants { get; set; }
        public int targetModeWeightMaxedRooms { get; set; }
        public int targetModeWeightMaxedCredits { get; set; }


        public bool balanceStayDepartMode { get; set; }

        public string buildingsDistanceMatrix { get; set; }
        public string levelsDistanceMatrix { get; set; }

        public bool sortBuildingIntervals { get; set; } = true;
        public int buildingDistanceMultiplier { get; set; } = 1;

        public int weightepsilonStayDepart { get; set; }

        public int maxDepartures { get; set; }
        public int maxStays { get; set; }

        public bool maxDeparturesReducesCredits { get; set; }
        public int maxDeparturesEquivalentCredits { get; set; }
        public int maxDeparturesReductionThreshold { get; set; }

        public bool maxStaysIncreasesCredits { get; set; }
        public int maxStaysEquivalentCredits { get; set; }
        public int maxStaysIncreaseThreshold { get; set; }

        public bool levelMovementReducesCredits { get; set; }
        public int levelMovementEquivalentCredits { get; set; }
        public int levelMovementReductionThreshold { get; set; }//F 3 - F 2=1

        public bool buildingMovementReducesCredits { get; set; }
        public int buildingMovementEquivalentCredits { get; set; }


        public string cleaningPriorities { get; set; }
        public string otherCleaningPriorities { get; set; }
        public string strategy { get; set; }

        public float minutesPerCredit { get; set; } // when set to 0 Credits=Min

        public int CreditsToMinutes(int credits)
        {
            if (minutesPerCredit <= 0)
                return credits;
            return (int) (credits * minutesPerCredit); // scale credits into minutes
        }
    }
}
