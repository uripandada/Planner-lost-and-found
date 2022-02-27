namespace Planner.Common.Enums
{
	public enum RoomMessageFilterReferenceType
	{
		/// <summary>
		/// CHANGE SHEETS, PRIORITY
		/// </summary>
		OTHERS = 0,

		/// <summary>
		/// VACANT, OCCUPIED, STAY, DAYSTAY, ARRIVAL, ARRIVED, DEPARTURE, DEPARTED, ALL_ARRIVALS, ALL_DEPARTURES
		/// </summary>
		GUEST_STATUSES = 1,

		/// <summary>
		/// DIRTY, CLEAN 
		/// </summary>
		CLENLINESS = 2,

		/// <summary>
		/// ANY, NEW, FINISHED, DELAYED, PAUSED, DND, REFUSED, INSPECTED
		/// </summary>
		HOUSEKEEPING_STATUSES = 3,

		/// <summary>
		/// VIP, PMS_NOTE
		/// </summary>
		PMS = 4,

		RESERVATIONS = 5,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		ROOM_CATEGORIES = 6,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		BUILDINGS = 7,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		FLOORS = 8,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		ROOMS = 9,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		FLOOR_SECTIONS = 10,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		FLOOR_SUB_SECTIONS = 11, 
	}
}
