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

namespace Planner.Application.TaskManagement.Queries.GetCalendarTasks
{
    public class TasksCalendar
    {
        public DateTime FirstDateOfCalendar { get; set; }
        public List<TasksCalendarMonth> Months { get; set; }
    }

    public class TasksCalendarMonth
    { 
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public List<TasksCalendarDay> Days { get; set; }
    }

    public class TasksCalendarDay
    { 
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public bool IsToday { get; set; }
        public List<TasksCalendarTask> Tasks { get; set; }
        public bool AreSomeTasksHidden { get; set; }
        public int NumberOfHiddenTasks { get; set; }
    }

    public class TasksCalendarTask
    {
        public bool IsVisible { get; set; }
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public List<TasksCalendarTaskAction> Actions { get; set; }
    }

    public class TasksCalendarTaskAction
    {
        public int AssetQuantity { get; set; }
        public string AssetName { get; set; }
        public string ActionName { get; set; }

        public bool ShowMoreActions { get; set; }
        public int NumberOfMoreActions { get; set; }
    }


    public class GetCalendarTasksQuery: IRequest<TasksCalendar>
    {
        public DateTime CurrentDate { get; set; }
        public int MonthFrom { get; set; }
        public int YearFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
        public bool OnlyMyTasks { get; set; }
        public Guid? UserGroupId { get; set; }
        public Guid? UserSubGroupId { get; set; }
    }

    public class GetCalendarTasksQueryHandler: IRequestHandler<GetCalendarTasksQuery, TasksCalendar>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext _databaseContext;
        private readonly Guid _userId;

        public GetCalendarTasksQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
        {
            this._databaseContext = databaseContext;
            this._userId = contextAccessor.UserId();
        }

        public async Task<TasksCalendar> Handle(GetCalendarTasksQuery request, CancellationToken cancellationToken)
        {
            var firstOfMonth = new DateTime(request.YearFrom, request.MonthFrom, 1, 0, 0, 0, DateTimeKind.Unspecified);
            var firstMonday = DateTimeHelper.GetMonday(firstOfMonth);

            var currentMonth = request.MonthFrom;
            var currentYear = request.YearFrom;

            var fromDate = firstMonday;
            var toDate = new DateTime(request.YearTo, request.MonthTo, 1, 0, 0, 0, DateTimeKind.Unspecified).AddMonths(1);
            var dayAfterLastSunday = DateTimeHelper.GetNextSunday(DateTimeHelper.GetLastDayOfMonth(request.YearTo, request.MonthTo)).AddDays(1);

            var tasksQuery = this._databaseContext.SystemTasks
                .Include(st => st.User)
                .Include(st => st.Actions)
                .Where(st => st.StartsAt >= firstMonday && st.StartsAt < dayAfterLastSunday).AsQueryable();

            if (request.OnlyMyTasks)
            {
                tasksQuery = tasksQuery.Where(st => st.UserId != null && st.UserId == this._userId);
            }

            if (request.UserGroupId.HasValue)
            {
                tasksQuery = tasksQuery.Where(st => st.UserId != null && st.User.UserGroupId != null && st.User.UserGroupId.Value == request.UserGroupId.Value);
            }
            else if (request.UserSubGroupId.HasValue)
            {
                tasksQuery = tasksQuery.Where(st => st.User.UserSubGroupId != null && st.User.UserSubGroupId.Value == request.UserSubGroupId.Value);
            }

            var tasksMap = (await tasksQuery.ToListAsync()).GroupBy(t => t.StartsAt.ToString("yyyy-MM-dd")).ToDictionary(group => group.Key, group => group.OrderBy(t => t.StartsAt).ThenBy(t => t.CreatedAt).ToList());

            var calendar = new TasksCalendar 
            {
                FirstDateOfCalendar = firstMonday,
                Months = new List<TasksCalendarMonth>(),
            };

            var currentDate = firstMonday;

            var numberOfVisibleTasksTreshold = 5;

            while((currentYear == request.YearTo && currentMonth <= request.MonthTo) || currentYear < request.YearTo)
            {
                var month = new TasksCalendarMonth
                {
                    Days = new List<TasksCalendarDay>(),
                    Month = currentMonth,
                    Year = currentYear,
                    MonthName = new DateTime(currentYear, currentMonth, 1).ToString("MMMM"),
                };

                calendar.Months.Add(month);

                for(int weekIndex = 0; weekIndex < 7; weekIndex++)
                {
                    for(int dayIndex = 0; dayIndex < 7; dayIndex++)
                    {
                        var dateKey = currentDate.ToString("yyyy-MM-dd");
                        var dayTasks = tasksMap.ContainsKey(dateKey) ? tasksMap[dateKey] : new List<Domain.Entities.SystemTask>();

                        var day = new TasksCalendarDay()
                        {
                            Date = currentDate,
                            Day = currentDate.Day,
                            IsToday = currentDate.Date == request.CurrentDate.Date,
                            IsActive = currentDate.Month == currentMonth,
                            Tasks = dayTasks.Select(dt => {
                                var actions = new List<TasksCalendarTaskAction>();
                                var numberOfActions = dt.Actions.Count();

                                if (numberOfActions != 0)
                                {
                                    var firstAction = dt.Actions.First();

                                    actions.Add(new TasksCalendarTaskAction 
                                    { 
                                        ActionName = firstAction.ActionName,
                                        AssetName = firstAction.AssetName,
                                        AssetQuantity = firstAction.AssetQuantity,
                                        ShowMoreActions = numberOfActions > 1,
                                        NumberOfMoreActions = numberOfActions - 1
                                    });
                                }

                                return new TasksCalendarTask
                                {
                                    Id = dt.Id,
                                    Actions = actions,
                                    IsVisible = false,
                                    UserId = dt.UserId,
                                    UserName = dt.User != null ? $"{dt.User.FirstName} {dt.User.LastName}" : (dt.IsForPlannedAttendant ? "Planned attendant" : "N/A"),
                                };
                            }).ToList(),
                            AreSomeTasksHidden = false,
                            NumberOfHiddenTasks = 0,
                        };
                        
                        if(day.Tasks.Count > numberOfVisibleTasksTreshold)
                        {
                            day.AreSomeTasksHidden = true;
                            day.NumberOfHiddenTasks = day.Tasks.Count - numberOfVisibleTasksTreshold + 1;

                            for(int i = 0; i < numberOfVisibleTasksTreshold - 1; i++)
                            {
                                day.Tasks[i].IsVisible = true;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < day.Tasks.Count; i++)
                            {
                                day.Tasks[i].IsVisible = true;
                            }
                        }

                        month.Days.Add(day);

                        currentDate = currentDate.AddDays(1);

                    }

                    if ((currentDate.Year == currentYear && currentDate.Month > currentMonth) || (currentDate.Year > currentYear))
                    {
                        if (currentDate.Day > 1)
                        {
                            currentDate = currentDate.AddDays(-7);
                        }
                        break;
                    }
                }


                currentMonth++;
                //currentDate = DateTimeHelper.GetMonday(currentDate);
                if(currentMonth > 12)
                {
                    currentMonth = 1;
                    currentYear++;
                }
            }

            return calendar;
        }
    }
}
