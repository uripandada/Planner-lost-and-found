using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Google.OrTools.Sat;

namespace Planner.CpsatCleaningCalculator
{
    public class BuildingIntervalConstraints
    {


        // MakeBuildingIntervals
        //
        // same pattern as make level intervals
        //
        // arguments are
        // the model,
        // the attendant,
        // the list of cleaning decision variable for the attendant
        // a list of LevelCleaningInterval will be filled, one per level, based on the cleanings that are possible for this attendant
        //
        public static void Make_Building_Intervals(
            CpModel model,
            Attendant attendant,
            List<BuildingCleanings> buildingsCleanings,
            AttendantInterval attendant_interval,
            List<CleaningInterval> attendant_cleaning_intervals,
            List<LevelCleaningInterval> attendant_level_cleanings,
            List<BuildingCleaningInterval> attendant_building_intervals,
            List<IntVar> attendant_room_travel,
            List<IntVar> attendant_floor_travel,
            List<IntVar> attendant_building_travel,
            IntVar levelsCleaned,
            IntVar buildingsCleaned,
            int max_travel_time,
            List<Affinity> affinities,
            Dictionary<Cleaning, List<IntVar>> room_literals,
            IntVar rooms_cleaned,
            DateTime earliest_date,
            Distances roomDistances,
            HotelStrategy hotelStrategy
        )
        {

            // Log.GetLog().Warning($"make building intervals call, {attendant_level_cleanings.Count} possible level cleanings assigned,  {buildingsCleanings.Count()} buildings for {attendant.Username}");

            // track the attendant's work assignments

            // set up boundaries for this attendant
            var maxCredits = hotelStrategy.CPSat.maxCredits;
            if (attendant.CurrentTimeSlot?.MaxCredits != null && attendant.CurrentTimeSlot.MaxCredits > 0)
            {
                maxCredits =  attendant.CurrentTimeSlot.MaxCredits;
            }
            var maxRooms = hotelStrategy.CPSat.maxRooms;
            if (attendant.CurrentTimeSlot?.noOfRooms != null && attendant.CurrentTimeSlot.noOfRooms > 0)
            {
                maxRooms = attendant.CurrentTimeSlot.noOfRooms;
            }

            var attendantCleanings = new AttendantCleanings(attendant, maxRooms, maxCredits);
            var wantedAffinityTypes = new List<AffinityType>(){AffinityType.BUILDING, AffinityType.FLOOR, AffinityType.FLOOR_SECTION, AffinityType.FLOOR_SUB_SECTION};

            // iterate over each floor in the building cleaning, and make level interval constraints
            Dictionary<string, List<LevelCleaningInterval>> allBldgCleaningIntervals = new Dictionary<string, List<LevelCleaningInterval>>();
            var morningflag = true;
            BuildingCleaningInterval building_interval = null;
            var hasPreplan = false;
            if (hotelStrategy.CPSat.UsePrePlan && attendant.Cleanings.Count() > 0)
            {
                maxRooms -= attendant.Cleanings.Count();
                maxCredits -= attendant.Cleanings.Sum(c => c.Credits);
                hasPreplan = true;
            }
            Console.WriteLine($"entering make building cleanings loop with maxRooms {maxRooms} and maxCredits {maxCredits}");

            // sort the buildings
            buildingsCleanings.Sort(delegate (BuildingCleanings a,
                                              BuildingCleanings b)
            {
                // preplan first
                // then affinity
                // then least already assigned
                // (maybe then use building distance from most affinity building, but that is complicated)
                // then most priority cleanings
                // then most cleanings

                // does the attendant have a preplan, and if so, which buildings
                if (hasPreplan)
                {
                    var aPreplan = a.MatchesPreplanned(attendant);
                    var bPreplan = b.MatchesPreplanned(attendant);
                    // Console.WriteLine($"preplan for {a.BuildingName} is {aPreplan}, preplan for {b.BuildingName} is {bPreplan}");
                    if (aPreplan != bPreplan)
                    {
                        if (aPreplan) return -1;
                        else if (bPreplan) return 1;
                    }
                }
                // does the attendant have affinity, and if so,
                // affinity for a level in one of the buildings
                var aAffinity = false;
                var bAffinity = false;
                if (affinities.Count() > 0)
                {
                    aAffinity =
                        affinities.Any( affinity => affinity.Building == a.BuildingName);
                    bAffinity =
                        affinities.Any( affinity => affinity.Building == b.BuildingName);
                }
                if (aAffinity != bAffinity)
                {
                    if (aAffinity) return -1;
                    else if (bAffinity) return 1;
                }
                // still here, so sort on least assigned levels in building
                if (a.countUnassignedCleanings() != b.countUnassignedCleanings())
                {
                    return b.countUnassignedCleanings().CompareTo(a.countUnassignedCleanings());
                }
                // still here, sort on total rooms
                return b.Count.CompareTo(a.Count);

            });

            // if (floorAffinities.Count() > 0)
            // {
            //     // debugging
            //     buildingsCleanings.ForEach(l => Console.WriteLine(l.BuildingName));
            //     Console.WriteLine(floorAffinities[0]);
            // }

            // if I've filled up all the buildings and still have attendants, then start over
            var cleaningsRemaining = buildingsCleanings.Sum( bc => bc.countUnassignedCleanings());
            Console.WriteLine($"remaining cleanings unassigned is {cleaningsRemaining}");

            if (cleaningsRemaining < maxRooms)
            {
                foreach (var buildingCleanings in buildingsCleanings)
                {
                    buildingCleanings.resetCleaningCounters();
                }
            }

            foreach (var buildingCleanings in buildingsCleanings)
            {
                List<LevelCleaningInterval> thisBldgCleaningIntervals = new List<LevelCleaningInterval>();
                if (attendantCleanings.isFullyAssigned())
                {
                    break;
                }
                var currentUnassignedCleanings = buildingCleanings.countUnassignedCleanings();
                Console.WriteLine($"current cleanings unassigned for building {buildingCleanings.BuildingName} is {currentUnassignedCleanings}");
                if (! buildingCleanings.fullyAssignedMorning())
                {
                    Console.WriteLine("considering buildingCleanings");
                    var morningLevelsCleanings = buildingCleanings.MorningLevelsCleanings;
                    LevelIntervalConstraints.Make_Level_Intervals(
                        model,
                        attendant,
                        attendantCleanings,
                        attendant_interval,
                        morningLevelsCleanings,
                        attendant_cleaning_intervals,
                        thisBldgCleaningIntervals,
                        attendant_room_travel,
                        attendant_floor_travel,
                        levelsCleaned,
                        earliest_date,
                        max_travel_time,
                        affinities,
                        room_literals,
                        rooms_cleaned,
                        roomDistances,
                        hotelStrategy
                    );
                    Console.WriteLine($"done making level intervals for the morning, got back {thisBldgCleaningIntervals.Count()}");
                    if (thisBldgCleaningIntervals.Count() == 0)
                    {
                        // Console.WriteLine("building cleaning intervals count is zero for the morning");
                        // which means attendant cannot be used for this building, probably due to some exclusivity constraint conflict
                        morningflag = false;
                    }
                    else
                    {
                        attendant_level_cleanings.AddRange(thisBldgCleaningIntervals);
                        allBldgCleaningIntervals.Add(buildingCleanings.BuildingName, thisBldgCleaningIntervals);

                        building_interval = BuildingCleaningInterval.CreateIntervalVar(model, attendant, 0, "", thisBldgCleaningIntervals);
                        // add to the list
                        attendant_building_intervals.Add(building_interval);
                        AttendantToIntervalTiming(model, attendant_interval, building_interval);
                    }
                    Console.WriteLine($"after AM loop, attendant is fully assigned? {attendantCleanings.isFullyAssigned()}");

                }
                if(buildingCleanings.AfternoonCount > 0 && ! buildingCleanings.fullyAssignedAfternoon() && ! buildingCleanings.fullyAssignedMorning())


                {
                    var pm_start = buildingCleanings.GetPMStart();
                    var pm_flag = $"{pm_start}";

                    List<LevelCleaningInterval> pmBldgCleaningIntervals = new List<LevelCleaningInterval>();
                    LevelIntervalConstraints.Make_Level_Intervals(
                        model,
                        attendant,
                        attendantCleanings,
                        attendant_interval,
                        //buildingCleanings.LevelsCleanings,
                        buildingCleanings.AfternoonLevelsCleanings,
                        attendant_cleaning_intervals,
                        pmBldgCleaningIntervals,
                        attendant_room_travel,
                        attendant_floor_travel,
                        levelsCleaned,
                        earliest_date,
                        max_travel_time,
                        affinities,
                        room_literals,
                        rooms_cleaned,
                        roomDistances,
                        hotelStrategy,
                        pm_flag
                    );
                    Console.WriteLine($"done making pm level intervals, got back {pmBldgCleaningIntervals.Count()}");
                    if (pmBldgCleaningIntervals.Count() == 0)
                        continue;
                    attendant_level_cleanings.AddRange(pmBldgCleaningIntervals);
                    var pm_building_interval = BuildingCleaningInterval.CreateIntervalVar(model, attendant, pm_start, pm_flag, pmBldgCleaningIntervals);
                    // add to the list
                    attendant_building_intervals.Add(pm_building_interval);
                    AttendantToIntervalTiming(model, attendant_interval, pm_building_interval);
                    if (morningflag)
                    {
                        // ordering pm after am
                        var litAMPM = model.NewBoolVar($"building {pm_building_interval.BuildingName} visited AM and PM by {attendant.Username}");
                        // visiting AM and PM implies litAMPM
                        model.AddBoolOr(new ILiteral[] {building_interval.Literal.Not(), pm_building_interval.Literal.Not(), litAMPM});
                        // litAMPM implies visiting AM and PM both true
                        model.AddImplication(litAMPM, building_interval.Literal);
                        model.AddImplication(litAMPM, pm_building_interval.Literal);

                        model.Add(pm_building_interval.Start >= building_interval.End).OnlyEnforceIf(litAMPM);
                    }
                    Console.WriteLine($"after PM loop, attendant is fully assigned? {attendantCleanings.isFullyAssigned()}");
                }
            }
        }
        // Helper to compare by building name, preferring numeric sort if possible
        private static int MaybeInt(BuildingCleaningInterval a)
        {
            // extract numeric portion from the name
            Regex rx = new Regex(@"\d+", RegexOptions.Compiled);
            // Find matches.
            MatchCollection matches = rx.Matches(a.BuildingName);
            if (matches.Count() > 0 && Int32.TryParse(matches[0].Value, out int anum))
            {
                return anum;
            }
            else
            {
                return -1;
            }
        }


