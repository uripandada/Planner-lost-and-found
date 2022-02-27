using Planner.Common.Enums;
using System;

namespace Planner.Common.Extensions
{
	public static class RccHousekeepingStatusExtensions
	{
		public static RccHousekeepingStatus ToStatus(this RccHousekeepingStatusCode code)
		{
			switch (code)
			{
				case RccHousekeepingStatusCode.DNN:
					return RccHousekeepingStatus.DoNotDisturb;

				case RccHousekeepingStatusCode.HC:
					return RccHousekeepingStatus.Clean;

				case RccHousekeepingStatusCode.HCI:
					return RccHousekeepingStatus.CleanInspected;

				case RccHousekeepingStatusCode.HD:
					return RccHousekeepingStatus.Dirty;

				case RccHousekeepingStatusCode.HP:
					return RccHousekeepingStatus.HkInProgress;

				case RccHousekeepingStatusCode.LUG:
					return RccHousekeepingStatus.Luggage;

				case RccHousekeepingStatusCode.NO:
					return RccHousekeepingStatus.NoChanges;

				case RccHousekeepingStatusCode.OHC:
					return RccHousekeepingStatus.OccupiedClean;

				case RccHousekeepingStatusCode.OHCI:
					return RccHousekeepingStatus.OccupiedCleanInspected;

				case RccHousekeepingStatusCode.OHD:
					return RccHousekeepingStatus.OccupiedDirty;

				case RccHousekeepingStatusCode.OHP:
					return RccHousekeepingStatus.OccupiedHkInProgress;

				case RccHousekeepingStatusCode.OOO:
					return RccHousekeepingStatus.OutOfOrder;

				case RccHousekeepingStatusCode.OOS:
					return RccHousekeepingStatus.OutOfService;

				case RccHousekeepingStatusCode.PU:
					return RccHousekeepingStatus.Pickup;

				case RccHousekeepingStatusCode.TD:
					return RccHousekeepingStatus.TidyUpRequired;

				case RccHousekeepingStatusCode.TDNR:
					return RccHousekeepingStatus.TurnDownNotRequests;
					return RccHousekeepingStatus.TurnDownDone; // YES IT CAN RETURN TWO DIFFERENT STATUSES - NOT DETERMINED REALLY. Yes I know the second line is not reachable.

				case RccHousekeepingStatusCode.TDR:
					return RccHousekeepingStatus.TurnDownRequests;

				case RccHousekeepingStatusCode.VHC:
					return RccHousekeepingStatus.VacantClean;

				case RccHousekeepingStatusCode.VHCI:
					return RccHousekeepingStatus.VacantCleanInspected;

				case RccHousekeepingStatusCode.VHD:
					return RccHousekeepingStatus.VacantDirty;

				case RccHousekeepingStatusCode.VHP:
					return RccHousekeepingStatus.VacantHkInProgress;
			}

			throw new NotSupportedException($"Conversion is not defined for: RccHousekeepingStatusCode.{code.ToString()} -> RccHousekeepingStatus");
		}

		public static RccHousekeepingStatusCode ToCode(this RccHousekeepingStatus status)
		{
			switch (status)
			{
				case RccHousekeepingStatus.DoNotDisturb:
					return RccHousekeepingStatusCode.DNN;

				case RccHousekeepingStatus.Clean:
					return RccHousekeepingStatusCode.HC;

				case RccHousekeepingStatus.CleanInspected:
					return RccHousekeepingStatusCode.HCI;

				case RccHousekeepingStatus.Dirty:
					return RccHousekeepingStatusCode.HD;

				case RccHousekeepingStatus.HkInProgress:
					return RccHousekeepingStatusCode.HP;

				case RccHousekeepingStatus.Luggage:
					return RccHousekeepingStatusCode.LUG;

				case RccHousekeepingStatus.NoChanges:
					return RccHousekeepingStatusCode.NO;

				case RccHousekeepingStatus.OccupiedClean:
					return RccHousekeepingStatusCode.OHC;

				case RccHousekeepingStatus.OccupiedCleanInspected:
					return RccHousekeepingStatusCode.OHCI;

				case RccHousekeepingStatus.OccupiedDirty:
					return RccHousekeepingStatusCode.OHD;

				case RccHousekeepingStatus.OccupiedHkInProgress:
					return RccHousekeepingStatusCode.OHP;

				case RccHousekeepingStatus.OutOfOrder:
					return RccHousekeepingStatusCode.OOO;

				case RccHousekeepingStatus.OutOfService:
					return RccHousekeepingStatusCode.OOS;

				case RccHousekeepingStatus.Pickup:
					return RccHousekeepingStatusCode.PU;

				case RccHousekeepingStatus.TidyUpRequired:
					return RccHousekeepingStatusCode.TD;

				case RccHousekeepingStatus.TurnDownNotRequests:
					return RccHousekeepingStatusCode.TDNR;

				case RccHousekeepingStatus.TurnDownDone:
					return RccHousekeepingStatusCode.TDNR;

				case RccHousekeepingStatus.TurnDownRequests:
					return RccHousekeepingStatusCode.TDR;

				case RccHousekeepingStatus.VacantClean:
					return RccHousekeepingStatusCode.VHC;

				case RccHousekeepingStatus.VacantCleanInspected:
					return RccHousekeepingStatusCode.VHCI;

				case RccHousekeepingStatus.VacantDirty:
					return RccHousekeepingStatusCode.VHD;

				case RccHousekeepingStatus.VacantHkInProgress:
					return RccHousekeepingStatusCode.VHP;
			}

			throw new NotSupportedException($"Conversion is not defined for: RccHousekeepingStatus.{status.ToString()} -> RccHousekeepingStatus");
		}
	}
}
