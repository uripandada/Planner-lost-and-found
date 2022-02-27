using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{
	public class SystemTaskHistoryActionData
	{
		public Guid Id { get; set; }
		public string ActionName { get; set; }
		public string AssetName { get; set; }
		public string AssetGroupName { get; set; }
		public Guid AssetId { get; set; }
		public Guid AssetGroupId { get; set; }
		public int AssetQuantity { get; set; }
	}

	public class SystemTaskHistoryData
	{
		//public string ActionName { get; set; }
		//public string AssetName { get; set; }
		//public Guid AssetId { get; set; }
		//public Guid? AssetModelId { get; set; }

		public IEnumerable<SystemTaskHistoryActionData> Actions { get; set; }
		public string TypeKey { get; set; }
		public string RepeatsForKey { get; set; }
		public string RecurringTypeKey { get; set; }
		public bool MustBeFinishedByAllWhos { get; set; }

		public int Credits { get; set; }
		public decimal Price { get; set; }

		public string PriorityKey { get; set; }

		public bool IsGuestRequest { get; set; }
		public bool IsShownInNewsFeed { get; set; }
		public bool IsRescheduledEveryDayUntilFinished { get; set; }
		public bool IsMajorNotificationRaisedWhenFinished { get; set; }
		public bool IsBlockingCleaningUntilFinished { get; set; }

		public Guid? UserId { get; set; }
		public bool IsForPlannedAttendant { get; set; }

		public string FromReservationId { get; set; }
		public string FromName { get; set; }
		public string FromHotelName { get; set; }
		public string FromHotelId { get; set; }
		public Guid? FromWarehouseId { get; set; }
		public Guid? FromRoomId { get; set; }
		public string ToReservationId { get; set; }
		public Guid? ToWarehouseId { get; set; }
		public Guid? ToRoomId { get; set; }
		public string ToName { get; set; }
		public string ToHotelId { get; set; }
		public string ToHotelName { get; set; }

		/// <summary>
		/// Values: FROM_TO, TO
		/// </summary>
		public string WhereTypeKey { get; set; }

		public string EventModifierKey { get; set; }
		public string EventKey { get; set; }
		public string EventTimeKey { get; set; }

		public string StatusKey { get; set; }
		public DateTime StartsAt { get; set; }

		public bool IsManuallyModified { get; set; }

		public string Comment { get; set; }
	}
}
