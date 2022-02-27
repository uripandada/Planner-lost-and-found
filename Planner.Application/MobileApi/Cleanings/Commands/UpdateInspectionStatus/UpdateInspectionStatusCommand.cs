using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Cleanings.Commands.UpdateCleaningStatus;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomHousekeepingStatusesForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetListOfRoomStatusesForMobile;
using Planner.Application.MobileApi.Rooms.Queries.GetRoomDetailsForMobile;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Cleanings.Commands.UpdateInspectionStatus
{
	public class UpdateInspectionStatusCommand : IRequest<ExtendedMobileRoomDetails>
	{
		public Guid CleaningId { get; set; }
		public string Status { get; set; }
		public long TimeStamp { get; set; }
	}

	public class UpdateInspectionStatusCommandHandler : IRequestHandler<UpdateInspectionStatusCommand, ExtendedMobileRoomDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;
		private readonly ISystemEventsService _eventsService;

		public UpdateInspectionStatusCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService eventsService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HttpContext.User.HotelGroupId();
			this._eventsService = eventsService;
		}

		public async Task<ExtendedMobileRoomDetails> Handle(UpdateInspectionStatusCommand request, CancellationToken cancellationToken)
		{
			RccHousekeepingStatusCode rccHkStatus;
			if (!Enum.TryParse(request.Status, out rccHkStatus))
			{
				throw new Exception("Unknown enum value");
			}

			var cleaning = await this._databaseContext.Cleanings.Include(cpi => cpi.Cleaner).Include(cpi => cpi.CleaningPlanItems).FirstOrDefaultAsync(cpi => cpi.Id == request.CleaningId);
			if (cleaning == null) throw new Exception("Cleaning not found during cleaning status update.");

			var oldCleaningStatus = cleaning.Status;
			var roomData = await this._LoadMobileRoomDetails(cleaning.RoomId);
			var roomDetails = roomData.MobileRoomDetails;
			var roomEntity = roomData.Room;

			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, roomDetails.HotelId, true);

			var newInspection = new CleaningInspection
			{
				CleaningId = cleaning.Id,
				CreatedById = this._userId,
				StartedAt = dateTime,
				EndedAt = dateTime,
				Id = Guid.NewGuid(),
				IsFinished = true,
				IsSuccess = false,
				Note = "",
			};

			var dndChanged = false;
			var isDnd = false;
			var oooChanged = false;
			var isOOO = false;
			var oosChanged = false;
			var isOOS = false;
			var isCleanChanged = false;
			var isClean = false;
			var isInspectedChanged = false;
			var isInspected = false;
			var isHousekeepingInProgressChanged = false;
			var isHousekeepingInProgress = false;

			var cleaningStatusChanged = false;
			var cleaningStatus = CleaningProcessStatus.UNKNOWN;

			switch (rccHkStatus)
			{
				case RccHousekeepingStatusCode.DNN:
					dndChanged = true;
					isDnd = true;

					cleaningStatusChanged = true;
					cleaningStatus = CleaningProcessStatus.DO_NOT_DISTURB;

					newInspection.Note = "Inspected - room is DND.";
					break;

				case RccHousekeepingStatusCode.HC:
				case RccHousekeepingStatusCode.OHC:
				case RccHousekeepingStatusCode.VHC:
					isCleanChanged = true;
					isClean = true;
					isInspectedChanged = true;
					isInspected = false;
					isHousekeepingInProgressChanged = true;
					isHousekeepingInProgress = false;

					cleaningStatusChanged = true;
					cleaningStatus = CleaningProcessStatus.FINISHED;

					newInspection.Note = "Inspected - room is properly cleaned but not inspected.";
					break;

				case RccHousekeepingStatusCode.HCI:
				case RccHousekeepingStatusCode.OHCI:
				case RccHousekeepingStatusCode.VHCI:
					isCleanChanged = true;
					isClean = true;
					isInspectedChanged = true;
					isInspected = true;
					isHousekeepingInProgressChanged = true;
					isHousekeepingInProgress = false;

					cleaningStatusChanged = true;
					cleaningStatus = CleaningProcessStatus.INSPECTION_FINISHED;

					newInspection.IsSuccess = true;
					newInspection.Note = "Inspected - room is properly cleaned.";
					break;

				case RccHousekeepingStatusCode.HD:
				case RccHousekeepingStatusCode.VHD:
				case RccHousekeepingStatusCode.OHD:
					isCleanChanged = true;
					isClean = false;
					isInspectedChanged = true;
					isInspected = false;
					isHousekeepingInProgressChanged = true;
					isHousekeepingInProgress = false;

					cleaningStatusChanged = true;
					cleaningStatus = CleaningProcessStatus.NEW;

					newInspection.Note = "Inspected - room is dirty.";
					break;

				case RccHousekeepingStatusCode.HP:
				case RccHousekeepingStatusCode.OHP:
				case RccHousekeepingStatusCode.VHP:
					isCleanChanged = true;
					isClean = false;
					isInspectedChanged = true;
					isInspected = false;
					isHousekeepingInProgressChanged = true;
					isHousekeepingInProgress = true;

					cleaningStatusChanged = true;
					cleaningStatus = CleaningProcessStatus.IN_PROGRESS;

					newInspection.Note = "Inspected - cleaning is in progress.";
					break;
				
				case RccHousekeepingStatusCode.OOO:
					oooChanged = true;
					isOOO = true;

					newInspection.Note = "Inspected - room is out of order.";
					break;
				
				case RccHousekeepingStatusCode.OOS:
					oosChanged = true;
					isOOS = true;

					newInspection.Note = "Inspected - room is out of service.";
					break;
				
				case RccHousekeepingStatusCode.LUG:

					newInspection.Note = "Inspected - luggage.";
					break;

				case RccHousekeepingStatusCode.NO:
					newInspection.Note = "Inspected - no change.";
					break;
				
				case RccHousekeepingStatusCode.PU:
					newInspection.Note = "Inspected - pick up.";
					break;
				
				case RccHousekeepingStatusCode.TD:
					newInspection.Note = "Inspected - turn down.";
					break;

				case RccHousekeepingStatusCode.TDNR:
					newInspection.Note = "Inspected - turn down not request / turn down done.";
					break;

				case RccHousekeepingStatusCode.TDR:
					newInspection.Note = "Inspected - turn down requests.";
					break;

				default:
					throw new NotSupportedException("The status change is not supported");
			}

			var updateRoom = false;
			var updateCleaning = false;

			if(!roomEntity.RccHousekeepingStatus.HasValue || roomEntity.RccHousekeepingStatus.Value != rccHkStatus)
			{
				updateRoom = true;
				roomEntity.RccHousekeepingStatus = rccHkStatus;
			}

			if (oooChanged)
			{
				if (roomEntity.IsOutOfOrder != isOOO)
				{
					updateRoom = true;
					roomEntity.IsOutOfOrder = isOOO;
				}
			}

			if (oosChanged)
			{
				if(roomEntity.IsOutOfService != isOOS)
				{
					updateRoom = true;
					roomEntity.IsOutOfService = isOOS;

				}
			}

			if (dndChanged)
			{
				if(roomEntity.IsDoNotDisturb != isDnd)
				{
					updateRoom = true;
					roomEntity.IsDoNotDisturb = isDnd;

				}
			}

			if (isCleanChanged)
			{
				if(roomEntity.IsClean != isClean)
				{
					updateRoom = true;
					roomEntity.IsClean = isClean;

				}
			}

			if (isHousekeepingInProgressChanged)
			{
				if(roomEntity.IsCleaningInProgress != isHousekeepingInProgress)
				{
					updateRoom = true;
					roomEntity.IsCleaningInProgress = isHousekeepingInProgress;
				}
			}


			if (isInspectedChanged)
			{
				if(roomEntity.IsInspected != isInspected)
				{
					updateRoom = true;
					roomEntity.IsInspected = isInspected;
				}

				if(cleaning.IsInspected != isInspected)
				{
					updateRoom=true;
					updateCleaning = true;
					cleaningStatus = CleaningProcessStatus.INSPECTION_FINISHED;
					cleaningStatusChanged = true;
				}
			}

			var updateCleaningStatus = false;
			var isDndRetry = false;
			if (cleaningStatusChanged)
			{
				updateRoom = true;
				
				if(cleaning.Status != cleaningStatus)
				{
					cleaning.Status = cleaningStatus;
					updateCleaningStatus = true;
				}
				else if(cleaning.Status == CleaningProcessStatus.DO_NOT_DISTURB)
				{
					isDndRetry = true;
					updateCleaningStatus = true;
				}
			}

			if (updateRoom)
			{
				roomEntity.ModifiedAt = dateTime;
				roomEntity.ModifiedById = this._userId;

				if (updateCleaning)
				{
					cleaning.Status = cleaningStatus;
					cleaning.InspectedById = this._userId;
					cleaning.IsInspected = true;
					cleaning.IsInspectionSuccess = true;
					cleaning.IsReadyForInspection = false;
				}

				await this._databaseContext.CleaningInspections.AddAsync(newInspection);
				await this._databaseContext.SaveChangesAsync(cancellationToken);

				if (updateCleaningStatus)
				{
					var cleaningChangedEventData = new CleaningChangedEventData
					{
						At = dateTime,
						HotelGroupId = this._hotelGroupId,
						RoomId = cleaning.RoomId,
						Status = cleaning.Status,
						UserId = this._userId,
						CleaningPlanId = cleaning.CleaningPlanId,
						CleaningId = cleaning.Id,
						IsDndRetry = isDndRetry,
						BedId = cleaning.RoomBedId,
						IsRemovedFromSentPlan = false,
						CleaningPlanItemId = cleaning.CleaningPlanItems != null && cleaning.CleaningPlanItems.Any() ? cleaning.CleaningPlanItems.First().Id : Guid.Empty,
					};

					switch (cleaningStatus)
					{
						case CleaningProcessStatus.DO_NOT_DISTURB:
							await this._eventsService.CleaningInspectionDoNotDisturb(cleaningChangedEventData, newInspection.Note);
							break;

						case CleaningProcessStatus.REQUIRES_INSPECTION:
							await this._eventsService.CleaningInspectionRequiresInspection(cleaningChangedEventData, newInspection.Note);
							break;
							
						case CleaningProcessStatus.INSPECTION_FINISHED:
							await this._eventsService.CleaningInspectionFinished(cleaningChangedEventData, newInspection.Note);
							break;

						case CleaningProcessStatus.IN_PROGRESS:
							await this._eventsService.CleaningInspectionCleaningInProgress(cleaningChangedEventData, newInspection.Note);
							break;

						case CleaningProcessStatus.NEW:
							await this._eventsService.CleaningInspectionNewCleaning(cleaningChangedEventData, newInspection.Note);
							break;
					}
				}
				else
				{
					var cleaningPlanItemId = cleaning.CleaningPlanItems != null && cleaning.CleaningPlanItems.Any() ? cleaning.CleaningPlanItems.First().Id : Guid.Empty;
					await this._eventsService.CleaningInspectionChanged(this._hotelGroupId, this._userId, cleaning.RoomId, cleaning.RoomBedId, cleaning.Id, cleaning.CleaningPlanId, cleaningPlanItemId, dateTime, newInspection.Note, cleaningStatus);
				}

				var newHousekeepingDetails = roomEntity.CalculateCurrentHousekeepingStatus();
				roomDetails.HousekeepingStatus.Code = newHousekeepingDetails.RccHousekeepingStatusCode.ToString();
				roomDetails.HousekeepingStatus.Label = newHousekeepingDetails.Description;
				roomDetails.HousekeepingStatus.Color = newHousekeepingDetails.RccHousekeepingStatusCode.ToHexColor();
			}

			return roomDetails;
		}

		private class RoomData
		{
			public RccHousekeepingStatusCode RccHousekeepingStatusCode { get; set; }
			public RccRoomStatusCode RccRoomStatusCode { get; set; }
			public Room Room { get; set; }
			public ExtendedMobileRoomDetails MobileRoomDetails { get; set; }
		}

		private async Task<RoomData> _LoadMobileRoomDetails(Guid roomId)
		{
			var r = await this._databaseContext.Rooms
				.Include(r => r.RoomNotes.Where(rn => !rn.IsArchived))
				.Include(r => r.Reservations)
				.FirstOrDefaultAsync(r => r.Id == roomId);

			if (r == null) throw new Exception("Unable to find room details.");

			var d = new Shared.Models.RoomForMobile();

			var details = new ExtendedMobileRoomDetails
			{
				Id = r.Id,
				Name = r.Name,
				PmsName = r.ExternalId,
				BuildingId = r.BuildingId,
				BuildingName = r.Building?.Name,
				FloorId = r.FloorId,
				FloorName = r.Floor?.Name,
				FloorNumber = r.Floor?.Number,
				HotelId = r.HotelId,
				IsGuestIn = r.IsOccupied,
				IsTemporaryRoom = !r.FloorId.HasValue || !r.BuildingId.HasValue,
				Section = r.FloorSectionName,
				SubSection = r.FloorSubSectionName,
				RoomCategoryId = r.CategoryId,
				RoomHousekeepingId = r.Id,
				RoomStatusId = r.Id,
				RoomNoteId = r.RoomNotes != null && r.RoomNotes.Any() ? r.RoomNotes.First().Id : null,

				AttendantStatus = d.AttendantStatus,
				AttendantStatusNight = d.AttendantStatusNight,
				Comment = d.Comment,
				Description = d.Description,
				ExternalInfoId = d.ExternalInfoId,
				Features = d.Features,
				IsChangeSheets = d.IsChangeSheets,
				IsLongStay = d.IsLongStay,
				IsRoomBlocked = d.IsRoomBlocked,
				IsRoomRestocked = d.IsRoomRestocked,
				LastDate = d.LastDate,
				OverwriteCredits = d.OverwriteCredits,
				RoomAccessId = d.RoomAccessId,
				RoomMaintenanceId = d.RoomMaintenanceId,
				SortValue = d.SortValue,
				Sublocations = d.Sublocations,
				Tags = d.Tags,
				TurndownService = d.TurndownService,
				UpdateTapTs = d.UpdateTapTs,
				UpdateTs = d.UpdateTs,
				UpdateType = d.UpdateType,
				UpdateUsername = d.UpdateUsername,
			};

			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, r.HotelId, true);
			var roomHousekeepingDescription = r.CalculateCurrentHousekeepingStatus();
			var roomStatusDescription = r.CalculateReservationStatusForDate(dateTime, r.Reservations);

			details.HousekeepingStatus = new MobileRoomHousekeepingStatus
			{
				Code = roomHousekeepingDescription.RccHousekeepingStatusCode.ToString(),
				Color = "000000",
				HotelId = r.HotelId,
				Id = r.Id,
				Label = roomHousekeepingDescription.RccHousekeepingStatus.ToString(),
				RoomId = r.Id,
			};

			details.RoomStatus = new MobileRoomStatus
			{
				Code = roomStatusDescription.RccRoomStatusCode.ToString(),
				Color = "000000",
				HotelId = r.HotelId,
				Id = r.Id,
				Label = roomStatusDescription.RccRoomStatus.ToString(),
				RoomId = r.Id,
			};

			return new RoomData
			{
				Room = r,
				MobileRoomDetails = details,
				RccHousekeepingStatusCode = roomHousekeepingDescription.RccHousekeepingStatusCode,
				RccRoomStatusCode = roomStatusDescription.RccRoomStatusCode,
			};
		}
	}
}
