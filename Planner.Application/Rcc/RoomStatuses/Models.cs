using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Rcc.RoomStatuses
{
	public class RccHotelGroupRoomStatusChanges
	{
		public DateTime At { get; set; }
		public Guid HotelGroupId { get; set; }
		public string HotelGroupKey { get; set; }
		public IEnumerable<RccHotelRoomStatusChanges> HotelRoomStatuses { get; set; }
	}

	public class RccHotelRoomStatusChanges
	{
		public string HotelId { get; set; }
		public IEnumerable<RccRoomStatusData> RoomStatuses { get; set; }
	}

	public class RccRoomStatusData
	{
		////--"rsCommonKey": null,
		//public string RsCommonKey { get; set; }

		////"roomAccess": "Chambre",
		//public string RoomAccess { get; set; }

		////"isPriority": false,
		//public bool IsPriority { get; set; }

		////"roomStatus": "Libre",
		//public string RoomStatus { get; set; }

		////"hkCommonKey": null,
		//public string HkCommonKey { get; set; }

		////"updateTs": 1638369190,
		//public long UpdateTs { get; set; }

		////"updateTapTs": null,
		//public long UpdateTapTs { get; set; }

		////"updateType": "roomHousekeeping",
		//public string UpdateType { get; set; }

		////"attendantStatus": "",
		//public string AttendantStatus { get; set; }

		////"roomHousekeeping": "Libre Propre Inspectée",
		//public string RoomHousekeeping { get; set; }

		////"isChangeSheets": false
		//public bool IsChangeSheets { get; set; }


		//// room.RoomName, room.HousekeepingCode, room.RoomStatusCode, room.LastUpdate, room.UpdateUsername

		/// "roomName": "606",
		public string RoomName { get; set; }

		/// "hkCode": "VHCI",
		/// RccHousekeepingStatusCode enum
		public string HkCode { get; set; }

		/// "rsCode": "VAC",
		/// RccRoomStatusCode enum - ROOM STATUS CODE
		public string RsCode { get; set; }

		/// --"lastUpdate": "2021-12-01 14:33:10.306255",
		public DateTime LastUpdate { get; set; }

		/// "updateUsername": null,
		public string UpdateUsername { get; set; }
	}
}
