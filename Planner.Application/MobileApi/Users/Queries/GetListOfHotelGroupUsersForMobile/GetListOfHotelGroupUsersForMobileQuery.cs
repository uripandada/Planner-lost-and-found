using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
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

namespace Planner.Application.MobileApi.Models
{
	public class MobileModels
	{
		public class User
		{
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
		}
	}
}

namespace Planner.Application.MobileApi.Users.Queries.GetListOfHotelGroupUsersForMobile
{


	public class MobileUsersPreview
	{
		/// <summary>
		/// Hotel group name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Just one image? I guess image url. Or null.
		/// </summary>
		public string Images { get; set; }

		public IEnumerable<MobileUserPreview> Users { get; set; }

		// ---- BELOW ARE ADDED EXTRA PROPERTIES
		public string HotelId { get; set; }
		public Guid HotelGroupId { get; set; }
		public string HotelGroupName { get; set; }
		public string HotelName { get; set; }
	}

	public class MobileUserPreview
	{
		/// <summary>
		/// Avatar full username
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Avatar image url
		/// </summary>
		public string Image { get; set; }
		public string Language { get; set; }

		// ---- BELOW ARE ADDED EXTRA PROPERTIES
	}

	public class GetListOfHotelGroupUsersForMobileQuery: IRequest<MobileUsersPreview>
	{
		/// <summary>
		/// Hotel group key
		/// </summary>
		public string HotelUsername { get; set; }

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

	public class GetListOfHotelGroupUsersForMobileQueryHandler : IRequestHandler<GetListOfHotelGroupUsersForMobileQuery, MobileUsersPreview>, IAmWebApplicationHandler
	{

		private readonly IMasterDatabaseContext _masterDatabaseContext;
		private readonly IDatabaseContext _databaseContext;

		public GetListOfHotelGroupUsersForMobileQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._databaseContext = databaseContext;
		}

		public async Task<MobileUsersPreview> Handle(GetListOfHotelGroupUsersForMobileQuery request, CancellationToken cancellationToken)
		{
			if (request.HotelUsername.IsNull() || request.UserType.IsNull())
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

			var tenant = await this._masterDatabaseContext.HotelGroupTenants.FirstOrDefaultAsync(t => t.Key.Trim().ToLower() == request.HotelUsername.Trim().ToLower());

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
			//var cleanerRole = await this._databaseContext.Roles.Where(r => r.Name == SystemDefaults.Roles.Attendant.Name).FirstOrDefaultAsync();

			if(role == null)
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

			var users = await this._databaseContext.Users.Include(u => u.Avatar).Where(u => u.UserRoles.Any(ur => ur.RoleId == role.Id)).ToArrayAsync();
		   
			return new MobileUsersPreview
			{
				Name = tenant.Name,
				HotelGroupId = tenant.Id,
				HotelGroupName = tenant.Name,
				HotelId = null,
				HotelName = null,
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
