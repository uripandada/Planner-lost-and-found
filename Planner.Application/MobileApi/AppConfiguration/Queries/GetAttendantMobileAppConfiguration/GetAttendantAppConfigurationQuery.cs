using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Shared.Models;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.AppConfiguration.Queries.GetAttendantMobileAppConfiguration
{
	public class AttendantAppConfiguration
	{
		// "_id": "58aab35d1b93e01f7be7ca0d",
		// "__v": 0,
		public int AutomaticChangeSheets { get; set; } // "automaticChangeSheets": 0,
		public int CreditMinutes { get; set; } // "creditMinutes": 0,
		public int DepartureMultiplier { get; set; } // "departureMultiplier": 1,
		public bool DisableChangeSheetsOnSoonDeparture { get; set; } // "disableChangeSheetsOnSoonDeparture": false,
		public string[] EnabledTurndownCategories { get; set; } // "enabledTurndownCategories": [],
		/// <summary>
		/// DEPRECATED. Hotel Id should be HotelGroupId.
		/// </summary>
		public string HotelId { get; set; } // "hotelId": "55d06baa99295b3a52000000",
		public bool IsAdminOnlyAssetCreate { get; set; } // "isAdminOnlyAssetCreate": false,
		public bool IsAdvancedRestockEnabled { get; set; } // "isAdvancedRestockEnabled": false,
		public bool IsAllNightly { get; set; } // "isAllNightly": false,
		public bool IsEnableTurndown { get; set; } // "isEnableTurndown": false,
		public bool IsLFWaitingDisabled { get; set; } // "isLFWaitingDisabled": false,
		public bool IsOCCNightly { get; set; } // "isOCCNightly": false,
		public bool IsRemoveMessagesDaily { get; set; } // "isRemoveMessagesDaily": false,
		public bool IsRemovePriorityOnFinish { get; set; } // "isRemovePriorityOnFinish": false,
		public bool IsRemovePriorityOnInspected { get; set; } // "isRemovePriorityOnInspected": false,
		public bool IsRenotifyPN { get; set; } // "isRenotifyPN": false,
		public bool IsRoomNotesWebDisabled { get; set; } // "isRoomNotesWebDisabled": false,
		public bool IsShowAllTaskCounts { get; set; } // "isShowAllTaskCounts": false,
		public bool IsSimpleRestockEnabled { get; set; } // "isSimpleRestockEnabled": false,
		public bool ShouldResetDirty { get; set; } // "shouldResetDirty": false,
		public int StayMultiplier { get; set; } // "stayMultiplier": 1,
		public int VacantMultiplier { get; set; } // "vacantMultiplier": 1,
		public bool IsDisableAttendantAudits { get; set; } // "isDisableAttendantAudits": false,
		public bool IsEnableShowNonPlanned { get; set; } // "isEnableShowNonPlanned": false,
		public bool IsEnableRunnerRecentTasks { get; set; } // "isEnableRunnerRecentTasks": false,
		public bool IsDisableRunnerMarkExtras { get; set; } // "isDisableRunnerMarkExtras": false,
		public bool IsEnableRunnerExtra { get; set; } // "isEnableRunnerExtra": false,
		public bool IsEnableRunnerInspection { get; set; } // "isEnableRunnerInspection": false,
		public bool IsDisableRunnerAudits { get; set; } // "isDisableRunnerAudits": false,
		public bool IsRequireRunnerInventoryConfirmation { get; set; } // "isRequireRunnerInventoryConfirmation": false,
		public bool IsDisableRunnerExperience { get; set; } // "isDisableRunnerExperience": false,
		public bool IsDisableRunnerTurndown { get; set; } // "isDisableRunnerTurndown": false,
		public bool IsDisableMaintenanceExperience { get; set; } // "isDisableMaintenanceExperience": false,
		public bool IsDutyBackup { get; set; } // "isDutyBackup": false,
		public bool IsDutyEnabled { get; set; } // "isDutyEnabled": true,
		public bool IsMaintenanceFiltersPersist { get; set; } // "isMaintenanceFiltersPersist": false,
		public bool IsRequireMaintenanceActionTasks { get; set; } // "isRequireMaintenanceActionTasks": true,
		public bool IsDisableMaintenanceLiteTasks { get; set; } // "isDisableMaintenanceLiteTasks": false,
		public bool IsMaintenanceNotificationsPN { get; set; } // "isMaintenanceNotificationsPN": false,
		public bool IsMaintenanceLitePN { get; set; } // "isMaintenanceLitePN": false,
		public bool IsMaintenanceNormalPN { get; set; } // "isMaintenanceNormalPN": false,
		public bool IsMaintenancePriorityPN { get; set; } // "isMaintenancePriorityPN": false,
		public bool IsEnableAttendantPlannings { get; set; } // "isEnableAttendantPlannings": true,
		public bool IsEnableAttendantAuditsCreate { get; set; } // "isEnableAttendantAuditsCreate": false,
		public bool IsEnableAttendantAudits { get; set; } // "isEnableAttendantAudits": true,
		public bool IsDisableAttendantExperience { get; set; } // "isDisableAttendantExperience": false,
		public bool IsDisableAttendantTurndown { get; set; } // "isDisableAttendantTurndown": false,
		public bool IsEnableAttendantRecentTasks { get; set; } // "isEnableAttendantRecentTasks": false,
		public bool IsShowAttendantTasks { get; set; } // "isShowAttendantTasks": false,
		public bool IsDisableAttendantCreateNotes { get; set; } // "isDisableAttendantCreateNotes": false,
		public bool IsDisableAttendantMarkExtras { get; set; } // "isDisableAttendantMarkExtras": false,
		public bool IsRequireAttendantInventoryConfirmation { get; set; } // "isRequireAttendantInventoryConfirmation": false,
		public bool IsDisableAttendantFinishCancel { get; set; } // "isDisableAttendantFinishCancel": false,
		public bool IsHideAttendantMaintenance { get; set; } // "isHideAttendantMaintenance": false,
		public bool IsHideAttendantInventory { get; set; } // "isHideAttendantInventory": false,
		public bool IsHideAttendantLF { get; set; } // "isHideAttendantLF": false,
		public bool IsHideAttendantGallery { get; set; } // "isHideAttendantGallery": false,
		public bool IsHideAttendantNotes { get; set; } // "isHideAttendantNotes": false,
		public bool IsHideAttendantDescription { get; set; } // "isHideAttendantDescription": false,
		public bool IsHideAttendantCategory { get; set; } // "isHideAttendantCategory": false,
		public bool IsHideAttendantCredits { get; set; } // "isHideAttendantCredits": false,
		public bool IsHideAttendantMainFilter { get; set; } // "isHideAttendantMainFilter": false,
		public bool IsEnableVoucherDnd { get; set; } // "isEnableVoucherDnd": false,
		public bool IsShowCreditsMain { get; set; } // "isShowCreditsMain": false,
		public bool IsDisableAttendantCleaningLimit { get; set; } // "isDisableAttendantCleaningLimit": false,
		public bool IsDisableAttendantPauseLimit { get; set; } // "isDisableAttendantPauseLimit": false,
		public bool IsDisablePMSNotesAttendantDep { get; set; } // "isDisablePMSNotesAttendantDep": false,
		public bool IsDisablePMSNotesAttendant { get; set; } // "isDisablePMSNotesAttendant": false,
		public bool IsAttendantNotificationsPN { get; set; } // "isAttendantNotificationsPN": false,
		public bool IsAttendantNormalPN { get; set; } // "isAttendantNormalPN": false,
		public bool IsAttendantPriorityPN { get; set; } // "isAttendantPriorityPN": false,
		public string ExplicitWorkflowOccupied { get; set; } // "explicitWorkflowOccupied": "OHC",
		public string ExplicitWorkflowVacant { get; set; } // "explicitWorkflowVacant": "VHCI",
		public bool IsEnableExplicitAttendantWorkflow { get; set; } // "isEnableExplicitAttendantWorkflow": false,
		public bool IsEnableAttendantWorkflow { get; set; } // "isEnableAttendantWorkflow": false,
		public int AttendantMinimumMinutes { get; set; } // "attendantMinimumMinutes": 1,
		public bool IsHideAttendantTimer { get; set; } // "isHideAttendantTimer": false,
		public bool IsDisableInspectorPlanning { get; set; } // "isDisableInspectorPlanning": false,
		public bool IsEnableInspectorConcierge { get; set; } // "isEnableInspectorConcierge": false,
		public bool IsDisableInspectorExperience { get; set; } // "isDisableInspectorExperience": false,
		public bool IsDisableInspectorTurndown { get; set; } // "isDisableInspectorTurndown": false,
		public bool IsDisableRestocksDifferences { get; set; } // "isDisableRestocksDifferences": false,
		public bool IsRequireInspectorActionTasks { get; set; } // "isRequireInspectorActionTasks": false,
		public bool IsDisableInspectorLiteTasks { get; set; } // "isDisableInspectorLiteTasks": false,
		public bool IsDisableUnblocksDifferences { get; set; } // "isDisableUnblocksDifferences": false,
		public bool IsDisableMessagesDifferences { get; set; } // "isDisableMessagesDifferences": false,
		public bool IsDisablePriorityDifferences { get; set; } // "isDisablePriorityDifferences": false,
		public bool IsWebAutoLogout { get; set; } // "isWebAutoLogout": false,
		public bool IsWebHotelScheduleEnabled { get; set; } // "isWebHotelScheduleEnabled": true,
		public bool IsDisablePlanningExportLongStay { get; set; } // "isDisablePlanningExportLongStay": false,
		public bool IsDisablePlanningExportChangeSheets { get; set; } // "isDisablePlanningExportChangeSheets": false,
		public bool IsDisablePlanningExportPriority { get; set; } // "isDisablePlanningExportPriority": false,
		public bool IsDisablePlanningExportCredits { get; set; } // "isDisablePlanningExportCredits": false,
		public bool IsDisablePlanningExportPMSNote { get; set; } // "isDisablePlanningExportPMSNote": false,
		public bool IsDisablePlanningExportOccupants { get; set; } // "isDisablePlanningExportOccupants": false,
		public bool IsDisablePlanningExportName { get; set; } // "isDisablePlanningExportName": false,
		public bool IsDisablePlanningExportCategory { get; set; } // "isDisablePlanningExportCategory": false,
		public bool IsDisablePlanningExportHousekeeping { get; set; } // "isDisablePlanningExportHousekeeping": false,
		public bool IsLimitPlanningContainers { get; set; } // "isLimitPlanningContainers": true,
		public bool IsDisableLFWaiting { get; set; } // "isDisableLFWaiting": true,
		public bool IsLegacyAttendantIcons { get; set; } // "isLegacyAttendantIcons": false,
		public bool IsDisableRoomNotesWeb { get; set; } // "isDisableRoomNotesWeb": false,
		public bool IsShowAllTasks { get; set; } // "isShowAllTasks": false,
		public bool IsEnableMice { get; set; } // "isEnableMice": false,
		public string NetworkMessage { get; set; } // "networkMessage": "medium",
		public bool IsEnableAdvancedMessages { get; set; } // "isEnableAdvancedMessages": true,
		public bool IsEnableAudits { get; set; } // "isEnableAudits": true,
		public bool IsUpgradedPlanningV3 { get; set; } // "isUpgradedPlanningV3": false,
		public bool IsUpgradedPlanning { get; set; } // "isUpgradedPlanning": true,
		public bool IsAdvancedTaskForm { get; set; } // "isAdvancedTaskForm": true,
		public bool IsHostsEnabled { get; set; } // "isHostsEnabled": false,
		public bool IsShowDurableAssets { get; set; } // "isShowDurableAssets": true,
		public bool IsTaskMaintenance { get; set; } // "isTaskMaintenance": true,
		public bool IsDisableLiteTasks { get; set; } // "isDisableLiteTasks": false,
		public bool IsLimitAssetActionAdmin { get; set; } // "isLimitAssetActionAdmin": false,
		public bool IsRemovePriorityRooms { get; set; } // "isRemovePriorityRooms": true,
		public bool IsResetRoomDirty { get; set; } // "isResetRoomDirty": false,
		public bool IsPushDNDChangeSheets { get; set; } // "isPushDNDChangeSheets": false,
		public bool IsChangeSheetsDelay { get; set; } // "isChangeSheetsDelay": false,
		public int ChangeSheetsInternval { get; set; } // "changeSheetsInternval": 2,
		public bool IsChangeSheetsEnabled { get; set; } // "isChangeSheetsEnabled": false,
		public bool IsAutomaticMessages { get; set; } // "isAutomaticMessages": false,
		public bool IsRemoveMessageNightly { get; set; } // "isRemoveMessageNightly": true,
		public int OccupiedRoomsInterval { get; set; } // "occupiedRoomsInterval": 2,
		public bool IsOccupiedRoomsInterval { get; set; } // "isOccupiedRoomsInterval": false,
		public bool IsAparthotelSettings { get; set; } // "isAparthotelSettings": true,
		public bool IsAllRoomsNightly { get; set; } // "isAllRoomsNightly": false,
		public bool IsOccupiedRoomsDeparture { get; set; } // "isOccupiedRoomsDeparture": false,
		public bool IsOccupiedRoomsNightly { get; set; } // "isOccupiedRoomsNightly": false
	}

	public class AttendantPermission
	{

	}

	public class AttendantMobileAppConfiguration
	{
		public UserForMobile User { get; set; }
		public HotelForMobile Hotel { get; set; }
		/// <summary>
		/// Groups are currently not supported.
		/// </summary>
		public string[] Groups { get; set; }
		/// <summary>
		/// Permission system changed so this is DEPRECATED.
		/// </summary>
		public AttendantPermission[] Permissions { get; set; }
		public AttendantAppConfiguration Config { get; set; }
	}

	public class GetAttendantMobileAppConfigurationQuery : IRequest<AttendantMobileAppConfiguration>
	{
		public Guid UserId { get; set; }
	}

	public class GetAttendantMobileAppConfigurationQueryHandler : IRequestHandler<GetAttendantMobileAppConfigurationQuery, AttendantMobileAppConfiguration>, IAmWebApplicationHandler
	{
		private readonly UserManager<User> _userManager;
		private IDatabaseContext _databaseContext;
		private IMasterDatabaseContext _masterContext;
		private readonly Guid _userId;

		public GetAttendantMobileAppConfigurationQueryHandler(IMasterDatabaseContext masterContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
		{
			this._masterContext = masterContext;
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._userManager = userManager;
		}

		public async Task<AttendantMobileAppConfiguration> Handle(GetAttendantMobileAppConfigurationQuery request, CancellationToken cancellationToken)
		{
			var user = await this._userManager.Users
				.Include(u => u.UserGroup)
				.Include(u => u.UserSubGroup)
				.Where(u => u.Id == request.UserId)
				.FirstOrDefaultAsync();

			if(user == null) throw new Exception("Unknown user for loading full attendant application state.");

			var userRole = await this._databaseContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == request.UserId);
			if (userRole == null) throw new Exception("User doesn't belong to any role when loading full attendant application state.");
			var role = await this._databaseContext.Roles.FindAsync(userRole.RoleId);

			var userClaims = await this._userManager.GetClaimsAsync(user);
			var hotelIdClaims = userClaims.Where(c => c.Type == Planner.Domain.Values.ClaimsKeys.HotelId).ToArray();

			if (!hotelIdClaims.Any()) throw new Exception("User is not assigned to a hotel when loading full attendant application state.");

			var loadAllHotels = hotelIdClaims.Any(c => c.Value == "ALL");
			var hotelIds = new string[0];
			var hotels = new Domain.Entities.Hotel[0];
			
			if (loadAllHotels)
			{
				hotels = await this._databaseContext.Hotels.ToArrayAsync();
			}
			else
			{
				hotelIds = hotelIdClaims.Select(c => c.Value).ToArray();
				hotels = await this._databaseContext.Hotels.Where(h => hotelIds.Contains(h.Id)).ToArrayAsync();
			}

			if (!hotels.Any()) throw new Exception("Unable to find hotels assigned to the user when loading full attendant application state.");

			var hotelGroupId = this._databaseContext.HotelGroupTenant.Id;
			var hotelGroupName = this._databaseContext.HotelGroupTenant.Name;
			var hotelGroupKey = this._databaseContext.HotelGroupTenant.Key;

			// WARNING! In order to make mobile app compatible, some hotel must be chosen as a hotel of choice. 
			// Usually, the attendant (cleaner) will have only one assigned hotel so it will make no difference.
			var firstHotel = hotels.First();
			var floorIds = await this._databaseContext.Floors.Where(f => f.HotelId == firstHotel.Id).Select(f => f.Id).ToArrayAsync();
			var sqlQuery = $@"
				SELECT DISTINCT u.*
				FROM public.asp_net_users u
				JOIN public.asp_net_user_claims uc ON u.id = uc.user_id
				WHERE uc.claim_type = 'hotel_id'
				AND (uc.claim_value = 'ALL' OR uc.claim_value = '{firstHotel.Id}')";
			var userIds = await this._databaseContext.Users.FromSqlRaw(sqlQuery).Select(u => u.Id).ToArrayAsync();
			
			var userAvatar = await this._databaseContext.ApplicationUserAvatar.Select(a => new { Id = a.Id, Url = a.FileUrl }).FirstOrDefaultAsync(a => a.Id == user.Id);

			var response = new AttendantMobileAppConfiguration
			{
				Groups = new string[0],
				Permissions = new AttendantPermission[0],
				User = new UserForMobile
				{
					Id = user.Id,
					Email = user.Email,
					First_name=user.FirstName,
					Last_name = user.LastName,
					Image = userAvatar == null ? null : userAvatar.Url,
					Thumbnail = userAvatar == null ? null : userAvatar.Url,
					Hotel = hotelGroupId.ToString(),
					HotelUsername = hotelGroupKey,
					HotelUsernameRequired = true,
					HotelGroupId = hotelGroupId,
					HotelGroupKey = hotelGroupKey,
					HotelGroupName = hotelGroupName,
					AvailableHotels = hotels.Select(h => new AvailableHotelForMobile { HotelId = h.Id, HotelName = h.Name }).ToArray(),
					Status = "active",
					UserGroupId = user.UserGroupId,
					UserGroupName = user.UserGroup == null ? null : user.UserGroup.Name,
					Username = user.UserName,
					UserSubGroupId = user.UserSubGroupId,
					UserSubGroupName = user.UserSubGroup == null ? null : user.UserSubGroup.Name,
					Country = "",
					Zip = "",
					State = "",
					City = "",
					Street = "",
					Organization = "", 
					Role = 0,
					RoleId = role.Id,
					RoleName = role.Name,
					Permissions = new string[0],
					EmployeeId = null,
					Groups = new string[0],
					IsAdministrator = false,
					IsAttendant = true,
					IsFoodBeverage = false,
					IsHost = false,
					IsInspector = false,
					IsMaintenance = false,
					IsOnDuty = true,
					IsBypassAttendant = false,
					IsReceptionist = false,
					IsRoomRunner = false,
					IsRoomsService = false,
					IsSuperAdmin = false,
					Language = user.Language,
					AppVersion = "N/A",
					Hashed_password = "DEPRECATED",
					Salt = "DEPRECATED",
				},
				Hotel = new HotelForMobile
				{
					Id = firstHotel.Id,
					Name = firstHotel.Name,
					AttendantCancelMinutes = 0,
					AttendantDefaultView = "quantity",
					Username = hotelGroupKey,
					Users = userIds,
					Zip = "",
					City = "",
					Country = "",
					CreditMinutes = 1,
					DepartureMultiplier = 1,
					StayMultiplier = 1,
					VacantMultiplier = 0,
					DisableAttendantTimer = true,
					DisableGuestLocator = false,
					Display = null,
					EscalationPhones = new string[0],
					ExplicitOccupiedWorkflow = "OHC",
					ExplicitVacantWorkflow = "VHC",
					Floors = floorIds,
					Images = "",
					IsAdminOnlyAssetCreate = false,
					IsAdvancedLayout = true,
					IsAllNightly = false,
					IsAttendantBypass = false,
					IsAttendantDisablePMSNotes = false,
					IsAttendantImmediateTasks = true,
					IsAttendantLitePN = false,
					IsAttendantNormalPN = false,
					IsAttendantNotificationPN = false,
					IsAttendantOneStep = false,
					IsAttendantPriorityColor = true,
					IsAttendantPriorityPN = false,
					IsAttendantTaskNotes = false,
					IsAttendantWorkflow = true,
					IsDisableDNDPhoto = true,
					IsDisablePMSNotesDEP = false,
					IsEnabledMaintenanceUnassigned = false,
					IsExplicitWorkflow = false,
					IsHideCleaningData = false,
					IsHideGuestInfo = false,
					IsHostEnabled = true,
					IsInspectionRemovesPriority = false,
					IsLFWaitingDisabled = true,
					IsLimitAttendantContainers = true,
					IsMaintenanceLitePN = true,
					IsMaintenanceNormalPN = true,
					IsMaintenanceNotificationPN = true,
					IsMaintenancePriorityPN = true,
					IsOCCNightly = false,
					IsPlanningNightEnabled = true,
					IsRemoveMessagesDaily = true,
					IsRemovePriorityOnFinish = true,
					IsRenotifyPN = false,
					IsRestockEnabled = false,
					IsRoomNotesAttendantDisabled = false,
					IsRoomNotesInspectorDisabled = false,
					IsRoomNotesWebDisabled = false,
					IsShowAllTaskCounts = false,
					IsShowGuestName = true,
					IsTaskMaintenance = true,
					IsWebAttendantIcons = false,
					Modules = new HotelModulesForMobile
					{
						IsAuditsDisabled = false,
						IsEnableCiCo = false,
						IsEnablePlanner = false,
						IsExperiencesDisabled = false,
						IsInventoryDisabled = false,
						IsPreventativeTasksDisabled = false,
						IsRunnerDisabled = false,
						IsTurndownDisabled = false,
					},
					NightPlanningTime = 21,
					Notes = new string[0],
					Organizations = new string[0],
					Phone = "",
					Remarks = "",
					RemoteApi = "",
					ShouldResetDirty = true,
					State = "WVL",
					Street = "",
					Thumbnail = "",
					Timezone = 0,
				},
				Config = new AttendantAppConfiguration
				{
					AutomaticChangeSheets = 0,
					CreditMinutes = 0,
					DepartureMultiplier = 1,
					DisableChangeSheetsOnSoonDeparture= false,
					EnabledTurndownCategories = new string[0],
					HotelId = firstHotel.Id,
					IsAdminOnlyAssetCreate = false,
					IsAdvancedRestockEnabled =false,
					IsAllNightly = false,
					IsEnableTurndown = false,
					IsLFWaitingDisabled = false,
					IsOCCNightly = false,
					IsRemoveMessagesDaily = false,
					IsRemovePriorityOnFinish = false,
					IsRemovePriorityOnInspected = false,
					IsRenotifyPN = false,
					IsRoomNotesWebDisabled = false,
					IsShowAllTaskCounts = false,
					IsSimpleRestockEnabled = false,
					ShouldResetDirty = false,
					StayMultiplier=1,
					VacantMultiplier=1,
					IsDisableAttendantAudits = false,
					IsEnableShowNonPlanned = false,
					IsEnableRunnerRecentTasks = false,
					IsDisableRunnerMarkExtras = false,
					IsDutyEnabled = true,
					IsRequireMaintenanceActionTasks = true,
					IsEnableRunnerExtra = false,
					IsEnableRunnerInspection = false,
					IsDisableRunnerAudits = false,
					IsRequireRunnerInventoryConfirmation = false,
					IsDisableRunnerExperience = false,
					IsDisableRunnerTurndown = false,
					IsDisableMaintenanceExperience = false,
					IsDutyBackup = false,
					IsMaintenanceFiltersPersist = false,
					IsDisableMaintenanceLiteTasks = false,
					IsMaintenanceNotificationsPN = false,
					IsMaintenanceLitePN = false,
					IsMaintenanceNormalPN = false,
					IsMaintenancePriorityPN = false,
					IsEnableAttendantPlannings = true,
					IsEnableAttendantAuditsCreate = false,
					IsEnableAttendantAudits = true,
					IsDisableAttendantExperience = false,
					IsDisableAttendantTurndown = false,
					IsEnableAttendantRecentTasks = false,
					IsShowAttendantTasks = false,
					IsDisableAttendantCreateNotes = false,
					IsDisableAttendantMarkExtras = false,
					IsRequireAttendantInventoryConfirmation = false,
					IsDisableAttendantFinishCancel = false,
					IsHideAttendantMaintenance = false,
					IsHideAttendantInventory = false,
					IsHideAttendantLF = false,
					IsHideAttendantGallery = false,
					IsHideAttendantNotes = false,
					IsHideAttendantDescription = false,
					IsHideAttendantCategory = false,
					IsHideAttendantCredits = false,
					IsHideAttendantMainFilter = false,
					IsEnableVoucherDnd = false,
					IsShowCreditsMain = false,
					IsDisableAttendantCleaningLimit = false,
					IsDisableAttendantPauseLimit = false,
					IsDisablePMSNotesAttendantDep = false,
					IsDisablePMSNotesAttendant = false,
					IsAttendantNotificationsPN = false,
					IsAttendantNormalPN = false,
					IsAttendantPriorityPN = false,
					ExplicitWorkflowOccupied = "OHC",
					ExplicitWorkflowVacant = "VHCI",
					IsEnableExplicitAttendantWorkflow = false,
					IsEnableAttendantWorkflow = false,
					AttendantMinimumMinutes = 1,
					IsHideAttendantTimer = false,
					IsDisableInspectorPlanning = false,
					IsEnableInspectorConcierge = false,
					IsDisableInspectorExperience = false,
					IsDisableInspectorTurndown = false,
					IsDisableRestocksDifferences = false,
					IsRequireInspectorActionTasks = false,
					IsDisableInspectorLiteTasks = false,
					IsDisableUnblocksDifferences = false,
					IsDisableMessagesDifferences = false,
					IsWebAutoLogout = false,
					IsWebHotelScheduleEnabled = true,
					IsDisablePlanningExportLongStay = false,
					IsDisablePlanningExportChangeSheets = false,
					IsDisablePlanningExportPriority = false,
					IsDisablePlanningExportCredits = false,
					IsDisablePlanningExportPMSNote = false,
					IsDisablePlanningExportOccupants = false,
					IsDisablePlanningExportName = false,
					IsDisablePlanningExportCategory = false,
					IsDisablePlanningExportHousekeeping = false,
					IsLimitPlanningContainers = true,
					IsDisableLFWaiting = true,
					IsLegacyAttendantIcons = false,
					IsDisableRoomNotesWeb = false,
					IsShowAllTasks = false,
					IsEnableMice = false,
					NetworkMessage = "medium",
					IsEnableAdvancedMessages = true,
					IsEnableAudits = true,
					IsUpgradedPlanningV3 = false,
					IsUpgradedPlanning = true,
					IsAdvancedTaskForm = true,
					IsHostsEnabled = false,
					IsShowDurableAssets = true,
					IsTaskMaintenance = true,
					IsDisableLiteTasks = false,
					IsLimitAssetActionAdmin = false,
					IsRemovePriorityRooms = true,
					IsResetRoomDirty = false,
					IsPushDNDChangeSheets = false,
					IsChangeSheetsDelay = false,
					ChangeSheetsInternval = 2,
					IsChangeSheetsEnabled = false,
					IsAutomaticMessages = false,
					IsRemoveMessageNightly = true,
					OccupiedRoomsInterval = 2,
					IsOccupiedRoomsInterval = false,
					IsAparthotelSettings = true,
					IsOccupiedRoomsDeparture = false,
					IsOccupiedRoomsNightly = false,
					IsAllRoomsNightly = false,
					IsDisablePriorityDifferences = false
				}
			};

			return response;
		}
	}
}
