using System;
using System.Collections.Generic;

namespace Planner.Domain.Entities
{

	public class SystemTask : ChangeTrackingBaseEntity
	{
		public Guid Id { get; set; }
		//public string ActionName { get; set; }
		//public string AssetName { get; set; }
		//public Guid AssetId { get; set; }
		//public Guid? AssetModelId { get; set; }
		//public int AssetQuantity { get; set; }

		public IEnumerable<SystemTaskAction> Actions { get; set; }

		/// <summary>
		/// TaskType enum: SINGLE, RECURRING, EVENT, BALANCED
		/// </summary>
		public string TypeKey { get; set; }
		
		/// <summary>
		/// "NUMBER_OF_DAYS", "NUMBER_OF_OCCURENCES", "SPECIFIC_DATE"
		/// </summary>
		public string RepeatsForKey { get; set; }

		/// <summary>
		/// RecurringTaskType enum: DAILY, WEEKLY, MONTHLY, SPECIFIC_TIME, EVERY,
		/// </summary>
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

		/// <summary>
		/// If the UserId is null it means that the task is really ment for the planned attendant.
		/// Once the cleaning is assigned to a group (cleaner), only then the task can be updated
		/// to have the proper cleaner assigned.
		/// Task is assigned to a user AT THE POINT OF SENDING THE PLAN, not really planning the cleaning.
		/// The Task should be unassigned if the cleaning is "removed" or the new plan is created without 
		/// the cleaning that has a task for a planned attendant.
		/// </summary>
		public Guid? UserId { get; set; }
		public User User { get; set; }

		/// <summary>
		/// If the UserId is null it means that the task is really ment for the planned attendant.
		/// Once the cleaning is assigned to a group (cleaner), only then the task can be updated
		/// to have the proper cleaner assigned.
		/// Task is assigned to a user AT THE POINT OF SENDING THE PLAN, not really planning the cleaning.
		/// The Task should be unassigned if the cleaning is "removed" or the new plan is created without 
		/// the cleaning that has a task for a planned attendant.
		/// </summary>
		public bool IsForPlannedAttendant { get; set; }
		
		public string FromReservationId { get; set; }
		public Reservation FromReservation { get; set; }
		public Guid? FromWarehouseId { get; set; }
		public Warehouse FromWarehouse { get; set; }
		public Guid? FromRoomId { get; set; }
		public Room FromRoom { get; set; }
		public string FromHotelId { get; set; }
		public Hotel FromHotel { get; set; }
		public string FromName { get; set; }

		public string ToReservationId { get; set; }
		public Reservation ToReservation { get; set; }
		public Guid? ToWarehouseId { get; set; }
		public Warehouse ToWarehouse { get; set; }
		public Guid? ToRoomId { get; set; }
		public Room ToRoom { get; set; }
		public string ToHotelId { get; set; }
		public Hotel ToHotel { get; set; }
		public string ToName { get; set; }

		/// <summary>
		/// Values: FROM_TO, TO
		/// </summary>
		public string WhereTypeKey { get; set; }

		public string EventModifierKey { get; set; } // nullable
		/// <summary>
		/// EventTaskType enum
		/// </summary>
		public string EventKey { get; set; } // nullable
		/// <summary>
		/// TaskEventTimeType enum
		/// </summary>
		public string EventTimeKey { get; set; } // nullable
		/// <summary>
		/// TaskStatusType enum
		/// </summary>
		public string StatusKey { get; set; }
		public DateTime StartsAt { get; set; }

		public Guid SystemTaskConfigurationId { get; set; }
		public SystemTaskConfiguration SystemTaskConfiguration { get; set; }

		public bool IsManuallyModified { get; set; }

		public IEnumerable<SystemTaskHistory> History { get; set; }
		public IEnumerable<SystemTaskMessage> Messages { get; set; }

		public string Comment { get; set; }
	}

	public class ExtendedSystemTask : SystemTask
	{
		public string FromRoomName { get; set; }
		public string FromWarehouseName { get; set; }
		public string FromReservationGuestName { get; set; }
		public string FromHotelName { get; set; }
		public string FromBuildingName { get; set; }
		public Guid? FromBuildingId { get; set; }
		public string FromFloorName { get; set; }
		public Guid? FromFloorId { get; set; }
		
		public string ToRoomName { get; set; }
		public string ToWarehouseName { get; set; }
		public string ToReservationGuestName { get; set; }
		public string ToHotelName { get; set; }
		public string ToBuildingName { get; set; }
		public Guid? ToBuildingId { get; set; }
		public string ToFloorName { get; set; }
		public Guid? ToFloorId { get; set; }

		public string UserFullName { get; set; }
		public string UserUsername { get; set; }
	}
}
