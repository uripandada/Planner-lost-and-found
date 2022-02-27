using System.Collections.Generic;

namespace Planner.Common.Enums
{
	public static class COLORS
	{
		public static string ToHexColor(this RccRoomStatusCode code)
		{
			return ROOM_STATUSES[code];
		}

		public static string ToHexColor(this RccHousekeepingStatusCode code)
		{
			return ROOM_HOUSEKEEPING_STATUSES[code];
		}

		public readonly static Dictionary<RccHousekeepingStatusCode, string> ROOM_HOUSEKEEPING_STATUSES = new Dictionary<RccHousekeepingStatusCode, string>
		{
			// red: E74C3C
			// green: 2ECC71
			// blue: 3498DB
			// orange: F39C12

			// clean statuses - blue
			{ RccHousekeepingStatusCode.HC, "3498DB" },
			{ RccHousekeepingStatusCode.OHC, "3498DB" },
			{ RccHousekeepingStatusCode.VHC, "3498DB" },
			
			// inspected statuses - green
			{ RccHousekeepingStatusCode.HCI, "2ECC71" },
			{ RccHousekeepingStatusCode.OHCI, "2ECC71" },
			{ RccHousekeepingStatusCode.VHCI, "2ECC71" },
			
			// general statuses - dark gray
			{ RccHousekeepingStatusCode.DNN, "454545" },
			{ RccHousekeepingStatusCode.LUG, "454545" },
			{ RccHousekeepingStatusCode.NO, "454545" },
			{ RccHousekeepingStatusCode.PU, "454545" },
			{ RccHousekeepingStatusCode.TDNR, "454545" },
			{ RccHousekeepingStatusCode.TDR, "454545" },
			
			// out of service / order - gray
			{ RccHousekeepingStatusCode.OOO, "777777" },
			{ RccHousekeepingStatusCode.OOS, "777777" },
			
			// in progress statuses - orange
			{ RccHousekeepingStatusCode.HP, "F39C12" },
			{ RccHousekeepingStatusCode.OHP, "F39C12" },
			{ RccHousekeepingStatusCode.VHP, "F39C12" },

			// dirty statuses - red
			{ RccHousekeepingStatusCode.TD, "E74C3C" },
			{ RccHousekeepingStatusCode.HD, "E74C3C" },
			{ RccHousekeepingStatusCode.OHD, "E74C3C" },
			{ RccHousekeepingStatusCode.VHD, "E74C3C" },
		};

		public readonly static Dictionary<RccHousekeepingStatusCode, string> ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS = new Dictionary<RccHousekeepingStatusCode, string>
		{
			// red: E74C3C
			// green: 2ECC71
			// blue: 3498DB
			// orange: F39C12

			// clean statuses - blue
			{ RccHousekeepingStatusCode.HC, "Clean" },
			{ RccHousekeepingStatusCode.OHC, "Occupied clean" },
			{ RccHousekeepingStatusCode.VHC, "Vacant clean" },
			
			// inspected statuses - green
			{ RccHousekeepingStatusCode.HCI, "Clean inspected" },
			{ RccHousekeepingStatusCode.OHCI, "Occupied clean inspected" },
			{ RccHousekeepingStatusCode.VHCI, "Vacant clean inspected" },
			
			// general statuses - dark gray
			{ RccHousekeepingStatusCode.DNN, "Do not disturb" },
			{ RccHousekeepingStatusCode.LUG, "Luggage" },
			{ RccHousekeepingStatusCode.NO, "No changes" },
			{ RccHousekeepingStatusCode.PU, "Pickup" },
			{ RccHousekeepingStatusCode.TDNR, "Turn down done" },
			{ RccHousekeepingStatusCode.TDR, "Turn down requests" },
			
			// out of service / order - gray
			{ RccHousekeepingStatusCode.OOO, "Out of order" },
			{ RccHousekeepingStatusCode.OOS, "Out of service" },
			
			// in progress statuses - orange
			{ RccHousekeepingStatusCode.HP, "Housekeeping in progress" },
			{ RccHousekeepingStatusCode.OHP, "Occupied, housekeeping in progress" },
			{ RccHousekeepingStatusCode.VHP, "Vacant, housekeeping in progress" },

			// dirty statuses - red
			{ RccHousekeepingStatusCode.TD, "Tidy up required" },
			{ RccHousekeepingStatusCode.HD, "Dirty" },
			{ RccHousekeepingStatusCode.OHD, "Occupied dirty" },
			{ RccHousekeepingStatusCode.VHD, "Vacant dirty" },
		};

