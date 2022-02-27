using FluentValidation;
using IdentityServer4.EntityFramework.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Helpers;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Commands.InsertHotelGroup
{
	public class SaveHotelGroup
	{
		public string ConnectionKey { get; set; } // "TEST_SERVER", "DEV_SERVER", "CUSTOM_SERVER"
		public string Key { get; set; } // "hotel-royal"
		public string Name { get; set; } // "Hotel Royal"
		public bool IsActive { get; set; }

		public string ConnectionStringUserId { get; set; }
		public string ConnectionStringPassword { get; set; }
		public string ConnectionStringHost { get; set; }
		public string ConnectionStringPort { get; set; }
		public string ConnectionStringDatabase { get; set; }
		public bool ConnectionStringPooling { get; set; }
	}

	public class InsertHotelGroupCommand : SaveHotelGroup, IRequest<ProcessResponse<Guid>>
	{
	}

	public class InsertHotelGroupCommandHandler : IRequestHandler<InsertHotelGroupCommand, ProcessResponse<Guid>>, IAmAdminApplicationHandler
	{
		private IMasterDatabaseContext _masterDatabaseContext;
		private ITenantManager _tenantManager;
		private IOptions<OperationalStoreOptions> _operationalStoreOptions;
		private IPasswordHasher<Domain.Entities.User> passwordHasher;

		public InsertHotelGroupCommandHandler(IMasterDatabaseContext masterDatabaseContext, ITenantManager tenantManager, IOptions<OperationalStoreOptions> operationalStoreOptions, IPasswordHasher<Domain.Entities.User> passwordHasher)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._tenantManager = tenantManager;
			this._operationalStoreOptions = operationalStoreOptions;
			this.passwordHasher = passwordHasher;
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertHotelGroupCommand request, CancellationToken cancellationToken)
		{
			var connectionString = (string)null;

			if (request.ConnectionKey == "CUSTOM_SERVER")
			{
				connectionString = ConnectionStringHelper.GeneratePostgreSqlConnectionString(
					request.ConnectionStringUserId?.Trim(),
					request.ConnectionStringPassword?.Trim(),
					request.ConnectionStringHost?.Trim(),
					request.ConnectionStringPort?.Trim(),
					request.ConnectionStringDatabase?.Trim(),
					true
				);
			}
			else if (request.ConnectionKey == "DEV_SERVER")
			{
				connectionString = ConnectionStringHelper.GeneratePostgreSqlConnectionString(
					"postgres",
					"SuperJakaLozinka",
					"dev.okulus.hr",
					"54320",
					request.ConnectionStringDatabase?.Trim(),
					true
				);
			}
			else if (request.ConnectionKey == "TEST_SERVER")
			{
				connectionString = ConnectionStringHelper.GeneratePostgreSqlConnectionString(
					"postgres",
					"SuperJakaLozinka",
					"test-database",
					"5432",
					request.ConnectionStringDatabase?.Trim(),
					true
				);
			}

			var tenant = new Domain.Entities.Master.HotelGroupTenant
			{
				ConnectionString = connectionString,
				Id = Guid.NewGuid(),
				IsActive = request.IsActive,
				Key = request.Key,
				Name = request.Name,
				DatabaseName = request.ConnectionStringDatabase
			};


			var defaultAdministratorRoleId = Guid.NewGuid();
			var defaultAdministratorRole = new Domain.Entities.Role
			{
				Id = defaultAdministratorRoleId,
				IsSystemRole = true,
				Name = SystemDefaults.Roles.Administrator.Name,
				NormalizedName = SystemDefaults.Roles.Administrator.NormalizedName,
				HotelAccessTypeKey = "ALL",
			};
			var defaultMaintenanceRoleId = Guid.NewGuid();
			var defaultMaintenanceRole = new Domain.Entities.Role
			{
				Id = defaultMaintenanceRoleId,
				IsSystemRole = true,
				Name = SystemDefaults.Roles.Maintenance.Name,
				NormalizedName = SystemDefaults.Roles.Maintenance.NormalizedName,
				HotelAccessTypeKey = "MULTIPLE",
			};
			var defaultAttendantRoleId = Guid.NewGuid();
			var defaultAttendantRole = new Domain.Entities.Role
			{
				Id = defaultAttendantRoleId,
				IsSystemRole = true,
				Name = SystemDefaults.Roles.Attendant.Name,
				NormalizedName = SystemDefaults.Roles.Attendant.NormalizedName,
				HotelAccessTypeKey = "SINGLE",
			};
			var defaultHostRoleId = Guid.NewGuid();
			var defaultHostRole = new Domain.Entities.Role
			{
				Id = defaultHostRoleId,
				IsSystemRole = true,
				Name = SystemDefaults.Roles.Host.Name,
				NormalizedName = SystemDefaults.Roles.Host.NormalizedName,
				HotelAccessTypeKey = "MULTIPLE",
			};
			var defaultRunnerRoleId = Guid.NewGuid();
			var defaultRunnerRole = new Domain.Entities.Role
			{
				Id = defaultRunnerRoleId,
				IsSystemRole = true,
				Name = SystemDefaults.Roles.Runner.Name,
				NormalizedName = SystemDefaults.Roles.Runner.NormalizedName,
				HotelAccessTypeKey = "MULTIPLE",
			};
			var defaultInspectorRoleId = Guid.NewGuid();
			var defaultInspectorRole = new Domain.Entities.Role
			{
				Id = defaultInspectorRoleId,
				IsSystemRole = true,
				Name = SystemDefaults.Roles.Inspector.Name,
				NormalizedName = SystemDefaults.Roles.Inspector.NormalizedName,
				HotelAccessTypeKey = "SINGLE",
			};
			var defaultReceptionistRoleId = Guid.NewGuid();
			var defaultReceptionistRole = new Domain.Entities.Role
			{
				Id = defaultReceptionistRoleId,
				IsSystemRole = true,
				Name = SystemDefaults.Roles.Receptionist.Name,
				NormalizedName = SystemDefaults.Roles.Receptionist.NormalizedName,
				HotelAccessTypeKey = "SINGLE",
			};

			var defaultAdminUserId = Guid.NewGuid();
			var defaultAdminUser = new Domain.Entities.User
			{
				Id = defaultAdminUserId,
				UserName = SystemDefaults.Users.DefaultAdminUserName,
				PasswordHash = null,
				FirstName = "Default",
				LastName = "Administrator",
				IsActive = true,
				IsSubGroupLeader = false,
				NormalizedUserName = SystemDefaults.Users.DefaultAdminNormalizedUserName,
				LockoutEnabled = true,
				AccessFailedCount = 0,
				SecurityStamp = Guid.NewGuid().ToString()
			};

			defaultAdminUser.PasswordHash = this.passwordHasher.HashPassword(defaultAdminUser, SystemDefaults.Users.DefaultAdminPassword);

			var userRole = new IdentityUserRole<Guid> { RoleId = defaultAdministratorRoleId, UserId = defaultAdminUserId };

			var userClaims = new List<IdentityUserClaim<Guid>>
			{
				//new IdentityUserClaim<Guid> { UserId = defaultAdminUserId , ClaimType = "given_name", ClaimValue = defaultAdminUser.FirstName },
				//new IdentityUserClaim<Guid> { UserId = defaultAdminUserId , ClaimType = "family_name", ClaimValue = defaultAdminUser.LastName },
				new IdentityUserClaim<Guid> { UserId = defaultAdminUserId , ClaimType = "hotel_group_id", ClaimValue = tenant.Id.ToString() },
				new IdentityUserClaim<Guid> { UserId = defaultAdminUserId , ClaimType = "hotel_id", ClaimValue = "ALL" },
			};

			var roleIds = new List<Guid> 
			{ 
				defaultAdministratorRoleId,
				defaultAttendantRoleId,
				defaultHostRoleId,
				defaultRunnerRoleId,
				defaultMaintenanceRoleId,
				defaultReceptionistRoleId,
				defaultInspectorRoleId
			};

			var roleClaims = roleIds.SelectMany(roleId => 
			{
				return new List<IdentityRoleClaim<Guid>>
				{
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.SettingsClaimKeys.Rooms, ClaimValue= ClaimsKeys.SettingsClaimKeys.Rooms , },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.SettingsClaimKeys.Assets , ClaimValue= ClaimsKeys.SettingsClaimKeys.Assets , },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.SettingsClaimKeys.Users, ClaimValue= ClaimsKeys.SettingsClaimKeys.Users ,  },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.SettingsClaimKeys.RoleManagement, ClaimValue= ClaimsKeys.SettingsClaimKeys.RoleManagement ,  },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.SettingsClaimKeys.RoomCategories , ClaimValue= ClaimsKeys.SettingsClaimKeys.RoomCategories , },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.SettingsClaimKeys.HotelSettings, ClaimValue= ClaimsKeys.SettingsClaimKeys.HotelSettings ,  },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.SettingsClaimKeys.Colors, ClaimValue= ClaimsKeys.SettingsClaimKeys.Colors ,  },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.Tasks, ClaimValue= ClaimsKeys.ManagementClaimKeys.Tasks , },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.Reservations, ClaimValue= ClaimsKeys.ManagementClaimKeys.Reservations ,  },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.CleaningPlanner, ClaimValue= ClaimsKeys.ManagementClaimKeys.CleaningPlanner ,  },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.CleaningCalendar, ClaimValue= ClaimsKeys.ManagementClaimKeys.CleaningCalendar ,  },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.ReservationCalendar, ClaimValue= ClaimsKeys.ManagementClaimKeys.ReservationCalendar, },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.LostAndFound, ClaimValue= ClaimsKeys.ManagementClaimKeys.LostAndFound , },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.OnGuard, ClaimValue= ClaimsKeys.ManagementClaimKeys.OnGuard,  },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.RoomInsights, ClaimValue= ClaimsKeys.ManagementClaimKeys.RoomInsights, },
					new IdentityRoleClaim<Guid> { RoleId = roleId, ClaimType= ClaimsKeys.ManagementClaimKeys.UserInsights, ClaimValue= ClaimsKeys.ManagementClaimKeys.UserInsights, },
				};
			}).ToArray();

			//var roleClaims = new List<IdentityRoleClaim<Guid>>
			//{
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.SettingsClaimKeys.Rooms, ClaimValue= ClaimsKeys.SettingsClaimKeys.Rooms , },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.SettingsClaimKeys.Assets , ClaimValue= ClaimsKeys.SettingsClaimKeys.Assets , },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.SettingsClaimKeys.Users, ClaimValue= ClaimsKeys.SettingsClaimKeys.Users ,  },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.SettingsClaimKeys.RoleManagement, ClaimValue= ClaimsKeys.SettingsClaimKeys.RoleManagement ,  },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.SettingsClaimKeys.RoomCategories , ClaimValue= ClaimsKeys.SettingsClaimKeys.RoomCategories , },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.SettingsClaimKeys.HotelSettings, ClaimValue= ClaimsKeys.SettingsClaimKeys.HotelSettings ,  },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.SettingsClaimKeys.Colors, ClaimValue= ClaimsKeys.SettingsClaimKeys.Colors ,  },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.Tasks, ClaimValue= ClaimsKeys.ManagementClaimKeys.Tasks , },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.Reservations, ClaimValue= ClaimsKeys.ManagementClaimKeys.Reservations ,  },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.CleaningPlanner, ClaimValue= ClaimsKeys.ManagementClaimKeys.CleaningPlanner ,  },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.CleaningCalendar, ClaimValue= ClaimsKeys.ManagementClaimKeys.CleaningCalendar ,  },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.ReservationCalendar, ClaimValue= ClaimsKeys.ManagementClaimKeys.ReservationCalendar, },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.LostAndFound, ClaimValue= ClaimsKeys.ManagementClaimKeys.LostAndFound , },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.OnGuard, ClaimValue= ClaimsKeys.ManagementClaimKeys.OnGuard,  },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.RoomInsights, ClaimValue= ClaimsKeys.ManagementClaimKeys.RoomInsights, },
			//	new IdentityRoleClaim<Guid> { RoleId = defaultAdministratorRoleId, ClaimType= ClaimsKeys.ManagementClaimKeys.UserInsights, ClaimValue= ClaimsKeys.ManagementClaimKeys.UserInsights, },
			//};

			var defaultRoomCategoryId = Guid.NewGuid();
			var defaultRoomCategory = new Domain.Entities.RoomCategory
			{
				Id = defaultRoomCategoryId,
				CreatedById = defaultAdminUserId,
				CreatedAt = DateTime.UtcNow,
				ModifiedById = defaultAdminUserId,
				ModifiedAt = DateTime.UtcNow,
				IsPrivate = false,
				IsPublic = false,
				Name = "Standard",
				IsDefaultForReservationSync = true,
				IsSystemDefaultForReservationSync = true,
			};

			var rccHousekeepingStatuses = Enum.GetValues(typeof(RccHousekeepingStatusCode)).Cast<RccHousekeepingStatusCode>();
			var statusColors = new List<Domain.Entities.RccHousekeepingStatusColor>();

			foreach (var status in rccHousekeepingStatuses)
			{
				var color = "454545";
				if (COLORS.ROOM_HOUSEKEEPING_STATUSES.ContainsKey(status))
				{
					color = COLORS.ROOM_HOUSEKEEPING_STATUSES[status];
				}

				var description = "Unknown";
				if (COLORS.ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS.ContainsKey(status))
				{
					description = COLORS.ROOM_HOUSEKEEPING_STATUS_DESCRIPTIONS[status];
				}

				statusColors.Add(new Domain.Entities.RccHousekeepingStatusColor
				{
					ColorHex = color,
					RccCode = status,
				});
			}

			using (var databaseContext = this._tenantManager.CreateDatabaseContext(tenant.ConnectionString, this._operationalStoreOptions, null))
			{
				//try
				//{
					await databaseContext.Database.MigrateAsync(cancellationToken);

					await databaseContext.Roles.AddAsync(defaultAdministratorRole, cancellationToken);
					await databaseContext.Roles.AddAsync(defaultAttendantRole, cancellationToken);
					await databaseContext.Roles.AddAsync(defaultHostRole, cancellationToken);
					await databaseContext.Roles.AddAsync(defaultInspectorRole, cancellationToken);
					await databaseContext.Roles.AddAsync(defaultMaintenanceRole, cancellationToken);
					await databaseContext.Roles.AddAsync(defaultReceptionistRole, cancellationToken);
					await databaseContext.Roles.AddAsync(defaultRunnerRole, cancellationToken);

					await databaseContext.Users.AddAsync(defaultAdminUser, cancellationToken);
					await databaseContext.UserRoles.AddAsync(userRole, cancellationToken);
					await databaseContext.UserClaims.AddRangeAsync(userClaims, cancellationToken);
					await databaseContext.RoleClaims.AddRangeAsync(roleClaims, cancellationToken);

					await databaseContext.RoomCategorys.AddAsync(defaultRoomCategory, cancellationToken);

					await databaseContext.RccHousekeepingStatusColors.AddRangeAsync(statusColors, cancellationToken);

					await databaseContext.SaveChangesAsync(cancellationToken);
				//}
				//catch (Exception ex)
				//{
				//	return new ProcessResponse<Guid>
				//	{
				//		Data = Guid.Empty,
				//		HasError = true,
				//		IsSuccess = false,
				//		Message = "Error while trying to connect to the database"
				//	};
				//}

				try
				{
					await this._masterDatabaseContext.HotelGroupTenants.AddAsync(tenant, cancellationToken);
					await this._masterDatabaseContext.SaveChangesAsync(cancellationToken);
				}
				catch (Exception ex)
				{
					await databaseContext.Database.EnsureDeletedAsync(cancellationToken);

					return new ProcessResponse<Guid>
					{
						Data = Guid.Empty,
						HasError = true,
						IsSuccess = false,
						Message = "Error while inserting group. WARNING! YOU MUST DELETE THE CREATED TENANT DATABASE MANUALLY."
					};
				}
			}

			return new ProcessResponse<Guid>
			{
				Data = tenant.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Hotel group created",
			};
		}
	}

	public class InsertHotelGroupCommandValidator : AbstractValidator<InsertHotelGroupCommand>
	{
		private readonly IMasterDatabaseContext _masterDatabaseContext;

		public InsertHotelGroupCommandValidator(IMasterDatabaseContext masterDatabaseContext)
		{
			this._masterDatabaseContext = masterDatabaseContext;

			RuleFor(command => command.ConnectionStringHost).Custom((connectionString, context) => {
				var cmd = context.InstanceToValidate as InsertHotelGroupCommand;
				if(cmd.ConnectionKey == "CUSTOM_SERVER" && connectionString.IsNull())
				{
					context.AddFailure(nameof(InsertHotelGroupCommand.ConnectionStringHost), "Required.");
				}
			});
			RuleFor(command => command.ConnectionStringPassword).Custom((pwd, context) => {
				var cmd = context.InstanceToValidate as InsertHotelGroupCommand;
				if (cmd.ConnectionKey == "CUSTOM_SERVER" && pwd.IsNull())
				{
					context.AddFailure(nameof(InsertHotelGroupCommand.ConnectionStringPassword), "Required.");
				}
			});
			RuleFor(command => command.ConnectionStringPort).Custom((port, context) => {
				var cmd = context.InstanceToValidate as InsertHotelGroupCommand;
				if (cmd.ConnectionKey == "CUSTOM_SERVER" && port.IsNull())
				{
					context.AddFailure(nameof(InsertHotelGroupCommand.ConnectionStringPort), "Required.");
				}
			});
			RuleFor(command => command.ConnectionStringUserId).Custom((userId, context) => {
				var cmd = context.InstanceToValidate as InsertHotelGroupCommand;
				if (cmd.ConnectionKey == "CUSTOM_SERVER" && userId.IsNull())
				{
					context.AddFailure(nameof(InsertHotelGroupCommand.ConnectionStringUserId), "Required.");
				}
			});
			RuleFor(command => command.IsActive).NotEmpty();
			RuleFor(command => command.Name).NotEmpty();
			
			RuleFor(command => command.Key).NotEmpty().MustAsync(async (command, key, propertyValidatorContext, cancellationToken) =>
			{
				var existingDatabase = await this._masterDatabaseContext.HotelGroupTenants.Where(t => t.Key.ToLower() == key.ToLower()).FirstOrDefaultAsync();
				return existingDatabase == null;
			}).WithMessage("DATABASE_KEY_ALREADY_EXISTS");

			RuleFor(command => command.ConnectionStringDatabase).NotEmpty().MustAsync(async (command, databaseName, propertyValidatorContext, cancellationToken) =>
			{
	//            if(command.ConnectionKey == "CUSTOM_SERVER")
				//{
					var existingDatabase = await this._masterDatabaseContext.HotelGroupTenants.Where(t => t.DatabaseName.ToLower() == databaseName.ToLower()).FirstOrDefaultAsync();
					return existingDatabase == null;
				//}
				//else
				//{
	//                return true;
				//}
			}).WithMessage("DATABASE_NAME_ALREADY_EXISTS");
		}
	}
}
