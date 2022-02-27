using System;
using System.Collections.Generic;

namespace Planner.Application.AutomaticHousekeepingUpdateSettingss.Models
{
	public class AutomaticHousekeepingUpdateSettingsListItem
	{
		public Guid Id { get; set; }
		public bool Dirty { get; set; }
		public bool Clean { get; set; }
		public bool CleanNeedsInspection { get; set; }
		public bool Inspected { get; set; }
		public bool Vacant { get; set; }
		public bool Occupied { get; set; }
		public bool DoNotDisturb { get; set; }
		public bool DoDisturb { get; set; }
		public bool OutOfService { get; set; }
		public bool InService { get; set; }
		public string RoomNameRegex { get; set; }
		public Common.Enums.AutomaticHousekeepingUpdateCleaningStatusTo UpdateStatusTo { get; set; }
		public Common.Enums.AutomaticHousekeepingUpdateCleaningStatusWhen UpdateStatusWhen { get; set; }
		public string UpdateStatusAtTime { get; set; }
	}
}