		public readonly static Dictionary<RccRoomStatusCode, string> ROOM_STATUSES = new Dictionary<RccRoomStatusCode, string>
		{
			{ RccRoomStatusCode.ARR, "000000" },
			{ RccRoomStatusCode.ARL, "000000" },
			{ RccRoomStatusCode.ARD, "000000" },
			{ RccRoomStatusCode.DEP, "000000" },
			{ RccRoomStatusCode.DPE, "000000" },
			{ RccRoomStatusCode.DPD, "000000" },
			{ RccRoomStatusCode.DA, "000000" },
			{ RccRoomStatusCode.DPEA, "000000" },
			{ RccRoomStatusCode.DPAR, "000000" },
			{ RccRoomStatusCode.DARD, "000000" },
			{ RccRoomStatusCode.STAY, "000000" },
			{ RccRoomStatusCode.DU, "000000" },
			{ RccRoomStatusCode.DUA, "000000" },
			{ RccRoomStatusCode.NR, "000000" },
			{ RccRoomStatusCode.OCC, "000000" },
			{ RccRoomStatusCode.VAC, "000000" },
			{ RccRoomStatusCode.OOO, "000000" },
			{ RccRoomStatusCode.OOS, "000000" },
			{ RccRoomStatusCode.PU, "000000" },
		};
	}

	public enum RccRoomStatusCode
	{
		/// <summary>
		/// Guest arrives today regardless of checking in.
		/// </summary>
		ARR,

		/// <summary>
		/// Guest arrives today but has not checked in.
		/// </summary>
		ARL,
		
		/// <summary>
		/// Guest arrived today and has checked in.
		/// </summary>
		ARD,

		/// <summary>
		/// Guest leaves today regardless of checking out.
		/// </summary>
		DEP,
		
		/// <summary>
		/// Guest leaves today but has not checked out.
		/// </summary>
		DPE,
		
		/// <summary>
		/// Guest leaves today and has checked out.
		/// </summary>
		DPD,

		/// <summary>
		/// Guest leaves today and another arrives today regardless of checked in or out
		/// </summary>
		DA,
		
		/// <summary>
		/// Guest will leave today but has not checked out yet. Another guest is expected to arrive later.
		/// </summary>
		DPEA,
		
		/// <summary>
		/// Guest has departed and another is expected to arrive.
		/// </summary>
		DPAR,
		
		/// <summary>
		/// Guest from this morning as departed and the second guest has arrived.
		/// </summary>
		DARD, 

		/// <summary>
		/// Guest will continue their stay
		/// </summary>
		STAY, 

		/// <summary>
		/// Guest day use
		/// </summary>
		DU,
		
		/// <summary>
		/// Guest day use with another guest expecting to arrive later
		/// </summary>
		DUA,

		/// <summary>
		/// No reservation but occupied
		/// </summary>
		NR,

		/// <summary>
		/// General occupation of the room
		/// </summary>
		OCC,
		
		/// <summary>
		/// General vacancy of the room
		/// </summary>
		VAC,

		/// <summary>
		/// Out of order
		/// </summary>
		OOO,
		
		/// <summary>
		/// Out of service
		/// </summary>
		OOS, 
		
		/// <summary>
		/// Pick up
		/// </summary>
		PU
	}
}
