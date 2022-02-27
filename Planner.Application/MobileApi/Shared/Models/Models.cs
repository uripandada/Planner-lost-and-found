using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Shared.Models
{
	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomExternalInfoForMobile
	{
		//     location: { type: String },
		//     lat: { type: Number },
		//     long: { type: Number },
		//     access: { type: String },
		//     trash: { type: String },
		//     parking: { type: String },
		//     wifi: { type: String },
		//     other: { type: String },
		//     ownerName: { type: String },
		//     ownerPhone: { type: String },
		//     ownerEmail: { type: String },
		public string Location { get; set; } = null;
		public long Lat { get; set; } = 0L;
		public long Long { get; set; } = 0L;
		public string Access { get; set; } = null;
		public string Trash { get; set; } = null;
		public string Parking { get; set; } = null;
		public string Wifi { get; set; } = null;
		public string Other { get; set; } = null;
		public string OwnerName { get; set; } = null;
		public string OwnerPhone { get; set; } = null;
		public string OwnerEmail { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomMessageForMobile
	{
		//     message: String,
		//     dateTs: Number,
		//     userId: ObjectId,
		//     messageId: String,
		//     messageType: String,
		//     schedule: String,
		//     startDate: String,
		//     endDate: String
		public string Message { get; set; } = null;
		public string DateTs { get; set; } = null;
		public Guid UserId { get; set; } = Guid.Empty;
		public Guid MessageId { get; set; } = Guid.Empty;
		public string MessageType { get; set; } = "NULL message";
		public string Schedule { get; set; } = null;
		public string StartDate { get; set; } = null;
		public string EndDate { get; set; } = null;

	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomExtraForMobile
	{

		//     label: String,
		//     credits: Number,
		//     isCompleted: Boolean,
		//     endDate: String
		public string Label { get; set; } = null;
		public int Credits { get; set; } = 0;
		public bool IsCompleted { get; set; } = false;
		public string EndDate { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class HotelModulesForMobile
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

	/// <summary>
	/// Completed.
	/// </summary>
	public class HotelForMobile
	{
		// "__v": 10,
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
		public HotelModulesForMobile Modules { get; set; } // "modules": { ... }
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

	/// <summary>
	/// Completed.
	/// </summary>
	public class AssetCategoryForMobile
	{
		public string Label { get; set; } = "NULL category"; // label: { type: String, required: true, validate: labelValidations },
		public string HotelId { get; set; } = Guid.Empty.ToString(); // hotelId: { type: ObjectId, ref: 'Hotel', index: true },
		public HotelForMobile Hotel { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class AssetExternalCompanyForMobile
	{
		public string Name { get; set; } = "NULL external company";
		public string Phone { get; set; } = null;
		public string Email { get; set; } = null;
		public string Notes { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class AssetAttachmentForMobile
	{
		public string Name { get; set; } = "NULL attachment";
		public string Url { get; set; } = null;
		public string Mimetype { get; set; } = null;

	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class AssetForMobile
	{
		public string Name { get; set; } = ""; // name: { type: String, required: true, validate: nameValidations },
		public string[] Aliases { get; set; } = new string[0]; // aliases: [{ type: String }],
		public string HotelId { get; set; } = Guid.Empty.ToString(); // hotelId: { type: ObjectId, ref: 'Hotel', index: true },
		public HotelForMobile Hotel { get; set; } = null;
		public Guid AssetId { get; set; } = Guid.Empty; // assetId: { type: String, required: false },
		public AssetCategoryForMobile AssetCategory { get; set; } = null; // assetCategory: { type: ObjectId, ref: 'AssetCategory' },
		public DateTime FirstuseDate { get; set; } = DateTime.UtcNow; // firstuseDate: { type: Date, default: Date.now() },
		public RoomCategoryForMobile RoomCategory { get; set; } = null; // roomCategory: { type: ObjectId, ref: 'RoomCategory' },
		public RoomAreaForMobile RoomArea { get; set; } = null; // roomArea: { type: ObjectId, ref: 'RoomArea' },
		public string Image { get; set; } = ""; // image: { type: String, required: true },
		public string ImageHighRes { get; set; } = ""; // imageHighRes: { type: String, default: "" },
		public string ImageLowRes { get; set; } = ""; // imageLowRes: { type: String, default: "" },
		public string Remark { get; set; } = null; // remark: { type: String, required: false },
		public string Attachment { get; set; } = ""; // attachment: { type: String, default: '' },
		public string AttachmentName { get; set; } = ""; // attachmentName: { type: String, default: '' },
		public string DefaultType { get; set; } = "normal"; // defaultType: { type: String, default: 'normal' },
		public int GlobalInventory { get; set; } = 0; // globalInventory: { type: Number, default: 0 },
		public int InventoryWarning { get; set; } = -1; // inventoryWarning: { type: Number, default: -1 },
		public AssetExternalCompanyForMobile ExternalCompany { get; set; } = null; // externalCompany: [{ name: String, phone: String, email: String, notes: String }],
		public AssetAttachmentForMobile[] Attachments { get; set; } = new AssetAttachmentForMobile[0]; // attachments: [{ name: String, url: String, mimetype: String }],
		public string LastAction { get; set; } = null; // lastAction: { type: String, required: false },
		public DateTime LastDate { get; set; } = DateTime.UtcNow; // lastDate: { type: Date, default: new Date() },
		public int Price { get; set; } = 0;  // price: { type: Number, default: 0 },
		public int Tax { get; set; } = 0; // tax: { type: Number, default: 0 },
		public int? ManagedActions { get; set; } = null; // managedActions: [{ type: Number }],
		public int[] CustomActions { get; set; } = new int[0]; // customActions: [{ type: Number }]
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomCategoryForMobile
	{
		public Guid Id { get; set; }
		public string Label { get; set; } = "NULL category"; // label: { type: String, required: true, validate: labelValidations },
		public int? Credits { get; set; } = 1; // credits: { type: Number, default: 1, required: false },
		public string HotelId { get; set; } = Guid.Empty.ToString(); // hotelId: { type: ObjectId, ref: 'Hotel', index: true },
		public HotelForMobile Hotel { get; set; } = null;
		public bool IsPublic { get; set; } = false; // isPublic: { type: Boolean, default: false },
		public bool IsPrivate { get; set; } = false; // isPrivate: { type: Boolean, default: false },
		public bool IsOutside { get; set; } = false; // isOutside: { type: Boolean, default: false },
		public int CreditsStay { get; set; } = 0; // creditsStay: { isActive: false, value: 0 },
		public int CreditsDep { get; set; } = 0; // creditsDep: { isActive: false, value: 0 },
		public int CreditsCS { get; set; } = 0; // creditsCS: { isActive: false, value: 0 },
		public int CreditsLS { get; set; } = 0; // creditsLS: { isActive: false, value: 0 },
		public int CreditsOther { get; set; } = 0; // creditsOther: { isActive: false, value: 0 }
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomStatusForMobile
	{
		public string Label { get; set; } = "NULL status"; // label: { type: String, required: true, validate: labelValidations },
		public string Code { get; set; } = null; // code: { type: String, required: false, validate: codeValidations },
		public string Color { get; set; } = "000000"; // color: { type: String, default: '000000' },
		public RoomHousekeepingForMobile[] RoomHousekeepings { get; set; } = new RoomHousekeepingForMobile[0]; // roomHousekeepings: [{ type: ObjectId, ref: 'RoomHousekeeping'}],
		public string HotelId { get; set; } = Guid.Empty.ToString(); // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
		public HotelForMobile Hotel { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomMaintenanceForMobile
	{
		public string Label { get; set; } = "NULL maintenance"; // label: { type: String, required: true, validate: labelValidations },
		public string HotelId { get; set; } = Guid.Empty.ToString(); // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
		public HotelForMobile Hotel { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomHousekeepingForMobile
	{
		public string Label { get; set; } = null; // label: { type: String, required: true, validate: labelValidations },
		public string Code { get; set; } = null; // code: { type: String, required: false, validate: codeValidations },
		public string Color { get; set; } = "000000"; // color: { type: String, default: '000000' },
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
		public HotelForMobile Hotel { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class ConfigurationForMobile
	{
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
		public HotelForMobile Hotel { get; set; } = null;

		// System
		public bool IsOccupiedRoomsNightly { get; set; } = false; // isOccupiedRoomsNightly: { type: Boolean, default: false },
		public bool IsOccupiedRoomsDeparture { get; set; } = false; // isOccupiedRoomsDeparture: { type: Boolean, default: false },
		public bool IsAllRoomsNightly { get; set; } = false; // isAllRoomsNightly: { type: Boolean, default: false },
		public bool IsAparthotelSettings { get; set; } = false; // isAparthotelSettings: { type: Boolean, default: false },
		public bool IsOccupiedRoomsInterval { get; set; } = false; // isOccupiedRoomsInterval: { type: Boolean, default: false },
		public int OccupiedRoomsInterval { get; set; } = 2; // occupiedRoomsInterval: { type: Number, default: 2 },
		public bool IsRemoveMessageNightly { get; set; } = false; // isRemoveMessageNightly: { type: Boolean, default: false },
		public bool IsAutomaticMessages { get; set; } = false; // isAutomaticMessages: { type: Boolean, default: false },
		public string AutomaticRoomMessage { get; set; } = null; // automaticRoomMessage: { message: String, interval: Number },
		public bool IsChangeSheetsEnabled { get; set; } = false; // isChangeSheetsEnabled: { type: Boolean, default: false },
		public int ChangeSheetsInternval { get; set; } = 2; // changeSheetsInternval: { type: Number, default: 2 },
		public bool IsChangeSheetsDelay { get; set; } = false; // isChangeSheetsDelay: { type: Boolean, default: false },
		public bool IsPushDNDChangeSheets { get; set; } = false; // isPushDNDChangeSheets: { type: Boolean, default: false },
		public bool IsResetRoomDirty { get; set; } = false; // isResetRoomDirty: { type: Boolean, default: false },
		public bool IsRemovePriorityRooms { get; set; } = false; // isRemovePriorityRooms: { type: Boolean, default: false },
		public bool IsLimitAssetActionAdmin { get; set; } = false; // isLimitAssetActionAdmin: { type: Boolean, default: false },
		public bool IsDisableLiteTasks { get; set; } = false; // isDisableLiteTasks: { type: Boolean, default: false },
		public bool IsTaskMaintenance { get; set; } = false; // isTaskMaintenance: { type: Boolean, default: false },
		public bool IsShowDurableAssets { get; set; } = false; // isShowDurableAssets: { type: Boolean, default: false },
		public bool IsHostsEnabled { get; set; } = false; // isHostsEnabled: { type: Boolean, default: false },
		public bool IsAdvancedTaskForm { get; set; } = false; // isAdvancedTaskForm: { type: Boolean, default: false },
		public bool IsUpgradedPlanning { get; set; } = false; // isUpgradedPlanning: { type: Boolean, default: false },
		public bool IsUpgradedPlanningV3 { get; set; } = false; // isUpgradedPlanningV3: { type: Boolean, default: false },
		public bool IsEnableAudits { get; set; } = false; // isEnableAudits: { type: Boolean, default: false },
		public bool IsEnableAdvancedMessages { get; set; } = false; // isEnableAdvancedMessages: { type: Boolean, default: false },
		public string NetworkMessage { get; set; } = "medium"; // networkMessage: { type: String, default: 'medium' },
		public bool IsEnableMice { get; set; } = false; // isEnableMice: { type: Boolean, default: false },
											   
		// Web
		public bool IsShowAllTasks { get; set; } = false; // isShowAllTasks: { type: Boolean, default: false },
		public bool IsDisableRoomNotesWeb { get; set; } = false; // isDisableRoomNotesWeb: { type: Boolean, default: false },
		public bool IsLegacyAttendantIcons { get; set; } = false; // isLegacyAttendantIcons: { type: Boolean, default: false },
		public bool IsDisableLFWaiting { get; set; } = false; // isDisableLFWaiting: { type: Boolean, default: false },
		public bool IsLimitPlanningContainers { get; set; } = false; // isLimitPlanningContainers: { type: Boolean, default: false },
		public bool IsDisablePlanningExportHousekeeping { get; set; } = false; // isDisablePlanningExportHousekeeping: { type: Boolean, default: false },
		public bool IsDisablePlanningExportCategory { get; set; } = false; // isDisablePlanningExportCategory: { type: Boolean, default: false },
		public bool IsDisablePlanningExportName { get; set; } = false; // isDisablePlanningExportName: { type: Boolean, default: false },
		public bool IsDisablePlanningExportOccupants { get; set; } = false; // isDisablePlanningExportOccupants: { type: Boolean, default: false },
		public bool IsDisablePlanningExportPMSNote { get; set; } = false; // isDisablePlanningExportPMSNote: { type: Boolean, default: false },
		public bool IsDisablePlanningExportCredits { get; set; } = false; // isDisablePlanningExportCredits: { type: Boolean, default: false },
		public bool IsDisablePlanningExportPriority { get; set; } = false; // isDisablePlanningExportPriority: { type: Boolean, default: false },
		public bool IsDisablePlanningExportChangeSheets { get; set; } = false; // isDisablePlanningExportChangeSheets: { type: Boolean, default: false },
		public bool IsDisablePlanningExportLongStay { get; set; } = false; // isDisablePlanningExportLongStay: { type: Boolean, default: false },
		public bool IsWebHotelScheduleEnabled { get; set; } = false; // isWebHotelScheduleEnabled: { type: Boolean, default: false },
		public bool IsWebAutoLogout { get; set; } = false; // isWebAutoLogout: { type: Boolean, default: false },
		
		// Inspector
		public bool IsDisablePriorityDifferences { get; set; } = false; // isDisablePriorityDifferences: { type: Boolean, default: false },
		public bool IsDisableMessagesDifferences { get; set; } = false; // isDisableMessagesDifferences: { type: Boolean, default: false },
		public bool IsDisableUnblocksDifferences { get; set; } = false; // isDisableUnblocksDifferences: { type: Boolean, default: false },
		public bool IsDisableInspectorLiteTasks { get; set; } = false; // isDisableInspectorLiteTasks: { type: Boolean, default: false },
		public bool IsRequireInspectorActionTasks { get; set; } = false; // isRequireInspectorActionTasks: { type: Boolean, default: false },
		public bool IsDisableRestocksDifferences { get; set; } = false; // isDisableRestocksDifferences: { type: Boolean, default: false },
		public bool IsDisableInspectorTurndown { get; set; } = false; // isDisableInspectorTurndown: { type: Boolean, default: false },
		public bool IsDisableInspectorExperience { get; set; } = false; // isDisableInspectorExperience: { type: Boolean, default: false },
		public bool IsEnableInspectorConcierge { get; set; } = false; // isEnableInspectorConcierge: { type: Boolean, default: false },
		public bool IsDisableInspectorPlanning { get; set; } = false; // isDisableInspectorPlanning: { type: Boolean, default: false },
		
		// Attendant
		public bool IsHideAttendantTimer { get; set; } = false; // isHideAttendantTimer: { type: Boolean, default: false },
		public int AttendantMinimumMinutes { get; set; } = 2; // attendantMinimumMinutes: { type: Number, default: 2 },
		public bool IsEnableAttendantWorkflow { get; set; } = false; // isEnableAttendantWorkflow: { type: Boolean, default: false },
		public bool IsEnableExplicitAttendantWorkflow { get; set; } = false; // isEnableExplicitAttendantWorkflow: { type: Boolean, default: false },
		public string ExplicitWorkflowVacant { get; set; } = "VHCI"; // explicitWorkflowVacant: { type: String, default: 'VHCI' },
		public string ExplicitWorkflowOccupied { get; set; } = "OHC"; // explicitWorkflowOccupied: { type: String, default: 'OHC' },
		public bool IsAttendantPriorityPN { get; set; } = false; // isAttendantPriorityPN: { type: Boolean, default: false },
		public bool IsAttendantNormalPN { get; set; } = false; // isAttendantNormalPN: { type: Boolean, default: false },
		public bool IsAttendantNotificationsPN { get; set; } = false; // isAttendantNotificationsPN: { type: Boolean, default: false },
		public bool IsDisablePMSNotesAttendant { get; set; } = false; // isDisablePMSNotesAttendant: { type: Boolean, default: false },
		public bool IsDisablePMSNotesAttendantDep { get; set; } = false; // isDisablePMSNotesAttendantDep: { type: Boolean, default: false },
		public bool IsDisableAttendantPauseLimit { get; set; } = false; // isDisableAttendantPauseLimit: { type: Boolean, default: false },
		public bool IsDisableAttendantCleaningLimit { get; set; } = false; // isDisableAttendantCleaningLimit: { type: Boolean, default: false },
		public bool IsShowCreditsMain { get; set; } = false; // isShowCreditsMain: { type: Boolean, default: false },
		public bool IsEnableVoucherDnd { get; set; } = false; // isEnableVoucherDnd: { type: Boolean, default: false },
		public bool IsHideAttendantMainFilter { get; set; } = false; // isHideAttendantMainFilter: { type: Boolean, default: false },
		public bool IsHideAttendantCredits { get; set; } = false; // isHideAttendantCredits: { type: Boolean, default: false },
		public bool IsHideAttendantCategory { get; set; } = false; // isHideAttendantCategory: { type: Boolean, default: false },
		public bool IsHideAttendantDescription { get; set; } = false; // isHideAttendantDescription: { type: Boolean, default: false },
		public bool IsHideAttendantNotes { get; set; } = false; // isHideAttendantNotes: { type: Boolean, default: false },
		public bool IsHideAttendantGallery { get; set; } = false; // isHideAttendantGallery: { type: Boolean, default: false },
		public bool IsHideAttendantLF { get; set; } = false; // isHideAttendantLF: { type: Boolean, default: false },
		public bool IsHideAttendantInventory { get; set; } = false; // isHideAttendantInventory: { type: Boolean, default: false },
		public bool IsHideAttendantMaintenance { get; set; } = false; // isHideAttendantMaintenance: { type: Boolean, default: false },
		public bool IsDisableAttendantFinishCancel { get; set; } = false; // isDisableAttendantFinishCancel: { type: Boolean, default: false },
		public bool IsRequireAttendantInventoryConfirmation { get; set; } = false; // isRequireAttendantInventoryConfirmation: { type: Boolean, default: false },
		public bool IsDisableAttendantMarkExtras { get; set; } = false; // isDisableAttendantMarkExtras: { type: Boolean, default: false },
		public bool IsDisableAttendantCreateNotes { get; set; } = false; // isDisableAttendantCreateNotes: { type: Boolean, default: false },
		public bool IsShowAttendantTasks { get; set; } = false; // isShowAttendantTasks: { type: Boolean, default: false },
		public bool IsEnableAttendantRecentTasks { get; set; } = false; // isEnableAttendantRecentTasks: { type: Boolean, default: false },
		public bool IsDisableAttendantTurndown { get; set; } = false; // isDisableAttendantTurndown: { type: Boolean, default: false },
		public bool IsDisableAttendantExperience { get; set; } = false; // isDisableAttendantExperience: { type: Boolean, default: false },
		public bool IsEnableAttendantAudits { get; set; } = false; // isEnableAttendantAudits: { type: Boolean, default: false },
		public bool IsEnableAttendantAuditsCreate { get; set; } = false; // isEnableAttendantAuditsCreate: { type: Boolean, default: false },
		public bool IsEnableAttendantPlannings { get; set; } = false; // isEnableAttendantPlannings: { type: Boolean, default: false },
					
		// Maintenance
		public bool IsMaintenancePriorityPN { get; set; } = false; // isMaintenancePriorityPN: { type: Boolean, default: false },
		public bool IsMaintenanceNormalPN { get; set; } = false; // isMaintenanceNormalPN: { type: Boolean, default: false },
		public bool IsMaintenanceLitePN { get; set; } = false; // isMaintenanceLitePN: { type: Boolean, default: false },
		public bool IsMaintenanceNotificationsPN { get; set; } = false; // isMaintenanceNotificationsPN: { type: Boolean, default: false },
		public bool IsDisableMaintenanceLiteTasks { get; set; } = false; // isDisableMaintenanceLiteTasks: { type: Boolean, default: false },
		public bool IsRequireMaintenanceActionTasks { get; set; } = false; // isRequireMaintenanceActionTasks: { type: Boolean, default: false },
		public bool IsMaintenanceFiltersPersist { get; set; } = false; // isMaintenanceFiltersPersist: { type: Boolean, default: false },
		public bool IsDutyEnabled { get; set; } = false; // isDutyEnabled: { type: Boolean, default: false },
		public bool IsDutyBackup { get; set; } = false; // isDutyBackup: { type: Boolean, default: false },
		public bool IsDisableMaintenanceExperience { get; set; } = false; // isDisableMaintenanceExperience: { type: Boolean, default: false },
		
		// Runner
		public bool IsDisableRunnerTurndown { get; set; } = false; // isDisableRunnerTurndown: { type: Boolean, default: false },
		public bool IsDisableRunnerExperience { get; set; } = false; // isDisableRunnerExperience: { type: Boolean, default: false },
		public bool IsRequireRunnerInventoryConfirmation { get; set; } = false; // isRequireRunnerInventoryConfirmation: { type: Boolean, default: false },
		public bool IsDisableRunnerAudits { get; set; } = false; // isDisableRunnerAudits: { type: Boolean, default: false },
		public bool IsEnableRunnerInspection { get; set; } = false; // isEnableRunnerInspection: { type: Boolean, default: false },
		public bool IsEnableRunnerExtra { get; set; } = false; // isEnableRunnerExtra: { type: Boolean, default: false },
		public bool IsDisableRunnerMarkExtras { get; set; } = false; // isDisableRunnerMarkExtras: { type: Boolean, default: false },
		public bool IsEnableRunnerRecentTasks { get; set; } = false; // isEnableRunnerRecentTasks: { type: Boolean, default: false },
		public bool IsEnableShowNonPlanned { get; set; } = false; // isEnableShowNonPlanned: { type: Boolean, default: false },
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class PermissionForMobile
	{
		public string Name { get; set; } = "NULL permission"; // name: { type: String },
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
		public HotelForMobile Hotel { get; set; } = null;
		public UserForMobile[] Users { get; set; } = new UserForMobile[0]; // users: [ { type: ObjectId, ref: 'User' } ],
		public bool IsMaintenanceViewable { get; set; } = false; // isMaintenanceViewable: { type: Boolean, default: false },
		public bool IsHousekeepingViewable { get; set; } = false; // isHousekeepingViewable: { type: Boolean, default: false },
		public bool IsConciergeViewable { get; set; } = false; // isConciergeViewable: { type: Boolean, default: false },
		public bool IsPlanningViewable { get; set; } = false; // isPlanningViewable: { type: Boolean, default: false },
		public bool IsDashboardViewable { get; set; } = false; // isDashboardViewable: { type: Boolean, default: false },
		public bool IsQuestionnairesViewable { get; set; } = false; // isQuestionnairesViewable: { type: Boolean, default: false },
		public bool IsInventoryViewable { get; set; } = false; // isInventoryViewable: { type: Boolean, default: false },
		public bool IsLostFoundViewable { get; set; } = false; // isLostFoundViewable: { type: Boolean, default: false },
		public bool IsExperiencesViewable { get; set; } = false; // isExperiencesViewable: { type: Boolean, default: false },
		public bool IsHistoryViewable { get; set; } = false; // isHistoryViewable: { type: Boolean, default: false },
		public bool IsAssetsViewable { get; set; } = false; // isAssetsViewable: { type: Boolean, default: false },
		public bool IsCatalogViewable { get; set; } = false; // isCatalogViewable: { type: Boolean, default: false },
		public bool IsRoomsViewable { get; set; } = false; // isRoomsViewable: { type: Boolean, default: false },
		public bool IsUsersViewable { get; set; } = false; // isUsersViewable: { type: Boolean, default: false },
		public bool IsGroupsViewable { get; set; } = false; // isGroupsViewable: { type: Boolean, default: false },
		public bool IsPermissionsViewable { get; set; } = false; // isPermissionsViewable: { type: Boolean, default: false },
		public bool IsSettingsViewable { get; set; } = false; // isSettingsViewable: { type: Boolean, default: false },
		public bool IsDashboardTasksViewable { get; set; } = false; // isDashboardTasksViewable: { type: Boolean, default: false },
		public bool IsDashboardInventoryViewable { get; set; } = false; // isDashboardInventoryViewable: { type: Boolean, default: false },
		public bool IsDashboardAttendantViewable { get; set; } = false; // isDashboardAttendantViewable: { type: Boolean, default: false },
		public bool IsDashboardAuditViewable { get; set; } = false; // isDashboardAuditViewable: { type: Boolean, default: false },
		public bool IsDashboardExperiencesViewable { get; set; } = false; // isDashboardExperiencesViewable: { type: Boolean, default: false },
		public bool IsDashboardTasksReportViewable { get; set; } = false; // isDashboardTasksReportViewable: { type: Boolean, default: false },
		public bool IsDashboardAttendantReportViewable { get; set; } = false; // isDashboardAttendantReportViewable: { type: Boolean, default: false },
		public bool IsExperiencesArchiveEnabled { get; set; } = false; // isExperiencesArchiveEnabled: { type: Boolean, default: false },
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class VipCodesForMobile
	{
		public string Label { get; set; } = "NULL vip code"; // label: { type: String, required: true, validate: labelValidations },
		public string Code { get; set; } = ""; // code: { type: String, required: true },
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
		public HotelForMobile Hotel { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// DEPRECATED. NOT USED. NOT SUPPORTED.
	/// </summary>
	public class RoomCatalogForMobile
	{
		public string Image { get; set; } = null; // image: String,
		public Guid RoomId { get; set; } // roomId: { type: ObjectId, ref: 'Room' },
		public RoomForMobile Room { get; set; }
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
		public HotelForMobile Hotel { get; set; } = null;
		public string Comment { get; set; } = ""; // comment: { type: String, default: '' },
		public string Description { get; set; } = ""; // description: { type: String, default: '' },
		public bool IsPdf { get; set; } = false; // isPdf: { type: Boolean, default: false },
		public DateTime LastDate { get; set; } = DateTime.UtcNow; // lastDate: { type: Date, default: Date.now() }
	}

	/// <summary>
	/// Completed.
	/// DEPRECATED. NOT USED. NOT SUPPORTED.
	/// </summary>
	public class RoomAccessForMobile 
	{
		public string Label { get; set; } = "NULL room access"; // label: { type: String, required: true, validate: labelValidations },
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true }
		public HotelForMobile Hotel { get; set; } = null;
		public bool HasHousekeeping { get; set; } = true; // hasHousekeeping: { type: Boolean, default: true },
		public bool IsPMSDisabled { get; set; } = false; // isPMSDisabled: { type: Boolean, default: false },
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomAreaForMobile
	{
		public string Label { get; set; } = null; // label: { type: String, required: true, validate: labelValidations },
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true },
		public HotelForMobile Hotel { get; set; } = null;
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomNoteForMobile
	{
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel' },
		public HotelForMobile Hotel { get; set; } = null;
		public Guid RoomId { get; set; } = Guid.Empty; // roomId: { type: ObjectId, ref: 'Room' },
		public RoomForMobile Room { get; set; } = null;
		public Guid UserId { get; set; } = Guid.Empty; // userId: { type: ObjectId, ref: 'User' },
		public UserForMobile User { get; set; } = null;
		public string Note { get; set; } = null; // note: String,
		public DateTime LastDate { get; set; } = DateTime.UtcNow; // lastDate: { type: Date, default: Date.now() }
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class UserForMobile
	{
		/// <summary>
		/// WARNING _id CHANGED TO id.
		/// </summary>
		public Guid Id { get; set; } // "_id": "55d06bab99295b3a52000085",
		public string Email { get; set; } // "email": "tgodicheau@leportroyal.com",
		public string Username { get; set; } // "username": "marieh",
		public string First_name { get; set; } // "first_name": "Marie-Helene",
		public string Last_name { get; set; } // "last_name": "Poulin",
		/// <summary>
		/// Currently not supported. Always empty string.
		/// </summary>
		public string Organization { get; set; } // "organization": null,

		/// <summary>
		/// WARNING: THIS PROPERTY SHOULD BE DEPRECATED. Users are no longer a part of a single hotel but a hotel group 
		/// which can have multiple hotels. Use data from AvailableHotels property if you need info about the hotels, and use hotel group properties: HotelGroupId, HotelGroupKey, HotelGroupName.
		/// Set to HotelGroupId value.
		/// </summary>
		public string Hotel { get; set; } // "hotel": "55d06baa99295b3a52000000",
		/// <summary>
		/// WARNING: THIS PROPERTY SHOULD BE DEPRECATED. Users are no longer a part of a single hotel but a hotel group 
		/// which can have multiple hotels. Use data from AvailableHotels property if you need info about the hotels, and use hotel group properties: HotelGroupId, HotelGroupKey, HotelGroupName.
		/// Set to HotelGroupKey value.
		/// </summary>
		public string HotelUsername { get; set; } // "hotelUsername": "rcfr",

		public bool HotelUsernameRequired { get; set; } // "hotelUsernameRequired": true,

		/// <summary>
		/// Avatar image URL - Regular size
		/// </summary>
		public string Image { get; set; } // "image": "https://www.filepicker.io/api/file/5JdEsj9LS8azEAdobNy0",
		/// <summary>
		/// Avatar image URL - Thumbnail size
		/// </summary>
		public string Thumbnail { get; set; } // "thumbnail": "https://www.filepicker.io/api/file/StHPyEXT5Gt96D8cyR8k",
		/// <summary>
		/// Currently not supported. Always false.
		/// </summary>
		public bool IsBypassAttendant { get; set; } // "isBypassAttendant": false,
		/// <summary>
		/// Currently not supported. Always true.
		/// </summary>
		public bool IsOnDuty { get; set; } // "isOnDuty": false,
		/// <summary>
		/// Currently not supported. Always empty array.
		/// </summary>
		public string[] Groups { get; set; } // "groups": [],
		/// <summary>
		/// Currently not supported. Always "en" value.
		/// </summary>
		public string Language { get; set; } // "language": "en",
		/// <summary>
		/// User status - "active" or "inactive"
		/// </summary>
		public string Status { get; set; } // "status": "active"

		/// <summary>
		/// Users postal code. Currently not supported. Always empty string.
		/// </summary>
		public string Zip { get; set; } // "zip": "",
		/// <summary>
		/// Users country. Currently not supported. Always empty string.
		/// </summary>
		public string Country { get; set; } // "country": "",
		/// <summary>
		/// Users coutry state. Currently not supported. Always empty string.
		/// </summary>
		public string State { get; set; } // "state": "",
		/// <summary>
		/// Users city. Currently not supported. Always empty string.
		/// </summary>
		public string City { get; set; } // "city": "",
		/// <summary>
		/// Users street address. Currently not supported. Always empty string.
		/// </summary>
		public string Street { get; set; } // "street": "",


		// Boolean properties below should be modified to conform with our new role/permission system
		// Boolean properties below should be modified to conform with our new role/permission system
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsSuperAdmin { get; set; } // "isSuperAdmin": false,
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsHost { get; set; } // "isHost": false,
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsFoodBeverage { get; set; } // "isFoodBeverage": false,
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsRoomRunner { get; set; } // "isRoomRunner": false,
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsRoomsService { get; set; } // "isRoomsServices": false,
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsReceptionist { get; set; } // "isReceptionist": false,
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsMaintenance { get; set; } // "isMaintenance": false,
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsInspector { get; set; } // "isInspector": false,
		/// <summary>
		/// Always true. Will be deprecated in the future.
		/// </summary>
		public bool IsAttendant { get; set; } // "isAttendant": true,
		/// <summary>
		/// Not supported. Always false. Will be deprecated in the future.
		/// </summary>
		public bool IsAdministrator { get; set; } // "isAdministrator": false,
												  // Boolean properties above should be modified to conform with our new role/permission system
												  // Boolean properties above should be modified to conform with our new role/permission system

		// Properties below are newly added to the user class and should be handled on the front end properly
		// Properties below are newly added to the user class and should be handled on the front end properly
		public Guid HotelGroupId { get; set; }
		public string HotelGroupKey { get; set; }
		public string HotelGroupName { get; set; }
		public AvailableHotelForMobile[] AvailableHotels { get; set; }
		public Guid? UserGroupId { get; set; }
		public string UserGroupName { get; set; }
		public Guid? UserSubGroupId { get; set; }
		public string UserSubGroupName { get; set; }
		public Guid RoleId { get; set; }
		public string RoleName { get; set; }
		// Properties above are newly added to the user class and should be handled on the front end properly
		// Properties above are newly added to the user class and should be handled on the front end properly


		/// <summary>
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// </summary>
		public int Role { get; set; }
		/// <summary>
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// </summary>
		public string EmployeeId { get; set; } // "employeeId": "",
		/// <summary>
		/// Currently not supported. Always null.
		/// </summary>
		public string AppVersion { get; set; } // "appVersion": "",
		/// <summary>
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// </summary>
		public string[] Permissions { get; set; } // "permissions": ["605b3226e0767406e4f70c2d"],
		/// <summary>
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// </summary>
		public string Salt { get; set; } // "salt": "050e3190afa263f6d6549959eeb0dd01",
		/// <summary>
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// DEPRECATED! NOT USED ANY MORE!
		/// </summary>
		public string Hashed_password { get; set; } // "hashed_password": "4ce0eef7572421617eec4e27357bcf03c2e12a49",
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class AvailableHotelForMobile
	{
		public string HotelId { get; set; }
		public string HotelName { get; set; }
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class RoomForMobile
	{
		public string Name { get; set; } = "NULL room"; // name: { type: String },
		public string PmsName { get; set; } = null; // pmsName: { type: String, default: null },
		public string Description { get; set; } = null; // description: String,
		public string HotelId { get; set; } = null; // hotelId: { type: ObjectId, ref: 'Hotel', index: true },
		public HotelForMobile Hotel { get; set; } = null;
		public AssetForMobile[] Assets { get; set; } = new AssetForMobile[0]; // assets:[ { type: ObjectId, ref: 'AssetRoom' } ],
		public RoomCatalogForMobile[] Catalogs { get; set; } = new RoomCatalogForMobile[0]; // catalogs:[ { type: ObjectId, ref: 'RoomCatalog' }],
		public FloorForMobile Floor { get; set; } = null; // floor: { type: ObjectId, ref: 'Floor' },
		public Guid? RoomAccessId { get; set; } = null;
		public RoomAccessForMobile RoomAccess { get; set; } = null; // roomAccess: { type: ObjectId, ref: 'RoomAccess' },
		public Guid? RoomCategoryId { get; set; } = null;
		public RoomCategoryForMobile RoomCategory { get; set; } = null; // roomCategory: { type: ObjectId, ref: 'RoomCategory' },
		public Guid? RoomStatusId { get; set; } = null;
		public RoomStatusForMobile RoomStatus { get; set; } = null; // roomStatus: { type: ObjectId, ref: 'RoomStatus' },
		public Guid? RoomMaintenanceId { get; set; } = null;
		public RoomMaintenanceForMobile RoomMaintenance { get; set; } = null; // roomMaintenance: { type: ObjectId, ref: 'RoomMaintenance' },
		public Guid? RoomHousekeepingId { get; set; } = null;
		public RoomHousekeepingForMobile RoomHousekeeping { get; set; } = null; // roomHousekeeping: { type: ObjectId, ref: 'RoomHousekeeping' },
		public RoomAreaForMobile[] RoomAreas { get; set; } = new RoomAreaForMobile[0]; // roomAreas:[ { type: ObjectId, ref: 'RoomArea', require: false } ],
		public string Comment { get; set; } = ""; // comment: { type: String, default: "" },
		public Guid? RoomNoteId { get; set; } = null;
		public RoomNoteForMobile RoomNote { get; set; } = null; // roomNote: { type: ObjectId, ref: 'RoomNote' },
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
		public Guid? ExternalInfoId { get; set; } = null;
		public RoomExternalInfoForMobile ExternalInfo { get; set; } = null; // externalInfo: { ... },
		public RoomMessageForMobile[] Messages { get; set; } = new RoomMessageForMobile[0]; // messages: [...],
		public RoomExtraForMobile[] Extras { get; set; } = new RoomExtraForMobile[0]; // extras: [...]


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
	}

	/// <summary>
	/// Completed.
	/// </summary>
	public class FloorForMobile
	{
		public Guid Id { get; set; } = Guid.Empty;
		public string Number { get; set; } = "N/A";
		public string Description { get; set; } = "NULL floor";

		public RoomForMobile[] Rooms { get; set; } = new RoomForMobile[0];
		public string HotelId { get; set; } = null;
		public HotelForMobile Hotel { get; set; } = null;
		public string LastAction { get; set; } = null;
		public DateTime LastDate { get; set; } = DateTime.UtcNow;
	}

}
