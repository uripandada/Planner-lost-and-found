namespace Planner.Common.Enums
{
	public enum MasterFilterType
	{
		ROOMS_OVERVIEW_DASHBOARD
	}

	public enum MasterFilterGroupType
	{
		GUESTS,

		/// <summary>
		/// VACANT, OCCUPIED, STAY, DAYSTAY, ARRIVAL, ARRIVED, DEPARTURE, DEPARTED, ALL_ARRIVALS, ALL_DEPARTURES
		/// </summary>
		GUEST_STATUSES,

		/// <summary>
		/// DIRTY, CLEAN 
		/// </summary>
		CLENLINESS,

		/// <summary>
		/// ANY, NEW, FINISHED, DELAYED, PAUSED, DND, REFUSED, INSPECTED
		/// </summary>
		HOUSEKEEPING_STATUSES,

		/// <summary>
		/// VIP, PMS_NOTE
		/// </summary>
		PMS,

		/// <summary>
		/// CHANGE SHEETS, PRIORITY
		/// </summary>
		OTHERS,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		ROOM_CATEGORIES,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		BUILDINGS,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		FLOORS,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		ROOMS,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		FLOOR_SECTIONS,

		/// <summary>
		/// LOADED FROM DB
		/// </summary>
		FLOOR_SUB_SECTIONS,
	}
}
