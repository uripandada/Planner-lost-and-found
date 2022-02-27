using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Google.OrTools.Sat;

namespace Planner.CpsatCleaningCalculator
{

    public class BuildingCleanings
    {

        public static string getBuildingName(Cleaning c)
        {
            if (c.Room.Building != null)
            {
                // Console.WriteLine("building name is not null");
                return c.Room.Building.LevelName;
            }
            else
            {
                Console.WriteLine("building name is null, use levelname");
                // try to use the floor/level information
                var levelName = c.Room.Floor.LevelName;
                string pattern = "-|/";

                string[] substrings = Regex.Split(levelName, pattern);    // Split on hyphens
                Console.WriteLine(substrings);
                return substrings[0];
            }
        }


        public static List<BuildingCleanings> makeBuildingsCleanings(IEnumerable<Cleaning> cleanings, DateTime earliest_date, DateTime earliest_noon, HashSet<Cleaning> preplannedCleanings=null)
        {
            if (preplannedCleanings == null)
                preplannedCleanings = new HashSet<Cleaning>();
            var buildingsCleanings = cleanings.GroupBy(
                getBuildingName, // group by building name
                c => c, // send the cleaning value to the aggregation function
                (buildingName, cleanings) => new BuildingCleanings(buildingName, cleanings, earliest_date, earliest_noon, preplannedCleanings)
            ).ToList();
            return buildingsCleanings;
        }


        public BuildingCleanings(string buildingname, IEnumerable<Cleaning> cleanings, DateTime earliest_date, DateTime earliest_noon, HashSet<Cleaning> preplannedCleanings)
        {
            BuildingName = buildingname;
            Cleanings = cleanings.ToList();
            var relevantPreplannedCleanings = new HashSet<Cleaning>(preplannedCleanings);
            relevantPreplannedCleanings.IntersectWith(Cleanings);
            Count = Cleanings.Count();
            EarliestFrom = Cleanings.Min(c => c.From);
            EarliestDate = earliest_date;
            LevelsCleanings = LevelCleanings.makeLevelsCleanings(BuildingName, Cleanings, earliest_noon);
            if (EarliestFrom < earliest_noon)
            {
                MorningCleanings =  Cleanings.Where(c => c.From < earliest_noon).ToList();
                // make sure morning cleanings contains all of the preplanned cleanings, so that preplan can all happen in same building interval
                if (relevantPreplannedCleanings.Count() > 0)
                {
                    // Console.WriteLine($"have {relevantPreplannedCleanings.Count()} preplanned cleanings for this building {BuildingName}");
                    relevantPreplannedCleanings.ExceptWith(MorningCleanings);
                }
                if (relevantPreplannedCleanings.Count() > 0)
                {
                    // Console.WriteLine($"have {relevantPreplannedCleanings.Count()} preplanned cleanings that need to be added to the morning cleanings set for this building {BuildingName}, and removed from afternoon cleanings set");
                    MorningCleanings.AddRange(relevantPreplannedCleanings);

                }
                MorningCount = MorningCleanings.Count();
                if(MorningCount>0)
                    MorningEarliestFrom = MorningCleanings.Min(c => c.From);
                AfternoonCleanings =  Cleanings.Where(c => (c.From >= earliest_noon &&
                                                            !relevantPreplannedCleanings.Contains(c))).ToList();

                AfternoonCount = AfternoonCleanings.Count();
                if(AfternoonCount>0)
                    AfternoonEarliestFrom = AfternoonCleanings.Min(c => c.From);
                MorningLevelsCleanings = LevelCleanings.makeLevelsCleanings(BuildingName, MorningCleanings, earliest_noon);
                AfternoonLevelsCleanings = LevelCleanings.makeLevelsCleanings(BuildingName, AfternoonCleanings, earliest_noon);
            }
            else
            {
                MorningCleanings =  Cleanings;
                MorningCount = Count;
                MorningEarliestFrom = EarliestFrom;
                MorningLevelsCleanings = LevelsCleanings;
            }

        }

        public string BuildingName { get; }
        public List<Cleaning> Cleanings { get; }
        public int Count { get; }
        public DateTime EarliestFrom { get; }
        public DateTime EarliestDate { get; }

        public List<Cleaning> MorningCleanings { get; }
        public int MorningCount { get; }
        public DateTime MorningEarliestFrom { get; }

        public List<Cleaning> AfternoonCleanings { get; }
        public int AfternoonCount { get; }
        public DateTime AfternoonEarliestFrom { get; }

        public List<LevelCleanings> LevelsCleanings { get; set; }
        public List<LevelCleanings> MorningLevelsCleanings { get; set; }
        public List<LevelCleanings> AfternoonLevelsCleanings { get; set; }

        public Boolean MatchesPreplanned(Attendant attendant)
        {
            var matchedPreplannedHashSet = CleaningInterval.FindPrePlannedCleanings(attendant, Cleanings);
            return matchedPreplannedHashSet.Count() > 0;
        }

        public long GetPMStart()
        {
            return (long)(AfternoonEarliestFrom - EarliestDate).TotalMinutes;
        }

        public Boolean fullyAssigned()
        {
            int unassignedLevels = countUnassignedMorning() + countUnassignedAfternoon();
            return unassignedLevels == 0;
        }
        public int countUnassignedCleanings()
        {
            int unassignedCleanings = MorningLevelsCleanings.Where(c => !c.fullyAssigned()).Sum(c => c.unassignedCleanings());
            unassignedCleanings += AfternoonLevelsCleanings.Where(c => !c.fullyAssigned()).Sum(c => c.unassignedCleanings());
            return unassignedCleanings;
        }

        public Boolean fullyAssignedMorning()
        {
            int unassignedLevels = MorningLevelsCleanings.Where(c => ! c.fullyAssigned()).Count();
            return unassignedLevels == 0;
        }
        public int countUnassignedMorning()
        {
            int unassignedLevels = MorningLevelsCleanings.Where(c => ! c.fullyAssigned()).Count();
            return unassignedLevels;
        }

        public Boolean fullyAssignedAfternoon()
        {
            int unassignedLevels = AfternoonLevelsCleanings.Where(c => ! c.fullyAssigned()).Count();
            return unassignedLevels == 0;
        }
        public int countUnassignedAfternoon()
        {
            int unassignedLevels = AfternoonLevelsCleanings.Where(c => ! c.fullyAssigned()).Count();
            return unassignedLevels;
        }

        public void resetCleaningCounters()
        {
            foreach (var lc in MorningLevelsCleanings)
            {
                lc.resetCleaningCounters();
            }
            foreach (var lc in AfternoonLevelsCleanings)
            {
                lc.resetCleaningCounters();
            }
        }
    }
}
