using Planner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.RoomMessages
{
	public class SaveSimpleRoomMessage
	{
		public string HotelId { get; set; }
		public RoomMessageForType ForType { get; set; }
		public string[] ReservationIds { get; set; }
		public Guid? RoomId { get; set; }
		public string Message { get; set; }
	}

	public class SaveComplexRoomMessage
	{
		public string HotelId { get; set; }
		public string Message { get; set; }
		public RoomMessageType Type { get; set; }
		public RoomMessageForType ForType { get; set; }
		public RoomMessageDateType DateType { get; set; }

		public string IntervalStartDate { get; set; }
		public string IntervalEndDate { get; set; }
		public int? IntervalEveryNumberOfDays { get; set; }

		public bool? ReservationOnArrivalDate { get; set; }
		public bool? ReservationOnDepartureDate { get; set; }
		public bool? ReservationOnStayDates { get; set; }
		public IEnumerable<SaveRoomMessageFilter> Filters { get; set; }
		public IEnumerable<string> Dates { get; set; }
	}

	public class SaveRoomMessageFilter
	{
		public string ReferenceId { get; set; }
		public RoomMessageFilterReferenceType ReferenceType { get; set; }
		public string ReferenceName { get; set; }
		public string ReferenceDescription { get; set; }
	}

	public class RoomMessageListItem
	{
		public Guid Id { get; set; }
		public string Message { get; set; }
		public string Description { get; set; }
		public string CreatedByName { get; set; }
		public string CreatedAtString { get; set; }

	}

	public class RoomMessageDetails
	{
		public Guid Id { get; set; }
		public string Message { get; set; }
		public bool IsDeleted { get; set; }

		public RoomMessageType Type { get; set; }
		public RoomMessageForType ForType { get; set; }
		public RoomMessageDateType DateType { get; set; }

		public DateTime? IntervalStartDate { get; set; }
		public DateTime? IntervalEndDate { get; set; }
		public int? IntervalNumberOfDays { get; set; }

		public bool? ReservationOnArrivalDate { get; set; }
		public bool? ReservationOnDepartureDate { get; set; }
		public bool? ReservationOnStayDates { get; set; }

		public IEnumerable<RoomMessageDetailsFilter> RoomMessageFilters { get; set; }
		public IEnumerable<DateTime> Dates { get; set; }
	}

	public class RoomMessageDetailsFilter
	{
		public Guid Id { get; set; }

		public string ReferenceId { get; set; }
		public RoomMessageFilterReferenceType ReferenceType { get; set; }
		public string ReferenceName { get; set; }
		public string ReferenceDescription { get; set; }

	}

	public class RoomMessageFilterValues
	{
		public IEnumerable<RoomMessageFilterGroup> TodayFilterValues { get; set;}
		public IEnumerable<RoomMessageFilterGroup> PlacesFilterValues { get; set;}
		public IEnumerable<RoomMessageFilterGroup> ReservationsFilterValues { get; set;}
	}

	public class RoomMessageFilterGroup
	{
		public RoomMessageFilterReferenceType ReferenceType { get; set; }
		public string Name { get; set; }
		public IEnumerable<RoomMessageFilterGroupItem> Items { get; set; }
	}

	public class RoomMessageFilterGroupItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
