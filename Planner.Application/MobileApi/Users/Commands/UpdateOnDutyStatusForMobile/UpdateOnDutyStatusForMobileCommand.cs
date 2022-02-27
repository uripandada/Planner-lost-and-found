using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Users.Queries.GetListOfUsersForMobile;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Users.Commands.UpdateOnDutyStatusForMobile
{
	public class UpdateOnDutyStatusForMobileCommand : IRequest<MobileUser>
	{
		public string HotelId { get; set; }
		public Guid UserId { get; set; }
		public bool Status { get; set; }
	}
	public class UpdateOnDutyStatusForMobileCommandHandler : IRequestHandler<UpdateOnDutyStatusForMobileCommand, MobileUser>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISystemEventsService _eventsService;
		private readonly Guid _hotelGroupId;

		public UpdateOnDutyStatusForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, ISystemEventsService eventsService)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._eventsService = eventsService;
			this._hotelGroupId = this._httpContextAccessor.HttpContext.User.HotelGroupId();
		}

		public async Task<MobileUser> Handle(UpdateOnDutyStatusForMobileCommand request, CancellationToken cancellationToken)
		{
			var hotelLocalDateProvider = new HotelLocalDateProvider();
			var now = await hotelLocalDateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);

			var user = await this._databaseContext.Users
				//.Include(u => u.UserRoles)
				.Where(u => u.Id == request.UserId)
				.FirstOrDefaultAsync();

			var isOnDuty = request.Status;

			user.IsOnDuty = isOnDuty;
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			if (user.IsOnDuty)
			{
				await this._eventsService.UserCameOnDuty(this._hotelGroupId, request.UserId, now);
			}
			else
			{
				await this._eventsService.UserCameOffDuty(this._hotelGroupId, request.UserId, now);
			}

			return new MobileUser
			{
				Id = user.Id,
				First_name = user.FirstName,
				Last_name = user.LastName,
				Username = user.UserName,
				Email = user.Email,
				EmployeeId = user.Id.ToString(),
				Language = user.Language,
				Status = user.IsActive ? "active" : "inactive",
				IsOnDuty = user.IsOnDuty,
				State = "active",
				IsAttendant = true,
				AppVersion = "ver-1",
				City = "",
				Country = "",
				Groups = new string[0],
				Hashed_password = "",
				Hotel = "",
				HotelUsername = "",
				Image = null,
				IsAdministrator = false,
				IsFoodBeverage = false,
				IsHost = false,
				IsInspector = false,
				IsMaintenance = false,
				IsReceptionist = false,
				IsRoomRunner = false,
				IsRoomsServices = false,
				IsSuperAdmin = false,
				Organization = "",
				Permissions = new string[0],
				Role = 0,
				Salt = null,
				Street = "",
				Thumbnail = "",
				Zip = "",
			};
		}
	}
}
