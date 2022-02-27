namespace Planner.Common.Enums
{


	/// <summary>
	/// 1. Task is claimed if not MustBeFinishedByAllWhos and status is not pending any more
	/// 2. Task is rejected if statusKey == "REJECTED"
	/// </summary>
	public enum TaskStatusType
	{
		PENDING,
		WAITING,
		STARTED,
		PAUSED,
		FINISHED,
		VERIFIED,
		CANCELLED,
		CLAIMED,
		REJECTED,
		UNKNOWN,
		CLAIMED_BY_SOMEONE_ELSE,
	}
}
