using System;
using System.Collections.Generic;
using System.Text;
using Google.OrTools.Sat;
using System.Linq;

namespace Planner.CpsatCleaningCalculator
{
    public class CleaningInterval
    {
        public static void FixupAttendantPrePlannedRooms(Attendant attendant,
                                                         List<Cleaning> cleanings)
        {
            // if the attendant has preplanned cleanings, make sure
            // the room information on those cleanings is correct (has
            // floor, building, etc) by looking at the list of cleanings
            //
            // this should not be necessary.  The real bug is that the
            // incoming room information is not being populated
            // properly by the server when it gets the information
            // from the client, but I don't know that code and don't
            // want to break anything.  THis is a second best
            // solution, but should work okay
            if (attendant.Cleanings != null && attendant.Cleanings.Count() > 0)
            {
                // find each room in the master list of cleanings
                foreach (var assigned in attendant.Cleanings)
                {
                    if (assigned.Room.Floor == null)
                    {
                        var official_cleaning = cleanings.Where(c => (c.Room.RoomName == assigned.Room.RoomName) ||
                                                                (c.Room.PmsRoomName == assigned.Room.PmsRoomName)).First();
                        // add anything else needed here.  the current bug is no Floor
                        assigned.Room.Floor = official_cleaning.Room.Floor;
                    }
                }
            }
        }

        public static HashSet<Cleaning> FindPrePlannedCleanings(List<Attendant> attendants,
                                                                IEnumerable<Cleaning> cleanings)
        {
            var preplanned = new HashSet<Cleaning>();
            foreach (Attendant a in attendants)
            {
                preplanned.UnionWith(FindPrePlannedCleanings(a, cleanings));
            }
            return preplanned;
        }

        public static HashSet<Cleaning> FindPrePlannedCleanings(Attendant attendant,
                                                                IEnumerable<Cleaning> cleanings)
        {
            // for an attendant, find all of the preplanned cleanings in the list
            var preplanned = new HashSet<Cleaning>();
            if (attendant.Cleanings != null && attendant.Cleanings.Count() > 0)
            {
                // find each room in the master list of cleanings
                foreach (var assigned in attendant.Cleanings)
                {
                    var official_cleaning = cleanings.Where(c =>
                    {
                        var isPlanned = false;
                        if (! String.IsNullOrEmpty(c.Room.RoomName))
                            isPlanned = c.Room.RoomName == assigned.Room.RoomName;
                        else if (! String.IsNullOrEmpty(c.Room.PmsRoomName))
                            isPlanned = c.Room.PmsRoomName == assigned.Room.PmsRoomName;
                        return isPlanned;
                    });
                    if (official_cleaning.Count() > 0)
                        preplanned.Add(official_cleaning.First());
                }
            }
            return preplanned;
        }

