using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Cleanings.Queries.GetListOfCleaningsForMobile;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Cleanings.Queries.GetCleaningDetailsForMobile
{
	public class CleaningDetailsForMobile : CleaningForMobile
	{

	}

	public class GetCleaningDetailsForMobileQuery : IRequest<CleaningDetailsForMobile>
	{
		public string HotelId { get; set; }
		public Guid RoomId { get; set; }
	}

	public class GetCleaningDetailsForMobileQueryHandler : IRequestHandler<GetCleaningDetailsForMobileQuery, CleaningDetailsForMobile>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetCleaningDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<CleaningDetailsForMobile> Handle(GetCleaningDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			var dateProvider = new HotelLocalDateProvider();
			var date = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, false);

			var cleaning = await this._databaseContext
				.Cleanings
				.Include(cpi => cpi.Room)
				.Include(cpi => cpi.RoomBed)
				.Include(cpi => cpi.Cleaner)
				.Where(cpi =>
					cpi.CleaningPlan.HotelId == request.HotelId
					&& cpi.IsActive
					&& cpi.IsPlanned
					&& cpi.StartsAt.Date == date
					&& (cpi.CleanerId == this._userId)
					&& cpi.RoomId == request.RoomId
					)
				.FirstOrDefaultAsync();

			if (cleaning == null) return null;

			var userIds = new HashSet<Guid>();
			if (!userIds.Contains(cleaning.CleanerId))
			{
				userIds.Add(cleaning.CleanerId);
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

			var cid = cleaning.CleanerId;
			var cleaner = usersMap[cid];

			var resultCleaning = new CleaningDetailsForMobile
			{
				Id = cleaning.Id,
				Assigned_time = null,
				Container_index = null,
				Creator_id = this._userId,
				Credits = cleaning.Credits.HasValue ? cleaning.Credits.Value : 0,
				Date_ts = cleaning.StartsAt.ToUnixTimeStamp(),
				Guest_status = null,
				Hotel_id = request.HotelId,
				Is_change_sheets = cleaning.IsChangeSheets ? 1 : 0,
				Is_priority = cleaning.IsPriority ? 1 : 0,
				Planning_date = cleaning.StartsAt.ToString("yyyy-MM-dd"),
				Planning_user_email = cleaner.Email,
				Planning_user_firstname = cleaner.FirstName,
				Planning_user_id = cleaner.Id,
				Planning_user_lastname = cleaner.LastName,
				Planning_user_username = cleaner.UserName,
				Room_id = cleaning.RoomId,
				Room_name = cleaning.Room.Name,
				Scheduled_order = null,
				Scheduled_ts = null,
				Name = cleaning.Description,
				Bed_id = cleaning.RoomBedId,
				Bed_name = cleaning.RoomBed == null ? null : cleaning.RoomBed.Name,
			};

			return resultCleaning;
		}
	}
}
