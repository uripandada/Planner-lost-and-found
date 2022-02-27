using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.AppConfiguration.Queries.GetAttendantMobileAppConfiguration;
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

namespace Planner.Application.MobileApi.AppConfiguration.Queries.GetAttendantMobileAppConfigurationForHotel
{
	public class GetAttendantMobileAppConfigurationForHotelQuery : IRequest<AttendantMobileAppConfiguration>
	{
		public string HotelName { get; set; }
	}

	public class GetAttendantMobileAppConfigurationForHotelQueryHandler : IRequestHandler<GetAttendantMobileAppConfigurationForHotelQuery, AttendantMobileAppConfiguration>, IAmWebApplicationHandler
	{
		private readonly UserManager<User> _userManager;
		private IDatabaseContext _databaseContext;
		private IMasterDatabaseContext _masterContext;
		private readonly Guid _userId;

		public GetAttendantMobileAppConfigurationForHotelQueryHandler(IMasterDatabaseContext masterContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
		{
			this._masterContext = masterContext;
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._userManager = userManager;
		}

		public async Task<AttendantMobileAppConfiguration> Handle(GetAttendantMobileAppConfigurationForHotelQuery request, CancellationToken cancellationToken)
		{
			var user = await this._userManager.Users
				.Include(u => u.UserGroup)
				.Include(u => u.UserSubGroup)
				.Where(u => u.Id == this._userId)
				.FirstOrDefaultAsync();

			if(user == null) throw new Exception("Unknown user for loading full attendant application state.");

			var userRole = await this._databaseContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == this._userId);
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

			var hotel = hotels.FirstOrDefault(h => h.Name.Trim().ToLower() == request.HotelName.Trim().ToLower());
			if(hotel == null)
			{
				throw new Exception("User doesn't belog to the hotel.");
			}

			var floorIds = await this._databaseContext.Floors.Where(f => f.HotelId == hotel.Id).Select(f => f.Id).ToArrayAsync();
			var sqlQuery = $@"
				SELECT DISTINCT u.*
				FROM public.asp_net_users u
				JOIN public.asp_net_user_claims uc ON u.id = uc.user_id
				WHERE uc.claim_type = 'hotel_id'
				AND (uc.claim_value = 'ALL' OR uc.claim_value = '{hotel.Id}')";
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
					Id = hotel.Id,
					Name = hotel.Name,
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
					HotelId = hotel.Id,
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
