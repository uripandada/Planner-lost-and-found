using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure.Signalr.Messages
{
	//public class RealTimeDashboardWorkerItemsChangedMessage
	//{
	//	public RoomViewDashboardWorkerItem[] Items { get; set; }
	//	public DateTime At { get; set; }
	//}
	public class RealTimeRefreshRoomsOverviewDashboardMessage
	{
		public IEnumerable<Guid> RoomIds { get; set; }
	}
	public class RealTimeMessagesChangedMessage
	{
		public IEnumerable<Guid> RoomIds { get; set; }
		public IEnumerable<string> ReservationIds { get; set; }
	}
	//public class RealTimeRoomsOverviewRoomStatusChangedMessage
	//{
	//	public string HotelId { get; set; }
	//	public Guid RoomId { get; set; }
	//	public bool IsClean { get; set; }
	//	public bool IsOccupied { get; set; }
	//}

	//public class RealTimeRoomsOverviewWorkerStatusChangedMessage
	//{
	//	public Guid UserId { get; set; }
	//	public string HotelId { get; set; }
	//	public Guid RoomId { get; set; }
	//	public bool IsOnDuty { get; set; }
	//	public string DutyDescription { get; set; }
	//}

}
