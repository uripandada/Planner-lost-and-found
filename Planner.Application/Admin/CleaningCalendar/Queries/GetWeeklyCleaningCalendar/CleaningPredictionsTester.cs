using Planner.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningCalendar.Queries.GetWeeklyCleaningCalendar
{
	public class CleaningPredictionsTester
	{
		public class CleaningCalendarTesterRoom : CleaningCalendarRoom
		{
			public string BasedOnsDescription { get; set; }
			public string ReservationsDescription { get; set; }

			public bool IsOutOfService { get; set; }
			public bool IsDoNotDisturb { get; set; }
			public string Section { get; set; }
			public string SubSection { get; set; }
			public string FloorId { get; set; }
			public List<CleaningCalendarTesterDay> Days { get; set; }
		}

		public class CleaningCalendarTesterDay : CleaningCalendarDay
		{
			public CleaningCalendarTesterDay()
			{
				this.Reservations = new List<CleaningCalendarReservation>();
				this.Cleanings = new List<CleaningCalendarTesterCleaning>();
			}

			public List<CleaningCalendarTesterCleaning> Cleanings { get; set; }
		}

		public class CleaningCalendarTesterCleaning : CleaningCalendarCleaning
		{
			public string CleaningType { get; set; }
			public int Credits { get; set; }
		}
		public class PredictionsTesterRequest
		{
			public string BasedOnKey { get; set; }
			public string ReservationsKey { get; set; }
		}

		public class PredictionsTesterResult
		{
			public IEnumerable<CalendarDay> Days { get; set; }
			public IEnumerable<CleaningCalendarTesterRoom> Rooms { get; set; }
			public IEnumerable<PredictionsTesterError> ErrorResults { get; set; }

		}

		public class PredictionsTesterError
		{
			public string RoomName { get; set; }
			public DateTime CleaningDate { get; set; }
			public int NumberOfReservations { get; set; }
			public int NumberOfPlugins { get; set; }
			public string Message { get; set; }
		}

		private Random _randomGenerator;
		//private Dictionary<Guid, string> _buildings { get; set; }
		//private Dictionary<Guid, Dictionary<Guid, string>> _buildingFloors { get; set; }
		//private Dictionary<Guid, Dictionary<Guid, string>> _floorSections { get; set; }
		//private Dictionary<Guid, Dictionary<Guid, string>> _sectionSubSections { get; set; }
		private readonly ICleaningProvider _cleaningProvider;
		public CleaningPredictionsTester()
		{
			//this._buildings = new Dictionary<Guid, string>();
			//this._buildingFloors = new Dictionary<Guid, Dictionary<Guid, string>>();
			//this._floorSections = new Dictionary<Guid, Dictionary<Guid, string>>();
			//this._sectionSubSections = new Dictionary<Guid, Dictionary<Guid, string>>();


			this._cleaningProvider = new CleaningProvider();
			this._randomGenerator = new Random();

			this.studentCategory = new CleaningProviderRequest.RoomCategory { Id = Guid.NewGuid(), Name = "Student", /*Credits = 20*/ };
			this.regularCategory = new CleaningProviderRequest.RoomCategory { Id = Guid.NewGuid(), Name = "Regular", /*Credits = 30*/ };
			this.deluxeCategory = new CleaningProviderRequest.RoomCategory { Id = Guid.NewGuid(), Name = "Deluxe", /*Credits = 40*/ };
		}


		private DateTime _cleaningStartDate { get; set; }
		private int _numberOfCleaningDays { get; set; }
		private List<CleaningProviderRequest.Room> _rooms { get; set; }
		private List<CalendarDay> _calendarDays { get; set; }
		private List<Domain.Entities.Floor> Floors { get; set; }



		private CleaningProviderRequest.RoomCategory studentCategory; 
		private CleaningProviderRequest.RoomCategory regularCategory; 
		private CleaningProviderRequest.RoomCategory deluxeCategory; 
		
		//{ get; set; } = this._GenerateCategory("Student", 20);
		//var regularCategory = this._GenerateCategory("Regular", 30);
		//var deluxeCategory = this._GenerateCategory("Deluxe", 40);

		private int roomNumber = 0;
		private int reservationNumber = 0;

		private enum ReservationStackType
		{
			VACANT,
			STAY,
			CHECKIN,
			CHECKOUT,
			DAYSTAY,
			DOUBLE_STAY,
			DOUBLE_STAY_CHECKIN,
			DOUBLE_STAY_CHECKOUT,
			DOUBLE_STAY_DAYSTAY,
			DOUBLE_CHECKIN,
			DOUBLE_CHECKOUT_CHECKIN,
			DOUBLE_CHECKOUT_CHECKIN_OVERLAPPED,
			DOUBLE_CHECKOUT,
			DOUBLE_CHECKOUT_DAYSTAY_CONTAINED,
			DOUBLE_CHECKOUT_DAYSTAY_OVERLAPPED,
			DOUBLE_CHECKOUT_DAYSTAY,
			DOUBLE_DAYSTAY,
			DOUBLE_DAYSTAY_CONTAINED,
			DOUBLE_DAYSTAY_OVERLAPPED,
			DOUBLE_CHECKIN_DAYSTAY_CONTAINED,
			DOUBLE_CHECKIN_DAYSTAY_OVERLAPPED,
			DOUBLE_CHECKIN_DAYSTAY,

			TRIPLE_DAYSTAY,
			TRIPLE_DAYSTAY_CONTAINED,
			TRIPLE_DAYSTAY_OVERLAPPED,
			TRIPLE_DAYSTAY_CASCADE,

			TRIPLE_STAY_CHECKIN_CHECKIN,
			TRIPLE_STAY_CHECKOUT_CHECKIN,
			TRIPLE_STAY_CHECKOUT_CHECKIN_OVERLAPPED,
			TRIPLE_STAY_CHECKOUT_CHECKOUT,
			TRIPLE_STAY_CHECKOUT_DAYSTAY_CONTAINED,
			TRIPLE_STAY_CHECKOUT_DAYSTAY_OVERLAPPED,
			TRIPLE_STAY_CHECKOUT_DAYSTAY,
			TRIPLE_STAY_DAYSTAY_DAYSTAY,
			TRIPLE_STAY_DAYSTAY_CONTAINED,
			TRIPLE_STAY_DAYSTAY_OVERLAPPED,
			TRIPLE_STAY_CHECKIN_DAYSTAY_CONTAINED,
			TRIPLE_STAY_CHECKIN_DAYSTAY_OVERLAPPED,
			TRIPLE_STAY_CHECKIN_DAYSTAY,

		}

		private IEnumerable<CleaningProviderRequest.RoomTest> _CreateRooms(DateTime date, DateTime minDate, DateTime maxDate, string reservationKey)
		{
			var alllRooms = new List<CleaningProviderRequest.RoomTest>();
			var rooms = this._CreateVacantRoomVariations();
			var reservationStackType = (ReservationStackType)Enum.Parse(typeof(ReservationStackType), reservationKey);

			foreach (var room in rooms)
			{
				room.Reservations = this._CreateReservations(room.Name, reservationStackType, date, minDate, maxDate);
				room.Description = reservationStackType.ToString();
				alllRooms.Add(room);
			}
			//foreach (var type in Enum.GetValues(typeof(ReservationStackType)).Cast<ReservationStackType>())
			//{
			//	var rooms = this._CreateVacantRoomVariations();

			//	foreach (var room in rooms)
			//	{
			//		room.Reservations = this._CreateReservations(room.Name, type, date, minDate, maxDate);
			//		alllRooms.Add(room);
			//	}
			//}


			//switch (reservationKey)
			//{
			//	case "": break;
			//	case "VACANT":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "STAY":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "CHECKIN":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "CHECKOUT":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "DAYSTAY":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "DOUBLE_STAY": break;
			//	case "DOUBLE_STAY_CHECKIN": break;
			//	case "DOUBLE_STAY_CHECKOUT": break;
			//	case "DOUBLE_STAY_DAYSTAY": break;
			//	case "DOUBLE_CHECKIN": break;
			//	case "DOUBLE_CHECKOUT_CHECKIN": break;
			//	case "DOUBLE_CHECKOUT_CHECKIN_OVERLAPPED": break;
			//	case "DOUBLE_CHECKOUT": break;
			//	case "DOUBLE_CHECKOUT_DAYSTAY_CONTAINED": break;
			//	case "DOUBLE_CHECKOUT_DAYSTAY_OVERLAPPED": break;
			//	case "DOUBLE_CHECKOUT_DAYSTAY": break;
			//	case "DOUBLE_DAYSTAY": break;
			//	case "DOUBLE_DAYSTAY_CONTAINED": break;
			//	case "DOUBLE_DAYSTAY_OVERLAPPED,": break;
			//	case "DOUBLE_CHECKIN_DAYSTAY_CONTAINED": break;
			//	case "DOUBLE_CHECKIN_DAYSTAY_OVERLAPPED": break;
			//	case "DOUBLE_CHECKIN_DAYSTAY": break;
			//	case "TRIPLE_DAYSTAY": break;
			//	case "TRIPLE_DAYSTAY_CONTAINED": break;
			//	case "TRIPLE_DAYSTAY_OVERLAPPED": break;
			//	case "TRIPLE_DAYSTAY_CASCADE": break;
			//	case "TRIPLE_STAY_CHECKIN_CHECKIN": break;
			//	case "TRIPLE_STAY_CHECKOUT_CHECKIN": break;
			//	case "TRIPLE_STAY_CHECKOUT_CHECKIN_OVERLAPPED":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_CHECKOUT_CHECKOUT":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_CHECKOUT_DAYSTAY_CONTAINED":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_CHECKOUT_DAYSTAY_OVERLAPPED":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_CHECKOUT_DAYSTAY":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_DAYSTAY_DAYSTAY":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_DAYSTAY_CONTAINED":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_DAYSTAY_OVERLAPPED":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_CHECKIN_DAYSTAY_CONTAINED":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_CHECKIN_DAYSTAY_OVERLAPPED":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//	case "TRIPLE_STAY_CHECKIN_DAYSTAY":
			//		foreach (var room in rooms)
			//		{
			//			room.Reservations = this._CreateReservations(room.Name, ReservationStackType.VACANT, date, minDate, maxDate);
			//			room.Description = ReservationStackType.VACANT.ToString();
			//			alllRooms.Add(room);
			//		}
			//		break;
			//}


			return alllRooms;
		}

		private CleaningProviderRequest.Reservation _CreateReservation(DateTime checkIn, DateTime checkOut, string externalId, Dictionary<string, string> otherProperties = null)
		{
			this.reservationNumber++;
			var reservation = new CleaningProviderRequest.Reservation
			{
				CheckIn = checkIn,
				CheckOut = checkOut,
				ExternalId = externalId,
				GuestName = $"Guest {this.reservationNumber}",
				Id = $"R-{this.reservationNumber}",
				IsActive = true,
				IsCheckedIn = false,
				IsCheckedOut = false,
				OtherProperties = otherProperties ?? new Dictionary<string, string>()
			};
			return reservation;
		}

		private CleaningProviderRequest.Reservation[] _CreateReservations(string roomName, ReservationStackType type, DateTime date, DateTime minDate, DateTime maxDate)
		{
			var reservations = new List<CleaningProviderRequest.Reservation>();
			switch (type)
			{
				case ReservationStackType.VACANT:
					break;
				case ReservationStackType.STAY:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName, new Dictionary<string, string> { { "ReservationSpaceCategory", "TEST_RESERVATION_SPACE_CATEGORY" }, { "OP_KEY_2", "TEST_OP_2" } }));
					break;
				case ReservationStackType.CHECKIN:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), maxDate, roomName));
					break;
				case ReservationStackType.CHECKOUT:
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(10, 0, 0)), roomName));
					break;
				case ReservationStackType.DAYSTAY:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(12, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_STAY:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName, new Dictionary<string, string> { { "ReservationSpaceCategory", "TEST_RESERVATION_SPACE_CATEGORY" }, { "Product", "TEST_PRODUCT_TAG" } }));
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName, new Dictionary<string, string> { { "OP_KEY_1", "TEST_OP_1" } }));
					break;
				case ReservationStackType.DOUBLE_STAY_CHECKIN:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), maxDate, roomName));
					break;
				case ReservationStackType.DOUBLE_STAY_CHECKOUT:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(10, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_STAY_DAYSTAY:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(12, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKIN:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(16, 0, 0)), maxDate, roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKOUT_CHECKIN:
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(12, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), maxDate, roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKOUT_CHECKIN_OVERLAPPED:
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(14, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), maxDate, roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKOUT:
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(10, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(12, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKOUT_DAYSTAY_CONTAINED:
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(14, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(12, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKOUT_DAYSTAY_OVERLAPPED:
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(12, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKOUT_DAYSTAY:
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(10, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_DAYSTAY:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(12, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_DAYSTAY_CONTAINED:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_DAYSTAY_OVERLAPPED:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKIN_DAYSTAY_CONTAINED:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKIN_DAYSTAY_OVERLAPPED:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.DOUBLE_CHECKIN_DAYSTAY:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(16, 0, 0)), maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_DAYSTAY:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(8, 0, 0)), date.Add(new TimeSpan(10, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(16, 0, 0)), date.Add(new TimeSpan(18, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_DAYSTAY_CONTAINED:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(8, 0, 0)), date.Add(new TimeSpan(10, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(18, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_DAYSTAY_OVERLAPPED:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(8, 0, 0)), date.Add(new TimeSpan(10, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), date.Add(new TimeSpan(18, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_DAYSTAY_CASCADE:
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), date.Add(new TimeSpan(18, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKIN_CHECKIN:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(16, 0, 0)), maxDate, roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKOUT_CHECKIN:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(12, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), maxDate, roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKOUT_CHECKIN_OVERLAPPED:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(14, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), maxDate, roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKOUT_CHECKOUT:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(10, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(12, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKOUT_DAYSTAY_CONTAINED:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(14, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(12, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKOUT_DAYSTAY_OVERLAPPED:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(12, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKOUT_DAYSTAY:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(minDate, date.Add(new TimeSpan(10, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_DAYSTAY_DAYSTAY:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(12, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_DAYSTAY_CONTAINED:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_DAYSTAY_OVERLAPPED:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(10, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKIN_DAYSTAY_CONTAINED:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKIN_DAYSTAY_OVERLAPPED:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(14, 0, 0)), maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(16, 0, 0)), roomName));
					break;
				case ReservationStackType.TRIPLE_STAY_CHECKIN_DAYSTAY:
					reservations.Add(this._CreateReservation(minDate, maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(16, 0, 0)), maxDate, roomName));
					reservations.Add(this._CreateReservation(date.Add(new TimeSpan(12, 0, 0)), date.Add(new TimeSpan(14, 0, 0)), roomName));
					break;
				default:
					break;
			}

			return reservations.ToArray();
		}


		public PredictionsTesterResult CalculateCleanings(DateTime cleaningDate, string reservationKey, string basedOnKey)
		{
			this._cleaningStartDate = this._GetNextWeekday(cleaningDate, DayOfWeek.Monday).Date;
			this._numberOfCleaningDays = 14;
			this._calendarDays = this._GenerateCalendarDays(this._cleaningStartDate, this._numberOfCleaningDays);

			var errors = new List<PredictionsTesterError>();

			// do everything on on all days of the week
			var mondayDate = this._cleaningStartDate;
			var sundayBefore = mondayDate.AddDays(-1);
			var tuesdayDate = mondayDate.AddDays(1);
			var wednesdayDate = mondayDate.AddDays(2);
			var thursdayDate = mondayDate.AddDays(3);
			var fridayDate = mondayDate.AddDays(4);
			var saturdayDate = mondayDate.AddDays(5);
			var sundayDate = mondayDate.AddDays(6);
			var mondayAfter = mondayDate.AddDays(7);
			var dates = new DateTime[] { 
				mondayDate, tuesdayDate, 
				wednesdayDate, 
				thursdayDate, fridayDate, saturdayDate, sundayDate,
				sundayDate.AddDays(1),
				sundayDate.AddDays(2),
				sundayDate.AddDays(3),
				sundayDate.AddDays(4),
				sundayDate.AddDays(5),
				sundayDate.AddDays(6),
				sundayDate.AddDays(7)
			};

			var plugins = this._GeneratePlugins(new Guid[0], new Guid[0], new string[0], new string[0], new string[0], new string[0], new CleaningPluginBasedOnRoomCategory[0], basedOnKey);
			var testerRooms = new List<CleaningCalendarTesterRoom>();
			//var cleanings = new List<CleaningProviderRequest.Cleaning>();


			var rooms = this._GenerateFullSetOfRoomsWithWeeklyReservations(dates, sundayBefore, mondayAfter, reservationKey);

			if (this._isPluginBasedOn(CleaningPluginBaseOnType.ROOM, plugins))
			{
				var basedOns = plugins.SelectMany(p => p.BasedOns.Where(bo => bo.Type == CleaningPluginBaseOnType.ROOM)).ToArray();
				foreach (var basedOn in basedOns)
				{
					basedOn.Rooms = rooms.Take(20).Select(r => new CleaningPluginRoomCredits
					{
						Credits = 99,
						RoomId = r.RoomId
					}).ToArray();
				}
			}
			if (this._isPluginBasedOn(CleaningPluginBaseOnType.FLOOR, plugins))
			{
				var basedOns = plugins.SelectMany(p => p.BasedOns.Where(bo => bo.Type == CleaningPluginBaseOnType.FLOOR)).ToArray();
				var floorIds = rooms.Select(r => r.FloorId).Distinct().Take(2);
				foreach (var basedOn in basedOns)
				{
					basedOn.FoorIds = floorIds;
				}
			}
			if (this._isPluginBasedOn(CleaningPluginBaseOnType.SECTION, plugins))
			{
				var basedOns = plugins.SelectMany(p => p.BasedOns.Where(bo => bo.Type == CleaningPluginBaseOnType.SECTION)).ToArray();
				var sections = rooms.Select(r => r.Section).Distinct().Take(2);
				foreach (var basedOn in basedOns)
				{
					basedOn.Sections = sections;
				}
			}
			if (this._isPluginBasedOn(CleaningPluginBaseOnType.SUB_SECTION, plugins))
			{
				var basedOns = plugins.SelectMany(p => p.BasedOns.Where(bo => bo.Type == CleaningPluginBaseOnType.SUB_SECTION)).ToArray();
				var subSections = rooms.Select(r => r.SubSection).Distinct().Take(2);
				foreach (var basedOn in basedOns)
				{
					basedOn.SubSections = subSections;
				}
			}
			if (this._isPluginBasedOn(CleaningPluginBaseOnType.ROOM_CATEGORY, plugins))
			{
				var basedOns = plugins.SelectMany(p => p.BasedOns.Where(bo => bo.Type == CleaningPluginBaseOnType.ROOM_CATEGORY)).ToArray();
				var categoryId = rooms.First()?.Category?.Id ?? Guid.Empty;
				foreach (var basedOn in basedOns)
				{
					basedOn.RoomCategories = new CleaningPluginBasedOnRoomCategory[] {
						new CleaningPluginBasedOnRoomCategory
						{
							CategoryId = categoryId,
							Credits = 222
						}
					};
				}
			}
			if (this._isPluginBasedOn(CleaningPluginBaseOnType.RESERVATION_SPACE_CATEGORY, plugins))
			{
				var basedOns = plugins.SelectMany(p => p.BasedOns.Where(bo => bo.Type == CleaningPluginBaseOnType.RESERVATION_SPACE_CATEGORY)).ToArray();
				foreach (var basedOn in basedOns)
				{
					basedOn.ReservationSpaceCategories = new string[] { "TEST_RESERVATION_SPACE_CATEGORY" };
				}
			}
			//if (this._isPluginBasedOn(CleaningPluginBaseOnType.PRODUCT_TAG, plugins))
			//{
			//	var basedOns = plugins.SelectMany(p => p.BasedOns.Where(bo => bo.Type == CleaningPluginBaseOnType.PRODUCT_TAG)).ToArray();
			//	foreach (var basedOn in basedOns)
			//	{
			//		basedOn.ProductsTagsExtended = new string[] { "TEST_PRODUCT_TAG" };
			//	}
			//}
			//if (this._isPluginBasedOn(CleaningPluginBaseOnType.OTHER_PROPERTIES, plugins))
			//{
			//	var basedOns = plugins.SelectMany(p => p.BasedOns.Where(bo => bo.Type == CleaningPluginBaseOnType.OTHER_PROPERTIES)).ToArray();
			//	foreach (var basedOn in basedOns)
			//	{
			//		basedOn.OtherProperties = new CleaningPluginKeyValue[] 
			//		{
			//			new CleaningPluginKeyValue { Key = "OP_KEY_1", Value = "TEST_OP_1"  },
			//			//new CleaningPluginKeyValue { Key = "OP_KEY_2", Value = "TEST_OP_2"  },
			//		};
			//	}
			//}


			foreach (var room in rooms)
			{
				var floorIdShort = $"000000{(room.FloorId == Guid.Empty ? "NoFloor" : room.FloorId.ToString())}";
				floorIdShort = floorIdShort.Substring(floorIdShort.Length - 6);

				var testerRoom = new CleaningCalendarTesterRoom
				{
					Name = room.Name,
					CategoryName = room.Category?.Name ?? "No room category",
					Days = new List<CleaningCalendarTesterDay>(),
					BasedOnsDescription = string.Join(", ", plugins.SelectMany(p => p.BasedOns.Select(bo => (bo as CleaningPluginBasedOnTest).Description)).ToArray()),
					ReservationsDescription = room.Description,
					IsOutOfService = room.IsOutOfService,
					IsDoNotDisturb = room.IsDoNotDisturb,
					Section = room.Section,
					SubSection = room.SubSection,
					FloorId = floorIdShort
				};
				testerRooms.Add(testerRoom);

				var allRoomReservations = room.Reservations;

				foreach (var date in dates)
				{
					var cleaningCalendarDay = new CleaningCalendarTesterDay
					{
						Date = date,
						DateString = date.ToString("yyyy-MM-dd HH:mm"),
						DayName = date.DayOfWeek.ToString(),
						Cleanings = new List<CleaningCalendarTesterCleaning>(),
						Reservations = new List<CleaningCalendarReservation>()
					};

					var activeReservations = this._filterActiveReservations(date, allRoomReservations);
					room.Reservations = activeReservations;

					var calculatedCleanings = (this._cleaningProvider.CalculateCleanings(date, new CleaningProviderRequest.Room[] { room }, plugins)).Results?.FirstOrDefault();
					if (calculatedCleanings == null)
					{
						// If calculated cleanings == null it means that there are no cleanings
						calculatedCleanings = new Common.Data.ProcessResponse<CleaningProviderRequest.Cleaning[]>
						{
							Data = new CleaningProviderRequest.Cleaning[0],
							HasError = false,
							IsSuccess = true,
							Message = ""
						};
					}

					if (calculatedCleanings.HasError)
					{
						errors.Add(new PredictionsTesterError { Message = calculatedCleanings.Message, CleaningDate = date, RoomName = room.Name, NumberOfReservations = room.Reservations.Length, NumberOfPlugins = plugins.Count() });
						continue;
					}

					cleaningCalendarDay.Cleanings = calculatedCleanings.Data.Select(c => new CleaningCalendarTesterCleaning 
					{ 
						CleaningName = c.PluginName,
						HasRecommendedInterval = c.ShouldNotEndAfter.HasValue && c.ShouldNotStartBefore.HasValue,
						RecommendedIntervalFromTimeString = c.ShouldNotStartBefore?.ToString("HH:mm") ?? null,
						RecommendedIntervalToTimeString = c.ShouldNotEndAfter?.ToString("HH:mm") ?? null,
						CleaningType = c.Type.ToString(),
						Credits = c.Credits
					}).ToList();

					cleaningCalendarDay.Reservations = activeReservations.Select(r => new CleaningCalendarReservation 
					{ 
						GuestName = r.GuestName,
						IsArrival = r.CheckIn.Date == date.Date,
						IsDeparture = r.CheckOut.Date == date.Date,
						ReservationId = r.Id
					}).ToList();

					testerRoom.Days.Add(cleaningCalendarDay);
					
				}

				room.Reservations = allRoomReservations;
			}


			return new PredictionsTesterResult
			{
				Days = this._calendarDays,
				Rooms = testerRooms,
				ErrorResults = errors
			};
		}

		private bool _isPluginBasedOn(CleaningPluginBaseOnType type, IEnumerable<CleaningProviderPlugin> plugins)
		{
			return plugins.Any(p => p.BasedOns.Any(bo => bo.Type == type));
		}

		private CleaningProviderRequest.Reservation[] _filterActiveReservations(DateTime date, CleaningProviderRequest.Reservation[] reservations)
		{
			return reservations.Where(r => r.CheckIn.Date <= date && r.CheckOut.Date >= date).ToArray();
		}


		public enum BasedOnGroupType
		{
			Daily_AnyTime, // OK
			Daily_AnyTime_StayDeparture, // OK
			Daily_SpecificTimes, // OK
			Weekly_FullWeek_AnyTime, // OK
			Weekly_WorkDays_AnyTime, // OK
			Weekly_WorkDays_SpecificTimes, // OK
			Monthly_Start, // OK
			Monthly_Middle, // OK
			Monthly_End, // OK
			WeekBased_AllWeeks, // OK
			Periodical_OnceEvery2Nights_FromCheckin, // OK
			Periodical_OnceEvery3Nights_FromMonday, // OK
			Periodical_OnceEvery2Nights_FromTuesday, // OK
			Periodical_OnceEvery2Nights_FromWednesday, // OK
			Periodical_OnceEvery2Nights_FromThursday, // OK
			Periodical_OnceEvery2Nights_FromFriday, // OK
			Periodical_OnceEvery2Nights_FromSaturday, // OK
			Periodical_OnceEvery2Nights_FromSunday, // OK
			Periodical_Balance2CleaningsOverPeriod, // OK
			Periodical_Balance3CleaningsOverReservation, // OK
			Periodical_Mixed, // OK
			Periodical_Mixed_SundayPostponedToMonday, // OK
			Daily_AnyTime_OnlyDepartures, // OK
			Daily_AnyTime_OnlyVacants, // OK
			Daily_AnyTime_OnlyStays, // OK
			Daily_AnyTime_OnlyOutOfService, // OK
			Daily_AnyTime_Nights, // OK
			Daily_AnyTime_Nights_Every2FromCheckIn, // OK
			Daily_AnyTime_Nights_Every3FromFirstMonday, // OK
			Daily_AnyTime_Rooms, // OK
			Daily_AnyTime_Floors, // OK
			Daily_AnyTime_Sections, // OK
			Daily_AnyTime_SubSections, // OK
			Daily_AnyTime_ReservationSpaceCategories, // OK
			Daily_AnyTime_ProductsOrTags, // OK
			Daily_AnyTime_RoomCategories, // OK
			Daily_AnyTime_OtherProperties, // OK
			Periodical_Mixed_OnlyStay, // OK
			Weekly_WorkDays_AnyTime_OnlyStay, // OK
			Daily_AnyTime_Nights_OnlyStay, // OK
			Daily_AnyTime_Rooms_OnlyStay, // OK
			Daily_AnyTime_Floors_OnlyStay, // OK
			Daily_AnyTime_Sections_OnlyStay, // OK
			Daily_AnyTime_SubSections_OnlyStay, // OK
			Daily_AnyTime_ReservationSpaceCategories_OnlyStay, // OK
			Daily_AnyTime_ProductsOrTags_OnlyStay, // OK
			Daily_AnyTime_RoomCategories_OnlyStay, // OK
			Periodical_Mixed_OnlyStay_Floors, // OK
			Weekly_WorkDays_AnyTime_OnlyStay_Floors, // OK
			Daily_AnyTime_Nights_OnlyStay_RoomCategories, // OK
			Daily_AnyTime_Rooms_OnlyStay_RoomCategories, // OK
			Daily_AnyTime_Floors_OnlyStay_RoomCategories, // OK
			Daily_AnyTime_Sections_OnlyStay_RoomCategories, // OK
			Daily_AnyTime_SubSections_OnlyStay_RoomCategories, // OK
			Daily_AnyTime_ReservationSpaceCategories_OnlyStay_RoomCategories, // OK
			Daily_AnyTime_ProductsOrTags_OnlyStay_RoomCategories, // OK
			Periodical_Mixed_OnlyStay_Floors_RoomCategories, // OK 
			Weekly_WorkDays_AnyTime_OnlyStay_Floors_RoomCategories, // OK
		}
		private IEnumerable<CleaningProviderPlugin> _GeneratePlugins(IEnumerable<Guid> roomIds, IEnumerable<Guid> floorIds, IEnumerable<string> sections, IEnumerable<string> subSections, IEnumerable<string> reservationSpaceCategories, IEnumerable<string> productsTags, IEnumerable<CleaningPluginBasedOnRoomCategory> categories, string basedOnKey)
		{
			var basedOnGroupType = (BasedOnGroupType)Enum.Parse(typeof(BasedOnGroupType), basedOnKey);
			switch (basedOnGroupType)
			{
				case BasedOnGroupType.Daily_AnyTime: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime() };
				case BasedOnGroupType.Daily_AnyTime_StayDeparture: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_StayDeparture() };
				case BasedOnGroupType.Daily_SpecificTimes: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_SpecificTimes() };
				case BasedOnGroupType.Weekly_FullWeek_AnyTime: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Weekly_FullWeek_AnyTime() };
				case BasedOnGroupType.Weekly_WorkDays_AnyTime: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Weekly_WorkDays_AnyTime() };
				case BasedOnGroupType.Weekly_WorkDays_SpecificTimes: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Weekly_WorkDays_SpecificTimes() };
				case BasedOnGroupType.Monthly_Start: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Monthly_Start() };
				case BasedOnGroupType.Monthly_Middle: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Monthly_Middle() };
				case BasedOnGroupType.Monthly_End: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Monthly_End() };
				case BasedOnGroupType.WeekBased_AllWeeks: return new List<CleaningProviderPlugin>() { this._CreatePlugin_WeekBased_AllWeeks() };
				case BasedOnGroupType.Periodical_OnceEvery2Nights_FromCheckin: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_OnceEvery2Nights_FromCheckin() };
				case BasedOnGroupType.Periodical_OnceEvery3Nights_FromMonday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_OnceEvery3Nights_FromMonday() };
				case BasedOnGroupType.Periodical_OnceEvery2Nights_FromTuesday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_OnceEvery2Nights_FromTuesday() };
				case BasedOnGroupType.Periodical_OnceEvery2Nights_FromWednesday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_OnceEvery2Nights_FromWednesday() };
				case BasedOnGroupType.Periodical_OnceEvery2Nights_FromThursday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_OnceEvery2Nights_FromThursday() };
				case BasedOnGroupType.Periodical_OnceEvery2Nights_FromFriday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_OnceEvery2Nights_FromFriday() };
				case BasedOnGroupType.Periodical_OnceEvery2Nights_FromSaturday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_OnceEvery2Nights_FromSaturday() };
				case BasedOnGroupType.Periodical_OnceEvery2Nights_FromSunday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_OnceEvery2Nights_FromSunday() };
				case BasedOnGroupType.Periodical_Balance2CleaningsOverPeriod: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_Balance2CleaningsOverPeriod() };
				case BasedOnGroupType.Periodical_Balance3CleaningsOverReservation: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_Balance3CleaningsOverReservation() };
				case BasedOnGroupType.Periodical_Mixed: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_Mixed() };
				case BasedOnGroupType.Periodical_Mixed_SundayPostponedToMonday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_Mixed_SundayPostponedToMonday() };
				case BasedOnGroupType.Daily_AnyTime_OnlyDepartures: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_OnlyDepartures() };
				case BasedOnGroupType.Daily_AnyTime_OnlyVacants: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_OnlyVacants() };
				case BasedOnGroupType.Daily_AnyTime_OnlyStays: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_OnlyStays() };
				case BasedOnGroupType.Daily_AnyTime_OnlyOutOfService: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_OnlyOutOfService() };
				case BasedOnGroupType.Daily_AnyTime_Nights: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Nights() };
				case BasedOnGroupType.Daily_AnyTime_OtherProperties: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_OtherProperties(new CleaningPluginKeyValue[] { new CleaningPluginKeyValue { Key = "OP_KEY_1", Value = "TEST_OP_1" } }) };
				
				case BasedOnGroupType.Daily_AnyTime_Nights_Every2FromCheckIn: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Nights_Every2FromCheckIn() };
				case BasedOnGroupType.Daily_AnyTime_Nights_Every3FromFirstMonday: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Nights_Every3FromFirstMonday() };

				case BasedOnGroupType.Daily_AnyTime_Rooms: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Rooms(roomIds) };
				case BasedOnGroupType.Daily_AnyTime_Floors: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Floors(floorIds) };
				case BasedOnGroupType.Daily_AnyTime_Sections: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Sections(sections) };
				case BasedOnGroupType.Daily_AnyTime_SubSections: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_SubSections(subSections) };
				case BasedOnGroupType.Daily_AnyTime_ReservationSpaceCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_ReservationSpaceCategories(reservationSpaceCategories) };
				case BasedOnGroupType.Daily_AnyTime_ProductsOrTags: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_ProductsOrTags(productsTags) };
				case BasedOnGroupType.Daily_AnyTime_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_RoomCategories(categories) };
				case BasedOnGroupType.Periodical_Mixed_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_Mixed_OnlyStay() };
				case BasedOnGroupType.Weekly_WorkDays_AnyTime_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Weekly_WorkDays_AnyTime_OnlyStay() };
				case BasedOnGroupType.Daily_AnyTime_Nights_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Nights_OnlyStay() };
				case BasedOnGroupType.Daily_AnyTime_Rooms_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Rooms_OnlyStay(roomIds) };
				case BasedOnGroupType.Daily_AnyTime_Floors_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Floors_OnlyStay(floorIds) };
				case BasedOnGroupType.Daily_AnyTime_Sections_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Sections_OnlyStay(sections) };
				case BasedOnGroupType.Daily_AnyTime_SubSections_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_SubSections_OnlyStay(subSections) };
				case BasedOnGroupType.Daily_AnyTime_ReservationSpaceCategories_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_ReservationSpaceCategories_OnlyStay(reservationSpaceCategories) };
				case BasedOnGroupType.Daily_AnyTime_ProductsOrTags_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_ProductsOrTags_OnlyStay(productsTags) };
				case BasedOnGroupType.Daily_AnyTime_RoomCategories_OnlyStay: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_RoomCategories_OnlyStay(categories) };
				case BasedOnGroupType.Periodical_Mixed_OnlyStay_Floors: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_Mixed_OnlyStay_Floors(floorIds) };
				case BasedOnGroupType.Weekly_WorkDays_AnyTime_OnlyStay_Floors: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Weekly_WorkDays_AnyTime_OnlyStay_Floors(floorIds) };
				case BasedOnGroupType.Daily_AnyTime_Nights_OnlyStay_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Nights_OnlyStay_RoomCategories(categories) };
				case BasedOnGroupType.Daily_AnyTime_Rooms_OnlyStay_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Rooms_OnlyStay_RoomCategories(roomIds, categories) };
				case BasedOnGroupType.Daily_AnyTime_Floors_OnlyStay_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Floors_OnlyStay_RoomCategories(floorIds, categories) };
				case BasedOnGroupType.Daily_AnyTime_Sections_OnlyStay_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_Sections_OnlyStay_RoomCategories(sections, categories) };
				case BasedOnGroupType.Daily_AnyTime_SubSections_OnlyStay_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_SubSections_OnlyStay_RoomCategories(subSections, categories) };
				case BasedOnGroupType.Daily_AnyTime_ReservationSpaceCategories_OnlyStay_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_ReservationSpaceCategories_OnlyStay_RoomCategories(reservationSpaceCategories, categories) };
				case BasedOnGroupType.Daily_AnyTime_ProductsOrTags_OnlyStay_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Daily_AnyTime_ProductsOrTags_OnlyStay_RoomCategories(productsTags, categories) };
				case BasedOnGroupType.Periodical_Mixed_OnlyStay_Floors_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Periodical_Mixed_OnlyStay_Floors_RoomCategories(floorIds, categories) };
				case BasedOnGroupType.Weekly_WorkDays_AnyTime_OnlyStay_Floors_RoomCategories: return new List<CleaningProviderPlugin>() { this._CreatePlugin_Weekly_WorkDays_AnyTime_OnlyStay_Floors_RoomCategories(floorIds, categories) };
			}

			return new List<CleaningProviderPlugin>();

		}
		
		/// <summary>
		/// Clean every day at any time
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOnTest[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time but only stays and departures
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_StayDeparture()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOnTest[]
			{
				this._Create_BasedOn_Occupancy_StayDeparture(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at specific times
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_SpecificTimes()
		{


			var plugin1 = this._Create_Daily_SpecificTimes_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day of the week at any time
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Weekly_FullWeek_AnyTime()
		{


			var plugin1 = this._Create_Weekly_FullWeek_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean only workdays of every week at any time, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Weekly_WorkDays_AnyTime()
		{


			var plugin1 = this._Create_Weekly_WorkDays_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean only workdays of every week at any time, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Weekly_WorkDays_SpecificTimes()
		{


			var plugin1 = this._Create_Weekly_WorkDays_SpecificTimes_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every start of the month, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Monthly_Start()
		{


			var plugin1 = this._Create_Monthly_Start_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every middle of the month, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Monthly_Middle()
		{


			var plugin1 = this._Create_Monthly_Middle_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every end of the month, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Monthly_End()
		{


			var plugin1 = this._Create_Monthly_End_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every week of the year, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_WeekBased_AllWeeks()
		{


			var plugin1 = this._Create_WeekBased_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically every 7 nights from the checkin
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_OnceEvery2Nights_FromCheckin()
		{


			var plugin1 = this._Create_Periodical_OnceEvery2Nights_FromCheckin_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically every 7 nights from first monday
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_OnceEvery3Nights_FromMonday()
		{


			var plugin1 = this._Create_Periodical_OnceEvery3Nights_FromMonday_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically every 7 nights from first tuesday
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_OnceEvery2Nights_FromTuesday()
		{


			var plugin1 = this._Create_Periodical_OnceEvery2Nights_FromTuesday_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically every 7 nights from first Wednesday
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_OnceEvery2Nights_FromWednesday()
		{


			var plugin1 = this._Create_Periodical_OnceEvery2Nights_FromWednesday_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically every 7 nights from first Thursday
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_OnceEvery2Nights_FromThursday()
		{


			var plugin1 = this._Create_Periodical_OnceEvery2Nights_FromThursday_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically every 7 nights from first Friday
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_OnceEvery2Nights_FromFriday()
		{


			var plugin1 = this._Create_Periodical_OnceEvery2Nights_FromFriday_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically every 7 nights from first Saturday
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_OnceEvery2Nights_FromSaturday()
		{


			var plugin1 = this._Create_Periodical_OnceEvery2Nights_FromSaturday_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically every 7 nights from first Sunday
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_OnceEvery2Nights_FromSunday()
		{


			var plugin1 = this._Create_Periodical_OnceEvery2Nights_FromSunday_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically balance 2 cleanings over period, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_Balance2CleaningsOverPeriod()
		{


			var plugin1 = this._Create_Periodical_Balance2CleaningsOverPeriod_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically balance 3 cleanings over reservation duration, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_Balance3CleaningsOverReservation()
		{


			var plugin1 = this._Create_Periodical_Balance3CleaningsOverReservation_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically mixed balanced, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_Mixed()
		{


			var plugin1 = this._Create_Periodical_Mixed_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically mixed balanced, postpone sundays to mondays, everything
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_Mixed_SundayPostponedToMonday()
		{


			var plugin1 = this._Create_Periodical_Mixed_SundayPostponedToMonday_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_All()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only departures
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_OnlyDepartures()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Departure()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only vacant
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_OnlyVacants()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Vacant()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_OnlyStays()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Stay()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only out of service
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_OnlyOutOfService()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_OutOfService()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only on nights 2, 4, 6, 8, 10 of the stay
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Nights()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Nights()
			};
			return plugin1;
		}
		/// <summary>
		/// Clean every 2 nights from first check in at any time
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Nights_Every2FromCheckIn()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Nights_Every2FromCheckIn()
			};
			return plugin1;
		}
		/// <summary>
		/// Clean every 3 nights from first monday at any time
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Nights_Every3FromFirstMonday()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Nights_Every3FromFirstMonday()
			};
			return plugin1;
		}

		/// <summary>
		/// Clean every day at any time, only in specific rooms with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Rooms(IEnumerable<Guid> roomIds)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Rooms(roomIds)
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only specific floors
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Floors(IEnumerable<Guid> floorIds)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Floors(floorIds)
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only specific sections
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Sections(IEnumerable<string> sections)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Sections(sections)
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only specific sub sections
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_SubSections(IEnumerable<string> subSections)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_SubSections(subSections)
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only if specific ReservationSpaceCategory is present
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_ReservationSpaceCategories(IEnumerable<string> reservationSpaceCategories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_ReservationSpaceCategory(reservationSpaceCategories)
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only if specific Product/Tag is preset
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_ProductsOrTags(IEnumerable<string> productsTags)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_ProductOrTags(productsTags)
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only if specific OtherProperty is preset
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_OtherProperties(IEnumerable<CleaningPluginKeyValue> otherProperties)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_OtherProperties(otherProperties)
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_RoomCategories(IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_RoomCategory(categories)
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically mixed balanced, only stays
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_Mixed_OnlyStay()
		{


			var plugin1 = this._Create_Periodical_Mixed_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Stay()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean only workdays of every week at any time, only stays
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Weekly_WorkDays_AnyTime_OnlyStay()
		{


			var plugin1 = this._Create_Weekly_WorkDays_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Stay()
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only on nights 2, 4, 6, 8, 10 of the stay
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Nights_OnlyStay()
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Nights(),
				this._Create_BasedOn_Occupancy_Stay(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only in specific rooms with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Rooms_OnlyStay(IEnumerable<Guid> roomIds)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Rooms(roomIds),
				this._Create_BasedOn_Occupancy_Stay(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only specific floors
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Floors_OnlyStay(IEnumerable<Guid> floorIds)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Floors(floorIds),
				this._Create_BasedOn_Occupancy_Stay(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only specific sections
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Sections_OnlyStay(IEnumerable<string> sections)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Sections(sections),
				this._Create_BasedOn_Occupancy_Stay(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only specific sub sections
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_SubSections_OnlyStay(IEnumerable<string> subSections)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_SubSections(subSections),
				this._Create_BasedOn_Occupancy_Stay(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only if specific ReservationSpaceCategory is present
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_ReservationSpaceCategories_OnlyStay(IEnumerable<string> reservationSpaceCategories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_ReservationSpaceCategory(reservationSpaceCategories),
				this._Create_BasedOn_Occupancy_Stay(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only if specific Product/Tag is preset
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_ProductsOrTags_OnlyStay(IEnumerable<string> productsTags)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_ProductOrTags(productsTags),
				this._Create_BasedOn_Occupancy_Stay(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_RoomCategories_OnlyStay(IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_RoomCategory(categories),
				this._Create_BasedOn_Occupancy_Stay(),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically mixed balanced, only stays, specific floors
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_Mixed_OnlyStay_Floors(IEnumerable<Guid> floorIds)
		{


			var plugin1 = this._Create_Periodical_Mixed_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_Floors(floorIds),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean only workdays of every week at any time, only stays, specific floors
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Weekly_WorkDays_AnyTime_OnlyStay_Floors(IEnumerable<Guid> floorIds)
		{


			var plugin1 = this._Create_Weekly_WorkDays_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_Floors(floorIds),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only on nights 2, 4, 6, 8, 10 of the stay, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Nights_OnlyStay_RoomCategories(IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Nights(),
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only in specific rooms with specific credit cost, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Rooms_OnlyStay_RoomCategories(IEnumerable<Guid> roomIds, IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Rooms(roomIds),
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only specific floors, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Floors_OnlyStay_RoomCategories(IEnumerable<Guid> floorIds, IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Floors(floorIds),
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only specific sections, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_Sections_OnlyStay_RoomCategories(IEnumerable<string> sections, IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Sections(sections),
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only specific sub sections, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_SubSections_OnlyStay_RoomCategories(IEnumerable<string> subSections, IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_SubSections(subSections),
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only if specific ReservationSpaceCategory is present, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_ReservationSpaceCategories_OnlyStay_RoomCategories(IEnumerable<string> reservationSpaceCategories, IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_ReservationSpaceCategory(reservationSpaceCategories),
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean every day at any time, only stays, only if specific Product/Tag is preset, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Daily_AnyTime_ProductsOrTags_OnlyStay_RoomCategories(IEnumerable<string> productsTags, IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Daily_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_ProductOrTags(productsTags),
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean periodically mixed balanced, only stays, specific floors, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Periodical_Mixed_OnlyStay_Floors_RoomCategories(IEnumerable<Guid> floorIds, IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Periodical_Mixed_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_Floors(floorIds),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}

		/// <summary>
		/// Clean only workdays of every week at any time, only stays, specific floors, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>
		private CleaningProviderPlugin _CreatePlugin_Weekly_WorkDays_AnyTime_OnlyStay_Floors_RoomCategories(IEnumerable<Guid> floorIds, IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{


			var plugin1 = this._Create_Weekly_WorkDays_AnyTime_Plugin();
			plugin1.BasedOns = new CleaningPluginBasedOn[]
			{
				this._Create_BasedOn_Occupancy_Stay(),
				this._Create_BasedOn_Floors(floorIds),
				this._Create_BasedOn_RoomCategory(categories),
			};
			return plugin1;


		}















		/// <summary>
		/// --Clean every day at any time, everything
		/// --Clean every day at specific times, everything
		/// --Clean every day of the week at any time, everything
		/// --Clean only workdays of every week at any time, everything
		/// --Clean only workdays of every week at specific times, everything
		/// --Clean every start of the month, everything
		/// --Clean every middle of the month, everything
		/// --Clean every end of the month, everything
		/// --Clean every week of the year, everything
		/// --Clean periodically every 7 nights from the checkin
		/// --Clean periodically every 7 nights from first monday
		/// --Clean periodically every 7 nights from first tuesday
		/// --Clean periodically every 7 nights from first wednesday
		/// --Clean periodically every 7 nights from first thursday
		/// --Clean periodically every 7 nights from first friday
		/// --Clean periodically every 7 nights from first saturday
		/// --Clean periodically every 7 nights from first sunday
		/// --Clean periodically balance 2 cleanings over period, everything
		/// --Clean periodically balance 3 cleanings over reservation duration, everything
		/// --Clean periodically mixed balanced, everything
		/// --Clean periodically mixed balanced, postpone sundays to mondays, everything
		/// 
		/// --Clean every day at any time, only departures
		/// --Clean every day at any time, only vacant // TODO: EXPAND TO INCLUDE PERIOD
		/// --Clean every day at any time, only stays
		/// --Clean every day at any time, only out of service
		/// --Clean every day at any time, only on nights 2, 4, 6, 8, 10 of the stay
		/// --Clean every day at any time, only in specific rooms with specific credit cost
		/// --Clean every day at any time, only specific floors
		/// --Clean every day at any time, only specific sections 
		/// --Clean every day at any time, only specific sub sections
		/// --Clean every day at any time, only if specific ReservationSpaceCategory is present
		/// --Clean every day at any time, only if specific Product/Tag is preset
		/// Clean every day at any time, only if specific OtherProperty is preset
		/// --Clean every day at any time, only specific room categories with specific credit cost
		/// --Clean periodically mixed balanced, only stays
		/// --Clean only workdays of every week at any time, only stays
		/// 
		/// 
		/// --Clean every day at any time, only stays, only on nights 2, 4, 6, 8, 10 of the stay
		/// --Clean every day at any time, only stays, only in specific rooms with specific credit cost
		/// --Clean every day at any time, only stays, only specific floors
		/// --Clean every day at any time, only stays, only specific sections 
		/// --Clean every day at any time, only stays, only specific sub sections
		/// --Clean every day at any time, only stays, only if specific ReservationSpaceCategory is present
		/// --Clean every day at any time, only stays, only if specific Product/Tag is preset
		/// Clean every day at any time, only stays, only if specific OtherProperty is preset
		/// --Clean every day at any time, only stays, only specific room categories with specific credit cost
		/// --Clean periodically mixed balanced, only stays, only specific floors
		/// --Clean only workdays of every week at any time, only stays, only specific floors
		/// 
		/// 
		/// --Clean every day at any time, only stays, only on nights 2, 4, 6, 8, 10 of the stay, only specific room categories with specific credit cost
		/// --Clean every day at any time, only stays, only in specific rooms with specific credit cost, only specific room categories with specific credit cost
		/// --Clean every day at any time, only stays, only specific floors, only specific room categories with specific credit cost
		/// --Clean every day at any time, only stays, only specific sections , only specific room categories with specific credit cost
		/// --Clean every day at any time, only stays, only specific sub sections, only specific room categories with specific credit cost
		/// --Clean every day at any time, only stays, only if specific ReservationSpaceCategory is present, only specific room categories with specific credit cost
		/// --Clean every day at any time, only stays, only if specific Product/Tag is preset, only specific room categories with specific credit cost
		/// Clean every day at any time, only stays, only if specific OtherProperty is preset, only specific room categories with specific credit cost
		/// --Clean periodically mixed balanced, only stays, only specific floors, only specific room categories with specific credit cost
		/// --Clean only workdays of every week at any time, only stays, only specific floors, only specific room categories with specific credit cost
		/// </summary>
		/// <returns></returns>












		private CleaningProviderPlugin _Create_Daily_AnyTime_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "DAILY";
			plugin.DailyCleaningTimeTypeKey = "ANY_TIME";
			plugin.Name = "Daily any time";
			return plugin;
		}

		private CleaningProviderPlugin _Create_Daily_SpecificTimes_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "DAILY";
			plugin.DailyCleaningTimeTypeKey = "SPECIFIC_TIMES";
			plugin.DailyCleaningTypeTimes = new string[] { "10:00", "12:00", "14:00" };
			plugin.Name = "Daily specific times";
			return plugin;
		}

		private CleaningProviderPlugin _Create_Weekly_FullWeek_AnyTime_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "WEEKLY";
			plugin.WeeklyCleanOnMonday = true;
			plugin.WeeklyCleanOnTuesday = true;
			plugin.WeeklyCleanOnWednesday = true;
			plugin.WeeklyCleanOnThursday = true;
			plugin.WeeklyCleanOnFriday = true;
			plugin.WeeklyCleanOnSaturday = true;
			plugin.WeeklyCleanOnSunday = true;
			plugin.WeeklyTimeMondayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeTuesdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeWednesdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeThursdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeFridayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeSaturdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeSundayTypeKey = "ANY_TIME";
			plugin.Name = "Weekly, full week, any time";
			return plugin;
		}
		private CleaningProviderPlugin _Create_Weekly_WorkDays_AnyTime_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "WEEKLY";
			plugin.WeeklyCleanOnMonday = true;
			plugin.WeeklyCleanOnTuesday = true;
			plugin.WeeklyCleanOnWednesday = true;
			plugin.WeeklyCleanOnThursday = true;
			plugin.WeeklyCleanOnFriday = true;
			plugin.WeeklyCleanOnSaturday = false;
			plugin.WeeklyCleanOnSunday = false;
			plugin.WeeklyTimeMondayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeTuesdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeWednesdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeThursdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeFridayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeSaturdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeSundayTypeKey = "ANY_TIME";
			plugin.Name = "Weekly, work days, any time";
			return plugin;
		}
		private CleaningProviderPlugin _Create_Weekly_WorkDays_SpecificTimes_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "WEEKLY";
			plugin.WeeklyCleanOnMonday = true;
			plugin.WeeklyCleanOnTuesday = true;
			plugin.WeeklyCleanOnWednesday = true;
			plugin.WeeklyCleanOnThursday = true;
			plugin.WeeklyCleanOnFriday = true;
			plugin.WeeklyCleanOnSaturday = false;
			plugin.WeeklyCleanOnSunday = false;
			plugin.WeeklyTimeMondayTypeKey = "SPECIFIC_TIMES";
			plugin.WeeklyTimeTuesdayTypeKey = "SPECIFIC_TIMES";
			plugin.WeeklyTimeWednesdayTypeKey = "SPECIFIC_TIMES";
			plugin.WeeklyTimeThursdayTypeKey = "SPECIFIC_TIMES";
			plugin.WeeklyTimeFridayTypeKey = "SPECIFIC_TIMES";
			plugin.WeeklyTimeSaturdayTypeKey = "ANY_TIME";
			plugin.WeeklyTimeSundayTypeKey = "ANY_TIME";
			plugin.WeeklyCleaningTypeMondayTimes = new string[] { "10:00", "12:00", "14:00" };
			plugin.WeeklyCleaningTypeTuesdayTimes = new string[] { "10:00", "12:00", "14:00" };
			plugin.WeeklyCleaningTypeWednesdayTimes = new string[] { "10:00", "12:00", "14:00" };
			plugin.WeeklyCleaningTypeThursdayTimes = new string[] { "10:00", "12:00", "14:00" };
			plugin.WeeklyCleaningTypeFridayTimes = new string[] { "10:00", "12:00", "14:00" };
			plugin.WeeklyCleaningTypeSaturdayTimes = new string[] { "10:00", "12:00", "14:00" };
			plugin.WeeklyCleaningTypeSundayTimes = new string[] { "10:00", "12:00", "14:00" };
			plugin.Name = "Weekly, work days, specific times";
			return plugin;
		}
		private CleaningProviderPlugin _Create_Monthly_Start_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "MONTHLY";
			plugin.MonthlyCleaningTypeTimeOfMonthKey = "BEGINNING_OF_MONTH";
			plugin.Name = "Monthly start";
			return plugin;
		}
		private CleaningProviderPlugin _Create_Monthly_Middle_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "MONTHLY";
			plugin.MonthlyCleaningTypeTimeOfMonthKey = "MIDDLE_OF_MONTH";
			plugin.Name = "Monthly middle";
			return plugin;
		}
		private CleaningProviderPlugin _Create_Monthly_End_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "MONTHLY";
			plugin.MonthlyCleaningTypeTimeOfMonthKey = "END_OF_MONTH";
			plugin.Name = "Monthly end";
			return plugin;
		}
		private CleaningProviderPlugin _Create_NoCleaning_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "NO_CLEANING";
			return plugin;
		}
		private CleaningProviderPlugin _Create_WeekBased_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "WEEK_BASED";
			plugin.WeekBasedCleaningTypeWeeks = Enumerable.Range(0, 53);
			return plugin;
		}
		private CleaningProviderPlugin _Create_Periodical_OnceEvery2Nights_FromCheckin_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 2,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "CHECK_IN",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}
		private CleaningProviderPlugin _Create_Periodical_OnceEvery3Nights_FromMonday_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 3,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "FIRST_MONDAY",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}

		private CleaningProviderPlugin _Create_Periodical_OnceEvery2Nights_FromTuesday_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 2,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "FIRST_TUESDAY",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}

		private CleaningProviderPlugin _Create_Periodical_OnceEvery2Nights_FromWednesday_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 2,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "FIRST_WEDNESDAY",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}

		private CleaningProviderPlugin _Create_Periodical_OnceEvery2Nights_FromThursday_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 2,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "FIRST_THURSDAY",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}


		private CleaningProviderPlugin _Create_Periodical_OnceEvery2Nights_FromFriday_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 2,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "FIRST_FRIDAY",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}


		private CleaningProviderPlugin _Create_Periodical_OnceEvery2Nights_FromSaturday_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 2,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "FIRST_SATURDAY",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}

		private CleaningProviderPlugin _Create_Periodical_OnceEvery2Nights_FromSunday_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 2,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "FIRST_SUNDAY",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}



		private CleaningProviderPlugin _Create_Periodical_Balance2CleaningsOverPeriod_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 2,
					EveryNumberOfDays = 4,
					PeriodTypeKey = "BALANCE_OVER_PERIOD",
					FromDayKey = "CHECK_IN",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}

		private CleaningProviderPlugin _Create_Periodical_Balance3CleaningsOverReservation_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 1,
					NumberOfCleanings = 3,
					EveryNumberOfDays = 1,
					PeriodTypeKey = "BALANCE_OVER_RESERVATION",
					FromDayKey = "CHECK_IN",
					ToNights = 1
				}
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}

		private CleaningProviderPlugin _Create_Periodical_Mixed_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "FROM",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 1,
					PeriodTypeKey = "BALANCE_OVER_PERIOD",
					FromDayKey = "CHECK_IN",
					ToNights = 3
				},
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "FROM",
					FromNights = 4,
					NumberOfCleanings = 2,
					EveryNumberOfDays = 1,
					PeriodTypeKey = "BALANCE_OVER_PERIOD",
					FromDayKey = "CHECK_IN",
					ToNights = 8
				},
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 9,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 5,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "CHECK_IN",
					ToNights = 1
				},
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = false;
			return plugin;
		}
		private CleaningProviderPlugin _Create_Periodical_Mixed_SundayPostponedToMonday_Plugin()
		{
			var plugin = this._CreateDefaultPlugin();
			plugin.TypeKey = "PERIODICAL";
			plugin.PeriodicalIntervals = new CleaningPluginPeriodicalInterval[]
			{
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "FROM",
					FromNights = 1,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 1,
					PeriodTypeKey = "BALANCE_OVER_PERIOD",
					FromDayKey = "CHECK_IN",
					ToNights = 7
				},
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "FROM",
					FromNights = 8,
					NumberOfCleanings = 2,
					EveryNumberOfDays = 1,
					PeriodTypeKey = "BALANCE_OVER_PERIOD",
					FromDayKey = "CHECK_IN",
					ToNights = 14
				},
				new CleaningPluginPeriodicalInterval {
					IntervalTypeKey = "MORE_THAN",
					FromNights = 15,
					NumberOfCleanings = 1,
					EveryNumberOfDays = 5,
					PeriodTypeKey = "ONCE_EVERY_N_DAYS",
					FromDayKey = "CHECK_IN",
					ToNights = 1
				},
			};
			plugin.PeriodicalPostponeSundayCleaningsToMonday = true;
			return plugin;
		}


		private CleaningPluginBasedOnTest _Create_BasedOn_All()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.ALL,
				Description = "All"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Nights()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.NIGHTS,
				Nights = new int[] { 2, 4, 6, 8, 10 },
				NightsTypeKey = "SPECIFIC_NIGHTS",
				Description = "Nights 2,4,6,8,10"
			};
		}


		private CleaningPluginBasedOnTest _Create_BasedOn_Nights_Every2FromCheckIn()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.NIGHTS,
				NightsEveryNumberOfDays = 2,
				NightsTypeKey = "PERIODICAL",
				NightsFromKey = "CHECK_IN",
				Description = "Every 2 nights from check in."
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Nights_Every3FromFirstMonday()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.NIGHTS,
				NightsEveryNumberOfDays = 3,
				NightsTypeKey = "PERIODICAL",
				NightsFromKey = "FIRST_MONDAY",
				Description = "Every 3 nights from monday."
			};
		}

		private CleaningPluginBasedOnTest _Create_BasedOn_Rooms(IEnumerable<Guid> roomIds)
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.ROOM,
				Rooms = new CleaningPluginRoomCredits[0],
				Description = "Rooms"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Occupancy_Departure()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.OCCUPATION,
				CleanDeparture = true,
				CleanOutOfService = false,
				CleanStay = false,
				CleanVacant = false,
				Description = "Occupation clean"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Occupancy_OutOfService()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.OCCUPATION,
				CleanDeparture = false,
				CleanOutOfService = true,
				CleanStay = false,
				CleanVacant = false,
				Description = "Occupation OOS"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Occupancy_Stay()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.OCCUPATION,
				CleanDeparture = false,
				CleanOutOfService = false,
				CleanStay = true,
				CleanVacant = false,
				Description = "Occupation stay"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Occupancy_StayDeparture()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.OCCUPATION,
				CleanDeparture = true,
				CleanOutOfService = false,
				CleanStay = true,
				CleanVacant = false,
				Description = "Occupation stay & departure"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Occupancy_Vacant()
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.OCCUPATION,
				CleanDeparture = false,
				CleanOutOfService = false,
				CleanStay = false,
				CleanVacant = true,
				CleanVacantEveryNumberOfDays = 2,
				Description = "Occupation vacant"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Floors(IEnumerable<Guid> floorIds)
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.FLOOR,
				FoorIds = floorIds,
				Description = "Floors"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_Sections(IEnumerable<string> sections)
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.SECTION,
				Sections = sections,
				Description = "Sections"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_SubSections(IEnumerable<string> subSections)
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.SUB_SECTION,
				SubSections = subSections,
				Description = "Sub sections"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_RoomCategory(IEnumerable<CleaningPluginBasedOnRoomCategory> categories)
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.ROOM_CATEGORY,
				RoomCategories = categories,
				Description = "Room categories"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_ReservationSpaceCategory(IEnumerable<string> reservationSpaceCategories)
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.RESERVATION_SPACE_CATEGORY,
				ReservationSpaceCategories = reservationSpaceCategories,
				Description = "Reservation space categories"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_ProductOrTags(IEnumerable<string> productsOrTags)
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.PRODUCT_TAG,
				//ProductsTags = productsOrTags,
				Description = "Product tag"
			};
		}
		private CleaningPluginBasedOnTest _Create_BasedOn_OtherProperties(IEnumerable<CleaningPluginKeyValue> otherProperties)
		{
			return new CleaningPluginBasedOnTest
			{
				Type = CleaningPluginBaseOnType.OTHER_PROPERTIES,
				//OtherProperties = otherProperties,
				Description = "Other properties"
			};
		}







		private CleaningProviderPlugin _CreateDefaultPlugin()
		{
			return new CleaningProviderPlugin
			{
				TypeKey = "DAILY", // DAILY, WEEKLY, MONTHLY, PERIODICAL, NO_CLEANING, WEEK_BASED
				ChangeSheets = false,
				CleanOnHolidays = false,
				CleanOnSaturday = false,
				CleanOnSunday = false,
				Color = "#000000",
				DailyCleaningTimeTypeKey = "ANY_TIME", // PECIFIC_TIMES, ANY_TIME
				BasedOns = new CleaningPluginBasedOn[0],
				DailyCleaningTypeTimes = new string[0],
				Description = "",
				DisplayStyleKey = "",
				HotelId = "",
				Id = Guid.Empty,
				Instructions = "",
				IsActive = true,
				IsTopRule = false,
				MonthlyCleaningTypeTimeOfMonthKey = null,
				Name = "No Name",
				OrdinalNumber = 0,
				PeriodicalIntervals = new CleaningPluginPeriodicalInterval[0],
				PeriodicalPostponeSundayCleaningsToMonday = false,
				PostponeUntilVacant = false,
				StartsCleaningAfter = null,
				WeekBasedCleaningTypeWeeks = new int[0],
				WeeklyCleaningTypeFridayTimes = new string[0],
				WeeklyCleaningTypeMondayTimes = new string[0],
				WeeklyCleaningTypeSaturdayTimes = new string[0],
				WeeklyCleaningTypeSundayTimes = new string[0],
				WeeklyCleaningTypeThursdayTimes = new string[0],
				WeeklyCleaningTypeTuesdayTimes = new string[0],
				WeeklyCleaningTypeWednesdayTimes = new string[0],
				WeeklyCleanOnFriday = true,
				WeeklyCleanOnMonday = true,
				WeeklyCleanOnSaturday = true,
				WeeklyCleanOnSunday = true,
				WeeklyCleanOnThursday = true,
				WeeklyCleanOnTuesday = true,
				WeeklyCleanOnWednesday = true,
				WeeklyTimeFridayTypeKey = "ANY_TIME",
				WeeklyTimeMondayTypeKey = "ANY_TIME",
				WeeklyTimeSaturdayTypeKey = "ANY_TIME",
				WeeklyTimeSundayTypeKey = "ANY_TIME",
				WeeklyTimeThursdayTypeKey = "ANY_TIME",
				WeeklyTimeTuesdayTypeKey = "ANY_TIME",
				WeeklyTimeWednesdayTypeKey = "ANY_TIME",
			};
		}

		private IEnumerable<CleaningProviderRequest.RoomTest> _GenerateFullSetOfRoomsWithWeeklyReservations(IEnumerable<DateTime> dates, DateTime sundayBefore, DateTime mondayAfter, string reservationKey)
		{
			var allRooms = new List<CleaningProviderRequest.RoomTest>(); //this._CreateVacantRoomVariations();

			foreach (var date in dates)
			{
				allRooms.AddRange(this._CreateRooms(date, sundayBefore, mondayAfter, reservationKey));
			}

			return allRooms;
		}

		private List<CleaningProviderRequest.RoomTest> _CreateVacantRoomVariations()
		{

			var building1 = new Domain.Entities.Building { Id = Guid.NewGuid(), Name = "Building 1" };

			var b1floor1 = new Domain.Entities.Floor { Id = Guid.NewGuid(), Name = "Floor 1B1", BuildingId = building1.Id };
			var sec11 = "S1.1";
			var s11sub1 = "S1.1SUB1";
			var s11sub2 = "S1.1SUB2";
			var sec12 = "S1.2";
			var s12sub1 = "S1.2SUB1";

			var b1floor2 = new Domain.Entities.Floor { Id = Guid.NewGuid(), Name = "Floor 2B1", BuildingId = building1.Id };
			var sec2 = "S2";
			var s2sub1 = "S2SUB1";

			var building2 = new Domain.Entities.Building { Id = Guid.NewGuid(), Name = "Building 2" };

			var b2floor1 = new Domain.Entities.Floor { Id = Guid.NewGuid(), Name = "Floor 1B2", BuildingId = building2.Id };
			var sec3 = "S3";
			var s3sub1 = "S3SUB1";

			var rooms = new List<CleaningProviderRequest.RoomTest>
			{
				// DIFFERENT CATEGORIES
				this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub1, false, studentCategory, "-"),
				//this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub1, false, regularCategory, "-"),
				//this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub1, false, deluxeCategory, "-"),

				////// OUT OF SERVICE
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub1, true, studentCategory),
				//this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub1, true, regularCategory, "-"),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub1, true, deluxeCategory),

				////DIFFERENT SUB SECTION
				//this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub2, false, studentCategory, "-"),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub2, false, regularCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub2, false, deluxeCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub2, true, studentCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub2, true, regularCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec11, s11sub2, true, deluxeCategory),

				
				////DIFFERENT SECTION
				//this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec12, s12sub1, false, studentCategory, "-"),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec12, s12sub1, false, regularCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec12, s12sub1, false, deluxeCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec12, s12sub1, true, studentCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec12, s12sub1, true, regularCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor1.Id, sec12, s12sub1, true, deluxeCategory),
				
				//// DIFFERENT FLOOR
				////this._CreateSingleVacantRoom(roomNumber++, b1floor2.Id, sec2, s2sub1, false, studentCategory),
				//this._CreateSingleVacantRoom(roomNumber++, b1floor2.Id, sec2, s2sub1, false, regularCategory, "-"),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor2.Id, sec2, s2sub1, false, deluxeCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor2.Id, sec2, s2sub1, true, studentCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor2.Id, sec2, s2sub1, true, regularCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b1floor2.Id, sec2, s2sub1, true, deluxeCategory),

				//// DIFFERENT BUILDING
				//this._CreateSingleVacantRoom(roomNumber++, b2floor1.Id, sec3, s3sub1, false, studentCategory, "-"),
				////this._CreateSingleVacantRoom(roomNumber++, b2floor1.Id, sec3, s3sub1, false, regularCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b2floor1.Id, sec3, s3sub1, false, deluxeCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b2floor1.Id, sec3, s3sub1, true, studentCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b2floor1.Id, sec3, s3sub1, true, regularCategory),
				////this._CreateSingleVacantRoom(roomNumber++, b2floor1.Id, sec3, s3sub1, true, deluxeCategory),
			};

			return rooms;
		}

		private CleaningProviderRequest.RoomTest _CreateSingleVacantRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		{
			var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
			return room;
		}

		//private CleaningProviderRequest.RoomTest _CreateSingleStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}

		//private CleaningProviderRequest.RoomTest _CreateSingleCheckInRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}

		//private CleaningProviderRequest.RoomTest _CreateSingleCheckOutRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}

		//private CleaningProviderRequest.RoomTest _CreateSingleDayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}

		//private CleaningProviderRequest.RoomTest _CreateDoubleStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleStayCheckInRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleStayCheckOutRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleStayDayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckInRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckInCheckOutRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckInCheckOutOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckOutRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckInStayDayStayContainedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckInStayDayStayOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckInStayDayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleDayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleDayStayOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleDayStayContainedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckInDayStayContainedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleCheckInDayStayOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateDoubleDayStayCheckInRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}




		//private CleaningProviderRequest.RoomTest _CreateTripleStayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayStayCheckInRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayStayCheckOutRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayStayDayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckInRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckInCheckOutRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckInCheckOutOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckOutRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckInStayDayStayContainedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckInStayDayStayOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckInStayDayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayDayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayDayStayOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayDayStayContainedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckInDayStayContainedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayCheckInDayStayOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleStayDayStayCheckInRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}


		//private CleaningProviderRequest.RoomTest _CreateTripleDayStayRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleDayStayOverlappedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleDayStayContainedRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}
		//private CleaningProviderRequest.RoomTest _CreateTripleDayStayCascadeRoom(int roomNumber, Guid floorId, string section, string subSection, bool isOutOfService, CleaningProviderRequest.RoomCategory category, string description)
		//{
		//	var room = this._GenerateRoom(roomNumber, category, floorId, section: section, subSection: subSection, isOutOfService: isOutOfService, description: description);
		//	return room;
		//}



		//private Domain.Entities.Room _CreateEntityRoom(int roomNumber, Domain.Entities.RoomCategory category, bool isOutOfService)
		//{
		//	var roomName = $"Room {roomNumber}";
		//	return new Domain.Entities.Room
		//	{
		//		CategoryId = category.Id,
		//		Category = category,
		//		FloorSectionName = "N/A",
		//		AreaId = null,
		//		Area = null,
		//		BuildingId = null,
		//		Building = null,
		//		FloorId = null,
		//		Floor = null,
		//		CreatedAt = DateTime.MinValue,
		//		CreatedBy = null,
		//		CreatedById = Guid.Empty,
		//		ExternalId = roomName,
		//		FloorSubSectionName = "N/A",
		//		HotelId = "",
		//		Hotel = null,
		//		Id = Guid.NewGuid(),
		//		IsAutogeneratedFromReservationSync = false,
		//		IsClean = false,
		//		IsDoNotDisturb = false,
		//		IsOccupied = false,
		//		IsOutOfOrder = false,
		//		ModifiedAt = DateTime.MinValue,
		//		ModifiedBy = null,
		//		ModifiedById = Guid.Empty,
		//		Name = roomName,
		//		OrdinalNumber = roomNumber,
		//		Reservations = null,
		//		RoomAssetModels = null,
		//		RoomAssets = null,
		//		TypeKey = "HOTEL"
		//	};
		//}

		//private List<CleaningCalendarRoom> _GenerateRooms(int numberOfRooms)
		//{
		//	var rooms = new List<CleaningCalendarRoom>();

		//	for (int i = 0; i < numberOfRooms; i++)
		//	{
		//		rooms.Add(this._GenerateRoom(i + 1, "Regular category"));
		//	}

		//	return rooms;
		//}

		private CleaningProviderRequest.RoomCategory _GenerateCategory(string name/*, int credits*/)
		{
			return
				new CleaningProviderRequest.RoomCategory
				{
					//Credits = credits,
					Id = Guid.NewGuid(),
					Name = name
				};
		}

		//private IEnumerable<CleaningProviderRequest.RoomCategory> _GenerateCategories()
		//{
		//	return new CleaningProviderRequest.RoomCategory[] {
		//		new CleaningProviderRequest.RoomCategory
		//		{
		//			Credits = 20,
		//			Id = Guid.NewGuid(),
		//			Name = "Student category"
		//		},
		//		new CleaningProviderRequest.RoomCategory
		//		{
		//			Credits = 30,
		//			Id = Guid.NewGuid(),
		//			Name = "Regular category"
		//		},
		//		new CleaningProviderRequest.RoomCategory
		//		{
		//			Credits = 40,
		//			Id = Guid.NewGuid(),
		//			Name = "Deluxe category"
		//		},
		//	};
		//}

		private CleaningProviderRequest.RoomTest _GenerateRoom(int roomNumber, CleaningProviderRequest.RoomCategory category, Guid floorId, string description = "", string hotelId = "", bool isDoNotDisturb = false, bool isOutOfService = false, string section = null, string subSection = null)
		{
			var roomName = $"Room {roomNumber}";
			return new CleaningProviderRequest.RoomTest
			{
				Name = roomName,
				Category = category,
				ExternalId = roomName,
				FloorId = floorId,
				HotelId = hotelId,
				RoomId = Guid.NewGuid(),
				IsDoNotDisturb = isDoNotDisturb,
				IsOutOfService = isOutOfService,
				Section = section,
				SubSection = subSection,
				Reservations = new CleaningProviderRequest.Reservation[0],
				PreviousCleaningDate = DateTime.UtcNow.AddDays(-5),
				Description = description,
				IsClean = false,
				IsBed = false,
				IsPriority = false,
			};
		}

		private List<CalendarDay> _GenerateCalendarDays(DateTime from, int numberOfDays)
		{
			var firstDateOfPeriod = from;
			var lastDateOfPeriod = from.AddDays(numberOfDays - 1);

			var calendarDays = new List<CalendarDay>();
			var dateOfPeriod = firstDateOfPeriod;
			while (dateOfPeriod <= lastDateOfPeriod)
			{
				var day = new CalendarDay
				{
					Date = dateOfPeriod,
					DayName = dateOfPeriod.DayOfWeek.ToString(),
					DateString = dateOfPeriod.ToString("yyyy-MM-dd")
				};
				calendarDays.Add(day);
				dateOfPeriod = dateOfPeriod.AddDays(1);
			}

			return calendarDays;
		}

		private DateTime _GetNextWeekday(DateTime start, DayOfWeek day)
		{
			// The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
			int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
			return start.AddDays(daysToAdd);
		}

		//private void _GenerateBuildings(int numberOfBuildings)
		//{
		//	for(int buildingCounter = 1; buildingCounter <= numberOfBuildings; buildingCounter++)
		//	{
		//		this._Buildings.Add(Guid.NewGuid(), $"Building {buildingCounter}");
		//	}
		//}
		//private Dictionary<Guid, string> _GenerateFloors()
		//{

		//}
		//private Dictionary<Guid, string> _GenerateSections(string sectionPrefix)
		//{

		//}
		//private Dictionary<Guid, string> _GenerateSubSections(string subSectionPrefix)
		//{

		//}

		private int _Random(int from, int to)
		{
			return this._randomGenerator.Next(from, to + 1);
		}
	}
}

/// <summary>
/// Clean every day at any time, everything
/// Clean every day at specific times, everything
/// Clean every day of the week at any time, everything
/// Clean only workdays of every week at any time, everything
/// Clean only workdays of every week at specific times, everything
/// Clean every start of the month, everything
/// Clean every middle of the month, everything
/// Clean every end of the month, everything
/// Clean every end of the month, everything
/// Clean every week of the year, everything
/// Clean periodically every 7 nights from the checkin
/// Clean periodically every 7 nights from first monday
/// Clean periodically every 7 nights from first tuesday
/// Clean periodically every 7 nights from first wednesday
/// Clean periodically every 7 nights from first thursday
/// Clean periodically every 7 nights from first friday
/// Clean periodically every 7 nights from first saturday
/// Clean periodically every 7 nights from first sunday
/// Clean periodically balance 2 cleanings over period, everything
/// Clean periodically balance 3 cleanings over reservation duration, everything
/// Clean periodically mixed balanced, everything
/// Clean periodically mixed balanced, postpone sundays to mondays, everything
/// 
/// Clean every day at any time, only departures
/// Clean every day at any time, only vacant // TODO: EXPAND TO INCLUDE PERIOD
/// Clean every day at any time, only stays
/// Clean every day at any time, only out of service
/// Clean every day at any time, only on nights 2, 4, 6, 8, 10 of the stay
/// Clean every day at any time, only in specific rooms with specific credit cost
/// Clean every day at any time, only specific floors
/// Clean every day at any time, only specific sections 
/// Clean every day at any time, only specific sub sections
/// Clean every day at any time, only if specific ReservationSpaceCategory is present
/// Clean every day at any time, only if specific Product/Tag is preset
/// Clean every day at any time, only if specific OtherProperty is preset
/// Clean every day at any time, only specific room categories with specific credit cost
/// Clean periodically mixed balanced, only stays
/// Clean only workdays of every week at any time, only stays
/// 
/// 
/// Clean every day at any time, only stays, only on nights 2, 4, 6, 8, 10 of the stay
/// Clean every day at any time, only stays, only in specific rooms with specific credit cost
/// Clean every day at any time, only stays, only specific floors
/// Clean every day at any time, only stays, only specific sections 
/// Clean every day at any time, only stays, only specific sub sections
/// Clean every day at any time, only stays, only if specific ReservationSpaceCategory is present
/// Clean every day at any time, only stays, only if specific Product/Tag is preset
/// Clean every day at any time, only stays, only if specific OtherProperty is preset
/// Clean every day at any time, only stays, only specific room categories with specific credit cost
/// Clean periodically mixed balanced, only stays, only specific floors
/// Clean only workdays of every week at any time, only stays, only specific floors
/// 
/// 
/// Clean every day at any time, only stays, only on nights 2, 4, 6, 8, 10 of the stay, only specific room categories with specific credit cost
/// Clean every day at any time, only stays, only in specific rooms with specific credit cost, only specific room categories with specific credit cost
/// Clean every day at any time, only stays, only specific floors, only specific room categories with specific credit cost
/// Clean every day at any time, only stays, only specific sections , only specific room categories with specific credit cost
/// Clean every day at any time, only stays, only specific sub sections, only specific room categories with specific credit cost
/// Clean every day at any time, only stays, only if specific ReservationSpaceCategory is present, only specific room categories with specific credit cost
/// Clean every day at any time, only stays, only if specific Product/Tag is preset, only specific room categories with specific credit cost
/// Clean every day at any time, only stays, only if specific OtherProperty is preset, only specific room categories with specific credit cost
/// Clean periodically mixed balanced, only stays, only specific floors, only specific room categories with specific credit cost
/// Clean only workdays of every week at any time, only stays, only specific floors, only specific room categories with specific credit cost
/// </summary>
/// <returns></returns>