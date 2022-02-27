using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Google.OrTools.Sat;

namespace Planner.CpsatCleaningCalculator
{
    public class LevelCleaningInterval
    {

        public static LevelCleaningInterval CreateIntervalVar(CpModel model,
                                                              Attendant attendant,
                                                              DateTime reference_date,
                                                              string levelname,
                                                              List<CleaningInterval> level_cleaning_intervals,
                                                              IntVar roomsTravelTime,
                                                              HotelStrategy hotelStrategy,
                                                              string flag)
        {
            long maxgap = 10; // arbitrarily allow 10 minutes of room to room travel maybe??

            // grab building/floor/section/subsection for LevelCleaningInterval obj
            // (note fix with appropriate c# = ? a: b thing)
            // assume they are all the same, or they wouldn't be part
            // of the same level decision list
            Level building = null;
            if (level_cleaning_intervals[0].Cleaning.Room.Building == null)
            {
                building = new Level();
                building.LevelName = getBuildingName(level_cleaning_intervals[0].Cleaning.Room.Floor.LevelName);// level_cleaning_intervals[0].Cleaning.Room.Floor;
            }
            else
                building = level_cleaning_intervals[0].Cleaning.Room.Building;

            Level floor = level_cleaning_intervals[0].Cleaning.Room.Floor;
            Level section = level_cleaning_intervals[0].Cleaning.Room.Section;
            Level subsection = level_cleaning_intervals[0].Cleaning.Room.Subsection;

            var earliest_from = level_cleaning_intervals.Min(cleaning_interval => cleaning_interval.Cleaning.From);
            var latest_to = level_cleaning_intervals.Max(cleaning_interval => cleaning_interval.Cleaning.To);

            var levelfrom = (long)(earliest_from - reference_date).TotalMinutes;
            var levelto = (long)(latest_to - reference_date).TotalMinutes;

            // Console.WriteLine($"level interval creation, with levelfrom = {levelfrom}, levelto={levelto}, testing preplan");

            // check to see if there are any preplan values outside of the standard cleaning.From and cleaning.To range.  If so, must use those values to

            // note that the check here is minimum of
            // cleaning_interval.From, which is the preplan value.
            // NOT cleaning_interval.Cleaning.From, which is the
            // unscheduled range of allowed values.  so don't comment
            // this out thinking it is the same as above...it is not
            // the same
            var preplanfrom = level_cleaning_intervals.Min(cleaning_interval => cleaning_interval.From );
            if (preplanfrom != null)
                levelfrom = Math.Min(levelfrom, (long) preplanfrom);

            var preplanto = level_cleaning_intervals.Max(cleaning_interval => cleaning_interval.To );
            if (preplanto != null)
                levelto = Math.Max(levelto, (long) preplanto);

            // now handle an edge case of a really strange preplan with big gaps
            if (preplanto.HasValue && preplanfrom.HasValue)
            {
                long? preplannedCredits = level_cleaning_intervals.Sum(cleaning_interval => cleaning_interval.To - cleaning_interval.From);
                long ppC = preplannedCredits.HasValue ? preplannedCredits.Value : 0;
                long sumPreplanGap = (preplanto.Value - preplanfrom.Value) - ppC;
                maxgap = Math.Max(maxgap, sumPreplanGap);
            }

            // Console.WriteLine($"level interval creation, with levelfrom = {levelfrom}, levelto={levelto}, after testing for preplan");

            IntVar start = model.NewIntVar(levelfrom, levelto, $"start_attendant_{attendant.Username}_level_{levelname}");
            IntVar end = model.NewIntVar(levelfrom, levelto, $"end_attendant_{attendant.Username}_level_{levelname}");
            IntVar dur = model.NewIntVar(0, levelto - levelfrom, $"duration_attendant_{attendant.Username}_level_{levelname}");
            var lit = model.NewBoolVar($"interval_attendant_{attendant.Username}_cleans_level_{levelname}");
            IntervalVar interval = model.NewOptionalIntervalVar(
                start,
                dur,
                end,
                lit,
                $"interval_attendant_{attendant.Username}_level_{levelname}");

            // Add model constraints to link this interval with all possible cleaning events
            foreach (var cleaning_interval in level_cleaning_intervals)
            {
                // bind the cleaning decision with this level decision
                // if the cleaning happens, then this level must happen
                model.AddImplication(cleaning_interval.Literal, lit);
                // make times consistent
                model.Add(cleaning_interval.Start >= start).OnlyEnforceIf(cleaning_interval.Literal);
                model.Add(cleaning_interval.End <= end).OnlyEnforceIf(cleaning_interval.Literal);
            }

            // Add condition that if no cleanings, the level literal is 0
            var anyRoomsCleaned = LinearExpr.Sum(level_cleaning_intervals.Select(lci => lci.Literal));
            model.Add(anyRoomsCleaned > 0).OnlyEnforceIf(lit);
            model.Add(anyRoomsCleaned == 0).OnlyEnforceIf(lit.Not());

            // add a constraint that the duration of the level interval must equal the sum of the cleaning durations
            var sumDurationCleaned = LinearExpr.ScalProd(level_cleaning_intervals.Select(lci => lci.Literal),
                                                         level_cleaning_intervals.Select(lci => hotelStrategy.CPSat.CreditsToMinutes(lci.Cleaning.Credits)));
            model.Add(sumDurationCleaned + roomsTravelTime + maxgap >= dur ).OnlyEnforceIf(lit);

            var li = new LevelCleaningInterval(levelname, building, floor, section, subsection, start, end, dur, interval, lit, (preplanto.HasValue || preplanfrom.HasValue), flag);

            li.From = levelfrom;
            li.To = levelto;

            return li;
        }

        public static string getBuildingName(string floorName)
        {

            return floorName.Split("-")[0];
        }


        private LevelCleaningInterval(string levelname, Level building, Level floor, Level section, Level subsection, IntVar s, IntVar e, IntVar d, IntervalVar i, IntVar lit, bool isPrePlanned, string flag)
        {
            LevelName = levelname;
            Building = building;// new Level() {LevelName= getBuildingName(levelname) };
            Floor = floor;
            Section = section;
            Subsection = subsection;
            Start = s;
            End = e;
            Duration = d;
            Interval = i;
            Literal = lit;
            IsPrePlanned = isPrePlanned;
            Flag = flag;
        }

        public String Flag { get; }
        public String LevelName { get; }
        public Level Building { get; }
        // should never be null
        public Level Floor { get; }
        public Level Section { get; }
        public Level Subsection { get; }
        public IntVar Start { get; }
        public IntVar End { get; }
        public IntVar Duration { get; }
        public IntervalVar Interval { get; }
        public IntVar Literal { get; }
        public IntVar IsFirstLiteral { get; set; }
        public IntVar IsLastLiteral { get; set; }
        public bool IsPrePlanned { get; set; }
        public long? From { get; set; }
        public long? To { get; set; }
    }
}
