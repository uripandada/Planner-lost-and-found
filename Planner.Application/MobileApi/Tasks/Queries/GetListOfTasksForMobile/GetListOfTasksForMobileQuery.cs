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

namespace Planner.Application.MobileApi.Tasks.Queries.GetListOfTasksForMobile
{
	public class MobileTask
	{
		public Guid Id { get; set; }
		public long Date_ts { get; set; }
		public long Last_ts { get; set; }
		public string Hotel_id { get; set; }
		public Guid Creator_id { get; set; }
		public Guid Uuid { get; set; }
		public Guid? Group_uuid { get; set; }
		public string Task { get; set; }
		/// <summary>
		/// quick, action, confirmation, notification
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// JSON field
		/// </summary>
		public MobileTaskMeta Meta { get; set; }
		/// <summary>
		/// JSON field
		/// </summary>
		public MobileTaskGuestInfo Guest_info { get; set; }
		/// <summary>
		/// JSON field
		/// </summary>
		public MobileTaskAssigned Assigned { get; set; }
		public Guid Responsible_id { get; set; }
		public string Responsible_first_name { get; set; }
		public string Responsible_last_name { get; set; }
		/// <summary>
		///  YYYY-MM-DD
		/// </summary>
		public string Due_date { get; set; }
		/// <summary>
		///  YYYY-MM-DD
		/// </summary>
		public string Start_date { get; set; } 
		public long? Due_ts { get; set; }
		/// <summary>
		/// JSON field
		/// </summary>
		public IEnumerable<MobileTaskMessage> Messages { get; set; }
		/// <summary>
		/// JSON field
		/// </summary>
		public IEnumerable<MobileTaskStatusUpdateHistory> Updates { get; set; }
		/// <summary>
		/// JSON field
		/// </summary>
		public string Chain { get; set; }
		public bool Is_claimed { get; set; }
		public bool Is_rejected { get; set; }
		public bool Is_started { get; set; }
		public bool Is_completed { get; set; }
		public bool Is_paused { get; set; }
		public bool Is_cancelled { get; set; }
		public bool Is_required { get; set; }
		public bool Is_optional { get; set; }
		public bool Is_priority { get; set; }
		public bool Is_group { get; set; }
		public bool Is_hidden { get; set; }
		public string Comment { get; set; }
		public IEnumerable<string> Image_urls { get; set; }
		public bool Is_guest_request { get; set; }
	}

	public class MobileTaskStatusUpdateHistory
	{
		public Guid User_id { get; set; }
		public string Update_type { get; set; }
		public string Status { get; set; }
	}

	public class MobileTaskMessage
	{
		public string Message { get; set; }
		public Guid User_id { get; set; }
		public long Date_ts { get; set; }
	}

	public class MobileTaskMeta
	{
		public bool IsBlocking { get; set; }
		public bool IsHousekeeping { get; set; }
		public bool IsConcierge { get; set; }
		public bool IsMaintenance { get; set; }
		/// <summary>
		/// always null
		/// </summary>
		public string EstimatedTime { get; set; }
		public Guid CreatorId { get; set; }
		public MobileTaskMetaAction Action { get; set; }
		/// <summary>
		/// Building name + floor name
		/// </summary>
		public string Location { get; set; }
		public Guid Room_id { get; set; }
	}
	
	public class MobileTaskMetaAction
	{
		/// <summary>
		/// Never used--- replace with something else?
		/// </summary>
		public int Id { get; set; }
		public string Hotel_id { get; set; }
		public long Date_ts { get; set; }
		public long Creator_id { get; set; }
		public string Label { get; set; }
		public MobileTaskMetaActionBody Body { get; set; }
		/// <summary>
		/// Contains "1
		/// </summary>
		public int Is_active { get; set; }
		public bool IsSelected { get; set; }
		
	}
	
	public class MobileTaskMetaActionBody
	{
		public string Task_type { get; set; }
		public bool Is_Mandatory { get; set; }
	}

	public class MobileTaskAssigned
	{
		public bool Is_mandatory { get; set; }
		public bool IsPlannedAttendant { get; set; }
		public bool IsPlannedRunner { get; set; }
		public IEnumerable<Guid> User_ids { get; set; }
		/// <summary>
		/// Comma separated list of "{firstName} {lastName}"
		/// </summary>
		public string Label { get; set; }
	}

	public class MobileTaskGuestInfo
	{
		public string Guest_name { get; set; }
	}

	public class GetListOfTasksForMobileQuery: IRequest<IEnumerable<MobileTask>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfTasksForMobileQueryHandler : IRequestHandler<GetListOfTasksForMobileQuery, IEnumerable<MobileTask>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly string _roleName;

