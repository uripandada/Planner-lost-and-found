using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
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

namespace Planner.Application.MobileApi.Cleanings.Commands.UpdateCleaningStatus
{
	public class ExtendedMobileRoomDetails: MobileRoomDetails
	{
		public MobileRoomHousekeepingStatus HousekeepingStatus { get; set; }
		public MobileRoomStatus RoomStatus { get; set; }
	}

	public class UpdateCleaningStatusCommand: IRequest<ExtendedMobileRoomDetails>
	{
		public Guid CleaningId { get; set; }
		public string Status { get; set; }
		public long TimeStamp { get; set; }
	}

	public class UpdateCleaningStatusCommandHandler : IRequestHandler<UpdateCleaningStatusCommand, ExtendedMobileRoomDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;
		private readonly ISystemEventsService _eventsService;

		public UpdateCleaningStatusCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, ISystemEventsService eventsService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HttpContext.User.HotelGroupId();
			this._eventsService = eventsService;
		}

		public async Task<ExtendedMobileRoomDetails> Handle(UpdateCleaningStatusCommand request, CancellationToken cancellationToken)
		{
			var cleaning = await this._databaseContext.Cleanings.Include(cpi => cpi.Cleaner).Include(cpi => cpi.CleaningPlanItems).FirstOrDefaultAsync(cpi => cpi.Id == request.CleaningId);
			if (cleaning == null) throw new Exception("Cleaning not found during cleaning status update.");

			var oldCleaningStatus = cleaning.Status;
			var roomData = await this._LoadMobileRoomDetails(cleaning.RoomId);
			var roomDetails = roomData.MobileRoomDetails;
			var roomEntity = roomData.Room;

			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, roomDetails.HotelId, true);

			var updateDndStatus = false;
			var isDnd = false;
			var isDndRetry = false;
			switch (request.Status.ToLower())
			{
				case "cleaning":
					// Cleaning started
					// Cleaning restarted
					// Cleaning unpaused
					cleaning.Status = CleaningProcessStatus.IN_PROGRESS;
					roomEntity.IsCleaningInProgress = true;

					if (roomEntity.IsDoNotDisturb)
					{
						updateDndStatus = true;
						isDnd = false;
					}
					break;
				case "paused":
					// Cleaning paused
					cleaning.Status = CleaningProcessStatus.PAUSED;
					roomEntity.IsCleaningInProgress = true;
					break;
				case "finish":
					// Cleaning finished
					// Inspection finished
					if (cleaning.Status == CleaningProcessStatus.INSPECTION_STARTED)
					{
						cleaning.Status = CleaningProcessStatus.INSPECTION_FINISHED;
						cleaning.InspectedById = this._userId;
						cleaning.IsInspected = true;
						cleaning.IsInspectionSuccess = true;
						cleaning.IsReadyForInspection = false;
						roomEntity.IsCleaningInProgress = false;
						roomEntity.IsClean = true;
					}
					else
					{
						cleaning.Status = CleaningProcessStatus.FINISHED;
						cleaning.IsReadyForInspection = cleaning.IsInspectionRequired; // if inspection is required, set the ready for inspection flag to false
						roomEntity.IsCleaningInProgress = false;

						//if (!cleaning.IsInspectionRequired)
						//{
							roomEntity.IsClean = true;
						//}
					}
					break;
				case "delay":
					cleaning.Status = CleaningProcessStatus.DELAYED;
					roomEntity.IsCleaningInProgress = false;
					break;
				case "confirm-dnd": // Confirm dnd can be sent multiple times after initial dnd is sent.
					cleaning.Status = CleaningProcessStatus.DO_NOT_DISTURB;
					roomEntity.IsCleaningInProgress = false;

					// if the DND flag is not set, set it straight away
					if (!roomEntity.IsDoNotDisturb)
					{
						updateDndStatus = true;
						isDnd = true;
					}
					isDndRetry = true;
					break;
				case "dnd": // dnd status is sent first time when the dnd is spotted
					cleaning.Status = CleaningProcessStatus.DO_NOT_DISTURB;
					roomEntity.IsCleaningInProgress = false;

					if (!roomEntity.IsDoNotDisturb)
					{
						updateDndStatus = true;
						isDnd = true;
					}
					else
					{
						// log it as retry if the room is already DND
						isDndRetry = true;
					}
					break;
				case "refuse":
					cleaning.Status = CleaningProcessStatus.REFUSED;
					roomEntity.IsCleaningInProgress = false;
					break;
				case "cancel-dnd":
					cleaning.Status = CleaningProcessStatus.NEW;
					roomEntity.IsCleaningInProgress = false;
					updateDndStatus = true;
					isDnd = false;
					break;
				case "cancel-refuse":
					cleaning.Status = CleaningProcessStatus.NEW;
					roomEntity.IsCleaningInProgress = false;
					break;
				case "cancel-finish":
					cleaning.Status = CleaningProcessStatus.NEW;

					roomEntity.IsCleaningInProgress = false;
					roomEntity.IsClean = false;
					break;
				case "voucher":
					// 
					break;
				case "no-check":
					// 
					break;
				case "":
					// Room cancel? 
					break;
				case "cleaning-started": // TO REPLACE 'cleaning'
					if (oldCleaningStatus == CleaningProcessStatus.NEW || oldCleaningStatus == CleaningProcessStatus.SEEN_NEW)
					{
						cleaning.Status = CleaningProcessStatus.IN_PROGRESS;
						roomEntity.IsCleaningInProgress = true;
					}
					else
					{
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
					}
					break;
				case "cleaning-restarted": // TO REPLACE 'cleaning'
					if (oldCleaningStatus == CleaningProcessStatus.INSPECTION_FAILED || oldCleaningStatus == CleaningProcessStatus.SEEN_INSPECTION_FAILED)
					{
						cleaning.Status = CleaningProcessStatus.IN_PROGRESS;
						roomEntity.IsCleaningInProgress = true;
					}
					else
					{
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
					}
					break;
				case "cleaning-unpaused": // TO REPLACE 'cleaning'
					if (oldCleaningStatus == CleaningProcessStatus.PAUSED)
					{
						cleaning.Status = CleaningProcessStatus.IN_PROGRESS;
						roomEntity.IsCleaningInProgress = true;
					}
					else
					{
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
					}
					break;
				case "cleaning-finished": // TO REPLACE 'finish'
					if (oldCleaningStatus == CleaningProcessStatus.IN_PROGRESS || oldCleaningStatus == CleaningProcessStatus.NEW || oldCleaningStatus == CleaningProcessStatus.SEEN_NEW)
					{
						cleaning.Status = CleaningProcessStatus.FINISHED;
						cleaning.IsReadyForInspection = cleaning.IsInspectionRequired; // if inspection is required, set the ready for inspection flag to false
						roomEntity.IsCleaningInProgress = false;

						if (!cleaning.IsInspectionRequired)
						{
							roomEntity.IsClean = true;
						}
					}
					else
					{
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
					}
					break;
				case "inspection-finished": // TO REPLACE 'finish'
					if (oldCleaningStatus == CleaningProcessStatus.INSPECTION_STARTED || oldCleaningStatus == CleaningProcessStatus.REQUIRES_INSPECTION)
					{
						cleaning.Status = CleaningProcessStatus.INSPECTION_FINISHED;
						cleaning.InspectedById = this._userId;
						cleaning.IsInspected = true;
						cleaning.IsInspectionSuccess = true;
						cleaning.IsReadyForInspection = false;
						roomEntity.IsCleaningInProgress = false;
						roomEntity.IsClean = true;
					}
					else
					{
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
						// THIS SHOULD NOT HAPPEN BUT IT WILL!
					}
					break;
				default:
					throw new NotSupportedException("The status change is not supported");
			}


			if(oldCleaningStatus != cleaning.Status || updateDndStatus || isDndRetry || request.Status.ToLower() == "cancel-finish")
			{
				roomEntity.ModifiedAt = dateTime;
				roomEntity.ModifiedById = this._userId;
				roomEntity.RccHousekeepingStatus = roomData.RccHousekeepingStatusCode;
				roomEntity.RccRoomStatus = roomData.RccRoomStatusCode;

				if (updateDndStatus)
				{
					roomEntity.IsDoNotDisturb = isDnd;
				}

				var newHousekeepingDetails = roomEntity.CalculateCurrentHousekeepingStatus();
				roomDetails.HousekeepingStatus.Code = newHousekeepingDetails.RccHousekeepingStatusCode.ToString();
				roomDetails.HousekeepingStatus.Label = newHousekeepingDetails.Description;
				roomDetails.HousekeepingStatus.Color = newHousekeepingDetails.RccHousekeepingStatusCode.ToHexColor();

				roomEntity.RccHousekeepingStatus = newHousekeepingDetails.RccHousekeepingStatusCode;

				await this._databaseContext.SaveChangesAsync(cancellationToken);

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

				switch (cleaning.Status)
				{
					case CleaningProcessStatus.FINISHED:
						await this._eventsService.CleaningFinished(cleaningChangedEventData);
						break;
					case CleaningProcessStatus.INSPECTION_FINISHED:
						await this._eventsService.CleaningInspectionFinished(cleaningChangedEventData, null);
						break;
					case CleaningProcessStatus.IN_PROGRESS:
						if(oldCleaningStatus == CleaningProcessStatus.PAUSED)
						{
							await this._eventsService.CleaningUnpaused(cleaningChangedEventData);
						}
						else
						{
							await this._eventsService.CleaningStarted(cleaningChangedEventData);
						}
						break;
					case CleaningProcessStatus.REFUSED:
						await this._eventsService.CleaningRefused(cleaningChangedEventData);
						break;
					case CleaningProcessStatus.DO_NOT_DISTURB:
						await this._eventsService.CleaningDoNotDisturb(cleaningChangedEventData);
						break;
					case CleaningProcessStatus.DELAYED:
						await this._eventsService.CleaningDelayed(cleaningChangedEventData);
						break;
					case CleaningProcessStatus.PAUSED:
						await this._eventsService.CleaningPaused(cleaningChangedEventData);
						break;
					case CleaningProcessStatus.NEW:
						await this._eventsService.CleaningNew(cleaningChangedEventData);
						break;
				}

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
