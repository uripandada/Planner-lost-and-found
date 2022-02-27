using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Reservations.Queries.GetListOfReservationsForMobile
{
	public class MobileReservation
	{
		public string Id { get; set; }
		public string Hotel_id { get; set; }

		public string Pms_id { get; set; }
		/// <summary>
		/// DEPRECATED. Not used any more.
		/// </summary>
		public string Source { get; set; }

		public string Guest_name { get; set; }
		public string Room_name { get; set; }
		public Guid? Room_id { get; set; }
		public string Bed_name { get; set; }
		public Guid? Bed_id { get; set; }
		public int? Floor_number { get; set; }
		public Guid? Floor_id { get; set; }
		public DateTime? Check_in_date { get; set; }
		public DateTime? Check_out_date { get; set; }
		public long? Expected_arrival_ts { get; set; }
		public long? Arrival_ts { get; set; }
		public long? Expected_departure_ts { get; set; }
		public long? Departure_ts { get; set; }
		public int Occupants { get; set; }
		public int Adults { get; set; }
		public int Children { get; set; }
		public int Infants { get; set; }
		public string Group_name { get; set; }
		public string Pms_note { get; set; }
		public string Rc_note { get; set; }
		/// <summary>
		/// TODO: Find out what really is_active numbers mean.
		/// </summary>
		public int Is_active { get; set; } = 1;
		public int Is_vip { get; set; } = 0;
		public string Vip { get; set; }
		public int Is_priority { get; set; } = 0;
		public int Is_arrival { get; set; } = 0;

		/// <summary>
		/// Prior room id
		/// DEPRECATED! Not used any more.
		/// </summary>
		public string Rm_prior { get; set; } = "";
		/// <summary>
		/// Current room id
		/// DEPRECATED! Not used any more.
		/// </summary>
		public string Rm_current { get; set; } = "";
		/// <summary>
		/// Who knows what this was used for?
		/// DEPRECATED! Not used any more.
		/// </summary>
		public string Guest { get; set; } = "";

		/// <summary>
		/// ADDED!
		/// </summary>
		public bool IsTemporaryRoom { get; set; }

	}

	public class GetListOfReservationsForMobileQuery : IRequest<IEnumerable<MobileReservation>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfReservationsForMobileQueryHandler : IRequestHandler<GetListOfReservationsForMobileQuery, IEnumerable<MobileReservation>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public GetListOfReservationsForMobileQueryHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<IEnumerable<MobileReservation>> Handle(GetListOfReservationsForMobileQuery request, CancellationToken cancellationToken)
		{
			try
			{
				var hotelLocalDateProvider = new HotelLocalDateProvider();
				var date = await hotelLocalDateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId);
				//var date = await this._GetHotelCurrentLocalDate(request.HotelId);

				var fromCheckOutDate = date.AddDays(0);
				var toCheckInDate = date.AddDays(1);
				var reservations = await this._databaseContext.Reservations
					.Include(r => r.Room)
					.ThenInclude(rm => rm.Floor)
					.Include(r => r.RoomBed)
					.Where(r => r.HotelId == request.HotelId && r.IsActive && (r.CheckIn.HasValue && r.CheckIn.Value <= toCheckInDate) && (r.CheckOut.HasValue && r.CheckOut.Value >= fromCheckOutDate))
					.ToArrayAsync();

				var ress = reservations.Select(r =>
				{
					var dates = this._GetReservationArrivalDepartureDates(r);

					var res = new MobileReservation
					{
						Occupants = r.NumberOfAdults + r.NumberOfChildren + r.NumberOfInfants,
						Adults = r.NumberOfAdults,
						Children = r.NumberOfChildren,
						Infants = r.NumberOfInfants,
						Arrival_ts = r.ActualCheckIn.HasValue ? r.ActualCheckIn.Value.ConvertToTimestamp() : null,
						Check_in_date = dates.ArrivalDate,
						Check_out_date = dates.DepartureDate,
						Departure_ts = dates.DepartedDate.HasValue ? dates.DepartedDate.Value.ConvertToTimestamp() : null,
						Expected_arrival_ts = dates.ArrivalDate.HasValue ? dates.ArrivalDate.Value.ConvertToTimestamp() : null,
						Expected_departure_ts = dates.DepartureDate.HasValue ? dates.DepartureDate.Value.ConvertToTimestamp() : null,
						//Floor_id = r.Room?.Floor?.Id,
						//Floor_number = r.Room?.Floor?.Number,
						Group_name = r.Group,
						Guest_name = r.GuestName,
						IsTemporaryRoom = r.Room?.Floor == null,
						Hotel_id = r.HotelId,
						Id = r.Id,
						Is_active = r.IsActive ? 1 : 0,
						Is_arrival = r.RccReservationStatusKey == Common.Enums.RccReservationStatus.Arrival.ToString() || r.RccReservationStatusKey == Common.Enums.RccReservationStatus.Arrived.ToString() ? 1 : 0,
						Is_priority = 0,
						Is_vip = string.IsNullOrWhiteSpace(r.Vip) ? 0 : 1,
						Vip = r.Vip,
						Pms_id = null,
						Pms_note = r.PmsNote,
						Rc_note = "",
						//Room_id = r.Room?.Id,
						//Room_name = r.Room?.Name,
						Source = "", // LEAVE THIS ""
						Rm_current = null, // LEAVE THIS null! THE REPERCUSSIONS ARE UNKNOWN!
						Rm_prior = null, // LEAVE THIS null! THE REPERCUSSIONS ARE UNKNOWN!
						Guest = "", // LEAVE THIS EMPTY! THE REPERCUSSIONS ARE UNKNOWN!
					};

					if (r.Room != null)
					{
						res.Room_id = r.Room.Id;
						res.Room_name = r.Room.Name;

						if (r.Room.Floor == null)
						{
							res.IsTemporaryRoom = true;
						}
						else
						{
							res.Floor_id = r.Room.Floor.Id;
							res.Floor_number = r.Room.Floor.Number;
						}
					}

					if (r.RoomBed != null)
					{
						res.Bed_id = r.RoomBed.Id;
						res.Bed_name = r.RoomBed.Name;
					}

					return res;
				}).ToArray();

				return ress;
			}
			catch (Exception ex)
			{
				throw new Exception("EXCEPTION WHILE RETURNING RESERVATIONS!" + ex.Message);
			}

		}

		private (DateTime? ArrivalDate, DateTime? ArrivedDate, DateTime? DepartureDate, DateTime? DepartedDate) _GetReservationArrivalDepartureDates(Domain.Entities.Reservation r)
		{
			var arrivalDate = r.CheckIn;
			var arrivedDate = r.ActualCheckIn;
			var departureDate = r.CheckOut;
			var departedDate = r.ActualCheckOut;

			if (r.RccReservationStatusKey == Common.Enums.RccReservationStatus.Departed.ToString())
			{
				if (departedDate == null)
				{
					departedDate = departureDate;
				}
				else if (departureDate == null)
				{
					departureDate = departedDate;
				}

				if (arrivedDate == null)
				{
					arrivedDate = arrivalDate;
				}
				else if (arrivalDate == null)
				{
					arrivalDate = arrivedDate;
				}
			}
			else if (r.RccReservationStatusKey == Common.Enums.RccReservationStatus.Departure.ToString())
			{
				if (departureDate == null)
				{
					departureDate = departedDate;
				}

				if (arrivedDate == null)
				{
					arrivedDate = arrivalDate;
				}
				else if (arrivalDate == null)
				{
					arrivalDate = arrivedDate;
				}
			}
			else if (r.RccReservationStatusKey == Common.Enums.RccReservationStatus.Current.ToString())
			{
				if (arrivedDate == null)
				{
					arrivedDate = arrivalDate;
				}
				else if (arrivalDate == null)
				{
					arrivalDate = arrivedDate;
				}
			}
			else if (r.RccReservationStatusKey == Common.Enums.RccReservationStatus.Arrived.ToString())
			{
				if (arrivedDate == null)
				{
					arrivedDate = arrivalDate;
				}
				else if (arrivalDate == null)
				{
					arrivalDate = arrivedDate;
				}
			}
			else if (r.RccReservationStatusKey == Common.Enums.RccReservationStatus.Arrival.ToString())
			{
				if (arrivalDate == null)
				{
					arrivalDate = arrivedDate;
				}
			}

			return (arrivalDate, arrivedDate, departureDate, departedDate);
		}
	}
}
