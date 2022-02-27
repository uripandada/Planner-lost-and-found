using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace Planner.Domain.Entities
{
	public class CleaningPlan: BaseEntity
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public bool IsSent { get; set; }
		public DateTime? SentAt { get; set; }

		public Guid? SentById { get; set; }
		public User SentBy { get; set; }

		public IEnumerable<CleaningPlanSendingHistory> SendingHistory { get; set; }

		public IEnumerable<CleaningPlanGroup> Groups { get; set; }
		public IEnumerable<CleaningPlanItem> UngroupedItems { get; set; }
		public IEnumerable<Cleaning> Cleanings { get; set; }

		public CleaningPlanCpsatConfiguration CleaningPlanCpsatConfiguration { get; set; }
	}
}
