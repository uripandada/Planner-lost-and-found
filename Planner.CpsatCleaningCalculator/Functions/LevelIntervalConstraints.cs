using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Google.OrTools.Sat;


namespace Planner.CpsatCleaningCalculator
{
    public class LevelIntervalConstraints
    {
        // JEM: I'm not a fan of this change (both bools default to false), but needed short term.  Eventually will want to pass in real values for the bools
        public static string MakeLevelName(Cleaning cleaning, bool useSections = false, bool useSubsections = false)
        {
            string AllLevelsName = "";
            // check if Buildings, Sections, Subsections are being used for this Room
            // (there is probably a clever C# construct for this)
            if (cleaning.Room.Building != null)
            {
                // using buildings
                AllLevelsName += cleaning.Room.Building.LevelName;
            }
            // all rooms should at least have a floor defined
            AllLevelsName += $":{cleaning.Room.Floor.LevelName}";
            if (useSections && cleaning.Room.Section.LevelName != null)
            {
                // using sections
                AllLevelsName += $":{cleaning.Room.Section.LevelName}";
            }
            if (useSubsections && cleaning.Room.Subsection.LevelName != null)
            {
                // using subsections
                AllLevelsName += $":{cleaning.Room.Subsection.LevelName}";
            }
            return AllLevelsName;
        }

        // // This will soon be deprecated, I think, as it serves no
        // // purpose when the optional intervals are limited from the
        // // very beginning.
        // public static void AssignPreferredLevels(List<Attendant> attendants,
        //                                          List<Cleaning> cleanings,
        //                                          IEnumerable<BuildingCleanings> buildingsCleanings,
        //                                          HotelStrategy strategy)
        // {


        //     // this will turn off assigning level affinity if any
        //     // attendants have a level affinity
        //     var noPreferredLevel = attendants.Where(a => a.CurrentTimeSlot?.Affinity == null ||
        //                                             (a.CurrentTimeSlot?.Affinity.Levels == null &&
        //                                              a.CurrentTimeSlot?.Affinity.Buildings == null));
        //     // List<Attendant> noPreferredBuilding = attendants.Where(a => a.CurrentTimeSlot?.Affinity == null || a.CurrentTimeSlot?.Affinity.Buildings == null);
        //     if (noPreferredLevel.Count() < attendants.Count())
        //         return;

        //     // still here? great, let's assign some level affinity

        //     // first examine the jobs for each level
        //     Dictionary<string, List<Cleaning>> detected_levels = new Dictionary<string, List<Cleaning>>();
        //     // csharp is a little bit irritating sometimes

        //     int sum_c = 0;
        //     int sum_r = cleanings.Count();
        //     string pattern = "-";

        //     var level_keys = new Dictionary<string, List<string>>();
        //     // the value of the dictionary has 4 entries: building, floor, section, subsection;  if the incoming data is missing, then the value entered is "0"

        //     foreach (var cleaning in cleanings)
        //     {
        //         if (CleaningIntervalConstraints.CleaningIsImpossible(cleaning, 1))
        //             continue;

        //         sum_c += cleaning.Credits;

        //         var level_details = new List<string>();

        //         // building in the first slot
        //         if (cleaning.Room.Building != null)
        //         {
        //             level_details.Add(cleaning.Room.Building.LevelName);
        //         }
        //         else
        //         {
        //             level_details.Add("0");
        //         }
        //         // now floor.  sometimes/often floor also includes building information
        //         if (cleaning.Room.Floor != null)
        //         {
        //             level_details.Add(GetMinimalLevelName(cleaning.Room.Floor.LevelName, level_details[0]));
        //         }
        //         else
        //         {
        //             level_details.Add("0");
        //         }
        //         // section
        //         if (cleaning.Room.Section != null)
        //         {
        //             level_details.Add(GetMinimalLevelName(cleaning.Room.Section.LevelName, level_details[1]));
        //         }
        //         else
        //         {
        //             level_details.Add("0");
        //         }

        //         // subsection
        //         if (cleaning.Room.Subsection != null)
        //         {
        //             level_details.Add(GetMinimalLevelName(cleaning.Room.Subsection.LevelName, level_details[2]));
        //         }
        //         else
        //         {
        //             level_details.Add("0");
        //         }
        //         string levelkey = $"{level_details[0]}-{level_details[1]}-{level_details[2]}-{level_details[3]}";
        //         if (detected_levels.ContainsKey(levelkey))
        //         {
        //             detected_levels[levelkey].Add(cleaning);
        //         }
        //         else
        //         {
        //             level_keys.Add(levelkey, level_details);
        //             detected_levels[levelkey] = new List<Cleaning>() {cleaning};
        //         }
        //     }

        //     // now sort that dict by building, floor, section
        //     if (level_keys.Count() <= 2)
        //     {
        //         // don't bother with setting levels
        //         return;
        //     }
        //     var lks = level_keys.Values.ToList();
        //     lks.Sort(delegate(List<string> x,
        //                       List<string> y)
        //     {
        //         if (Int32.TryParse(x[0], out int x1) &&
        //             Int32.TryParse(y[0], out int y1))
        //         {
        //             if (x1 == y1)
        //             {
        //                 if (Int32.TryParse(x[1], out int x2) &&
        //                     Int32.TryParse(y[1], out int y2))
        //                 {
        //                     if (x2 == y2)
        //                     {
        //                         if (Int32.TryParse(x[2], out int x3) &&
        //                             Int32.TryParse(y[2], out int y3))
        //                         {
        //                             if (x3 == y3)
        //                             {
        //                                 if (Int32.TryParse(x[3], out int x4) &&
        //                                     Int32.TryParse(y[3], out int y4))
        //                                 {
        //                                     return x4.CompareTo(y4);
        //                                 }
        //                                 return  x[3].CompareTo(y[3]);
        //                             }
        //                             return x3.CompareTo(y3);
        //                         }
        //                         return  x[2].CompareTo(y[2]);
        //                     }
        //                     return x2.CompareTo(y2);
        //                 }
        //                 return  x[1].CompareTo(y[1]);
        //             }
        //             return x1.CompareTo(y1);
        //         }
        //         return  x[0].CompareTo(y[0]);
        //     });

