using System.Collections.Generic;

namespace Planner.Domain.Values
{
	public enum LostAndFoundStatus
	{
		Unknown = 0,
		WaitingRoomMaid = 12,
		Unclaimed = 1,
		ClientContacted = 2,
		ClientUndecided = 3,
		WaitingForClientReturn = 4,
		WaitingForShipment = 5,
		OTShipped = 6,
		WaitingForHandDelivered = 7,
		HandDelivered = 8,
		Expired = 9,
		RefusedByTheClient = 10,
		BadReferencing = 11,
		Deleted = 13,
	}

	public enum RccLostAndFoundStatus
	{
		OPEN = 0,
		EMAILED = 1,
		PHONED = 2,
		PENDING = 3,
		WAITING = 4,
		MAILED = 5,
		HAND_DELIVERED = 6,
		EXPIRED = 7,
		REFUSED = 8,
		DELETED = 9,
		CLOSED = 10,
		UNKNOWN = 11,
	}


	public class RccLostAndFoundStatusKeys
	{
		public static readonly Dictionary<RccLostAndFoundStatus, string> RccStatuses = new Dictionary<RccLostAndFoundStatus, string>
		{
			{ RccLostAndFoundStatus.OPEN, "open" },
			{ RccLostAndFoundStatus.EMAILED, "emailed" },
			{ RccLostAndFoundStatus.PHONED, "phoned" },
			{ RccLostAndFoundStatus.PENDING, "pending" },
			{ RccLostAndFoundStatus.WAITING, "waiting" },
			{ RccLostAndFoundStatus.MAILED, "mailed" },
			{ RccLostAndFoundStatus.HAND_DELIVERED, "hand-delivered" },
			{ RccLostAndFoundStatus.EXPIRED, "expired" },
			{ RccLostAndFoundStatus.REFUSED, "refused" },
			{ RccLostAndFoundStatus.DELETED, "delete" },
			{ RccLostAndFoundStatus.CLOSED, "closed" },
			{ RccLostAndFoundStatus.UNKNOWN, "unknown" },
		};
		public static readonly Dictionary<RccLostAndFoundStatus, string> RccStatusDescriptions = new Dictionary<RccLostAndFoundStatus, string>
		{
			{ RccLostAndFoundStatus.OPEN, "Open" },
			{ RccLostAndFoundStatus.EMAILED, "Emailed" },
			{ RccLostAndFoundStatus.PHONED, "Phoned" },
			{ RccLostAndFoundStatus.PENDING, "Pending" },
			{ RccLostAndFoundStatus.WAITING, "Waiting" },
			{ RccLostAndFoundStatus.MAILED, "Mailed" },
			{ RccLostAndFoundStatus.HAND_DELIVERED, "Hand delivered" },
			{ RccLostAndFoundStatus.EXPIRED, "Expired" },
			{ RccLostAndFoundStatus.REFUSED, "Refused" },
			{ RccLostAndFoundStatus.DELETED, "Delete" },
			{ RccLostAndFoundStatus.CLOSED, "Closed" },
			{ RccLostAndFoundStatus.UNKNOWN, "Unknown" },
		};
		public static readonly Dictionary<string, RccLostAndFoundStatus> Statuses = new Dictionary<string, RccLostAndFoundStatus>
		{
			{ "open", RccLostAndFoundStatus.OPEN },
			{ "emailed", RccLostAndFoundStatus.EMAILED },
			{ "phoned", RccLostAndFoundStatus.PHONED },
			{ "pending", RccLostAndFoundStatus.PENDING  },
			{ "waiting", RccLostAndFoundStatus.WAITING },
			{ "mailed" , RccLostAndFoundStatus.MAILED},
			{ "hand-delivered", RccLostAndFoundStatus.HAND_DELIVERED},
			{ "expired", RccLostAndFoundStatus.EXPIRED},
			{ "refused", RccLostAndFoundStatus.REFUSED },
			{ "delete", RccLostAndFoundStatus.DELETED },
			{ "closed", RccLostAndFoundStatus.CLOSED },
			{ "unknown", RccLostAndFoundStatus.UNKNOWN },
		};
	}
}
