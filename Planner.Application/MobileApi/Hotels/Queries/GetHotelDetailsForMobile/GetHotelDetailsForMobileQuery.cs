using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
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

namespace Planner.Application.MobileApi.Hotels.Queries.GetHotelDetailsForMobile
{
	public class MobileHotelModuleDetails
	{
		public bool IsEnablePlanner { get; set; } = false; // "isEnablePlanner": false,
		public bool IsEnableCiCo { get; set; } = false; // "isEnableCiCo": false,
		public bool IsRunnerDisabled { get; set; } = false; // "isRunnerDisabled": false,
		public bool IsPreventativeTasksDisabled { get; set; } = false; // "isPreventativeTasksDisabled": false,
		public bool IsTurndownDisabled { get; set; } = false; // "isTurndownDisabled": false,
		public bool IsInventoryDisabled { get; set; } = false; // "isInventoryDisabled": false,
		public bool IsExperiencesDisabled { get; set; } = false; // "isExperiencesDisabled": false,
		public bool IsAuditsDisabled { get; set; } = false; // "isAuditsDisabled": false
	}

	public class MobileHotelDetails
	{
		public string Id { get; set; } = Guid.Empty.ToString();   // "_id": "55d06baa99295b3a52000000",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string City { get; set; } = ""; // "city": "Oostkamp",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string Country { get; set; } = ""; // "country": "België",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// The name of the property is plural but it seems that the old code set only 1 url to this prop.
		/// </summary>
		public string Images { get; set; } = null; // "images": "https://www.filepicker.io/api/file/VHFvv5mT32Q5iP2segnZ",
		public string Name { get; set; } = "NULL hotel"; // "name": "Hotel Royal",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string Phone { get; set; } = ""; // "phone": "1-418-692-2777",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string Remarks { get; set; } = ""; // "remarks": "",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string State { get; set; } = ""; // "state": "WVL",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string Street { get; set; } = ""; // "street": "Legeweg 157-O",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string Thumbnail { get; set; } = ""; // "thumbnail": "https://www.filepicker.io/api/file/3gk4IjvcR0CgJBYbXPwd",
		/// <summary>
		/// DEPRECATED. Current value set to hotel group key. Hotels don't have usernames any more.
		/// </summary>
		public string Username { get; set; } = "N/A"; // "username": "rcfr",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string Zip { get; set; } = ""; // "zip": "8020",
		/// <summary>
		/// Currently not supported. All properties are always false.
		/// </summary>
		public MobileHotelModuleDetails Modules { get; set; } = null; // "modules": { ... }
		public bool IsHideCleaningData { get; set; } = false; // "isHideCleaningData": false,
		public bool IsLimitAttendantContainers { get; set; } = true; // "isLimitAttendantContainers": true,
		public bool IsRemoveMessagesDaily { get; set; } = true; // "isRemoveMessagesDaily": true,
		public bool IsRenotifyPN { get; set; } = false; // "isRenotifyPN": false,
		public bool IsWebAttendantIcons { get; set; } = false; // "isWebAttendantIcons": false,
		public bool IsAdminOnlyAssetCreate { get; set; } = false; // "isAdminOnlyAssetCreate": false,
		public bool IsRemovePriorityOnFinish { get; set; } = true; // "isRemovePriorityOnFinish": true,
		public bool IsShowAllTaskCounts { get; set; } = false; // "isShowAllTaskCounts": false,
		public bool IsInspectionRemovesPriority { get; set; } = false; // "isInspectionRemovesPriority": false,
		public bool IsAttendantDisablePMSNotes { get; set; } = false; // "isAttendantDisablePMSNotes": false,
		public bool IsAttendantTaskNotes { get; set; } = false; // "isAttendantTaskNotes": false,
		public string[] EscalationPhones { get; set; } = new string[0]; // "escalationPhones": [ "+33783244401", "+33783244401", "+33783244401" ],
		public string ExplicitOccupiedWorkflow { get; set; } = "OHC"; // "explicitOccupiedWorkflow": "OHC",
		public string ExplicitVacantWorkflow { get; set; } = "VHC"; // "explicitVacantWorkflow": "VHC",
		public bool IsExplicitWorkflow { get; set; } = false; // "isExplicitWorkflow": false,
		public bool IsAttendantPriorityColor { get; set; } = true; // "isAttendantPriorityColor": true,
		public bool ShouldResetDirty { get; set; } = true; // "shouldResetDirty": true,
		public bool IsHostEnabled { get; set; } = true; // "isHostEnabled": true,
		public bool IsRestockEnabled { get; set; } = false; // "isRestockEnabled": false,
		public bool IsAttendantImmediateTasks { get; set; } = true; // "isAttendantImmediateTasks": true,
		public bool IsAttendantOneStep { get; set; } = false; // "isAttendantOneStep": false,
		public bool IsAttendantNotificationPN { get; set; } = false; // "isAttendantNotificationPN": false,
		public bool IsAttendantLitePN { get; set; } = false; // "isAttendantLitePN": false,
		public bool IsAttendantNormalPN { get; set; } = false; // "isAttendantNormalPN": false,
		public bool IsAttendantPriorityPN { get; set; } = false; // "isAttendantPriorityPN": false,
		public bool IsMaintenanceNotificationPN { get; set; } = true; // "isMaintenanceNotificationPN": true,
		public bool IsMaintenanceLitePN { get; set; } = true; // "isMaintenanceLitePN": true,
		public bool IsMaintenanceNormalPN { get; set; } = true; // "isMaintenanceNormalPN": true,
		public bool IsMaintenancePriorityPN { get; set; } = true; // "isMaintenancePriorityPN": true,
		public bool IsDisableDNDPhoto { get; set; } = true; // "isDisableDNDPhoto": true,
		public bool IsDisablePMSNotesDEP { get; set; } = false; // "isDisablePMSNotesDEP": false,
		public bool IsTaskMaintenance { get; set; } = true; // "isTaskMaintenance": true,
		public bool IsRoomNotesAttendantDisabled { get; set; } = false; // "isRoomNotesAttendantDisabled": false,
		public bool IsRoomNotesInspectorDisabled { get; set; } = false; // "isRoomNotesInspectorDisabled": false,
		public bool IsRoomNotesWebDisabled { get; set; } = false; // "isRoomNotesWebDisabled": false,
		public int NightPlanningTime { get; set; } = 21; // "nightPlanningTime": 21,
		public bool IsPlanningNightEnabled { get; set; } = true; // "isPlanningNightEnabled": true,
		public bool IsLFWaitingDisabled { get; set; } = true; // "isLFWaitingDisabled": true,
		public bool IsEnabledMaintenanceUnassigned { get; set; } = false; // "isEnabledMaintenanceUnassigned": false,
		public bool DisableGuestLocator { get; set; } = false; // "disableGuestLocator": false,
		public string AttendantDefaultView { get; set; } = "quantity"; // "attendantDefaultView": "quantity",
		public bool IsAttendantBypass { get; set; } = false; // "isAttendantBypass": false,
		public bool IsAttendantWorkflow { get; set; } = true; // "isAttendantWorkflow": true,
		public int Timezone { get; set; } = 1350; // "timezone": 1350,
		public int AttendantCancelMinutes { get; set; } = 0; // "attendantCancelMinutes": 0,
		public bool DisableAttendantTimer { get; set; } = true; // "disableAttendantTimer": true,
		public bool IsAdvancedLayout { get; set; } = true; // "isAdvancedLayout": true,
		public int DepartureMultiplier { get; set; } = 1; // "departureMultiplier": 1,
		public int StayMultiplier { get; set; } = 1; // "stayMultiplier": 1,
		public int VacantMultiplier { get; set; } = 0; // "vacantMultiplier": 0,
		public int CreditMinutes { get; set; } = 1; // "creditMinutes": 1,
		public bool IsShowGuestName { get; set; } = true; // "isShowGuestName": true,
		public bool IsHideGuestInfo { get; set; } = false; // "isHideGuestInfo": false,
		public bool IsAllNightly { get; set; } = false; // "isAllNightly": false,
		public bool IsOCCNightly { get; set; } = false; // "isOCCNightly": false,
		public string[] Organizations { get; set; } = new string[0]; // "organizations": [],
		public Guid[] Floors { get; set; } = new Guid[0]; // "floors": [ "5507b1dc99295b21d876410b", "5507b1dc99295b21d8764120", "5507b1dc99295b21d8764132" ],
		public Guid[] Users { get; set; } = new Guid[0]; // "users": [ "5507b1dc99295b21d87640e5", "5507b1dc99295b21d87640e6", "5507b1dc99295b21d87640e7" ],
		public string[] Notes { get; set; } = new string[0]; // "notes": [],
		public string RemoteApi { get; set; } = null; // "remoteApi": "",
		/// <summary>
		/// Currently not supported on the backend. Some kind of file URL.
		/// </summary>
		public string Display { get; set; } = null; // "display": "https://cdn.filepicker.io/YwJpYTaRhy1JFyFoORgO"
	}

