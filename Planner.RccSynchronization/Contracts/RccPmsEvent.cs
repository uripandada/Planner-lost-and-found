namespace Planner.RccSynchronization.Contracts
{
	public class RccPmsEvent
	{
		public string ReservationId { get; set; }
		public string EventName { get; set; }
		public string RoomName { get; set; }
		public string OldRoomName { get; set; }
		public string Name { get; set; }
		public int? Adults { get; set; }
		public int? Children { get; set; }
		public string Country { get; set; }
		public string CiDate { get; set; } //": "2022-03-24T15:00:00",
		public string CoDate { get; set; } //": "2022-06-15T12:00:00",
		public string Status { get; set; } //": "Arrival",
		public string Notes { get; set; } //": " | RATE:SEMB2B05",
		public string HotelName { get; set; } //": null,
		public string Timestamp { get; set; } //": "2021-12-17T18:37:46.717",
		public bool? IsRaw { get; set; }
	}
}