        private static void AttendantToIntervalTiming(CpModel model,
                                                      AttendantInterval attendant_interval,
                                                      BuildingCleaningInterval building_interval)
        {
            model.Add(attendant_interval.Start <= building_interval.Start).OnlyEnforceIf(building_interval.Literal);
            model.Add(attendant_interval.End >= building_interval.End).OnlyEnforceIf(building_interval.Literal);

            // as there is only one, set a hard implication: if a building is cleaned, the attendant is being used
            model.AddImplication(building_interval.Literal, attendant_interval.Literal);
        }



        public static void ConstrainAttendantBuildingIntervals(
            CpModel model,
            Attendant attendant,
            AttendantInterval attendant_interval,
            List<BuildingCleaningInterval> attendant_building_intervals,
            List<IntVar> attendant_building_travel,
            IntVar buildingsCleaned,
            Distances campus_distance_matrix,
            HotelStrategy hotelStrategy
        )
        {
            // The intervals cannot overlap, meaning the attendant
            // must do one building at a time and also that the
            // attendant cannot come back to this building later
            var buildings_intervals = attendant_building_intervals.Select(abi => abi.Interval);
            model.AddNoOverlap(buildings_intervals);

            int max_building_travel_time = hotelStrategy.CPSat.maxBuildingTravelTime; // max of sum of bldg to bldg travel time
            int max_building_to_building_distance_allowed = hotelStrategy.CPSat.maxBuildingToBuildingDistanceAllowed;

            // count up building changes by creating an equality
            var buildings_literals = attendant_building_intervals.Select(abi => abi.Literal);
            var totalBuildingsCleaned = LinearExpr.Sum(buildings_literals);
            model.Add(totalBuildingsCleaned == buildingsCleaned);
            model.Add(buildingsCleaned == 0).OnlyEnforceIf(attendant_interval.Literal.Not());
            model.Add(buildingsCleaned > 0).OnlyEnforceIf(attendant_interval.Literal);

            bool sortBuildingIntervals = hotelStrategy.CPSat.sortBuildingIntervals;

            // order the buildings in preferred visit order
            var sortedBuildings = Sort_Building_Intervals(attendant_building_intervals, campus_distance_matrix);
            var sortedLookup = new Dictionary<string, int>();
            if (sortBuildingIntervals)
            {
                for (var i = 0; i < sortedBuildings.Count(); i++)
                {
                    var earlyBuilding = sortedBuildings[i];
                    var thiskey = $"{earlyBuilding.BuildingName}-{earlyBuilding.From}";
                    sortedLookup.Add(thiskey, i);
                    // Console.WriteLine($"added thiskey = {thiskey} to sorted lookup with value {i}");
                    for (var j = i+1; j < sortedBuildings.Count(); j++)
                    {
                        var lateBuilding = sortedBuildings[j];
                        var lit = model.NewBoolVar($"building {earlyBuilding.BuildingName} and  {lateBuilding.BuildingName} are both visited by {attendant.Username}");
                        // visiting both buildings implies lit is true
                        model.AddBoolOr(new ILiteral[] {earlyBuilding.Literal.Not(), lateBuilding.Literal.Not(), lit});
                        // lit implies visiting both is true
                        model.AddImplication(lit, earlyBuilding.Literal);
                        model.AddImplication(lit, lateBuilding.Literal);

                        // now lit represents that both early building and late building *are* visted by this attendant.  So can use that literal as a flag to enforce that early building must be cleaned before late building
                        // Console.WriteLine($"{lit} is true if {earlyBuilding.Literal} and {lateBuilding.Literal}, and it implies {earlyBuilding.End} <= {lateBuilding.Start}");
                        model.Add(earlyBuilding.End < lateBuilding.Start).OnlyEnforceIf(lit);
                    }
                }
            }


            // now order the building to building shifts
            // Console.WriteLine($"create betweenBuildingsTravelTime from 0 to {max_building_travel_time}");
            var betweenBuildingsTravelTime = model.NewIntVar(0, max_building_travel_time, $"atndnt {attendant.Username}, building to building travel time");
            Building_To_Building_Ordering(model, attendant, attendant_building_intervals,
                                          betweenBuildingsTravelTime, attendant_interval.Literal, max_building_to_building_distance_allowed, campus_distance_matrix, sortedLookup);
            attendant_building_travel.Add(betweenBuildingsTravelTime);
        }


