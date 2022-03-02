using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetPageOfTaskConfigurationsForGrid
{
    public class TaskConfigurationGridItem
    {
        public Guid Id { get; set; }
        public IEnumerable<TaskConfigurationGridWhat> Whats { get; set; }
        public IEnumerable<TaskConfigurationGridWho> Whos { get; set; }
        public IEnumerable<TaskConfigurationGridWhere> Wheres { get; set; }
        public string TypeKey { get; set; }
        public TaskConfigurationGridProgress Progress { get; set; }
        public bool IsHighPriority { get; set; }
        public bool IsGuestRequest { get; set; }
        public bool MustBeCompletedByEveryone { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string TaskTypeDescription { get; set; }
        public string TaskTimeDescription { get; set; }
        public string TaskRepeatsForDescription { get; set; }

    }

    public class TaskConfigurationGridWhat
    {
        public int AssetQuantity { get; set; }
        public string ActionName { get; set; }
        public string AssetName { get; set; }
    }
    public class TaskConfigurationGridWho
    {
        public string Id { get; set; }
        public string TypeKey { get; set; }
        public string Name { get; set; }
        public string TypeDescription { get; set; }
    }
    public class TaskConfigurationGridWhere
    {
        public string Id { get; set; }
        public string TypeKey { get; set; }
        public string Name { get; set; }
        public string TypeDescription { get; set; }
    }
    public class TaskConfigurationGridProgress
    {
        /// <summary>
        /// Used only when the task configuration yielded a single task.
        /// The value really contains that single task status key.
        /// </summary>
        public string StatusKey { get; set; }

        /// <summary>
        /// Used only when the task configuration yielded a single task.
        /// The value really contains that single task status key description.
        /// </summary>
        public string StatusDescription { get; set; }

        public int NumberOfTasks { get; set; }
        public int NumberOfCompletedTasks { get; set; }
        public decimal CompletionFactor { get; set; }
    }
    public class TaskCounts
    {
        public int NumberOfTasks { get; set; }
        public int NumberOfPendingTasks { get; set; }
        public int NumberOfWaitingTasks { get; set; }
        public int NumberOfStartedTasks { get; set; }
        public int NumberOfPausedTasks { get; set; }
        public int NumberOfFinishedTasks { get; set; }
        public int NumberOfVerifiedTasks { get; set; }
        public int NumberOfCancelledTasks { get; set; }
        public int NumberOfClaimedBySomeoneElseTasks { get; set; }

        public decimal CompletionFactor { get; set; }
        public string CompletionPercentString { get; set; }
        public string CompletionStatus { get; set; }
        public decimal VerificationFactor { get; set; }
        public string VerificationPercentString { get; set; }
        public string VerificationStatus { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class GetPageOfTaskConfigurationsForGridQuery : IRequest<PageOf<TaskConfigurationGridItem>>
    {

        public string SortKey { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }

        public string ActionName { get; set; }
        public Guid? AssetId { get; set; }
        public Guid? AssetGroupId { get; set; }
        public TaskWhoData[] Whos { get; set; }
        public TaskWhereData[] Wheres { get; set; }

        public Guid? UserGroupId { get; set; }
        public Guid? UserSubGroupId { get; set; }
    }

    public class GetPageOfTaskConfigurationsForGridQueryHandler : IRequestHandler<GetPageOfTaskConfigurationsForGridQuery, PageOf<TaskConfigurationGridItem>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext _databaseContext;
        private readonly Guid _userId;

        public GetPageOfTaskConfigurationsForGridQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
        {
            this._databaseContext = databaseContext;
            this._userId = contextAccessor.UserId();
        }
        public async Task<PageOf<TaskConfigurationGridItem>> Handle(GetPageOfTaskConfigurationsForGridQuery request, CancellationToken cancellationToken)
        {
            var query = this._databaseContext
                .SystemTaskConfigurations
                .Include(stc => stc.Tasks)
                .AsQueryable();

            if (request.AssetId.HasValue && request.AssetGroupId.HasValue)
            {
                query = query.Where(t => t.Tasks.Any(ta => ta.Actions.Any(a => a.AssetId == request.AssetId.Value && a.ActionName == request.ActionName && a.AssetGroupId == request.AssetGroupId.Value)));
            }
            else if (request.AssetGroupId.HasValue && !request.AssetId.HasValue)
            {
                query = query.Where(t => t.Tasks.Any(ta => ta.Actions.Any(a => a.AssetGroupId == request.AssetGroupId.Value && a.ActionName == request.ActionName)));
            }
            else if (request.AssetId.HasValue && !request.AssetGroupId.HasValue)
            {
                query = query.Where(t => t.Tasks.Any(ta => ta.Actions.Any(a => a.AssetId == request.AssetId.Value && a.ActionName == request.ActionName)));
            }

            if (request.Whos != null && request.Whos.Any())
            {
                var userIds = new List<Guid?>();
                var userGroupIds = new List<Guid>();
                var userSubGroupIds = new List<Guid>();
                foreach (var who in request.Whos)
                {
                    switch (who.TypeKey)
                    {
                        case nameof(TaskWhoType.GROUP):
                            userGroupIds.Add(new Guid(who.ReferenceId));
                            break;
                        case nameof(TaskWhoType.SUBGROUP):
                            userSubGroupIds.Add(new Guid(who.ReferenceId));
                            break;
                        case nameof(TaskWhoType.USER):
                            userIds.Add(new Guid(who.ReferenceId));
                            break;
                    }
                }

                if (userIds.Any())
                {
                    query = query.Where(t => t.Tasks.Any(ta => ta.UserId != null && userIds.Contains(ta.UserId)));
                }

                if (userGroupIds.Any())
                {
                    query = query.Where(t => t.Tasks.Any(ta => ta.UserId != null && ta.User.UserGroupId != null && userGroupIds.Contains(ta.User.UserGroupId.Value)));
                }

                if (userSubGroupIds.Any())
                {
                    query = query.Where(t => t.Tasks.Any(ta => ta.UserId != null && ta.User.UserSubGroupId != null && userSubGroupIds.Contains(ta.User.UserSubGroupId.Value)));
                }
            }

            if (request.UserGroupId.HasValue)
            {
                query = query.Where(t => t.Tasks.Any(ta => ta.UserId != null && ta.User.UserGroupId != null && ta.User.UserGroupId.Value == request.UserGroupId.Value));
            }

            if (request.UserSubGroupId.HasValue)
            {
                query = query.Where(t => t.Tasks.Any(ta => ta.UserId != null && ta.User.UserSubGroupId != null && ta.User.UserSubGroupId.Value == request.UserSubGroupId.Value));
            }

            if (request.Wheres != null && request.Wheres.Any())
            {
                var roomIds = new List<Guid>();
                var floorIds = new List<Guid>();
                var buildingIds = new List<Guid>();
                var hotelIds = new List<string>();
                var reservationIds = new List<string>();
                var warehouseIds = new List<Guid>();
                foreach (var where in request.Wheres)
                {
                    switch (where.TypeKey)
                    {
                        case nameof(TaskWhereType.BUILDING):
                            buildingIds.Add(new Guid(where.ReferenceId));
                            break;
                        case nameof(TaskWhereType.FLOOR):
                            floorIds.Add(new Guid(where.ReferenceId));
                            break;
                        case nameof(TaskWhereType.HOTEL):
                            hotelIds.Add(where.ReferenceId);
                            break;
                        case nameof(TaskWhereType.RESERVATION):
                            reservationIds.Add(where.ReferenceId);
                            break;
                        case nameof(TaskWhereType.ROOM):
                            roomIds.Add(new Guid(where.ReferenceId));
                            break;
                        case nameof(TaskWhereType.WAREHOUSE):
                            warehouseIds.Add(new Guid(where.ReferenceId));
                            break;
                    }
                }

                if (roomIds.Any())
                {
                    query = query.Where(c => c.Tasks.Any(t => (t.FromRoomId != null && roomIds.Contains(t.FromRoomId.Value)) || (t.ToRoomId != null && roomIds.Contains(t.ToRoomId.Value))));
                }

                if (hotelIds.Any())
                {
                    query = query.Where(c => c.Tasks.Any(t => (t.FromHotelId != null && hotelIds.Contains(t.FromHotelId)) || (t.ToHotelId != null && hotelIds.Contains(t.ToHotelId))));
                }

                if (reservationIds.Any())
                {
                    query = query.Where(c => c.Tasks.Any(t => (t.FromReservationId != null && reservationIds.Contains(t.FromReservationId)) || (t.ToReservationId != null && reservationIds.Contains(t.ToReservationId))));
                }

                if (warehouseIds.Any())
                {
                    query = query.Where(c => c.Tasks.Any(t => (t.FromWarehouseId != null && warehouseIds.Contains(t.FromWarehouseId.Value)) || (t.ToWarehouseId != null && warehouseIds.Contains(t.ToWarehouseId.Value))));
                }
            }

            switch (request.SortKey)
            {
                case "CREATED_AT_ASC":
                    query = query.OrderBy(tc => tc.CreatedAt);
                    break;
                case "CREATED_AT_DESC":
                default:
                    query = query.OrderByDescending(tc => tc.CreatedAt);
                    break;
            }

            var totalNumberOfTaskConfigurations = await query.CountAsync();

            if (request.Skip.HasValue)
            {
                query = query.Skip(request.Skip.Value);
            }

            if (request.Take.HasValue)
            {
                query = query.Take(request.Take.Value);
            }

            var taskConfigurations = await query.ToListAsync();
            var taskConfigurationIds = taskConfigurations.Select(tc => tc.Id).ToArray();
            var gridItems = new List<TaskConfigurationGridItem>();

            foreach (var taskConfiguration in taskConfigurations)
            {
                var taskTypeDescription = "";
                var taskTimeDescription = "";
                var taskRepeatsForDescription = "";
                switch (taskConfiguration.Data.TaskTypeKey)
                {
                    case nameof(TaskType.BALANCED):
                        // Balanced over period "" ""
                        taskTypeDescription = "Balanced tasks over period of time";
                        taskTimeDescription = $"From {taskConfiguration.Data.StartsAtTimes.FirstOrDefault().ToString("dd.MM.yyyy. HH:mm")} until {taskConfiguration.Data.EndsAtTime?.ToString("dd.MM.yyyy. HH:MM") ?? "?"}";
                        taskRepeatsForDescription = null;
                        break;
                    case nameof(TaskType.EVENT):
                        // On {event name}
                        taskTypeDescription = "On event tasks";
                        taskTimeDescription = $"On {taskConfiguration.Data.EventKey}";
                        taskRepeatsForDescription = null;
                        break;
                    case nameof(TaskType.RECURRING):

                        switch (taskConfiguration.Data.RecurringTaskTypeKey)
                        {
                            case nameof(RecurringTaskType.DAILY):
                                // From 13.03.2020. 14:30, recurring daily at 09:00, 10:00, 3 times
                                var dailyStartsAtDescription = $"{taskConfiguration.Data.StartsAtTimes.FirstOrDefault().ToString("dd.MM.yyyy. HH:mm")}";
                                var dailyAtTimesDescription = string.Join(", ", taskConfiguration.Data.RecurringTaskRepeatTimes.Where(rt => rt.Key == "daily").Select(rt => rt.RepeatTimes).FirstOrDefault());
                                var dailyRepeatsForDescription = this._GetRepeatsForDescription(taskConfiguration.Data.RepeatsForKey, taskConfiguration.Data.RepeatsForNrDays, taskConfiguration.Data.RepeatsForNrOccurences, taskConfiguration.Data.RepeatsUntilTime);

                                taskTypeDescription = "Daily tasks";
                                taskTimeDescription = $"At times {dailyAtTimesDescription}, from {dailyStartsAtDescription}";
                                taskRepeatsForDescription = dailyRepeatsForDescription;
                                break;
                            case nameof(RecurringTaskType.EVERY):
                                // From 13.03.2020. 14:30, recurring every 6 days, 3 times
                                var everyStartsAtDescription = $"{taskConfiguration.Data.StartsAtTimes.FirstOrDefault().ToString("dd.MM.yyyy. HH:mm")}";
                                var everyRepeatsForDescription = this._GetRepeatsForDescription(taskConfiguration.Data.RepeatsForKey, taskConfiguration.Data.RepeatsForNrDays, taskConfiguration.Data.RepeatsForNrOccurences, taskConfiguration.Data.RepeatsUntilTime);

                                taskTypeDescription = "Periodical tasks";
                                taskTimeDescription = $"Every {taskConfiguration.Data.RecurringEveryNumberOfDays} days from {everyStartsAtDescription}";
                                taskRepeatsForDescription = everyRepeatsForDescription;
                                break;
                            case nameof(RecurringTaskType.MONTHLY):
                                // From 13.03.2020. 14:30, recurring monthly, every 1 at 08:00, 14 at 09:00, 21 at 10:00, until 13.04.2020. 13:00
                                var monthlyStartsAtDescription = $"{taskConfiguration.Data.StartsAtTimes.FirstOrDefault().ToString("dd.MM.yyyy. HH:mm")}";
                                var monthlyTimesDescription = string.Join(", ", taskConfiguration.Data.RecurringTaskRepeatTimes.Select(rt => $"{rt.Key} at {rt.RepeatTimes.FirstOrDefault() ?? "?"})").ToArray());
                                var monthlyRepeatsForDescription = this._GetRepeatsForDescription(taskConfiguration.Data.RepeatsForKey, taskConfiguration.Data.RepeatsForNrDays, taskConfiguration.Data.RepeatsForNrOccurences, taskConfiguration.Data.RepeatsUntilTime);

                                taskTypeDescription = "Monthly tasks";
                                taskTimeDescription = $"Monthly every {monthlyTimesDescription}, from {monthlyStartsAtDescription}";
                                taskRepeatsForDescription = monthlyRepeatsForDescription;
                                break;
                            case nameof(RecurringTaskType.SPECIFIC_TIME):
                                // Recurring at specific times, 13.03.2020. 10:30, 14.02.2020. 18:45
                                var specificTimes = taskConfiguration.Data.StartsAtTimes.OrderBy(t => t).Select(t => t.ToString("dd.MM.yyyy. HH:mm")).ToArray();

                                taskTypeDescription = "Specific time tasks";
                                taskTimeDescription = string.Join(", ", specificTimes);
                                taskRepeatsForDescription = null;
                                break;
                            case nameof(RecurringTaskType.WEEKLY):
                                // From 13.03.2020. 14:30, recurring weekly, MON at 09:00, 10:00, WED at 14:00, SAT at 20:00, for 3 days
                                var weeklyStartsAtDescription = $"{taskConfiguration.Data.StartsAtTimes.FirstOrDefault().ToString("dd.MM.yyyy. HH:mm")}";
                                var weeklyTimesDescription = string.Join(", ", taskConfiguration.Data.RecurringTaskRepeatTimes.Select(rt => $"{rt.Key} at {string.Join(", ", rt.RepeatTimes)})").ToArray());
                                var weeklyRepeatsForDescription = this._GetRepeatsForDescription(taskConfiguration.Data.RepeatsForKey, taskConfiguration.Data.RepeatsForNrDays, taskConfiguration.Data.RepeatsForNrOccurences, taskConfiguration.Data.RepeatsUntilTime);

                                taskTypeDescription = "Weekly tasks";
                                taskTimeDescription = $"Weekly at {weeklyTimesDescription}, from {weeklyStartsAtDescription}";
                                taskRepeatsForDescription = weeklyRepeatsForDescription;
                                break;
                        }
                        break;
                    case nameof(TaskType.SINGLE):
                        taskTypeDescription = "";//"Single task(s)";
                        taskTimeDescription = taskConfiguration.Data.StartsAtTimes.FirstOrDefault().ToString("dd.MM.yyyy. HH:mm");
                        taskRepeatsForDescription = null;
                        break;
                }

                var progress = new TaskConfigurationGridProgress();
                if (taskConfiguration.Tasks.Any())
                {
                    if(taskConfiguration.Tasks.Count() == 1)
                    {
                        var task = taskConfiguration.Tasks.First();
                        progress.StatusKey = task.StatusKey;
                        progress.StatusDescription = this._GetTaskStatusDescription(task.StatusKey);
                        progress.NumberOfTasks = 1;
                        progress.NumberOfCompletedTasks = (new HashSet<string> { nameof(TaskStatusType.CLAIMED_BY_SOMEONE_ELSE), nameof(TaskStatusType.FINISHED), nameof(TaskStatusType.CANCELLED), nameof(TaskStatusType.VERIFIED) }).Contains(progress.StatusKey) ? 1 : 0;
                        progress.CompletionFactor = progress.NumberOfCompletedTasks == 1 ? 1m : 0m;
                    }
                    else
                    {
                        var counts = this._GetNumberOfTaskCounts(taskConfiguration.Tasks);
                        progress.StatusKey = "";
                        progress.StatusDescription = "";
                        progress.NumberOfTasks = counts.NumberOfTasks;
                        progress.NumberOfCompletedTasks = counts.NumberOfFinishedTasks + counts.NumberOfVerifiedTasks + counts.NumberOfCancelledTasks + counts.NumberOfClaimedBySomeoneElseTasks;
                        progress.CompletionFactor = counts.CompletionFactor;
                    }
                }
                else
                {
                    progress.StatusKey = "";
                    progress.StatusDescription = "";
                    progress.NumberOfTasks = 0;
                    progress.NumberOfCompletedTasks = 0;
                    progress.CompletionFactor = 0m;
                }

                var c = new TaskConfigurationGridItem
                {
                    Comment = taskConfiguration.Data.Comment,
                    CreatedAt = taskConfiguration.CreatedAt,
                    Id = taskConfiguration.Id,
                    IsGuestRequest = taskConfiguration.Data.IsGuestRequest,
                    IsHighPriority = taskConfiguration.Data.PriorityKey == "HIGH",
                    MustBeCompletedByEveryone = taskConfiguration.Data.MustBeFinishedByAllWhos,
                    Progress = progress,
                    TaskRepeatsForDescription = taskRepeatsForDescription,
                    TaskTimeDescription = taskTimeDescription,
                    TaskTypeDescription = taskTypeDescription,
                    TypeKey = taskConfiguration.Data.TaskTypeKey,
                    Whats = taskConfiguration.Data.Whats.Select(w => new TaskConfigurationGridWhat { ActionName = w.ActionName, AssetName = w.AssetName, AssetQuantity = w.AssetQuantity }).ToArray(),
                    Wheres = taskConfiguration.Data.Wheres.Select(w => new TaskConfigurationGridWhere { Id = w.ReferenceId, Name = w.ReferenceName, TypeDescription = w.TypeDescription, TypeKey = w.TypeKey }).ToArray(),
                    Whos = taskConfiguration.Data.Whos.Select(w => new TaskConfigurationGridWho { Id = w.ReferenceId, Name = w.ReferenceName, TypeDescription = w.TypeDescription, TypeKey = w.TypeKey }).ToArray(),
                };
                gridItems.Add(c);
            }

            return new PageOf<TaskConfigurationGridItem>
            {
                Items = gridItems,
                TotalNumberOfItems = totalNumberOfTaskConfigurations,
            };
        }

        private string _GetRepeatsForDescription(string repeatsForKey, int? nrDays, int? nrOccurences, DateTime? untilDate)
        {
            switch (repeatsForKey)
            {
                case nameof(RepeatsForType.NUMBER_OF_DAYS):
                    return $"Repeats for {nrDays?.ToString() ?? "UNKNOWN"} days";
                case nameof(RepeatsForType.NUMBER_OF_OCCURENCES):
                    return $"Repeats {nrOccurences?.ToString() ?? "UNKNOWN"} times";
                case nameof(RepeatsForType.SPECIFIC_DATE):
                    return $"Repeats until {untilDate?.ToString("dd.MM.yyyy. HH:mm") ?? "UNKNOWN date"}";
                default:
                    return "";
            }
        }

        private string _GetTaskStatusDescription(string key)
        {
            switch (key)
            {
                case nameof(TaskStatusType.CANCELLED):
                    return "Cancelled";
                case nameof(TaskStatusType.CLAIMED):
                    return "Claimed";
                case nameof(TaskStatusType.CLAIMED_BY_SOMEONE_ELSE):
                    return "Claimed by someone else";
                case nameof(TaskStatusType.FINISHED):
                    return "Finished";
                case nameof(TaskStatusType.PAUSED):
                    return "Paused";
                case nameof(TaskStatusType.PENDING):
                    return "Pending";
                case nameof(TaskStatusType.REJECTED):
                    return "Rejected";
                case nameof(TaskStatusType.STARTED):
                    return "Started";
                case nameof(TaskStatusType.VERIFIED):
                    return "Verified";
                case nameof(TaskStatusType.WAITING):
                    return "Waiting";
                case nameof(TaskStatusType.UNKNOWN):
                default:
                    return "Unknown";
            }
        }

        private TaskCounts _GetNumberOfTaskCounts(IEnumerable<Planner.Domain.Entities.SystemTask> tasks)
        {
            var counts = new TaskCounts();

            foreach(var t in tasks)
            {
                counts.NumberOfTasks++;
                switch (t.StatusKey)
                {
                    case nameof(TaskStatusType.PENDING):
                        counts.NumberOfPendingTasks++;
                        break;
                    case nameof(TaskStatusType.CANCELLED):
                        counts.NumberOfCancelledTasks++;
                        break;
                    case nameof(TaskStatusType.FINISHED):
                        counts.NumberOfFinishedTasks++;
                        break;
                    case nameof(TaskStatusType.PAUSED):
                        counts.NumberOfPausedTasks++;
                        break;
                    case nameof(TaskStatusType.STARTED):
                        counts.NumberOfStartedTasks++;
                        break;
                    case nameof(TaskStatusType.VERIFIED):
                        counts.NumberOfVerifiedTasks++;
                        break;
                    case nameof(TaskStatusType.WAITING):
                        counts.NumberOfWaitingTasks++;
                        break;
                    case nameof(TaskStatusType.CLAIMED_BY_SOMEONE_ELSE):
                        counts.NumberOfClaimedBySomeoneElseTasks++;
                        break;
                }
            }

            counts.CompletionFactor = counts.NumberOfTasks == 0 ? 0 : ((counts.NumberOfFinishedTasks + counts.NumberOfVerifiedTasks + counts.NumberOfCancelledTasks + counts.NumberOfClaimedBySomeoneElseTasks) / (decimal)counts.NumberOfTasks);

            var nrActiveTasks = counts.NumberOfTasks - counts.NumberOfCancelledTasks - counts.NumberOfClaimedBySomeoneElseTasks;
            counts.VerificationFactor = nrActiveTasks == 0 ? 0 : (counts.NumberOfVerifiedTasks) / (decimal)(nrActiveTasks);

            counts.CompletionPercentString = String.Format("{0:P0}", counts.CompletionFactor);

            if (counts.CompletionFactor < 0.5m)
                counts.CompletionStatus = "STARTING";
            else if (counts.CompletionFactor < 0.8m)
                counts.CompletionStatus = "IN_PROGRESS";
            else if (counts.CompletionFactor < 1m)
                counts.CompletionStatus = "ALMOST_COMPLETE";
            else
                counts.CompletionStatus = "COMPLETE";

            counts.VerificationPercentString = String.Format("{0:P0}", counts.VerificationFactor);

            if (counts.VerificationFactor < 0.5m)
                counts.VerificationStatus = "STARTING";
            else if (counts.VerificationFactor < 0.8m)
                counts.VerificationStatus = "IN_PROGRESS";
            else if (counts.VerificationFactor < 1m)
                counts.VerificationStatus = "ALMOST_COMPLETE";
            else
                counts.VerificationStatus = "COMPLETE";

            counts.IsCompleted = counts.CompletionFactor == 1m;


            return counts;
        }
    }
}
