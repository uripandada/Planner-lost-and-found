using Planner.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Domain.Entities
{
	public static class RoomMessageQueryExtensions
	{
		public static IQueryable<RoomMessage> FilterByDate(this IQueryable<RoomMessage> query, DateTime date)
		{
			return query.Where(m => m.RoomMessageDates.Any(md => md.Date == date));
		}
		public static IQueryable<RoomMessage> FilterByRoomId(this IQueryable<RoomMessage> query, Guid roomId, DateTime? date)
		{
			var q = query.Where(m => m.RoomMessageRooms.Any(md => md.RoomId == roomId));

			if (date.HasValue)
			{
				q = q.FilterByDate(date.Value);
			}

			return q;
		}
		public static IQueryable<RoomMessage> FilterByReservationId(this IQueryable<RoomMessage> query, string reservationId, DateTime? date)
		{
			var q = query.Where(m => m.RoomMessageReservations.Any(md => md.ReservationId == reservationId));

			if (date.HasValue)
			{
				q = q.FilterByDate(date.Value);
			}

			return q;
		}
	}

	public class RoomMessage: BaseEntity
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

		public IEnumerable<RoomMessageFilter> RoomMessageFilters { get; set; }
		public IEnumerable<RoomMessageDate> RoomMessageDates { get; set; }
		public IEnumerable<RoomMessageRoom> RoomMessageRooms { get; set; }
		public IEnumerable<RoomMessageReservation> RoomMessageReservations { get; set; }
	}

	public class RoomMessageFilter
	{
		public Guid Id { get; set; }
		public Guid RoomMessageId { get; set; }
		public RoomMessage RoomMessage { get; set; }

		public string ReferenceId { get; set; }
		public RoomMessageFilterReferenceType ReferenceType { get; set; }
		public string ReferenceName { get; set; }
		public string ReferenceDescription { get; set; }

	}
	public class RoomMessageDate
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public Guid RoomMessageId { get; set; }
		public RoomMessage RoomMessage { get; set; }
	}
	public class RoomMessageRoom
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public Guid RoomMessageId { get; set; }
		public RoomMessage RoomMessage { get; set; }
		public Guid RoomId { get; set; }
		public Room Room { get; set; }
		public Guid? RoomBedId { get; set; }
		public Room RoomBed { get; set; }

	}
	public class RoomMessageReservation
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public Guid RoomMessageId { get; set; }
		public RoomMessage RoomMessage { get; set; }
		public string ReservationId { get; set; }
		public Reservation Reservation { get; set; }

	}
}