        public static List<BuildingCleaningInterval> Sort_Building_Intervals(
            List<BuildingCleaningInterval> attendant_building_cleanings,
            Distances campus_distance_matrix
        )
        {
            // starting from the first building in the list, sort all
            // buildings from nearest to first building to the farthest.

            var firstBuildingName = attendant_building_cleanings[0].BuildingName;
            // Log.GetLog().Debug($"sorted buildings will start from {firstBuildingName}");
            // var sortedBuildings = new List<BuildingCleaningInterval>(attendant_building_cleanings);
            // sort the list first based on start time, then on location relative to "first" building

            var sortedBuildings = attendant_building_cleanings.OrderBy(i => i.From)
                .ThenBy(i => BuildingsMath.BuildingToBuildingDistance(firstBuildingName, i.BuildingName, campus_distance_matrix))
                .ThenBy(MaybeInt)
                .ThenBy(i => i.BuildingName)
                .ToList();


            //             sortedBuildings.Sort(delegate(BuildingCleaningInterval a, BuildingCleaningInterval b)
            // {
            //     // if ((a.From >= timesplit && b.From < timesplit) ||
            //     //     (a.From < timesplit && b.From >= timesplit))
            //     //     return (b.From).CompareTo(a.From);

            //     var distanceToA = BuildingsMath.BuildingToBuildingDistance(
            //         first.BuildingName,
            //         a.BuildingName,
            //         campus_distance_matrix);

            //     var distanceToB = BuildingsMath.BuildingToBuildingDistance(
            //         first.BuildingName,
            //         b.BuildingName,
            //         campus_distance_matrix);

            //     if (distanceToA == distanceToB)
            //         return a.BuildingName.CompareTo(b.BuildingName);
            //     else
            //         return distanceToA.CompareTo(distanceToB);
            // });
            // debugging
            //Log.GetLog().Debug("sorted buildings for cleaning in order is");
            // sortedBuildings.ForEach(b => Log.GetLog().Debug($"{b.From}, {b.BuildingName}, {BuildingsMath.BuildingToBuildingDistance(firstBuildingName, b.BuildingName, campus_distance_matrix)}"));
            // debugging
            return sortedBuildings;
        }

