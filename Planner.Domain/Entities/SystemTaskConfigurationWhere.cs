namespace Planner.Domain.Entities
{
	public class SystemTaskConfigurationWhere
	{
		public string TypeKey { get; set; }
		public string TypeDescription { get; set; }
		public string ReferenceId { get; set; }
		public string ReferenceName { get; set; }

	}

	//public class SingleTaskOptions
	//{
	//	//public Guid Id { get; set; }
	//	//public DateTime StartsAt { get; set; }
	//}

	//public class DailyRecurringTaskOptions
	//{
	//	//public Guid Id { get; set; }
	//	//public DateTime StartsAt { get; set; }
	//	//public TimeSpan[] RepeatTimes { get; set; } ///// FIX THIS ONE!

	//	//public int RepeatsForNrOccurences { get; set; }
	//	//public int RepeatsForNrDays { get; set; }
	//	//public DateTime? RepeatsUntilTime { get; set; }
	//}
	//public class WeeklyRecurringTaskOptions
	//{
	//	//public Guid Id { get; set; }
	//	//public DateTime StartsAt { get; set; }
	//	//public IEnumerable<WeeklyRecurringTaskItemOptions> WeeklyRecurrences { get; set; }

	//	//public int RepeatsForNrOccurences { get; set; }
	//	//public int RepeatsForNrDays { get; set; }
	//	//public DateTime? RepeatsUntilTime { get; set; }
	//}

	//public class WeeklyRecurringTaskItemOptions
	//{
	//	//public Guid Id { get; set; }
	//	//public string DayKey { get; set; }
	//	//public TimeSpan[] RepeatTimes { get; set; }
	//}

	//public class MonthlyRecurringTaskOptions
	//{
	//	//public Guid Id { get; set; }
	//	//public DateTime StartsAt { get; set; }
	//	//public IEnumerable<MonthlyRecurringTaskItemOptions> MonthlyRecurrences { get; set; }
	//	//public int RepeatsForNrOccurences { get; set; }
	//	//public int RepeatsForNrDays { get; set; }
	//	//public DateTime? RepeatsUntilTime { get; set; }
	//}

	//public class MonthlyRecurringTaskItemOptions
	//{
	//	//public Guid Id { get; set; }
	//	//public int NthOfMonth { get; set; }
	//	//public TimeSpan[] RepeatTime { get; set; }
	//}

	//public class SpecificTimesRecurringTaskOptions
	//{
	//	//public Guid Id { get; set; }
	//	//public DateTime StartsAt { get; set; }
	//}

	//public class EventTaskOptions
	//{
	//	public Guid Id { get; set; }
	//	public DateTime StartsAt { get; set; }

	//	public string EventModifierKey { get; set; } // nullable
	//	public string EventKey { get; set; } // nullable
	//	public string EventTimeKey { get; set; } // nullable
	//}
}
