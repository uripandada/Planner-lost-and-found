using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Planner.CpsatCleaningCalculator;

namespace Planner.Tests
{
    [Collection("CPSAT Test Collection")]
    public class PlanningCPSATHRTTests : PlanningCPSATTestsBase
    {

        public PlanningCPSATHRTTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {

            Strategy.CPSat.solverRunTime                          = 120;


            Strategy.CPSat.UsePrePlan                             = false;
            Strategy.CPSat.CompletePrePlan                        = false;
            // Strategy.CPSat.CommunicatingRooms                  = ;
            Strategy.CPSat.minRooms                               = 0;
            Strategy.CPSat.maxRooms                               = 17;
            Strategy.CPSat.minCredits                             = 0;
            Strategy.CPSat.maxCredits                             = 170;
            Strategy.CPSat.maxTravelTime                          = 10000;
            Strategy.CPSat.maxBuildingTravelTime                  = 100;
            Strategy.CPSat.maxNumberOfLevelsPerAttendant          = 30;
            Strategy.CPSat.maxShiftFloorAllowed                   = 3;
            Strategy.CPSat.maxBuildingToBuildingDistanceAllowed   = 100;
            Strategy.CPSat.maxNumberOfBuildingsPerAttendant       = 10;

            Strategy.CPSat.epsilonCredits                         = -1;
            Strategy.CPSat.epsilonRooms                           = 0; //-1;

            Strategy.CPSat.weightTravelTime                       = -5;
            Strategy.CPSat.weightLevelChange                      = -50;

            Strategy.CPSat.weightCredits                          = 50;
            Strategy.CPSat.weightRoomsCleaned                     = 50;
            Strategy.CPSat.awardLevel                             = 10;
            Strategy.CPSat.awardRoom                              = 5;
            Strategy.CPSat.awardBuilding                          = 10;
            Strategy.CPSat.weightLevelAward                       = 10;
            Strategy.CPSat.weightRoomAward                        = 10;
            Strategy.CPSat.weightBuildingAward                    = 1;
            // Strategy.CPSat.weightMinimizeAttendants            = 0;

            Strategy.CPSat.useTargetMode                          = false;
            Strategy.CPSat.targetModeMinimizeAttendants           = false;

            Strategy.CPSat.balanceStayDepartMode                  = false;
            Strategy.CPSat.weightepsilonStayDepart                = 0;
            Strategy.CPSat.maxDepartures = 0;
            Strategy.CPSat.maxStays = 0;

            Strategy.CPSat.maxDeparturesReducesCredits            = true;
            Strategy.CPSat.maxDeparturesEquivalentCredits         = 10;
            Strategy.CPSat.maxDeparturesReductionThreshold        = 10;

            Strategy.CPSat.maxStaysIncreasesCredits            = true;
            Strategy.CPSat.maxStaysEquivalentCredits           = 10;
            Strategy.CPSat.maxStaysIncreaseThreshold           = 10;


            Strategy.CPSat.levelMovementReducesCredits            = true;
            Strategy.CPSat.levelMovementReductionThreshold        = 5;
            Strategy.CPSat.levelMovementEquivalentCredits         = 10;

            Strategy.CPSat.buildingMovementReducesCredits         = false;
            // Strategy.CPSat.buildingMovementReductionThreshold    = 2;
            Strategy.CPSat.buildingMovementEquivalentCredits      = 25;

            Strategy.CPSat.sortBuildingIntervals                  = true;

            // Strategy.CPSat.cleaningPriorities = "Others";
            // Strategy.CPSat.otherCleaningPriorities = "DEP/ARR,Departure,Arrival";

            Strategy.CPSat.limitAttendantsPerLevel =false;

            Strategy.CPSat.weightFloorsCompleted                  = 20;
        }