        // Building to building ordering, via circuit constraint
        // arguments are
        // the model,
        // the attendant,
        // the list of cleaning decision variable for the attendant
        // the IntVar representing the bounds for travel time.
        // the IntVar representing if the attendant is being used
        private static void Building_To_Building_Ordering(
            CpModel model,
            Attendant a,
            List<BuildingCleaningInterval> attendant_building_cleanings,
            IntVar betweenBuildingsTravelTime,
            IntVar attendantLiteral,
            int max_building_to_building_distance_allowed,
            Distances campus_distance_matrix,
            Dictionary<string, int> sortedLookup
        )
        {
            // work out the ordering and travel time between the buildings the attendant might clean

            var arcs = new List<Tuple<int, int, ILiteral>>();
            // a place to stash cleaning to cleaning transition literals
            // for travel count, times
            var transition_literals = new List<IntVar>();
            var transition_times = new List<int>();

            var skip_buildings_attendant_lit = model.NewBoolVar($"atndnt {a.Username} is not used, so skip all buildings");
            var whole_arc_skip = new Tuple<int, int, ILiteral>(0, 0, skip_buildings_attendant_lit); //a loop to itself =  skip the origin node, and skip the whole arc constraint
            arcs.Add(whole_arc_skip);

            // link the skip literal to the clean buildings literal
            // if the attendant is not cleaning, then nothing to do here (skip buildings is true)
            model.AddImplication(attendantLiteral.Not(), skip_buildings_attendant_lit);


            for (int j1 = 0; j1 < attendant_building_cleanings.Count; ++j1) //j1 is the first building, j2 is the 2nd building
            {
                var this_building_decision = attendant_building_cleanings[j1];
                var buildingname = this_building_decision.BuildingName;
                var fromtime = this_building_decision.From;
                //every building can be potentially first or last, this the loop
                //We're listing all the first

                var start_lit = model.NewBoolVar($"atndnt {a.Username}: {buildingname}-{fromtime} is first building");

                this_building_decision.IsFirstLiteral = start_lit;

                var arc0 = new Tuple<int, int, ILiteral>(0, j1 + 1, start_lit);
                arcs.Add(arc0);

                //We're listing all the finals
                // Final arc from an arc to the dummy node.
                var end_lit = model.NewBoolVar($"atndnt {a.Username}: {buildingname}-{fromtime} is last building");
                var arcLast = new Tuple<int, int, ILiteral>(j1 + 1, 0, end_lit);
                arcs.Add(arcLast);

                //We're listing all the Skipped
                // if this building is not visited by this attendant, need a self-terminating arc
                var skip_lit = model.NewBoolVar($"atndnt {a.Username}: {buildingname}-{fromtime} is skipped");
                var arc_skip = new Tuple<int, int, ILiteral>(j1 + 1, j1 + 1, skip_lit); //a loop to itself =  skip that node in the circuit
                arcs.Add(arc_skip);

                // link the skip literal to the clean building literal
                // if not cleaning the building, then skip self-loop is true
                model.Add(skip_lit != this_building_decision.Literal);
                model.AddImplication(attendantLiteral.Not(), skip_lit);

                // Console.WriteLine($"skip lit is {skip_lit}");
                // Console.WriteLine($"attendant literal is {attendantLiteral}");

                // now consider the building that comes after j1 IN THE LOOP
                for (int j2 = 0; j2 < attendant_building_cleanings.Count; ++j2)
                {
                    if (j1 == j2)
                    {
                        // cannot transition from a building to itself, so skip this case
                        continue;
                    }
                    BuildingCleaningInterval next_building_decision = attendant_building_cleanings[j2];
                    var next_buildingname = next_building_decision.BuildingName;
                    var next_fromtime = next_building_decision.From;
                    var thiskey = $"{buildingname}-{fromtime}";
                    var nextkey = $"{next_buildingname}-{next_fromtime}";
                    // in the case that buildings are sorted, use that here to eliminate some transitions
                    if (sortedLookup.ContainsKey(thiskey) &&
                        sortedLookup.ContainsKey(nextkey))
                    {
                        // Console.WriteLine($"lookup: thiskey {thiskey}  is {sortedLookup.ContainsKey(thiskey)}, nextkey {nextkey} is {sortedLookup.ContainsKey(nextkey)}");
                        if (sortedLookup[thiskey] > sortedLookup[nextkey])
                        {
                            // cannot transition against the sort
                            continue;
                        }
                    }
                    //If you don't want to allow going from building j1 to building j2, just skip then "continue"
                    int building_distance = BuildingsMath.BuildingToBuildingDistance(buildingname, next_buildingname, campus_distance_matrix);
                    if (building_distance > max_building_to_building_distance_allowed)
                    {
                        // Log.GetLog().Warning($"Forbid building shift from {buildingname} to {next_buildingname} because distance of {building_distance} exceeds {max_building_to_building_distance_allowed}");
                        continue;
                    }
                    // Log.GetLog().Warning($"Considering building shift from {buildingname} to {next_buildingname} (change {building_distance} minutes) does not exceed {max_building_to_building_distance_allowed}");

                    var lit = model.NewBoolVar($"atndnt {a.Username}: {next_buildingname}-{next_fromtime} follows {buildingname}-{fromtime}");
                    // Console.WriteLine($"considering building to building transition literal: {lit}");
                    var arc_j1_j2 = new Tuple<int, int, ILiteral>(j1 + 1, j2 + 1, lit);
                    arcs.Add(arc_j1_j2);

                    // We add the reified precedence to link the literal with the
                    // times of the two tasks.

                    // FIXME: In order for this constraint to work
                    // properly, it MUST be the case that
                    // BuildingToBuilding returns an accurate
                    // estimate of the time it takes to travel
                    // from "this cleaning building" to "next cleaning building"
                    // Console.WriteLine($"possible constraint: {next_building_decision.Start} >= {this_building_decision.End} + {building_distance}");
                    model.Add(next_building_decision.Start >= this_building_decision.End + building_distance).OnlyEnforceIf(lit);
                    // allow at most 1 hour between building end and building start
                    model.Add(next_building_decision.Start <= this_building_decision.End + building_distance + 60).OnlyEnforceIf(lit);

                    // collect the j_k literal, and the j_k distance for the objective fn
                    transition_literals.Add(lit);
                    transition_times.Add(building_distance);

                    // have to link the literal back with assigned building literals
                    model.AddImplication(lit, this_building_decision.Literal);
                    model.AddImplication(lit, next_building_decision.Literal);
                }
            }

            model.AddCircuit(arcs);

            // diagnostic
            // Console.WriteLine(string.Join(",\n", arcs));
            // Console.WriteLine(string.Join(", ", transition_times));
            // Console.WriteLine(string.Join(", ", transition_literals));
            // add constraint on travel time
            var totalTravelTime = LinearExpr.ScalProd(transition_literals, transition_times);
            // set the constraint by forcing this equality.  It
            // also links the passed in IntVar travelTime to be
            // equal to the sum of all of the travel times for
            // this attendant.
            // Console.WriteLine($"constraining totalTravelTime {totalTravelTime} to equal between buildings travel time {betweenBuildingsTravelTime}");
            model.Add(totalTravelTime == betweenBuildingsTravelTime);

            return;

        }


    }
}
