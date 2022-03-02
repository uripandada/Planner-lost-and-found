using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Queries.GetTaskDetails
{
	public class TaskImageData
	{
		public string ImageUrl { get; set; }
		public string FileName { get; set; }
		public Guid FileId { get; set; }
	}

	public class TaskDetailsBriefHistoryItem
	{
		public string UserName { get; set; }
		public Guid? UserId { get; set; }
		public string Message { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
	}

	public class TaskDetailsActionData
	{
		public string ActionName { get; set; }
		public string AssetName { get; set; }
		public string AssetGroupName { get; set; }
		public Guid AssetId { get; set; }
		public Guid AssetGroupId { get; set; }
		public int AssetQuantity { get; set; }
		public string AssetImageUrl { get; set; }
	}

	public class TaskDetailsData
	{
		public Guid Id { get; set; }
		
		public IEnumerable<TaskDetailsActionData> Actions { get; set; }

		public string TypeKey { get; set; }
		public string TypeDescription { get; set; }
		public string RepeatsForKey { get; set; }
		public string RecurringTypeKey { get; set; }
		public bool MustBeFinishedByAllWhos { get; set; }



		public int Credits { get; set; }
		public decimal Price { get; set; }
		public string PriorityKey { get; set; }
		public bool IsGuestRequest { get; set; }
		public bool IsShownInNewsFeed { get; set; }
		public bool IsRescheduledEveryDayUntilFinished { get; set; }
		public bool IsMajorNotificationRaisedWhenFinished { get; set; }
		public bool IsBlockingCleaningUntilFinished { get; set; }


		public bool IsForPlannedAttendant { get; set; }
		public Guid? UserId { get; set; }
		public string UserFullName { get; set; }
		public string UserUsername { get; set; }
		public string UserInitials { get; set; }
		public string UserAvatarImageUrl { get; set; }

		//public Guid? RoomId { get; set; }
		//public string RoomName { get; set; }
		//public bool IsRoomOccupied { get; set; }

		//public Guid? BuildingId { get; set; }
		//public string BuildingName { get; set; }
		//public string HotelId { get; set; }
		//public string HotelName { get; set; }
		//public Guid? FloorId { get; set; }
		//public string FloorName { get; set; }
		//public string ReservationId { get; set; }



		public string FromReservationId { get; set; }
		public string FromReservationGuestName { get; set; }
		public Guid? FromWarehouseId { get; set; }
		public string FromWarehouseName { get; set; }
		public Guid? FromRoomId { get; set; }
		public string FromRoomName { get; set; }
		public string FromHotelId { get; set; }
		public string FromHotelName { get; set; }
		//public string FromName { get; set; }

		public string ToReservationId { get; set; }
		public string ToReservationGuestName { get; set; }
		public Guid? ToWarehouseId { get; set; }
		public string ToWarehouseName { get; set; }
		public Guid? ToRoomId { get; set; }
		public string ToRoomName { get; set; }
		public string ToHotelId { get; set; }
		public string ToHotelName { get; set; }
		//public string ToName { get; set; }



		public string WhereTypeKey { get; set; }
		public string Comment { get; set; }

		public string UserAvatarUrl { get; set; }
		public string AssetMainImageUrl { get; set; }

		public string EventModifierKey { get; set; }
		public string EventKey { get; set; }
		public string EventTimeKey { get; set; }

		public string StatusKey { get; set; }
		public string StatusDescription { get; set; }
		public DateTime StartsAt { get; set; }
		public string When { get; set; }

		public Guid SystemTaskConfigurationId { get; set; }

		public bool IsManuallyModified { get; set; }

		public string CreatedByUserFullName { get; set; }
		public DateTimeOffset CreatedAt { get; set; }

		public TaskImageData[] Images { get; set; }
		public string[] FilestackImageUrls { get; set; }

		public IEnumerable<TaskDetailsBriefHistoryItem> BriefHistory { get; set; }
	}

	public class GetTaskDetailsQuery : IRequest<TaskDetailsData>
	{
		public Guid Id { get; set; }
	}

	public class GetTaskDetailsQueryHandler : IRequestHandler<GetTaskDetailsQuery, TaskDetailsData>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly IFileService _fileService;

		public GetTaskDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._fileService = fileService;
		}

		public async Task<TaskDetailsData> Handle(GetTaskDetailsQuery request, CancellationToken cancellationToken)
		{
			var d = await this._databaseContext
				.SystemTasks
				.Where(t => t.Id == request.Id)
				.Select(t => new 
				{
					IsForPlannedAttendant = t.IsForPlannedAttendant,
					WindowsTimeZoneId = t.FromHotel != null ? t.FromHotel.WindowsTimeZoneId : t.ToHotel.WindowsTimeZoneId,
					IanaTimeZoneId = t.FromHotel != null ? t.FromHotel.IanaTimeZoneId : t.ToHotel.IanaTimeZoneId,
					Actions = t.Actions.Select(a => new 
					{
						ActionName = a.ActionName,
						AssetId = a.AssetId,
						AssetGroupId = a.AssetGroupId,
						AssetGroupName = a.AssetGroupName,
						AssetName = a.AssetName,
						AssetQuantity = a.AssetQuantity,
					}).ToArray(),
					//BuildingId = t.BuildingId,
					//BuildingName = t.Building.Name,
					EventKey = t.EventKey,
					EventModifierKey = t.EventModifierKey,
					EventTimeKey = t.EventTimeKey,
					//FloorId = t.FloorId,
					//FloorName = t.Floor.Name,
					//HotelId = t.HotelId,
					//HotelName = t.Hotel.Name,
					Id = t.Id,
					IsManuallyModified = t.IsManuallyModified,
					MustBeFinishedByAllWhos = t.MustBeFinishedByAllWhos,
					RecurringTypeKey = t.RecurringTypeKey,
					RepeatsForKey = t.RepeatsForKey,
					//ReservationId = t.ReservationId,
					//RoomId = t.RoomId,
					//RoomName = t.Room.Name,
					StartsAt = t.StartsAt,
					StatusDescription = "",
					StatusKey = t.StatusKey,
					SystemTaskConfigurationId = t.SystemTaskConfigurationId,
					TypeDescription = "",
					TypeKey = t.TypeKey,
					UserFirstName = t.User.FirstName,
					UserLastName = t.User.LastName,
					UserId = t.UserId,
					UserUsername = t.User.UserName,
					When = "",
					WhereTypeKey = t.WhereTypeKey,
					CreatedByUserFullName = $"{t.CreatedBy.FirstName} {t.CreatedBy.LastName}",
					CreatedAt = t.CreatedAt,
					Data = t.SystemTaskConfiguration.Data,
					IsBlockingCleaningUntilFinished = t.IsBlockingCleaningUntilFinished,
					IsGuestRequest = t.IsGuestRequest,
					IsMajorNotificationRaisedWhenFinished = t.IsMajorNotificationRaisedWhenFinished,
					IsRescheduledEveryDayUntilFinished = t.IsRescheduledEveryDayUntilFinished,
					IsShownInNewsFeed = t.IsShownInNewsFeed,
					Credits = t.Credits,
					Price = t.Price,
					PriorityKey = t.PriorityKey,
					//IsRoomOccupied = t.Room.IsOccupied,
					UserAvatarImageUrl = t.User.Avatar.FileUrl,
					t.FromName,
					t.FromHotelId,
					FromHotelName = t.FromHotel.Name,
					t.FromRoomId,
					FromRoomName = t.FromRoomId == null ? null : t.FromRoom.Name,
					t.FromWarehouseId,
					FromWarehouseName = t.FromWarehouseId == null ? null : t.FromWarehouse.Name,
					t.FromReservationId,
					FromReservationGuestName = t.FromReservationId == null ? null : t.FromReservation.GuestName,
					t.ToName,
					t.ToHotelId,
					ToHotelName = t.ToHotel.Name,
					t.ToRoomId,
					ToRoomName = t.ToRoomId == null ? null : t.ToRoom.Name,
					t.ToWarehouseId,
					ToWarehouseName = t.ToWarehouseId == null ? null : t.ToWarehouse.Name,
					t.ToReservationId,
					ToReservationGuestName = t.ToReservationId == null ? null : t.ToReservation.GuestName,
				})
				.FirstOrDefaultAsync();

			var historyItems = await this._databaseContext.SystemTaskHistorys
				.Where(h => h.SystemTaskId == d.Id)
				.OrderByDescending(h => h.CreatedAt)
				.Take(5)
				.Select(h => new TaskDetailsBriefHistoryItem
				{
					CreatedAt = h.CreatedAt,
					Message = h.Message,
					UserId = h.CreatedById,
					UserName = h.CreatedBy.UserName,
				})
				.ToArrayAsync();

			var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(d.WindowsTimeZoneId, d.IanaTimeZoneId);
			var currentHotelTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneConverter.TZConvert.GetTimeZoneInfo(timeZoneId));

			var userFullName = "N/A";
			var userName = "N/A";
			var userInitials = "N/A";
			if (d.UserId.HasValue)
			{
				userFullName = $"{d.UserFirstName} {d.UserLastName}";
				userName = d.UserUsername;
				userInitials = $"{d.UserFirstName[0]}{d.UserLastName[0]}";
			}
			else if (d.IsForPlannedAttendant)
			{
				userFullName = "Planned Attendant";
				userName = "For the cleaner who will do the cleaning";
				userInitials = "*";
			}

			var taskDetails = new TaskDetailsData
			{
				BriefHistory = historyItems,
				Actions = d.Actions.Select(a => new TaskDetailsActionData {
					ActionName = a.ActionName,
					AssetId = a.AssetId,
					AssetGroupId = a.AssetGroupId,
					AssetGroupName = a.AssetGroupName,
					AssetName = a.AssetName,
					AssetQuantity = a.AssetQuantity,
					AssetImageUrl = null
				}).ToArray(),
				CreatedAt = d.CreatedAt,
				CreatedByUserFullName = d.CreatedByUserFullName,
				EventKey = d.EventKey,
				EventModifierKey = d.EventModifierKey,
				EventTimeKey = d.EventTimeKey,
				Id = d.Id,
				Images = new TaskImageData[0],
				FilestackImageUrls = d.Data.FilestackImageUrls == null ? new string[0] : d.Data.FilestackImageUrls.ToArray(),
				//d.Data.Files.Select(i => new TaskImageData
				//{
				//	FileName = i.FileName,
				//	ImageUrl = i.FileUrl,
				//	FileId = i.FileId
				//}).ToArray(),
				IsManuallyModified = d.IsManuallyModified,
				MustBeFinishedByAllWhos = d.MustBeFinishedByAllWhos,
				RecurringTypeKey = d.RecurringTypeKey,
				RepeatsForKey = d.RepeatsForKey,
				StartsAt = d.StartsAt,
				StatusDescription = TaskDescriptions.GetTaskStatusDescription(d.StatusKey),
				StatusKey = d.StatusKey,
				SystemTaskConfigurationId = d.SystemTaskConfigurationId,
				TypeDescription = TaskDescriptions.GetTaskTypeDescription(d.TypeKey, d.RecurringTypeKey, d.EventModifierKey, d.EventKey, true),
				TypeKey = d.TypeKey,
				UserFullName = userFullName,
				UserId = d.UserId,
				IsForPlannedAttendant = d.IsForPlannedAttendant,
				UserUsername = userName,
				When = TaskDescriptions.GetWhen(d.StartsAt, d.TypeKey, d.EventTimeKey, d.EventModifierKey, d.EventKey, currentHotelTime), // TODO: FIX THIS UTC NOW! MUST BE HOTELS CURRENT TIME
				WhereTypeKey = d.WhereTypeKey,
				Comment = d.Data.Comment,
				UserAvatarUrl = null,
				IsBlockingCleaningUntilFinished = d.IsBlockingCleaningUntilFinished,
				IsGuestRequest = d.IsGuestRequest,
				IsMajorNotificationRaisedWhenFinished = d.IsMajorNotificationRaisedWhenFinished,
				IsRescheduledEveryDayUntilFinished = d.IsRescheduledEveryDayUntilFinished,
				IsShownInNewsFeed = d.IsShownInNewsFeed,
				Credits = d.Credits,
				Price = d.Price,
				PriorityKey = d.PriorityKey,
				UserAvatarImageUrl = d.UserAvatarImageUrl,
				UserInitials = userInitials,
				FromHotelId = d.FromHotelId,
				FromHotelName = d.FromHotelName,
				FromReservationId = d.FromReservationId,
				FromRoomId = d.FromRoomId,
				FromWarehouseId = d.FromWarehouseId,
				ToHotelId = d.ToHotelId,
				ToHotelName = d.ToHotelName,
				ToReservationId = d.ToReservationId,
				ToRoomId = d.ToRoomId,
				ToWarehouseId = d.ToWarehouseId,
				FromReservationGuestName = d.FromReservationGuestName,
				FromRoomName = d.FromRoomName,
				FromWarehouseName = d.FromWarehouseName,
				ToReservationGuestName = d.ToReservationGuestName,
				ToRoomName = d.ToRoomName,
				ToWarehouseName = d.ToWarehouseName,
			};

			var assetIds = taskDetails.Actions.Select(a => a.AssetId).ToArray();

			var images = await this._databaseContext.AssetFiles.Where(af => assetIds.Contains(af.AssetId) && af.IsPrimaryImage == true).Select(af => new { af.File.FileName, af.AssetId }).ToArrayAsync();

			if (images.Any())
			{
				foreach(var image in images)
				{
					var fileUrl = this._fileService.GetAssetFileUrl(image.AssetId, image.FileName);
					var actions = taskDetails.Actions.Where(a => a.AssetId == image.AssetId).ToArray();
					foreach(var action in actions)
					{
						action.AssetImageUrl = fileUrl;
					}
				}
			}

			return taskDetails;
		}
	}

	//public static class TaskDescriptions
	//{
	//	public static string GetTaskStatusDescription(string statusKey)
	//	{
	//		switch (statusKey)
	//		{
	//			case nameof(TaskStatusType.NEW):
	//				return "New";
	//			case nameof(TaskStatusType.COMPLETED):
	//				return "Completed";
	//			case nameof(TaskStatusType.PAUSED):
	//				return "Paused";
	//			case nameof(TaskStatusType.REVIEWED):
	//				return "Reviewed";
	//			case nameof(TaskStatusType.STARTED):
	//				return "Started";
	//			default:
	//				return "Unknown status";
	//		}

	//	}
	//}
}
