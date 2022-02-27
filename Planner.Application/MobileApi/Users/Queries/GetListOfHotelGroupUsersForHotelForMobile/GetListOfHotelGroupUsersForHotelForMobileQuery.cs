using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Users.Queries.GetListOfHotelGroupUsersForMobile;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Users.Queries.GetListOfHotelGroupUsersForHotelForMobile
{
	public class GetListOfHotelGroupUsersForHotelForMobileQuery : IRequest<MobileUsersPreview>
	{
		/// <summary>
		/// Hotel name
		/// </summary>
		public string HotelName { get; set; }

		/// <summary>
		/// Hotel group key
		/// </summary>
		public string HotelGroupKey { get; set; }

		/// <summary>
		/// Available types:
		/// "attendant"
		/// "room_runner"
		/// "maintenance"
		/// "inspector"
		/// "reception"
		/// "administrator"
		/// "host"
		/// "foodbeverage"
		/// "roomsservices"
		/// </summary>
		public string UserType { get; set; }
	}

	public class GetListOfHotelGroupUsersForHotelForMobileQueryHandler : IRequestHandler<GetListOfHotelGroupUsersForHotelForMobileQuery, MobileUsersPreview>, IAmWebApplicationHandler
	{

		private readonly IMasterDatabaseContext _masterDatabaseContext;
		private readonly IDatabaseContext _databaseContext;

		public GetListOfHotelGroupUsersForHotelForMobileQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._databaseContext = databaseContext;
		}

		public async Task<MobileUsersPreview> Handle(GetListOfHotelGroupUsersForHotelForMobileQuery request, CancellationToken cancellationToken)
		{
			var tenant = await this._masterDatabaseContext.HotelGroupTenants.FirstOrDefaultAsync(t => t.Key.Trim().ToLower() == request.HotelGroupKey.Trim().ToLower());

			if (tenant == null)
			{
				return new MobileUsersPreview
				{
					Name = "N/A",
					HotelGroupId = Guid.Empty,
					HotelGroupName = "N/A",
					HotelId = "N/A",
					HotelName = "N/A",
					Images = null,
					Users = new MobileUserPreview[0],
				};
			}

			this._databaseContext.SetTenantId(tenant.Id);

			var role = (Domain.Entities.Role)null;
			if (request.UserType.IsNotNull())
			{

				switch (request.UserType.ToLower())
				{
					case "attendant":
						role = await this._databaseContext.Roles.Where(r => r.NormalizedName == SystemDefaults.Roles.Attendant.NormalizedName).FirstOrDefaultAsync();
						break;
					case "maintenance":
						role = await this._databaseContext.Roles.Where(r => r.NormalizedName == SystemDefaults.Roles.Maintenance.NormalizedName).FirstOrDefaultAsync();
						break;
					case "inspector":
						role = await this._databaseContext.Roles.Where(r => r.NormalizedName == SystemDefaults.Roles.Inspector.NormalizedName).FirstOrDefaultAsync();
						break;
					case "reception":
						role = await this._databaseContext.Roles.Where(r => r.NormalizedName == SystemDefaults.Roles.Receptionist.NormalizedName).FirstOrDefaultAsync();
						break;
					case "administrator":
						//role = await this._databaseContext.Roles.Where(r => r.NormalizedName == SystemDefaults.Roles.Administrator.NormalizedName).FirstOrDefaultAsync();
						break;
					case "host":
						role = await this._databaseContext.Roles.Where(r => r.NormalizedName == SystemDefaults.Roles.Host.NormalizedName).FirstOrDefaultAsync();
						break;
					case "room_runner":
						role = await this._databaseContext.Roles.Where(r => r.NormalizedName == SystemDefaults.Roles.Runner.NormalizedName).FirstOrDefaultAsync();
						break;
				}
			}

			if (role == null)
			{
				return new MobileUsersPreview
				{
					Name = "N/A",
					HotelGroupId = Guid.Empty,
					HotelGroupName = "N/A",
					HotelId = "N/A",
					HotelName = "N/A",
					Images = null,
					Users = new MobileUserPreview[0],
				};
			}

			var hotel = await this._databaseContext.Hotels.FirstOrDefaultAsync(h => h.Name.Trim().ToLower() == request.HotelName.Trim().ToLower());

			if (hotel == null)
			{
				return new MobileUsersPreview
				{
					Name = "N/A",
					HotelGroupId = Guid.Empty,
					HotelGroupName = "N/A",
					HotelId = "N/A",
					HotelName = "N/A",
					Images = null,
					Users = new MobileUserPreview[0],
				};
			}

			var users = await this._databaseContext.Users
				.Include(u => u.Avatar)
				.Where(u =>
					u.UserRoles.Any(ur => ur.RoleId == role.Id) &&
					u.UserClaims.Any(uc => uc.ClaimType == Domain.Values.ClaimsKeys.HotelId && (uc.ClaimValue == "ALL" || uc.ClaimValue == hotel.Id))
				)
				.ToArrayAsync();

			return new MobileUsersPreview
			{
				Name = tenant.Name,
				HotelGroupId = tenant.Id,
				HotelGroupName = tenant.Name,
				HotelId = hotel.Id,
				HotelName = hotel.Name,
				Images = null,
				Users = users.Select(u => new MobileUserPreview
				{
					Image = u.Avatar == null ? null : u.Avatar.FileUrl,
					Username = u.UserName,
					Language = u.Language.IsNotNull() ? u.Language : "en"
				}).ToArray(),
			};
		}
	}
}
