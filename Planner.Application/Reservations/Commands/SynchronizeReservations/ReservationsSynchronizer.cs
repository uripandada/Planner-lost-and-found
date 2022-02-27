using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.RccSynchronization;
using Planner.RccSynchronization.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Reservations.Commands.SynchronizeReservations
{
	public class OccupancyChangeError
	{
		public string ReservationId { get; set; }
		public string GuestName { get; set; }
		public Guid OldRoomId { get; set; }
		public string OldRoomName { get; set; }
		public Guid NewRoomId { get; set; }
		public string NewRoomName { get; set; }
		public string Message { get; set; }
	}
	public class OccupancyChangeOldRoomWarning
	{
		public string ReservationId { get; set; }
		public string GuestName { get; set; }
		public string RoomExternalId { get; set; }
		public string Message { get; set; }
	}

	public class MultipleRoomMatchWarning
	{
		public string ReservationId { get; set; }
		public string Message { get; set; }
		public IEnumerable<MultipleRoomMatchWarningItem> MatchedRooms { get; set; }
	}

	public class MultipleRoomMatchWarningItem
	{
		public bool IsChosen { get; set; }
		public Guid RoomId { get; set; }
		public string RoomName { get; set; }
		public string ExternalId { get; set; }
	}

	public class RoomExternalIdChangeWarning
	{
		public Guid RoomId { get; set; }
		public string ReservationId { get; set; }
		public string RoomName { get; set; }
		public string OldExternalId { get; set; }
		public string NewExternalId { get; set; }
		public string Message { get; set; }
	}

	public class ReservationLogMessage
	{
		public string Type { get; set; }
		public string ReservationId { get; set; }
		public Guid? RoomId { get; set; }
		public List<string> Messages { get; set; } = new List<string>();
	}
	public class RoomLogMessage
	{
		public string Type { get; set; }
		public Guid RoomId { get; set; }
		public List<string> Messages { get; set; } = new List<string>();
	}

	public class ReservationChangedMessage
	{
		public string Type { get; set; }
		public string ReservationId { get; set; }
		public Guid? RoomId { get; set; }
		public Guid? BedId { get; set; }
		public string Message { get; set; }
	}

	public class ReservationChange
	{
		public bool DoInsertReservation { get; set; }
		public bool DoUpdateReservation { get; set; }
		public ReservationChangedMessage Message { get; set; }
		public Domain.Entities.Reservation Reservation { get; set; }
	}

	public class ReservationChanges
	{
		public IEnumerable<Domain.Entities.RoomHistoryEvent> RoomHistoryEvents { get; set; }
		public bool UpdateRooms { get; set; }
		public IEnumerable<ReservationChange> ReservationsToInsert { get; set; }
		public IEnumerable<ReservationChange> ReservationsToUpdate { get; set; }
		public IEnumerable<ReservationChange> ReservationsToDeactivate { get; set; }
	}

	public class OutOfServiceChanges
	{
		public IEnumerable<Domain.Entities.RoomHistoryEvent> RoomHistoryEvents { get; set; }
		public IEnumerable<OutOfServiceChange> Changes { get; set; }
	}

	public class OutOfServiceChange
	{
		public Guid RoomId { get; set; }
		public bool IsOutOfService { get; set; }
	}

	public class ProductChanges
	{
		public IEnumerable<Domain.Entities.RccProduct> ProductsToInsert { get; set; }
		public IEnumerable<Domain.Entities.RccProduct> ProductsToUpdate { get; set; }
	}

	public enum RccReservationSynchronizationMessageSeverity
	{ 
		EXCEPTION = 1,
		ERROR = 2,
		INFO = 3,
		WARNING = 4,
	}

	public enum RccReservationSynchronizationMessageType
	{ 
		SYNC_STARTED = 1,
		SYNC_ENDED = 2,
		LOAD_LOCAL_DATE = 3,
		LOAD_RESERVATIONS_FROM_RCC = 4,
		LOAD_PMS_EVENTS_FROM_RCC = 5,
		LOAD_DEFAULT_ROOM_CATEGORY = 6,
		DEFAULT_ROOM_CATEGORY_DOESNT_EXIST = 7,
		FIND_PRODUCT_CHANGES = 8,
		SAVE_PRODUCT_CHANGES = 9,
		FIND_ROOMS_AND_BEDS_CHANGES = 10,
		SAVE_ROOMS_AND_BEDS_CHANGES = 11,
		FIND_RESERVATIONS_CHANGES = 12,
		SAVE_RESERVATIONS_CHANGES = 13,
		FIND_HOUSEKEEPING_CHANGES = 14,
		SAVE_HOUSEKEEPING_CHANGES = 15,
		FIND_SAVE_OOS_CHANGES = 16,
		FIND_PMS_EVENT_CHANGES = 17,
		SAVE_PMS_EVENT_CHANGES = 18,

	}

	public class RccReservationSynchronizationMessageParameters
	{
		public string SynchronizationId { get; set; }
		public string HotelId { get; set; }
		public string RoomIds { get; set; }
		public string ReservationIds { get; set; }
	}

	public class RccReservationSynchronizationMessage
	{
		public Guid Id { get; set; }
		public DateTime At { get; set; }
		public RccReservationSynchronizationMessageType Type { get; set; }
		public RccReservationSynchronizationMessageSeverity Severity { get; set; }
		public RccReservationSynchronizationMessageParameters Parameters { get; set; }
		public string Message { get; set; }
	}

	public interface IReservationsSynchronizer
	{
		Task<ReservationsSynchronizationResult> Synchronize(string hotelId, string hotelName, CancellationToken cancellationToken);
	}

	public class ReservationsSynchronizer : IReservationsSynchronizer
	{
		private readonly IRccApiClient _rccApi;
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		public ReservationsSynchronizer(IRccApiClient rccApi, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._rccApi = rccApi;
			this._databaseContext = databaseContext;

			// This is here because of the fact that service can be called form an automatic scheduled jobs service
			if (contextAccessor.HttpContext != null)
			{
				this._userId = contextAccessor.UserId();
			}
			else
			{
				this._userId = databaseContext.Users.OrderBy(x => x.Id).First().Id;
			}
		}

		private DateTime _synchronizationTime;
		private DateTime _localSynchronizationTime;
		private DateTime _utcSynchronizationTime;

		private RccReservationsSnapshot _rccReservationsSnapshot = null;
		private RccPmsEventsSnapshot _rccPmsEventsSnapshot = null;
		private Domain.Entities.RoomCategory _defaultRoomCategory = null;
		private Dictionary<string, RccPmsEvent[]> _pmsEvents = new Dictionary<string, RccPmsEvent[]>();

		private List<RccReservationSynchronizationMessage> ReservationLogMessages = new List<RccReservationSynchronizationMessage>();

		public async Task<ReservationsSynchronizationResult> Synchronize(string hotelId, string hotelName, CancellationToken cancellationToken)
		{
			this._ResetState();

			var synchronizationId = Guid.NewGuid();
			var dateProvider = new HotelLocalDateProvider();
			var dateTime = DateTime.UtcNow;


			// LOAD THE CURRENT HOTEL LOCAL TIME
			try
			{
				dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, hotelId, true);
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while loading hotel local date. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.LOAD_LOCAL_DATE,
				});
			}

			this._synchronizationTime = DateTime.UtcNow;
			this._localSynchronizationTime = dateTime;
			this._utcSynchronizationTime = DateTime.UtcNow;

			// LOAD THE RESERVATIONS FROM RCC
			try
			{
				this._rccReservationsSnapshot = await this._rccApi.GetReservations(hotelId);
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while loading reservations from RCC. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.LOAD_RESERVATIONS_FROM_RCC,
				});
			}

			// LOAD THE PMS EVENTS FROM RCC
			try
			{
				this._rccPmsEventsSnapshot = await this._rccApi.GetPmsEvents(hotelId);
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while loading PMS events from RCC. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.LOAD_PMS_EVENTS_FROM_RCC,
				});
			}
			// Group pms events by reservation
			this._pmsEvents = this._rccPmsEventsSnapshot.Events.GroupBy(e => e.ReservationId).ToDictionary(group => group.Key, group => group.ToArray());

			// Clean the reservations by trimming room names, reservation ids,...
			this._cleanRccReservationsSnapshot();

			// LOAD DEFAULT ROOM CATEGORY
			try
			{
				this._defaultRoomCategory = await this._LoadDefaultRoomCategory();
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while loading default room category. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.LOAD_DEFAULT_ROOM_CATEGORY,
				});
			}

			if(this._defaultRoomCategory == null)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Default room category doesn't exist.",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.ERROR,
					Type = RccReservationSynchronizationMessageType.DEFAULT_ROOM_CATEGORY_DOESNT_EXIST,
				});
			}

			// Find product changes
			var productChanges = (ProductChanges)null;
			try
			{
				productChanges = await this._FindProductChanges(this._rccReservationsSnapshot.Products);
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while finding product changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.FIND_PRODUCT_CHANGES,
				});
			}

			// Save product changes
			try
			{
				await this._SaveProductChanges(productChanges, cancellationToken);
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while saving product changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.SAVE_PRODUCT_CHANGES,
				});
			}

			// Find room and bed changes
			var roomAndBedChanges = (RoomsAndBedsChanges)null;
			try
			{
				roomAndBedChanges = await this._FindRoomsAndBedsChanges(hotelId, this._userId, this._utcSynchronizationTime, this._rccReservationsSnapshot.Reservations, dateTime);
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while finding rooms and beds changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.FIND_ROOMS_AND_BEDS_CHANGES,
				});
			}

			// Save room and bed changes
			try
			{
				await this._SaveRoomAndBedChanges(roomAndBedChanges, cancellationToken);
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while saving rooms and beds changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.SAVE_ROOMS_AND_BEDS_CHANGES,
				});
			}

			// Find reservation changes
			var reservationChanges = (ReservationChanges)null;
			try
			{
				reservationChanges = await this._FindReservationsChanges(hotelId, this._userId, this._utcSynchronizationTime, this._rccReservationsSnapshot.Reservations, dateTime);
			}
			catch(Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while finding reservations changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.FIND_RESERVATIONS_CHANGES,
				});
			}

			// Save reservation changes
			try
			{
				await this._SaveReservationChanges(reservationChanges, cancellationToken);
			}
			catch (Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while saving reservations changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.SAVE_RESERVATIONS_CHANGES,
				});
			}

			// Find housekeeping changes
			var housekeepingChanges = (HousekeepingChanges)null;
			try
			{
				var newRoomIds = roomAndBedChanges.RoomsToInsert.Select(r => r.Id).ToHashSet();
				var newBedIds = roomAndBedChanges.BedsToInsert.Select(r => r.Id).ToHashSet();

				housekeepingChanges = await this._FindHousekeepingChanges(hotelId, this._localSynchronizationTime, newRoomIds, newBedIds);
			}
			catch (Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while finding housekeeping changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.FIND_HOUSEKEEPING_CHANGES,
				});
			}

			// Save housekeeping changes
			try
			{
				await this._SaveHousekeepingChanges(housekeepingChanges, cancellationToken);
			}
			catch (Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while saving housekeeping changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.SAVE_HOUSEKEEPING_CHANGES,
				});
			}

			// Find and save room OOS changes
			try
			{
				await this._FindAndSaveRoomOutOfServiceChanges(hotelId, dateTime);
			}
			catch (Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while finding and saving OOS changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.FIND_SAVE_OOS_CHANGES,
				});
			}

			// Find PMS event changes
			var eventsToInsert = (IEnumerable<Domain.Entities.RoomHistoryEvent>)null;
			try
			{
				eventsToInsert = await this._FindPmsEventChanges(hotelId, this._localSynchronizationTime);
			}
			catch (Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while finding PMS event changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.FIND_PMS_EVENT_CHANGES,
				});
			}

			// Save PMS event changes
			try
			{
				if (eventsToInsert.Any())
				{
					await this._databaseContext.RoomHistoryEvents.AddRangeAsync(eventsToInsert);
					await this._databaseContext.SaveChangesAsync(cancellationToken);
				}
			}
			catch (Exception ex)
			{
				this.ReservationLogMessages.Add(new RccReservationSynchronizationMessage
				{
					At = DateTime.UtcNow,
					Id = Guid.NewGuid(),
					Message = $"Error while saving PMS event changes. {ex.Message}. {ex.StackTrace ?? ""}",
					Parameters = new RccReservationSynchronizationMessageParameters { HotelId = hotelId, SynchronizationId = synchronizationId.ToString() },
					Severity = RccReservationSynchronizationMessageSeverity.EXCEPTION,
					Type = RccReservationSynchronizationMessageType.SAVE_PMS_EVENT_CHANGES,
				});
			}

			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			// SAVE LOG MESSAGES
			//await this._databaseContext.

			var syncResult = new ReservationsSynchronizationResult
			{
				HotelId = hotelId,
				HotelName = hotelName,
				NewReservations = reservationChanges.ReservationsToInsert.Select(r => this._CreateReservationData(r.Reservation)).ToArray(),
				UpdatedReservations = reservationChanges.ReservationsToUpdate.Select(r => this._CreateReservationData(r.Reservation)).ToArray(),
				DeactivatedReservations = reservationChanges.ReservationsToDeactivate.Select(r => this._CreateReservationData(r.Reservation)).ToArray(),
				AutogeneratedBeds = roomAndBedChanges.BedsToInsert.Select(b => this._CreateReservationBedData(b.RoomId, b.RoomId.ToString(), b)).ToArray(),
				AutogeneratedRooms = roomAndBedChanges.RoomsToInsert.Select(r => this._CreateReservationRoomData(r)).ToArray(),
				RoomsWithNewExternalId = roomAndBedChanges.RoomsWithNewExternalId.Select(r => this._CreateReservationRoomData(r)).ToArray(),
				BedsWithNewExternalId = roomAndBedChanges.BedsWithNewExternalId.Select(b => this._CreateReservationBedData(b.RoomId, b.RoomId.ToString(), b)).ToArray(),
				NewProducts = productChanges.ProductsToInsert.Select(p => new SyncProductData { Id = p.Id, Name = p.ExternalName }).ToArray(),
			};

			return syncResult;
		}

		private void _cleanRccReservationsSnapshot()
		{
			if(this._rccReservationsSnapshot == null || this._rccReservationsSnapshot.Reservations == null)
			{
				return;
			}

			foreach(var r in this._rccReservationsSnapshot.Reservations)
			{
				if (r.PMSRoomName.IsNotNull()) r.PMSRoomName = r.PMSRoomName.Trim();
				if (r.RcRoomName.IsNotNull()) r.RcRoomName = r.RcRoomName.Trim();
				if (r.ParentRoomName.IsNotNull()) r.ParentRoomName = r.ParentRoomName.Trim();
				if (r.ReservationId.IsNotNull()) r.ReservationId = r.ReservationId.Trim();
			}
		}

		private void _ResetState()
		{
			this._rccReservationsSnapshot = null;
			this._defaultRoomCategory = null;
		}

		private async Task _SaveHousekeepingChanges(HousekeepingChanges changes, CancellationToken cancellationToken)
		{
			var dataChanged = false;

			if (changes.BedsToUpdate.Any() || changes.RoomsToUpdate.Any())
			{
				dataChanged = true;
			}

			if (changes.RoomHistoryEvents != null && changes.RoomHistoryEvents.Any())
			{
				dataChanged = true;
				await this._databaseContext.RoomHistoryEvents.AddRangeAsync(changes.RoomHistoryEvents);
			}

			if (dataChanged)
			{
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}
		}
		
		private async Task _SaveReservationChanges(ReservationChanges changes, CancellationToken cancellationToken)
		{
			var dataChanged = false;
			if (changes.ReservationsToInsert.Any())
			{
				await this._databaseContext.Reservations.AddRangeAsync(changes.ReservationsToInsert.Select(r => r.Reservation).ToArray());
				dataChanged = true;
			}

			if (dataChanged || changes.ReservationsToUpdate.Any() || changes.ReservationsToDeactivate.Any() || changes.UpdateRooms)
			{
				dataChanged = true;
			}

			if(changes.RoomHistoryEvents != null && changes.RoomHistoryEvents.Any())
			{
				dataChanged = true;
				await this._databaseContext.RoomHistoryEvents.AddRangeAsync(changes.RoomHistoryEvents);
			}

			if (dataChanged)
			{
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}
		}

		private async Task _SaveProductChanges(ProductChanges changes, CancellationToken cancellationToken)
		{
			var dataChanged = false;
			if (changes.ProductsToInsert.Any())
			{
				await this._databaseContext.RccProducts.AddRangeAsync(changes.ProductsToInsert);
				dataChanged = true;
			}

			if (dataChanged || changes.ProductsToUpdate.Any())
			{
				dataChanged = true;
			}

			if (dataChanged)
			{
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}
		}

		private async Task _SaveRoomAndBedChanges(RoomsAndBedsChanges changes, CancellationToken cancellationToken)
		{
			var dataChanged = false;
			if (changes.RoomsToInsert.Any())
			{
				await this._databaseContext.Rooms.AddRangeAsync(changes.RoomsToInsert);
				dataChanged = true;
			}
			
			if (changes.BedsToInsert.Any())
			{
				await this._databaseContext.RoomBeds.AddRangeAsync(changes.BedsToInsert);
				dataChanged = true;
			}

			if(changes.RoomHistoryEvents != null && changes.RoomHistoryEvents.Any())
			{
				await this._databaseContext.RoomHistoryEvents.AddRangeAsync(changes.RoomHistoryEvents);
				dataChanged = true;
			}

			if (dataChanged || changes.RoomsToUpdate.Any() || changes.BedsToUpdate.Any())
			{
				dataChanged = true;
			}

			if (dataChanged)
			{
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}
		}

		private class RoomsAndBedsChanges
		{
			public IEnumerable<Domain.Entities.Room> RoomsToInsert { get; set; }
			public IEnumerable<Domain.Entities.Room> RoomsToUpdate { get; set; }
			public IEnumerable<Domain.Entities.Room> RoomsWithNewExternalId { get; set; }
			public IEnumerable<Domain.Entities.RoomBed> BedsToInsert { get; set; }
			public IEnumerable<Domain.Entities.RoomBed> BedsToUpdate { get; set; }
			public IEnumerable<Domain.Entities.RoomBed> BedsWithNewExternalId { get; set; }
			public IEnumerable<Domain.Entities.RoomHistoryEvent> RoomHistoryEvents { get; internal set; }
		}

		private async Task<ReservationChanges> _FindReservationsChanges(string hotelId, Guid userId, DateTime utcSynchronizationTime, IEnumerable<RccReservation> rccReservations, DateTime currentHotelLocalDateTime)
		{
			var currentHotelLocalDate = currentHotelLocalDateTime.Date;

			var reservations = await this._databaseContext.Reservations
				.Where(r => r.HotelId == hotelId)
				.ToDictionaryAsync(r => r.Id);

			var rooms = await this._databaseContext.Rooms
				.Include(r => r.RoomBeds)
				.Where(r => r.HotelId == hotelId)
				.ToListAsync();

			var reservationChangesToInsert = new List<ReservationChange>();
			var reservationChangesToUpdate = new List<ReservationChange>();
			var reservationChangesToDeactivate = new List<ReservationChange>();
			var existingReservationIds = new HashSet<string>();

			var rccReservationsMap = rccReservations.GroupBy(r => r.ReservationId).ToDictionary(group => group.Key, group => group.ToArray());

			foreach(var existingReservation in reservations.Values)
			{
				if (!existingReservationIds.Contains(existingReservation.Id)) existingReservationIds.Add(existingReservation.Id);

				if (rccReservationsMap.ContainsKey(existingReservation.Id))
				{
					// UPDATE RESERVATION!

					// TODO: IF THE rccReservationsMap[existingReservation.Id] HAS MORE THAN 1 ELEMENT, IT MEANS THAT THERE IS A DUPLICATE RESERVATION
					// TODO: IF THE rccReservationsMap[existingReservation.Id] HAS MORE THAN 1 ELEMENT, IT MEANS THAT THERE IS A DUPLICATE RESERVATION
					
					var rccReservation = rccReservationsMap[existingReservation.Id].First();

					var roomExternalId = rccReservation.ParentRoomName.IsNotNull() ? rccReservation.ParentRoomName : rccReservation.PMSRoomName;
					var bedExternalId = rccReservation.ParentRoomName.IsNotNull() ? rccReservation.PMSRoomName : null;
					
					var room = rooms.FirstOrDefault(r => r.ExternalId == roomExternalId);
					var bed = (Domain.Entities.RoomBed)null;
					if (room != null && bedExternalId.IsNotNull())
					{
						bed = room.RoomBeds.FirstOrDefault(rb => rb.ExternalId == bedExternalId);
					}

					var newReservation = this._CreateReservation(rccReservation, hotelId, room?.Id, bed?.Id, currentHotelLocalDate);
					var reservationUpdateChange = this._FindReservationUpdateChanges(existingReservation, newReservation, rccReservation, room, bed);
					if(reservationUpdateChange.DoUpdateReservation)
					{
						reservationChangesToUpdate.Add(reservationUpdateChange);
					}
				}
				else
				{
					//// DEACTIVATE RESERVATION!

					if (!existingReservation.IsActive)
					{
						continue;
					}

					existingReservation.IsActive = false;
					reservationChangesToDeactivate.Add(new ReservationChange
					{
						Reservation = existingReservation,
						DoInsertReservation = false,
						DoUpdateReservation = true,
						Message = this._AddReservationMessage("INFO", existingReservation.Id, existingReservation.RoomId, existingReservation.RoomBedId, $"Reservation updated. Deactivated.")
					});
				}
			}

			foreach (var rccReservation in rccReservations)
			{
				if (existingReservationIds.Contains(rccReservation.ReservationId)) 
				{
					// RESERVATION IS ALREADY UPDATED
					continue;
				}

				// INSERT RESERVATION
				var roomExternalId = rccReservation.ParentRoomName.IsNotNull() ? rccReservation.ParentRoomName : rccReservation.PMSRoomName;
				var bedExternalId = rccReservation.ParentRoomName.IsNotNull() ? rccReservation.PMSRoomName : null;

				var room = rooms.FirstOrDefault(r => r.ExternalId == roomExternalId);
				var bed = (Domain.Entities.RoomBed)null;
				if (room != null && bedExternalId.IsNotNull())
				{
					bed = room.RoomBeds.FirstOrDefault(rb => rb.ExternalId == bedExternalId);
				}

				var reservationInsertChange = this._FindReservationInsertChanges(rccReservation, room, bed, hotelId, currentHotelLocalDate);
				reservationChangesToInsert.Add(reservationInsertChange);
			}

			var updateRooms = false;
			var roomHistoryEvents = new List<Domain.Entities.RoomHistoryEvent>();

			foreach (var room in rooms)
			{
				if(room.TypeKey == RoomTypeEnum.HOSTEL.ToString() && room.RoomBeds != null)
				{
					foreach(var bed in room.RoomBeds)
					{
						var oldIsOccupied = bed.IsOccupied;
						var newIsOccupied = bed.IsOccupied;

						// CI, CO, STAY, ARR, DEP, ARR|DEP, CI|DEP, ARR|CO, CI|CO
						if (reservations.Any(r =>
							r.Value.IsActive &&
							r.Value.RoomId == room.Id &&
							r.Value.RoomBedId == bed.Id &&
							r.Value.ReservationStatusKey.IsNotNull() &&
							(!r.Value.ReservationStatusKey.StartsWith("ARR") &&
							!r.Value.ReservationStatusKey.EndsWith("CO"))						
						))
						{
							newIsOccupied = true;
						}
						else if (reservationChangesToInsert.Any(u =>
							u.Reservation.IsActive &&
							u.Reservation.RoomId == room.Id &&
							u.Reservation.RoomBedId == bed.Id &&
							u.Reservation.ReservationStatusKey.IsNotNull() &&
							(!u.Reservation.ReservationStatusKey.StartsWith("ARR") &&
							!u.Reservation.ReservationStatusKey.EndsWith("CO"))
						))
						{
							newIsOccupied = true;
						}
						else
						{
							newIsOccupied = false;
						}

						if (newIsOccupied != oldIsOccupied)
						{
							bed.IsOccupied = newIsOccupied;
							updateRooms = true;
							roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
							{
								At = currentHotelLocalDateTime,
								Id = Guid.NewGuid(),
								Message = newIsOccupied ? "Bed is now occupied - automatic service detected the checkin." : "Bed is now vacant - automatic service detected the checkout.",
								NewData = null,
								OldData = null,
								RoomBedId = bed.Id,
								RoomId = bed.RoomId,
								Type = newIsOccupied ? RoomEventType.RCCSYNC_ROOM_IS_OCCUPIED : RoomEventType.RCCSYNC_ROOM_IS_VACANT,
							});
						}

						var oldRccHkStatus = bed.RccHousekeepingStatus;
						var status = bed.CalculateCurrentHousekeepingStatus();
						var newRccHkStatus = status.RccHousekeepingStatusCode;
						if (oldRccHkStatus != newRccHkStatus)
						{
							bed.RccHousekeepingStatus = newRccHkStatus;
							updateRooms = true;
							roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
							{
								At = currentHotelLocalDateTime,
								Id = Guid.NewGuid(),
								Message = $"RCC housekeeping status changed: {oldRccHkStatus.ToString()} -> {newRccHkStatus.ToString()}",
								NewData = null,
								OldData = null,
								RoomBedId = bed.Id,
								RoomId = bed.RoomId,
								Type = RoomEventType.RCCSYNC_RCC_HK_STATUS_CHANGED
							});
						}

						if (oldIsOccupied && !newIsOccupied) // Someone has checked out
						{
							bed.IsGuestCurrentlyIn = false;
							updateRooms = true;
							roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
							{
								At = currentHotelLocalDateTime,
								Id = Guid.NewGuid(),
								Message = "Guest left the room bed - automatic service detected the checkout.",
								NewData = null,
								OldData = null,
								RoomBedId = bed.Id,
								RoomId = bed.RoomId,
								Type = RoomEventType.RCCSYNC_GUEST_LEFT_ROOM
							});
						}
						else if(!oldIsOccupied && newIsOccupied) // Someone has checked in
						{
							bed.IsGuestCurrentlyIn = true;
							updateRooms = true;
							roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
							{
								At = currentHotelLocalDateTime,
								Id = Guid.NewGuid(),
								Message = "Guest entered the room bed - automatic service detected the checkin.",
								NewData = null,
								OldData = null,
								RoomBedId = bed.Id,
								RoomId = bed.RoomId,
								Type = RoomEventType.RCCSYNC_GUEST_ENTERED_ROOM
							});
						}
					}
				}
				else
				{
					var oldIsOccupied = room.IsOccupied;
					var newIsOccupied = room.IsOccupied;

					if(reservations.Any(r =>
						r.Value.IsActive &&
						r.Value.RoomBedId == null &&
						r.Value.RoomId == room.Id &&
						r.Value.ReservationStatusKey.IsNotNull() &&
						!r.Value.ReservationStatusKey.StartsWith("ARR")&&
						!r.Value.ReservationStatusKey.EndsWith("CO")
					))
					{
						newIsOccupied = true;
					}
					else if(reservationChangesToInsert.Any(u => 
						u.Reservation.IsActive &&
						u.Reservation.RoomBedId == null &&
						u.Reservation.RoomId == room.Id &&
						u.Reservation.ReservationStatusKey.IsNotNull() &&
						!u.Reservation.ReservationStatusKey.StartsWith("ARR") &&
						!u.Reservation.ReservationStatusKey.EndsWith("CO")
					))
					{
						newIsOccupied = true;
					}
					else
					{
						newIsOccupied = false;
					}

					if(newIsOccupied != oldIsOccupied)
					{
						room.IsOccupied = newIsOccupied;
						updateRooms = true;
						roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
						{
							At = currentHotelLocalDateTime,
							Id = Guid.NewGuid(),
							Message = newIsOccupied ? "Room is now occupied - automatic service detected the checkin." : "Room is now vacant - automatic service detected the checkout.",
							NewData = null,
							OldData = null,
							RoomBedId = null,
							RoomId = room.Id,
							Type = newIsOccupied ? RoomEventType.RCCSYNC_ROOM_IS_OCCUPIED : RoomEventType.RCCSYNC_ROOM_IS_VACANT,
						});
					}

					var oldRccHkStatus = room.RccHousekeepingStatus;
					var status = room.CalculateCurrentHousekeepingStatus();
					var newRccHkStatus = status.RccHousekeepingStatusCode;
					if(oldRccHkStatus != newRccHkStatus)
					{
						room.RccHousekeepingStatus = newRccHkStatus;
						updateRooms = true;
						roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
						{
							At = currentHotelLocalDateTime,
							Id = Guid.NewGuid(),
							Message = $"RCC housekeeping status changed: {oldRccHkStatus.ToString()} -> {newRccHkStatus.ToString()}",
							NewData = null,
							OldData = null,
							RoomBedId = null,
							RoomId = room.Id,
							Type = RoomEventType.RCCSYNC_RCC_HK_STATUS_CHANGED
						});
					}

					if (oldIsOccupied && !newIsOccupied) // Someone has checked out
					{
						room.IsGuestCurrentlyIn = false;
						updateRooms = true;
						roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
						{
							At = currentHotelLocalDateTime,
							Id = Guid.NewGuid(),
							Message = "Guest left the room - automatic service detected the checkout.",
							NewData = null,
							OldData = null,
							RoomBedId = null,
							RoomId = room.Id,
							Type = RoomEventType.RCCSYNC_GUEST_LEFT_ROOM
						});
					}
					else if (!oldIsOccupied && newIsOccupied) // Someone has checked in
					{
						room.IsGuestCurrentlyIn = true;
						updateRooms = true;
						roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
						{
							At = currentHotelLocalDateTime,
							Id = Guid.NewGuid(),
							Message = "Guest entered the room - automatic service detected the checkin.",
							NewData = null,
							OldData = null,
							RoomBedId = null,
							RoomId = room.Id,
							Type = RoomEventType.RCCSYNC_GUEST_ENTERED_ROOM
						});
					}
				}
			}


			return new ReservationChanges
			{
				RoomHistoryEvents = roomHistoryEvents,
				UpdateRooms = updateRooms,
				ReservationsToInsert = reservationChangesToInsert,
				ReservationsToUpdate = reservationChangesToUpdate,
				ReservationsToDeactivate = reservationChangesToDeactivate,
			};
		}

		private async Task _FindAndSaveRoomOutOfServiceChanges(string hotelId, DateTime currentHotelLocalDateTime)
		{
			if(this._rccReservationsSnapshot.OutOfServiceRoomNames == null || !this._rccReservationsSnapshot.OutOfServiceRoomNames.Any())
			{
				return;
			}

			var outOfServiceRoomNames = this._rccReservationsSnapshot.OutOfServiceRoomNames.Where(rn => rn.IsNotNull()).ToArray();
			var outOfServiceRoomNamesSet = new HashSet<string>();
			var roomHistoryEvents = new List<Domain.Entities.RoomHistoryEvent>();

			foreach (var rn in outOfServiceRoomNames)
			{
				if(!outOfServiceRoomNamesSet.Contains(rn)) outOfServiceRoomNamesSet.Add(rn);
			}

			var rooms = await this._databaseContext.Rooms.Where(r => r.HotelId == hotelId && (outOfServiceRoomNames.Contains(r.Name) || r.IsOutOfService)).ToArrayAsync();
			var dataChanged = false;
			foreach(var room in rooms)
			{
				if (outOfServiceRoomNamesSet.Contains(room.Name))
				{
					if (room.IsOutOfService)
					{
						// THE ROOM IS ALREADY OUT OF SERVICE - DO NOTHING
						continue;
					}
					else
					{
						// SET OUT OF SERVICE STATUS
						room.IsOutOfService = true;
						roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
						{
							At = currentHotelLocalDateTime,
							Id = Guid.NewGuid(),
							Message = $"Room is out of service.",
							NewData = null,
							OldData = null,
							RoomBedId = null,
							RoomId = room.Id,
							Type = RoomEventType.RCCSYNC_ROOM_OUT_OF_SERVICE,
						});
						dataChanged = true;
					}
				}
				else
				{
					if (room.IsOutOfService)
					{
						// REMOVE OUT OF SERVICE STATUS
						room.IsOutOfService = false;
						roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
						{
							At = currentHotelLocalDateTime,
							Id = Guid.NewGuid(),
							Message = $"Room is in service.",
							NewData = null,
							OldData = null,
							RoomBedId = null,
							RoomId = room.Id,
							Type = RoomEventType.RCCSYNC_ROOM_IN_SERVICE,
						});
						dataChanged = true;
					}
					else
					{
						// DO NOTHING, room is not contained in OOS room names list nor is it out of service.
						continue;
					}
				}
			}

			if (dataChanged)
			{
				if (roomHistoryEvents.Any())
				{
					await this._databaseContext.RoomHistoryEvents.AddRangeAsync(roomHistoryEvents);
				}

				await this._databaseContext.SaveChangesAsync(CancellationToken.None);
			}
		}
		
		private async Task<RoomsAndBedsChanges> _FindRoomsAndBedsChanges(string hotelId, Guid userId, DateTime utcSynchronizationTime, IEnumerable<RccReservation> rccReservations, DateTime currentHotelLocalDateTime)
		{
			var rooms = await this._databaseContext.Rooms
				.Include(r => r.RoomBeds)
				.Where(r => r.HotelId == hotelId).ToListAsync();

			var autogeneratedRooms = new Dictionary<string, Domain.Entities.Room>();
			var autogeneratedBeds = new Dictionary<string, Dictionary<string, Domain.Entities.RoomBed>>();

			var roomsToUpdate = new Dictionary<Guid, Domain.Entities.Room>();
			var roomsToInsert = new Dictionary<Guid, Domain.Entities.Room>();
			var bedsToInsert = new Dictionary<Guid, Domain.Entities.RoomBed>();
			var bedsToUpdate = new Dictionary<Guid, Domain.Entities.RoomBed>();
			var roomsWithNewExternalId = new Dictionary<Guid, Domain.Entities.Room>();
			var bedsWithNewExternalId = new Dictionary<Guid, Domain.Entities.RoomBed>();
			var roomHistoryEvents = new List<Domain.Entities.RoomHistoryEvent>();

			foreach (var rccReservation in rccReservations)
			{
				// The reservation is not assigned to a room so skip the room generation
				if (rccReservation.PMSRoomName.IsNull())
				{
					continue;
				}

				// If the reservation's PMSRoomName is set try to find a match by ExternalId
				// External roomId in the system is referenced to the PMSRoomName property when synchronizing reservations from RCC.
				var externalRoomId = rccReservation.ParentRoomName.IsNotNull() ? rccReservation.ParentRoomName : rccReservation.PMSRoomName;

				var room = (Domain.Entities.Room)null;

				// First check if the room is already autogenerated
				if (autogeneratedRooms.ContainsKey(externalRoomId))
				{
					room = autogeneratedRooms[externalRoomId];
				}
				// If it is not, try to find it amongst the existing rooms
				else
				{
					// Try to find the room by externalId.
					room = rooms.FirstOrDefault(r => r.ExternalId.IsNotNull() && r.ExternalId == externalRoomId);

					if (room == null)
					{
						// Try to find the room by name.
						room = rooms.FirstOrDefault(r => r.Name == externalRoomId);

						if (room != null && room.ExternalId == null)
						{
							roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
							{
								At = currentHotelLocalDateTime,
								Id = Guid.NewGuid(),
								Message = $"Room's external id changed: NULL -> {externalRoomId}.",
								NewData = null,
								OldData = null,
								RoomBedId = null,
								RoomId = room.Id,
								Type = RoomEventType.RCCSYNC_ROOM_EXTERNAL_ID_CHANGED,
							});
					
							// If the room is found by name and the external id is not set, UPDATE IT
							room.ExternalId = externalRoomId;
							room.ModifiedAt = utcSynchronizationTime;
							room.ModifiedById = userId;

							if (!roomsToUpdate.ContainsKey(room.Id)) roomsToUpdate.Add(room.Id, room);
							if (!roomsWithNewExternalId.ContainsKey(room.Id)) roomsWithNewExternalId.Add(room.Id, room);
						}
					}
				}

				// If the room is still null, it needs to be autogenerated
				if (room == null)
				{
					room = this._CreateAnAutogeneratedRoom(rccReservation, hotelId);

					if (!autogeneratedRooms.ContainsKey(externalRoomId)) 
					{ 
						autogeneratedRooms.Add(externalRoomId, room);
					}
					if (!roomsToInsert.ContainsKey(room.Id))
					{
						roomsToInsert.Add(room.Id, room);
						roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
						{
							At = currentHotelLocalDateTime,
							Id = Guid.NewGuid(),
							Message = $"Room created by the system.",
							NewData = null,
							OldData = null,
							RoomBedId = null,
							RoomId = room.Id,
							Type = RoomEventType.RCCSYNC_ROOM_CREATED,
						});
					}
				}

				// If the room bed is not set, we have finished room check
				if (rccReservation.ParentRoomName.IsNull())
				{
					continue;
				}

				if (!autogeneratedBeds.ContainsKey(externalRoomId)) autogeneratedBeds.Add(externalRoomId, new Dictionary<string, Domain.Entities.RoomBed>());

				var externalBedId = rccReservation.PMSRoomName;
				var bed = (Domain.Entities.RoomBed)null;

				// First check if the bed is already generated
				if (autogeneratedBeds[externalRoomId].ContainsKey(externalBedId))
				{
					bed = autogeneratedBeds[externalRoomId][externalBedId];
				}
				// If it is not, try to find it amongst the existing beds
				else
				{
					// Try to find it by external id.
					bed = room.RoomBeds.FirstOrDefault(b => b.ExternalId.IsNotNull() && b.ExternalId == externalBedId);

					if (bed == null)
					{
						// Try to find it by name.
						bed = room.RoomBeds.FirstOrDefault(b => b.Name == externalBedId);

						if (bed != null && bed.ExternalId == null)
						{
							// If the bed is found by name and the external id is not set, UPDATE IT
							bed.ExternalId = externalBedId;
							roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
							{
								At = currentHotelLocalDateTime,
								Id = Guid.NewGuid(),
								Message = $"Room bed's {externalBedId} external id changed: NULL -> {externalBedId}.",
								NewData = null,
								OldData = null,
								RoomBedId = bed.Id,
								RoomId = bed.RoomId,
								Type = RoomEventType.RCCSYNC_BED_EXTERNAL_ID_CHANGED,
							});

							if (!bedsToUpdate.ContainsKey(bed.Id)) bedsToUpdate.Add(bed.Id, bed);
							if (!bedsWithNewExternalId.ContainsKey(bed.Id)) bedsWithNewExternalId.Add(bed.Id, bed);
						}
					}
				}

				if (bed == null)
				{
					bed = this._CreateAnAutogeneratedBed(rccReservation, room.Id);

					if(!autogeneratedBeds[externalRoomId].ContainsKey(externalBedId)) autogeneratedBeds[externalRoomId].Add(externalBedId, bed);
					if (!bedsToInsert.ContainsKey(bed.Id)) 
					{ 
						bedsToInsert.Add(bed.Id, bed);
						roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
						{
							At = currentHotelLocalDateTime,
							Id = Guid.NewGuid(),
							Message = $"Room bed {externalBedId} created by the system.",
							NewData = null,
							OldData = null,
							RoomBedId = bed.Id,
							RoomId = bed.RoomId,
							Type = RoomEventType.RCCSYNC_BED_CREATED,
						});
					}
				}
			}

			return new RoomsAndBedsChanges
			{
				RoomHistoryEvents = roomHistoryEvents,
				BedsToInsert = bedsToInsert.Values.ToArray(),
				BedsToUpdate = bedsToUpdate.Values.ToArray(),
				BedsWithNewExternalId = bedsWithNewExternalId.Values.ToArray(),
				RoomsToInsert = roomsToInsert.Values.ToArray(),
				RoomsToUpdate= roomsToUpdate.Values.ToArray(),
				RoomsWithNewExternalId = roomsWithNewExternalId.Values.ToArray(),
			};
		}

		private async Task<Domain.Entities.RoomCategory> _LoadDefaultRoomCategory()
		{
			var categories = await this._databaseContext.RoomCategorys.ToArrayAsync();

			var category = categories.FirstOrDefault(c => c.IsDefaultForReservationSync);

			if (category == null)
			{
				category = categories.FirstOrDefault(c => c.IsSystemDefaultForReservationSync);
			}

			if (category == null)
			{
				category = categories.FirstOrDefault();
			}

			return category;
		}

		private Domain.Entities.Room _CreateAnAutogeneratedRoom(RccReservation reservation, string hotelId)
		{
			var externalId = reservation.ParentRoomName.IsNotNull() ? reservation.ParentRoomName : reservation.PMSRoomName;
			var roomName = reservation.ParentRoomName.IsNotNull() ? reservation.ParentRoomName : reservation.RcRoomName;

			return new Domain.Entities.Room
			{
				Id = Guid.NewGuid(),
				AreaId = null,
				BuildingId = null,
				CategoryId = this._defaultRoomCategory == null ? null : this._defaultRoomCategory.Id,
				CreatedAt = this._synchronizationTime,
				CreatedById = this._userId,
				ModifiedById = this._userId,
				ModifiedAt = this._synchronizationTime,
				ExternalId = externalId,
				FloorId = null,
				FloorSectionName = null,
				FloorSubSectionName = null,
				HotelId = hotelId,
				IsAutogeneratedFromReservationSync = true,
				Name = roomName,
				OrdinalNumber = 0,
				TypeKey = RoomTypeEnum.UNKNOWN.ToString(),
				IsDoNotDisturb = false,
				IsClean = true,
				IsOccupied = false,
				IsOutOfOrder = false,
				IsCleaningInProgress = false,
				RoomBeds = new List<Domain.Entities.RoomBed>(),
				Reservations = new List<Domain.Entities.Reservation>(),
				IsGuestCurrentlyIn = false,
				IsInspected = false,
				IsOutOfService = false,
				IsCleaningPriority = false,
				RccRoomStatus = RccRoomStatusCode.VAC,
				RccHousekeepingStatus = RccHousekeepingStatusCode.HD,
			};
		}

		private Domain.Entities.RoomBed _CreateAnAutogeneratedBed(RccReservation reservation, Guid roomId)
		{
			return new Domain.Entities.RoomBed
			{
				Id = Guid.NewGuid(),
				ExternalId = reservation.PMSRoomName,
				Name = reservation.PMSRoomName,
				IsAutogeneratedFromReservationSync = true,
				RoomId = roomId,
				IsClean = true,
				IsCleaningInProgress = false,
				IsDoNotDisturb = false,
				IsOccupied = false,
				IsOutOfOrder = false,
				IsCleaningPriority = false,
				IsGuestCurrentlyIn = false,
				IsInspected = false,
				IsOutOfService = false,
				RccRoomStatus = RccRoomStatusCode.VAC,
				RccHousekeepingStatus = RccHousekeepingStatusCode.HD,
			};
		}

		private Domain.Entities.Reservation _CreateReservation(RccReservation pmsReservation, string hotelId, Guid? roomId, Guid? roomBedId, DateTime currentHotelLocalDate)
		{
			var roomExternalId = pmsReservation.PMSRoomName;
			var roomName = pmsReservation.RcRoomName;
			var bedExternalId = (string)null;
			var bedName = (string)null;

			if (pmsReservation.ParentRoomName.IsNotNull())
			{
				roomExternalId = pmsReservation.ParentRoomName;
				roomName = pmsReservation.ParentRoomName;
				bedExternalId = pmsReservation.PMSRoomName;
				bedName = pmsReservation.RcRoomName;
			}

			var r = new Domain.Entities.Reservation
			{
				Id = pmsReservation.ReservationId,
				ActualCheckIn = pmsReservation.ActualCheckIn,
				ActualCheckOut = pmsReservation.ActualCheckOut,
				CheckIn = pmsReservation.CheckIn,
				CheckOut = pmsReservation.CheckOut,
				GuestName = pmsReservation.Name,
				HotelId = hotelId,
				IsActive = true,
				IsSynchronizedFromRcc = true,
				LastTimeModifiedBySynchronization = this._synchronizationTime,
				NumberOfAdults = pmsReservation.Adults.HasValue ? pmsReservation.Adults.Value : 0,
				NumberOfChildren = pmsReservation.Children.HasValue ? pmsReservation.Children.Value : 0,
				NumberOfInfants = pmsReservation.Infants.HasValue ? pmsReservation.Infants.Value : 0,
				OtherProperties = pmsReservation.OtherProperties.Select(op => new Domain.Entities.ReservationOtherProperty { Key = op.Key, Value = op.Value }).ToArray(),
				PmsNote = pmsReservation.PmsNote,
				PMSRoomName = roomExternalId,
				RccReservationStatusKey = pmsReservation.Status,
				RoomName = roomName,
				SynchronizedAt = this._synchronizationTime,
				Vip = pmsReservation.Vip,
				RoomId = roomId,
				Group = pmsReservation.GroupName,
				BedName = bedName,
				PMSBedName = bedExternalId,
				RoomBedId = roomBedId,
				
				IsActiveToday = false,
				ReservationStatusKey = null,
				ReservationStatusDescription = null,
			};

			//// RESERVATION ACTUAL CHECKIN AND ACTUAL CHECKOUTS SHOULD BE LOADED FROM EVENTS HERE
			//if (this._pmsEvents.ContainsKey(r.Id))
			//{
			//	var events = this._pmsEvents[r.Id];
			//}

			var pmsReservationStatusValue = pmsReservation.Status.IsNotNull() ? pmsReservation.Status.Trim().ToLower() : "__#NO_VALUE#__"; // Just some string that is not a valid pmsReservation.Status value (Current, Arrival, Arrived, Departure, Departed)
			var isRccCurrentReservation = pmsReservationStatusValue == "current";
			var isRccDepartedReservation = pmsReservationStatusValue == "departed";
			var reservationStatusChange = Domain.Entities.ReservationEntityExtensions.CalculateReservationStatus(r, currentHotelLocalDate, isRccCurrentReservation, isRccDepartedReservation);

			if (reservationStatusChange.IsToday)
			{
				r.IsActiveToday = true;
				r.ReservationStatusKey = string.Join("|", reservationStatusChange.Statuses.Select(s => s.StatusKey).ToArray());
				r.ReservationStatusDescription = string.Join("|", reservationStatusChange.Statuses.Select(s => s.StatusDescription).ToArray());
			}
			else
			{
				r.IsActiveToday = false;
				r.ReservationStatusKey = null;
				r.ReservationStatusDescription = null;
			}

			return r;
		}

		private Domain.Entities.RccProduct _CreateRccProduct(RccProduct product)
		{
			return new Domain.Entities.RccProduct
			{
				CategoryId = product.CategoryId,
				ExternalName = product.ExternalName ?? "NO-EXTERNAL-NAME",
				Id = product.ProductId ?? Guid.NewGuid().ToString(),
				IsActive = product.IsActive,
				ServiceId = product.ServiceId,
			};
		}

		private ReservationChangedMessage _AddReservationMessage(string type, string reservationId, Guid? roomId, Guid? bedId, string message)
		{
			return new ReservationChangedMessage
			{
				BedId = bedId,
				Message = message,
				ReservationId = reservationId,
				RoomId = roomId,
				Type = type,
			};
		}

		private class HousekeepingChanges
		{
			public IEnumerable<Domain.Entities.Room> RoomsToUpdate { get; set; }
			public IEnumerable<Domain.Entities.RoomBed> BedsToUpdate { get; set; }
			public IEnumerable<Domain.Entities.RoomHistoryEvent> RoomHistoryEvents { get; internal set; }
		}

		private async Task<IEnumerable<Domain.Entities.RoomHistoryEvent>> _FindPmsEventChanges(string hotelId, DateTime localSynchronizationTime)
		{
			var eventsToInsert =new List<Domain.Entities.RoomHistoryEvent>();

			var rooms = await this._databaseContext.Rooms
				.Include(r => r.RoomBeds)
				.Where(r => r.HotelId == hotelId && r.ExternalId != null)
				.ToDictionaryAsync(r => r.ExternalId);

			var roomBedHistoryEvents = (await this._databaseContext
				.RoomHistoryEvents
				.Where(rbh => rbh.RoomBedId != null && rbh.RoomBed.Room.HotelId == hotelId && rbh.Type == RoomEventType.PMS_EVENT)
				.Select(rbh => new 
				{
					RoomId = rbh.RoomId,
					RoomExternalId = rbh.Room.ExternalId,
					RoomBedId = rbh.RoomBedId,
					RoomBedExternalId = rbh.RoomBed.ExternalId,
					Message = rbh.Message,
					At = rbh.At
				})
				.ToArrayAsync())
				.Where(rh => rh.RoomBedExternalId != null)
				.GroupBy(rh => rh.RoomBedExternalId)
				.ToDictionary(rh => rh.Key, rh => rh.ToArray());

			var roomHistoryEvents = (await this._databaseContext
				.RoomHistoryEvents
				.Where(rh => rh.RoomBedId == null && rh.Room.HotelId == hotelId && rh.Type == RoomEventType.PMS_EVENT)
				.Select(rbh => new
				{
					RoomId = rbh.RoomId,
					RoomExternalId = rbh.Room.ExternalId,
					Message = rbh.Message,
					At = rbh.At
				})
				.ToArrayAsync())
				.Where(rh => rh.RoomExternalId != null)
				.GroupBy(rh => rh.RoomExternalId)
				.ToDictionary(rh => rh.Key, rh => rh.ToArray());

			foreach(var reservationId in this._pmsEvents.Keys)
			{
				var reservationEvents = this._pmsEvents[reservationId];

				foreach(var e in reservationEvents)
				{
					if (e.RoomName.IsNull())
					{
						continue;
					}

					DateTime eventDate;
					if(
						!DateTime.TryParseExact(e.Timestamp, "yyyy-MM-ddTHH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out eventDate) &&
						!DateTime.TryParse(e.Timestamp, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out eventDate)
					)
					{
						continue;
					}

					var roomExternalId = e.RoomName;
					var isBedEvent = false;

					if (isBedEvent)
					{

					}
					else
					{
						if (!rooms.ContainsKey(roomExternalId))
						{
							continue;
						}

						var room = rooms[roomExternalId];
						var eventExists = false;
						if (roomHistoryEvents.ContainsKey(roomExternalId))
						{
							var events = roomHistoryEvents[roomExternalId];
							//2021-12-17T19:59:01.193
							// not always

							eventExists = events.FirstOrDefault(ei => ei.Message == e.EventName && ei.At == eventDate) != null;
						}

						if (!eventExists)
						{
							eventsToInsert.Add(new Domain.Entities.RoomHistoryEvent
							{
								At = eventDate,
								Id = Guid.NewGuid(),
								Message = e.EventName,
								NewData = null,
								OldData = null,
								RoomBedId = null,
								RoomId = room.Id,
								Type = RoomEventType.PMS_EVENT,
								UserId = null,
							});
						}
					}
				}
			}

			return eventsToInsert;
		}

		private async Task<HousekeepingChanges> _FindHousekeepingChanges(string hotelId, DateTimeOffset localSynchronizationTime, HashSet<Guid> newRoomIds, HashSet<Guid> newBedIds)
		{
			var rooms = await this._databaseContext.Rooms
				.Include(r => r.Reservations.Where(rr => rr.IsActive))
				.Where(r => (r.TypeKey == "HOTEL" || r.TypeKey == "APPARTMENT") && r.HotelId == hotelId)
				.ToArrayAsync();

			var roomBeds = await this._databaseContext.RoomBeds
				.Include(rb => rb.Reservations.Where(rbr => rbr.IsActive))
				.Where(rb => rb.Room.TypeKey == "HOSTEL" && rb.Room.HotelId == hotelId)
				.ToArrayAsync();

			var date = localSynchronizationTime.Date;
			var dateTime = localSynchronizationTime.DateTime;

			var roomsToUpdate = new List<Domain.Entities.Room>();
			var bedsToUpdate = new List<Domain.Entities.RoomBed>();
			var roomHistoryEvents = new List<Domain.Entities.RoomHistoryEvent>();

			foreach (var room in rooms)
			{
				var activeReservations = room.Reservations.Where(rm => rm.CheckIn.HasValue && rm.CheckIn.Value.Date <= date && (!rm.CheckOut.HasValue || (rm.CheckOut.HasValue && rm.CheckOut.Value.Date >= date))).ToArray();
				var updateRoom = false;

				var roomStatus = room.CalculateReservationStatusForDate(dateTime, activeReservations);
				if(room.RccRoomStatus == null || room.RccRoomStatus != roomStatus.RccRoomStatusCode)
				{
					roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
					{
						At = dateTime,
						Id = Guid.NewGuid(),
						Message = $"RCC room status changed: {(room.RccRoomStatus == null ? "N/A" : room.RccRoomStatus)} -> {roomStatus.RccRoomStatusCode.ToString()}.",
						NewData = null,
						OldData = null,
						RoomBedId = null,
						RoomId = room.Id,
						Type = RoomEventType.RCCSYNC_RCC_ROOM_STATUS_CHANGED,
					});

					room.RccRoomStatus = roomStatus.RccRoomStatusCode;
					updateRoom = true;
				}

				if(updateRoom) roomsToUpdate.Add(room);
			}

			foreach(var bed in roomBeds)
			{
				var activeReservations = bed.Reservations.Where(b => b.CheckIn.HasValue && b.CheckIn.Value.Date <= date && (!b.CheckOut.HasValue || (b.CheckOut.HasValue && b.CheckOut.Value.Date >= date))).ToArray();
				var updateBed = false;

				var roomStatus = bed.CalculateReservationStatusForDate(dateTime, activeReservations);
				if (bed.RccRoomStatus != null || bed.RccRoomStatus != roomStatus.RccRoomStatusCode)
				{
					roomHistoryEvents.Add(new Domain.Entities.RoomHistoryEvent
					{
						At = dateTime,
						Id = Guid.NewGuid(),
						Message = $"RCC room bed status changed: {(bed.RccRoomStatus == null ? "N/A" : bed.RccRoomStatus)} -> {roomStatus.RccRoomStatusCode.ToString()}.",
						NewData = null,
						OldData = null,
						RoomBedId = bed.Id,
						RoomId = bed.RoomId,
						Type = RoomEventType.RCCSYNC_RCC_ROOM_BED_STATUS_CHANGED,
					});

					bed.RccRoomStatus = roomStatus.RccRoomStatusCode;
					updateBed = true;
				}

				if (updateBed) bedsToUpdate.Add(bed);
			}

			return new HousekeepingChanges
			{
				RoomHistoryEvents = roomHistoryEvents,
				BedsToUpdate = bedsToUpdate,
				RoomsToUpdate = roomsToUpdate,
			};
		}

		//private bool _isAnyReservationOccupyingTheRoom(IEnumerable<Domain.Entities.Reservation> reservations, DateTime dateTime)
		//{
		//	if (reservations == null || !reservations.Any())
		//	{
		//		// If there are no active reservations the room is not occupied
		//		return false;
		//	}
		//	else
		//	{
		//		// If there is an active reservation that checked in before today
		//		foreach (var reservation in reservations)
		//		{
		//			if(this._isReservationOccupyingTheRoom(reservation, dateTime))
		//			{
		//				return true;
		//			}
		//		}
		//	}

		//	return false;
		//}

		//private bool _isReservationOccupyingTheRoom(Domain.Entities.Reservation reservation, DateTime dateTime)
		//{
		//	switch (reservation.RccReservationStatusKey)
		//	{
		//		case "Unknown":
		//			break;
		//		case "Arrival":
		//			if (reservation.CheckIn.HasValue && reservation.CheckIn.Value < dateTime)
		//			{
		//				if (reservation.CheckOut.HasValue && reservation.CheckOut.Value < dateTime)
		//				{
		//					break;
		//				}

		//				return true;
		//			}
		//			break;
		//		case "Current":
		//			return true;
		//		case "Departure":
		//			if (reservation.CheckOut.HasValue && reservation.CheckOut.Value > dateTime)
		//			{
		//				if (reservation.CheckIn.HasValue && reservation.CheckIn.Value > dateTime)
		//				{
		//					break;
		//				}

		//				return true;
		//			}
		//			break;
		//		case "Canceled":
		//			break;
		//		case "ToRemove":
		//			break;
		//	}

		//	return false;
		//}

		private ReservationData _CreateReservationData(Domain.Entities.Reservation pmsReservation)
		{
			return new ReservationData
			{
				Id = pmsReservation.Id,
				ActualCheckIn = pmsReservation.ActualCheckIn,
				ActualCheckOut = pmsReservation.ActualCheckIn,
				CheckIn = pmsReservation.CheckIn,
				CheckOut = pmsReservation.CheckOut,
				GuestName = pmsReservation.GuestName,
				HotelId = pmsReservation.HotelId,
				IsActive = pmsReservation.IsActive,
				IsSynchronizedFromRcc = pmsReservation.IsSynchronizedFromRcc,
				LastTimeModifiedBySynchronization = pmsReservation.LastTimeModifiedBySynchronization,
				NumberOfAdults = pmsReservation.NumberOfAdults,
				NumberOfChildren = pmsReservation.NumberOfChildren,
				NumberOfInfants = pmsReservation.NumberOfInfants,
				OtherProperties = pmsReservation.OtherProperties.Select(op => new ReservationOtherPropertyData { Key = op.Key, Value = op.Value }).ToArray(),
				PmsNote = pmsReservation.PmsNote,
				PMSRoomName = pmsReservation.PMSRoomName,
				RccReservationStatusKey = pmsReservation.RccReservationStatusKey,
				RoomName = pmsReservation.RoomName,
				SynchronizedAt = pmsReservation.SynchronizedAt,
				Vip = pmsReservation.Vip,
				Group = pmsReservation.Group,
			};
		}

		private ReservationRoomData _CreateReservationRoomData(Domain.Entities.Room room)
		{
			return new ReservationRoomData
			{
				ExternalId = room.ExternalId,
				Id = room.Id,
				Name = room.Name
			};
		}
		
		private ReservationBedData _CreateReservationBedData(Guid roomId, string roomName, Domain.Entities.RoomBed bed)
		{
			return new ReservationBedData
			{
				Id = bed.Id,
				Name = bed.Name,
				ExternalId = bed.ExternalId,
				RoomId = roomId,
				RoomName = roomName,
			};
		}

		private async Task<ProductChanges> _FindProductChanges(IEnumerable<RccProduct> rccProducts)
		{
			var products = await this._databaseContext.RccProducts.ToListAsync();

			var toInsert = new List<Domain.Entities.RccProduct>();
			var toUpdate = new List<Domain.Entities.RccProduct>();

			var checkedProductIds = new HashSet<string>();

			foreach (var existingProduct in products)
			{
				var product = rccProducts.FirstOrDefault(p => p.ProductId == existingProduct.Id);

				if (product == null)
				{
					existingProduct.IsActive = false;
				}
				else
				{
					existingProduct.CategoryId = product.CategoryId;
					existingProduct.ExternalName = product.ExternalName;
					existingProduct.ServiceId = product.ServiceId;
					existingProduct.IsActive = product.IsActive;
				}

				toUpdate.Add(existingProduct);
				checkedProductIds.Add(existingProduct.Id);
			}

			foreach (var product in this._rccReservationsSnapshot.Products)
			{
				if (checkedProductIds.Contains(product.ProductId))
					continue;

				toInsert.Add(this._CreateRccProduct(product));
			}

			return new ProductChanges
			{
				ProductsToInsert = toInsert,
				ProductsToUpdate = toUpdate,
			};
		}

		private ReservationChange _FindReservationInsertChanges(RccReservation rccReservation, Domain.Entities.Room room, Domain.Entities.RoomBed bed, string hotelId, DateTime currentHotelLocalDate)
		{
			var newReservation = this._CreateReservation(rccReservation, hotelId, room?.Id, bed?.Id, currentHotelLocalDate);
			var changedMessage = this._AddReservationMessage("INFO", rccReservation.ReservationId, room?.Id, bed?.Id, "New reservation.");

			return new ReservationChange
			{
				Message = changedMessage,
				Reservation = newReservation,
				DoInsertReservation = true,
				DoUpdateReservation = false,
			};
		}

		private enum ReservationChangeType
		{
			NO_CHANGE = 1,
			BED_TO_BED_SAME_ROOM = 2,
			BED_TO_ROOM_ERROR = 3,
			BED_TO_ROOM = 4,
			BED_UNASSIGNED = 5,
			ROOM_TO_BED_ERROR = 6,
			ROOM_TO_BED = 7,
			ROOM_TO_ROOM = 8,
			ROOM_UNASSIGNED = 9,
			ROOM_ASSIGNED = 10,
			BED_ASSIGNED = 11,
			BED_TO_BED_DIFFERENT_ROOMS = 12,
			NO_CHANGE_UNASSIGNED = 13,

		}

		private ReservationChangeType _GetReservationRoomMoveType(string oldRoomId, string oldBedId, string newRoomId, string newBedId)
		{
			var changeType = ReservationChangeType.NO_CHANGE;

			if (oldRoomId == null)
			{
				if (newRoomId == null)
				{
					// 1. - no change
					changeType = ReservationChangeType.NO_CHANGE_UNASSIGNED;

				}
				else
				{
					if (newBedId == null)
					{
						// 10. room ASSIGNMENT
						changeType = ReservationChangeType.ROOM_ASSIGNED;
					}
					else
					{
						// 11. room and bed ASSIGNMENT
						changeType = ReservationChangeType.BED_ASSIGNED;
					}
				}
			}
			else
			{
				if (newRoomId == null)
				{
					if (oldBedId == null)
					{
						// 9. room UNASSIGNMENT
						changeType = ReservationChangeType.ROOM_UNASSIGNED;
					}
					else
					{
						// 5. bed UNASSIGNMENT
						changeType = ReservationChangeType.BED_UNASSIGNED;
					}
				}
				else
				{
					if (oldRoomId == newRoomId)
					{
						if (oldBedId == null && newBedId == null)
						{
							// 1. - no change
							changeType = ReservationChangeType.NO_CHANGE;
						}
						else if (oldBedId == null)
						{
							// 6. room to bed MOVE - ERROR
							changeType = ReservationChangeType.ROOM_TO_BED_ERROR;
						}
						else if (newBedId == null)// newBedId == null
						{
							// 3. bed to room MOVE - ERROR
							changeType = ReservationChangeType.BED_TO_ROOM_ERROR;
						}
						else
						{
							if (oldBedId == newBedId)
							{
								// 1. - no change
								changeType = ReservationChangeType.NO_CHANGE;
							}
							else
							{
								// 2. bed to bed MOVE (in the same room)
								changeType = ReservationChangeType.BED_TO_BED_SAME_ROOM;
							}
						}
					}
					else // oldRoomId != newRoomId
					{
						if (oldBedId == null && newBedId == null)
						{
							// 8. room to room MOVE
							changeType = ReservationChangeType.ROOM_TO_ROOM;
						}
						else if (oldBedId == null)
						{
							// 7. room to room and bed MOVE
							changeType = ReservationChangeType.ROOM_TO_BED;
						}
						else if (newBedId == null) // newBedId == null
						{
							// 4. room and bed to room MOVE
							changeType = ReservationChangeType.BED_TO_ROOM;
						}
						else
						{
							if (oldBedId == newBedId)
							{
								// 1. - no change
								changeType = ReservationChangeType.NO_CHANGE;
							}
							else
							{
								// 12. bed to bed MOVE (between different rooms)
								changeType = ReservationChangeType.BED_TO_BED_DIFFERENT_ROOMS;
							}
						}
					}
				}
			}

			return changeType;
		}

		private ReservationChange _FindReservationUpdateChanges(Domain.Entities.Reservation reservation, Domain.Entities.Reservation newReservation, RccReservation rccReservation, Domain.Entities.Room room, Domain.Entities.RoomBed bed)
		{
			var isReservationActive = newReservation.PMSRoomName.IsNotNull();

			// 1. No change

			// 2. Move between beds in the same hostel room
			// room X bed A -> room X bed B ===> bed to bed MOVE

			// 3. Move from the hostel room bed to the same room which is hotel room.
			// THIS IS AN ERROR CASE AND SHOULD NOT HAPPEN!!
			// room X bed A -> room X ===> bed to room MOVE

			// 4. Move from the hostel room bed to another hotel room
			// room X bed A -> room Y ===> room and bed to room MOVE

			// 5. Move from the hostel room bed no room
			// room X bed A -> nothing ===> bed UNASSIGNMENT

			// 6. Move from hotel room to hostel room bed
			// THIS IS AN ERROR CASE AND SHOULD NOT HAPPEN!!
			// room X -> room X bed A ===> room to bed MOVE

			// 7. Move from hotel room to another hostel room bed
			// room X -> room Y bed C ===> room to room and bed MOVE

			// 8. Move from hotel room to hotel room
			// room X -> room Y ===> room to room MOVE

			// 9. Move from hotel room to nothing
			// room X -> nothing ===> room UNASSIGNMENT

			// 10. Hotel room assigned to reservation
			// nothing -> room X ===> room ASSIGNMENT - hotel room was assigned to an unassigned reservation

			// 11. Hostel room bed is assigned to reservation
			// nothing -> room X bed A ===> room and bed ASSIGNMENT - hostel room bed was assigned to an unassigned reservation

			// 12. Move between beds between different hostel rooms
			// room X bed A -> room Y bed C ===> bed to bed MOVE

			var oldRoomExternalId = reservation.PMSRoomName;
			var oldBedExternalId = reservation.PMSBedName;
			var newRoomExternalId = newReservation.PMSRoomName;
			var newBedExternalId = newReservation.PMSBedName;

			var roomMoveType = this._GetReservationRoomMoveType(oldRoomExternalId, oldBedExternalId, newRoomExternalId, newBedExternalId);
			var changeMessage = (ReservationChangedMessage)null;

			// First check is for room move. Changed flag can be changed later also
			var reservationMoved = false;

			switch (roomMoveType)
			{
				case ReservationChangeType.BED_ASSIGNED:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation assigned to the hostel room {newRoomExternalId}, bed {newBedExternalId}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.BED_TO_BED_DIFFERENT_ROOMS:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation moved from the hostel room {oldRoomExternalId}, bed {oldBedExternalId} to the hostel room {newRoomExternalId}, bed {newBedExternalId}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.BED_TO_BED_SAME_ROOM:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation moved from the bed {oldBedExternalId} to the bed {newBedExternalId} in the hostel room {newRoomExternalId}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.BED_TO_ROOM:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation moved from the hostel room {oldRoomExternalId}, bed {oldBedExternalId} to the hotel room {newRoomExternalId}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.BED_TO_ROOM_ERROR:
					changeMessage = this._AddReservationMessage("ERROR", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation moved from the hostel room {oldRoomExternalId}, bed {oldBedExternalId} to the hotel room {newRoomExternalId}. THE ROOM CHANGED TYPE FROM HOSTEL TO HOTEL.");
					reservationMoved = true;
					break;
				case ReservationChangeType.BED_UNASSIGNED:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation unassigned from the hostel room {oldRoomExternalId}, bed {oldBedExternalId}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.ROOM_ASSIGNED:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation assigned to the hotel room {reservation.PMSRoomName}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.ROOM_TO_BED:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation moved from the hotel room {oldRoomExternalId} to the hostel room {newRoomExternalId}, bed {newBedExternalId}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.ROOM_TO_BED_ERROR:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation moved from the hotel room {oldRoomExternalId} to the hostel room {newRoomExternalId}, bed {newBedExternalId}. THE ROOM CHANGED TYPE FROM HOTEL TO HOSTEL.");
					reservationMoved = true;
					break;
				case ReservationChangeType.ROOM_TO_ROOM:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation moved from the hotel room {oldRoomExternalId} to the hotel room {newRoomExternalId}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.ROOM_UNASSIGNED:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation unassigned from the hotel room {reservation.PMSRoomName}.");
					reservationMoved = true;
					break;
				case ReservationChangeType.NO_CHANGE_UNASSIGNED:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated. Reservation is still unassigned.");
					reservationMoved = false;
					break;
				case ReservationChangeType.NO_CHANGE:
				default:
					changeMessage = this._AddReservationMessage("INFO", reservation.Id, room?.Id, bed?.Id, $"Reservation updated.");
					reservationMoved = false;
					break;
			}

			var reservationFieldChangedMessage = this._DidReservationDataChange(reservation, newReservation);

			if (reservationMoved || reservationFieldChangedMessage != null)
			{
				reservation.ActualCheckIn = newReservation.ActualCheckIn;
				reservation.ActualCheckOut = newReservation.ActualCheckOut;
				reservation.CheckIn = newReservation.CheckIn;
				reservation.CheckOut = newReservation.CheckOut;
				reservation.GuestName = newReservation.GuestName;
				reservation.IsSynchronizedFromRcc = true;
				reservation.LastTimeModifiedBySynchronization = this._synchronizationTime;
				reservation.NumberOfAdults = newReservation.NumberOfAdults;
				reservation.NumberOfChildren = newReservation.NumberOfChildren;
				reservation.NumberOfInfants = newReservation.NumberOfInfants;
				reservation.PMSBedName = newReservation.PMSBedName;
				reservation.PmsNote = newReservation.PmsNote;
				reservation.PMSRoomName = newReservation.PMSRoomName;
				reservation.RccReservationStatusKey = newReservation.RccReservationStatusKey;
				reservation.RoomBedId = newReservation.RoomBedId;
				reservation.RoomId = newReservation.RoomId;
				reservation.RoomName = newReservation.RoomName;
				reservation.Vip = newReservation.Vip;
				reservation.IsActiveToday = newReservation.IsActiveToday;
				reservation.ReservationStatusKey = newReservation.ReservationStatusKey;
				reservation.ReservationStatusDescription = newReservation.ReservationStatusDescription;
				reservation.BedName = newReservation.BedName;
				reservation.Group = newReservation.Group;
				reservation.IsActive = newReservation.IsActive;
				reservation.OtherProperties = new Domain.Entities.ReservationOtherProperty[0];

				if (!reservation.SynchronizedAt.HasValue)
				{
					reservation.SynchronizedAt = this._synchronizationTime;
				}

				if (reservationFieldChangedMessage.IsNotNull())
				{
					changeMessage.Message = (changeMessage.Message.IsNotNull() ? changeMessage.Message : "") + (reservationFieldChangedMessage.IsNotNull() ? (" " + reservationFieldChangedMessage) : "");
				}

				return new ReservationChange
				{
					DoInsertReservation = false,
					DoUpdateReservation = true,
					Message = changeMessage,
					Reservation = reservation,
				};
			}
			else
			{
				return new ReservationChange
				{
					DoInsertReservation = false,
					DoUpdateReservation = false,
					Message = changeMessage,
					Reservation = reservation,
				};
			}
		}

		private string _DidReservationDataChange(Domain.Entities.Reservation reservation, Domain.Entities.Reservation rccReservation)
		{
			var messages = new List<string>();

			if (reservation.ActualCheckIn != rccReservation.ActualCheckIn) 
			{
				messages.Add($"actual checkin date [{(reservation.ActualCheckIn.HasValue ? reservation.ActualCheckIn.Value.ToString("yyyy-MM-dd HH:mm") : "NULL")} -> {(rccReservation.ActualCheckIn.HasValue ? rccReservation.ActualCheckIn.Value.ToString("yyyy-MM-dd HH:mm") : "NULL")}]");
			}
			if (reservation.ActualCheckOut != rccReservation.ActualCheckOut) 
			{
				messages.Add($"actual checkout date [{(reservation.ActualCheckOut.HasValue ? reservation.ActualCheckOut.Value.ToString("yyyy-MM-dd HH:mm") : "NULL")} -> {(rccReservation.ActualCheckOut.HasValue ? rccReservation.ActualCheckOut.Value.ToString("yyyy-MM-dd HH:mm") : "NULL")}]");
			}
			if (reservation.CheckIn != rccReservation.CheckIn) 
			{
				messages.Add($"checkin date [{(reservation.CheckIn.HasValue ? reservation.CheckIn.Value.ToString("yyyy-MM-dd HH:mm") : "NULL")} -> {(rccReservation.CheckIn.HasValue ? rccReservation.CheckIn.Value.ToString("yyyy-MM-dd HH:mm") : "NULL")}]");
			}
			if (reservation.CheckOut != rccReservation.CheckOut) 
			{
				messages.Add($"check out date [{(reservation.CheckOut.HasValue ? reservation.CheckOut.Value.ToString("yyyy-MM-dd HH:mm") : "NULL")} -> {(rccReservation.CheckOut.HasValue ? rccReservation.CheckOut.Value.ToString("yyyy-MM-dd HH:mm") : "NULL")}]");
			}
			if (reservation.GuestName != rccReservation.GuestName) 
			{
				messages.Add($"guest name [{reservation.GuestName ?? "NULL"} -> {rccReservation.GuestName ?? "NULL"}]");
			}
			if (reservation.NumberOfAdults != rccReservation.NumberOfAdults) 
			{
				messages.Add("number of adults");
			}
			if (reservation.NumberOfChildren != rccReservation.NumberOfChildren) 
			{
				messages.Add("number of children");
			}
			if (reservation.NumberOfInfants != rccReservation.NumberOfInfants) 
			{
				messages.Add("number of infants");
			}
			if (reservation.PMSBedName != rccReservation.PMSBedName) 
			{
				messages.Add($"PMS bed name [{reservation.PMSBedName ?? "NULL"} -> {rccReservation.PMSBedName ?? "NULL"}]");
			}
			if (reservation.PmsNote != rccReservation.PmsNote) 
			{
				messages.Add("PMS note");
			}
			if (reservation.PMSRoomName != rccReservation.PMSRoomName) 
			{
				messages.Add($"PMS room name [{reservation.PMSRoomName ?? "NULL"} -> {rccReservation.PMSRoomName ?? "NULL"}]");
			}
			if (reservation.RccReservationStatusKey != rccReservation.RccReservationStatusKey) 
			{
				messages.Add($"status [{reservation.RccReservationStatusKey ?? "NULL"} -> {rccReservation.RccReservationStatusKey ?? "NULL"}]");
			}
			if (reservation.RoomBedId != rccReservation.RoomBedId) 
			{
				messages.Add("bed");
			}
			if (reservation.RoomId != rccReservation.RoomId) 
			{
				messages.Add("room");
			}
			if (reservation.RoomName != rccReservation.RoomName) 
			{
				messages.Add($"room name [{reservation.RoomName ?? "NULL"} -> {rccReservation.RoomName ?? "NULL"}]");
			}
			if (reservation.Vip != rccReservation.Vip) 
			{
				messages.Add("vip");
			}
			if (reservation.IsActiveToday != rccReservation.IsActiveToday) 
			{
				messages.Add($"is active today [{reservation.IsActiveToday} -> {rccReservation.IsActiveToday}]");
			}
			if (reservation.ReservationStatusKey != rccReservation.ReservationStatusKey) 
			{
				messages.Add($"status key [{reservation.ReservationStatusKey ?? "NULL"} -> {rccReservation.ReservationStatusKey ?? "NULL"}]");
			}
			if (reservation.ReservationStatusDescription != rccReservation.ReservationStatusDescription) 
			{
				messages.Add($"status description [{reservation.ReservationStatusDescription ?? "NULL"} -> {rccReservation.ReservationStatusDescription ?? "NULL"}]");
			}
			if (reservation.BedName != rccReservation.BedName) 
			{
				messages.Add($"bed name [{reservation.BedName ?? "NULL"} -> {rccReservation.BedName ?? "NULL"}]");
			}
			if (reservation.Group != rccReservation.Group) 
			{
				messages.Add("group");
			}
			if (reservation.IsActive != rccReservation.IsActive)
			{
				messages.Add($"is active [{reservation.IsActive} -> {rccReservation.IsActive}]");
			}

			if (!messages.Any())
				return null;

			return "Reservation changes: " + string.Join(", ", messages) + ".";
		}

		/////////////////////////////// DEPRECATED! OccupiedById doesn't exist any more.
		/////////////////////////////// DEPRECATED! OccupiedById doesn't exist any more.
		/////////////////////////////// DEPRECATED! OccupiedById doesn't exist any more.
		/////////////////////////////// DEPRECATED! OccupiedById doesn't exist any more.
		/////////////////////////////// DEPRECATED! OccupiedById doesn't exist any more.
		/////////////////////////////// DEPRECATED! OccupiedById doesn't exist any more.
		/////////////////////////////// DEPRECATED! OccupiedById doesn't exist any more.
		// REQUIRED TESTS:
		// 1. REQUIRED DATA
		//	- 1 hotel: H1
		//  - 2 buildings: B1, B2
		//  
		//	- rooms in B1: 
		//		RoomId	ExternalId	IsOccupied	OccupiedById	CreatedAt 
		//		101		101			false		null			2020-01-01
		//		102A	102			false		null			2020-01-01
		//		103		null		false		null			2020-01-01
		//		104		104			false		null			2020-01-01
		//		105		null		false		null			2020-01-01
		//		106		106			true		R-9000			2020-01-01
		//		107		107			true		R-9001			2020-01-01
		//		108		108			true		R-9002			2020-01-01
		//		109		109			true		R-9003			2020-01-01
		//		110		110			true		R-9004			2020-01-01
		//		111		111			true		R-9005			2020-01-01
		//
		//  - rooms in B2: 
		//		RoomId	ExternalId	IsOccupied	OccupiedById	CreatedAt 
		//		101		null		false		null			2020-02-01
		//		102		null		false		null			2020-02-01
		//		103		null		false		null			2020-02-01
		//  
		//  - existing reservations: R-9000, R-9001, R-9002, R-9003, R-9004
		//
		// 2. Tests: (Room is linked if it has ExternalId set)
		//	A. Test a request with a reservation 'R-4000' to a non existing room 'X100' - test non existing reservation room;
		//	B. Test a request with a reservation 'R-4001' to a vacant existing non-linked room '105' - test match room by name;
		//	C. Test a request with a reservation 'R-4002' to a vacant existing linked room '104' - test match room by external id;
		//	D. Test a request with a reservation 'R-4003' to a vacant existing linked room '101' (B1) with non-linked duplicate '101' (B2) - test handle non-linked duplicate room match; 
		//	E. Test a request with a reservation 'R-4004' to a vacant existing non-linked room '103' (B1) with non-linked duplicate '103' (B2) - test choosing non-linked duplicates;
		//	F. Test requests queue - behavior test - the room 102 exists on 1., is updated on 2. and doesn't exist anymore on 3. so it must be newly created because 2. will updated the ExternalId:
		//		1. 'R-4005' to 102
		//		2. 'R-4006' to 102A
		//		3. 'R-4007' to 102
		//	G. Test requests queue - behavior test - circular reservation movements:
		//		1. 'R-9000' to 107
		//		2. 'R-9001' to 108
		//		2. 'R-9002' to 106
		//		
		//	H. Test requests queue - behavior test - last occupant is not given new room:
		//		1. 'R-9003' to 110
		//		
		//	I. Test requests queue - behavior test - move to an unoccupied room:
		//		1. 'R-9005' to 105


	}
}
