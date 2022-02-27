using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Google.OrTools.Sat;

namespace Planner.CpsatCleaningCalculator
{
    public class LevelCleanings
    {

        public static List<LevelCleanings> makeLevelsCleanings(string buildingname, IEnumerable<Cleaning> cleanings, DateTime earliest_noon)
        {
            var levelsCleanings = cleanings.GroupBy(
                c => LevelIntervalConstraints.MakeLevelName(c), // group by level name
                c => c, // send the cleaning value to the aggregation function
                (levelName, cleanings) => new LevelCleanings(buildingname, levelName, cleanings, earliest_noon)
            ).ToList();
            return levelsCleanings;
        }

        public LevelCleanings(string buildingname, string levelname, IEnumerable<Cleaning> cleanings, DateTime earliest_noon)
        {
            BuildingName = buildingname;
            LevelName = levelname;
            Cleanings = new List<Cleaning>(cleanings);
            var sortCleanings = new List<Cleaning>(cleanings);
            sortCleanings.Sort(delegate(Cleaning a, Cleaning b)
                               {
                                   return a.Credits.CompareTo(b.Credits);
                               });
            SortedCleanings_ = sortCleanings;
            Count = cleanings.Count();
            EarliestFrom = cleanings.Min(c => c.From);
            if (EarliestFrom < earliest_noon)
            {
                AfternoonCleanings =  cleanings.Where(c => c.From >= earliest_noon);
                AfternoonCount = cleanings.Where(c => c.From >= earliest_noon).Count();
                if(AfternoonCount>0)
                    AfternoonEarliestFrom = cleanings.Where(c => c.From >= earliest_noon).Min(c => c.From);
            }
            else
            {
                AfternoonCleanings =  null;
                AfternoonCount = 0;
                AfternoonEarliestFrom = EarliestFrom;
            }
        }

        public Boolean allowAllorNothing(){
            return allOrNothing_;
        }

        public Boolean MatchesPreplanned(Attendant attendant)
        {
            var matchedPreplannedHashSet = CleaningInterval.FindPrePlannedCleanings(attendant, Cleanings);
            return matchedPreplannedHashSet.Count() > 0;
        }

        public Boolean fullyAssigned()
        {
            var totalCredits = Cleanings.Sum( c => c.Credits );
            if (AttendantCount_ > maxAttendants_) // arbitrary choice, but this level has few rooms
            {
                return true;
            }
            return (Count <= AssignedRooms_ || totalCredits <= AssignedCredits_);
        }

        public int unassignedCleanings()
        {
            return Count - AssignedRooms_;
        }

        public int unassignedCredits()
        {
            var totalCredits = Cleanings.Sum( c => c.Credits );
            return totalCredits - AssignedCredits_;
        }

        public void assignCleanings(int roomCount, int maxCredits)
        {
            var creditsPossible = SortedCleanings_.Skip(AssignedRooms_).Take(roomCount).Sum(c => c.Credits);
            AssignedCredits_ = AssignedCredits_ +  creditsPossible;
            AssignedRooms_ = AssignedRooms_ + roomCount;
            AttendantCount_++;
        }

        public int optionalCleanings()
        {
            return OptionalCleanings_;
        }

        public void assignAttendant()
        {
            // this is called when the attendant is not fully loaded
            // when assigned this floor, and so the solver will need
            // to do that.  It is a way to keep track of how many
            // attendants are on a floor
            AttendantCount_++;
            OptionalCleanings_ += Count;

        }
        public int assignedAttendants()
        {
            return AttendantCount_;
        }

        public Boolean levelHasPriorityCleaning(List<string> cleaningPriorities)
        {
            Boolean someRoomHasPriority = false;
            if (this.fullyAssigned()) return false;
            // check each cleaning on this level for any of the cleaningPriorities
            foreach (var priority in cleaningPriorities)
            {
                someRoomHasPriority = Cleanings.Any( c =>
                {
                    var thisRoomPriority = false;
                    switch (priority)
                    {
                        case "Departure":
                            if (c.Type == CleaningType.Departure)
                            {
                                thisRoomPriority = true;
                            }
                            break;
                        case "Stay":
                            if (c.Type == CleaningType.Stay)
                            {
                                thisRoomPriority = true;
                            }
                            break;
                        case "Arrival":
                            if (c.ArrivalExpected) // changed from Type == Vacant Dirty
                            {
                                thisRoomPriority = true;
                            }
                            break;
                        case "DEP/ARR":
                            if (c.Type == CleaningType.Departure && c.ArrivalExpected)
                            {
                                thisRoomPriority = true;
                            }
                            break;
                        case "ARR/DEP":
                            if (c.Type == CleaningType.Departure && c.ArrivalExpected)
                            {
                                thisRoomPriority = true;
                            }
                            break;
                        case "ChangeSheet":
                            if (c.Type == CleaningType.ChangeSheet)
                            {
                                thisRoomPriority = true;
                            }
                            break;

                        default:  // case "Others":
                            if (c.Label != null && c.Label.Contains(priority))
                            {
                                thisRoomPriority = true;
                            }
                            break;
                    }
                    return thisRoomPriority;
                    });
                if (someRoomHasPriority) break;
            }
            return someRoomHasPriority;
        }

        public void resetCleaningCounters()
        {
            // AssignedRooms_ = 0;
            // AssignedCredits_ = 0;
            maxAttendants_ *= 2;
            allOrNothing_ = false;
        }

        public string BuildingName { get; }
        public string LevelName { get; }
        public IEnumerable<Cleaning> Cleanings { get; }
        public int Count { get; }
        public DateTime EarliestFrom { get; }
        public IEnumerable<Cleaning> AfternoonCleanings { get; }
        public int AfternoonCount { get; }
        public DateTime AfternoonEarliestFrom { get; }
        private IEnumerable<Cleaning> SortedCleanings_ { get; }
        private int AssignedRooms_;
        private int OptionalCleanings_;
        private int AssignedCredits_;
        private int AttendantCount_;
        private int maxAttendants_ = 3;
        private Boolean allOrNothing_ = true;

    }
}
