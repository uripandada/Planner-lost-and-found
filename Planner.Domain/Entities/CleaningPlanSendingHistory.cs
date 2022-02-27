using System;

namespace Planner.Domain.Entities
{
	public class CleaningPlanSendingHistory
	{
		public Guid Id { get; set; }
		public DateTime SentAt { get; set; }
		public Guid SentById { get; set; }
		public User SentBy { get; set; }
		public Guid CleaningPlanId { get; set; }
		public CleaningPlan CleaningPlan { get; set; }
		public string CleaningPlanJson { get; set; }
	}
}
