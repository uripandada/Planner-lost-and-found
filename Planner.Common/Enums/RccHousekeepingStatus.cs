namespace Planner.Common.Enums
{
	public enum RccHousekeepingStatus
	{
		Dirty, // HD
		HkInProgress, // HP
		Clean, // HC
		CleanInspected, // HCI
		OccupiedDirty,
		OccupiedHkInProgress,
		OccupiedClean,
		OccupiedCleanInspected,
		VacantDirty,
		VacantHkInProgress,
		VacantClean,
		VacantCleanInspected,
		OutOfOrder,
		OutOfService,
		Pickup,
		TidyUpRequired,
		TurnDownRequests,
		TurnDownNotRequests,
		TurnDownDone,
		DoNotDisturb,
		NoChanges,
		Luggage,
	}

}
