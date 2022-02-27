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

namespace Planner.Application.MobileApi.Cleanings.Queries.GetListOfCleaningsForMobile
{
	public class CleaningForMobile
	{
		public Guid Id { get; set; }
		/// <summary>
		/// Date time unix timestamp
		/// </summary>
		public long Date_ts { get; set; }
		/// <summary>
		/// Date in format "YYYY-mm-DD"
		/// </summary>
		public string Planning_date { get; set; }
		public string Hotel_id { get; set; }
		public Guid Room_id { get; set; }
		public string Room_name { get; set; }
		public Guid? Bed_id { get; set; }
		public string Bed_name { get; set; }
		public Guid Planning_user_id { get; set; }
		public string Planning_user_username { get; set; }
		public string Planning_user_email { get; set; }
		public string Planning_user_firstname { get; set; }
		public string Planning_user_lastname { get; set; }
		public Guid? Creator_id { get; set; }
		public decimal Credits { get; set; }
		public int Is_priority { get; set; }
		public int Is_change_sheets { get; set; }
		public string Assigned_time { get; set; }
		public string Guest_status { get; set; }
		public long? Scheduled_ts { get; set; }
		public int? Scheduled_order { get; set; }
		public int? Container_index { get; set; }
		public Guid? Secondary_user_id { get; set; }
		public string Secondary_user_username { get; set; }
		public string Secondary_user_firstname { get; set; }
		public string Secondary_user_lastname { get; set; }


		public string Name { get; set; }
	}

	public class GetListOfCleaningsForMobileQuery: IRequest<IEnumerable<CleaningForMobile>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfCleaningsForMobileQueryHandler : IRequestHandler<GetListOfCleaningsForMobileQuery, IEnumerable<CleaningForMobile>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly string _roleName;

		public GetListOfCleaningsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
			this._roleName = contextAccessor.RoleName();
		}

		public async Task<IEnumerable<CleaningForMobile>> Handle(GetListOfCleaningsForMobileQuery request, CancellationToken cancellationToken)
		{
			var dateProvider = new HotelLocalDateProvider();
			var date = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, false);

			// Inspectors get all cleanings
			var isInspector = this._roleName == SystemDefaults.Roles.Inspector.Name;

			var cleanings = await this._databaseContext
				.Cleanings
				.Include(cpi => cpi.Room)
				.Include(cpi => cpi.RoomBed)
				.Include(cpi => cpi.Cleaner)
				.Where(cpi => 
					cpi.CleaningPlan.HotelId == request.HotelId 
					&& cpi.IsActive
					&& cpi.IsPlanned
					&& cpi.StartsAt.Date == date
					&& (cpi.CleanerId == this._userId || isInspector))
				.ToArrayAsync();


			var userIds = new HashSet<Guid>();
			foreach(var cleaning in cleanings)
			{
				if (!userIds.Contains(cleaning.CleanerId))
				{
					userIds.Add(cleaning.CleanerId);
				}
			}

			var usersMap = await this._databaseContext.Users
				.Where(u => userIds.Contains(u.Id))
				.Select(u => new 
				{ 
					u.Id,
					u.FirstName,
					u.LastName,
					u.UserName,
					u.Email
				})
				.ToDictionaryAsync(u => u.Id, u => u);

			var result = new List<CleaningForMobile>();
			foreach(var c in cleanings)
			{
				var cid = c.CleanerId;
				var cleaner = usersMap[cid];

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
					Room_id = c.RoomId,
					Room_name = c.Room.Name,
					Scheduled_order = null,
					Scheduled_ts = null,
					Name = c.Description,
					Bed_id = c.RoomBedId,
					Bed_name = c.RoomBed == null ? null : c.RoomBed.Name,
				};

				result.Add(resultCleaning);
			}

			return result;
		}
	}

}
