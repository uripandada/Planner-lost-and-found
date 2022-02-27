using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Domain.Entities
{
	public class CleaningGeneratorLog
	{
		public Guid Id { get; set; }
		public Guid GenerationId { get; set; }
		public DateTime At { get; set; }
		public DateTime CleaningPlanDate { get; set; }
		public string Message { get; set; }
		public string RoomDescription { get; set; }
		public string ReservationsDescription { get; set; }
		public string ReservationsEventsDescription { get; set; }
		public string PluginEventsDescription { get; set; }
		public string OrderedPluginsDescription { get; set; }
		public string CleaningEventsDescription { get; set; }
		public string CleaningsDescription { get; set; }
		public string HotelId { get; set; }
	}
}