        [Fact]
        public void TestJan19HRTNoVacantAssignment()
        {
            Strategy.CPSat.solverRunTime                          = 30;

            Strategy.CPSat.epsilonCredits                         = 0;
            Strategy.CPSat.epsilonRooms                           = -1;
            Strategy.CPSat.weightTravelTime                       = -1;
            Strategy.CPSat.weightLevelChange                      = -1;
            Strategy.CPSat.maxNumberOfLevelsPerAttendant          = 5;
            Strategy.CPSat.weightCredits                          = 75;

            Strategy.CPSat.maxShiftFloorAllowed                   = 10;

            Strategy.CPSat.levelMovementReducesCredits            = false;
            Strategy.CPSat.maxDeparturesReducesCredits            = false;
            Strategy.CPSat.maxStaysIncreasesCredits            = false;


            Strategy.CPSat.minRooms                               = 0;
            Strategy.CPSat.maxRooms                               = 6;
            Strategy.CPSat.minCredits                             = 0;
            Strategy.CPSat.maxCredits                             = 0;

            Strategy.CPSat.weightFloorsCompleted                  = 200;

            var hotelId = "6112261f089bda000f781ec6";
            var hotelString = "h8189";
            var dataDateString = "18jan2022";
            var mystart = DateTime.Parse("2022-01-19T00:00:00");
            var myend = DateTime.Parse("2022-01-20T00:00:00");
            int expectedProblems = 44;
            int attendantCount = 3;
            var distancesPath = Path.Combine("Data", $"{hotelId}-room-distances.json");
            var distancesJson = File.ReadAllText(distancesPath);
            var roomDistances = Distances.LoadFromJSON(distancesJson, Strategy);

            var acceptableTypes = new List<CleaningType>() {CleaningType.Stay, CleaningType.Departure};
            var assigned_jobs = BasicCPSatTest(hotelId, hotelString, dataDateString, mystart, myend, attendantCount, roomDistances, acceptableTypes);

            var affinities = new List<String>(){"A-18", "A-18", "A-19"};
            var assigned_jobs_affinities = BasicCPSatTest(hotelId, hotelString, dataDateString, mystart, myend, attendantCount, roomDistances, acceptableTypes, affinities);

            Assert.Equal(expectedProblems, Cleanings.Count());
        }
        [Fact]
        public void TestFeb09HRTAffinities()
        {
            Strategy.CPSat.solverRunTime                          = 30;

            Strategy.CPSat.epsilonCredits                         = -1;
            Strategy.CPSat.epsilonRooms                           = 0;
            Strategy.CPSat.weightTravelTime                       = -1;
            Strategy.CPSat.weightLevelChange                      = -1;
            Strategy.CPSat.maxNumberOfLevelsPerAttendant          = 5;
            Strategy.CPSat.weightCredits                          = 75;

            Strategy.CPSat.maxShiftFloorAllowed                   = 10;

            Strategy.CPSat.levelMovementReducesCredits            = false;
            Strategy.CPSat.maxDeparturesReducesCredits            = false;
            Strategy.CPSat.maxStaysIncreasesCredits            = false;


            Strategy.CPSat.minRooms                               = 0;
            Strategy.CPSat.maxRooms                               = 0;
            Strategy.CPSat.minCredits                             = 0;
            Strategy.CPSat.maxCredits                             = 90;

            Strategy.CPSat.weightFloorsCompleted                  = 50;

            Strategy.CPSat.awardLevel                             = 10;
            Strategy.CPSat.awardRoom                              = 5;
            Strategy.CPSat.awardBuilding                          = 10;
            Strategy.CPSat.weightLevelAward                       = 10;
            Strategy.CPSat.weightRoomAward                        = 10;

            var hotelId = "58d3f73e99295b2c84000000";
            var hotelString = "hrt";
            var dataDateString = "09feb2022fj";
            var mystart = DateTime.Parse("2022-02-10T00:00:00");
            var myend = DateTime.Parse("2022-02-11T00:00:00");
            int expectedProblems = 21;
            int attendantCount = 2;
            // var distancesPath = Path.Combine("Data", $"{hotelId}-room-distances.json");
            // var distancesJson = File.ReadAllText(distancesPath);
            // var roomDistances = Distances.LoadFromJSON(distancesJson, Strategy);

            var acceptableTypes = new List<CleaningType>() {CleaningType.Stay, CleaningType.Departure};
            var affinities = new List<String>(){"1"};
            var assigned_jobs_affinities = BasicCPSatTest(hotelId, hotelString, dataDateString, mystart, myend, attendantCount, null, acceptableTypes, affinities);

            Assert.Equal(expectedProblems, Cleanings.Count());
        }



    }
}