        //     // this sort is no good
        //     // var cleanings = _context.Cleanings.OrderBy(x => x.Room.Subsection.LevelName).ThenBy(y => y.Room.Section.LevelName).ThenBy(z => z.Room.Floor.LevelName);

        //     int ave_r = sum_r / attendants.Count();
        //     int ave_c = sum_c / attendants.Count();


        //     //Console.WriteLine($"sorted levels are {level_keys}");


        //     int levelIndex = 0;
        //     int workcredits = 0;
        //     int global_max_credits = strategy.CPSat.maxCredits;
        //     if (ave_c < global_max_credits && strategy.CPSat.epsilonCredits != 0 && ! strategy.CPSat.useTargetMode) // balance credits
        //         global_max_credits = ave_c;

        //     int workrooms = 0;
        //     int global_max_rooms = strategy.CPSat.maxRooms;
        //     if (ave_r < global_max_rooms && strategy.CPSat.epsilonRooms != 0 && ! strategy.CPSat.useTargetMode) // balance rooms
        //         global_max_rooms = ave_r;

        //     foreach (var a in attendants)
        //     {
        //         if (a.CurrentTimeSlot == null)
        //         {
        //             // fix that
        //             a.CurrentTimeSlots = new[] { strategy.GetDefaultTimeSlot() };
        //         }

        //         var maxCredits = global_max_credits;
        //         if (a.CurrentTimeSlot.MaxCredits > 0)
        //             maxCredits = a.CurrentTimeSlot.MaxCredits;

        //         var maxRooms = global_max_rooms;
        //         if (a.CurrentTimeSlot.noOfRooms > 0)
        //             maxRooms = a.CurrentTimeSlot.noOfRooms;

        //         a.CurrentTimeSlot.Affinity = new Affinity_OLD();
        //         var affinityLevels = new List<string>();
        //         string thiskey = null;
        //         while ((workrooms < maxRooms && workcredits < maxCredits) && levelIndex < level_keys.Count())
        //         {
        //             thiskey = string.Join("-", lks[levelIndex]);
        //             workcredits += detected_levels[thiskey].Sum(c=>c.Credits);
        //             workrooms += detected_levels[thiskey].Count();
        //             Cleaning cleaning = detected_levels[thiskey].First();

        //             affinityLevels.Add(cleaning.Room.Floor.LevelName);
        //             // Console.WriteLine(affinityLevels);
        //             // if (cleaning.Room.Building != null)
        //             //     a.CurrentTimeSlot.Affinity.Buildings = cleaning.Room.Building.LevelName;

        //             Console.WriteLine($"assigning preferred floor for {a.Username},  {detected_levels[thiskey].Count()} rooms on floor, cleaning floor {cleaning.Room.Floor.LevelName}, workcredits {workcredits} vs max credits {maxCredits}, workrooms {workrooms} vs max rooms {maxRooms}");
        //             if (workcredits <= maxCredits && workrooms <= maxRooms)
        //                 levelIndex += 1; // Only increment if advancing to next level; repeat levels if have exceeded worker's capacity
        //         }
        //         // merge levels into a comma separated string
        //         a.CurrentTimeSlot.Affinity.Levels = string.Join(",", affinityLevels);
        //         // Console.WriteLine($"{a.Username}: {a.CurrentTimeSlot.Affinity.Levels}");
        //         if (levelIndex >= level_keys.Count())
        //             break;
        //         // reset below zero maybe in order to assign enough levels to next attendant

        //         thiskey = string.Join("-", lks[levelIndex]);
        //         if (workcredits > maxCredits)
        //         {
        //             var this_credits = detected_levels[thiskey].Sum(c=>c.Credits);
        //             var extracredits = workcredits - maxCredits;
        //             workcredits =  -(this_credits - extracredits); // how many credits used by prior worker
        //         }
        //         else
        //         {
        //             workcredits = 0;
        //         }
        //         if (workrooms > maxRooms)
        //         {
        //             var this_rooms = detected_levels[thiskey].Count();
        //             var extrarooms = workrooms - maxRooms;
        //             workrooms =  -(this_rooms - extrarooms); // how many rooms used by prior worker
        //         }
        //         else
        //         {
        //             workrooms = 0;
        //         }
        //         // if (workrooms == 0 || workcredits == 0)
        //         //     levelIndex += 1;
        //     }
        // }


