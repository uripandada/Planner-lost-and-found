using System.Collections.Generic;

namespace Planner.Domain.Values
{
    public enum ExperienceTicketStatus
	{
        Pending = 0
	}
	public enum ExperienceClientRelationStatus
	{
		NoClientAction = 0,
		MeetWithClient = 1,
		MeetWithClientAtCO = 2,
	}

	public enum ExperienceResolutionStatus
	{
		None = 0,
		Resolved = 1,
		Closed = 2
	}
}