		public GetListOfTasksForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._roleName = contextAccessor.RoleName();
		}

		public async Task<IEnumerable<MobileTask>> Handle(GetListOfTasksForMobileQuery request, CancellationToken cancellationToken)
		{
			var dateProvider = new HotelLocalDateProvider();
			var date = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, false);

			// Inspectors get all tasks??
			var isInspector = this._roleName == SystemDefaults.Roles.Inspector.Name;

			// load only todays tasks!
			var tasks = await this._databaseContext.SystemTasks
				.Include(t => t.SystemTaskConfiguration)
				.Include(t => t.Actions)
				.Include(t => t.Messages)
				.Include(t => t.ToReservation)
				.Include(t => t.ToRoom)
				.ThenInclude(r => r.Floor)
				.Include(t => t.User)
				.Where(t => 
				(t.UserId == this._userId || isInspector) &&
				(t.ToRoomId != null || t.ToReservationId != null) &&
				t.StartsAt.Date == date &&
				((t.FromHotel != null && t.FromHotelId == request.HotelId) || (t.ToHotelId != null && t.ToHotelId == request.HotelId))
			).ToListAsync();

			var result = new List<MobileTask>();

			var hotelIds = new HashSet<string>();
			foreach(var task in tasks)
			{
				if (task.ToHotelId.IsNotNull() && !hotelIds.Contains(task.ToHotelId))
				{
					hotelIds.Add(task.ToHotelId);
				}
				if (task.FromHotelId.IsNotNull() && !hotelIds.Contains(task.FromHotelId))
				{
					hotelIds.Add(task.FromHotelId);
				}
			}

			var hotels = await this._databaseContext.Hotels.Where(h => hotelIds.Contains(h.Id)).ToListAsync();
			var hotelTimeZonesMap = new Dictionary<string, TimeZoneInfo>();
			foreach(var hotel in hotels)
			{
				hotelTimeZonesMap.Add(hotel.Id, TimeZoneInfo.FindSystemTimeZoneById(HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId)));
			}

			foreach(var t in tasks)
			{
				var taskDescription = string.Join(", ", t.Actions.Select(a => $"{a.ActionName} {a.AssetQuantity} x {a.AssetName}").ToArray());

				var item = new MobileTask
				{
					Assigned = new MobileTaskAssigned
					{
						IsPlannedAttendant = true,
						IsPlannedRunner = false,
						Is_mandatory = true,
						Label = t.IsForPlannedAttendant ? "Planned attendant" : (t.User == null ? "N/A" : $"{t.User.FirstName} {t.User.LastName}"),
						User_ids = new Guid[] { t.User.Id },
					},
					Chain = null,
					Creator_id = t.CreatedById ?? Guid.Empty,
					Date_ts = t.ToHotelId.IsNull() ? t.StartsAt.ToUnixTimeStamp() : (TimeZoneInfo.ConvertTimeToUtc(t.StartsAt, hotelTimeZonesMap[t.ToHotelId])).ToUnixTimeStamp(),
					Due_date = null,
					Due_ts = null,
					Group_uuid = null,
					Guest_info = new MobileTaskGuestInfo
					{
						Guest_name = t.ToReservation == null ? "" : t.ToReservation.GuestName
					},
					Last_ts = t.ModifiedAt.ToUnixTimeStamp(),
					Messages = t.Messages.Select(m => new MobileTaskMessage 
					{ 
						Date_ts = m.CreatedAt.ToUnixTimeStamp(),
						Message = m.Message,
						User_id = m.CreatedById ?? Guid.Empty,
					}).ToArray(),
					Meta = new MobileTaskMeta
					{
						CreatorId = t.CreatedById ?? Guid.Empty,
						EstimatedTime = null,
						IsBlocking = false,
						IsConcierge = false,
						IsHousekeeping = true,
						IsMaintenance = false,
						Room_id = t.ToRoomId.HasValue ? t.ToRoomId.Value : Guid.Empty,
						Location = t.ToRoomId.HasValue && t.ToRoom.Floor != null ? t.ToRoom.Floor.Name : "",
					},
					Responsible_first_name = t.User.FirstName,
					Responsible_id = t.User.Id,
					Responsible_last_name = t.User.LastName,
					Start_date = t.StartsAt.ToString("yyyy-MM-dd"),
					Task = taskDescription,
					Type = "quick",
					Uuid = t.Id,
					Hotel_id = t.ToHotelId,
					Id = t.Id,
					Updates = new MobileTaskStatusUpdateHistory[0],
					Is_required = true,
					Is_optional = false,
					Is_hidden = false,
					Is_group = false,
					Is_priority = false,

					Comment = t.Comment,

					Image_urls = (t.SystemTaskConfiguration?.Data?.FilestackImageUrls?.Any() ?? false) ? t.SystemTaskConfiguration.Data.FilestackImageUrls : new string[0],
					Is_guest_request = t.IsGuestRequest,

					Is_rejected = t.StatusKey == nameof(Common.Enums.TaskStatusType.REJECTED),
					Is_claimed = t.StatusKey == nameof(Common.Enums.TaskStatusType.WAITING),
					Is_cancelled = t.StatusKey == nameof(Common.Enums.TaskStatusType.CANCELLED),
					Is_completed = t.StatusKey == nameof(Common.Enums.TaskStatusType.FINISHED),
					Is_paused = t.StatusKey == nameof(Common.Enums.TaskStatusType.PAUSED),
					Is_started = t.StatusKey == nameof(Common.Enums.TaskStatusType.STARTED),
				};

				result.Add(item);
			}

			return result;
		}
	}
}