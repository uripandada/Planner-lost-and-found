using Planner.Common.Enums;
using System;

namespace Planner.Application.Infrastructure.Signalr.Messages
{
	public class RealTimeCleaningPlannerCleaningChangedMessage
	{
		public Guid RoomId { get; set; }
		public Guid CleaningPlanId { get; set; }
		public Guid CleaningPlanItemId { get; set; }
		public Guid CleaningId { get; set; }
		public CleaningProcessStatus CleaningProcessStatus { get; set; }
		public DateTime At { get; set; }
	}
}
