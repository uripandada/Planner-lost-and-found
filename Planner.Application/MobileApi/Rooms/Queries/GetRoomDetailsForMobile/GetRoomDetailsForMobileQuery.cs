using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Rooms.Queries.GetRoomDetailsForMobile
{
	public class MobileRoomDetails
	{
		public string Name { get; set; } = "NULL room"; // name: { type: String },
		public string PmsName { get; set; } = null; // pmsName: { type: String, default: null },
		public string Description { get; set; } = null; // description: String,
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true },
		public Guid? RoomAccessId { get; set; } = null; // roomAccess: { type: ObjectId, ref: 'RoomAccess' },
		public Guid? RoomCategoryId { get; set; } = null; // roomCategory: { type: ObjectId, ref: 'RoomCategory' },
		public Guid? RoomStatusId { get; set; } = null; // roomStatus: { type: ObjectId, ref: 'RoomStatus' },
		public Guid? RoomMaintenanceId { get; set; } = null; // roomMaintenance: { type: ObjectId, ref: 'RoomMaintenance' },
		public Guid? RoomHousekeepingId { get; set; } = null; // roomHousekeeping: { type: ObjectId, ref: 'RoomHousekeeping' },
		public string Comment { get; set; } = ""; // comment: { type: String, default: "" },
		public Guid? RoomNoteId { get; set; } = null; // roomNote: { type: ObjectId, ref: 'RoomNote' },
		public int OverwriteCredits { get; set; } = -1; // overwriteCredits: { type: Number, default: -1 },
		public DateTime LastDate { get; set; } = DateTime.UtcNow; // lastDate: { type: Date, default: Date.now() },
		public string AttendantStatus { get; set; } = ""; // attendantStatus: { type: String, default: "" },
		public string AttendantStatusNight { get; set; } = ""; // attendantStatusNight: { type: String, default: "" },
		public string TurndownService { get; set; } = ""; // turndownService: { type: String, default: "" },
		public bool IsGuestIn { get; set; } = false; // isGuestIn: { type: Boolean, default: false },
		public bool IsChangeSheets { get; set; } = false; // isChangeSheets: { type: Boolean, default: false },
		public bool IsLongStay { get; set; } = false; // isLongStay: { type: Boolean, default: false },
		public bool IsRoomBlocked { get; set; } = false; // isRoomBlocked: { type: Boolean, default: false },
		public bool IsRoomRestocked { get; set; } = false; // isRoomRestocked: { type: Boolean, default: false },
		public int SortValue { get; set; } = 0; // sortValue: { type: Number, default: 0 },
		public string Section { get; set; } = null; // section: { type: String, default: null },
		public string SubSection { get; set; } = null; // subsection: { type: String, default: null },
		public string[] Tags { get; set; } = new string[0]; // tags:[{ type: String, default: null}],
		public string[] Features { get; set; } = new string[0]; // features:[{ type: String, default: null}],
		public string[] Sublocations { get; set; } = new string[0]; // sublocations:[{ type: String, default: null}],
		public string UpdateUsername { get; set; } = null; // updateUsername: { type: String, default: null },
		public string UpdateType { get; set; } = null;  // updateType: { type: String, default: null },
		public int? UpdateTs { get; set; } = null; // updateTs: { type: Number, default: null },
		public int? UpdateTapTs { get; set; } = null; // updateTapTs: { type: Number, default: null },
		public Guid? ExternalInfoId { get; set; } = null; // externalInfo: { ... },

		/// <summary>
		/// NEW* - entity identifier
		/// </summary>
		public Guid Id { get; set; } = Guid.Empty;

		/// <summary>
		/// NEW* - floor identifier. Set if the room in not temporary.
		/// </summary>
		public Guid? FloorId { get; set; } = null;

		/// <summary>
		/// NEW* - floor name. Set if the room in not temporary.
		/// </summary>
		public string FloorName { get; set; } = null;

		/// <summary>
		/// NEW* - floor number. Set if the room in not temporary.
		/// </summary>
		public int? FloorNumber { get; set; } = null;

		/// <summary>
		/// NEW* - parent building identifier. Set if the room in not temporary.
		/// </summary>
		public Guid? BuildingId { get; set; } = null;

		/// <summary>
		/// NEW* - name of the building the floor is in. Set if the room in not temporary.
		/// </summary>
		public string BuildingName { get; set; } = null;

		/// <summary>
		/// NEW* - is temporary flag.
		/// </summary>
		public bool IsTemporaryRoom { get; set; } = true;

		/// <summary>
		/// HOTEL, HOSTEL, APPARTMENT
		/// </summary>
		public string TypeKey { get; set; }
	}

	public class GetRoomDetailsForMobileQuery: IRequest<MobileRoomDetails>
	{
		public Guid Id { get; set; }
	}

	public class GetRoomDetailsForMobileQueryHandler : IRequestHandler<GetRoomDetailsForMobileQuery, MobileRoomDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetRoomDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileRoomDetails> Handle(GetRoomDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			return await this._LoadMobileRoomDetails(request.Id);

			//var r = await this._databaseContext.Rooms.FindAsync(request.Id);
			//if (r == null) throw new Exception("Unable to find room details.");

			//var d = new Shared.Models.RoomForMobile();

			//return new MobileRoomDetails
			//{
			//	Id = r.Id,
			//	Name = r.Name,
			//	PmsName = r.ExternalId,
			//	BuildingId = r.BuildingId,
			//	BuildingName = r.Building?.Name,
			//	FloorId = r.FloorId,
			//	FloorName = r.Floor?.Name,
			//	FloorNumber = r.Floor?.Number,
			//	HotelId = r.HotelId,
			//	IsGuestIn = r.IsOccupied,
			//	IsTemporaryRoom = !r.FloorId.HasValue || !r.BuildingId.HasValue,
			//	Section = r.FloorSectionName,
			//	SubSection = r.FloorSubSectionName,

			//	AttendantStatus = d.AttendantStatus,
			//	AttendantStatusNight = d.AttendantStatusNight,
			//	Comment = d.Comment,
			//	Description = d.Description,
			//	ExternalInfoId = d.ExternalInfoId,
			//	Features = d.Features,
			//	IsChangeSheets = d.IsChangeSheets,
			//	IsLongStay = d.IsLongStay,
			//	IsRoomBlocked = d.IsRoomBlocked,
			//	IsRoomRestocked = d.IsRoomRestocked,
			//	LastDate = d.LastDate,
			//	OverwriteCredits = d.OverwriteCredits,
			//	RoomAccessId = d.RoomAccessId,
			//	RoomCategoryId = d.RoomCategoryId,
			//	RoomHousekeepingId = d.RoomHousekeepingId,
			//	RoomMaintenanceId = d.RoomMaintenanceId,
			//	RoomNoteId = d.RoomNoteId,
			//	RoomStatusId = d.RoomStatusId,
			//	SortValue = d.SortValue,
			//	Sublocations = d.Sublocations,
			//	Tags = d.Tags,
			//	TurndownService = d.TurndownService,
			//	UpdateTapTs = d.UpdateTapTs,
			//	UpdateTs = d.UpdateTs,
			//	UpdateType = d.UpdateType,
			//	UpdateUsername = d.UpdateUsername,
			//};
		}

		private async Task<MobileRoomDetails> _LoadMobileRoomDetails(Guid roomId)
		{
			var r = await this._databaseContext.Rooms
				.Include(r => r.RoomNotes.Where(rn => !rn.IsArchived).ToArray())
				.FirstOrDefaultAsync(r => r.Id == roomId);
			if (r == null) throw new Exception("Unable to find room details.");

			var d = new Shared.Models.RoomForMobile();

			var details = new MobileRoomDetails
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
				IsGuestIn = r.IsGuestCurrentlyIn,
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

				TypeKey = r.TypeKey,
			};

			//var dateProvider = new HotelLocalDateProvider();
			//var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, r.HotelId, true);
			//var roomHousekeepingDescription = r.CalculateCurrentHousekeepingStatus();
			//var roomStatusDescription = r.CalculateReservationStatusForDate(dateTime);


			return details;
		}
	}
}
