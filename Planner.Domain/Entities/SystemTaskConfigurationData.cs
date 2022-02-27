using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class SystemTaskConfigurationData
	{
		public string Comment { get; set; }
		public string TaskTypeKey { get; set; }
		public string RecurringTaskTypeKey { get; set; }
		public string RepeatsForKey { get; set; }
		public bool MustBeFinishedByAllWhos { get; set; }
		public int RecurringEveryNumberOfDays { get; set; }
		public int Credits { get; set; }
		public decimal Price { get; set; }
		public string PriorityKey { get; set; }
		public bool IsGuestRequest { get; set; }
		public bool IsShownInNewsFeed { get; set; }
		public bool IsRescheduledEveryDayUntilFinished { get; set; }
		public bool IsMajorNotificationRaisedWhenFinished { get; set; }
		public bool IsBlockingCleaningUntilFinished { get; set; }

		/// <summary>
		/// Values: FROM_TO, LIST
		/// </summary>
		public string WhatsTypeKey { get; set; }

		/// <summary>
		/// Used if WhatsTypeKey == "FROM_TO"
		/// </summary>
		public SystemTaskConfigurationWhere WhereFrom { get; set; }
		
		/// <summary>
		/// Used if WhatsTypeKey == "FROM_TO"
		/// </summary>
		public SystemTaskConfigurationWhere WhereTo { get; set; }
		
		public IEnumerable<SystemTaskConfigurationWhat> Whats { get; set; }
		public IEnumerable<SystemTaskConfigurationWho> Whos { get; set; }
		public IEnumerable<SystemTaskConfigurationWhere> Wheres { get; set; }
		public IEnumerable<SystemTaskConfigurationFile> Files { get; set; }
		public IEnumerable<string> FilestackImageUrls { get; set; }

		public IEnumerable<DateTime> StartsAtTimes { get; set; }
		public IEnumerable<SystemTaskRecurringTimeOptions> RecurringTaskRepeatTimes { get; set; }
		public int? RepeatsForNrOccurences { get; set; }
		public int? RepeatsForNrDays { get; set; }
		public DateTime? RepeatsUntilTime { get; set; }

		public string EventModifierKey { get; set; } // nullable
		public string EventKey { get; set; } // nullable
		public string EventTimeKey { get; set; } // nullable

		public DateTime? EndsAtTime { get; set; }
		public bool? ExcludeWeekends { get; set; }
		public bool? ExcludeHolidays { get; set; }
		public bool? PostponeWhenRoomIsOccupied { get; set; }
	}
}