        // MakeLevelIntervals
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
        // a list of LevelCleaningInterval will be filled, one per level, based on the cleanings that are possible for this attendant
        //
        public static Boolean Make_Level_Intervals(
            CpModel model,
            Attendant attendant,
            AttendantCleanings attendantCleanings,
            AttendantInterval attendant_interval,
            List<LevelCleanings> levelsCleanings,
            List<CleaningInterval> attendant_cleaning_intervals,
            List<LevelCleaningInterval> attendant_level_intervals,
            List<IntVar> _total_room_travel,
            List<IntVar> _total_floor_travel,
            IntVar levels_cleaned,
            DateTime _earliest_date,
            int max_travel_time,
            List<Affinity> affinities,
            Dictionary<Cleaning, List<IntVar>> room_literals,
            IntVar rooms_cleaned,
            Distances roomDistances,
            HotelStrategy hotelStrategy,
            string flag = ""
        )
        {
            Boolean firstPass = levelsCleanings.Any(lc => lc.allowAllorNothing());

            // Boolean attendantFullyAssigned = false;
            Console.WriteLine($"make level intervals call, {attendant_cleaning_intervals.Count} attendant cleanings, and {levelsCleanings.Count()} levels with cleanings for {attendant.Username}");
            CPSatSolutionInformation cpsi = hotelStrategy.CPSat.SolutionResult;

            Dictionary<string, List<CleaningInterval>> allLevelCleaningIntervals = new Dictionary<string, List<CleaningInterval>>();
            var buildingName = levelsCleanings.First().BuildingName;

            // set up boundaries for this attendant
            var maxCredits = hotelStrategy.CPSat.maxCredits;
            if (attendant.CurrentTimeSlot?.MaxCredits != null && attendant.CurrentTimeSlot.MaxCredits > 0)
            {
                maxCredits = attendant.CurrentTimeSlot.MaxCredits;
            }
            var maxRooms = hotelStrategy.CPSat.maxRooms;
            if (attendant.CurrentTimeSlot?.noOfRooms != null && attendant.CurrentTimeSlot.noOfRooms > 0)
            {
                maxRooms = attendant.CurrentTimeSlot.noOfRooms;
            }

            var hasPreplan = false;
            if (hotelStrategy.CPSat.UsePrePlan && attendant.Cleanings.Count() > 0)
            {
                maxRooms -= attendant.Cleanings.Count();
                maxCredits -= attendant.Cleanings.Sum(c => c.Credits);
                hasPreplan = true;
            }

            var cleaningPriorities = new List<string>();
            if (hotelStrategy.CPSat.cleaningPriorities != null)
            {
                cleaningPriorities = new List<string>() {hotelStrategy.CPSat.cleaningPriorities};
            }
            if (cleaningPriorities.Contains("Others") &&
                hotelStrategy.CPSat.otherCleaningPriorities!=null &&
                hotelStrategy.CPSat.otherCleaningPriorities != "")
            {
                cleaningPriorities.Remove("Others");
                var otherCleaningPriorities = hotelStrategy.CPSat.otherCleaningPriorities.Split(',').ToList();
                cleaningPriorities.AddRange(otherCleaningPriorities);
            }

            // sort the levels
            levelsCleanings.Sort(delegate (LevelCleanings a,
                                           LevelCleanings b)
            {
                // preplan first
                // then affinity
                // then least already assigned
                // then most priority cleanings
                // then most cleanings

                // does the attendant have a preplan, and if so, which buildings


                if (hasPreplan)
                {
                    var aPreplan = a.MatchesPreplanned(attendant);
                    var bPreplan = b.MatchesPreplanned(attendant);
                    // Console.WriteLine($"preplan for {a.LevelName} is {aPreplan}, preplan for {b.LevelName} is {bPreplan}");
                    if (aPreplan != bPreplan)
                    {
                        if (aPreplan) return -1;
                        else if (bPreplan) return 1;
                    }
                }
                // does the attendant have affinity, and if so, affinity for either level
                var aAffinity = false;
                var bAffinity = false;
                if (affinities.Count() > 0)
                {
                    aAffinity =
                        affinities.Any( fa =>
                        {
                            string[] anameparts = a.LevelName.Split(':');
                            // return fa == anameparts[1];
                            var match = fa.Floor == anameparts[1];
                            if (!match && anameparts.Count()>2)
                            {
                                match = fa.Section == anameparts[2];
                            }
                            if (!match && anameparts.Count()>3)
                            {
                                match = fa.SubSection == anameparts[3];
                            }
                            if (!match && fa.CleaningType != null)
                            {
                                // FIXME needs to be checked versus what is passed in
                                match = a.Cleanings.Any(c =>
                                {
                                    var matched = false;
                                    switch (c.Type)
                                    {
                                        case CleaningType.Departure:
                                            matched = "Departure" == fa.CleaningType;
                                            break;
                                        case CleaningType.Stay:
                                            matched = "Stay" == fa.CleaningType;
                                            break;
                                        case CleaningType.ChangeSheet:
                                            matched = "Change Sheet" == fa.CleaningType;
                                            break;
                                        case CleaningType.Vacant:
                                            matched = "Vacant" == fa.CleaningType;
                                            break;
                                        default:  // case "Custom":
                                            break;
                                    }
                                    return matched;
                                });
                            }
                            return match;
                        });
                    bAffinity =
                        affinities.Any( fa =>
                        {
                            string[] bnameparts = b.LevelName.Split(':');
                            // return fa == anameparts[1];
                            var match = fa.Floor == bnameparts[1];
                            if (!match && bnameparts.Count()>2)
                            {
                                match = fa.Section == bnameparts[2];
                            }
                            if (!match && bnameparts.Count()>3)
                            {
                                match = fa.SubSection == bnameparts[3];
                            }
                            if (!match && fa.CleaningType != null)
                            {
                                match = a.Cleanings.Any(c =>
                                {
                                    var matched = false;
                                    switch (c.Type)
                                    {
                                        case CleaningType.Departure:
                                            matched = "Departure" == fa.CleaningType;
                                            break;
                                        case CleaningType.Stay:
                                            matched = "Stay" == fa.CleaningType;
                                            break;
                                        case CleaningType.ChangeSheet:
                                            matched = "Change Sheet" == fa.CleaningType;
                                            break;
                                        case CleaningType.Vacant:
                                            matched = "Vacant" == fa.CleaningType;
                                            break;
                                        default:  // case "Custom":
                                            break;
                                    }
                                    return matched;
                                });
                            }
                            return match;
                        });
                }
                if (a.allowAllorNothing() != b.allowAllorNothing())
                {
                    if (a.allowAllorNothing()) return -1;
                    else if (b.allowAllorNothing()) return 1;
                }
                if (aAffinity != bAffinity)
                {
                    if (aAffinity) return -1;
                    else if (bAffinity) return 1;
                }
                // still here, so sort on attendants assigned count
                if (a.assignedAttendants() != b.assignedAttendants())
                {
                    // least to greatest
                    return a.assignedAttendants().CompareTo(b.assignedAttendants());
                }
                // still here, so sort on optional cleanings
                if (a.optionalCleanings() != b.optionalCleanings())
                {
                    // least to most
                    return a.optionalCleanings().CompareTo(b.optionalCleanings());
                }
                // still here, so sort on least assigned
                if (a.unassignedCleanings() != b.unassignedCleanings())
                {
                    // most unassigned to least unassigned
                    return b.unassignedCleanings().CompareTo(a.unassignedCleanings());
                }
                // still here, sort on total rooms
                // most to least
                return b.Count.CompareTo(a.Count);

            });

            // debug the sort
            Console.WriteLine("level sort is");
            levelsCleanings.ForEach(lc => Console.WriteLine(lc.LevelName));
            var favorRooms = true;
            if (hotelStrategy.CPSat.epsilonCredits < 0)
            {
                favorRooms = false;
            }
            // preplan pass, only if the attendant has preplanned
            if (hasPreplan)
            {
                foreach (var levelCleanings in levelsCleanings)
                {
                    if (allLevelCleaningIntervals.ContainsKey(levelCleanings.LevelName))
                    {
                        continue;
                    }
                    var lcPreplan = levelCleanings.MatchesPreplanned(attendant);
                    if (!lcPreplan)
                    {
                        continue;
                    }
                    Console.WriteLine($"looking at level {levelCleanings.LevelName}, with cleanings {levelCleanings.Cleanings.Count()} and unassigned cleanings of {levelCleanings.unassignedCleanings()} because it is a preplanned level for {attendant.Username}");
                    List<CleaningInterval> thisLevelCleaningIntervals = new List<CleaningInterval>();
                    CleaningIntervalConstraints.Make_Cleaning_Intervals(
                        model,
                        attendant,
                        levelCleanings.Cleanings,
                        thisLevelCleaningIntervals,
                        attendant_interval,
                        affinities,
                        room_literals,
                        _earliest_date,
                        flag,
                        hotelStrategy);
                    if (thisLevelCleaningIntervals.Count() == 0)
                        continue;
                    attendant_cleaning_intervals.AddRange(thisLevelCleaningIntervals);
                    allLevelCleaningIntervals.Add(levelCleanings.LevelName, thisLevelCleaningIntervals);
                    // this if case first handles maxing out rooms, then maxing out credits
                    if ( ! attendantCleanings.isFullyAssigned() &&
                         (favorRooms &&
                          levelCleanings.unassignedCleanings() >= attendantCleanings.maxCleanings) ||
                         (!favorRooms &&
                          levelCleanings.unassignedCredits() >= attendantCleanings.maxCredits)
                    )
                    {
                        levelCleanings.assignCleanings(attendantCleanings.maxCleanings, maxCredits);
                        attendantCleanings.addAssignedCleanings(attendantCleanings.maxCleanings);
                    }
                    else{
                        levelCleanings.assignAttendant();
                        attendantCleanings.addOptionalCleanings(levelCleanings.unassignedCleanings());
                    }
                }
            }
            // floor affinities pass, only if the attendant has affinities
            if (affinities.Count() > 0)
            {
                foreach (var levelCleanings in levelsCleanings)
                {
                    if (allLevelCleaningIntervals.ContainsKey(levelCleanings.LevelName))
                    {
                        continue;
                    }
                    if (attendantCleanings.isFullyAssigned())
                    {
                        break;
                    }
                    var lcAffinity =
                        affinities.Any( fa =>
                        {
                            string[] nameparts = levelCleanings.LevelName.Split(':');
                            var match = fa.Floor == nameparts[1];
                            if (!match && nameparts.Count()>2)
                            {
                                match = fa.Section == nameparts[2];
                            }
                            if (!match && nameparts.Count()>3)
                            {
                                match = fa.SubSection == nameparts[3];
                            }
                            if (!match && fa.CleaningType != null)
                            {
                                match = levelCleanings.Cleanings.Any(c =>
                                {
                                    var matched = false;
                                    switch (c.Type)
                                    {
                                        case CleaningType.Departure:
                                            matched = "Departure" == fa.CleaningType;
                                            break;
                                        case CleaningType.Stay:
                                            matched = "Stay" == fa.CleaningType;
                                            break;
                                        case CleaningType.ChangeSheet:
                                            matched = "Change Sheet" == fa.CleaningType;
                                            break;
                                        case CleaningType.Vacant:
                                            matched = "Vacant" == fa.CleaningType;
                                            break;
                                        default:  // case "Custom":
                                            break;
                                    }
                                    return matched;
                                });
                            }
                            return match;

                        });
                    if (!lcAffinity)
                    {
                        continue;
                    }
                    Console.WriteLine($"looking at level {levelCleanings.LevelName}, with cleanings {levelCleanings.Cleanings.Count()} and unassigned cleanings of {levelCleanings.unassignedCleanings()} because it is an affinity level for {attendant.Username}");
                    List<CleaningInterval> thisLevelCleaningIntervals = new List<CleaningInterval>();
                    CleaningIntervalConstraints.Make_Cleaning_Intervals(
                        model,
                        attendant,
                        levelCleanings.Cleanings,
                        thisLevelCleaningIntervals,
                        attendant_interval,
                        affinities,
                        room_literals,
                        _earliest_date,
                        flag,
                        hotelStrategy);
                    if (thisLevelCleaningIntervals.Count() == 0)
                        continue;
                    attendant_cleaning_intervals.AddRange(thisLevelCleaningIntervals);
                    allLevelCleaningIntervals.Add(levelCleanings.LevelName, thisLevelCleaningIntervals);
                    if (levelCleanings.unassignedCleanings() >= attendantCleanings.maxCleanings
                        && ! attendantCleanings.isFullyAssigned())
                    {
                        levelCleanings.assignCleanings(attendantCleanings.maxCleanings, maxCredits);
                        attendantCleanings.addAssignedCleanings(attendantCleanings.maxCleanings);
                    }
                    else{
                        levelCleanings.assignAttendant();
                        attendantCleanings.addOptionalCleanings(levelCleanings.unassignedCleanings());
                    }
                }
            }

            Console.WriteLine($"entering all or nothing loop with maxRooms {maxRooms} and maxCredits {maxCredits}");
            foreach (var levelCleanings in levelsCleanings)
            {
                if (flag != "" &&
                    attendantCleanings.getAssignedCleanings() >= attendantCleanings.maxCleanings)
                {
                    break;
                }
                if (flag == "" &&
                    attendantCleanings.isFullyAssigned())
                {
                    break;
                }
                if (allLevelCleaningIntervals.ContainsKey(levelCleanings.LevelName))
                {
                    continue;
                }
                if (! levelCleanings.allowAllorNothing())
                {
                    continue;
                }
                if (levelCleanings.fullyAssigned())
                {
                    continue;
                }

                if (levelCleanings.unassignedCleanings() < attendantCleanings.maxCleanings)
                {
                    continue;
                }
                // handle credits later, in the balance credits case (epsilonCredits != 0)
                // if (levelCleanings.unassignedCredits() < maxCredits)
                // {
                //     Console.WriteLine($"level {levelCleanings.LevelName} has fewer credits than required for all or nothing ({levelCleanings.unassignedCredits()} remaining versus {maxCredits} needed)");
                //     continue;
                // }

                // if still here, then it is possible for the
                // attendant to be fully assigned with just this floor

                Console.WriteLine($"looking at level {levelCleanings.LevelName} in all or nothing loop, with cleanings {levelCleanings.Cleanings.Count()} and unassigned cleanings of {levelCleanings.unassignedCleanings()}");
                List<CleaningInterval> thisLevelCleaningIntervals = new List<CleaningInterval>();
                CleaningIntervalConstraints.Make_Cleaning_Intervals(
                    model,
                    attendant,
                    levelCleanings.Cleanings, //,_context.Cleanings.ToList(),
                    thisLevelCleaningIntervals,
                    attendant_interval,
                    affinities,
                    room_literals,
                    _earliest_date,
                    flag,
                    hotelStrategy);
                // Console.WriteLine($"done making cleaning intervals, got back {thisLevelCleaningIntervals.Count()}");
                if (thisLevelCleaningIntervals.Count() == 0)
                    continue;
                attendant_cleaning_intervals.AddRange(thisLevelCleaningIntervals);
                allLevelCleaningIntervals.Add(levelCleanings.LevelName, thisLevelCleaningIntervals);

                // track the tentative assignment for other attendants
                levelCleanings.assignCleanings(maxRooms, maxCredits);
                attendantCleanings.addAssignedCleanings(attendantCleanings.maxCleanings);
                break;
            }

            // third pass, for the priority cleanings, if any
            if (cleaningPriorities.Count() > 0)
            {
                foreach (var levelCleanings in levelsCleanings)
                {
                    if (attendantCleanings.isOptionalPriorityFullyAssigned())
                    {
                        break;
                    }
                    var lcPriority = levelCleanings.levelHasPriorityCleaning(cleaningPriorities);
                    if (!lcPriority)
                    {
                        continue;
                    }
                    if (levelCleanings.fullyAssigned())
                    {
                        // other attendants already fully clean this floor
                        continue;
                    }
                    if (allLevelCleaningIntervals.ContainsKey(levelCleanings.LevelName))
                    {
                        continue;
                    }

                    Console.WriteLine($"looking at level {levelCleanings.LevelName}, with cleanings {levelCleanings.Cleanings.Count()} and unassigned cleanings of {levelCleanings.unassignedCleanings()} because there is a cleaning on this level with a priority");
                    List<CleaningInterval> thisLevelCleaningIntervals = new List<CleaningInterval>();
                    CleaningIntervalConstraints.Make_Cleaning_Intervals(
                        model,
                        attendant,
                        levelCleanings.Cleanings, //,_context.Cleanings.ToList(),
                        thisLevelCleaningIntervals,
                        attendant_interval,
                        affinities,
                        room_literals,
                        _earliest_date,
                        flag,
                        hotelStrategy);
                    if (thisLevelCleaningIntervals.Count() == 0)
                        continue;
                    attendant_cleaning_intervals.AddRange(thisLevelCleaningIntervals);
                    allLevelCleaningIntervals.Add(levelCleanings.LevelName, thisLevelCleaningIntervals);
                    levelCleanings.assignAttendant();
                    attendantCleanings.addOptionalPriorityCleanings(levelCleanings.unassignedCleanings());
                }
            }

            // if the attendant was not fully assigned above, then do another pass, allow any level to be assigned
            Console.WriteLine("entering final, optional cleanings loop");
            foreach (var levelCleanings in levelsCleanings)
            {
                if (flag == "" &&
                    attendantCleanings.isFullyAssigned(firstPass))
                {
                    Console.WriteLine($"attendant  {attendant.Username} is fully assigned, so break out of loop");
                    break;
                }
                if (levelCleanings.fullyAssigned() && levelCleanings.allowAllorNothing())
                {
                    // other attendants already fully clean this floor
                    Console.WriteLine($"level {levelCleanings.LevelName} is fully assigned and allows all or nothing, so skip");
                    continue;
                }
                if (allLevelCleaningIntervals.ContainsKey(levelCleanings.LevelName))
                {
                    Console.WriteLine($"level {levelCleanings.LevelName} is already assigned to this attendant");
                    continue;
                }

                Console.WriteLine($"looking at level {levelCleanings.LevelName}, with cleanings {levelCleanings.Cleanings.Count()} and unassigned cleanings {levelCleanings.unassignedCleanings()}");
                List<CleaningInterval> thisLevelCleaningIntervals = new List<CleaningInterval>();
                CleaningIntervalConstraints.Make_Cleaning_Intervals(
                    model,
                    attendant,
                    levelCleanings.Cleanings, //,_context.Cleanings.ToList(),
                    thisLevelCleaningIntervals,
                    attendant_interval,
                    affinities,
                    room_literals,
                    _earliest_date,
                    flag,
                    hotelStrategy);
                // Console.WriteLine($"done making cleaning intervals, got back {thisLevelCleaningIntervals.Count()}");
                if (thisLevelCleaningIntervals.Count() == 0)
                    continue;
                attendant_cleaning_intervals.AddRange(thisLevelCleaningIntervals);
                allLevelCleaningIntervals.Add(levelCleanings.LevelName, thisLevelCleaningIntervals);

                // possibly revisit and track the tentative assignment for other attendants
                levelCleanings.assignAttendant();
                attendantCleanings.addOptionalCleanings(levelCleanings.unassignedCleanings());

            }

            // now loop over each of the detected levels to create
            // the IntervalVars, and to apply the various
            // constraints to the model
            var thisLevelIntervals = new List<LevelCleaningInterval>();
            foreach (KeyValuePair<string, List<CleaningInterval>> kvp in allLevelCleaningIntervals)
            {
                var level_decision_list = kvp.Value;
                string level_key_name = kvp.Key;
                if (hotelStrategy.CPSat.preferredLevelsAreExclusive &&
                    affinities.Count() > 0)
                {
                    var overlap = affinities.Any( a =>
                    {
                        string[] nameparts = level_key_name.Split(':');
                        var match = nameparts[0] == a.Building;
                        if (!match && nameparts.Count() > 1)
                        {
                            match = a.Floor == nameparts[1];
                        }
                        if (!match && nameparts.Count()>2)
                        {
                            match = a.Section == nameparts[2];
                        }
                        if (!match && nameparts.Count()>3)
                        {
                            match = nameparts[3] == a.SubSection;
                        }
                        return match;
                    });
                    if (!overlap)
                    {
                        // in this case, do nothing---the attendant has preferred levels, but those levels do not contain this level
                        // force all possible cleanings for this attendant on this level to be false
                        // Console.WriteLine($"Attendant {attendant.Username} cannot process this level due to exclusivity constraints");
                        level_decision_list.ForEach(ci => model.Add(ci.Literal == 0));
                        continue;
                    }
                }
                // Console.WriteLine($"Process the level {level_key_name}");

                // first, create an optional interval in the usual way
                // minimum start via the cleanings

                // use this intvar to link the room to room circuit with the level total time
                var roomsTravelTime = model.NewIntVar(0, max_travel_time, $"atndnt {attendant.Username}, level {level_key_name}, level travel time");

                var level_interval = LevelCleaningInterval.CreateIntervalVar(model, attendant, _earliest_date, level_key_name, level_decision_list, roomsTravelTime, hotelStrategy, flag);
                // Console.WriteLine($"{level_interval.Literal} Start-End : {level_interval.Start} - {level_interval.End} :{level_interval.Duration} min");

                // add to the list
                attendant_level_intervals.Add(level_interval);
                thisLevelIntervals.Add(level_interval);

                // link in attendant interval to make times, literals consistent

                // as there is only one, set a hard implication
                model.AddImplication(level_interval.Literal, attendant_interval.Literal);


                // need to try to speed this loop up, it is slow.  Perhaps parallelize it, or perhaps parallelize this outer loop
                // level travel time is forced to be equal to the
                // sum of all (possible) room to room travels
                CleaningIntervalConstraints.Cleaning_To_Cleaning_Ordering(model,
                                              attendant,
                                              level_decision_list,
                                              roomsTravelTime,
                                              level_interval,
                                              roomDistances,
                                              cpsi);
                // collect for objective
                _total_room_travel.Add(roomsTravelTime);
            }
            var betweenLevelsTravelTime = model.NewIntVar(0, max_travel_time, $"atndnt {attendant.Username}, level to level travel time for building {buildingName}");
            Level_To_Level_Ordering(model, attendant, thisLevelIntervals,
                                    betweenLevelsTravelTime, attendant_interval.Literal,
                                    hotelStrategy.CPSat.maxShiftFloorAllowed,
                                    cpsi);
            _total_floor_travel.Add(betweenLevelsTravelTime);

            return attendantCleanings.isFullyAssigned();

        }

