using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Cleanings.Queries.GetListOfCleaningsForMobile;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Cleanings.Queries.GetListOfCleaningsForInspectionForMobile
{
	public class GetListOfCleaningsForInspectionForMobileQuery : IRequest<IEnumerable<CleaningForMobile>>
	{
		public string HotelId { get; set; }
	}
	public class GetListOfCleaningsForInspectionForMobileQueryHandler : IRequestHandler<GetListOfCleaningsForInspectionForMobileQuery, IEnumerable<CleaningForMobile>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly string _roleName;

		public GetListOfCleaningsForInspectionForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._roleName = contextAccessor.RoleName();
		}

		public async Task<IEnumerable<CleaningForMobile>> Handle(GetListOfCleaningsForInspectionForMobileQuery request, CancellationToken cancellationToken)
		{
			var dateProvider = new HotelLocalDateProvider();
			var date = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, false);
			var localHotelDateValue = date.ToString("yyyy-MM-dd");

			var latestRoomCleanings = await this._databaseContext.Cleanings.FromSqlInterpolated($@"
				SELECT 
					last_cleaning_of_group.*
				FROM 
					public.rooms r
					CROSS JOIN LATERAL (
						SELECT 
							c.*
						FROM 
							public.cleanings c
						WHERE 
							c.starts_at::date <= {localHotelDateValue}::date
							AND c.room_id = r.id
							AND c.room_bed_id IS NULL
							AND c.is_active = true
							AND c.is_planned = true
						ORDER BY 
							c.starts_at DESC
						LIMIT 1
					) AS last_cleaning_of_group
				WHERE 
					r.hotel_id = {request.HotelId};
			")
				.ToListAsync();

			var latestRoomBedCleanings = await this._databaseContext.Cleanings.FromSqlInterpolated($@"
				SELECT 
					last_cleaning_of_group.*
				FROM 
					public.room_beds rb
					LEFT JOIN public.rooms r ON rb.room_id = r.id
					CROSS JOIN LATERAL (
						SELECT 
							c.*
						FROM 
							public.cleanings c
						WHERE 
							c.starts_at::date <= {localHotelDateValue}::date
							AND c.room_id = rb.room_id
							AND c.room_bed_id IS NOT NULL
							AND c.room_bed_id = rb.id
							AND c.is_active = true
							AND c.is_planned = true
						ORDER BY 
							c.starts_at DESC
						LIMIT 1
					) AS last_cleaning_of_group
				WHERE 
					r.hotel_id = {request.HotelId};
			")
				.ToListAsync();

			var users = await this._databaseContext.Users.Select(u => new
			{
				u.Id,
				u.FirstName,
				u.LastName,
				u.UserName,
				u.Email
			}).ToDictionaryAsync(u => u.Id);

			var rooms = await this._databaseContext.Rooms
				.Where(r => r.HotelId == request.HotelId)
				.Select(r => new { r.Id, r.Name })
				.ToDictionaryAsync(r => r.Id);
			
			var beds = await this._databaseContext.RoomBeds
				.Where(rb => rb.Room.HotelId == request.HotelId)
				.Select(rb => new { rb.Id, rb.RoomId, rb.Name })
				.ToDictionaryAsync(rb => rb.Id);

			var result = new List<CleaningForMobile>();
			foreach (var c in latestRoomCleanings)
			{
				var cleaner = users[c.CleanerId];
				var room = rooms[c.RoomId];

				var resultCleaning = new CleaningForMobile
				{
					Id = c.Id,
					Assigned_time = null,
					Container_index = null,
					Creator_id = this._userId,
					Credits = c.Credits.HasValue ? c.Credits.Value : 0,
					Date_ts = c.StartsAt.ToUnixTimeStamp(),
					Guest_status = null,
					Hotel_id = request.HotelId,
					Is_change_sheets = c.IsChangeSheets ? 1 : 0,
					Is_priority = c.IsPriority ? 1 : 0,
					Planning_date = c.StartsAt.ToString("yyyy-MM-dd"),
					Planning_user_email = cleaner.Email,
					Planning_user_firstname = cleaner.FirstName,
					Planning_user_id = cleaner.Id,
					Planning_user_lastname = cleaner.LastName,
					Planning_user_username = cleaner.UserName,
					Room_id = room.Id,
					Room_name = room.Name,
					Scheduled_order = null,
					Scheduled_ts = null,
					Name = c.Description,
					Bed_id = null,
					Bed_name = null,
				};

				result.Add(resultCleaning);
			}
			foreach (var c in latestRoomBedCleanings)
			{
				var cleaner = users[c.CleanerId];
				var room = rooms[c.RoomId];
				var roomBed = beds[c.RoomBedId.Value];

				var resultCleaning = new CleaningForMobile
				{
					Id = c.Id,
					Assigned_time = null,
					Container_index = null,
					Creator_id = this._userId,
					Credits = c.Credits.HasValue ? c.Credits.Value : 0,
					Date_ts = c.StartsAt.ToUnixTimeStamp(),
					Guest_status = null,
					Hotel_id = request.HotelId,
					Is_change_sheets = c.IsChangeSheets ? 1 : 0,
					Is_priority = c.IsPriority ? 1 : 0,
					Planning_date = c.StartsAt.ToString("yyyy-MM-dd"),
					Planning_user_email = cleaner.Email,
					Planning_user_firstname = cleaner.FirstName,
					Planning_user_id = cleaner.Id,
					Planning_user_lastname = cleaner.LastName,
					Planning_user_username = cleaner.UserName,
					Room_id = room.Id,
					Room_name = room.Name,
					Scheduled_order = null,
					Scheduled_ts = null,
					Name = c.Description,
					Bed_id = roomBed.Id,
					Bed_name = roomBed.Name,
				};

				result.Add(resultCleaning);
			}

			return result;
		}
	}
}
