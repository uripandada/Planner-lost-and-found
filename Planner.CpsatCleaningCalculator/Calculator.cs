using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Google.OrTools.Sat;
using System.Globalization;
using System.Xml.Linq;
using System.Data.Common;
using System.Text;
using System.Net.Http;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.CompilerServices;

namespace Planner.CpsatCleaningCalculator
{

    public struct Date : IComparable<Date>
    {
        private readonly DateTime _dateTime;

        private Date(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public static implicit operator Date(DateTime dateTime)
        {
            return new Date(dateTime);
        }

        public static implicit operator DateTime(Date date)
        {
            return date._dateTime;
        }

        public override string ToString()
        {
            return _dateTime.ToString("yyyy-MM-dd");
        }

        private class DateConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return new Date(DateTime.Parse((string)value));
            }
        }

        public int CompareTo(Date other)
        {
            return _dateTime.CompareTo(other._dateTime);
        }
    }

    public class Level
    {
        public string LevelName { get; set; }
        public int RoomsCount { get; set; }
        public int FloorIndex { get; set; }
    }

    public class Attendant : Worker
    {
        public Guid? CleaningGroupId { get; set; }
        public List<Cleaning> Cleanings { get; set; } = new List<Cleaning>();
    }

    public class TimeSlot
    {
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public int MaxCredits { get; set; }
        public int noOfRooms { get; set; }
        public int MaxLevels { get; set; }
        public int MaxDepartures { get; set; }
        public int MaxStays { get; set; }
        public bool IsPreferred { get; set; }

        //public Affinity_OLD Affinity { get; set; }
        public List<Affinity> Affinities { get; set; }

        public List<string> Levels { get; set; } = new List<string>();
    }

    /// <summary>
    /// TODO: REMOVE THIS CLASS AFTER REFACTORING TO AFFINITIES!
    /// TODO: REMOVE THIS CLASS AFTER REFACTORING TO AFFINITIES!
    /// TODO: REMOVE THIS CLASS AFTER REFACTORING TO AFFINITIES!
    /// TODO: REMOVE THIS CLASS AFTER REFACTORING TO AFFINITIES!
    /// </summary>
    public partial class Affinity_OLD
    {
        public string Buildings { get; set; }
        public string Rooms { get; set; }
        public string RoomTypes { get; set; }
        public string Levels { get; set; }
        public string CleaningTypes { get; set; }
    }

    public enum AffinityType
	{
        UNKNOWN,
        BUILDING,
        FLOOR,
        FLOOR_SECTION,
        FLOOR_SUB_SECTION
	}

    public partial class Affinity
    {
        public AffinityType AffinityType { get; set; }

        public string Building { get; set; }
        public string Room { get; set; }
        public string RoomType { get; set; }

        /// <summary>
        /// Format is {Building(optional)}:{Floor(optional)}:{Section(optional)}:{Subsection(optional)}
        /// </summary>
        public string Level { get; set; }
        public string Floor { get; set; }
        public string Section { get; set; }
        public string SubSection { get; set; }
        public string CleaningType { get; set; }
    }

    public abstract class Worker
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        // public bool? IsSelected { get; set; }
        public string SecondWorkerUsername { get; set; }
        public bool? IsSecondWorker { get; set; }
        public TimeSlot[] CurrentTimeSlots { get; set; }
        public TimeSlot CurrentTimeSlot => CurrentTimeSlots.First(); //TODO use all time slots
    }

    public abstract class Plan
    {
        public string WorkerUsername { get; set; }
        public string CreatedBy { get; set; }
        public abstract DateTime From { get; set; }
        public abstract DateTime To { get; set; }
    }

    public class HotelStrategy
    {
        public CPSat CPSat { get; set; }
        public TimeSpan DefaultAttendantStartTime { get; set; } = TimeSpan.FromHours(8);
        public TimeSpan DefaultAttendantEndTime { get; set; } = TimeSpan.FromHours(20);
        public int DefaultMaxCredits { get; set; } = 0;
        public int ReserveBetweenCleanings { get; set; } = 15;
        public int TravelReserve { get; set; } = 5;

        public TimeSlot GetDefaultTimeSlot()
        {
            return new TimeSlot
            {
                From = DefaultAttendantStartTime,
                To = DefaultAttendantEndTime,
                MaxCredits = DefaultMaxCredits
            };
        }
    }

    public class Room
    {
        public string Id { get; set; }
        public bool IsDirty { get; set; }
        public string RoomName { get; set; }

        public string PmsRoomName { get; set; }

        public string RoomType { get; set; }

        //public string HotelInGroupName { get; set; }

        //public HousekeepingStatus HousekeepingStatus { get; set; }

        //public GuestCleaningStatus GuestCleaningStatus { get; set; }

        //public string RcHousekeepingCode { get; set; }

        public string Address { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        //public int? Credits { get; set; }

        //public string GroupId { get; set; }

        public string GroupName { get; set; }

        //public List<Reservation> Reservations { get; set; } = new List<Reservation>();

        //public List<Cleaning> Cleanings { get; set; } = new List<Cleaning>();

        //public List<OverridenCleaning> OverridenCleanings { get; set; } = new List<OverridenCleaning>();

        //public List<Cleaning> CleaningsForecast { get; set; } = new List<Cleaning>();

        ////public List<Hosting> Hostings { get; set; } = new List<Hosting>();

        ////public List<Running> Runnings { get; set; } = new List<Running>();

        //public JObject OtherProperties { get; set; }

        //public Dictionary<string, object> PlannerProperties { get; set; } = new Dictionary<string, object>();

        //public object this[string plannerPropertyName]
        //{
        //    get => PlannerProperties.TryGetValueOrNull(plannerPropertyName);
        //    set => PlannerProperties[plannerPropertyName] = value;
        //}

        public Level Floor { get; set; }

        //private Level building;

        public Level Building
        {
            get
            {
                var matches = Regex.Split(Floor.LevelName, "-");
                if (matches.Length == 2)
                    return new Level() { LevelName = matches[0] };
                return new Level() { LevelName = "0" };

            }
        }

        public Level Section { get; set; }

        public Level Subsection { get; set; }

        public int IndexOnFloor { get; set; }
        //public RoomType Type { get; set; }

        //public void SetFloor(Level floor, Level section, Level subsection)
        //{
        //    Floor = floor;
        //    Section = section;
        //    Subsection = subsection;
        //    IndexOnFloor = floor.RoomsCount++;
        //}
    }

    public abstract class Activity
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Credits { get; set; }
        public string Label { get; set; }
        public string Tags { get; set; }
        public Room Room { get; set; }
        public List<string> Levels { get; set; } = new List<string>();

        public abstract Plan GetPlan();
    }

    public class CleaningPlan : Plan
    {
        public override DateTime From { get; set; }
        public override DateTime To { get; set; }

        //public Attendant Attendant { get; set; }

        public override string ToString()
        {
            return $"Cleaning plan ({From:HH:mm}:{To:HH:mm}: {WorkerUsername})";
        }
    }

    public class Cleaning : Activity
    {
        //---------- newly added properties
        public Guid Id { get; set; }
        public Guid? CleaningPluginId { get; set; }
        public string CleaningPluginName { get; set; }
        public bool IsActive { get; set; }
        public bool IsCustom { get; set; }
        public bool IsPostponed { get; set; }
        public bool IsChangeSheets { get; set; }
        //---------- newly added properties


        public CleaningType Type { get; set; }
        public bool ArrivalExpected { get; set; }
        public CleaningPlan Plan { get; set; }

        public override Plan GetPlan()
        {
            return this.Plan;
        }
    }

    //public delegate void CpsatPlannerProgressChangedEventHandler(object sender, CleaningPlannerCPSAT.ProgressMessage message);

    public enum CpsatProgressStatus
    {
        STARTED,
        IN_PROGRESS,
        FINISHED_NOTHING_TO_DO,
        FINISHED_INFEASIBLE,
        FINISHED_INVALID,
        FINISHED_UNKNOWN,
        SOLVING,
        IN_PROGRESS_ALERT,
        FINISHED,
    }

    public class AutoGeneratedPlan
    {
        public string HotelId { get; set; }
        //public DateTime CleaningDate { get; set; }
        //public bool IsTodaysCleaningPlan { get; set; }

        public CleaningPlannerContext CleaningContext { get; set; }
        public IEnumerable<Cleaning> PlannedCleanings { get; set; }
    }

    public class CleaningPlannerCPSAT
    {
        public class ProgressMessage
        {
            public Guid CleaningPlanId { get; set; }
            public string StatusKey { get; set; }
            public string Message { get; set; }
            public string DateTimeString { get; set; }
        }

        private readonly CleaningPlannerContext _context;
        private readonly string _hotelId;

        public event EventHandler<ProgressMessage> CpsatPlannerProgressChanged;
        public event EventHandler<AutoGeneratedPlan> CpsatPlannerResultsGenerated;

        public void OnCpsatPlannerProgressChanged(ProgressMessage message)
        {
            var handler = this.CpsatPlannerProgressChanged;
            handler?.Invoke(this, message);
        }

        public void OnCpsatPlannerResultsGenerated(AutoGeneratedPlan plan)
        {
            var handler = this.CpsatPlannerResultsGenerated;
            handler?.Invoke(this, plan);
        }

        public CleaningPlannerCPSAT(CleaningPlannerContext context, string hotelId)
        {
            this._context = context;
            this._hotelId = hotelId;
        }

        public void Plan()
        {
            var timeZoneInfo = TimeZoneConverter.TZConvert.GetTimeZoneInfo(this._context.TimeZoneId);
            //TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm")

            //var x = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = "Solver algorithm started",
                StatusKey = CpsatProgressStatus.STARTED.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });

            // parameters for the solver call
            int solver_run_time = _context.HotelStrategy.CPSat.solverRunTime;

            // THESE NEED TO BE TAKEN FROM ENVIRONMENT OR CONFIG FILES
            Boolean log_progress = true;
            int cpus = 16;

            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Clear();

            var primaryAttendants = _context.Attendants.Where(a => !(a.IsSecondWorker ?? false)).ToList();
            var secondaryAttendants = _context.Attendants.Where(a => (a.IsSecondWorker ?? false)).ToList();
            Dictionary<Guid, int> attendantLookupIndex = primaryAttendants.Select((attendant, i) => new { attendant, i }).ToDictionary(p => p.attendant.Id, p => p.i);

            Boolean target_mode = _context.HotelStrategy.CPSat.useTargetMode; // true to aim for exactly max time and/or rooms
            Boolean target_mode_minimize_attendants = false;// _context.HotelStrategy.CPSat.targetModeMinimizeAttendants;

            //Constraints
            int large_number = 1000000000;
            int min_rooms = _context.HotelStrategy.CPSat.minRooms;
            int max_rooms = _context.HotelStrategy.CPSat.maxRooms;
            if (max_rooms == 0)
            {
                decimal cleaningsPerAttendant = _context.Cleanings.Count() / _context.Attendants.Count();
                // add 2 for a saftey
                max_rooms = (int)Math.Ceiling(cleaningsPerAttendant) + 2;
                _context.HotelStrategy.CPSat.maxRooms = max_rooms;
                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = $"WARNING: Max Cleanings cannot be 0.  Changed to {max_rooms}",
                    StatusKey = CpsatProgressStatus.STARTED.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });

            }

            int min_credits = _context.HotelStrategy.CPSat.minCredits;
            int max_credits = _context.HotelStrategy.CPSat.maxCredits;
            if (max_credits == 0)
            {
                var max_credits_per_room= (int)(_context.Cleanings.Max(c => c.Credits));
                max_credits = max_credits_per_room * max_rooms;
                //max_credits = (int)(_context.Cleanings.Sum(c => c.Credits)/_context.Attendants.Count());
                _context.HotelStrategy.CPSat.maxCredits = max_credits;
                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = $"WARNING: Max Credits cannot be 0.  Changed to {max_credits}",
                    StatusKey = CpsatProgressStatus.STARTED.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });
            }
            if (target_mode)
            {
                if (min_rooms > 0 || min_credits > 0)
                {
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("min rooms and min credits set to 0 for target mode");
                }
                min_rooms = 0;
                min_credits = 0;
                //target_mode_minimize_attendants = true;
            }
            // determine the credts to assume for a room, for saying that an attendant is at max effort.  Also used here and there as a way to proxy credits by a room cleaning
            var max_effort_credits =_context.Cleanings == null || !_context.Cleanings.Any() ? 0 : _context.Cleanings.Max(c => c.Credits) - 1;

            //int min_travel_time = 0;
            int max_travel_time = _context.HotelStrategy.CPSat.maxTravelTime;
            //it is defined in in

            int max_number_levels_per_attendant = _context.HotelStrategy.CPSat.maxNumberOfLevelsPerAttendant;// 10;//Sections/Subsections and floors and building.  want this larger
            if (max_number_levels_per_attendant == 0)
                max_number_levels_per_attendant = large_number;

            int max_building_count_per_attendant = _context.HotelStrategy.CPSat.maxNumberOfBuildingsPerAttendant==0? _context.HotelStrategy.CPSat.maxNumberOfBuildingsPerAttendant = 1: _context.HotelStrategy.CPSat.maxNumberOfBuildingsPerAttendant;
            if (max_building_count_per_attendant == 0)
                max_building_count_per_attendant = large_number;

            var total_assigned = new Dictionary<Guid, IntVar> { };
            var attendant_stays_cleaned = new Dictionary<Guid, IntVar> { };
            var attendant_departs_cleaned = new Dictionary<Guid, IntVar> { };
            var total_deviations = new Dictionary<Guid, IntVar> { };
            var total_room_travel = new List<IntVar> { };
            var total_floor_travel = new List<IntVar> { };
            var total_building_travel = new List<IntVar> { };
            var total_levels_cleaned = new Dictionary<Guid, IntVar> { };
            var total_buildings_cleaned = new Dictionary<Guid, IntVar> { };
            var total_credits = new Dictionary<Guid, IntVar> { };
            var total_credit_reductions = new Dictionary<Guid, IntVar> { };

            var total_buildings_award = new Dictionary<Guid, IntVar> { };
            var total_levels_award = new Dictionary<Guid, IntVar> { };
            var total_rooms_award = new Dictionary<Guid, IntVar> { };


            // objective function weights
            // positive means add to objective (increase its value= good things)
            // negative=penalty, subtract from objective (bad things)

            int weight_travel_time = _context.HotelStrategy.CPSat.weightTravelTime;// -1;//i want to subtract travel time from the objective function
            // may also want separate weight_floor_travel and
            // weight_building_travel (both negative) for the separate
            // building to building and floor to floor travel
            // components.

            int weight_credits = _context.HotelStrategy.CPSat.weightCredits;// 1;//i can add 1 rooms of 15 min or 15 rooms of 1 min
            if (weight_credits < 1)
                weight_credits = 1;  // Primary objective is to maximize cleaning, so the minimum useful value is 1
            int weightFloorsCompleted = _context.HotelStrategy.CPSat.weightFloorsCompleted;
            if (weightFloorsCompleted < 0)
                weightFloorsCompleted = 0;
            // Console.WriteLine($"weightFloorsCompleted = {weightFloorsCompleted}");

            int weight_rooms_cleaned = 1;// _context.HotelStrategy.CPSat.weightRoomsCleaned;

            int weight_epsilon_credits = _context.HotelStrategy.CPSat.epsilonCredits;// -1; // minimize differences in attentant credits
            int weight_epsilon_rooms = _context.HotelStrategy.CPSat.epsilonRooms;//0; // -30 adding 1 more room will penalize 30 minimize differences in attentant rooms

            Boolean balance_stay_depart_mode = _context.HotelStrategy.CPSat.balanceStayDepartMode; // true to balance numbers of stays and departures
            int weight_epsilon_stay_depart = 0; // must default to zero in case when balance stay depart is false
            if (balance_stay_depart_mode)
            {
                weight_epsilon_stay_depart = _context.HotelStrategy.CPSat.weightepsilonStayDepart;// suggested value is -15. Must be zero or negative
                if (weight_epsilon_stay_depart >= 0)
                    weight_epsilon_stay_depart = -max_effort_credits;
            }
            int max_departs = _context.HotelStrategy.CPSat.maxDepartures; // can override in timeslot per attendant
            int max_stays = _context.HotelStrategy.CPSat.maxStays;        // can override in timeslot per attendant

            // var building_distance_matrix = _context.HotelStrategy.CPSat.buildingsDistanceMatrix;
            // Distances distances = Distances.LoadFromFile(building_distance_matrix);
            var roomDistances = _context.RoomDistances;
            var buildDistances = _context.BuildingDistances;

            // target mode also might need more encouragement to drop
            // extra attendants.  set this larger (more negative) to
            // do that.  In testing:
            // 0 gave two workers one job each, 7 workers max jobs (not ideal)
            // -1 gave one worker 2 jobs, 7 workers max jobs (ideal)
            // -100 was too big, using just 7 workers and leaving two rooms uncleaned.
            // for balance mode, it should probably be zero
            // change to boolean
            int weight_minimize_attendants = 0;
            if (target_mode_minimize_attendants)
                weight_minimize_attendants = -1;
                // if there are more attendants than rooms, then the
                // problem is infeasible if all attentants are active.
                // But if the attendants are allowed to be given no
                // work, then the best solution is to just use one
                // attendant.  So in the edge case that attentants
                // count is greater than cleanings count, make
                // weight_minimize_attendants positive
            else
                weight_minimize_attendants = 100;



            Boolean maxDeparturesReducesCredits = _context.HotelStrategy.CPSat.maxDeparturesReducesCredits; // true means level changes reduces cleaning credits
            int maxDeparturesEquivalentCredits = _context.HotelStrategy.CPSat.maxDeparturesEquivalentCredits; // the cost in credits of changing a level (10?)
            int maxDeparturesThreshold = _context.HotelStrategy.CPSat.maxDeparturesReductionThreshold; // the point at which level changes trigger level movement credit reduction

            Boolean maxStaysIncreasesCredits = _context.HotelStrategy.CPSat.maxStaysIncreasesCredits; // true means level changes increases cleaning credits
            int maxStaysEquivalentCredits = _context.HotelStrategy.CPSat.maxStaysEquivalentCredits; // the cost in credits of changing a level (10?)
            int maxStaysThreshold = _context.HotelStrategy.CPSat.maxStaysIncreaseThreshold; // the point at which level changes trigger level movement credit increasing

            Boolean level_movement_reduces_credits = _context.HotelStrategy.CPSat.levelMovementReducesCredits; // true means level changes reduces cleaning credits
            int level_movement_equivalent_credits = _context.HotelStrategy.CPSat.levelMovementEquivalentCredits; // the cost in credits of changing a level (10?)
            int level_movement_threshold = _context.HotelStrategy.CPSat.levelMovementReductionThreshold; // the point at which level changes trigger level movement credit reduction

            Boolean building_movement_reduces_credits = _context.HotelStrategy.CPSat.buildingMovementReducesCredits; // true means bldg changes reduces cleaning credits
            int building_movement_equivalent_credits = _context.HotelStrategy.CPSat.buildingMovementEquivalentCredits; // the cost in credits of changing a bldg (20?)

            // Each attendant might have a list of preferred levels
            // and rooms.  or maybe a preferred floor, that gives the
            // bonus to all sections and subsections on that floor.
            //
            // If they do, then there is a bonus value for performing
            // a cleaning on that level.  That's the reward for
            // indiviudal levels
            int award_level = _context.HotelStrategy.CPSat.awardLevel;// 5;
            int award_room = _context.HotelStrategy.CPSat.awardRoom;// 10;
            int award_building = _context.HotelStrategy.CPSat.awardBuilding;// 10;

            // the sum of all awards is then weighted in the final objective function
            int weight_level_award = _context.HotelStrategy.CPSat.weightLevelAward;// 1;
            int weight_room_award = _context.HotelStrategy.CPSat.weightRoomAward;//1;
            if (weight_room_award == 0)
                weight_room_award = 1;
            int weight_building_award = _context.HotelStrategy.CPSat.weightBuildingAward;//1;

            int averageCreditsPerRoom = (int)Math.Ceiling(_context.Cleanings.Average(c => c.Credits));
            // adjust weight epsilon rooms if needed
            if (weight_epsilon_rooms != 0 && weight_epsilon_rooms > -averageCreditsPerRoom)
                // make sure that epsilon rooms weight is large enough to make an impact
                weight_epsilon_rooms = -averageCreditsPerRoom;

            // set the penalty for changing floors
            int weight_level_changes = _context.HotelStrategy.CPSat.weightLevelChange; // * averageCreditsPerRoom;
            // Think it about "it is worth changing level for cleaning N rooms"

            // goal is to choose a weight that will allow solver to
            // change levels to clean a room, but only if it is
            // beneficial.  -20 works okay, as in my test data I've
            // set cost of cleaning a room at 20 minutes.  A weight
            // like -25 means that the solver won't change levels just
            // to clean one room, but must find multiple rooms to
            // clean, because changing levels will cost 25, but
            // cleaning one room will only add 20.

            // var usePreAffinity = _context.HotelStrategy.CPSat.usePreAffinity;

            Dictionary<string, Level> unique_floors = new Dictionary<string, Level>();
            // initialize unique floors from cleanings
            // this is a hack because I can't find a way to access hotel directly
            LevelsMath.GetFloorsFromCleanings(_context.Cleanings.ToList(), unique_floors);
            // Creates the model.
            CpModel model = new CpModel();

            // Stash jobs in a dictionary
            // key is attendant, value is a list of cleaning decision variables
            var all_cleanings = new Dictionary<Guid, List<CleaningInterval>>();
            var all_level_cleanings = new Dictionary<Guid, List<LevelCleaningInterval>>();
            var all_building_cleanings = new Dictionary<Guid, List<BuildingCleaningInterval>>();

            var per_attendant_room_travel = new Dictionary<Guid, List<IntVar>>();
            var per_attendant_floor_travel = new Dictionary<Guid, List<IntVar>>();
            var per_attendant_building_travel = new Dictionary<Guid, List<IntVar>>();

            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = "Solver scanning input data...",
                StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });

            // Loop 0
            //
            // scan the cleanings and find the earliest date, so as to
            // properly compute absolute minutes in the next loop
            //
            DateTime earliest_date = _context.Cleanings.Min(cleaning => cleaning.From);
            // shift to midnight
            earliest_date = earliest_date.Add(-earliest_date.TimeOfDay);

            var earliest_noon = earliest_date.AddHours(12);
            // Loop 0.5
            //
            // scan the cleanings and identify per building those cleanings that must start after noon.
            var preplannedCleanings = CleaningInterval.FindPrePlannedCleanings(primaryAttendants, _context.Cleanings);
            var buildingsCleanings = BuildingCleanings.makeBuildingsCleanings(_context.Cleanings, earliest_date, earliest_noon, preplannedCleanings);

            // convenience for later
            var levelCleaningsLookup = new Dictionary<string, LevelCleanings>();
            foreach (var bc in buildingsCleanings)
            {
                var pm_start = bc.GetPMStart();
                foreach (var lc in bc.MorningLevelsCleanings)
                {
                    levelCleaningsLookup.Add($"{lc.LevelName}-", lc);
                }
                foreach (var lc in bc.AfternoonLevelsCleanings)
                {
                    levelCleaningsLookup.Add($"{lc.LevelName}-{pm_start}", lc);
                }
            }

            // if there are affinities set in the UI, then do not use the preAffinity code
            // var noPreferredLevel = primaryAttendants.Where(a => a.CurrentTimeSlot?.Affinity == null ||
            //                                                (a.CurrentTimeSlot?.Affinity.Levels == null &&
            //                                                 a.CurrentTimeSlot?.Affinity.Buildings == null));
             var noPreferredLevel = primaryAttendants.Where(a => a.CurrentTimeSlot?.Affinities == null  || a.CurrentTimeSlot.Affinities.Count() == 0).ToList();
            // if there are no affinities set in the UI, then noPreferredLevel.Count() will equal the number of attendants
            // if they are not equal, then do not use the preaffinity code
            // if (noPreferredLevel.Count() < primaryAttendants.Count())
            //     usePreAffinity = false;

            // scan the attendants, see if any have preferred buildings or floors
            // if (usePreAffinity)
            // {
            //     LevelIntervalConstraints.AssignPreferredLevels(primaryAttendants.ToList(),
            //                                                              _context.Cleanings.ToList(),
            //                                                              buildingsCleanings,
            //                                                              _context.HotelStrategy);
            // }


            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = "Sover Loop 2: iterate over attendants",
                StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });

            // for epsilon
            // first the expected average cleaning time is total
            // credits possible divided by number of attendants
            // or
            // the maximum credits per attendant
            //
            // Loop 1

            var room_literals = new Dictionary<Cleaning, List<IntVar>>(); //cleaning is key
            var attendant_intervals = new Dictionary<Guid, AttendantInterval>();
            var attendant_literals = new Dictionary<Guid, IntVar>();
            var level_literals = new Dictionary<string, List<IntVar>>(); // floor levelname is key
            var attendantStarts = new List<IntVar>();

            var allCommunicatingRoomIntervals = new Dictionary<Guid, IEnumerable<IEnumerable<CleaningInterval>>>();

            var sortedAttendants = new List<Attendant>(primaryAttendants);
            sortedAttendants.Sort(delegate(Attendant a, Attendant b)
            {
                var aNoAffinity = a.CurrentTimeSlot?.Affinities == null  || a.CurrentTimeSlot.Affinities.Count() == 0;
                var bNoAffinity = b.CurrentTimeSlot?.Affinities == null  || b.CurrentTimeSlot.Affinities.Count() == 0;
                // var aNoAffinity = (a.CurrentTimeSlot?.Affinity == null ||
                //                    (a.CurrentTimeSlot?.Affinity.Levels == null &&
                //                     a.CurrentTimeSlot?.Affinity.Buildings == null));

                // var bNoAffinity = (b.CurrentTimeSlot?.Affinity == null ||
                //                    (b.CurrentTimeSlot?.Affinity.Levels == null &&
                //                     b.CurrentTimeSlot?.Affinity.Buildings == null));

                if (aNoAffinity && bNoAffinity)
                    return 0;
                else if (aNoAffinity) return 1;
                else if (bNoAffinity) return -1;
                else return 0;
            });

            foreach (Attendant attendant in sortedAttendants)
            {
                var attendant_min_rooms = min_rooms;
                List<CleaningInterval> attendant_cleanings = new List<CleaningInterval>();
                if (_context.HotelStrategy.CPSat.UsePrePlan && attendant.Cleanings.Count() > 0)
                {
                    //Console.WriteLine("maybe fixup attendants assigned cleanings");
                    CleaningInterval.FixupAttendantPrePlannedRooms(attendant, _context.Cleanings.ToList());
                }
                var attendant_interval = AttendantInterval.CreateIntervalVar(model, attendant, earliest_date, _context.HotelStrategy);
                attendant_intervals.Add(attendant.Id, attendant_interval);
                attendant_literals.Add(attendant.Id, attendant_interval.Literal);

                // if we're not minimizing attendants, then require
                // that all attendants are active
                //if (weight_minimize_attendants == 0)
                //    model.Add(attendant_interval.Literal == 1);

                // set the attendant's floor preference: look in the
                // attendant's current time slot for floor
                // preferences, use that to get the actual floor from
                // unique_floors
                // List<Planner.Web.Model.Level> preferred_buildings = new List<Planner.Web.Model.Level>();
                var affinities = new List<Affinity>();
                // List<Planner.Web.Model.Room> preferred_rooms = new List<Planner.Web.Model.Room>();
                if (attendant.CurrentTimeSlot?.Affinities != null && attendant.CurrentTimeSlot?.Affinities.Count()>0)
                {
                    affinities.AddRange(attendant.CurrentTimeSlot.Affinities);
                    if (_context.HotelStrategy.CPSat.preferredLevelsAreExclusive)
                        attendant_min_rooms = 0;
                }
                var rooms_cleaned = model.NewIntVar(attendant_min_rooms, max_rooms, $"atndnt {attendant.Username}: assigned rooms");

                //In case of HOST, just put the remainer of the last cleaner
                if (attendant.CurrentTimeSlot.noOfRooms > 0)
                {//Should we put the min_room to 0
                    rooms_cleaned = model.NewIntVar(attendant_min_rooms, attendant.CurrentTimeSlot.noOfRooms, $"{attendant.Username}: {attendant.CurrentTimeSlot.noOfRooms} rooms");
                }
                if (_context.HotelStrategy.CPSat.UsePrePlan)
                {
                    // Console.WriteLine("use plreplan case");
                    if (attendant.Cleanings.Count() > 0)
                    {
                        // Console.WriteLine($"attendant {attendant.Username} has non zero cleanings");
                        if (!_context.HotelStrategy.CPSat.CompletePrePlan)
                        {//the plan is FIXED ahead
                            // Console.WriteLine($"PrePlan is fixed.  {attendant.Username}: pre assigned  {attendant.Cleanings.Count()} as rooms cleaned");
                            rooms_cleaned = model.NewConstant(attendant.Cleanings.Count(), $"atndnt {attendant.Username}: pre-assigned {attendant.Cleanings.Count()} rooms");
                        }
                        else
                        {
                            // might need to fix mismatch between preassigned count and the attendant's or global min or max room allowed.  pre plan always wins if a conflict
                            if (attendant.CurrentTimeSlot.noOfRooms > 0 && attendant.CurrentTimeSlot.noOfRooms < attendant.Cleanings.Count())
                                // Console.WriteLine($"{attendant.Username}: Adjust rooms cleaned max from current time slot of {attendant.CurrentTimeSlot.noOfRooms} to  {attendant.Cleanings.Count()}");
                                // need to increase the max allowed in the IntVar
                                rooms_cleaned = model.NewIntVar(attendant_min_rooms, attendant.Cleanings.Count(), $"atndnt {attendant.Username}: {attendant.Cleanings.Count()} max assigned rooms");
                            if (attendant.CurrentTimeSlot.noOfRooms == 0 && max_rooms < attendant.Cleanings.Count())
                                // Console.WriteLine($"{attendant.Username}: Adjust rooms cleaned max from global max of {max_rooms} to  {attendant.Cleanings.Count()}");
                                // need to increase the max allowed in the IntVar
                                rooms_cleaned = model.NewIntVar(attendant_min_rooms, attendant.Cleanings.Count(), $"atndnt {attendant.Username}: {attendant.Cleanings.Count()} max assigned rooms");
                        }
                    }
                }

                // collect "rooms_cleaned" by this attendant intvar for objective
                total_assigned.Add(attendant.Id, rooms_cleaned);
                // So, first buildings, then for each building levels, then for each level cleanings

                // set up per-building details
                var buildingsCleaned = model.NewIntVar(0, max_building_count_per_attendant, $"atndnt {attendant.Username}: building count");
                total_buildings_cleaned.Add(attendant.Id, buildingsCleaned);
                var attendant_building_cleanings = new List<BuildingCleaningInterval>();
                var attendant_building_travel = new List<IntVar> { };

                var levelsCleaned = model.NewIntVar(0, max_number_levels_per_attendant, $"atndnt {attendant.Username}: levels cleaned");
                // check versus time slot
                if (attendant.CurrentTimeSlot.MaxLevels > 0)
                    levelsCleaned = model.NewIntVar(0, attendant.CurrentTimeSlot.MaxLevels, $"atndnt {attendant.Username}: levels cleaned");
                total_levels_cleaned.Add(attendant.Id, levelsCleaned);


                var attendant_level_cleanings = new List<LevelCleaningInterval>();
                var attendant_floor_travel = new List<IntVar> { };
                var attendant_room_travel = new List<IntVar> { };

                BuildingIntervalConstraints.Make_Building_Intervals(
                    model,
                    attendant,
                    buildingsCleanings,
                    attendant_interval,
                    attendant_cleanings,
                    attendant_level_cleanings,
                    attendant_building_cleanings,
                    attendant_room_travel,
                    attendant_floor_travel,
                    attendant_building_travel,
                    levelsCleaned,
                    buildingsCleaned,
                    max_travel_time,
                    affinities,
                    room_literals,
                    rooms_cleaned,
                    earliest_date,
                    roomDistances,
                    _context.HotelStrategy
                );
                // scan through attendant level cleanings to pick off additions to level literals
                foreach (LevelCleaningInterval level_interval in attendant_level_cleanings)
                {
                    string name = $"{level_interval.LevelName}-{level_interval.Flag}";
                    if (level_literals.ContainsKey(name))
                    {
                        level_literals[name].Add(level_interval.Literal);
                    }
                    else
                    {
                        var _list = new List<IntVar> { };
                        _list.Add(level_interval.Literal);
                        level_literals.Add(name, _list);
                    }
                }

                // sometimes, due to exclusivity constraints, etc, an attendant has nothing to do
                if (attendant_building_cleanings.Count() == 0)
                {
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("-------------------------");
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Attendant {attendant.Username} cannot clean any rooms.  Double check exclusivity constraints and floor or building preferences");
                    continue;
                }

                // constrain all of the cleaning intervals for this attendant
                CleaningIntervalConstraints.ConstrainAttendantCleaningIntervals(
                    model, attendant, attendant_interval, attendant_cleanings, rooms_cleaned, roomDistances);

                // constrain all of the level intervals for this attendant
                LevelIntervalConstraints.ConstrainAttendantLevelIntervals(
                    model, attendant, attendant_interval, attendant_level_cleanings, attendant_floor_travel, max_travel_time, levelsCleaned, _context.HotelStrategy);

                // constrain all of the building intervals for this attendant
                BuildingIntervalConstraints.ConstrainAttendantBuildingIntervals(
                    model, attendant, attendant_interval, attendant_building_cleanings, attendant_building_travel, buildingsCleaned, buildDistances, _context.HotelStrategy);

                // finally, try to minimize the gap between attendant
                // start and first cleaning by treating it as a building
                // travel that needs to be minimized. hacky but should
                // work
                var attendantStartDelay = model.NewIntVar(0, max_travel_time, $"atndnt {attendant.Username} start time delay");
                foreach (var buildingInterval in attendant_building_cleanings)
                {
                    model.Add(attendantStartDelay == buildingInterval.FirstLevelStart - attendant_interval.Start).OnlyEnforceIf(buildingInterval.IsFirstLiteral);
                }
                attendantStarts.Add(attendantStartDelay);

                // store this attendant's possible cleaning assignments
                all_cleanings[attendant.Id] = attendant_cleanings;
                Console.WriteLine($"For {attendant.Username} possbile cleanings is size {attendant_cleanings.Count()}");
                all_level_cleanings.Add(attendant.Id, attendant_level_cleanings);
                Console.WriteLine($"For {attendant.Username} possbile level intervals is size {attendant_level_cleanings.Count()}");
                per_attendant_floor_travel.Add(attendant.Id, attendant_floor_travel);
                total_floor_travel.AddRange(attendant_floor_travel);
                per_attendant_room_travel.Add(attendant.Id, attendant_room_travel);
                total_room_travel.AddRange(attendant_room_travel);


                all_building_cleanings.Add(attendant.Id, attendant_building_cleanings);
                Console.WriteLine($"For {attendant.Username} possbile building intervals is size {attendant_building_cleanings.Count()}");
                per_attendant_building_travel.Add(attendant.Id, attendant_building_travel);
                total_building_travel.AddRange(attendant_building_travel);

                Console.WriteLine($"room literals count is {room_literals.Count()}");

            }

            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = "Solver Loop 3: Enforce that rooms are cleaned at most once",
                StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });

            //Loop 2
            // only clean each room once

            // also, correct the average cleaning calculation basedon the rooms that are actually possible to be cleaned, given timing, floor change, and other constraints.
            //
            // also, set up a reward for floors that are fully cleaned
            //
            // sum over all *possible* cleanings...those that can be performed
            int sum_all_cleanings_credits = 0;
            int sum_all_cleanings_rooms = 0;
            // track the cleaning credits of those *Actually*
            // performed in model to compute the averages
            var cleaning_credits = new List<IntVar>();
            var floorCleanings = new Dictionary<string, List<IntVar>>(); // floor levelname is key
            foreach (KeyValuePair<Cleaning, List<IntVar>> kvp in room_literals)
            {
                // the list is a list of IntVar, one true or false
                // value for each attendant who might perform the
                // cleaning.  Summing those up will give the total
                // number of attendants who are assigned to this
                // cleaning.  If all cleanings allow just one
                // attendant, then tell the model this fact.
                var list = kvp.Value;
                var cleaning = kvp.Key;
                var isCleaned = LinearExpr.Sum(list); // zero if not cleaned, one if cleaned
                model.Add(isCleaned <= 1);  // clean room at most once
                // create a bool var to track cleaned state across atendants
                var roomCleaned = model.NewBoolVar($"{cleaning.Room.RoomName} is cleaned");
                model.Add(isCleaned == 1).OnlyEnforceIf(roomCleaned);
                model.Add(isCleaned == 0).OnlyEnforceIf(roomCleaned.Not());
                // save this to the appropriate floor
                var floorname = LevelIntervalConstraints.MakeLevelName(cleaning);
                if (floorCleanings.ContainsKey(floorname))
                {
                    floorCleanings[floorname].Add(roomCleaned);
                }
                else
                {
                    var _list = new List<IntVar> { };
                    _list.Add(roomCleaned);
                    floorCleanings.Add(floorname, _list);
                }
                // Console.WriteLine($"{floorname}, {roomCleaned} vs {isCleaned}");
                var credits = kvp.Key.Credits;
                var CreditVar = model.NewIntVar(0, credits, $"cleaning credit for {kvp.Key.Room.RoomName}");
                model.Add(CreditVar == isCleaned * credits); // will be zero if room is not cleaned
                cleaning_credits.Add(CreditVar);

                // bump the credit summation
                sum_all_cleanings_credits += kvp.Key.Credits;
                sum_all_cleanings_rooms += 1;

            }

            // link floor cleanings to literal
            var floorsCleaned = new List<IntVar>();
            foreach (var kvp in floorCleanings)
            {
                // create an indicator BoolVar that tracks if all the cleanings on this floor are complete
                var cleaningLiterals = kvp.Value;
                var fullyCleaned = model.NewBoolVar($"floor {kvp.Key} is fully cleaned");
                // do the boolean linking dance
                var nots = new List<ILiteral>();
                foreach (var cl in cleaningLiterals)
                {
                    nots.Add(cl.Not());
                }
                nots.Add(fullyCleaned);
                // following along with the docs for creating multiple of booleans
                model.AddBoolOr(nots);
                foreach (var cl in cleaningLiterals)
                {
                    model.AddImplication(fullyCleaned, cl);
                }
                floorsCleaned.Add(fullyCleaned);
            }

            //Loop 2.5
            // limit the number of attendants per level

            if (_context.HotelStrategy.CPSat.limitAttendantsPerLevel)
            {
                // sometimes this is too restrictive.  If total cleanings possible is about the same as total cleanings to do, then allow an extra cleaner per level.
                var average_cleanings_possible = (int)(_context.Cleanings.Count() / primaryAttendants.Count());
                var addOneAttendant =  max_rooms + 2 >= average_cleanings_possible;
                //var addOneAttendant =  true;

                //max_cleanings_possible /  + 10 >= _context.Cleanings.Count()) // || Math.Abs(max_cleanings_possible - _context.Cleanings.Count()) < 10)
                foreach (var kvp in level_literals)
                {
                    var levelName = kvp.Key;
                    // determine the number of cleanins on this level
                    int needed = (int) Math.Ceiling((double)levelCleaningsLookup[levelName].Count / max_rooms);

                    // the following is arbitrary.  Just trying to
                    // handle the case that sometimes when the max
                    // possible is close to actual, then multiple
                    // attendants sometimes have to visit the same
                    // floors...the breakdown of rooms on floors
                    // rarely fits nicely into the exact numbers of
                    // rooms an attendant is allowed to clean.

                    if (addOneAttendant)
                    {
                        if (levelCleaningsLookup[levelName].Count > 3)
                            needed += 1;
                    }
                    Console.WriteLine($"level: {levelName}, cleanings {levelCleaningsLookup[levelName].Count}, needed attendants {needed}");

                    var list = kvp.Value;
                    var is_worked = LinearExpr.Sum(list); // zero if not cleaned, one or more if cleaned
                    model.Add(is_worked <= needed);  // fixed hack to use #attendants needed
                }
            }

            // compute the average credits in model, over the cleanings actually performed
            // get the number of attendants actually assigned work
            var num_att = model.NewIntVar(1, primaryAttendants.Count(), "number attendants assigned work");
            var sum_c = model.NewIntVar(0, sum_all_cleanings_credits, "sum_of_credits_for_all_cleaned_rooms");
            var ave_c = model.NewIntVar(0, sum_all_cleanings_credits, "ave_per_attendant_credits_for_all_cleaned_rooms");

            var sum_r = model.NewIntVar(0, _context.Cleanings.Count(), "sum_of_rooms_for_all_cleaned_rooms");
            var ave_r = model.NewIntVar(0, _context.Cleanings.Count(), "ave_per_attendant_rooms_for_all_cleaned_rooms");

            var sum_d = model.NewIntVar(0, _context.Cleanings.Count(), "sum of all cleaned departures");
            var ave_d = model.NewIntVar(0, _context.Cleanings.Count(), "expected per attendant cleaned departures");

            var sum_s = model.NewIntVar(0, _context.Cleanings.Count(), "sum of all cleaned stays");
            var ave_s = model.NewIntVar(0, _context.Cleanings.Count(), "expected per attendant cleaned stays");

            // use lists to collect each attendant's deviations from ideal
            var epsilons_rooms = new Dictionary<Guid, LinearExpr>();
            var epsilons_credits = new Dictionary<Guid, LinearExpr>();

            var epsilons_departs = new List<LinearExpr>();
            var epsilons_stays = new List<LinearExpr>();

            var worker_maxed_out_rooms = new Dictionary<Guid, IntVar>();
            var worker_maxed_out_credits = new Dictionary<Guid, IntVar>();

            var allAdditionalDepartures = new Dictionary<Guid, IntVar>();
            var allAdditionalStays = new Dictionary<Guid, IntVar>();

            var total_assigned_rooms = 0;
            var total_possible_rooms = _context.Cleanings.Count();
            var not_yet_done_rooms = total_assigned_rooms < total_possible_rooms;

            var total_assigned_credits = 0;
            var total_possible_credits = _context.Cleanings.Sum(c => c.Credits);
            var not_yet_done_credits = total_assigned_credits < total_possible_credits;


            if (target_mode)
            {

                // bugfix.  In some cases, the solver prefers to drop rooms rather
                // than serve them when in targets/credits mode.  for example, if
                // the target credits is 400 (16 rooms at an ave of 25 credits
                // each) and there are 17 rooms to clean, then the final room will
                // produce an epsilon for the attendant of 375 (target is 400,
                // credits performed is 25, so difference is 375).  This means that
                // unless cleaning the room brings a net benefit to the objective
                // function of more than 375, it will better to skip the room and
                // get an epsilon of zero for the attendant because the attendant
                // is unused and taken out of the epsilon competition.
                //
                // so to correct for this, it is necessary to bump up the room
                // cleaning award to at least target credits - credits from one
                // room.  But to make things simple, just bump it up to target
                // credits

                // Console.WriteLine($"constraining ave_c {ave_c}to equal max cleaning time {max_credits}");
                if (max_credits < sum_all_cleanings_credits)
                {
                    model.Add(ave_c == max_credits);
                    var maybe_adjust = max_credits / (max_effort_credits);
                    weight_credits = weight_credits < maybe_adjust ? maybe_adjust : weight_credits;
                    // weight_rooms_cleaned = max_credits;
                    // tried both.  bumping weight_credits seems to get to a solution faster
                }
                else
                {
                    model.Add(ave_c == sum_all_cleanings_credits);
                    var maybe_adjust = sum_all_cleanings_credits / (max_effort_credits);
                    weight_credits = weight_credits < maybe_adjust ? maybe_adjust : weight_credits;
                    // weight_rooms_cleaned = sum_all_cleanings_credits;
                }
                // Console.WriteLine($"constraining ave_r {ave_r}to equal max rooms {max_rooms}");
                if (max_rooms < sum_all_cleanings_rooms)
                    model.Add(ave_r == max_rooms);
                else
                    model.Add(ave_r == sum_all_cleanings_rooms);

            }
            else
            {
                model.AddDivisionEquality(ave_c, sum_c, num_att);
                model.AddDivisionEquality(ave_r, sum_r, num_att);
            }
            // now use ave_c when computing epsilon, below

            if (_context.Cleanings.Count() != room_literals.Count())
            {
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"total cleanings is {_context.Cleanings.Count()}.  Constraints dropped that count to {room_literals.Count()} cleanings that are possible to perform.");
            }
            else
            {
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"total cleanings is {room_literals.Count()}.");
            }

            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = "Solver Loop 4: Verify time is respected (no overlapping cleanings)",
                StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });

            //Loop 3
            // Create disjunctive constraints--no overlap, and unique paths for attendants.
            var hinted_rooms = new List<string>();

            // shuffle the sort of attendants such that the attendants with no affinity come first
            foreach (Attendant attendant in sortedAttendants)
            {
                if (!all_cleanings.ContainsKey(attendant.Id))
                {
                    continue;
                }
                // get all possible cleanings that might be assigned to attendant a
                var attendant_cleanings = all_cleanings[attendant.Id];
                var buildingsCleaned = total_buildings_cleaned[attendant.Id];
                var levelsCleaned = total_levels_cleaned[attendant.Id];

                // get the interval (work period) for the  attendant
                var attendant_interval = attendant_intervals[attendant.Id];
                var attendant_does_clean = attendant_interval.Literal;

                // make flat lists for OR Tools calls

                // for the no overlap constraint...all of the attendant's possible intervals
                var cleanings_intervals = new List<IntervalVar>();

                // this list allows counting up the assigned (true) cleanings
                var cleanings_literals = new List<IntVar>();

                // for the credits scalar product
                var creditss = new List<int>();

                //if this cleaning is one of the preferred buildings
                //var buildings_awards = new List<int>();

                //if this cleaning is one of the preferred levels
                //var levels_awards = new List<int>();

                //if this room is one of the preferred rooms
                var rooms_awards = new List<int>();
                List<IEnumerable<CleaningInterval>> communicating_room_intervals = new List<IEnumerable<CleaningInterval>>();
                // A loop to set up constraints (not for circuit constraint/ordering)
                CleaningIntervalConstraints.SetCommunicatingRooms(model,
                                                                  attendant_cleanings,
                                                                  _context.HotelStrategy.CPSat.CommunicatingRooms,
                                                                  communicating_room_intervals,
                                                                  _context.HotelStrategy);
                allCommunicatingRoomIntervals.Add(attendant.Id, communicating_room_intervals);

                for (int j1 = 0; j1 < attendant_cleanings.Count; ++j1) //j1 is the first job she's doing, j2 is the 2nd job
                {
                    var affinities = new List<Affinity>();
                    if (attendant.CurrentTimeSlot?.Affinities != null && attendant.CurrentTimeSlot.Affinities.Count() > 0)
                    {
                        affinities.AddRange(attendant.CurrentTimeSlot.Affinities);
                        affinities = attendant.CurrentTimeSlot.Affinities;
                    }
                    CleaningInterval this_cleaning_interval = attendant_cleanings[j1];
                    Cleaning this_cleaning = this_cleaning_interval.Cleaning;
                    Room room = this_cleaning.Room;

                    // for the no overlap constraint
                    cleanings_intervals.Add(this_cleaning_interval.Interval);

                    // for total room count, and total cleaning time sum
                    cleanings_literals.Add(this_cleaning_interval.Literal);
                    // now that there might be multiple cleaners (shortening the cleaning times), no longer use this
                    // creditss.Add(this_cleaning.Credits);
                    // instead, use the dur of the cleaning interval
                    creditss.Add(this_cleaning_interval.Credits);

                    rooms_awards.Add(CleaningIntervalConstraints.MakeRoomAward(attendant,
                                                                               affinities,
                                                                               this_cleaning_interval,
                                                                               _context.HotelStrategy.CPSat));
                }
                // now make constraints with those lists

                // hint preferred rooms for this attendant using the rooms_awards list

                // if using pre affinity, don't set the hard constraint on sticking to the preferred floors
                // if (usePreAffinity == true)
                // {
                //     CleaningIntervalConstraints.HintPreferredRooms(model,
                //                                                              attendant,
                //                                                              rooms_awards,
                //                                                              attendant_cleanings,
                //                                                              hinted_rooms,
                //                                                              max_rooms,
                //                                                              max_credits,
                //                                                              0,
                //                                                              _context.HotelStrategy.CPSat);
                // }
                // else
                // {
                //     Functions.CleaningIntervalConstraints.HintPreferredRooms(model,
                //                                                              attendant,
                //                                                              rooms_awards,
                //                                                              attendant_cleanings,
                //                                                              hinted_rooms,
                //                                                              max_rooms,
                //                                                              max_credits,
                //                                                              weight_credits,
                //                                                              _context.HotelStrategy.CPSat);
                // }

                // Other constraints.  Handle these by making
                // variables.  Can store to use as part of objective
                // function


                // ========== Circuit Constraint = Ordering Activities = Travel Time ==========

                // get all possible cleanings that might be assigned to attendant

                // diagnostic write to check amount of time spent processing each attendant
                // Console.WriteLine($"circuit constraint for attendant {attendant.Username}");
                // var attendant_cleanings = all_cleanings[attendant];

                // var travelTime = model.NewIntVar(0, max_travel_time, $"atndnt {attendant.Username}: travel time");
                // // collect for objective
                // total_travel.Add(travelTime);
                // pulled the per-level room ordering into the Make_Level_Intervals fn
                // CleaningOrdering(model, d, attendant, attendant_cleanings, travelTime, LevelChanges);

                // ===== total credits (cleaning time) per attendant =====
                //
                // if it is desired to reduce cleaning time based on
                // the number of level transitions, then do that here
                //
                // need to copy the literals list here to avoid a pass
                // by reference bug, as it is used elsewhere in the
                // model formulation
                var cleanings_literals_copy = new List<IntVar>(cleanings_literals);
                var movements = new List<IntVar>();
                var movement_cost = new List<int>();

                if (level_movement_reduces_credits)
                {
                    var extraLevelsChanges = model.NewIntVar(0, max_number_levels_per_attendant, $"atndnt {attendant.Username}: number of level changes");
                    var shouldPenalize = model.NewBoolVar($"atndt {attendant.Username} exceeds penalty floors");

                    // Implement shouldPenalize == (levelsCleaned > level_movement_threshhold).
                    model.Add(levelsCleaned > level_movement_threshold).OnlyEnforceIf(shouldPenalize);
                    model.Add(levelsCleaned <= level_movement_threshold).OnlyEnforceIf(shouldPenalize.Not());

                    // Create the two half-reified constraints linking the condition with the constraint
                    // First, shouldPenalize implies (extraLevelsChanges == levelsCleaned - threshold).
                    model.Add(extraLevelsChanges == levelsCleaned - level_movement_threshold).OnlyEnforceIf(shouldPenalize);
                    // Second, not(shouldPenalize) implies extraLevelsChanges == 0.
                    model.Add(extraLevelsChanges == 0).OnlyEnforceIf(shouldPenalize.Not());

                    // might be needed? if attendant is not used, no penalty needed
                    // model.Add(extraLevelsChanges == 0).OnlyEnforceIf(attendant_does_clean.Not());

                    // here, extraLevelsChanges is an integer, equal to
                    // the number of level changes over the threshold
                    // value.  When multiplied by the equivalent
                    // credits, it gives the "cost" of that extra
                    // level movement.  Because credits are bound by a
                    // maximum value, adding this extra movement has
                    // the net effect of reducing the total credits
                    // available to the cleaner.

                    // however, the downside is that this approach
                    // skews the credit balancing calculation.
                    // cleanings_literals_copy.Add(numBuildingsChanges);

                    // using a different list
                    movements.Add(extraLevelsChanges);
                    movement_cost.Add(level_movement_equivalent_credits);
                }
                // if it is desired to reduce cleaning time based on
                // the number of building transitions, then do that
                // here
                if (building_movement_reduces_credits)
                {
                    var numBuildingsChanges = model.NewIntVar(0, large_number, $"atndnt {attendant.Username}: number of building changes");
                    model.Add(numBuildingsChanges == buildingsCleaned - 1).OnlyEnforceIf(attendant_does_clean);
                    model.Add(numBuildingsChanges == 0).OnlyEnforceIf(attendant_does_clean.Not());
                    movements.Add(numBuildingsChanges);
                    movement_cost.Add(building_movement_equivalent_credits);
                }

                // add constraint on total cleaning time per attendant
                var totalCredits = LinearExpr.ScalProd(cleanings_literals_copy, creditss);
                // create a variable, ranging from min cleaning time (credits) to max cleaning time (credits), as constraint
                int _max_credits = max_credits;
                int _min_credits = min_credits;
                // some conditions for _max_credits.  If preplan, use that, or if current time slot, use that
                if (_context.HotelStrategy.CPSat.UsePrePlan && !_context.HotelStrategy.CPSat.CompletePrePlan && attendant.Cleanings.Count() > 0)
                    // in this case, cannot get more than the preplanned credits, so set max credits
                    _max_credits = attendant.Cleanings.Sum(c => c.Credits);
                else if (_context.HotelStrategy.CPSat.UsePrePlan && attendant.Cleanings.Count()>0)
                {
                    // a few possible special cases to test here, but need a summation temporary variable
                    var assigned_jobs_credits = attendant.Cleanings.Sum(c => c.Credits);
                    if (assigned_jobs_credits > _max_credits)
                        // more jobs assigned than "allowed", so extend max credits bound
                        _max_credits = assigned_jobs_credits;
                    else if (attendant.CurrentTimeSlot.MaxCredits > 0 && assigned_jobs_credits >  attendant.CurrentTimeSlot.MaxCredits)
                        // more jobs assigned than the current time slot limit, so extend max credits bound
                        _max_credits = assigned_jobs_credits;
                    else if (attendant.CurrentTimeSlot.MaxCredits > 0)
                        // here even though in preplan mode, the current time slot max credits is the upper bound
                        _max_credits = attendant.CurrentTimeSlot.MaxCredits;
                }
                else if (attendant.CurrentTimeSlot.MaxCredits != 0)
                    // in this case, not preplanned, but is the case that the attendant has a particular max credits assigned
                    _max_credits = attendant.CurrentTimeSlot.MaxCredits;

                // conditions for _min_credits.  If exclusive affinity and attendant has affinity, set _min_credits to zero
                if (_context.HotelStrategy.CPSat.preferredLevelsAreExclusive &&
                    attendant.CurrentTimeSlot?.Affinities != null &&
                    attendant.CurrentTimeSlot.Affinities.Count() >= 0)
                    //attendant.CurrentTimeSlot?.Affinity?.Levels != null)
                    _min_credits = 0;
                // create a cleaning credits variable that is bounded.
                var creditsBounded = model.NewIntVar(_min_credits, _max_credits, $"atndnt {attendant.Username}: cleaning time");

                // this is useful for reporting
                //
                // extract the possible departs and stays
                var attendant_possible_stays = all_cleanings[attendant.Id]
                    .Where(ci => ci.Cleaning.Type == CleaningType.Stay)
                    .Select(ci => ci.Literal).ToArray();
                var stays_cleaned = model.NewIntVar(0, attendant_possible_stays.Count(), $"atndnt {attendant.Username} count of stays cleaned");
                model.Add(stays_cleaned == LinearExpr.Sum(attendant_possible_stays));
                // constrain if needed
                if (attendant.CurrentTimeSlot.MaxStays > 0)
                    model.Add(stays_cleaned <= attendant.CurrentTimeSlot.MaxStays);
                else if (max_stays > 0)
                    model.Add(stays_cleaned <= max_stays);
                attendant_stays_cleaned.Add(attendant.Id, stays_cleaned);

                var attendant_possible_departs = all_cleanings[attendant.Id]
                    .Where(ci => ci.Cleaning.Type == CleaningType.Departure)
                    .Select(ci => ci.Literal);
                var departsMaximumLimit = attendant_possible_departs.Count();
                if (attendant.CurrentTimeSlot.MaxDepartures > 0)
                    departsMaximumLimit = attendant.CurrentTimeSlot.MaxDepartures;
                else if (max_departs > 0)
                    departsMaximumLimit = max_departs;

                var departs_cleaned = model.NewIntVar(0, departsMaximumLimit, $"atndnt {attendant.Username} count of departures cleaned");
                // constrain if needed

                model.Add(departs_cleaned == LinearExpr.Sum(attendant_possible_departs));
                attendant_departs_cleaned.Add(attendant.Id, departs_cleaned);

                // Next handle balancing stays and departs, if desired
                if (balance_stay_depart_mode)
                {
                    // Departures
                    if (attendant.CurrentTimeSlot.MaxDepartures > 0)
                    {
                        var epsilonAbs = model.NewIntVar(0, attendant.CurrentTimeSlot.MaxDepartures, $"abs.val. epsilon departures {attendant.Username}");
                        var epsilon = model.NewIntVar(0, attendant.CurrentTimeSlot.MaxDepartures, $"epsilon departures {attendant.Username}");
                        var epsilon_expression = attendant.CurrentTimeSlot.MaxDepartures - departs_cleaned;
                        model.Add(epsilon == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                        model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                        model.AddAbsEquality(epsilonAbs, epsilon);
                        epsilons_departs.Add(epsilonAbs);
                        // replacing
                        // model.Add(departs_cleaned <= attendant.CurrentTimeSlot.MaxDepartures + epsilonCleaningDeparts).OnlyEnforceIf(attendant_does_clean);
                    }
                    else
                    {
                        var epsilonAbs = model.NewIntVar(0, sum_all_cleanings_rooms, $"abs. val. epsilon departures {attendant.Username}");
                        var epsilon = model.NewIntVar(-sum_all_cleanings_rooms, sum_all_cleanings_rooms, $"epsilon departures {attendant.Username}");
                        var epsilon_expression = departs_cleaned - ave_d;
                        model.Add(epsilon == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                        model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                        model.AddAbsEquality(epsilonAbs, epsilon);
                        epsilons_departs.Add(epsilonAbs);
                        // replacing this
                        // model.Add(departs_cleaned <= ave_d + epsilonCleaningDeparts).OnlyEnforceIf(attendant_does_clean);
                        // model.Add(departs_cleaned >= ave_d - epsilonCleaningDeparts).OnlyEnforceIf(attendant_does_clean);
                    }
                    // Stays
                    if (attendant.CurrentTimeSlot.MaxStays > 0)
                    {
                        var epsilonAbs = model.NewIntVar(0, attendant.CurrentTimeSlot.MaxStays, $"abs. val. epsilon stays {attendant.Username}");
                        var epsilon = model.NewIntVar(0, attendant.CurrentTimeSlot.MaxStays, $"epsilon stays {attendant.Username}");
                        var epsilon_expression = attendant.CurrentTimeSlot.MaxStays - stays_cleaned;
                        model.Add(epsilon == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                        model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                        model.AddAbsEquality(epsilonAbs, epsilon);
                        epsilons_stays.Add(epsilonAbs);
                        // replacing
                        // model.Add(stays_cleaned <= attendant.CurrentTimeSlot.MaxStays + epsilonCleaningStays).OnlyEnforceIf(attendant_does_clean);
                    }
                    else
                    {
                        var epsilonAbs = model.NewIntVar(0, sum_all_cleanings_rooms, $"abs. val. epsilon stays {attendant.Username}");
                        var epsilon = model.NewIntVar(-sum_all_cleanings_rooms, sum_all_cleanings_rooms, $"epsilon stays {attendant.Username}");
                        var epsilon_expression = stays_cleaned - ave_s;
                        model.Add(epsilon == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                        model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                        model.AddAbsEquality(epsilonAbs, epsilon);
                        epsilons_stays.Add(epsilonAbs);
                        // replacing this
                        // model.Add(stays_cleaned <= ave_s + epsilonCleaningStays).OnlyEnforceIf(attendant_does_clean);
                        // model.Add(stays_cleaned >= ave_s - epsilonCleaningStays).OnlyEnforceIf(attendant_does_clean);
                    }
                }

                if (maxDeparturesReducesCredits)
                {
                    var shouldPenalizeDepartures = model.NewBoolVar($"atndt {attendant.Username} exceeds threshold departures");
                    var additionalDeparturesCleaned = model.NewIntVar(0, departsMaximumLimit, $"atndnt {attendant.Username}: additional departures cleaned above threshold");
                    model.Add(departs_cleaned > maxDeparturesThreshold).OnlyEnforceIf(shouldPenalizeDepartures);
                    model.Add(departs_cleaned <= maxDeparturesThreshold).OnlyEnforceIf(shouldPenalizeDepartures.Not());
                    model.Add(additionalDeparturesCleaned == departs_cleaned - maxDeparturesThreshold).OnlyEnforceIf(shouldPenalizeDepartures);
                    model.Add(additionalDeparturesCleaned == 0).OnlyEnforceIf(shouldPenalizeDepartures.Not());
                    model.Add(shouldPenalizeDepartures == 0).OnlyEnforceIf(attendant_does_clean.Not());

                    // Borrow the lists made for movement reducing credits, because it is simpler that way
                    Console.WriteLine($"{attendant.Username} setting up max departure reduces credits with {additionalDeparturesCleaned} and {departs_cleaned} vs threshhold {maxDeparturesThreshold} and ");
                    movements.Add(additionalDeparturesCleaned);
                    movement_cost.Add(maxDeparturesEquivalentCredits);
                    allAdditionalDepartures.Add(attendant.Id, additionalDeparturesCleaned);
                }


                if (maxStaysIncreasesCredits)
                {
                    var shouldRewardStays = model.NewBoolVar($"atndt {attendant.Username} exceeds threshold stays");
                    var additionalStaysCleaned = model.NewIntVar(0, attendant_possible_stays.Count(), $"atndnt {attendant.Username}: additional stays cleaned above threshold");
                    model.Add(stays_cleaned > maxStaysThreshold).OnlyEnforceIf(shouldRewardStays);
                    model.Add(stays_cleaned <= maxStaysThreshold).OnlyEnforceIf(shouldRewardStays.Not());
                    model.Add(additionalStaysCleaned == stays_cleaned - maxStaysThreshold).OnlyEnforceIf(shouldRewardStays);
                    model.Add(additionalStaysCleaned == 0).OnlyEnforceIf(shouldRewardStays.Not());
                    model.Add(shouldRewardStays == 0).OnlyEnforceIf(attendant_does_clean.Not());

                    // Borrow the lists made for movement increasing credits, because it is simpler that way
                    Console.WriteLine($"{attendant.Username} setting up max stay increases credits with {additionalStaysCleaned} and {stays_cleaned} vs threshhold {maxStaysThreshold} and ");
                    movements.Add(additionalStaysCleaned);
                    movement_cost.Add(-maxStaysEquivalentCredits);
                    allAdditionalStays.Add(attendant.Id, additionalStaysCleaned);
                }

                // maybe reduce max due to level or bldg movement
                if (movements.Count() > 0)
                {
                    Console.WriteLine($"use {movements.Count()} penalty movements to reduce allowed cleaning time {creditsBounded}");
                    var credit_reductions = model.NewIntVar(0, max_credits, $"total credit reductions due to movement, etc for {attendant.Username}");
                    model.Add(credit_reductions ==  LinearExpr.ScalProd(movements, movement_cost));
                    model.Add(creditsBounded <= max_credits - credit_reductions);
                    total_credit_reductions.Add(attendant.Id, credit_reductions);
                }
                model.Add(totalCredits == creditsBounded);
                // save the IntVar for for use in objective function
                total_credits.Add(attendant.Id, creditsBounded);

                // ===== total room reward =====
                // total room reward is scalar product of literals and potential awards
                var totalRoomReward = LinearExpr.ScalProd(cleanings_literals, rooms_awards);
                // total room reward is unconstrained, but make an int var to store the summation
                var RoomReward = model.NewIntVar(0, large_number, $"atndnt {attendant.Username}: room reward");
                var neg_rewards = rooms_awards.Where(a => a < 0);
                if (neg_rewards.Count() > 0)
                {
                    // Console.WriteLine($"might have a negative reward for  {attendant.Username}");
                    RoomReward = model.NewIntVar(-large_number, large_number, $"atndnt {attendant.Username}: room reward");
                }
                // set the constraint (and the equality)
                //
                model.Add(totalRoomReward == RoomReward);
                // save the IntVar for use in the objective fn
                total_rooms_award.Add(attendant.Id, RoomReward);

            }
            // reset the timeslot if necessary

            // if (usePreAffinity == true)
            // {
            //     Console.WriteLine("WARNING: usePreAffinity case is no longer tested");
            //     // usePreAffinity is false  if either of these were non null coming into this  from the UI
            //     var haveAffinity = primaryAttendants.Where(a => a.CurrentTimeSlot?.Affinities?.Count() != null );
            //     foreach (var a in haveAffinity)
            //     {
            //         a.CurrentTimeSlot.Affinity.Levels = null;
            //         a.CurrentTimeSlot.Affinity.Buildings = null;
            //     }
            // }


            //-------------------------------------------------------------------------------------------------
            // Create a linear sum of all variables times weights
            var all_room_travel = LinearExpr.Sum(total_room_travel);
            var all_floor_travel = LinearExpr.Sum(total_floor_travel);
            var all_building_travel = LinearExpr.Sum(total_building_travel);
            var all_ends = LinearExpr.Sum(total_assigned.Values); ///number of assigned rooms
            var all_deviations = LinearExpr.Sum(total_deviations.Values);
            var all_credits = LinearExpr.Sum(total_credits.Values);
            //var all_building_awards = LinearExpr.Sum(total_buildings_award);
            //var all_level_awards = LinearExpr.Sum(total_levels_award);
            var all_room_awards = LinearExpr.Sum(total_rooms_award.Values);

            var level_change_count = new List<IntVar>();
            if (weight_level_changes < 0)
            {

                foreach (var kvp in total_levels_cleaned)
                {

                    // attendant level changes
                    var attendantId = kvp.Key;
                    var attendant = primaryAttendants.First(a => a.Id == attendantId);

                    var levelchange = model.NewIntVar(0, max_number_levels_per_attendant, $"{attendant.Username} level changes");
                    var isusedLit = attendant_literals[attendant.Id];
                    var levelsCleaned = kvp.Value;

                    var changeLevels = model.NewBoolVar($"atndt {attendant.Username} changes levels at least once");

                    // special case for communicating rooms.  Do not
                    // penalize level changes due to serving communicating
                    // rooms

                    // FIXME how to do that?
                    // check this attendant's communicating room literals

                    var communicating_room_intervals = (IEnumerable<IEnumerable<CleaningInterval>>) new CleaningInterval[0][];
                    if (allCommunicatingRoomIntervals.ContainsKey(attendant.Id))
                    {
                        communicating_room_intervals = allCommunicatingRoomIntervals[attendant.Id];
                    }

                    var levelSpanning = new List<IEnumerable<CleaningInterval>>();
                    if (communicating_room_intervals.Count() > 0)
                    {
                        // check if any of the communicating room intervals span across levels
                        foreach (var linked_intervals in communicating_room_intervals)
                        {
                            var levelList = linked_intervals
                                .Select(i => i.Cleaning.Room.Floor.LevelName)
                                .Distinct();
                            if (levelList.Count() > 1)
                            {
                                levelSpanning.Add(linked_intervals);
                            }
                        }
                    }

                    if (levelSpanning.Count() > 0)
                    {
                        // subtract from the penalized level change count any changes due to communicating rooms on different levels.  Those should be free

                        // tricky logic, but not that hard.  For each of
                        // the lists of communicating cleaning intervals,
                        // if any of the literals in the list is true,
                        // then all of them are true.  So by collecting
                        // just the first literal of each group, I know
                        // whether all of the intervals are visited or
                        // none.
                        //
                        // then the number of level changes in each group
                        // is also collected, and multiplied by the
                        // boolean (0,1) to get the total number of level
                        // changes that are done in the service of
                        // communicating cleanings

                        var literal_list = new List<IntVar>();
                        var floor_change_count = new List<int>();
                        foreach (var linked_intervals in levelSpanning)
                        {
                            var floorCount = linked_intervals
                                .Select(i => i.Cleaning.Room.Floor.LevelName)
                                .Distinct()
                                .Count();
                            literal_list.Add(linked_intervals.First().Literal);
                            floor_change_count.Add(floorCount);
                        }
                        // now create channeling constraints

                        var totalFreeFloorChanges = LinearExpr.ScalProd(literal_list, floor_change_count);

                        model.Add(levelsCleaned > totalFreeFloorChanges + 1).OnlyEnforceIf(changeLevels);
                        model.Add(levelsCleaned <= totalFreeFloorChanges + 1).OnlyEnforceIf(changeLevels.Not());

                        model.Add(levelchange == ((levelsCleaned - totalFreeFloorChanges)*2)).OnlyEnforceIf(changeLevels);
                        model.Add(levelchange == 0).OnlyEnforceIf(changeLevels.Not());
                    }
                    else
                    {

                        model.Add(levelsCleaned > 1).OnlyEnforceIf(changeLevels);
                        model.Add(levelsCleaned <= 1).OnlyEnforceIf(changeLevels.Not());

                        //model.Add(levelchange == kvp.Value - 1).OnlyEnforceIf(isusedLit);
                        // try just level change == level count if levels is non zero

                        model.Add(levelchange == levelsCleaned).OnlyEnforceIf(changeLevels);
                        model.Add(levelchange == 0).OnlyEnforceIf(changeLevels.Not());
                    }
                    level_change_count.Add(levelchange);
                }
            }
            else
            {
                level_change_count.Add(model.NewConstant(0,"do not track level changes"));
            }
            level_change_count.ForEach(l => Console.WriteLine(l));
            var all_level_changes = LinearExpr.Sum(level_change_count);


            var summed_attendants = LinearExpr.Sum(attendant_literals.Values);

            // now going to add deviation score for cleaning time

            // The deviation of the sum of cleaner's cleaning time,
            // versus the expected average cleaning time
            //
            bool skip_solver = false;
            if (_context.HotelStrategy.CPSat.UsePrePlan && !_context.HotelStrategy.CPSat.CompletePrePlan)
            {
                // in this case, do not use the pre-plan attendants as
                // part of the balancing attendants
                var preplanned_attendants = primaryAttendants.Where(a => a.Cleanings.Count() > 0);
                var preplanned_attendants_count = preplanned_attendants.Count();
                if (preplanned_attendants_count == primaryAttendants.Count())
                    skip_solver = true;

                var preplanned_rooms = preplanned_attendants.Sum(a => a.Cleanings.Count());
                var preplanned_credits = preplanned_attendants.Sum(a => a.Cleanings.Sum(c => c.Credits));
                // Console.WriteLine($"preplanned rooms is {preplanned_rooms}, preplanned credits is {preplanned_credits}");
                summed_attendants = LinearExpr.Sum(attendant_literals.Values) - preplanned_attendants_count;
                all_ends = LinearExpr.Sum(total_assigned.Values) - preplanned_rooms;
                all_credits = LinearExpr.Sum(total_credits.Values) - preplanned_credits;
            }

            model.Add(all_ends == sum_r);
            model.Add(all_credits == sum_c);
            model.Add(summed_attendants == num_att);


            // also add sum for departs, stays
            // ugh  this is gross and needs refactoring
            //
            if (balance_stay_depart_mode)
            {
                var all_possible_cleaned_stays = new List<IntVar>();
                var all_possible_cleaned_departs = new List<IntVar>();
                foreach (var attendant in primaryAttendants)
                {
                    var attendant_possible_stays = all_cleanings[attendant.Id]
                        .Where(ci => ci.Cleaning.Type == CleaningType.Stay)
                        .Select(ci => ci.Literal);
                    all_possible_cleaned_stays.AddRange(attendant_possible_stays);

                    var attendant_possible_departs = all_cleanings[attendant.Id]
                        .Where(ci => ci.Cleaning.Type == CleaningType.Departure)
                        .Select(ci => ci.Literal);
                    all_possible_cleaned_departs.AddRange(attendant_possible_departs);
                }
                model.Add(sum_d == LinearExpr.Sum(all_possible_cleaned_departs));
                var total_departures = _context.Cleanings.Where(c => c.Type == CleaningType.Departure).Count();
                if (target_mode && max_departs > 0)
                {
                    if (max_departs < total_departures)
                        model.Add(ave_d == max_departs);
                    else
                        model.Add(ave_d == total_departures);
                }
                else
                {
                    // this doesn't work very well
                    // model.AddDivisionEquality(ave_d, sum_d, num_att);
                    model.Add(ave_d == total_departures / primaryAttendants.Count());
                }
                model.Add(sum_s == LinearExpr.Sum(all_possible_cleaned_stays));
                if (target_mode && max_stays > 0)
                {
                    if (max_stays < _context.Cleanings.Count())
                        model.Add(ave_s == max_stays);
                    else
                        model.Add(ave_s == _context.Cleanings.Count());
                }
                else
                {
                    model.AddDivisionEquality(ave_s, sum_s, num_att);
                }
            }


            var sumEpsilonRoomsSq = model.NewIntVar(0, large_number, "sum of epsilon rooms squared");
            var sumEpsilonCreditsSq = model.NewIntVar(0, large_number, "sum of epsilon credits squared");
            var sumEpsilonDepartsSq = model.NewIntVar(0, large_number, "sum of epsilon departs squared");
            var sumEpsilonStaysSq = model.NewIntVar(0, large_number, "sum of epsilon stays squared");


            // iterate over each attendant, in order to pick expected credits or max credits
            foreach (var attendant in primaryAttendants)
            {
                if (!all_cleanings.ContainsKey(attendant.Id))
                {
                    continue;
                }
                var this_assigned_rooms = 0;
                var this_assigned_credits = 0;
                var attendant_index = attendantLookupIndex[attendant.Id];
                var prior_attendant_index = attendant_index - 1;
                Attendant priorAttendant = null;
                if (prior_attendant_index >= 0)
                {
                    var attendantId = attendantLookupIndex.Where(kvp => kvp.Value == prior_attendant_index).Select(kvp => kvp.Key).First();
                    priorAttendant = primaryAttendants.First(a => a.Id == attendantId);
                    // Console.WriteLine($"attendant {attendant.Username} comes after {priorAttendant.Username}");
                }
                var attendant_credits = total_credits[attendant.Id];
                var attendant_cleaning_rooms = total_assigned[attendant.Id];

                // commented this out because can't do it anymore
                // but potentially, if an attendant has a short work time, might screw up the epsilon calc, and skew the balancing effort
                //
                // var desired_credits = expected_credits_per_attendant;
                // if( attendant.CurrentTimeSlot.MaxCredits > 0 && attendant.CurrentTimeSlot.MaxCredits > desired_credits )
                // {
                //     // desired_credits = attendant.CurrentTimeSlot.MaxCredits;
                var attendant_interval = attendant_intervals[attendant.Id];
                var attendant_does_clean = attendant_interval.Literal;

                // first handle ROOMS
                // NOTE ave_r is computed by model.  The variable is created above, after Loop2

                if (_context.HotelStrategy.CPSat.UsePrePlan && !_context.HotelStrategy.CPSat.CompletePrePlan && attendant.Cleanings.Count()>0)
                {
                    this_assigned_rooms = attendant.Cleanings.Count();
                    //We remove this cleaner from the competition
                    // maybe just do
                    // var epsilonAbs = model.NewConstant(0, $"abs. val. epsilon rooms {attendant.Username} is zero in preplan, no-complete case");
                    var epsilonAbs = model.NewIntVar(0, sum_all_cleanings_rooms, $"abs. val. epsilon rooms {attendant.Username} (preplan case)");
                    // var epsilon = model.NewIntVar(0, sum_all_cleanings_rooms, $"epsilon rooms {attendant.Username}");
                    if (weight_epsilon_rooms != 0)
                    {
                        var epsilon_expression = attendant.Cleanings.Count() - attendant_cleaning_rooms;
                        if (target_mode_minimize_attendants)
                            model.Add(epsilonAbs == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                        else
                            model.Add(epsilonAbs == epsilon_expression);
                        // model.Add(epsilonAbs == 0).OnlyEnforceIf(attendant_does_clean.Not());
                        // model.AddAbsEquality(epsilonAbs, epsilon);
                    }
                    epsilons_rooms.Add(attendant.Id, epsilonAbs);
                }
                else
                {

                    if (attendant.CurrentTimeSlot.noOfRooms > 0)
                    {
                        this_assigned_rooms = attendant.CurrentTimeSlot.noOfRooms;
                        //We remove this cleaner from the competition
                        // for now, get rid of the OnlyEnforceIf(attendant_does_clean) because it isn't used and is over complicating things
                        var epsilonAbs = model.NewIntVar(0, sum_all_cleanings_rooms, $"abs. val. epsilon rooms {attendant.Username} (timeslot has noOfRooms case)");
                        // var epsilon = model.NewIntVar(0, sum_all_cleanings_rooms, $"epsilon rooms {attendant.Username}");
                        if (weight_epsilon_rooms != 0)
                        {
                            var epsilon_expression = attendant.CurrentTimeSlot.noOfRooms - attendant_cleaning_rooms;
                            if (target_mode_minimize_attendants)
                                model.Add(epsilonAbs == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                            else
                                model.Add(epsilonAbs == epsilon_expression);
                            //model.Add(epsilonAbs == 0).OnlyEnforceIf(attendant_does_clean.Not());
                            // model.AddAbsEquality(epsilonAbs, epsilon);
                        }
                        epsilons_rooms.Add(attendant.Id, epsilonAbs);
                        // removing this
                        // model.Add(attendant_cleaning_rooms <= attendant.CurrentTimeSlot.noOfRooms + epsilonCleaningRooms).OnlyEnforceIf(attendant_does_clean);
                    }
                    else
                    {
                        if (weight_epsilon_rooms != 0)
                        {
                            this_assigned_rooms = max_rooms;
                            var epsilonAbs = model.NewIntVar(0, sum_all_cleanings_rooms, $"abs. val. epsilon rooms {attendant.Username}");
                            var epsilon = model.NewIntVar(-sum_all_cleanings_rooms, sum_all_cleanings_rooms, $"epsilon rooms {attendant.Username}");
                            var epsilon_expression = attendant_cleaning_rooms - ave_r;
                            if (target_mode_minimize_attendants)
                                model.Add(epsilon == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                            else
                                model.Add(epsilon == epsilon_expression);
                            //model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                            model.AddAbsEquality(epsilonAbs, epsilon);
                            epsilons_rooms.Add(attendant.Id, epsilonAbs);
                        }
                        else
                        {
                            // insert a dummy value
                            this_assigned_rooms = 0;
                            epsilons_rooms.Add(attendant.Id, model.NewConstant(0, $"abs. val. epsilon rooms {attendant.Username} is fixed to zero"));
                        }
                        // replacing this
                        // model.Add(attendant_cleaning_rooms <= ave_r + epsilonCleaningRooms).OnlyEnforceIf(attendant_does_clean);
                        // model.Add(attendant_cleaning_rooms >= ave_r - epsilonCleaningRooms).OnlyEnforceIf(attendant_does_clean);
                    }
                }

                // set max effort from room count constraints in target mode
                if (target_mode)
                {
                    var maximum_effort = model.NewBoolVar($"attendant {attendant.Username} does maximum rooms possible");
                    var variance = epsilons_rooms[attendant.Id];
                    // Console.WriteLine($"Attendant {attendant.Username} variance is {variance}");
                    model.Add(variance == 0).OnlyEnforceIf(maximum_effort);
                    //model.Add(variance > 0).OnlyEnforceIf(maximum_effort.Not());
                    model.AddImplication(maximum_effort, attendant_does_clean);

                    worker_maxed_out_rooms.Add(attendant.Id, maximum_effort);
                    // try to max out effort in order
                    if (attendant_index > 0 && weight_epsilon_rooms != 0)
                    {
                        // model.AddImplication(maximum_effort, worker_maxed_out_rooms[attendant_index-1]);
                        // Console.WriteLine($"this attendant rooms variance is {variance}, prior attendant variance is {epsilons_rooms[priorAttendant]}");
                        if (target_mode_minimize_attendants)
                        {
                            // Console.WriteLine($"{attendant_does_clean} implies {worker_maxed_out_rooms[priorAttendant]}");
                            model.AddImplication(attendant_does_clean,
                                                 worker_maxed_out_rooms[priorAttendant.Id]);
                            // note that it is not always true that if
                            // prior worker is maxed out then the
                            // current worker does clean, because it
                            // could be thet case that the work
                            // divides equally into the workers and
                            // the prior worker maxing out means that
                            // there is no more work to do.  So cannot
                            // use a hard equals constraint here.

                            // but it is needed.
                            if (not_yet_done_rooms)
                            {
                                model.AddImplication(worker_maxed_out_rooms[priorAttendant.Id],
                                                     attendant_does_clean);
                            }
                        }
                        // not sure about this one
                        // model.Add(variance >= epsilons_rooms[attendant_index - 1]).OnlyEnforceIf(attendant_does_clean);
                    }
                }
                // track the max possible assigned to this attendant
                total_assigned_rooms += this_assigned_rooms;
                // update the flag for the next iteration
                not_yet_done_rooms = total_assigned_rooms < total_possible_rooms;


                // now handle CREDITS
                // NOTE ave_c is computed by model.  The variable is created above, after Loop2
                if (_context.HotelStrategy.CPSat.UsePrePlan && !_context.HotelStrategy.CPSat.CompletePrePlan && attendant.Cleanings.Count() > 0)
                {
                    this_assigned_credits = attendant.Cleanings.Sum(c => c.Credits);

                    //We remove this cleaner from the competition
                    var epsilonAbs = model.NewConstant(0, $"abs. val. epsilon credits {attendant.Username} is zero in preplan no-complete case)");
                    // explanation: the sum of attendant.Cleanings
                    // credits is the *UI assigned* credits the
                    // attendant_credits is the *solver
                    // assigned* credits These must be identical in
                    // this case (because the solver is not allowed to
                    // "complete" the preplan, so epsilonAbs must be
                    // zero

                    // model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                    // model.AddAbsEquality(epsilonAbs, epsilon);
                    epsilons_credits.Add(attendant.Id, epsilonAbs);
                }
                else
                {

                    if (attendant.CurrentTimeSlot.MaxCredits > 0)
                    {
                        // Console.WriteLine($"{attendant.Username} has timeslot with max credits set to {attendant.CurrentTimeSlot.MaxCredits}");
                        var epsilonAbs = model.NewIntVar(0, sum_all_cleanings_credits, $"abs. val. epsilon credits {attendant.Username} (timeslot has MaxCredits case)");
                        // var epsilon = model.NewIntVar(-sum_all_cleanings_credits, sum_all_cleanings_credits, $"epsilon credits {attendant.Username}");
                        if ( weight_epsilon_credits != 0)
                        {
                            this_assigned_credits = max_effort_credits;

                            var epsilon_expression = attendant.CurrentTimeSlot.MaxCredits - attendant_credits;
                            if (target_mode_minimize_attendants)
                                model.Add(epsilonAbs == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                            else
                                model.Add(epsilonAbs == epsilon_expression);
                            // model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                            // model.AddAbsEquality(epsilonAbs, epsilon);
                            epsilons_credits.Add(attendant.Id, epsilonAbs);
                        }
                        else
                        {
                            // insert a dummy value
                            this_assigned_credits = 0;
                            epsilons_credits.Add(attendant.Id, model.NewConstant(0, $"abs. val. epsilon credits {attendant.Username} is fixed to zero"));
                        }
                        // replacing
                        // model.Add(attendant_credits <= attendant.CurrentTimeSlot.MaxCredits + epsilonCredits).OnlyEnforceIf(attendant_does_clean);
                    }
                    // sometimes possible to have room limit assigned, but not a credit limit
                    else if (attendant.CurrentTimeSlot.noOfRooms > 0)
                    {
                        // Console.WriteLine($"{attendant.Username} has timeslot with max credits set to {attendant.CurrentTimeSlot.MaxCredits}");
                        if ( weight_epsilon_credits != 0)
                        {
                            this_assigned_credits = max_effort_credits;

                            var epsilonAbs = model.NewIntVar(0, sum_all_cleanings_credits, $"abs. val. epsilon credits {attendant.Username} (timeslot has max noOfRooms but nothing for MaxCredits case)");
                            var epsilon_expression = (max_effort_credits+1) * (attendant.CurrentTimeSlot.noOfRooms - attendant_cleaning_rooms);
                            if (target_mode_minimize_attendants)
                                model.Add(epsilonAbs == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                            else
                                model.Add(epsilonAbs == epsilon_expression);
                            // model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                            // model.AddAbsEquality(epsilonAbs, epsilon);
                            epsilons_credits.Add(attendant.Id, epsilonAbs);
                        }
                        else
                        {
                            // insert a dummy value
                            this_assigned_credits = 0;
                            epsilons_credits.Add(attendant.Id, model.NewConstant(0, $"abs. val. epsilon credits {attendant.Username} is fixed to zero"));
                        }
                        // replacing
                        // model.Add(attendant_credits <= attendant.CurrentTimeSlot.MaxCredits + epsilonCredits).OnlyEnforceIf(attendant_does_clean);
                    }
                    else
                    {
                        if ( weight_epsilon_credits != 0)
                        {
                            this_assigned_credits = max_credits;
                            var epsilonAbs = model.NewIntVar(0, sum_all_cleanings_credits, $"abs. val. epsilon credits {attendant.Username}");
                            // Console.WriteLine($"handle credits the usual way, aiming for {ave_c}");
                            var epsilon = model.NewIntVar(-sum_all_cleanings_credits, sum_all_cleanings_credits, $"epsilon credits {attendant.Username}");
                            var epsilon_expression = attendant_credits - ave_c;
                            if (target_mode_minimize_attendants)
                                model.Add(epsilon == epsilon_expression).OnlyEnforceIf(attendant_does_clean);
                            else
                                model.Add(epsilon == epsilon_expression);
                            // model.Add(epsilon == 0).OnlyEnforceIf(attendant_does_clean.Not());
                            model.AddAbsEquality(epsilonAbs, epsilon);
                            epsilons_credits.Add(attendant.Id, epsilonAbs);
                        }
                        else
                        {
                            // insert a dummy value
                            this_assigned_credits = 0;
                            epsilons_credits.Add(attendant.Id, model.NewConstant(0, $"abs. val. epsilon credits {attendant.Username} is fixed to zero"));
                        }
                        // replacing
                        // model.Add(attendant_credits <= ave_c + epsilonCredits).OnlyEnforceIf(attendant_does_clean);
                        // model.Add(attendant_credits >= ave_c - epsilonCredits).OnlyEnforceIf(attendant_does_clean);
                    }
                }

                // set max effort from credits constraints in target mode
                if (target_mode)
                {
                    var maximum_effort = model.NewBoolVar($"attendant {attendant.Username} does maximum credits possible");
                    var variance = epsilons_credits[attendant.Id];

                    //For target mode of Credits, needs to change.
                    model.Add(variance <= max_effort_credits).OnlyEnforceIf(maximum_effort); //should probably be the mean cleaning time
                    // model.Add(variance > max_effort_credits).OnlyEnforceIf(new [] {attendant_does_clean, maximum_effort.Not()});
                    model.AddImplication(maximum_effort, attendant_does_clean);
                    model.AddImplication(attendant_does_clean.Not(), maximum_effort.Not());

                    worker_maxed_out_credits.Add(attendant.Id, maximum_effort);
                    if (attendant_index > 0 && weight_epsilon_credits != 0)
                    {
                        // model.AddImplication(maximum_effort, worker_maxed_out_credits[attendant_index-1]);
                        // Console.WriteLine($"credits variance is {variance}, prior is {epsilons_credits[attendant_index - 1]}");
                        if (target_mode_minimize_attendants)
                        {
                            // Console.WriteLine($"{attendant_does_clean} implies {worker_maxed_out_credits[priorAttendant]}");
                            model.AddImplication(attendant_does_clean,
                                                 worker_maxed_out_credits[priorAttendant.Id]);
                            if (not_yet_done_credits)
                            {
                                model.AddImplication(worker_maxed_out_rooms[priorAttendant.Id],
                                                     attendant_does_clean);
                            }

                        }
                        // model.Add(variance >= epsilons_credits[attendant_index - 1]).OnlyEnforceIf(attendant_does_clean);
                    }
                }

                // track the max possible assigned to this attendant
                total_assigned_credits += this_assigned_credits;
                // update the flag for the next iteration
                not_yet_done_credits = total_assigned_credits < total_possible_credits;


                // }else{
                //     Console.WriteLine($"attendant {attendant.Username} has max credits of {attendant.CurrentTimeSlot.MaxCredits} versus target average expected cleaning time of {expected_credits_per_attendant}, so is not included in epsilon constraint calculations");
                // }
            }

            // for all the different epsilon values, compute the sum of absolute values here for the objective function to minimize

            model.Add(sumEpsilonRoomsSq == LinearExpr.Sum(epsilons_rooms.Values));
            model.Add(sumEpsilonCreditsSq == LinearExpr.Sum(epsilons_credits.Values));

            model.Add(sumEpsilonDepartsSq == LinearExpr.Sum(epsilons_departs));
            model.Add(sumEpsilonStaysSq == LinearExpr.Sum(epsilons_stays));
            var allAttendantStarts = model.NewIntVar(0, max_travel_time, $"atndnt starts to minimize ");

            model.Add(allAttendantStarts == LinearExpr.Sum(attendantStarts));

            // reward for cleaning complete floors
            var totalFloorsCleaned = model.NewIntVar(0, floorsCleaned.Count(), $"Number of floors completely assigned");
            model.Add(totalFloorsCleaned == LinearExpr.Sum(floorsCleaned));

            // var count_maxed_rooms_workers = model.NewIntVar(0, primaryAttendants.Count(), "number of workers doing maximum rooms allowed");
            // var count_maxed_credits_workers = model.NewIntVar(0, primaryAttendants.Count(), "number of workers doing maximum credits allowed");


            if (skip_solver)
            {
                Console.WriteLine("skipping solbver because nothing to do");

                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = "Skipping solver, nothing to do...",
                    StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });
                foreach (var attendant in primaryAttendants)
                {
                    foreach (var assigned in attendant.Cleanings)
                    {
                        var cleaning = _context.Cleanings.Where(c => (c.Room.RoomName == assigned.Room.RoomName) ||
                                                                (c.Room.PmsRoomName == assigned.Room.PmsRoomName)).First();
                        cleaning.Plan = new CleaningPlan
                        {
                            WorkerUsername = attendant.Username,
                            // Attendant = attendant,
                            From = assigned.From,
                            To = assigned.To,
                            CreatedBy = assigned.Plan?.CreatedBy ?? "PrePlan"
                        };
                    }
                }
                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = "Finished, nothing to do",
                    StatusKey = CpsatProgressStatus.FINISHED_NOTHING_TO_DO.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });

                this.OnCpsatPlannerResultsGenerated(new AutoGeneratedPlan
                {
                    CleaningContext = this._context,
                    PlannedCleanings = this._context.Cleanings,
                    HotelId = this._hotelId,
                });

                return;
            }

            // create tracking variables for all of the elements of the objective
            var sumEpsilonStaysSqVar = model.NewIntVar(0, large_number, $"sum of epsilon stays");
            model.Add(sumEpsilonStaysSqVar == sumEpsilonStaysSq);

            var sumEpsilonDepartsSqVar = model.NewIntVar(0, large_number, $"sum of epsilon departs");
            model.Add(sumEpsilonDepartsSqVar == sumEpsilonDepartsSq);

            var sumEpsilonCreditsSqVar = model.NewIntVar(0, large_number, $"sum of epsilon credits");
            model.Add(sumEpsilonCreditsSqVar == sumEpsilonCreditsSq);

            var sumEpsilonRoomsSqVar = model.NewIntVar(0, large_number, $"sum of epsilon rooms");
            model.Add(sumEpsilonRoomsSqVar == sumEpsilonRoomsSq);

            var allRoomTravelVar = model.NewIntVar(0, large_number, $"all room to room travel");
            model.Add(allRoomTravelVar == all_room_travel);

            var allFloorTravelVar = model.NewIntVar(0, large_number, $"all floor to floor travel");
            model.Add(allFloorTravelVar == all_floor_travel);

            var allBuildingTravelVar = model.NewIntVar(0, large_number, $"all building to building travel");
            model.Add(allBuildingTravelVar == all_building_travel);

            var allCreditsVar = model.NewIntVar(0, large_number, $"sum of credits assigned for cleaning");
            model.Add(allCreditsVar == all_credits);

            var allEndsVar = model.NewIntVar(0, large_number, $"count of rooms assigned for cleaning");
            model.Add(allEndsVar == all_ends);

            var allRoomAwardsVar = model.NewIntVar(0, large_number, $"sum of awards for rooms with desired feature");
            model.Add(allRoomAwardsVar == all_room_awards);

            var allLevelChangesVar = model.NewIntVar(0, large_number, $"sum of floor changes across all attendants");
            model.Add(allLevelChangesVar == all_level_changes);

            // we try to maximize a single Value (objective= so i minimize the travel distance

            /// objective components
            /// floor to floor travel, in # of floors, weighted by weight_travel_time
            /// building to building travel, in minutes, weighted by weight_travel_time
            /// sum of all cleaning time by all, in minutes, weighted by weight_travel_time
            /// count of all rooms cleaned, in # of rooms, weighted by weight_rooms_cleaned
            /// sum of all level awards, weighted by weight_level_award
            /// sum of all room awards, weighted by weight_room1_award
            ///

            /// now adding epsilon, deviation from the mean cleaning
            /// time, in minutes, weighted by negative of
            /// weight_credits
            int scaleroomcleaning = 4;
            var objective =
                num_att * weight_minimize_attendants
                + sumEpsilonStaysSqVar * scaleroomcleaning  * weight_epsilon_stay_depart  // minimize epsilon (deviation from mean)
                + sumEpsilonDepartsSqVar * scaleroomcleaning * weight_epsilon_stay_depart  // minimize epsilon (deviation from mean)
                + sumEpsilonCreditsSqVar * scaleroomcleaning * weight_epsilon_credits  // minimize epsilon (deviation from mean)
                + sumEpsilonRoomsSqVar * scaleroomcleaning * weight_epsilon_rooms  //may be do a different weigh for the rooms
                + allRoomTravelVar * weight_travel_time  // minimize room travel
                + allFloorTravelVar * scaleroomcleaning * weight_travel_time  // minimize floor travel
                + allBuildingTravelVar * scaleroomcleaning * weight_travel_time  // minimize travel to bldgs
                + allCreditsVar * scaleroomcleaning * weight_credits  //maximize cleaning time
                + allEndsVar * scaleroomcleaning * weight_rooms_cleaned// maximize number rooms cleaned
                + totalFloorsCleaned * scaleroomcleaning * weightFloorsCompleted // placeholder weight
                //+ all_building_awards * weight_building_award // reward assigning preferred bldgs
                //+ all_level_awards * weight_level_award // reward assigning preferred floors

                + allAttendantStarts * -1 // try to start as small as possible

                + allRoomAwardsVar * scaleroomcleaning * weight_room_award // reward assigning preferred rooms
                + allLevelChangesVar * scaleroomcleaning * weight_level_changes; //minimize level changes
                // + all_floor_travel * weight_level_changes; //minimize level changes

            model.Maximize(objective);

            _context.HotelStrategy.CPSat.SolutionResult.SolverInformation.Add(model.ModelStats());

            // Creates the solver and solve.
            CpSolver solver = new CpSolver();

            solver.StringParameters = $"num_search_workers:{cpus}, log_search_progress: {log_progress}, max_time_in_seconds:{solver_run_time}";

            // to allow solver to use all of the cpus, uncomment the following line
            //solver.StringParameters = $"log_search_progress: {log_progress}, max_time_in_seconds:{solver_run_time}";

            //  VarArraySolutionPrinter cb = new VarArraySolutionPrinter(new IntVar[] { x, y, z });
            //solver.SearchAllSolutions(model, cb);

            var trackProgress = new List<IntVar>();
            trackProgress.AddRange(total_assigned.Values);
            trackProgress.AddRange(level_change_count);
            // add the objective components too
            trackProgress.Add(num_att);
            trackProgress.Add(sumEpsilonStaysSqVar);
            trackProgress.Add(sumEpsilonDepartsSqVar);
            trackProgress.Add(sumEpsilonCreditsSqVar);
            trackProgress.Add(sumEpsilonRoomsSqVar);
            trackProgress.Add(allRoomTravelVar);
            trackProgress.Add(allFloorTravelVar);
            trackProgress.Add(allBuildingTravelVar);
            trackProgress.Add(allCreditsVar);
            trackProgress.Add(allEndsVar);
            trackProgress.Add(totalFloorsCleaned);
            trackProgress.Add(allAttendantStarts);
            trackProgress.Add(allRoomAwardsVar);
            trackProgress.Add(allLevelChangesVar);

            var cb = new VarArraySolutionPrinterWithObjective(trackProgress.ToArray(), this.CpsatPlannerProgressChanged, timeZoneInfo);

            // CpSolverStatus status = solver.Solve(model);
            CpSolverStatus status = solver.Solve(model, cb);

            //StringBuilder objectFunctionExplainer=new StringBuilder();
            List<string> objectFunctionExplainer = new List<string>();

            objectFunctionExplainer.Add($"num_att: {solver.Value(num_att)} * weight_minimize_attendants: {weight_minimize_attendants}");
            objectFunctionExplainer.Add($"sumEpsilonStaysSq: {solver.Value(sumEpsilonStaysSq)}   * scaleroomcleaning: {scaleroomcleaning} * weight_epsilon_stay_depart: {weight_epsilon_stay_depart}");
            objectFunctionExplainer.Add($"sumEpsilonDepartsSq: {solver.Value(sumEpsilonDepartsSq)} * scaleroomcleaning: {scaleroomcleaning} * weight_epsilon_stay_depart: {weight_epsilon_stay_depart}");
            objectFunctionExplainer.Add($"sumEpsilonCreditsSq: {solver.Value(sumEpsilonCreditsSq)} * scaleroomcleaning: {scaleroomcleaning} * weight_epsilon_credits: {weight_epsilon_credits}");
            objectFunctionExplainer.Add($"sumEpsilonRoomsSq: {solver.Value(sumEpsilonRoomsSq)}   * scaleroomcleaning: {scaleroomcleaning} * weight_epsilon_rooms: {weight_epsilon_rooms}");
            objectFunctionExplainer.Add($"all_room_travel: {solver.Value(all_room_travel)}     * weight_travel_time: {weight_travel_time}");
            objectFunctionExplainer.Add($"all_floor_travel: {solver.Value(all_floor_travel)}    * scaleroomcleaning: {scaleroomcleaning} * weight_travel_time: {weight_travel_time}");
            objectFunctionExplainer.Add($"all_building_travel: {solver.Value(all_building_travel)} * scaleroomcleaning: {scaleroomcleaning} * weight_travel_time: {weight_travel_time}");
            objectFunctionExplainer.Add($"all_credits: {solver.Value(all_credits)}         * scaleroomcleaning: {scaleroomcleaning} * weight_credits: {weight_credits}");
            objectFunctionExplainer.Add($"all_ends: {solver.Value(all_ends)}            * scaleroomcleaning: {scaleroomcleaning} * weight_rooms_cleaned: {weight_rooms_cleaned}");
            objectFunctionExplainer.Add($"totalFloorsCleaned: {solver.Value(totalFloorsCleaned)}  * scaleroomcleaning: {scaleroomcleaning} * weightFloorsCompleted: {weightFloorsCompleted}");
            objectFunctionExplainer.Add($"allAttendantStarts: {solver.Value(allAttendantStarts)}  * -1");
            objectFunctionExplainer.Add($"all_room_awards: {solver.Value(all_room_awards)}     * scaleroomcleaning: {scaleroomcleaning} * weight_room_award: {weight_room_award}");
            objectFunctionExplainer.Add($"all_level_changes: {solver.Value(all_level_changes)}   * scaleroomcleaning: {scaleroomcleaning} * weight_level_changes: {weight_level_changes}");

            foreach (var line in objectFunctionExplainer)
            {

                Console.WriteLine(line);
                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = line,
                    StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });
            }



            //Console.WriteLine("---SufficientAssumptionsForInfeasibility---");
            //Console.WriteLine(solver.SufficientAssumptionsForInfeasibility());
            // foreach (var var_index in solver.SufficientAssumptionsForInfeasibility())
            // {
            //     Console.WriteLine(var_index);
            // }

            // this.OnCpsatPlannerProgressChanged(new ProgressMessage
            // {
            //     CleaningPlanId = Guid.Empty,
            //     Message = "Sufficient assumptions for infeasibility",
            //     StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
            //     DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            // });
            // this.OnCpsatPlannerProgressChanged(new ProgressMessage
            // {
            //     CleaningPlanId = Guid.Empty,
            //     Message = string.Join(", ", solver.SufficientAssumptionsForInfeasibility().Select(i => i.ToString())),
            //     StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
            //     DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            // });

            // uncomment to get a dump of the result hints and information to console
            // foreach (var line in _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation)
            //     Console.WriteLine(line);

            Console.WriteLine("---");

            var validationResult = model.Validate();
            Console.WriteLine(validationResult);

            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = validationResult,
                StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });

            if (status == CpSolverStatus.Infeasible)
            {
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("-------------------------");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("Infeasible");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("-------------------------");

                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = "The model is infeasible. Please review your constraints.",
                    StatusKey = CpsatProgressStatus.FINISHED_INFEASIBLE.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });

                this.OnCpsatPlannerResultsGenerated(new AutoGeneratedPlan
                {
                    CleaningContext = this._context,
                    PlannedCleanings = this._context.Cleanings,
                    HotelId = this._hotelId,
                });
                return;
            }
            else if (status == CpSolverStatus.ModelInvalid)
            {
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("-------------------------");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("Model Invalid");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("-------------------------");

                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = "The model is invalid.",
                    StatusKey = CpsatProgressStatus.FINISHED_INVALID.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });

                this.OnCpsatPlannerResultsGenerated(new AutoGeneratedPlan
                {
                    CleaningContext = this._context,
                    PlannedCleanings = this._context.Cleanings,
                    HotelId = this._hotelId,
                });
                return;
            }
            else if (status == CpSolverStatus.Unknown)
            {
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("Not enough time allowed to compute a solution");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add(solver.ResponseStats());

                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = "Not enough time to compute a solution",
                    StatusKey = CpsatProgressStatus.FINISHED_UNKNOWN.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });

                this.OnCpsatPlannerResultsGenerated(new AutoGeneratedPlan
                {
                    CleaningContext = this._context,
                    PlannedCleanings = this._context.Cleanings,
                    HotelId = this._hotelId,
                });
                return;
            }

            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"attendants used = {solver.Value(num_att)}");
            var log = new StringBuilder();

            var resultStatus = status.ToString();
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("-------------------------");
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add(resultStatus);
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("-------------------------");

            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = resultStatus,
                StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });


            // debugging totalFloorsCleaned.  Delete when it works okay in v2
            Console.WriteLine($"total floors completed = {solver.Value(totalFloorsCleaned)}");
            foreach (var fc in floorsCleaned)
            {
                Console.WriteLine($"{fc}, {solver.Value(fc)}");
            }
            foreach (var kvp in floorCleanings)
            {
                var cleaningLiterals = kvp.Value;
                foreach (var cl in cleaningLiterals)
                {
                    Console.WriteLine($"{cl}, {solver.Value(cl)}");
                }
            }

            // debugging attendant starts
            foreach (var start in attendantStarts)
            {
                Console.WriteLine($"{start}, {solver.Value(start)}");
            }

            // Statistics.
            // Console.WriteLine(solver.ResponseStats());
            // aggregate results
            // first, what rooms got cleaned?
            var skipped_rooms = new List<string>();
            foreach (KeyValuePair<Cleaning, List<IntVar>> kvp in room_literals)
            {
                Room room = kvp.Key.Room;
                Boolean skipped = true;
                foreach (var lit in kvp.Value)
                {
                    var result = solver.Value(lit);
                    if (result == 1)
                    {
                        skipped = false;
                        //Console.WriteLine($"room {room.RoomName} is cleaned {lit}");
                        continue;
                    }
                }
                if (skipped)
                {
                    skipped_rooms.Add(room.RoomName);
                }
            }
            skipped_rooms.Sort();
            string skippedCleaningsMessage;
            if (_context.Cleanings.Count() != room_literals.Count())
            {
                skippedCleaningsMessage = $"Total requested cleanings is {_context.Cleanings.Count()}.  Constraints eliminated {_context.Cleanings.Count() - room_literals.Count()}, for a total of {room_literals.Count()} cleanings that are possible to perform.  The solver was able to assign {room_literals.Count() - skipped_rooms.Count()} cleanings out of {room_literals.Count()}.";
            }
            else
            {
                skippedCleaningsMessage = $"Total requested cleanings is {_context.Cleanings.Count()}.  The solver was able to assign {room_literals.Count() - skipped_rooms.Count()}, and skipped {skipped_rooms.Count()}.  Over those assigned cleanings, the average expected cleaning time is {solver.Value(ave_c)}.";
            }
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add(skippedCleaningsMessage);

            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = skippedCleaningsMessage,
                StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });


            if (weight_epsilon_credits != 0)
            {
                var message = $"The final sum of epsilons cleaning time value is {solver.Value(sumEpsilonCreditsSq)}";
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add(message); //, meaning each attendant is within (+ or -) {solver.Value(epsilonCredits)} minutes of {solver.Value(ave_c)}.  ");

                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = message,
                    StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });
            }

            if (weight_epsilon_rooms != 0)
            {
                var message = $"The final sum of epsilons cleaning room count value is {solver.Value(sumEpsilonRoomsSq)}";
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add(message); //, meaning each attendant is within (+ or -) {solver.Value(epsilonCleaningRooms)} of {solver.Value(ave_r)}.  ");

                this.OnCpsatPlannerProgressChanged(new ProgressMessage
                {
                    CleaningPlanId = Guid.Empty,
                    Message = message,
                    StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                    DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                });
            }


            if (skipped_rooms.Count() > 0)
            {
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("Unassigned rooms:");
            }
            skipped_rooms.ForEach(delegate (string RoomName)
            {

                //_context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Hint : Check that there are no rooms that can't be done at the same time (DEP/ARR");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"{RoomName}");
            });

            // process the result, including save plan for display
            List<ObjectiveComponents> totalContributionToObjectiveValue = new List<ObjectiveComponents>();
            var active_cleaning_intervals = new List<CleaningInterval>();

            //compute here the mean credits for rooms
            //int average_cleaning_credits = (int)_context.Cleanings.Average(c => c.Room.Credits);
            //if (weight_epsilon_rooms < 0 && weight_epsilon_rooms != -average_cleaning_credits)
            //    Log.GetLog().Warning($"PAY ATTENTION : probably the weight epsilon room should be close to - {average_cleaning_credits}");

            long totalFloorTravel = 0;
            long totalBuildingTravel = 0;
            for (int attendant_index = 0; attendant_index < primaryAttendants.Count(); ++attendant_index)
            {
                var attendant = primaryAttendants[attendant_index];
                attendant.Cleanings.Clear();
                if (!all_cleanings.ContainsKey(attendant.Id))
                {
                    continue;
                }

                //Log.GetLog().Warning($"Attendant : {attendant.Username}");
                //Log.GetLog().Warning($"------------------------------------");

                //_context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Attendant : {attendant.Username}");
                //_context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"------------------------------------");
                int actual_credits_cleaned = 0;
                // int timeStart = 0;
                // int timeEnd = 0;
                var cleaning_jobs = new List<CleaningInterval>();

                long floortravel = 0;
                foreach (var floor_travel in per_attendant_floor_travel[attendant.Id])
                {
                    var result = solver.Value(floor_travel);
                    floortravel += result;
                }
                totalFloorTravel += floortravel;
                // all_building_cleanings[attendant].ForEach(bc => Console.WriteLine($"{attendant.Username} building {bc.BuildingName} literal {solver.Value(bc.Literal)} from: {solver.Value(bc.Start)}, to: {solver.Value(bc.End)}"));

                long buildingtravel = 0;
                foreach (var building_travel in per_attendant_building_travel[attendant.Id])
                {
                    var result = solver.Value(building_travel);
                    buildingtravel += result;
                }
                totalBuildingTravel += buildingtravel;
                foreach (var cleaning_interval in all_cleanings[attendant.Id])
                {
                    var result = solver.Value(cleaning_interval.Literal);
                    if (result == 1)
                    {
                        cleaning_jobs.Add(cleaning_interval);
                        actual_credits_cleaned += cleaning_interval.Cleaning.Credits;
                    }
                }
                // sort assigned jobs by start time
                cleaning_jobs.Sort(delegate (CleaningInterval x,
                                             CleaningInterval y)
                {
                    if (solver.Value(x.Literal) == 0 &&
                        solver.Value(y.Literal) == 0) return 0;
                    else if (solver.Value(x.Literal) == 0) return -1;
                    else if (solver.Value(y.Literal) == 0) return 1;
                    else return solver.Value(x.Start).CompareTo(solver.Value(y.Start));
                });
                // now print out and process the list of assigned job details, in order
                foreach (var cleaning_interval in cleaning_jobs)
                {
                    active_cleaning_intervals.Add(cleaning_interval);
                    var cleaning = cleaning_interval.Cleaning;
                    // use the earliest_date found in "loop 0" to
                    // recover the actual datetime from the "minutes"
                    // computed by the solver.
                    var start = earliest_date + TimeSpan.FromMinutes(solver.Value(cleaning_interval.Start));
                    var end = earliest_date + TimeSpan.FromMinutes(solver.Value(cleaning_interval.End));
                    //_context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add
                    string depArrState=$"{cleaning.Type}";
                    if (cleaning.Type == CleaningType.Departure)
                    {
                        if (cleaning.ArrivalExpected)
                            depArrState = "DEP/ARR";
                        else
                            depArrState = "DEP";
                    }
                    else if (cleaning.ArrivalExpected)
                        depArrState = "ARR";
                    Console.WriteLine($"{attendant.Username} cleans {String.Join('/',GetRoomLocation(cleaning))}  : type {depArrState}, literal {cleaning_interval.Literal},  earliest allowed: {cleaning.From} <   expected start: {start} -   expected end: {end} <=  latest end allowed: {cleaning.To}");

                    cleaning.Plan = new CleaningPlan
                    {
                        WorkerUsername = attendant.Username,
                        //Attendant = attendant,
                        From = start,
                        To = end,
                        CreatedBy = cleaning.Plan?.CreatedBy ?? "Planner"
                    };
                    attendant.Cleanings.Add(cleaning); // TODO Jonathan FIX why in Preplan Mode, am i adding those to this cleaner ?
                }
                // overall measures for the attendant...total travel, total cleaning, etc
                // Note, travel should now be on again...represents level to level travel times
                long credits = solver.Value(total_credits[attendant.Id]);

                // floor count in the ideal case should be one
                var uniqueFloorNames = cleaning_jobs.Select(c => c.Cleaning.Room.Floor.LevelName).Distinct();

                long rooms = solver.Value(total_assigned[attendant.Id]);
                long stays_cleaned = solver.Value(attendant_stays_cleaned[attendant.Id]);
                long departs_cleaned = solver.Value(attendant_departs_cleaned[attendant.Id]);

                long extraDepartures = 0;
                if (allAdditionalDepartures.ContainsKey(attendant.Id))
                {
                    extraDepartures = solver.Value(allAdditionalDepartures[attendant.Id]);
                }
                long extraStays = 0;
                if (allAdditionalStays.ContainsKey(attendant.Id))
                {
                    extraStays = solver.Value(allAdditionalStays[attendant.Id]);
                }
                long credit_reductions = 0;
                if (total_credit_reductions.ContainsKey(attendant.Id))
                {
                     credit_reductions = solver.Value(total_credit_reductions[attendant.Id]);
                }
                Console.WriteLine($"debugging: rooms is {rooms}, stays is {stays_cleaned}, departs is {departs_cleaned}.  Does it add up? {rooms == stays_cleaned+departs_cleaned}.  And extra departures is {extraDepartures} with penalty {maxDeparturesEquivalentCredits}; extra stays is {extraStays} with reward {maxStaysEquivalentCredits}; and change in credits is {credit_reductions}");

                long levels_cleaned = solver.Value(total_levels_cleaned[attendant.Id]);
                //long building_awards = solver.Value(total_buildings_award[attendant_index]);
                //long level_awards = solver.Value(total_levels_award[attendant_index]);
                long room_awards = solver.Value(total_rooms_award[attendant.Id]);

                if (level_movement_reduces_credits || building_movement_reduces_credits || maxDeparturesReducesCredits)
                {
                    if (_context.HotelStrategy.CPSat.maxCredits - credits < max_effort_credits && weight_epsilon_rooms < 0)
                    {
                        var message = $"PAY ATTENTION : looks like the Global cleaning time set in the Setting is limiting your attendants ";
                        _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add(message);

                        this.OnCpsatPlannerProgressChanged(new ProgressMessage
                        {
                            CleaningPlanId = Guid.Empty,
                            Message = message,
                            StatusKey = CpsatProgressStatus.IN_PROGRESS_ALERT.ToString(),
                            DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                        });
                    }
                }
                else
                {
                    if (_context.HotelStrategy.CPSat.maxCredits - actual_credits_cleaned < max_effort_credits && weight_epsilon_rooms < 0)
                    {
                        var message = $"PAY ATTENTION : looks like the Global cleaning time set in the Setting is limiting your attendants ";
                        _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add(message);

                        this.OnCpsatPlannerProgressChanged(new ProgressMessage
                        {
                            CleaningPlanId = Guid.Empty,
                            Message = message,
                            StatusKey = CpsatProgressStatus.IN_PROGRESS_ALERT.ToString(),
                            DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                        });
                    }
                }

                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"\n\n_____attendant {attendant.Username}___ ");
                if (_context.HotelStrategy.CPSat.maxRooms - rooms < 1 && weight_epsilon_credits < 0)
                {
                    var message = $"PAY ATTENTION : looks like the Global max rooms set in the Setting is limiting your attendants ";
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add(message);

                    this.OnCpsatPlannerProgressChanged(new ProgressMessage
                    {
                        CleaningPlanId = Guid.Empty,
                        Message = message,
                        StatusKey = CpsatProgressStatus.IN_PROGRESS_ALERT.ToString(),
                        DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
                    });
                }

                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"floor to floor travel is {floortravel}, ");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"building to building travel is {buildingtravel}, ");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"credits is {credits}");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"rooms is {rooms}");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"levels cleaned is {levels_cleaned}");
                var floorString = String.Join(", ", uniqueFloorNames);
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"floors: {uniqueFloorNames.Count()}: {floorString}");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"room_award is {room_awards}");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"stays cleaned is {stays_cleaned}");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"departs cleaned is {departs_cleaned}");
                if (weight_epsilon_credits != 0)
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"epsilonCredits: {solver.Value(epsilons_credits[attendant.Id])}");
                if (weight_epsilon_rooms != 0)
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"epsilonCleaningRooms: {solver.Value(epsilons_rooms[attendant.Id])}");



                ObjectiveComponents att_obj_comps = new ObjectiveComponents(
                    _floortravel: floortravel,
                    _buildingtravel: buildingtravel,
                    _credits: credits,
                    _cleaning_jobs: cleaning_jobs,
                    _room_awards: room_awards,
                    _epsilonCredits: solver.Value(sumEpsilonCreditsSq),
                    _epsilonCleaningRooms: solver.Value(sumEpsilonRoomsSq),
                    _stays_cleaned: stays_cleaned,
                    _departs_cleaned: departs_cleaned,
                    _epsilonCleaningStays: solver.Value(sumEpsilonStaysSq),
                    _epsilonCleaningDeparts: solver.Value(sumEpsilonDepartsSq),
                    _attendant_literal: solver.Value(attendant_literals[attendant.Id]),
                    _weight_minimize_attendants: weight_minimize_attendants,
                    _weight_epsilon_credits: weight_epsilon_credits,
                    _weight_epsilon_rooms: weight_epsilon_rooms,
                    _weight_epsilon_stay_depart: weight_epsilon_stay_depart,
                    _weight_travel_time: weight_travel_time,
                    _weight_credits: weight_credits,
                    _weight_rooms_cleaned: weight_rooms_cleaned,
                    _weight_building_award: weight_building_award,
                    _weight_level_award: weight_level_award,
                    _weight_room_award: weight_room_award);


                att_obj_comps.attendant = attendant;

                totalContributionToObjectiveValue.Add(att_obj_comps);
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"-------------Obj value contribution for {attendant.Username}: {att_obj_comps.get_total_contribution()}-------------");
                if (weight_epsilon_rooms != 0 && !target_mode)
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"{attendant.Username} cleans {cleaning_jobs.Count()} rooms, vs global average of {solver.Value(ave_r)}.  Actual difference minimized is {solver.Value(epsilons_rooms[attendant.Id])}");
                if (weight_epsilon_credits != 0 && !target_mode)
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"{attendant.Username} cleans {actual_credits_cleaned} minutes, with travel is {credits} vs target of {solver.Value(ave_c)}.");
                if (weight_epsilon_stay_depart != 0)
                    _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"{attendant.Username} cleans stays of {stays_cleaned} vs target of {solver.Value(ave_s)}, and departures of {departs_cleaned} vs target of {solver.Value(ave_d)}.");
                if (target_mode)
                {
                    if (weight_epsilon_rooms != 0)
                        _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"{attendant.Username} cleans {cleaning_jobs.Count()} rooms, leaving a variance of {solver.Value(epsilons_rooms[attendant.Id])}.");
                    if (weight_epsilon_credits != 0)
                        _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"{attendant.Username} cleans {total_credits}, leaving a variance of {solver.Value(epsilons_credits[attendant.Id])}.");
                }

                LogPlan(log);
                //Log.GetLog().WithFile(FileExtension.Txt, log.ToString()).Debug(log.ToString());
            }

            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add("---------- Global Statistics ----------");

            var epsilonRoomsNote = weight_epsilon_rooms != 0 ? "(active goal)" : "(ignored due to settings)";
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Room balancing {epsilonRoomsNote}: cleaning rooms epsilon value is: {solver.Value(sumEpsilonRoomsSq)} * {weight_epsilon_rooms}");

            var epsilonCreditsNote = weight_epsilon_credits != 0 ? "(active goal)" : "(ignored due to settings)";
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Credit balancing {epsilonCreditsNote}: cleaning credits epsilon value is: {solver.Value(sumEpsilonCreditsSq)} * {weight_epsilon_credits}");

            if (weight_epsilon_stay_depart != 0)
            {
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Final cleaning stays epsilon value is: {solver.Value(sumEpsilonStaysSq)} * {weight_epsilon_stay_depart}");
                _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Final cleaning departures epsilon value is: {solver.Value(sumEpsilonDepartsSq)} * {weight_epsilon_stay_depart}");
            }
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Final overall floor to floor travel is {totalFloorTravel} * {weight_travel_time}");
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"Final overall building to building travel is {totalBuildingTravel} * {weight_travel_time}");

            ObjectiveComponents all_obj_comps = new ObjectiveComponents(
                _floortravel: solver.Value(all_floor_travel),
                _buildingtravel: solver.Value(all_building_travel),
                _credits: solver.Value(all_credits),
                _cleaning_jobs: active_cleaning_intervals,
                _room_awards: solver.Value(all_room_awards),
                _epsilonCredits: solver.Value(sumEpsilonCreditsSq),
                _epsilonCleaningRooms: solver.Value(sumEpsilonRoomsSq),
                _stays_cleaned: solver.Value(sum_s),
                _departs_cleaned: solver.Value(sum_d),
                _epsilonCleaningStays: solver.Value(sumEpsilonStaysSq),
                _epsilonCleaningDeparts: solver.Value(sumEpsilonDepartsSq),
                _attendant_literal: solver.Value(num_att),
                _weight_minimize_attendants: weight_minimize_attendants,
                _weight_epsilon_credits: weight_epsilon_credits,
                _weight_epsilon_rooms: weight_epsilon_rooms,
                _weight_epsilon_stay_depart: weight_epsilon_stay_depart,
                _weight_travel_time: weight_travel_time,
                _weight_credits: weight_credits,
                _weight_rooms_cleaned: weight_rooms_cleaned,
                _weight_building_award: weight_building_award,
                _weight_level_award: weight_level_award,
                _weight_room_award: weight_room_award);

            // _context.HotelStrategy.CPSat.SolutionResult.objectiveComponent = all_obj_comps;
            var totalGlobalObjectValueMessage = $"Total Objective value {all_obj_comps.get_total_contribution()}";
            _context.HotelStrategy.CPSat.SolutionResult.RoomcheckingInformation.Add($"-------------{totalGlobalObjectValueMessage}-------------");
            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = totalGlobalObjectValueMessage,
                StatusKey = CpsatProgressStatus.IN_PROGRESS.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });
            var global_contributions = all_obj_comps.get_contributions_dictionary();

            int fake_time = 0;
            foreach (KeyValuePair<string, long> kvp in global_contributions.contributions)
            {
                _context.HotelStrategy.CPSat.SolutionResult.visualization.items.Add(new item() { x = fake_time, y = kvp.Value, group = kvp.Key });

            }

            foreach (var att_obj_comps in totalContributionToObjectiveValue)
            {
                fake_time += 1;
                var contributions = att_obj_comps.get_contributions_dictionary();
                foreach (KeyValuePair<string, long> kvp in contributions.contributions)
                {
                    _context.HotelStrategy.CPSat.SolutionResult.visualization.items.Add(new item() { x = fake_time, y = kvp.Value, group = kvp.Key });

                }
            }

            this.OnCpsatPlannerProgressChanged(new ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = "Solver algorithm finished !",
                StatusKey = CpsatProgressStatus.FINISHED.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo).ToString("yyyy-MM-dd HH:mm"),
            });

            this.OnCpsatPlannerResultsGenerated(new AutoGeneratedPlan
            {
                CleaningContext = this._context,
                PlannedCleanings = this._context.Cleanings,
                HotelId = this._hotelId,
            });
        }

        private void LogPlan(StringBuilder log)
        {
            //log.AppendLine("Solution:");
            //var solution = _context.Attendants.OrderBy(a => a.Name)
            //    .GroupJoin(_context.Cleanings.Where(c => c.Plan != null).OrderBy(c => c.Plan.From), a => a, c => c.Plan.Attendant, (a, c) => new { Attendant = a, Cleanings = c })
            //    .ToArray();
            //foreach (var group in solution)
            //{
            //    log.AppendLine($"{group.Attendant.Name}:");
            //    if (group.Cleanings != null)
            //    {
            //        foreach (var c in group.Cleanings)
            //        {
            //            log.AppendLine($"[{c.Plan.From:HH:mm} - {c.Plan.To:HH:mm}] - {ToString(c)}");
            //        }
            //    }
            //    log.AppendLine(string.Empty);
            //}

            //var unplannedCleanings = _context.Cleanings.Where(c => c.Plan == null).ToList();
            //log.AppendLine($"Unplanned cleanings ({unplannedCleanings.Count}):");
            //unplannedCleanings.ForEach(c => log.AppendLine(ToString(c)));
        }

        private string ToString(Cleaning cleaning)
        {
            return $"Room {cleaning.Room.RoomName} ({string.Join(", ", GetCleaningInfo(cleaning))})";
        }

        private IEnumerable<string> GetRoomLocation(Cleaning cleaning)
        {
            if (cleaning.Room?.Building?.LevelName != null)
            {
                yield return $"{cleaning.Room.Building.LevelName}";
            }
            if (cleaning.Room?.Floor?.LevelName  != null)
            {
                yield return $"{cleaning.Room.Floor.LevelName}";
            }
            if (cleaning.Room?.Section?.LevelName  != null)
            {
                yield return $"{cleaning.Room?.Section?.LevelName}";
            }
            if (cleaning.Room?.Subsection?.LevelName  != null)
            {
                yield return $"{cleaning.Room?.Subsection?.LevelName}";
            }
            yield return $"{cleaning.Room.PmsRoomName}";
        }


        private IEnumerable<string> GetCleaningInfo(Cleaning cleaning)
        {
            yield return cleaning.Type.ToString();
            yield return $"Credits: {cleaning.Credits}";
            if (cleaning.Type == CleaningType.Departure)
            {
                yield return $"ETD: {cleaning.From.TimeOfDay:hh\\:mm}";
            }
            if (cleaning.ArrivalExpected)
            {
                yield return $"ETA: {cleaning.To.TimeOfDay:hh\\:mm}";
            }
        }
    }

    public enum HousekeepingStatus
    {
        Unknown,
        Dirty,
        InProgress,
        Clean,
        Inspected
    }

    //public enum CleaningType
    //{
    //    Unknown,
    //    Stay,
    //    VacantDirty,
    //    VacantOther,
    //    Departure,
    //    Cleaning,
    //    NoCleaning,
    //    ChangeSheet
    //}

    public enum CleaningType
    {
        //Unkonwn, // OK, lets keep this one
        Departure,
        Stay,
        Custom,
        ChangeSheet,
        Vacant
    }

    public static class LinqExtensions
    {
        public static TValue TryGetValueOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : class =>
            key != null && dictionary.TryGetValue(key, out var value) ? value : null;

#if NET40 || NET452

        /// <summary>
        /// Returns the <see cref="HashSet{T}"/> corresponding with the
        /// <paramref name="collection"/>. Provided for compatibility with .NET Framework 4.5.2
        /// targets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> collection) => new HashSet<T>(collection);

#endif

    }




    public class CPSatSolutionInformation
    {
        public List<string> SolverInformation { get; set; }
        public List<string> RoomcheckingInformation { get; set; }

        public ObjectiveComponents objectiveComponent { get; set; }

        public Visualization visualization { get; set; }

        public CPSatSolutionInformation()
        {
            SolverInformation = new List<string>();
            RoomcheckingInformation = new List<string>();
            visualization = new Visualization();
        }

    }

    public class group
    {

        public string id;
        public long content;

    }

    public class item
    {

        public int x;
        public long y;
        public string group;


    }

    public class Visualization
    {
        public List<group> groups { get; set; }
        public List<item> items { get; set; }
        public Visualization()
        {
            groups = new List<group>();
            items = new List<item>();

        }



    }



}
