using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Google.OrTools.Sat;


namespace Planner.CpsatCleaningCalculator
{
    public class CleaningIntervalConstraints
    {

        private static void ProcessCleaning(CpModel model,
                                            Attendant attendant,
                                            Cleaning cleaning,
                                            List<CleaningInterval> cleaning_intervals,
                                            AttendantInterval attendant_interval,
                                            List<Affinity> affinities,
                                            Dictionary<Cleaning, List<IntVar>> room_literals,
                                            DateTime earliest_date,
                                            string buildingName,
                                            HotelStrategy hotelStrategy
        )
        {
            var cleaningSpeed = GetCleaningSpeed(attendant);
            int max_floor_shift_allowed = hotelStrategy.CPSat.maxShiftFloorAllowed; // attempting to consider actual floors changed -- units are numbers of floors

            // skip cleanings that are impossible
            if (CleaningIsImpossible(cleaning, cleaningSpeed))
            {
                hotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"{cleaning.Room.RoomName} is impossible to clean.  Credits exceeds allowed time window.");
                    Console.WriteLine($"{attendant.Username} cannot clean {cleaning.Room.RoomName}");
                    return;
            }

            // skip cleanings that are unlikely
            if (affinities.Count() > 0 && !LevelsMath.Can_Attendant_Clean(attendant, affinities, cleaning, earliest_date, max_floor_shift_allowed))
            {
                hotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"{attendant.Username} cannot clean {cleaning.Room.RoomName}");
                Console.WriteLine($"{attendant.Username} cannot clean {cleaning.Room.RoomName}");
                return;
            }

            // short circuit if attendant is a cleaning team and the credits is not high enough
            if (cleaningSpeed > 1 && cleaning.Credits < hotelStrategy.CPSat.minCreditsForMultipleCleanersCleaning)
            {
                Console.WriteLine($"{attendant.Username} paired with {attendant.SecondWorkerUsername} cannot clean {cleaning.Room.RoomName} as the credits are below the threshold of {hotelStrategy.CPSat.minCreditsForMultipleCleanersCleaning}");
                return;
            }

            var cleaning_interval = CleaningInterval.CreateIntervalVar(model, attendant, earliest_date, cleaning, buildingName, hotelStrategy);
            // Console.WriteLine($"CleaningIntervalConstraint : earliest_date:" + earliest_date);
            // Console.WriteLine($"Start-End : {cleaning_interval.Start} - {cleaning_interval.End} :{cleaning_interval.Duration} min");
            // link cleaning interval literal with attendant work period

            // if literal is true, that means this attendant
            // does this cleaning.  , must happen inside of
            // the attendant's work period (or periods, if
            // multiple intervals (future extension))
            model.AddImplication(cleaning_interval.Literal, attendant_interval.Literal);

