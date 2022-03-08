using System.Collections.Generic;

namespace Planner.Domain.Values
{
    public enum FoundStatus
	{
		Unknown = 0,
        WaitingRoomMaid = 1,
        Received = 2,
	}
	public enum GuestStatus
	{
		Unknown = 0,
		Unclaimed = 1,
		ClientContactedByEmail = 2,
		ClientContactedByPhone = 3,
		ClientUndecided = 4,
		WaitingForClientReturn = 5,
	}

	public enum DeliveryStatus
    {
		Unknown = 0,
		WaitingForHandDelivered = 1,
		WaitingForShipment = 2,
		OTShipped = 3,
		HandDelivered = 4,
	}

	public enum OtherStatus
    {
		Unknown = 0,
		Expired = 1,
		RefusedByTheClient = 2,
		BadReferencing = 3,
		Destroy = 4,
		ReturnedToInventor = 5,
		GivenToAnotherPerson = 6,
		DisappearedOrLost = 7,
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
