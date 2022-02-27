namespace Planner.Common.Enums
{
	/// <summary>
	/// Changes when any of the flags on the room changes:
	/// IsOutOfOrder
	/// IsOutOfService
	/// IsOccupied
	/// IsClean
	/// IsInspected
	/// IsCleaningInProgress
	/// </summary>

	public enum RccHousekeepingStatusCode
	{
		/// <summary>
		/// Dirty
		/// </summary>
		HD,
		
		/// <summary>
		/// Housekeeping in progress
		/// </summary>
		HP,
		
		/// <summary>
		/// Clean
		/// </summary>
		HC,
		
		/// <summary>
		/// Clean inspected
		/// </summary>
		HCI,
		
		/// <summary>
		/// Occupied dirty
		/// </summary>
		OHD,
		
		/// <summary>
		/// Occupied, housekeeping in progress
		/// </summary>
		OHP,
		
		/// <summary>
		/// Occupied clean
		/// </summary>
		OHC,
		
		/// <summary>
		/// Occupied clean inspected
		/// </summary>
		OHCI,
		
		/// <summary>
		/// Vacant dirty
		/// </summary>
		VHD,
		
		/// <summary>
		/// Vacant, housekeeping in progress
		/// </summary>
		VHP,
		
		/// <summary>
		/// Vacant clean
		/// </summary>
		VHC,

		/// <summary>
		/// Vacant clean inspected
		/// </summary>
		VHCI,

		/// <summary>
		/// Out of order
		/// </summary>
		OOO,

		/// <summary>
		/// Out of service
		/// </summary>
		OOS,

		/// <summary>
		/// Pickup
		/// </summary>
		PU,

		/// <summary>
		/// TidyUpRequired
		/// </summary>
		TD,

		/// <summary>
		/// Turn down requests
		/// </summary>
		TDR,

		/// <summary>
		/// Turn down not requests
		/// Turn down done
		/// </summary>
		TDNR,

		/// <summary>
		/// Do not disturb
		/// </summary>
		DNN,

		/// <summary>
		/// No changes
		/// </summary>
		NO,

		/// <summary>
		/// Luggage
		/// </summary>
		LUG,
	}
}
