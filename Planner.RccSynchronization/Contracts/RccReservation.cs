using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.RccSynchronization.Contracts
{
	public class RccReservation
	{

		public string ReservationId { get; set; }
		public string RcRoomName { get; set; }
		//public string RcRoomBedName { get; set; }
		public string PMSRoomName { get; set; }
		public string ParentRoomName { get; set; }
		public string Name { get; set; }
		public DateTime? CheckIn { get; set; }
		public DateTime? ActualCheckIn { get; set; }
		public DateTime? CheckOut { get; set; }
		public DateTime? ActualCheckOut { get; set; }
		public string Status { get; set; }
		public int? Adults { get; set; }
		public int? Children { get; set; }
		public int? Infants { get; set; }
		public string PmsNote { get; set; }
		public string Vip { get; set; }
		public IEnumerable<RccOtherProperty> OtherProperties { get; set; }
		public string GroupName { get; set; }
	}
}
