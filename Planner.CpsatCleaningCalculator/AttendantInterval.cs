using System;
using System.Collections.Generic;
using System.Text;
using Google.OrTools.Sat;
using System.Linq;

namespace Planner.CpsatCleaningCalculator
{
    public class AttendantInterval
    {
        public static AttendantInterval CreateIntervalVar(CpModel model,
                                                          Attendant attendant,
                                                          DateTime reference_date,
                                                          HotelStrategy hotelStrategy
        )
        {
            // for now assume one long working interval.

            // maybe later move to many, with breaks between

            // inspect the attendant's timeslot and any possible
            // preplanned cleanings to create the attendant interval
            var timeslot = attendant.CurrentTimeSlot;
            long tsFrom = (long)timeslot.From.TotalMinutes;
            long tsTo = (long)timeslot.To.TotalMinutes;

            // bugfix 2021-06-01 check preplan state, timing of
            // preplanned jobs to extend interval start, end if
            // necessary
            if (hotelStrategy.CPSat.UsePrePlan &&  attendant.Cleanings.Count() > 0)
            {

                var minFrom = attendant.Cleanings.Min(c => (long)(c.From - reference_date).TotalMinutes - LevelsMath.LevelToLevelDistance(c.Room.Floor));
                var maxTo = attendant.Cleanings.Max(c => (long)(c.To - reference_date).TotalMinutes + LevelsMath.LevelToLevelDistance(c.Room.Floor));


                // Expand the attendant interval if needed by preplan times
                if (minFrom < tsFrom)
                {
                    hotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Adjusted earliest start time of Attendant {attendant.Username} to {minFrom} (was {tsFrom}, due to earliest pre-assigned cleaning");
                    tsFrom = minFrom;
                }
                if (tsTo < maxTo)
                {
                    hotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Adjusted latest end time of Attendant {attendant.Username} to {maxTo} (was {tsTo}, due to latest pre-assigned cleaning");
                    tsTo = maxTo;
                }
            }

            // question if this will work as is, if if need to offset from earliest date
            var start = model.NewConstant(tsFrom);
            var end = model.NewConstant(tsTo);
            var dur = model.NewConstant(tsTo - tsFrom);
            // Console.WriteLine($"{attendant.Username} start {start} end {end}");
            // eventually, change the name of this literal to note the time period name too
            var lit = model.NewBoolVar($"Atndt {attendant.Username} is active");
            var interval = model.NewOptionalIntervalVar(start, dur, end, lit,
                                                        $"interval_atndt_{attendant.Username}");
            // when I make multiple intervals, need to add no overlap constraint to them too
            // then do work_periods.Add(new AttendantInterval(...));

            return new AttendantInterval(attendant, start, end, dur, interval, lit);
        }

        private AttendantInterval(Attendant a,
                                  IntVar s,
                                  IntVar e,
                                  IntVar d,
                                  IntervalVar i,
                                  IntVar l)
        {
            Attendant = a;
            Start = s;
            End = e;
            Duration = d;
            Interval = i;
            Literal = l;
        }
        public Attendant Attendant { get; }
        public IntVar Start { get; }
        public IntVar End { get; }
        public IntVar Duration { get; }
        public IntervalVar Interval { get; }
        public IntVar Literal { get; }

    }
}