            //We fill attendant cleanings list with a new cleaning decision variable
            cleaning_intervals.Add(cleaning_interval);
            // room_literals is a convenient construction to
            // make sure each cleaning is only performed once
            if (room_literals.ContainsKey(cleaning)) //literals is a true or false
            {
                room_literals[cleaning].Add(cleaning_interval.Literal);
            }
            else
            {
                var room_list = new List<IntVar> { };
                room_list.Add(cleaning_interval.Literal);
                room_literals.Add(cleaning, room_list);
            }
            return;
        }

        // MakeCleaningIntervals
        //
        // The idea here is to speed up the solver by grouping
        // together all cleanings on a level under a single
        // IntervalVar for the level.  Also add the necessary
        // constraints to the model to make this happen.
        //
        // If a cleaning cannot happen inside of the level
        // IntervalVar bounds, then it cannot be done by this
        // attendant.
        //
        // The plan is to use this approach to group together all
        // cleanings on a level, and also to minimize travel
        // between levels without having to examine each
        // combination of room to room...just examine level to
        // level.
        //
        // arguments are
        // the model,
        // the attendant,
        // the list of cleaning decision variable for the attendant
        // a list of CleaningInterval will be filled, one per cleaning, based on the cleanings that are possible for this attendant
        //
        public static void Make_Cleaning_Intervals(
            CpModel model,
            Attendant attendant,
            IEnumerable<Cleaning> cleanings,
            List<CleaningInterval> attendant_cleaning_intervals,
            AttendantInterval attendant_interval,
            List<Affinity> affinities,
            Dictionary<Cleaning, List<IntVar>> room_literals,
            DateTime earliest_date,
            string buildingName,
            HotelStrategy hotelStrategy
        )
        {
            // Console.WriteLine($"make cleaning intervals call, {cleanings.Count} possible cleanings for {attendant.Username}");
            foreach (Cleaning cleaning in cleanings)
            {
                // Console.WriteLine($"considering {cleaning.From}, {cleaning.To}, {cleaning.Room.RoomName} ");
                ProcessCleaning(model, attendant, cleaning,
                                attendant_cleaning_intervals,
                                attendant_interval,
                                affinities,
                                room_literals,
                                earliest_date,
                                buildingName,
                                hotelStrategy);

            }
        }

        public static void ConstrainAttendantCleaningIntervals(
            CpModel model,
            Attendant attendant,
            AttendantInterval attendant_interval,
            List<CleaningInterval> attendant_cleaning_intervals,
            IntVar rooms_cleaned,
            Distances roomDistances
        )
        {
            // Each possible work interval for this attendant
            // cannot overlap with others (a person can only do
            // one cleaning at a time)
            model.AddNoOverlap(attendant_cleaning_intervals.Select(c => c.Interval));

            // ===== total rooms cleaned per attendant =====
            // add constraint on total rooms per attendant
            var totalRoomsCleaned = LinearExpr.Sum(attendant_cleaning_intervals.Select(c => c.Literal));
            // create a variable, ranging from min rooms to max rooms, as constraint

            // set the constraint by linking to passed-in rooms_cleaned IntVar
            model.Add(totalRoomsCleaned == rooms_cleaned);

            // link attendant literal with model results
            model.Add(totalRoomsCleaned > 0).OnlyEnforceIf(attendant_interval.Literal);
            model.Add(totalRoomsCleaned == 0).OnlyEnforceIf(attendant_interval.Literal.Not());


            // A possible strategy to make a soft bound, will be
            // to create a variable that is the diff between the
            // target and the actual, then give it a coefficient,
            // then add it to the optimization goal to minimize

            // maybe clean up the level interval creation code and put
            // the room to room distance call here?  similar to
            // building constraints code
        }


        // return void for now, but maybe return the list of boolvar
        // for the transitions between cleanings??  But it seems like it
        // would be useless
        // inside my level, i want to order
        // arguments are
        // the model,
        // the attendant,
        // the list of cleaning decision variable for the attendant
        // the IntVar representing the bounds for travel time.
        // the IntVar representing the bounds for level changes.
        public static void Cleaning_To_Cleaning_Ordering(
            CpModel model,
            Attendant a,
            List<CleaningInterval> attendant_cleaning_intervals,
            IntVar betweenRoomsTravelTime,
            LevelCleaningInterval attendant_level_interval,
            Distances roomDistances,
            CPSatSolutionInformation cpsi
        )
        {
            // now work out the order of the rooms the attendant will clean
            var maxgap = 10;
            var arcs = new List<Tuple<int, int, ILiteral>>();
            // a place to stash cleaning to cleaning transition literals
            // for travel count, times
            var transition_literals = new List<IntVar>();
            var transition_times = new List<int>();

            var attendantLevelLiteral = attendant_level_interval.Literal;
            var skip_attendant_level_lit = model.NewBoolVar($"atndnt {a.Username} is not used");
            var whole_arc_skip = new Tuple<int, int, ILiteral>(0, 0, skip_attendant_level_lit); //a loop to itself =  skip the origin node, and skip the whole arc constraint
            arcs.Add(whole_arc_skip);

            // link the skip literal to the passed-in attendant level
            // cleaning literal if not using the attendant to clean
            // the level at all, then skip self-loop is true
            model.Add(skip_attendant_level_lit != attendantLevelLiteral);

            for (int j1 = 0; j1 < attendant_cleaning_intervals.Count; ++j1) //j1 is the first job she's doing, j2 is the 2nd job
            {

                var this_cleaning_decision = attendant_cleaning_intervals[j1];
                var this_cleaning = this_cleaning_decision.Cleaning;

                int zero_to_room_distance = RoomsMath.RoomToRoomDistance(
                    this_cleaning.Room, roomDistances
                );

                //every room can be potentially first or last, this the loop
                //We're listing all the first

                var roomName = this_cleaning.Room.RoomName;
                var start_lit = model.NewBoolVar($"atndnt {a.Username} cleaning {roomName} is first job on level");
                this_cleaning_decision.IsFirstLiteral = start_lit;
                model.AddImplication(start_lit, attendantLevelLiteral);
                model.AddImplication(start_lit, this_cleaning_decision.Literal);

                var arc0 = new Tuple<int, int, ILiteral>(0, j1 + 1, start_lit);
                arcs.Add(arc0);

                // collect the arc0 literal, and the 0 to j distance for the objective fn
                transition_literals.Add(start_lit);
                transition_times.Add(zero_to_room_distance);

                //We're listing all the finals
                // Final arc from an arc to the dummy node.
                var end_lit = model.NewBoolVar($"atndnt {a.Username} cleaning {roomName} is last job on level");
                this_cleaning_decision.IsLastLiteral = end_lit;
                model.AddImplication(end_lit, attendantLevelLiteral);
                model.AddImplication(end_lit, this_cleaning_decision.Literal);

                // as long as the time is not forced to a certain value, adjust the level cleaning time to account
                // for travel to the cleaning room if first or last
                if (!(this_cleaning_decision.From.HasValue))
                {
                    model.Add(attendant_level_interval.Start + zero_to_room_distance == this_cleaning_decision.Start).OnlyEnforceIf(start_lit);
                    model.Add(attendant_level_interval.End == this_cleaning_decision.End + zero_to_room_distance).OnlyEnforceIf(end_lit);
                }

                var arcLast = new Tuple<int, int, ILiteral>(j1 + 1, 0, end_lit);
                arcs.Add(arcLast);

                // collect the arclast literal, and the j to 0 distance for the objective fn
                transition_literals.Add(end_lit);
                transition_times.Add(zero_to_room_distance);

                //We're listing all the Skipped
                // if this room is not cleaned, need a self-terminating arc
                var skip_lit = model.NewBoolVar($"atndnt {a.Username} skips cleaning {roomName}");
                var arc_skip = new Tuple<int, int, ILiteral>(j1 + 1, j1 + 1, skip_lit); //a loop to itself =  skip that node in the circuit
                arcs.Add(arc_skip);

                // link the skip literal to the clean room literal
                // if not cleaning the room, then skip self-loop is true
                model.Add(skip_lit != this_cleaning_decision.Literal);
                model.AddImplication(attendantLevelLiteral.Not(), skip_lit);

                //If this level is not cleaned by attendant, then all skip lits will be true

                // now consider the job that comes after j1 IN THE LOOP
                for (int j2 = 0; j2 < attendant_cleaning_intervals.Count; ++j2)
                {
                    if (j1 == j2)
                    {
                        // cannot transition from a room to itself, so skip this case
                        continue;
                    }
                    //If you don't want to allow going from room j1 to room j2, just skip then "continue"
                    CleaningInterval next_cleaning_decision = attendant_cleaning_intervals[j2];
                    Cleaning next_cleaning = next_cleaning_decision.Cleaning;

                    var next_roomName = next_cleaning.Room.RoomName;

                    // if (!LevelsMath.Is_Transition_Allowed(this_cleaning.Room.Floor, next_cleaning.Room.Floor, max_floor_shift_allowed))
                    // {
                    //     continue;
                    // }

                    var lit = model.NewBoolVar($"atndnt {a.Username} cleaning {next_roomName} follows {roomName}");
                    // Console.WriteLine($"transition lit is {lit}");
                    var arc_j1_j2 = new Tuple<int, int, ILiteral>(j1 + 1, j2 + 1, lit);
                    arcs.Add(arc_j1_j2);

                    // We add the reified precedence to link the literal with the
                    // times of the two tasks.

                    // FIXME: In order for this constraint to work
                    // properly, it MUST be the case that
                    // RoomToRoomDistance returns an accurate
                    // estimate of the time it takes to travel
                    // from "this_cleaning" to "next_cleaning"
                    var distance = RoomsMath.RoomToRoomDistance(this_cleaning.Room, next_cleaning.Room, roomDistances);
                    // distance
                    var travelTime = distance;  // or maybe divide by a factor
                    // Only add a constraint relating the next start
                    // with the current end if at least one of the
                    // cleanings is preplanned already.
                    //
                    // if both cleanings are preplanned, then the
                    // interval is fixed by the user
                    if (!(this_cleaning_decision.From.HasValue && next_cleaning_decision.From.HasValue))
                    {
                        model.Add(next_cleaning_decision.Start >= this_cleaning_decision.End + (int)(travelTime)).OnlyEnforceIf(lit);
                        model.Add(next_cleaning_decision.Start <= this_cleaning_decision.End + (int)(travelTime) + maxgap).OnlyEnforceIf(lit);

                    }
                    else
                    {
                        // both cleanings are fixed, so change the travelTime
                        travelTime = (int)(next_cleaning_decision.From - this_cleaning_decision.To);
                        // maybe change travelTime to zero, because
                        // the optimizer can do nothing here?  but I
                        // think it makes no difference in that case.
                    }

                    // collect the j_k literal, and the j_k travel time for the objective fn
                    transition_literals.Add(lit);
                    transition_times.Add(travelTime);

                    // have to link the literal back with assigned room literals
                    model.AddImplication(lit, this_cleaning_decision.Literal);
                    model.AddImplication(lit, next_cleaning_decision.Literal);
                }
            }

            model.AddCircuit(arcs);

            // add constraint on travel time
            var totalTravelTime = LinearExpr.ScalProd(transition_literals, transition_times);
            // set the constraint by forcing this equality.  It
            // also links the passed in IntVar travelTime to be
            // equal to the sum of all of the travel times for
            // this attendant.
            model.Add(totalTravelTime == betweenRoomsTravelTime);

            return;

        }
        public static void SetCommunicatingRooms(
            CpModel model,
            List<CleaningInterval> attendant_cleaning_intervals,
            List<List<Room>> communicating_rooms,
            List<IEnumerable<CleaningInterval>> communicating_room_intervals,
            HotelStrategy hotelStrategy
        )
        {
            if (communicating_rooms == null)
                return ;

            /// Look at the communicating rooms list, see if all of
            /// them are in this attendant_cleaning_intervals.
            ///
            /// If so, then set implications that the rooms are all or
            /// nothing to this attendant.

            foreach (var linked_rooms in communicating_rooms)
            {
                // if all of the rooms are in the attendant_cleaning_intervals, then all-on or all-off
                // if only some of the rooms are in the intervals, then remove them all

                // a list to hold true or false
                var roomnames = linked_rooms.Select(r => r.RoomName);
                var matching_intervals = attendant_cleaning_intervals.Where(acl => roomnames.Contains(acl.Cleaning.Room.RoomName));
                if (matching_intervals.Count() == linked_rooms.Count())
                {
                    communicating_room_intervals.Add(matching_intervals);
                    // this is the case where all of the rooms are in
                    // the list of cleaning intervals.  So set them
                    // either all on, or all off.

                    // use boolean logic, tricky, but faster
                    var literals = matching_intervals.Select(i => i.Literal);
                    var not_literals = matching_intervals.Select(i => i.Literal.Not());
                    foreach (var lit in literals)
                    {
                        // all on if one is on
                        model.AddBoolAnd(literals).OnlyEnforceIf(lit);
                        // all off if one is off
                        model.AddBoolAnd(not_literals).OnlyEnforceIf(lit.Not());
                    }
                }
                else if (matching_intervals.Count() > 0)
                {
                    // some but not all intervals are possibly
                    // assigned to this attendant, so deny them all by setting literal to false
                    // (because the attendant cannot do them all, so force that none are done)
                    foreach (var interval in matching_intervals)
                    {
                        model.Add(interval.Literal == 0);
                    }
                }
            }
        }

        public static int GetCleaningSpeed(Attendant attendant)
        {
            var cleaningSpeed = 1;
            if (attendant.SecondWorkerUsername != null)
            {
                cleaningSpeed = 2;
            }
            return cleaningSpeed;
        }

        public static bool CleaningIsImpossible(Cleaning cleaning, int cleaningSpeed)
        {
            var allowedWindow = (long)(cleaning.To - cleaning.From).TotalMinutes;
            return allowedWindow < cleaning.Credits/cleaningSpeed;
        }

        public static int MakeRoomAward(Attendant attendant,
                                        List<Affinity> affinities,
                                        CleaningInterval this_cleaning_interval,
                                        CPSat CPSat
        )
        {

            var tempRoomAward = 0;
            if (this_cleaning_interval.From != null && this_cleaning_interval.To != null)
            {
                // this cleaning is preplanned, must be performed
                return 1000;
            }
            var this_cleaning = this_cleaning_interval.Cleaning;
            if (CleaningIsImpossible(this_cleaning, GetCleaningSpeed(attendant)))
                    return 0;

            // Console.WriteLine($"Considering award for {this_cleaning_interval.Interval}");

            int award_level = CPSat.awardLevel;
            int award_room = CPSat.awardRoom;
            int award_building = CPSat.awardBuilding;
            // var affinities = new List<Affinity>();
            // if (attendant.CurrentTimeSlot?.Affinities != null)
            // {
            //     affinities = attendant.CurrentTimeSlot.Affinities;
            // }

            //if (building is the prefered building for this attendant)
            if (affinities.Any(a => a.Building != null && a.Building == this_cleaning.Room.Building.LevelName))
            {
                tempRoomAward += award_building;
                Console.WriteLine($"building affinity boost: {award_building} to {tempRoomAward}");
            }

            // if (level or section or subsection  is the prefered level/section/subsection for this attendant)
            if (affinities.Any(a =>
            {
                var match = (
                    (a.Floor != null && a.Floor == this_cleaning.Room.Floor?.LevelName) ||
                    (a.Section != null && a.Section == this_cleaning.Room.Section?.LevelName) ||
                    (a.SubSection != null && a.SubSection == this_cleaning.Room.Subsection?.LevelName));
                return match;
            }))
            {
                tempRoomAward += award_level;
                Console.WriteLine($"level affinity boost: {award_level} to {tempRoomAward}");
            }

            //Affinity for cleaning type
            if (affinities.Any(a => a.CleaningType != null && a.CleaningType == this_cleaning.Type.ToString()))
            {
                tempRoomAward += award_room;
                Console.WriteLine($"cleaning type affinity boost: {award_room} to {tempRoomAward}");
            }



            if (CPSat.cleaningPriorities != null)
            {
                var cleaningPriorities = new List<string>() {CPSat.cleaningPriorities};
                if (cleaningPriorities.Contains("Others") &&
                    CPSat.otherCleaningPriorities!=null &&
                    CPSat.otherCleaningPriorities != "")
                {
                    cleaningPriorities.Remove("Others");
                    var otherCleaningPriorities = CPSat.otherCleaningPriorities.Split(',').ToList();
                    cleaningPriorities.AddRange(otherCleaningPriorities);
                }

                // I forget why I wanted to use a decrementing
                // priority mechanism here, but as I look at this
                // while working on the DEP/ARR priority, I cannot see
                // how it helps.  So commenting it out for now
                int stepdown = award_room / (cleaningPriorities.Count() + 1);
                var award_priority = award_room;// + stepdown;

                foreach (var priority in cleaningPriorities)
                {
                    // Console.WriteLine($"priority case is {priority}");
                    //award_priority -= stepdown;
                    if (award_priority < 1)
                        award_priority = 1;

                    switch (priority)
                    {
                        case "Departure":
                            if (this_cleaning.Type == CleaningType.Departure)
                            {
                                tempRoomAward += award_priority;
                            }
                            break;
                        case "Stay":
                            if (this_cleaning.Type == CleaningType.Stay)
                            {
                                tempRoomAward += award_priority;
                            }
                            break;
                        case "Arrival":
                            if (this_cleaning.ArrivalExpected) // changed from Type == Vacant Dirty
                            {
                                tempRoomAward += award_priority;
                            }
                            break;
                        case "DEP/ARR":
                            if (this_cleaning.Type == CleaningType.Departure && this_cleaning.ArrivalExpected)
                            {
                                tempRoomAward += award_priority;
                            }
                            break;
                        case "ARR/DEP":
                            if (this_cleaning.Type == CleaningType.Departure && this_cleaning.ArrivalExpected)
                            {
                                tempRoomAward += award_priority;
                            }
                            break;
                        case "ChangeSheet":
                            if (this_cleaning.Type == CleaningType.ChangeSheet)
                            {
                                tempRoomAward += award_priority;
                            }
                            break;

                        default:  // case "Others":
                            if (this_cleaning.Label != null && this_cleaning.Label.Contains(priority))
                            {
                                tempRoomAward += award_priority;
                            }
                            // else
                            // {
                            //     CPSat.SolutionResult.RoomcheckingInformation.Add($"Hint: cleaning priority {priority} not found in this room");
                            // }
                            break;
                    }
                }
            }

            if (attendant.CurrentTimeSlot.Affinities.Any(affinity => affinity.Room == this_cleaning.Room.PmsRoomName)
                 ||
                (attendant.CurrentTimeSlot.Affinities.Any(affinity => affinity.RoomType == this_cleaning.Room.RoomType)))

            {
                tempRoomAward += award_room;
                Console.WriteLine($"room or room-type match; room award is {tempRoomAward}");
            }
            if (tempRoomAward > 0)
                Console.WriteLine($"For {attendant.Username}-{this_cleaning_interval.Cleaning.Room.RoomName}, room award is {tempRoomAward}");

            return tempRoomAward;

        }

        public static void HintPreferredRooms(CpModel model,
                                              Attendant attendant,
                                              List<int> rooms_awards,
                                              List<CleaningInterval> attendant_cleaning_intervals,
                                              List<string> hinted_rooms,
                                              int max_rooms,
                                              int max_credits,
                                              int weight_cleaning_time,
                                              CPSat CPSat
        )
        {
            // do nothing if preplan and attendant has assigned rooms
            if (CPSat.UsePrePlan && attendant.Cleanings.Count()>0)
            {
                return;
            }
            // sort based on highest award to lowest award.
            // also maybe track hinted roooms?
            int cumul_rooms = max_rooms;
            if (attendant.CurrentTimeSlot.noOfRooms > 0)
                cumul_rooms = attendant.CurrentTimeSlot.noOfRooms;
            int cumul_credits = max_credits;
            if (attendant.CurrentTimeSlot.MaxCredits > 0)
                cumul_credits = attendant.CurrentTimeSlot.MaxCredits;

            var max_number_levels_per_attendant = CPSat.maxNumberOfLevelsPerAttendant;
            var levelsVisited = new List<string>();
            // sort cleanings based on highest room award to lowest
            var rooms_awards_dict = new Dictionary<CleaningInterval, int>();
            // brute force ugly assignment for now.  refactor later
            var min_credits = attendant_cleaning_intervals.Min(ci => ci.Cleaning.Credits);

            for (int i = 0; i < attendant_cleaning_intervals.Count; ++i)
            {
                if (rooms_awards[i] > 0)
                    rooms_awards_dict.Add(attendant_cleaning_intervals[i], rooms_awards[i]);
            }

            // now sort that dict
            var cleanings_awards_list = rooms_awards_dict.ToList();
            if (cleanings_awards_list.Count() == 0)
                return ;
            // still here, then there is something non zero to hint
            var sortedlist = cleanings_awards_list
                .OrderByDescending(x => x.Value)
                .ThenBy(f => PriorityBuilding(attendant, f.Key.Cleaning.Room?.Building.LevelName))
                .ThenBy(f => PriorityLevel(attendant, f.Key.Cleaning.Room?.Floor.LevelName))
                .ThenBy(b => b.Key.Cleaning.Room?.Building.LevelName)
                .ThenBy(f => ExtractPart(f.Key.Cleaning.Room.Floor.LevelName, 1)) //f.Key.Cleaning.Room.Floor.LevelName)
                .ThenBy(f => ExtractPart(f.Key.Cleaning.Room.Floor.LevelName, 2))
                .ThenBy(s => ExtractPart(s.Key.Cleaning.Room?.Section?.LevelName, 1))
                .ThenBy(s => ExtractPart(s.Key.Cleaning.Room?.Section?.LevelName, 2))
                .ThenBy(s => ExtractPart(s.Key.Cleaning.Room?.Subsection?.LevelName, 1))
                .ThenBy(s => ExtractPart(s.Key.Cleaning.Room?.Subsection?.LevelName, 2))
                .ThenBy(t => t.Key.Cleaning.From)
                .ThenBy(t => t.Key.Cleaning.To)
                .ThenBy(r => MaybeInt(r.Key.Cleaning.Room.RoomName))
                .ThenBy(r => r.Key.Cleaning.Room.RoomName);
            // debugging to see what is up with sort
            // Console.WriteLine(String.Join("\n ", sortedlist.Select(x => $"{x.Value}, {x.Key.Cleaning.From}, {x.Key.Cleaning.To}, {x.Key.Cleaning.Room.Building.LevelName}, {x.Key.Cleaning.Room.Floor.LevelName}, {x.Key.Cleaning.Room.Section.LevelName}, {x.Key.Cleaning.Room.RoomName}")));

            foreach (var kvp in sortedlist)
            {
                var cleaning_interval = kvp.Key;
                if (hinted_rooms.Contains(cleaning_interval.Cleaning.Room.RoomName))
                    continue;

                var thislevel = cleaning_interval.Cleaning.Room.Building.LevelName;
                if (cleaning_interval.Cleaning.Room.Floor != null)
                    thislevel = $"{thislevel}, {cleaning_interval.Cleaning.Room.Floor.LevelName}";
                if (cleaning_interval.Cleaning.Room.Section != null)
                    thislevel = $"{thislevel}, {cleaning_interval.Cleaning.Room.Section.LevelName}";
                if (cleaning_interval.Cleaning.Room.Subsection != null)
                    thislevel = $"{thislevel}, {cleaning_interval.Cleaning.Room.Subsection.LevelName}";
                if (!levelsVisited.Contains(thislevel))
                {
                    if (levelsVisited.Count() < max_number_levels_per_attendant) // keep allowing level changes up to limit
                    {
                        levelsVisited.Add(thislevel);
                    }
                    else
                    {
                        // try the next entry in the list, maybe it is on a level we already visited
                        // Console.WriteLine("should move on to next entry in award list");
                        continue;
                    }
                }

                // not sure if this is really effective
                model.AddHint(cleaning_interval.Literal, 1);

                // Console.WriteLine($"Setting hint for {cleaning_interval.Literal}, award is {kvp.Value} {cumul_rooms} {cumul_credits} {levelsVisited.Count()}");

                hinted_rooms.Add(cleaning_interval.Cleaning.Room.RoomName);
                cumul_rooms -= 1;
                cumul_credits -= cleaning_interval.Cleaning.Credits;

                if (cumul_rooms <= 0)
                    break;
                if (cumul_credits < min_credits)
                    break;
            }
            if (weight_cleaning_time > 0 && (cumul_rooms <= 0 || cumul_credits < min_credits) && ! CPSat.balanceStayDepartMode)
            {
                // there are definitely enough "award" rooms for this
                // person to get a full schedule, so tell the solver
                // all other rooms are deprecated.
                //
                // the hard approach here is to set all award=0 rooms literal to false
                //
                // the softer approach is to set all award=0 rooms to
                // have a negative award.  Then the attendant can
                // clean them if really there is nothing else, but
                // cleaning them is less beneficial than another
                // attendant cleaning them.
                //
                // not sure which is best right now, trying the soft approach first

                // Console.WriteLine("hints are sufficient for a full schedule, so deprecating non-award rooms");
                // for (int i = 0; i < attendant_cleaning_intervals.Count; ++i)
                // {
                //     if (rooms_awards[i] <= 0)
                //     {
                //         rooms_awards[i] = (int)(-1 * attendant_cleaning_intervals[i].Cleaning.Credits /2); // * (weight_cleaning_time/2));

                //     }
                // }
            }

        }
        private static int MaybeInt(string input)
        {
            if (Int32.TryParse(input, out int x1))
            {
                return x1;
            }
            return 0;
        }

        private static int ExtractPart(string input, int part)
        {
            if (input == null)
                return 0;
            string pattern = "-|\\.";
            var xmatches = Regex.Split(input, pattern);
            if (xmatches.Count() >= part)
            {
                if (Int32.TryParse(xmatches[part-1], out int x1))
                {
                    return x1;
                }
            }
            return MaybeInt(input);

        }

        private static int PriorityBuilding(Attendant attendant, string input)
        {
            if (input == null)
                return 0;
            //if (attendant.CurrentTimeSlot.Affinity?.Buildings?.Split(',').Contains(input) == true)

            if (attendant.CurrentTimeSlot.Affinities.Any(affinity => affinity.Building == input))
                return -1;
            return 1;
        }

        private static int PriorityLevel(Attendant attendant, string input)
        {
            if (input == null)
                return 0;


            //if (attendant.CurrentTimeSlot.Affinity?.Levels?.Split(',').Contains(input) == true)
           if (attendant.CurrentTimeSlot.Affinities.Any(affinity => affinity.Level == input))
           return -1;
         return 1;
        }

    }
}
