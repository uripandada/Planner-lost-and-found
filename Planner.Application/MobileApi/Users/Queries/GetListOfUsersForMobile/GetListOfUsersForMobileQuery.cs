using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Users.Queries.GetListOfUsersForMobile
{
	public class MobileUser
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		/// <summary>
		/// HotelGroupKey
		/// </summary>
		public string HotelUsername { get; set; }
		public string First_name { get; set; }
		public string Last_name { get; set; }
		/// <summary>
		/// Not sent, only for checking
		/// </summary>
		public string Hashed_password { get; set; }
		/// <summary>
		/// Not sent, only for checking
		/// </summary>
		public string Salt { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
		public string Zip { get; set; }
		public string Status { get; set; }

		public string Image { get; set; }
		public string Thumbnail { get; set; }
		public string Language { get; set; }

		public bool IsAdministrator { get; set; }
		public bool IsAttendant { get; set; }
		public bool IsInspector { get; set; }
		public bool IsMaintenance { get; set; }
		public bool IsReceptionist { get; set; }
		public bool IsRoomsServices { get; set; }
		public bool IsRoomRunner { get; set; }
		public bool IsFoodBeverage { get; set; }
		public bool IsHost { get; set; }
		public bool IsSuperAdmin { get; set; }
		public bool IsOnDuty { get; set; }

		public string AppVersion { get; set; }
		public string EmployeeId { get; set; }

		public string Hotel { get; set; }
		public string Organization { get; set; }

		/// <summary>
		/// This can't be set this way. This must be done some other way.
		/// </summary>
		public int Role { get; set; }
		public string[] Groups { get; set; }
		public string[] Permissions { get; set; }

		public Guid? UserGroupId { get; set; }
		public Guid? UserSubGroupId { get; set; }
	}

	public class GetListOfUsersForMobileQuery: IRequest<IEnumerable<MobileUser>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfUsersForMobileQueryHandler: IRequestHandler<GetListOfUsersForMobileQuery, IEnumerable<MobileUser>>, IAmWebApplicationHandler
	{

		private readonly UserManager<User> _userManager;
		private readonly IDatabaseContext _databaseContext;

		public GetListOfUsersForMobileQueryHandler(IDatabaseContext databaseContext, UserManager<User> userManager)
		{
			this._databaseContext = databaseContext;
			this._userManager = userManager;
		}

		public async Task<IEnumerable<MobileUser>> Handle(GetListOfUsersForMobileQuery request, CancellationToken cancellationToken)
		{
			var users = await this._databaseContext.Users
				.Include(u => u.UserRoles)
				.Where(u => u.UserClaims.Any(uc => uc.ClaimType == ClaimsKeys.HotelId && (uc.ClaimValue == "ALL" || uc.ClaimValue == request.HotelId)))
				.ToArrayAsync();

			var roleMap = await this._databaseContext.Roles.ToDictionaryAsync(r => r.Id, r => r);

			return users.Select(u => {
				var isAttendant = false;
				var isInspector = false;
				var isMaintenance = false;
				var isAdministrator = false;

				if (u.UserRoles.Any())
				{
					var normalizedRoleName = roleMap[u.UserRoles.First().RoleId].NormalizedName;
					isAttendant = normalizedRoleName == Common.SystemDefaults.Roles.Attendant.NormalizedName;
					isInspector = normalizedRoleName == Common.SystemDefaults.Roles.Inspector.NormalizedName;
					isMaintenance = normalizedRoleName == Common.SystemDefaults.Roles.Maintenance.NormalizedName;
					isAdministrator = normalizedRoleName == Common.SystemDefaults.Roles.Administrator.NormalizedName;
				}
				else
				{
					isAttendant = true;
				}

				return new MobileUser
				{
					Id = u.Id,
					First_name = u.FirstName,
					Last_name = u.LastName,
					Username = u.UserName,
					Email = u.Email,
					EmployeeId = u.Id.ToString(),
					Language = u.Language.IsNotNull() ? u.Language : "en",
					
					IsAttendant = isAttendant,
					IsMaintenance = isMaintenance,
					IsAdministrator = isAdministrator,
					IsInspector = isInspector,
					
					AppVersion = "ver-1",
					City = "",
					Country = "",
					Groups = new string[0],
					Hashed_password = "",
					Hotel = "",
					HotelUsername = "",
					Image = null,
					IsFoodBeverage = false,
					IsHost = false,
					IsOnDuty = false,
					IsReceptionist = false,
					IsRoomRunner = false,
					IsRoomsServices = false,
					IsSuperAdmin = false,
					Organization = "",
					Permissions = new string[0],
					Role = 0,
					Salt = null,
					State = "active",
					Status = "active",
					Street = "",
					Thumbnail = "",
					Zip = "",

					UserGroupId = u.UserGroupId,
					UserSubGroupId = u.UserSubGroupId,
				}; 
			}).ToArray();
		}
	}
}
