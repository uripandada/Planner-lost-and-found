using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Planner.Application.Infrastructure
{
	public static class TaskConfigurationDescriptionProvider
	{
		public static TaskConfigurationDescription Describe(this Domain.Entities.SystemTaskConfiguration configuration)
		{

			var description = "";
			switch (configuration.Data.TaskTypeKey)
			{
				case nameof(TaskType.SINGLE):
					description = "Single task.";
					break;
				case nameof(TaskType.RECURRING):
					switch (configuration.Data.RecurringTaskTypeKey)
					{
						case nameof(RecurringTaskType.DAILY):
							var dailyStartsAtTime = configuration.Data.StartsAtTimes.FirstOrDefault();
							description = $"Daily tasks, starting from {dailyStartsAtTime.ToString("dd MMM HH:mm")} then repeating every day at {string.Join(", ", configuration.Data.RecurringTaskRepeatTimes.FirstOrDefault()?.RepeatTimes)}, {configuration.DescribeDefaultRepeatsFor()}.";
							break;
						case nameof(RecurringTaskType.MONTHLY):
							var monthlyStartsAtTime = configuration.Data.StartsAtTimes.FirstOrDefault();
							var monthlyAts = new List<string>();
							foreach(var rt in configuration.Data.RecurringTaskRepeatTimes)
							{
								monthlyAts.Add($"{rt.Key} of month at {rt.RepeatTimes.FirstOrDefault()}");
							}
							description = $"Monthly tasks, starting from {monthlyStartsAtTime.ToString("dd MMM HH: mm")} then repating every {string.Join(", ", monthlyAts)}, {configuration.DescribeDefaultRepeatsFor()}.";
							break;
						case nameof(RecurringTaskType.SPECIFIC_TIME):
							description = $"Tasks at {string.Join(", ", configuration.Data.StartsAtTimes.Select(t => t.ToString("dd MMM HH: mm")).ToArray())}.";
							break;
						case nameof(RecurringTaskType.WEEKLY):
							var weeklyStartsAtTime = configuration.Data.StartsAtTimes.FirstOrDefault();
							var weeklyAts = new List<string>();
							foreach (var rt in configuration.Data.RecurringTaskRepeatTimes)
							{
								if(rt.RepeatTimes != null && rt.RepeatTimes.Any())
								{
									weeklyAts.Add($"{rt.Key} at {string.Join(", ", rt.RepeatTimes)}");
								}
							}
							description = $"Weekly tasks, starting from {weeklyStartsAtTime.ToString("dd MMM HH: mm")} then repeating every {string.Join(", ", weeklyAts)}, {configuration.DescribeDefaultRepeatsFor()}.";
							break;
						default:
							description = "UNKONWN_RECURRING_TASK_TYPE";
							break;
					}
					break;
				case nameof(TaskType.EVENT):
					var eventTaskDescription = $"{TaskManagement.Queries.GetTasksData.TaskDescriptions.GetEventModifierDescription(configuration.Data.EventModifierKey)} {TaskManagement.Queries.GetTasksData.TaskDescriptions.GetEventTypeDescription(configuration.Data.EventKey)}";
					switch (configuration.Data.EventTimeKey)
					{
						case nameof(TaskEventTimeType.ON_NEXT):
							description = $"On next {eventTaskDescription}.";
							break;
						case nameof(TaskEventTimeType.ON_DATE):
							description = $"On {eventTaskDescription} at {configuration.Data.StartsAtTimes.FirstOrDefault().ToString("dd MMM HH: mm")}.";
							break;
						case nameof(TaskEventTimeType.EVERY_TIME):
							description = $"On every {eventTaskDescription}, starting from {configuration.Data.StartsAtTimes.FirstOrDefault().ToString("dd MMM HH: mm")}, {configuration.DescribeDefaultRepeatsFor()}.";
							break;
						default:
							description = "UNKNOWN_EVENT_TIME";
							break;
					}
					break;
				default:
					description = "UNKONWN_TASK_TYPE";
					break;
			}

			return new TaskConfigurationDescription
			{
				Actions = configuration.Data.Whats == null ? new TaskConfigurationActionDescription[0] : configuration.Data.Whats.Select(w => new TaskConfigurationActionDescription 
				{ 
					ActionName = w.ActionName,
					AssetName = w.AssetName,
					AssetQuantity = w.AssetQuantity
				}).ToArray(),
				Description = description,
			};
		}

		public static string DescribeDefaultRepeatsFor(this Domain.Entities.SystemTaskConfiguration configuration)
		{
			var description = "";
			switch (configuration.Data.RepeatsForKey)
			{
				case "NUMBER_OF_DAYS":
					description = $"for {configuration.Data.RepeatsForNrDays} days";
					break;
				case "NUMBER_OF_OCCURENCES":
					description = $"{configuration.Data.RepeatsForNrOccurences} times";
					break;
				case "SPECIFIC_DATE":
					description = $"until {configuration.Data.RepeatsUntilTime?.ToString("dd MMM HH:mm")}";
					break;
				default:
					description = "UNKNOWN";
					break;
			}

			return description;
		}
	}
}
