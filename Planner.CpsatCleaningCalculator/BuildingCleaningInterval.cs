using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Google.OrTools.Sat;

namespace Planner.CpsatCleaningCalculator
{
    public class BuildingCleaningInterval
    {

        public static BuildingCleaningInterval CreateIntervalVar(CpModel model,
                                                                 Attendant attendant,
                                                                 long earliest_start,
                                                                 string flag,
                                                                 List<LevelCleaningInterval> level_intervals)
        {
            // maybe make this a constant
            long horizon_time = 1000000000;
            string buildingname = level_intervals.First().Building.LevelName;
            IntVar start = model.NewIntVar(earliest_start, horizon_time, $"start_attendant_{attendant.Username}_building_{buildingname}_{flag}");
            IntVar end = model.NewIntVar(earliest_start, horizon_time, $"end_attendant_{attendant.Username}_building_{buildingname}_{flag}");

            IntVar dur = model.NewIntVar(0, horizon_time, $"duration_attendant_{attendant.Username}_building_{buildingname}_{flag}");
            var lit = model.NewBoolVar($"interval_attendant_{attendant.Username}_cleans_building_{buildingname}_{flag}");
            IntervalVar interval = model.NewOptionalIntervalVar(
                start,
                dur,
                end,
                lit,
                $"interval_attendant_{attendant.Username}_cleans_building_{buildingname}_{flag}");



            // Add model constraints to link this building interval with all possible level intervals
            foreach (var level_interval in level_intervals)
            {
                // bind the level cleaning decision with this building decision
                // if the level happens, then this building must happen
                model.AddImplication(level_interval.Literal, lit);
                // make times consistent
                var travel_to_level = LevelsMath.LevelToLevelDistance(level_interval.Floor);
                // Console.WriteLine($"start is {level_interval.Start}, travel_to_level is {travel_to_level}, literal is {level_interval.IsFirstLiteral}");
                model.Add(start == level_interval.Start - travel_to_level).OnlyEnforceIf(level_interval.IsFirstLiteral);
                model.Add(end == level_interval.End + travel_to_level).OnlyEnforceIf(level_interval.IsLastLiteral);

                // I think this is extra/redundant
                // model.Add(level_interval.Start >= start).OnlyEnforceIf(level_interval.Literal);
                // model.Add(level_interval.End <= end).OnlyEnforceIf(level_interval.Literal);

            }

            // Add condition that if levels in the building are cleaned, then the building literal is 0
            var anyLevelCleaned = LinearExpr.Sum(level_intervals.Select(c => c.Literal));
            model.Add(anyLevelCleaned > 0).OnlyEnforceIf(lit);
            model.Add(anyLevelCleaned == 0).OnlyEnforceIf(lit.Not());

            // add a constraint that the duration of the building interval must equal the sum of the contained (active) level durations
            // var sumDurationCleaned = LinearExpr.Sum(level_intervals.Select(lci => lci.Duration));
            // model.Add(sumDurationCleaned <= dur).OnlyEnforceIf(lit);

            var li = new BuildingCleaningInterval(buildingname, start, end, dur, interval, lit);
            li.From = earliest_start;
            li.To = horizon_time;

            var firstLevelStart = model.NewIntVar(0, horizon_time, $"atndnt {buildingname} first level start time");
            foreach (var levelInterval in level_intervals)
            {
                model.Add(firstLevelStart == levelInterval.Start).OnlyEnforceIf(levelInterval.IsFirstLiteral);
            }
            li.FirstLevelStart = firstLevelStart;

            return li;
        }


        private BuildingCleaningInterval(string buildingname, IntVar s, IntVar e, IntVar d, IntervalVar i, IntVar lit)
        {
            BuildingName = buildingname;
            Start = s;
            End = e;
            Duration = d;
            Interval = i;
            Literal = lit;
        }

        public String BuildingName { get; }
        public IntVar Start { get; }
        public IntVar End { get; }
        public IntVar Duration { get; }
        public IntervalVar Interval { get; }
        public IntVar Literal { get; }
        public IntVar IsFirstLiteral { get; set; }
        public IntVar FirstLevelStart { get; set; }
        public long From { get; set; }
        public long To { get; set; }
    }
}
