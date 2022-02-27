using Planner.Common.Enums;
using System;

namespace Planner.Domain.Entities
{
	public class AutomaticHousekeepingUpdateSettings
	{
		public Guid Id { get; set; }
		public DateTime CreatedAt { get; set; }
		public string HotelId { get; set; }
		public Hotel Hotel { get; set; }

		public bool Dirty { get; set; } // Room flag isDirty = true,
		public bool Clean { get; set; } // Room flag isDirty = false,
		public bool CleanNeedsInspection { get; set; } 
		public bool Inspected { get; set; } 
		public bool Vacant { get; set; } 
		public bool Occupied { get; set; } 
		public bool DoNotDisturb { get; set; } 
		public bool DoDisturb { get; set; } 
		public bool OutOfService { get; set; } 
		public bool InService { get; set; } 
		public string RoomNameRegex { get; set; } 
		
		public AutomaticHousekeepingUpdateCleaningStatusTo UpdateStatusTo { get; set; } 
		public AutomaticHousekeepingUpdateCleaningStatusWhen UpdateStatusWhen { get; set; }
		public string UpdateStatusAtTime { get; set; }
	}
}
