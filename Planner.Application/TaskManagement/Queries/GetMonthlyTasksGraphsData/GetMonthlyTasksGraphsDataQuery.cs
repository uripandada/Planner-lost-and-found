using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetPageOfTasks;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetMonthlyTasksGraphsData
{
	public class MonthlyTasksGraphsViewModel
	{
		public MonthlyTasksLabelViewModel[] Labels { get; set; }
		public int[] NumberOfWorkers { get; set; }
		public int MaxNumberOfWorkers { get; set; }
		public int[] NumberOfTasks { get; set; }
		public int MaxNumberOfTasks { get; set; }
		public decimal[] AverageTasksPerWorker { get; set; }
		public decimal MaxAverageTasksPerWorker { get; set; }
	}

	public class MonthlyTasksLabelViewModel
	{
		public string Key { get; set; }
		public string Text { get; set; }
		public int Day { get; set; }
	}

	public class GetMonthlyTasksGraphsDataQuery : IRequest<MonthlyTasksGraphsViewModel>, IAmWebApplicationHandler
	{
		public DateTime MonthDate { get; set; }
	}
	public class GetMonthlyTasksGraphsDataQueryHandler : IRequestHandler<GetMonthlyTasksGraphsDataQuery, MonthlyTasksGraphsViewModel>
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetMonthlyTasksGraphsDataQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MonthlyTasksGraphsViewModel> Handle(GetMonthlyTasksGraphsDataQuery request, CancellationToken cancellationToken)
		{
			var year = request.MonthDate.Date.Year;
			var month = request.MonthDate.Date.Month;

			var startsAt = new DateTime(request.MonthDate.Date.Year, request.MonthDate.Date.Month, 1);
			var endsAt = startsAt.AddMonths(1);

			var result = await this._databaseContext.NumberOfTasksPerUser.FromSqlInterpolated($@"
				SELECT 
					COUNT(id)::integer AS number_of_tasks,
					date_part('year',t.starts_at)::integer AS year,
					date_part('month',t.starts_at)::integer AS month,
					date_part('day',t.starts_at)::integer AS day,
					t.user_id
				FROM 
					public.system_tasks t
				WHERE 
					t.starts_at >= {startsAt}
					AND t.starts_at < {endsAt}
				GROUP BY
					date_part('year',t.starts_at),
					date_part('month',t.starts_at),
					date_part('day',t.starts_at),
					t.user_id;
			").ToListAsync();

			var resultMap = result.GroupBy(r => r.Day).ToDictionary(g => g.Key, g => g.ToArray());

			var numberOfDaysInMonth = DateTime.DaysInMonth(year, month);

			var response = new MonthlyTasksGraphsViewModel
			{
				AverageTasksPerWorker = new decimal[numberOfDaysInMonth],
				Labels = new MonthlyTasksLabelViewModel[numberOfDaysInMonth],
				NumberOfTasks = new int[numberOfDaysInMonth],
				NumberOfWorkers = new int[numberOfDaysInMonth],
				MaxAverageTasksPerWorker = 0,
				MaxNumberOfWorkers = 0,
				MaxNumberOfTasks = 0
			};
			for (var dayIndex = 0; dayIndex < numberOfDaysInMonth; dayIndex++)
			{
				var day = dayIndex + 1;

				response.AverageTasksPerWorker[dayIndex] = 0m;
				response.NumberOfTasks[dayIndex] = 0;
				response.NumberOfWorkers[dayIndex] = 0;
				response.Labels[dayIndex] = new MonthlyTasksLabelViewModel
				{
					Day = day,
					Key = day.ToString(),
					Text = $"{day.ToString()}."
				};
			}

			foreach (var dayTasksPair in resultMap)
			{
				var dayTasks = dayTasksPair.Value;
				var day = dayTasksPair.Key;
				var dayIndex = day - 1;

				var totalTasksInDay = 0;
				var totalWorkersInDay = 0;

				foreach (var dayTask in dayTasks)
				{
					totalWorkersInDay++;
					totalTasksInDay += dayTask.NumberOfTasks;
				}

				var avgTasksPerUserInDay = (totalTasksInDay / (decimal)totalWorkersInDay);

				response.NumberOfTasks[dayIndex] = totalTasksInDay;
				response.NumberOfWorkers[dayIndex] = totalWorkersInDay;
				response.AverageTasksPerWorker[dayIndex] = avgTasksPerUserInDay;

				if(totalTasksInDay > response.MaxNumberOfTasks)
				{
					response.MaxNumberOfTasks = totalTasksInDay;
				}
				if(totalWorkersInDay > response.MaxNumberOfWorkers)
				{
					response.MaxNumberOfWorkers = totalWorkersInDay;
				}
				if(avgTasksPerUserInDay > response.MaxAverageTasksPerWorker)
				{
					response.MaxAverageTasksPerWorker = avgTasksPerUserInDay;
				}
			}

			return response;
		}
	}
}
