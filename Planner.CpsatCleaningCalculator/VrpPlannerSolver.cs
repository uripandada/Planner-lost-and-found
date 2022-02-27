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

    public class OverridenCleaning
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string Label { get; set; }
    }

    public class CallbackNodeEvaluator
    {
        public delegate int GetValue(int node);
        public delegate int GetDistance(int fromNode, int toNode);

        private readonly Google.OrTools.ConstraintSolver.RoutingIndexManager _routingIndexManager;
        private readonly int[,] _distances;

        public CallbackNodeEvaluator(Google.OrTools.ConstraintSolver.RoutingIndexManager routingIndexManager, GetDistance callback, int nodesCount, bool excludeStart = true, bool excludeEnd = true)
        {
            _routingIndexManager = routingIndexManager;
            _distances = new int[nodesCount, nodesCount];
            for (int i = 0; i < nodesCount; i++)
            {
                for (int j = 0; j < nodesCount; j++)
                {
                    if (i == j || j == 0 || i == nodesCount - 1 || (excludeStart && i == 0) || (excludeEnd && j == nodesCount - 1))
                    {
                        continue;
                    }
                    _distances[i, j] = excludeStart ? callback(i - 1, j - 1) : callback(i, j);
                }
            }
        }

        public long Call(long fromIndex, long toIndex)
        {
            var fromNode = _routingIndexManager.IndexToNode(fromIndex);
            var toNode = _routingIndexManager.IndexToNode(toIndex);
            return _distances[fromNode, toNode];
        }
    }

    public class ConstraintsConfiguration : ConstraintsBase
    {
        public delegate (long objectiveValue, string description) GetObjectiveValue(Attendant[] attendants, Cleaning[] cleanings);

        private readonly List<(string name, GetObjectiveValue getObjectiveValue)> _objectiveValueProviders = new List<(string name, GetObjectiveValue getObjectiveValue)>();
        public override ConstraintsConfiguration Configuration => this;
        public ConstraintsConfiguration(Google.OrTools.ConstraintSolver.RoutingModel routingModel, Google.OrTools.ConstraintSolver.RoutingIndexManager routingIndexManager) : base(routingModel, routingIndexManager) { }
        private readonly List<CallbackNodeEvaluator> _evaluators = new List<CallbackNodeEvaluator>();

        public void LogObjectiveValues(StringBuilder log, Google.OrTools.ConstraintSolver.Assignment assignment, Attendant[] attendants, Cleaning[] cleanings)
        {
            var totalObjectiveValue = assignment.ObjectiveValue();
            foreach (var objectiveValueProvider in _objectiveValueProviders)
            {
                var (objectiveValue, description) = objectiveValueProvider.getObjectiveValue(attendants, cleanings);
                if (objectiveValue == 0)
                {
                    continue;
                }
                totalObjectiveValue -= objectiveValue;
                log.Append($"{objectiveValueProvider.name}: {objectiveValue}");
                if (description != null)
                {
                    log.Append($" ({description})");
                }
                log.AppendLine();
            }
            log.AppendLine($"Other objective values: {totalObjectiveValue}");
        }

        public void RegisterObjectiveValueProvider(GetObjectiveValue getObjectiveValue, [CallerFilePath] string filePath = null, [CallerMemberName] string memberName = null)
        {
            var name = memberName != ".ctor" ? memberName : Path.GetFileNameWithoutExtension(filePath);
            _objectiveValueProviders.Add((name, getObjectiveValue));
        }

        public int Register(Google.OrTools.ConstraintSolver.RoutingModel routingModel, Google.OrTools.ConstraintSolver.RoutingIndexManager routingIndexManager, CallbackNodeEvaluator.GetDistance callback, bool excludeStart = true, bool excludeEnd = true)
        {
            var evaluator = new CallbackNodeEvaluator(routingIndexManager, callback, routingModel.Nodes(), excludeStart, excludeEnd);
            _evaluators.Add(evaluator);
            return routingModel.RegisterTransitCallback(evaluator.Call);
        }
    }

    public class TimeDimension : DimensionBase
    {
        protected TimeDimension(ConstraintsConfiguration configuration, CallbackNodeEvaluator.GetDistance getDistance)
            : base(configuration)
        {
            RoutingModel.AddDimension(RegisterEvaluator(getDistance), BigNumber, BigNumber, false, DimensionName);
        }

        public TimeDimension AddCreditsWindowConstraints(Cleaning[] cleanings)
        {
            for (int i = 0; i < cleanings.Length; i++)
            {
                var cleaning = cleanings[i];
                var index = RoutingIndexManager.NodeToIndex(i + 1);
                if (cleaning.Plan == null)
                {
                    var timeFrom = (int)cleaning.From.TimeOfDay.TotalMinutes;
                    var timeTo = (int)(cleaning.From.Date == cleaning.To.Date ? cleaning.To.TimeOfDay : TimeSpan.FromDays(1)).TotalMinutes - cleaning.Credits;
                    Dimension.CumulVar(index).SetRange(timeFrom, timeTo);
                }
                else
                {
                    var timeFrom = (int)cleaning.Plan.From.TimeOfDay.TotalMinutes;
                    Dimension.CumulVar(index).SetValue(timeFrom);
                }
            }
            return this;
        }

        //public TimeDimension AddHostingTimeWindowConstraints(Hosting[] hostings)
        //{
        //    for (int i = 0; i < hostings.Length; i++)
        //    {
        //        var hosting = hostings[i];
        //        var index = RoutingIndexManager.NodeToIndex(i + 1);
        //        var timeFrom = (int)hosting.From.TimeOfDay.TotalMinutes;
        //        Dimension.CumulVar(index).SetValue(timeFrom);
        //    }
        //    return this;
        //}

        //public TimeDimension AddRunningTimeWindowConstraints(Running[] runnings)
        //{
        //    for (int i = 0; i < runnings.Length; i++)
        //    {
        //        var running = runnings[i];
        //        var index = RoutingIndexManager.NodeToIndex(i + 1);
        //        if (running.Plan == null)
        //        {
        //            var timeFrom = (int)running.From.TimeOfDay.TotalMinutes;
        //            var timeTo = (int)(running.From.Date == running.To.Date ? running.To.TimeOfDay : TimeSpan.FromDays(1)).TotalMinutes - running.Credits;
        //            Dimension.CumulVar(index).SetRange(timeFrom, timeTo);
        //        }
        //        else
        //        {
        //            var timeFrom = (int)running.Plan.From.TimeOfDay.TotalMinutes;
        //            Dimension.CumulVar(index).SetValue(timeFrom);
        //        }
        //    }
        //    return this;
        //}

        public TimeDimension AddWorkerTimeWindowConstraints<TWorker>(TWorker[] workers) where TWorker : Worker
        {
            for (int i = 0; i < workers.Length; i++)
            {
                var timeSlot = workers[i].CurrentTimeSlot;

                var startIndex = RoutingModel.Start(i);
                var timeFrom = (int)timeSlot.From.TotalMinutes;
                Dimension.CumulVar(startIndex).SetValue(timeFrom);

                var endIndex = RoutingModel.End(i);
                var timeTo = (int)timeSlot.To.TotalMinutes;
                Dimension.CumulVar(endIndex).SetMax(timeTo);
            }
            return this;
        }

        public TimeDimension AddTotalTimeOptimization(long coefficient)
        {
            Dimension.SetSpanCostCoefficientForAllVehicles(coefficient);

            Configuration.RegisterObjectiveValueProvider((attendants, cleanings) =>
            {
                var minutesPerAttendant = attendants
                    .Select(a => (attendant: a.Username, plans: a.Cleanings
                        .Select(c => c.Plan)
                        .Where(p => p != null)
                        .OrderBy(p => p.From)
                        .ToArray()))
                    .Where(a => a.plans.Any())
                    .Select(a => (attendant: a.attendant, minutes: (a.plans.Last().To - a.plans.First().From).TotalMinutes))
                    .ToArray();
                return minutesPerAttendant.Any()
                    ? ((int)minutesPerAttendant.Sum(a => a.minutes) * coefficient, $"({string.Join(" + ", minutesPerAttendant.Select(a => $"{a.minutes} min({a.attendant})"))}) * {coefficient}")
                    : (0, null);
            });
            return this;
        }

        public TimeDimension AddMakespanOptimization(long coefficient)
        {
            Dimension.SetGlobalSpanCostCoefficient(coefficient);

            Configuration.RegisterObjectiveValueProvider((attendants, cleanings) =>
            {
                var plans = cleanings.Select(c => c.Plan).Where(p => p != null).ToArray();
                if (!plans.Any())
                {
                    return (0, null);
                }
                var min = plans.Min(p => p.From);
                var max = plans.Max(p => p.To);
                var minutes = (max - min).TotalMinutes;
                return ((int)minutes * coefficient, $"{minutes} min({min:HH:mm} - {max:HH:mm}) * {coefficient}");
            });
            return this;
        }

        public TimeDimension AddStartTimeOptimization(long coefficient)
        {
            Dimension.SetGlobalSpanCostCoefficient(coefficient);

            Configuration.RegisterObjectiveValueProvider((attendants, cleanings) =>
            {
                var plans = cleanings.Select(c => c.Plan).Where(p => p != null).ToArray();
                if (!plans.Any())
                {
                    return (0, null);
                }
                var min = plans.Min(p => p.From);
                var max = plans.Max(p => p.To);
                var minutes = (max - min).TotalMinutes;
                return ((int)minutes * coefficient, $"{minutes} min({min:HH:mm} - {max:HH:mm}) * {coefficient}");
            });
            return this;
        }
    }

    public class TimeDimension<TActivity> : TimeDimension where TActivity : Activity
    {
        public TimeDimension(ConstraintsConfiguration configuration, TActivity[] activities, GeoDistances distances)
            : base(configuration, (from, to) => distances.GetDistance(activities, from, to)) { }
    }

    public class LevelsDimension<TActivity> : DimensionBase where TActivity : Activity
    {
        public LevelsDimension(ConstraintsConfiguration configuration, TActivity[] activities, long coefficient)
            : base(configuration)
        {
            var evaluator = RegisterActivitiesEvaluator((from, to) => GetLevelDistanceBetweenActivities(activities, from, to));
            RoutingModel.AddDimension(evaluator, Zero, BigNumber, true, DimensionName);
            Dimension.SetSpanCostCoefficientForAllVehicles(coefficient);

            Configuration.RegisterObjectiveValueProvider((attendants, cleanings) =>
            {
                var distances = attendants.SelectMany(a => a.Cleanings.Take(a.Cleanings.Count - 1).Zip(a.Cleanings.Skip(1), GetLevelDistanceBetweenActivities)).Where(d => d > 0).ToArray();
                var distancesDescriptions = distances.GroupBy(d => d).OrderBy(g => g.Key).Select(g => $"{g.Count()} * {g.Key}");
                return (distances.Sum() * coefficient, $"({string.Join(" + ", distancesDescriptions)}) * {coefficient}");
            });
        }

        private int GetLevelDistanceBetweenActivities(TActivity[] activities, int fromActivity, int toActivity)
        {
            var originActivity = activities[fromActivity];
            var destinationActivity = activities[toActivity];
            return GetLevelDistanceBetweenActivities(originActivity, destinationActivity);
        }

        /// <example>
        /// activity A levels: Hotel 1, Floor 1.1, Section 1.1.1
        /// activity B levels: Hotel 1, Floor 1.1, Section 1.1.1
        /// => distance 0
        /// </example>
        /// <example>
        /// activity A levels: Hotel 1, Floor 1.1, Section 1.1.1
        /// activity B levels: Hotel 1, Floor 1.1, Section 1.1.2
        /// => distance 1
        /// </example>
        /// <example>
        /// activity A levels: Hotel 1, Floor 1.1, Section 1.1.1
        /// activity B levels: Hotel 1, Floor 1.2, Section 1.2.1
        /// => distance 2
        /// </example>
        /// <example>
        /// activity A levels: Hotel 1, Floor 1.1, Section 1.1.1
        /// activity B levels: Hotel 2, Floor 2.1, Section 2.1.1
        /// => distance 3
        /// </example>
        private int GetLevelDistanceBetweenActivities(Activity originActivity, Activity destinationActivity)
        {
            var commonLevels = originActivity.Levels
                .Zip(destinationActivity.Levels, (o, d) => new { o, d })
                .TakeWhile(p => p.o == p.d)
                .Count();
            return originActivity.Levels.Count - commonLevels;
        }
    }

    public class WorkerLevelsDimension<TWorker, TActivity> : DimensionBase where TWorker : Worker where TActivity : Activity
    {
        public WorkerLevelsDimension(ConstraintsConfiguration configuration, TWorker[] workers, TActivity[] activities,
            long coefficient)
            : base(configuration)
        {
            var evaluators = workers.Select(w => RegisterEvaluator((from, to) => GetAttendantLevelDistance(w, activities, to))).ToArray();
            RoutingModel.AddDimensionWithVehicleTransits(evaluators, Zero, BigNumber, true, DimensionName);
            Dimension.SetSpanCostCoefficientForAllVehicles(coefficient);

            Configuration.RegisterObjectiveValueProvider((attendants, cleanings) =>
            {
                var distances = attendants.SelectMany(a => a.Cleanings.Select(c => GetAttendantLevelDistance((TWorker)(object)a, (TActivity)(object)c))).Where(d => d > 0).ToArray();
                var distancesDescriptions = distances.GroupBy(d => d).OrderBy(g => g.Key).Select(g => $"{g.Count()} * {g.Key}");
                return (distances.Sum() * coefficient, $"({string.Join(" + ", distancesDescriptions)}) * {coefficient}");
            });
        }

        private int GetAttendantLevelDistance(TWorker worker, TActivity[] activities, int toNode)
        {
            var activityIndex = toNode - 1;
            //if (typeof(TActivity) == typeof(Hosting))
            //{
            //    activityIndex /= 2;
            //}

            if (activityIndex == activities.Length)
            {
                return 0;
            }

            var activity = activities[activityIndex];
            return GetAttendantLevelDistance(worker, activity);
        }

        /// <example>
        /// worker levels: Floor 1, Floor 2, Floor 3
        /// activity levels: Floor 1
        /// => distance 0
        /// </example>
        /// <example>
        /// worker levels: Floor 1, Floor 2, Floor 3
        /// activity levels: Floor 2
        /// => distance 1
        /// </example>
        /// <example>
        /// worker levels: Floor 1, Floor 2, Floor 3
        /// activity levels: Floor 3
        /// => distance 2
        /// </example>
        /// <example>
        /// worker levels: Floor 1, Floor 2, Floor 3
        /// activity levels: Floor 4
        /// => distance int.MaxValue
        /// </example>
        private int GetAttendantLevelDistance(TWorker worker, TActivity activity)
        {
            var levels = worker.CurrentTimeSlot.Levels;
            var distance = levels.TakeWhile(l => !activity.Levels.Contains(l)).Count();
            return levels.Any() && distance == levels.Count ? int.MaxValue : distance;
        }
    }

    public class CleaningDisjunctions : ConstraintsBase
    {
        public CleaningDisjunctions(ConstraintsConfiguration configuration, Cleaning[] cleanings, long coefficient)
            : base(configuration)
        {
            for (var i = 0; i < cleanings.Length; i++)
            {
                RoutingModel.AddDisjunction(new long[] { i + 1 }, GetCleaningPriorities(cleanings[i]).Sum(p => p.value) * coefficient);
            }

            Configuration.RegisterObjectiveValueProvider((attendants, cleanings1) =>
            {
                var planned = cleanings1.Where(c => c.Plan == null).ToArray();
                var priorities = planned.SelectMany(GetCleaningPriorities).ToArray();
                var value = priorities.Sum(p => p.value) * coefficient;
                var descriptions = priorities.GroupBy(p => p).OrderByDescending(g => g.Key.value).Select(g => $"{g.Count()} * {g.Key.value}({g.Key.description})");
                var description = $"({string.Join(" + ", descriptions)}) * {coefficient}";
                return (value, description);
            });
        }

        private IEnumerable<(int value, string description)> GetCleaningPriorities(Cleaning cleaning)
        {
            if (cleaning.ArrivalExpected) yield return (16, "Arrival");
            if (cleaning.Type == CleaningType.Departure) yield return (8, "Departure");
            if (cleaning.Type == CleaningType.Stay) yield return (4, "Stay");
            if (cleaning.Type == CleaningType.Vacant)
            {
                if (cleaning.Room.IsDirty)
                {
                    yield return (2, "Vacant Dirty");
                }
                else
                {
                    yield return (1, "Vacant Other");
                }
            }
            //if (cleaning.Type == CleaningType.VacantOther) ;
            if (cleaning.Tags?.Contains("VIP") ?? false) yield return (2, "VIP");
        }
    }

    public class CreditsLimitPerAttendant : DimensionBase
    {
        public CreditsLimitPerAttendant(ConstraintsConfiguration configuration, Cleaning[] cleanings, Attendant[] attendants)
            : base(configuration)
        {
            if (attendants.Any(a => a.CurrentTimeSlot.MaxCredits > 0))
            {
                var capacities = attendants.Select(a => (long)a.CurrentTimeSlot.MaxCredits).ToArray();
                var evaluator = RegisterEvaluator((from, to) => from > 0 ? cleanings[from - 1].Credits : 0);
                var zeroEvaluator = RegisterEvaluator((from, to) => 0);
                var evaluators = capacities.Select(c => c > 0 ? evaluator : zeroEvaluator).ToArray();
                RoutingModel.AddDimensionWithVehicleTransitAndCapacity(evaluators, 0, capacities, true, DimensionName);
            }
        }
    }

    public class CleaningsLimitPerAttendant : DimensionBase
    {
        protected override string DimensionName { get; }

        public CleaningsLimitPerAttendant(ConstraintsConfiguration configuration, Cleaning[] cleanings, int cleaningsLimitPerAttendant, params CleaningType[] limitedCleaningTypes)
            : base(configuration)
        {
            DimensionName = $"{string.Join("And", limitedCleaningTypes)}CleaningsLimitPerAttendant";
            if (cleaningsLimitPerAttendant > 0)
            {
                var hashset = limitedCleaningTypes.ToHashSet();
                var evaluator = limitedCleaningTypes.Any()
                    ? RegisterEvaluator((from, to) => from > 0 && hashset.Contains(cleanings[from - 1].Type) ? 1 : 0)
                    : RegisterEvaluator((from, to) => from > 0 ? 1 : 0);
                RoutingModel.AddDimension(evaluator, 0, cleaningsLimitPerAttendant, true, DimensionName);
            }
        }
    }

    public abstract class ConstraintsBase
    {
        protected const int Zero = 0;
        protected const int BigNumber = 1000000;
        public virtual ConstraintsConfiguration Configuration { get; }
        protected Google.OrTools.ConstraintSolver.RoutingModel RoutingModel { get; }
        protected Google.OrTools.ConstraintSolver.RoutingIndexManager RoutingIndexManager { get; }
        protected Google.OrTools.ConstraintSolver.Solver Solver => RoutingModel.solver();

        protected ConstraintsBase(ConstraintsConfiguration configuration) : this(configuration.RoutingModel, configuration.RoutingIndexManager)
        {
            Configuration = configuration;
        }

        protected ConstraintsBase(Google.OrTools.ConstraintSolver.RoutingModel routingModel, Google.OrTools.ConstraintSolver.RoutingIndexManager routingIndexManager)
        {
            RoutingModel = routingModel;
            RoutingIndexManager = routingIndexManager;
        }

        public TimeDimension<TActivity> AddTimeDimension<TActivity>(TActivity[] activities, GeoDistances distances) where TActivity : Activity
        {
            return new TimeDimension<TActivity>(Configuration, activities, distances);
        }

        public LevelsDimension<TActivity> AddLevelsDimension<TActivity>(TActivity[] activities, long coefficient) where TActivity : Activity
        {
            return new LevelsDimension<TActivity>(Configuration, activities, coefficient);
        }

        public WorkerLevelsDimension<TWorker, TActivity> AddWorkerLevelsDimension<TWorker, TActivity>(TWorker[] workers,
            TActivity[] activities, long coefficient) where TActivity : Activity where TWorker : Worker
        {
            return new WorkerLevelsDimension<TWorker, TActivity>(Configuration, workers, activities, coefficient);
        }

        public CleaningDisjunctions AddCleaningDisjunctions(Cleaning[] cleanings, long coefficient)
        {
            return new CleaningDisjunctions(Configuration, cleanings, coefficient);
        }

        public CreditsLimitPerAttendant AddCreditsLimitPerAttendant(Cleaning[] cleanings, Attendant[] attendants)
        {
            return new CreditsLimitPerAttendant(Configuration, cleanings, attendants);
        }

        public CleaningsLimitPerAttendant AddCleaningsLimitPerAttendant(Cleaning[] cleanings, int cleaningsLimitPerAttendant, params CleaningType[] limitedCleaningTypes)
        {
            return new CleaningsLimitPerAttendant(Configuration, cleanings, cleaningsLimitPerAttendant, limitedCleaningTypes);
        }

        //public HostingDisjunctions AddHostingDisjunctions(Hosting[] hostings)
        //{
        //    return new HostingDisjunctions(Configuration, hostings);
        //}

        //public RunningDisjunctions AddRunningDisjunctions(Running[] runnings)
        //{
        //    return new RunningDisjunctions(Configuration, runnings);
        //}

        public OrderByPriorityDimension AddOrderByPriority(Cleaning[] cleanings, long coefficient)
        {
            return new OrderByPriorityDimension(Configuration, cleanings, coefficient);
        }

        public OrderByPositionsDimension AddOrderByPosition(Cleaning[] cleanings, long coefficient)
        {
            return new OrderByPositionsDimension(Configuration, cleanings, coefficient);
        }

        public Balancing<TWorker> AddBalancing<TWorker>(TWorker[] workers, long coefficient) where TWorker : Worker
        {
            return new Balancing<TWorker>(Configuration, workers, coefficient);
        }

        public BalancingByCredits<TActivity> AddBalancingByCredits<TActivity>(TActivity[] activities) where TActivity : Activity
        {
            return new BalancingByCredits<TActivity>(Configuration, activities);
        }

        public Grouping<TWorker, TActivity> AddGrouping<TWorker, TActivity>(TWorker[] workers, TActivity[] activities) where TWorker : Worker where TActivity : Activity
        {
            return new Grouping<TWorker, TActivity>(Configuration, workers, activities);
        }

        public PreferredAttendants AddPreferredAttendants(Cleaning[] cleanings, Attendant[] attendants, long coefficient)
        {
            return new PreferredAttendants(Configuration, cleanings, attendants, coefficient);
        }

        public PlannedAttendants<TWorker, TActivity> AddPlannedAttendants<TWorker, TActivity>(TActivity[] activities, TWorker[] workers)
            where TWorker : Worker where TActivity : Activity
        {
            return new PlannedAttendants<TWorker, TActivity>(Configuration, activities, workers);
        }
    }

    public class OrderByPriorityDimension : DimensionBase
    {
        private readonly Cleaning[] _cleanings;

        public OrderByPriorityDimension(ConstraintsConfiguration configuration, Cleaning[] cleanings, long coefficient)
            : base(configuration)
        {
            _cleanings = cleanings;
            RoutingModel.AddDimension(RegisterActivitiesEvaluator(GetOrder), Zero, BigNumber, true, DimensionName);
            Dimension.SetSpanCostCoefficientForAllVehicles(coefficient);
        }

        private int GetOrder(int fromActivity, int toActivity)
        {
            int GetPriority(Cleaning cleaning)
            {
                int minutesInDay = (int)TimeSpan.FromDays(1).TotalMinutes;

                return cleaning.ArrivalExpected ? (int)cleaning.To.TimeOfDay.TotalMinutes                                   // Arrivals first
                    : cleaning.Type == CleaningType.Departure ? minutesInDay + (int)cleaning.From.TimeOfDay.TotalMinutes    // Departures after
                    : minutesInDay * 2;                                                                                     // And the rest
            }

            return GetPriority(_cleanings[fromActivity]) > GetPriority(_cleanings[toActivity]) ? 100 : 0;
        }
    }

    public class OrderByPositionsDimension : DimensionBase
    {
        private readonly Cleaning[] _cleanings;

        public OrderByPositionsDimension(ConstraintsConfiguration configuration, Cleaning[] cleanings, long coefficient)
            : base(configuration)
        {
            if (coefficient > 0)
            {
                _cleanings = cleanings;
                RoutingModel.AddDimension(RegisterActivitiesEvaluator(GetOrder), Zero, BigNumber, true, DimensionName);
                Dimension.SetSpanCostCoefficientForAllVehicles(coefficient);
            }
        }

        private int GetOrder(int fromActivity, int toActivity)
        {
            var from = _cleanings[fromActivity].Room;
            var to = _cleanings[toActivity].Room;

            var distance = Math.Abs(from.IndexOnFloor - to.IndexOnFloor);
            return from.Floor.FloorIndex == to.Floor.FloorIndex
                ? Math.Min(distance, from.Floor.RoomsCount - distance)
                : Math.Abs(from.Floor.FloorIndex - to.Floor.FloorIndex) * 10;
        }

    }

    public class Balancing<TWorker> : ConstraintsBase where TWorker : Worker
    {
        public Balancing(ConstraintsConfiguration configuration, TWorker[] workers, long coefficient)
            : base(configuration)
        {
            if (workers.Length > 1 && coefficient > 0) // In case of one attendant no need to balance the work && 0 => turn the constraint OFF
            {
                var linearCostCoefficient = 31 * coefficient;
                var quadraticCostCoefficient = -30 * coefficient;

                RoutingModel.SetAmortizedCostFactorsOfAllVehicles(linearCostCoefficient, quadraticCostCoefficient);

                Configuration.RegisterObjectiveValueProvider((attendants, cleanings) =>
                {
                    var activeAttendants = attendants.Where(a => a.Cleanings.Any()).ToArray();
                    var linearCost = linearCostCoefficient * activeAttendants.Length;
                    var cleaningCounts = activeAttendants.Select(a => a.Cleanings.Count).GroupBy(c => c).ToArray();
                    var quadraticCost = quadraticCostCoefficient * cleaningCounts.Sum(g => g.Key * g.Key * g.Count());
                    var countsDescription =
                        string.Join(" + ", cleaningCounts.Select(g => $"{g.Count()} * ({g.Key}c)^2"));
                    return (linearCost - quadraticCost,
                        $"{activeAttendants.Length}a * {linearCostCoefficient} - ({countsDescription}) * ({quadraticCostCoefficient})");
                });
            }
        }
    }

    public abstract class DimensionBase : ConstraintsBase
    {
        protected virtual string DimensionName => GetType().Name.Replace("Dimension", string.Empty).Replace("`1", string.Empty);
        public Google.OrTools.ConstraintSolver.RoutingDimension Dimension => RoutingModel.GetDimensionOrDie(DimensionName);

        protected DimensionBase(ConstraintsConfiguration configuration)
            : base(configuration) { }

        protected int RegisterEvaluator(CallbackNodeEvaluator.GetDistance callback)
        {
            return Configuration.Register(RoutingModel, RoutingIndexManager, callback, false, false);
        }

        protected int RegisterActivitiesEvaluator(CallbackNodeEvaluator.GetDistance callback)
        {
            return Configuration.Register(RoutingModel, RoutingIndexManager, callback);
        }

        protected int RegisterActivitiesEvaluator(CallbackNodeEvaluator.GetValue callback)
        {
            return Configuration.Register(RoutingModel, RoutingIndexManager, (from, to) => callback(from), excludeEnd: false);
        }
    }

    public class BalancingByCredits<TActivity> : ConstraintsBase where TActivity : Activity
    {
        private const int CreditsBalanceCoefficient = 1;

        public BalancingByCredits(ConstraintsConfiguration configuration, TActivity[] activities)
            : base(configuration)
        {
            var creditsDimension = new CreditsDimension(Configuration, activities);
            var creditsBalanceDimension = new CreditsBalanceDimension(Configuration);
            creditsBalanceDimension.Dimension.SetSpanCostCoefficientForAllVehicles(CreditsBalanceCoefficient);
            for (var i = 0; i < activities.Length; i++)
            {
                Solver.Add(creditsBalanceDimension.Dimension.CumulVar(i + 1) == Solver.MakeSquare(creditsDimension.Dimension.CumulVar(i + 1)));
            }
        }

        private class CreditsDimension : DimensionBase
        {
            private readonly TActivity[] _activities;

            public CreditsDimension(ConstraintsConfiguration configuration, TActivity[] activities)
                : base(configuration)
            {
                _activities = activities;
                RoutingModel.AddDimension(RegisterActivitiesEvaluator(GetCredits), Zero, BigNumber, true, DimensionName);
            }

            private int GetCredits(int fromActivity, int toActivity)
            {
                return _activities[toActivity].Credits;
            }
        }

        private class CreditsBalanceDimension : DimensionBase
        {
            public CreditsBalanceDimension(ConstraintsConfiguration configuration)
                : base(configuration)
            {
                RoutingModel.AddConstantDimensionWithSlack(Zero, BigNumber, BigNumber, true, DimensionName);
            }
        }
    }

    public class Grouping<TWorker, TActivity> : ConstraintsBase where TWorker : Worker where TActivity : Activity
    {
        public Grouping(ConstraintsConfiguration configuration, TWorker[] workers, TActivity[] activities)
            : base(configuration)
        {
            for (int i = 0; i < activities.Length; i++)
            {
                for (int j = 0; j < workers.Length; j++)
                {
                    if (activities[i].Room.GroupName != workers[j].GroupName)
                    {
                        RoutingModel.VehicleVar(i + 1).RemoveValue(j);
                    }
                }
            }
        }
    }

    public class PlannedAttendants<TWorker, TActivity> : ConstraintsBase where TWorker : Worker where TActivity : Activity
    {
        public PlannedAttendants(ConstraintsConfiguration configuration, TActivity[] activities, TWorker[] workers)
            : base(configuration)
        {
            for (int i = 0; i < activities.Length; i++)
            {
                var plan = activities[i].GetPlan();
                if (plan == null)
                {
                    continue;
                }
                for (int j = 0; j < workers.Length; j++)
                {
                    if (plan.WorkerUsername != workers[j].Username)
                    {
                        RoutingModel.VehicleVar(i + 1).RemoveValue(j);
                    }
                }
            }
        }
    }

    public class PreferredAttendants : DimensionBase
    {
        public PreferredAttendants(ConstraintsConfiguration configuration, Cleaning[] cleanings, Attendant[] attendants, long coefficient) : base(configuration)
        {
            if (coefficient > 0 && attendants.Any(a => a.CurrentTimeSlot.IsPreferred))
            {
                var evaluator = RegisterEvaluator((from, to) => from > 0 ? cleanings[from - 1].Credits : 0);
                var zeroEvaluator = RegisterEvaluator((from, to) => 0);
                var evaluators = attendants.Select(a => a.CurrentTimeSlot.IsPreferred ? zeroEvaluator : evaluator).ToArray();
                RoutingModel.AddDimensionWithVehicleTransits(evaluators, 0, BigNumber, true, DimensionName);
                Dimension.SetSpanCostCoefficientForAllVehicles(coefficient);

                Configuration.RegisterObjectiveValueProvider((attendants1, cleanings1) =>
                {
                    var values = attendants1.Where(a => !a.CurrentTimeSlot.IsPreferred).Select(a => a.Cleanings.Sum(c => c.Credits)).ToArray();
                    var value = values.Sum() * coefficient;
                    var description = $"({string.Join(" + ", values)}) * {coefficient}";
                    return (value, description);
                });
            }
        }
    }

}