	public class GetHotelDetailsForMobileQuery: IRequest<MobileHotelDetails>
	{
		public string HotelId { get; set; }
	}

	public class GetHotelDetailsForMobileQueryHandler : IRequestHandler<GetHotelDetailsForMobileQuery, MobileHotelDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetHotelDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileHotelDetails> Handle(GetHotelDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			if (hotel == null) throw new Exception("Unable to find hotel while loading hotel details.");

			// This will create an object with default values
			var d = new Application.MobileApi.Shared.Models.HotelForMobile();

			return new MobileHotelDetails
			{
				Id = hotel.Id,
				Name = hotel.Name,
				AttendantCancelMinutes = d.AttendantCancelMinutes,
				AttendantDefaultView = d.AttendantDefaultView,
				City = d.City,
				Country = d.Country,
				CreditMinutes = d.CreditMinutes,
				DepartureMultiplier = d.DepartureMultiplier,
				DisableAttendantTimer = d.DisableAttendantTimer,
				DisableGuestLocator = d.DisableGuestLocator,
				Display = d.Display,
				EscalationPhones = d.EscalationPhones,
				ExplicitOccupiedWorkflow = d.ExplicitOccupiedWorkflow,
				ExplicitVacantWorkflow = d.ExplicitVacantWorkflow,
				Floors = d.Floors,
				Images = d.Images,
				IsAdminOnlyAssetCreate = d.IsAdminOnlyAssetCreate,
				IsAdvancedLayout = d.IsAdvancedLayout,
				IsAllNightly = d.IsAllNightly,
				IsAttendantBypass = d.IsAttendantBypass,
				IsAttendantDisablePMSNotes = d.IsAttendantDisablePMSNotes,
				IsAttendantImmediateTasks = d.IsAttendantImmediateTasks,
				IsAttendantLitePN = d.IsAttendantLitePN,
				IsAttendantNormalPN = d.IsAttendantNormalPN,
				IsAttendantNotificationPN = d.IsAttendantNotificationPN,
				IsAttendantOneStep = d.IsAttendantOneStep,
				IsAttendantPriorityColor = d.IsAttendantPriorityColor,
				IsAttendantPriorityPN = d.IsAttendantPriorityPN,
				IsAttendantTaskNotes = d.IsAttendantTaskNotes,
				IsAttendantWorkflow = d.IsAttendantWorkflow,
				IsDisableDNDPhoto = d.IsDisableDNDPhoto,
				IsDisablePMSNotesDEP = d.IsDisablePMSNotesDEP,
				IsEnabledMaintenanceUnassigned = d.IsEnabledMaintenanceUnassigned,
				IsExplicitWorkflow = d.IsExplicitWorkflow,
				IsHideCleaningData = d.IsHideCleaningData,
				IsHideGuestInfo = d.IsHideGuestInfo,
				IsHostEnabled = d.IsHostEnabled,
				IsInspectionRemovesPriority = d.IsInspectionRemovesPriority,
				IsLFWaitingDisabled = d.IsLFWaitingDisabled,
				IsLimitAttendantContainers = d.IsLimitAttendantContainers,
				IsMaintenanceLitePN = d.IsMaintenanceLitePN,
				IsMaintenanceNormalPN = d.IsMaintenanceNormalPN,
				IsMaintenanceNotificationPN = d.IsMaintenanceNotificationPN,
				IsMaintenancePriorityPN = d.IsMaintenancePriorityPN,
				IsOCCNightly = d.IsOCCNightly,
				IsPlanningNightEnabled = d.IsPlanningNightEnabled,
				IsRemoveMessagesDaily = d.IsRemoveMessagesDaily,
				IsRemovePriorityOnFinish = d.IsRemovePriorityOnFinish,
				IsRenotifyPN = d.IsRenotifyPN,
				IsRestockEnabled = d.IsRestockEnabled,
				IsRoomNotesAttendantDisabled = d.IsRoomNotesAttendantDisabled,
				IsRoomNotesInspectorDisabled = d.IsRoomNotesInspectorDisabled,
				IsRoomNotesWebDisabled = d.IsRoomNotesWebDisabled,
				IsShowAllTaskCounts = d.IsShowAllTaskCounts,
				IsShowGuestName = d.IsShowGuestName,
				IsTaskMaintenance = d.IsTaskMaintenance,
				IsWebAttendantIcons = d.IsWebAttendantIcons,
				Modules = d.Modules == null ? null : new MobileHotelModuleDetails
				{
					IsAuditsDisabled = d.Modules.IsAuditsDisabled,
					IsEnableCiCo = d.Modules.IsEnableCiCo,
					IsEnablePlanner = d.Modules.IsEnablePlanner,
					IsExperiencesDisabled = d.Modules.IsExperiencesDisabled,
					IsInventoryDisabled = d.Modules.IsInventoryDisabled,
					IsPreventativeTasksDisabled = d.Modules.IsPreventativeTasksDisabled,
					IsRunnerDisabled = d.Modules.IsRunnerDisabled,
					IsTurndownDisabled = d.Modules.IsTurndownDisabled,
				},
				NightPlanningTime = d.NightPlanningTime,
				Notes = d.Notes,
				Organizations = d.Organizations,
				Phone = d.Phone,
				Remarks = d.Remarks,
				RemoteApi = d.RemoteApi,
				ShouldResetDirty = d.ShouldResetDirty,
				State = d.State,
				StayMultiplier = d.StayMultiplier,
				Street = d.Street,
				Thumbnail = d.Thumbnail,
				Timezone = d.Timezone,
				Username = d.Username,
				Users = d.Users,
				VacantMultiplier = d.VacantMultiplier,
				Zip = d.Zip,
			};
		}
	}
}
