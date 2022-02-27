using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Planner.RccSynchronization.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Planner.RccSynchronization
{
	public class RccApiClient : IRccApiClient
	{
		public async Task<RccPmsEventsSnapshot> GetPmsEvents(string hotelId)
		{
			var requestUrl = "https://rcapi.roomchecking.com/GetPmsEventsForRcNext";

			using (var client = new HttpClient())
			{

				client.DefaultRequestHeaders.Accept.Clear();

				client.DefaultRequestHeaders.Add("Api-Key", "");
				client.DefaultRequestHeaders.Add("Hotel-Id", hotelId);
				client.DefaultRequestHeaders.Add("Accept", "application/json");

				var response = await client.GetAsync(requestUrl);
				var jsonResponse = await response.Content.ReadAsStringAsync();

				JArray result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

				var events = new List<RccPmsEvent>();

				foreach (var eventToken in result)
				{
					var pmsEvent = new RccPmsEvent
					{
						Adults = eventToken["Adults"]?.Value<int?>(),
						Children = eventToken["Children"]?.Value<int?>(),
						Name = eventToken["Name"]?.Value<string>(),
						ReservationId = eventToken["ReservationId"]?.Value<string>(),
						Status = eventToken["Status"]?.Value<string>(),
						Timestamp = eventToken["Timestamp"]?.Value<string>(),
						RoomName = eventToken["RoomName"]?.Value<string>(),
						OldRoomName = eventToken["OldRoomName"]?.Value<string>(),
						Notes = eventToken["Notes"]?.Value<string>(),
						IsRaw = eventToken["IsRaw"]?.Value<bool?>(),
						HotelName = eventToken["HotelName"]?.Value<string>(),
						EventName = eventToken["EventName"]?.Value<string>(),
						Country = eventToken["Country"]?.Value<string>(),
						CoDate = eventToken["CoDate"]?.Value<string>(),
						CiDate = eventToken["CiDate"]?.Value<string>(),
					};

					events.Add(pmsEvent);
				}

				return new RccPmsEventsSnapshot
				{
					Events = events,
				};
			}
		}

		private RccReservationsSnapshot _DummyReservations()
		{
			var roomId1001 = new Guid("1821deb9-bf19-45ae-98b2-7bff3a046f80"); // Room
			var roomId1002 = new Guid("8cd304d5-5e87-406f-9cc5-5980fec7b607"); // Room
			var roomId1003 = new Guid("53781368-9ebb-4e4a-96f8-af8d64a4ca4d"); // Room
			var roomId1004 = new Guid("5796e393-a2a2-4d45-9788-43c40d57a760"); // Room
			var roomId1005 = new Guid("73bd0eb4-bf4b-4d8c-ab98-183e98835bd4"); // Room
			var roomId1006 = new Guid("a6553899-12d9-4cb0-a52a-d0a68106b369"); // Room
			var roomId1007 = new Guid("0b7ad5f5-5650-4bc9-bf13-15a0ee451391"); // Room
			var roomId1008 = new Guid("2d0a4df2-e116-4b52-9ab1-3833e508a0cb"); // Room
			var roomId1009 = new Guid("fa2dc476-1f7a-4481-9a1f-493a6b9e4cc7"); // Room
			var roomId1010 = new Guid("58e4f4d3-9aaf-4488-a518-f59c5f3de165");
			var roomId1011 = new Guid("3c649922-2454-46e0-92f2-5923dd2202c9");
			var roomId1012 = new Guid("85710e74-9fd5-4244-8d8a-37af2da9e285");


			var reservationIdMartin = "reservation-martin";
			var reservationIdJohn = "reservation-john";
			var reservationIdMatt = "reservation-matt";
			var reservationIdAaron = "reservation-aaron";
			var reservationIdBob = "reservation-bob";
			var reservationIdClide = "reservation-clide";

			var now = DateTime.Now;
			var currentLocalDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Unspecified);

			//// Reservation - arrival in the future
			//var reservationMartin = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddDays(2).AddHours(14),
			//	CheckOut = currentLocalDate.AddDays(5).AddHours(10),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Arrival",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Martin",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1001",
			//	RcRoomName = "1001",
			//	ReservationId = reservationIdMartin,
			//	Vip = null
			//};

			//// Reservation - arrival today, departure in the future
			//var reservationJohn = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(14).AddMinutes(30),
			//	CheckOut = currentLocalDate.AddDays(1).AddHours(10),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Arrived",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "John",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1001",
			//	RcRoomName = "1001",
			//	ReservationId = reservationIdJohn,
			//	Vip = null
			//};

			//// Reservation - arrived today, departure in the future
			//var reservationAaron = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(13).AddMinutes(33),
			//	CheckOut = currentLocalDate.AddDays(1).AddHours(10),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Arrived",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Aaron",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1002",
			//	RcRoomName = "1002",
			//	ReservationId = reservationIdAaron,
			//	Vip = null
			//};


			//// Reservation - arrival today, departure today
			//var reservationMatt = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(14),
			//	CheckOut = currentLocalDate.AddHours(20),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Arrival",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Matt",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1003",
			//	RcRoomName = "1003",
			//	ReservationId = reservationIdMatt,
			//	Vip = null
			//};

			//// Reservation - arrived today, departure today
			//var reservationBob = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(12).AddMinutes(22),
			//	CheckOut = currentLocalDate.AddHours(20),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Arrived",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Bob",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1004",
			//	RcRoomName = "1004",
			//	ReservationId = reservationIdBob,
			//	Vip = null
			//};

			//// Reservation - arrived today, departure today
			//var reservationBobAlt = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(12).AddMinutes(22),
			//	CheckOut = currentLocalDate.AddHours(20),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Departure",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "BobAlt",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1004",
			//	RcRoomName = "1004",
			//	ReservationId = "reservation-bob-alt",
			//	Vip = null
			//};

			//// Reservation - stay
			//var reservationClide = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddDays(-2).AddHours(12).AddMinutes(22),
			//	CheckOut = currentLocalDate.AddDays(2).AddHours(20),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Current",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Clide",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1005",
			//	RcRoomName = "1005",
			//	ReservationId = reservationIdClide,
			//	Vip = null
			//};

			//// Reservation - arrived before today, departure today
			//var reservationEthan = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddDays(-4).AddHours(16).AddMinutes(44),
			//	CheckOut = currentLocalDate.AddHours(20),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Departure",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Ethan",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1006",
			//	RcRoomName = "1006",
			//	ReservationId = "reservation-ethan",
			//	Vip = null
			//};

			//// Reservation - arrived before today, departed today
			//var reservationFrank = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddDays(-4).AddHours(10).AddMinutes(20),
			//	CheckOut = currentLocalDate.AddHours(20).AddMinutes(20),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Departed",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Frank",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1007",
			//	RcRoomName = "1007",
			//	ReservationId = "reservation-frank",
			//	Vip = null
			//};

			//// Reservation - arrived today, departure today
			//var reservationGary = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(10).AddMinutes(20),
			//	CheckOut = currentLocalDate.AddHours(16),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Departure",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Gary",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1008",
			//	RcRoomName = "1008",
			//	ReservationId = "reservation-gary",
			//	Vip = null
			//};

			//// Reservation - arrived today, departed today
			//var reservationHank = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(10).AddMinutes(10),
			//	CheckOut = currentLocalDate.AddHours(20).AddMinutes(20),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Departed",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Hank",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1009",
			//	RcRoomName = "1009",
			//	ReservationId = "reservation-hank",
			//	Vip = null
			//};

			//// Reservation - arrived today, departed today
			//var reservationIan = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddDays(-2).AddHours(14).AddMinutes(14),
			//	CheckOut = currentLocalDate.AddDays(10).AddHours(10).AddMinutes(0),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Current",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Ian",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1010",
			//	RcRoomName = "1010",
			//	ReservationId = "reservation-ian",
			//	Vip = null
			//};
			//// Reservation - arrived today, departed today
			//var reservationJake = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddDays(-20).AddHours(14).AddMinutes(0),
			//	CheckOut = currentLocalDate.AddHours(10).AddMinutes(0),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Departure",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Jake",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1011",
			//	RcRoomName = "1011",
			//	ReservationId = "reservation-jake",
			//	Vip = null
			//};
			//// Reservation - arrived today, departed today
			//var reservationKevin = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddDays(-10).AddHours(14).AddMinutes(0),
			//	CheckOut = currentLocalDate.AddHours(10).AddMinutes(10),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Departed",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Kevin",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1012",
			//	RcRoomName = "1012",
			//	ReservationId = "reservation-kevin",
			//	Vip = null
			//};
			//// Reservation - arrived today, departed today
			//var reservationLouie = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(14).AddMinutes(0),
			//	CheckOut = currentLocalDate.AddDays(10).AddHours(10).AddMinutes(0),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Arrival",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Louie",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1014",
			//	RcRoomName = "1014",
			//	ReservationId = "reservation-louie",
			//	Vip = null
			//};
			// Reservation - arrived today, departed today
			//var reservationMary = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(14).AddMinutes(0),
			//	CheckOut = currentLocalDate.AddDays(10).AddHours(10).AddMinutes(0),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Arrival",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Mary",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1015",
			//	RcRoomName = "1015",
			//	ReservationId = "reservation-mary",
			//	Vip = null
			//};
			//// Reservation - arrived today, departed today
			//var reservationNate = new RccReservation
			//{
			//	CheckIn = currentLocalDate.AddHours(14).AddMinutes(0),
			//	CheckOut = currentLocalDate.AddDays(10).AddHours(10).AddMinutes(0),
			//	ActualCheckIn = null,
			//	ActualCheckOut = null,
			//	Status = "Arrival",
			//	Adults = 1,
			//	Children = 0,
			//	Infants = 0,
			//	GroupName = null,
			//	Name = "Nate",
			//	OtherProperties = new RccOtherProperty[0],
			//	ParentRoomName = null,
			//	PmsNote = null,
			//	PMSRoomName = "1016",
			//	RcRoomName = "1016",
			//	ReservationId = "reservation-nate",
			//	Vip = null
			//};


			// Reservation - arrived today, departed today
			var reservationK4 = new RccReservation
			{
				CheckIn = currentLocalDate.AddDays(0).AddHours(14).AddMinutes(0),
				CheckOut = currentLocalDate.AddDays(10).AddHours(10).AddMinutes(0),
				ActualCheckIn = null,
				ActualCheckOut = null,
				Status = "Arrival",
				Adults = 1,
				Children = 0,
				Infants = 0,
				GroupName = null,
				Name = "K4",
				OtherProperties = new RccOtherProperty[0],
				ParentRoomName = null,
				PmsNote = null,
				PMSRoomName = "1025",
				RcRoomName = "1025",
				ReservationId = "reservation-k4",
				Vip = null
			};

			return new RccReservationsSnapshot
			{
				OutOfServiceRoomNames = new List<string>(),
				Products = new List<RccProduct>(),
				Reservations = new List<RccReservation>() 
				{
					reservationK4,
					//reservationAaron,
					//reservationBob,
					//reservationBobAlt,
					//reservationClide,
					//reservationEthan,
					//reservationFrank,
					//reservationGary,
					//reservationHank,
					//reservationJohn,
					//reservationMartin,
					//reservationMatt,
					//reservationIan,
					//reservationJake,
					//reservationKevin,
					////reservationLouie,
					//reservationMary,
					//reservationNate,
				},
			};
		}

		public async Task<RccReservationsSnapshot> GetReservations(string hotelId)
		{
			//if(hotelId == "6112261f089bda000f781ec6")
			//{
			//	return this._DummyReservations();
			//}

			var requestUrl = "https://rcapi.roomchecking.com/GetReservationsForRcNext";

			using (var client = new HttpClient())
			{

				client.DefaultRequestHeaders.Accept.Clear();
			
				client.DefaultRequestHeaders.Add("Api-Key", "");
				client.DefaultRequestHeaders.Add("Hotel-Id", hotelId);
				client.DefaultRequestHeaders.Add("Accept", "application/json");
			
				var response = await client.GetAsync(requestUrl);
				var jsonResponse = await response.Content.ReadAsStringAsync();

				JObject result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

				var products = new List<RccProduct>();
				var reservations = new List<RccReservation>();

				foreach (var productToken in (result["products"] as JArray))
				{
					var product = new RccProduct
					{
						CategoryId = productToken["CategoryId"]?.Value<string>(),
						IsActive = productToken["IsActive"]?.Value<bool?>() ?? false,
						ExternalName = productToken["ExternalName"]?.Value<string>(),
						ProductId = productToken["ProductId"]?.Value<string>(),
						ServiceId = productToken["ServiceId"]?.Value<string>(),
					};

					products.Add(product);
				}

				foreach(var reservationToken in (result["reservations"] as JArray))
				{
					var otherProperties = new List<RccOtherProperty>();
					var groupName = (string)null;

					try
					{
						var otherPropertiesToken = reservationToken["OtherProperties"] as JObject;
						foreach(var property in otherPropertiesToken.Properties())
						{
							if(property.Name == "group_name" && property.Value.Type == JTokenType.String)
							{
								groupName = property.Value.Value<string>();
							}

							otherProperties.Add(new RccOtherProperty
							{
								Key = property.Name,
								Value = property.Value.Type == JTokenType.String ? property.Value.Value<string>() : $"UNSUPPORTED TYPE: {property.Value.Type.ToString()}",
							});
						}
					}
					catch(Exception ex)
					{
						// Just ignore the exception
					}

					var rccReservation = new RccReservation
					{
						ActualCheckIn = reservationToken["ActualCheckIn"]?.Value<DateTime?>(),
						ActualCheckOut = reservationToken["ActualCheckOut"]?.Value<DateTime?>(),
						Adults = reservationToken["Adults"]?.Value<int?>(),
						CheckIn = reservationToken["CheckIn"]?.Value<DateTime?>(),
						CheckOut = reservationToken["CheckOut"]?.Value<DateTime?>(),
						Children = reservationToken["Children"]?.Value<int?>(),
						Infants = reservationToken["Infants"]?.Value<int?>(),
						Name = reservationToken["Name"]?.Value<string>() ?? "No name",
						PmsNote = reservationToken["PmsNote"]?.Value<string>(),
						PMSRoomName = reservationToken["PMSRoomName"]?.Value<string>(),
						ParentRoomName = reservationToken["ParentRoomName"]?.Value<string>(),
						RcRoomName = reservationToken["RcRoomName"]?.Value<string>(),
						ReservationId = reservationToken["ReservationId"]?.Value<string>(),
						Status = reservationToken["Status"]?.Value<string>(),
						Vip = reservationToken["Vip"]?.Value<string>(),
						OtherProperties = otherProperties,
						GroupName = groupName,
					};

					reservations.Add(rccReservation);
				}

				return new RccReservationsSnapshot 
				{ 
					OutOfServiceRoomNames = result["outOfServiceRoomNames"].ToObject<string[]>(), 
					Reservations = reservations,
					Products = products,
				};
			}
		}
	}
}
