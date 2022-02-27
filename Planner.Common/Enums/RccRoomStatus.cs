namespace Planner.Common.Enums
{
	public enum RccRoomStatus
	{
		/// <summary>
		/// Guest arrives today regardless of checking in.
		/// </summary>
		ArrivesToday,

		/// <summary>
		/// Guest arrives today but has not checked in.
		/// </summary>
		ArrivesTodayNotCheckedIn,

		/// <summary>
		/// Guest arrived today and has checked in.
		/// </summary>
		ArrivedTodayCheckedIn, 

		/// <summary>
		/// Guest leaves today regardless of checking out.
		/// </summary>
		DepartsToday,

		/// <summary>
		/// Guest leaves today but has not checked out.
		/// </summary>
		DepartsTodayNotCheckedOut, 

		/// <summary>
		/// Guest leaves today and has checked out.
		/// </summary>
		DepartsTodayCheckedOut,

		/// <summary>
		/// Guest leaves today and another arrives today regardless of checked in or out
		/// </summary>
		DepartsTodayArrivesToday,

		/// <summary>
		/// Guest will leave today but has not checked out yet. Another guest is expected to arrive later.
		/// </summary>
		DepartsTodayNotCheckedOutArrivesTodayNotCheckedIn,

		/// <summary>
		/// Guest has departed and another is expected to arrive.
		/// </summary>
		DepartsTodayCheckedOutArrivesTodayNotCheckedIn,

		/// <summary>
		/// Guest from this morning as departed and the second guest has arrived.
		/// </summary>
		DepartsTodayCheckedOutArrivesTodayCheckedIn,

		/// <summary>
		/// Guest will continue their stay.
		/// </summary>
		StayToday,

		/// <summary>
		/// Guest day use
		/// </summary>
		DayUseToday,

		/// <summary>
		/// Guest day use with another guest expecting to arrive later
		/// </summary>
		DayUseTodayArrivesTodayNotCheckedIn,

		/// <summary>
		/// No reservation but occupied
		/// </summary>
		NoReservationButOccupied,

		/// <summary>
		/// General occupation of the room
		/// </summary>
		Occupied, 

		/// <summary>
		/// General vacancy of the room
		/// </summary>
		Vacant,

		/// <summary>
		/// Out of order
		/// </summary>
		OutOfOrder,

		/// <summary>
		/// Out of service
		/// </summary>
		OutOfService,

		/// <summary>
		/// Pick up
		/// </summary>
		PickUp,
	}
}
