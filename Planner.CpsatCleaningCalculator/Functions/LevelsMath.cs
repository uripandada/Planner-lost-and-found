using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Planner.CpsatCleaningCalculator
{
    public class LevelsMath
    {

        ///<summary>
        /// Populate a List<Level> for all of the actual "Floors" in the list of cleanings, for use in computing floor to floor movement, etc
        ///</summary>
        static public void GetFloorsFromCleanings(List<Cleaning> cleanings, Dictionary<string, Level> floors)
        {
            // iterate over cleanings, look at room, save floor to set
            foreach (var c in cleanings)
            {
                var floor_name= LevelIntervalConstraints.MakeLevelName(c);

                floors.TryAdd(floor_name, c.Room.Floor); ;
                //if (c.Room.Section != null)
                //    floors.TryAdd(c.Room.Section.LevelName, c.Room.Floor);
                //if (c.Room.Subsection != null)
                //    floors.TryAdd(c.Room.Subsection.LevelName, c.Room.Floor);
            }
            //cleanings.ForEach(c => floors.TryAdd(c.Room.Floor.LevelName, c.Room.Floor));

            return;
        }


        ///<summary>
        /// Check if Attendant can clean Cleaning.
        ///
        /// This function checks the cleaning in both time and space to see if it is possible for the Attendant to perform the Cleaning.
        ///
        /// All cleanings have From and To datetime values.  The Attendant's currently have TimeSpan From and To values, representing hours in the day that they can work.
        ///
        /// To make these comparable, a reference date is required to be supplied.
        ///
        /// <param name="reference_date">The earliest DateTime in the
        /// simulation, which provides context for the Attendant's
        /// TimeSpan values for From and To, compared to the absolute
        /// DateTime From and To values of the Cleaning.</param>
        ///
        /// If the cleaning falls outside of the Attendant's working
        /// period, the result is false.
        ///
        /// If the times match up, then the next decision is whether
        /// or not the Attendant is allowed to travel to this
        /// Cleaning.  For this decision, this function operates on
        /// the assumption that the Attendant has one or more
        /// preferred levels.
        ///
        /// If the Attendant does not have a preferred level, then the
        /// result will be true...the assumption is that without a
        /// preference, any room is possible.
        ///
        /// If the Attendant does have one or more preferred Levels,
        /// then the function works as follows.
        ///
        /// If the cleaning is for a room on one of the preferred
        /// Levels, then the result is true.
        ///
        /// If the cleaning is for a room that is not on one of the
        /// preferred levels, then there is a calculation to figure
        /// out how far away the room is from one of those levels
        /// (using the Cleaning.Room.Floor values) and then will
        /// compare that against the parameter maxLevelsChange.
        /// </summary>
        static public Boolean Can_Attendant_Clean(Attendant a, List<Affinity> affinities, Cleaning cleaning, DateTime reference_date, int maxLevelsChange = 3)
        {
            // first check that times are compatible

            var timeslot = a.CurrentTimeSlot;
            // question if this will work as is, if if need to offset from earliest date
            var earliest_start = (long)timeslot.From.TotalMinutes;
            var latest_end = (long)timeslot.To.TotalMinutes;

            var from = (long)(cleaning.From - reference_date).TotalMinutes;
            var to = (long)(cleaning.To - reference_date).TotalMinutes;
            // if
            // from > latest_end, cleaning starts after attendant stops
            // to < earliest_start, cleaning ends before attendant starts
            // so cleaning is outside of range and cannot clean it
            if (from >= latest_end || to <= earliest_start)
            {
                Console.WriteLine($"skipping this case:from is {from}, to is {to}, worker shift is {earliest_start} to {latest_end}");
                return false;
            }

            // still here means passed the time overlap test.  Now
            // check if the floors are within allowed distance

            // if the List<Level> affinities is empty, then any floor is allowed
            if (affinities.Count() == 0)
            {
                // Console.WriteLine($"Attendant has no affinities floors, so returning true for {from} to {to}");
                return true;
            }

            // check each of the affinities floors versus the max level cost
            //
            // no, for now don't bother.  The need to limit floor
            // changes should be handled already in creating the
            // optional cleanings lists possible for the attendant
            Boolean result = false;
            // foreach (Affinity affinity in affinities)
            // {
            //     result = Is_Transition_Allowed(affinity, cleaning.Room.Floor, maxLevelsChange);
            //     // Console.WriteLine($"checking {a.Username}, affinities floor {level.LevelName} vs cleaning floor: {cleaning.Room.Floor.LevelName} result is {result}");
            //     if (result) break;
            // }
            result = true;
            return result;
        }
        static public int LevelToLevelDistance(Level fromFloor)
        {
            // else compute floor difference from zero
            //
            // note floor index is probably zero based, so maybe this
            // should include a +1??  but it doesn't really matter in
            // the end, as it all balances out
            //
            // 13 may 2021 bugfix
            // for hinkley, issue with indexing out of order and
            // sequential, so parse the LevelName and compare to zero
            // no idea why Split isn't working here, so using regex split
            string pattern = "-";
            // string pattern = "-|.";
            var matches = Regex.Split(fromFloor.LevelName, pattern);

            if (matches.Count() == 2 && Int32.TryParse(matches[1], out int j))
            {
                // Console.WriteLine($"unary level distance for {fromFloor.LevelName} is {j}");
                return j;
            }
            else
            {
                // maybe it is a single number?
                var resultString = Regex.Match(fromFloor.LevelName, @"\d+").Value;
                if (resultString != null && Int32.TryParse(resultString, out int k))
                {
                    return k;
                }
                else
                {
                    // fallback to index
                    Console.WriteLine($"level name {fromFloor.LevelName} could not be parsed as int-int combination or as a string with an embedded integer.  Using Floor index to get floor to zero travel.");
                    return fromFloor.FloorIndex;
                }
            }
        }

        static public int LevelToLevelDistance(Level fromFloor, Level toFloor)
        {
            if (fromFloor == toFloor) return 0;
            // else compute floor difference
            // Console.WriteLine($"binary level distance from {fromFloor.LevelName} to {toFloor.LevelName} is {Math.Abs(fromFloor.FloorIndex - toFloor.FloorIndex)}");
            return Math.Abs(LevelToLevelDistance(fromFloor) - LevelToLevelDistance(toFloor));
        }

        static public Boolean Is_Transition_Allowed(Level fromFloor, Level toFloor, int maxLevelsChange = 3)
        {
            if (fromFloor == toFloor) return true;
            // else compute floor difference
            int diff = Math.Abs(fromFloor.FloorIndex - toFloor.FloorIndex);
            return diff <= maxLevelsChange;
        }




    }
}
