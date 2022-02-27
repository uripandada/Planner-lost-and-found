using System;
using System.Text;

namespace Planner.Application.Infrastructure
{
	public class HotelTimeZone
	{
		public string HotelId { get; set; }
		public string WindowsTimeZoneId { get; set; }
		public string IanaTimeZoneId { get; set; }
		public DateTime CurrentHotelTime { get; set; }
	}
}