        public static CleaningInterval CreateIntervalVar(CpModel model,
                                                         Attendant attendant,
                                                         DateTime reference_date,
                                                         Cleaning cleaning,
                                                         string flag,
                                                         HotelStrategy hotelStrategy)
        {

            // from and to need to be integer minutes.
            // Subtract actual date from the reference_date
            // found above, then get the total minutes from
            // that
            //
            var cleaningFrom = (long)(cleaning.From - reference_date).TotalMinutes;
            var cleaningTo = (long)(cleaning.To - reference_date).TotalMinutes;
            long plannedFrom = -1;
            long plannedTo = -1;

            int cleaningSpeed = CleaningIntervalConstraints.GetCleaningSpeed(attendant);
            // used to be that credits was minutes.  now it is not
            //
            int plannedDuration = (int) ((plannedTo - plannedFrom)/cleaningSpeed);
            // cleaning speed is an integer, the same as # of attendants.
            // but not sure if this is right...are credits divided among attendants?
            int credits = cleaning.Credits/cleaningSpeed;
            int minutes = hotelStrategy.CPSat.CreditsToMinutes(credits);
            if (hotelStrategy.CPSat.UsePrePlan )//for both cases
            {

                var a = attendant.Cleanings.Where(x => (x.Room.PmsRoomName == cleaning.Room.PmsRoomName) || (x.Room.RoomName == cleaning.Room.RoomName)).FirstOrDefault();
                //Here we force the solver to use already solved solution for certain cleaners
                if (a != null)
                {
                    plannedFrom = (long)(a.From - reference_date).TotalMinutes;
                    plannedTo = (long)(a.To - reference_date).TotalMinutes;
                    plannedDuration = (int) ((plannedTo - plannedFrom)/cleaningSpeed);

                    // Console.WriteLine($"planned cleaning  from is {a.From} in minutes is {plannedFrom}, to is {a.To}, in minutes is {plannedTo}, difference is {plannedTo - plannedFrom} versus credits of {a.Credits}");
                    // Console.WriteLine($"cleaning details: cleaning earliest allowed is {cleaningFrom} vs planned start at {plannedFrom}\n            cleaning latest allowed is {cleaningTo} vs planned end at {plannedTo}");

                    if (plannedFrom < cleaningFrom)
                    {
                        hotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"For cleaning {cleaning.Room.PmsRoomName}, earliest allowed cleaning time of {cleaning.From} is later than the pre-planned time of {a.From}.  The plan is using the pre-planned times, but double check that this wasn't an oversight, for example a Departure that is mistakenly scheduled as a Stay.");
                        cleaningFrom = plannedFrom;
                    }

                    if (plannedTo > cleaningTo)
                    {
                        hotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"For cleaning {cleaning.Room.PmsRoomName}, latest allowed cleaning time of {cleaning.To} is earlier than the pre-planned cleaning end time of {a.To}.  The plan is using the pre-planned times, but double check that this wasn't an oversight, for example an Arrival cleaning that is mistakenly scheduled as a Departure.");
                        cleaningTo = plannedTo;
                    }

                    // commenting this out for now.  credits are not time any more
                    // if (credits < plannedDuration)
                    // {
                    //     Console.WriteLine($"caution: cleaning credits of {credits} is less than the difference between the planned From {plannedFrom} and the planned To {plannedTo} of {plannedDuration}");
                    // }
                    if (minutes > plannedDuration)
                    {
                        Console.WriteLine($"Pay attention: cleaning credits of {credits} results in more minutes ({minutes}) than the difference between the planned From {plannedFrom} and the planned To {plannedTo} of {plannedDuration}.  Changing (reducing) the minutes to make this consistent");
                        minutes = plannedDuration;
                    }
                }
            }

            //pick the min between the attendant timeslot and
            //the max of the room cleaning by enforcing intervalvar
            IntVar start = model.NewIntVar(
                cleaningFrom,
                cleaningTo,
                $"start_attendant_{attendant.Username}_room_{cleaning.Room.RoomName} {flag}"); ;
            // Console.WriteLine($"start var is {start_var}");

            IntVar end = model.NewIntVar(
                cleaningFrom,
                cleaningTo,
                $"end_attendant_{attendant.Username}_room_{cleaning.Room.RoomName} {flag}");

            //maybe this attendant cleans the rooms
            //literal = is the room cleaned ?
            var literal_desc = $"interval_attendant_{attendant.Username}_cleans_room_{cleaning.Room.RoomName} {flag}";
            if (attendant.SecondWorkerUsername != null)
            {
                literal_desc = $"interval_attendant_{attendant.Username}_and_{attendant.SecondWorkerUsername}_clean_room_{cleaning.Room.RoomName} {flag}";
            }
            var lit = model.NewBoolVar(literal_desc);

            IntVar dur = model.NewConstant(minutes);
            IntervalVar interval = model.NewOptionalIntervalVar( //optional because the attendant might not clean this room
                start, dur, end, lit,
                $"interval_attendant_{attendant.Username}_room_{cleaning.Room.RoomName} {flag}");

            var ci = new CleaningInterval(cleaning, start, end, dur, interval, lit, credits, minutes);

            // finally, if we have a pre-plan for this attendant and cleaning, then
            // force that solution to be used
            if (plannedFrom >= 0)
            {
                // note the condition in the CleaningInterval for later use
                ci.From = plannedFrom;
                ci.To = plannedTo;

                // We give the solver a solution by setting constraints
                model.Add(start == plannedFrom);
                model.Add(end <= plannedTo);
                model.Add(lit == 1);
            }

            return ci;
        }

        private CleaningInterval(Cleaning c,
                                 IntVar s,
                                 IntVar e,
                                 IntVar d,
                                 IntervalVar i,
                                 IntVar l,
                                 int credits,
                                 int minutes
        )
        {
            Cleaning = c;
            Start = s;
            End = e;
            Duration = d;
            Interval = i;
            Literal = l;
            Credits = credits;
            Minutes = minutes;
        }
        public Cleaning Cleaning { get; }
        public IntVar Start { get; }
        public IntVar End { get; }
        public IntVar Duration { get; }
        public IntervalVar Interval { get; }
        public IntVar Literal { get; }
        public IntVar IsFirstLiteral { get; set; }
        public IntVar IsLastLiteral { get; set; }
        public long? From { get; set; }
        public long? To { get; set; }
        public int Credits { get; set; }
        public int Minutes { get; set; }
    }
}