        public static void ConstrainAttendantLevelIntervals(
            CpModel model,
            Attendant attendant,
            AttendantInterval attendant_interval,
            List<LevelCleaningInterval> attendant_level_intervals,
            List<IntVar> _total_floor_travel,
            int max_travel_time,
            IntVar floorsCleaned,
            HotelStrategy hotelStrategy
        )
        {
            CPSatSolutionInformation cpsi = hotelStrategy.CPSat.SolutionResult;

            // The intervals cannot overlap, meaning the attendant
            // must do one level at a time and also that the
            // attendant cannot come back to this level later (If
            // you want that, you have to make multiple level
            // intervals, but that seems like a slippery slope)
            var levels_intervals = attendant_level_intervals.Select(ali => ali.Interval);
            model.AddNoOverlap(levels_intervals);

            // no levels anymore, but floors, so need to create a new variable to sum floors
            model.Add(floorsCleaned == 0).OnlyEnforceIf(attendant_interval.Literal.Not());
            model.Add(floorsCleaned > 0).OnlyEnforceIf(attendant_interval.Literal);
            var floorLiterals = new Dictionary<Level, IntVar>();
            foreach (var level_decision in attendant_level_intervals)
            {
                // if literal is true, that means this attendant
                // does this cleaning.  , must happen inside of
                // the attendant's work period
                //
                // but to be parsimonious, you only have to worry
                // about it for the first and last intervals.  Really
                // can just do the building interval, but maybe there
                // is no building

                model.Add(attendant_interval.Start <= level_decision.Start).OnlyEnforceIf(level_decision.Literal);
                model.Add(attendant_interval.End >= level_decision.End).OnlyEnforceIf(level_decision.Literal);
                var floor = level_decision.Floor;

                // if not in floor literals yet, create the literal
                if (!floorLiterals.ContainsKey(floor))
                {
                    floorLiterals.Add(floor, model.NewBoolVar($"atndnt {attendant.Username} cleans floor {floor.LevelName}"));
                }
                // channeling constraints.  If this level is true, then floor is true
                model.Add(floorLiterals[floor] == 1).OnlyEnforceIf(level_decision.Literal);
                model.Add(level_decision.Literal == 0).OnlyEnforceIf(floorLiterals[floor].Not());

            }
            // count up floor changes by creating an equality
            var totalFloorsCleaned = LinearExpr.Sum(floorLiterals.Values);

            model.Add(totalFloorsCleaned == floorsCleaned);


        }


        // Level to level ordering, via circuit constraint
        // arguments are
        // the model,
        // the attendant,
        // the list of cleaning decision variable for the attendant
        // the IntVar representing the bounds for travel time.
        // the IntVar representing if the attendant is being used
        private static void Level_To_Level_Ordering(
            CpModel model,
            Attendant a,
            List<LevelCleaningInterval> attendant_level_intervals,
            IntVar betweenLevelsTravelTime,
            IntVar attendantLiteral,
            int max_level_shift_allowed,
            CPSatSolutionInformation cpsi
        )
        {
            // work out the ordering and travel time between the levels the attendant might clean
            var maxgap = 10;
            var arcs = new List<Tuple<int, int, ILiteral>>();
            // a place to stash cleaning to cleaning transition literals
            // for travel count, times
            var transition_literals = new List<IntVar>();
            var transition_times = new List<int>();

            var skip_attendant_lit = model.NewBoolVar($"atndnt {a.Username} is not used");
            var whole_arc_skip = new Tuple<int, int, ILiteral>(0, 0, skip_attendant_lit); //a loop to itself =  skip the origin node, and skip the whole arc constraint
            arcs.Add(whole_arc_skip);

            // link the skip literal to the incoming attendant does clean literal
            // if attendant is not cleaning, then skip considering any of the levels
            model.AddImplication(attendantLiteral.Not(), skip_attendant_lit);

            for (int j1 = 0; j1 < attendant_level_intervals.Count; ++j1) //j1 is the first job she's doing, j2 is the 2nd job
            {
                var this_level_decision = attendant_level_intervals[j1];
                var levelname = this_level_decision.LevelName;

                int zero_to_level_distance = LevelsMath.LevelToLevelDistance(
                    this_level_decision.Floor
                );

                // every level can be potentially first or last, this the loop
                // We're listing all the first

                var start_lit = model.NewBoolVar($"atndnt {a.Username}: {levelname} is first level");
                this_level_decision.IsFirstLiteral = start_lit;

                var arc0 = new Tuple<int, int, ILiteral>(0, j1 + 1, start_lit);
                arcs.Add(arc0);

                // collect the arc0 literal, and the 0 to j distance for the objective fn
                transition_literals.Add(start_lit);
                transition_times.Add(zero_to_level_distance);

                //We're listing all the finals
                // Final arc from an arc to the dummy node.
                var end_lit = model.NewBoolVar($"atndnt {a.Username}: {levelname} is last level");
                var arcLast = new Tuple<int, int, ILiteral>(j1 + 1, 0, end_lit);
                arcs.Add(arcLast);
                this_level_decision.IsLastLiteral = end_lit;

                // collect the arclast literal, and the j to 0 distance for the objective fn
                transition_literals.Add(end_lit);
                transition_times.Add(zero_to_level_distance);

                //We're listing all the Skipped
                // if this level is not visited by this attendant, need a self-terminating arc
                var skip_lit = model.NewBoolVar($"atndnt {a.Username}: {levelname} is skipped");
                var arc_skip = new Tuple<int, int, ILiteral>(j1 + 1, j1 + 1, skip_lit); //a loop to itself =  skip that node in the circuit
                arcs.Add(arc_skip);

                // link the skip literal to the clean room literal
                // if not cleaning the room, then skip self-loop is true
                model.Add(skip_lit != this_level_decision.Literal);
                model.AddImplication(attendantLiteral.Not(), skip_lit);

                // Console.WriteLine($"skip lit is {skip_lit}");
                // Console.WriteLine($"attendant literal is {attendantLiteral}");

                // now consider the level that comes after j1 IN THE LOOP
                for (int j2 = 0; j2 < attendant_level_intervals.Count; ++j2)
                {
                    if (j1 == j2)
                    {
                        // cannot transition from a room to itself, so skip this case
                        continue;
                    }
                    //If you don't want to allow going from room j1 to room j2, just skip then "continue"
                    LevelCleaningInterval next_level_decision = attendant_level_intervals[j2];
                    // note, this is to prevent building to building
                    // shifts Most likely we are only considering
                    // same-building levels here, but better safe than
                    // sorry
                    var next_levelname = next_level_decision.LevelName;
                    // Console.WriteLine($"this decision is {this_level_decision.Literal},building: {this_level_decision.Building.LevelName}, floor: {this_level_decision.Floor.LevelName}");
                    // Console.WriteLine($"next potential decision is {next_level_decision.Literal}, building: {next_level_decision.Building.LevelName}, floor: {next_level_decision.Floor.LevelName}");
                    if (this_level_decision.Building.LevelName != next_level_decision.Building.LevelName)
                    {
                        cpsi.RoomcheckingInformation.Add($"Forbid level shift from {levelname} to {next_levelname} because they are in different buildings");
                        continue;
                    }
                    int level_distance = LevelsMath.LevelToLevelDistance(
                        this_level_decision.Floor,
                        next_level_decision.Floor);
                    if (level_distance > max_level_shift_allowed)
                    {
                        cpsi.RoomcheckingInformation.Add($"Forbid level shift from {levelname} to {next_levelname} because floor shift of {level_distance} exceeds {max_level_shift_allowed}");
                        continue;
                    }
                    //cpsi.RoomcheckingInformation.Add($"Considering level shift from {levelname} to {next_levelname} (change {level_distance} floors) does not exceed {max_level_shift_allowed}");

                    var lit = model.NewBoolVar($"atndnt {a.Username}: {next_levelname} follows {levelname}");
                    // Console.WriteLine($"considering level to level transition literal: {lit}");
                    var arc_j1_j2 = new Tuple<int, int, ILiteral>(j1 + 1, j2 + 1, lit);
                    arcs.Add(arc_j1_j2);

                    // We add the reified precedence to link the literal with the
                    // times of the two tasks.:if theliteral is true, then it happens

                    // FIXME: In order for this constraint to work
                    // properly, it MUST be the case that
                    // LevelToLevel returns an accurate
                    // estimate of the time it takes to travel
                    // from "this cleaning level" to "next cleaning level"
                    model.Add(next_level_decision.Start >= this_level_decision.End + level_distance).OnlyEnforceIf(lit);
                    if (!(this_level_decision.IsPrePlanned && next_level_decision.IsPrePlanned))
                    {
                        model.Add(next_level_decision.Start <= this_level_decision.End + level_distance + maxgap).OnlyEnforceIf(lit);
                    }
                    // new ILiteral[]{lit,
                    //     this_level_decision.Literal,
                    //     next_level_decision.Literal
                    // });

                    // collect the j_k literal, and the j_k distance for the objective fn
                    transition_literals.Add(lit);
                    if (level_distance == 0)
                    {
                        // Console.WriteLine($"for attendant {a.Username}: {next_levelname} follows {levelname}, cost is zero");
                        transition_times.Add(level_distance);  // add 1 to penalize shifting floors!
                    }
                    else
                    {
                        // Console.WriteLine($"for attendant {a.Username}: {next_levelname} follows {levelname}, cost is {level_distance}");
                        transition_times.Add(level_distance);  // add 1 to penalize shifting floors!
                    }
                    // have to link the literal back with assigned room literals
                    model.AddImplication(lit, this_level_decision.Literal);
                    model.AddImplication(lit, next_level_decision.Literal);
                }
            }

            model.AddCircuit(arcs);

            // // diagnostic
            // Console.WriteLine(string.Join(",\n", arcs));
            // for (var i=0; i<transition_literals.Count(); i++)
            // {
            //     Console.WriteLine($" literal {transition_literals[i]} has time {transition_times[i]}");
            // }
            // add constraint on travel time
            var totalTravelTime = LinearExpr.ScalProd(transition_literals, transition_times);
            // set the constraint by forcing this equality.  It
            // also links the passed in IntVar travelTime to be
            // equal to the sum of all of the travel times for
            // this attendant.
            model.Add(totalTravelTime == betweenLevelsTravelTime);

            return;

        }
        public static string GetMinimalLevelName(string input, string mightmatch)
        {
            // sometimes/often a floor, section, subsection also includes the prior level information and a dash
            var pattern = "-";
            var levelmatches = Regex.Split(input, pattern);
            if (levelmatches.Count() > 1)
            {
                if (levelmatches[0] == mightmatch)
                    return levelmatches[1];
                else
                    return input;
            }
            else
            {
                return input;
            }

        }
        public static List<string> GetLevelAndSectionNames(List<CleaningInterval> attendant_cleanings)
        {
            var levelnames = attendant_cleanings.Select(ldl => ldl.Cleaning.Room.Floor?.LevelName).Distinct().ToList();
            var sectionnames = attendant_cleanings.Select(ldl => ldl.Cleaning.Room.Section?.LevelName).Distinct().ToList();
            var subsectionnames = attendant_cleanings.Select(ldl => ldl.Cleaning.Room.Subsection?.LevelName).Distinct().ToList();
            var names = new List<string>();
            if (levelnames.Count() > 0)
                names.AddRange(levelnames);
            if (sectionnames.Count() > 0)
                names.AddRange(sectionnames);
            if (subsectionnames.Count() > 0)
                names.AddRange(subsectionnames);
            return names;
        }
    }
}
