using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common.Shared;
using Planner.Domain.Entities;
using Planner.Domain.Entities.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Planner.Persistence.DataSeed
{
	public static class MasterDataSeed
	{
		public static async Task SeedAuthenticationClients(IMasterDatabaseContext masterDatabaseContext, IServiceProvider serviceProvider)
		{
			var cancellationToken = System.Threading.CancellationToken.None;

			using var scope = serviceProvider.CreateScope();

			var context = scope.ServiceProvider.GetRequiredService<MasterDatabaseContext>();
			await context.Database.EnsureCreatedAsync(cancellationToken);

			var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

			if (await manager.FindByClientIdAsync("postman", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "postman",
					ClientSecret = "postman-secret",
					DisplayName = "Postman",
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Token,
						OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

						OpenIddictConstants.Permissions.Prefixes.Scope + "api"
					}
				}, cancellationToken);
			}
			if (await manager.FindByClientIdAsync("postman-auth-code", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "postman-auth-code",
					ClientSecret = "postman-auth-code-secret",
					DisplayName = "Postman Auth Code Client",
					RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Authorization,
						OpenIddictConstants.Permissions.Endpoints.Token,

						OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
						OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

						OpenIddictConstants.Permissions.Prefixes.Scope + "api",

						OpenIddictConstants.Permissions.ResponseTypes.Code
					}
				}, cancellationToken);
			}
			if (await manager.FindByClientIdAsync("postman-auth-code", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "postman-auth-code2",
					ClientSecret = "postman-auth-code-secret2",
					DisplayName = "Postman Auth Code Client2",
					RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Authorization,
						OpenIddictConstants.Permissions.Endpoints.Token,

						OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
						OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

						OpenIddictConstants.Permissions.Prefixes.Scope + "api",

						OpenIddictConstants.Permissions.ResponseTypes.Code
					}
				}, cancellationToken);
			}
			if (await manager.FindByClientIdAsync("postman-refresh", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "postman-refresh",
					ClientSecret = "postman-refresh-secret",
					DisplayName = "Postman Refresh Token Client",
					RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Authorization,
						OpenIddictConstants.Permissions.Endpoints.Token,

						OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
						OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
						OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

						OpenIddictConstants.Permissions.Prefixes.Scope + "api",

						OpenIddictConstants.Permissions.ResponseTypes.Code
					}
				}, cancellationToken);
			}

			if (await manager.FindByClientIdAsync("console-client") is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "console-client",
					ClientSecret = "console-client-secret",
					DisplayName = "Console client",
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Token,
						OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

						OpenIddictConstants.Permissions.Prefixes.Scope + "api"
					}
				});
			}

			// THIS IS THE REAL CLIENT FOR BOTH DEVELOPMENT AND TESTING!!!
			if (await manager.FindByClientIdAsync("roomchecking-client", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "roomchecking-client",
					//ClientSecret = "roomchecking-client-secret",
					DisplayName = "Roomchecking API client",
					PostLogoutRedirectUris = { new Uri("https://localhost:5001/authentication/logout-callback"), new Uri("https://test-next.roomchecking.com/authentication/logout-callback") },
					RedirectUris = { new Uri("https://localhost:5001/authentication/login-callback"), new Uri("https://test-next.roomchecking.com/authentication/login-callback") },
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Authorization,
						OpenIddictConstants.Permissions.Endpoints.Token,
						OpenIddictConstants.Permissions.Endpoints.Logout,
						OpenIddictConstants.Permissions.Endpoints.Revocation,

						OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
						OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
						OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

						OpenIddictConstants.Permissions.Scopes.Profile,

						OpenIddictConstants.Permissions.Prefixes.Scope + "api",
						OpenIddictConstants.Permissions.Prefixes.Scope + "hotelid",
						OpenIddictConstants.Permissions.Prefixes.Scope + "hotelgroupid",

						OpenIddictConstants.Permissions.ResponseTypes.Code
					}
				}, cancellationToken);
			}

			// THIS IS THE REAL CLIENT FOR MOBILE APPS BOTH DEVELOPMENT AND TESTING!!!
			if (await manager.FindByClientIdAsync("roomchecking-mobile-client", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "roomchecking-mobile-client",
					//ClientSecret = "roomchecking-client-secret",
					DisplayName = "Roomchecking API mobile client",
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Authorization,
						OpenIddictConstants.Permissions.Endpoints.Token,
						OpenIddictConstants.Permissions.Endpoints.Logout,
						OpenIddictConstants.Permissions.Endpoints.Revocation,

						OpenIddictConstants.Permissions.GrantTypes.Password,
						OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

						OpenIddictConstants.Permissions.Scopes.Profile,

						OpenIddictConstants.Permissions.Prefixes.Scope + "api",
						OpenIddictConstants.Permissions.Prefixes.Scope + "hotelid",
						OpenIddictConstants.Permissions.Prefixes.Scope + "hotelgroupid",
					}
				}, cancellationToken);
			}

			if (await manager.FindByClientIdAsync("roomchecking-client-testing", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "roomchecking-client-testing",
					//ClientSecret = "roomchecking-client-secret",
					DisplayName = "Roomchecking API - Testing environment",
					PostLogoutRedirectUris = { new Uri("https://test-next.roomchecking.com/authentication/logout-callback") },
					RedirectUris = { new Uri("https://test-next.roomchecking.com/authentication/login-callback") },
					Permissions =
					{
						OpenIddictConstants.Permissions.Endpoints.Authorization,
						OpenIddictConstants.Permissions.Endpoints.Token,
						OpenIddictConstants.Permissions.Endpoints.Logout,
						OpenIddictConstants.Permissions.Endpoints.Revocation,

						OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
						OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
						OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

						OpenIddictConstants.Permissions.Scopes.Profile,

						OpenIddictConstants.Permissions.Prefixes.Scope + "api",
						OpenIddictConstants.Permissions.Prefixes.Scope + "hotelid",
						OpenIddictConstants.Permissions.Prefixes.Scope + "hotelgroupid",

						OpenIddictConstants.Permissions.ResponseTypes.Code
					}
				}, cancellationToken);
			}
		}


		public static async Task SeedMasterData(IMasterDatabaseContext masterDatabaseContext, IServiceProvider serviceProvider)
		{
			await masterDatabaseContext.Database.MigrateAsync();

			var userManager = _NewUserManager(masterDatabaseContext as MasterDatabaseContext, serviceProvider);

			var user1 = _NewUser("admin1@email.com", "masteradmin1");
			var user1Password = "masteradmintest123123";
			await _SeedUser(userManager, user1, user1Password);

			var user2 = _NewUser("admin2@email.com", "masteradmin2");
			var user2Password = "masteradmintest123123";
			await _SeedUser(userManager, user2, user2Password);

			var user3 = _NewUser("admin3@email.com", "masteradmin3");
			var user3Password = "masteradmintest123123";
			await _SeedUser(userManager, user3, user3Password);
		}

		private static MasterUser _NewUser(string email, string userName)
		{
			return new MasterUser
			{
				Email = email,
				UserName = userName
			};
		}

		private static UserManager<MasterUser> _NewUserManager(MasterDatabaseContext context, IServiceProvider serviceProvider)
		{
			var userStore = new UserStore<MasterUser, Role, MasterDatabaseContext, Guid>(context);
			var identityOptions = new IdentityOptions();
			var passwordHasher = new PasswordHasher<MasterUser>();
			var userValidators = new List<IUserValidator<MasterUser>>();
			var passwordValidators = new List<IPasswordValidator<MasterUser>>();
			var keyNormalizer = new UpperInvariantLookupNormalizer();
			var userErrors = new IdentityErrorDescriber();
			var userManager = new UserManager<MasterUser>(userStore, Options.Create(identityOptions), passwordHasher, userValidators, passwordValidators, keyNormalizer, userErrors, serviceProvider, null);
			return userManager;
		}

		private static async Task _SeedUser(UserManager<MasterUser> userManager, MasterUser user, string password)
		{
			if (await userManager.FindByEmailAsync(user.Email) == null)
			{
				var createUserResult = await userManager.CreateAsync(user, password);
				//var emailconfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
				//await userManager.ConfirmEmailAsync(user, emailconfirmationToken);
			}
		}
	}


	public static class DataSeed
	{
		public static async Task SeedMultitenantAsync(IMasterDatabaseContext masterContext, IServiceProvider serviceProvider)
		{
			//var hotelGroupTenants = _NewTestHotelGroupTenants(masterContext);

			//await _SeedHotelGroupTenants(masterContext, hotelGroupTenants);
			//await masterContext.SaveChangesAsync(System.Threading.CancellationToken.None);

			var hotelGroupTenants = await masterContext.HotelGroupTenants.Where(x=>x.IsActive).ToListAsync();

			var globalHotelCounter = 0;
			foreach (var tenant in hotelGroupTenants)
			{
				try
				{
					// TENANT INITIALIZATION PART

					var databaseContext = _NewDatbaseContext(tenant, serviceProvider);
					var userManager = _NewUserManager(databaseContext, serviceProvider);
					var roleManager = _NewRoleManager(databaseContext);

					// CREATING THE DB IF NECESSARY

					await databaseContext.Database.MigrateAsync();

#if DEBUG
					// DATA CREATION PART
					//var hotels = _NewTenantHotels(globalHotelCounter, 5);
					//	globalHotelCounter += hotels.Count();

					//	var roles = _NewTenantRoles();

					//	var adminUser = _NewTenantUser("Admin", "Roomchecking", "admin@email.com", "rcadmin");
					//	var adminUserClaims = _NewTenantUserClaims(adminUser, tenant.Id);
					//	var adminUserPassword = "admintest123123";
					//	var adminUserRole = "Admin";

					//	var regularUser1 = _NewTenantUser("User1", "Roomchecking", "user1@email.com", "rcuser1");
					//	var regularUser1Claims = _NewTenantUserClaims(regularUser1, tenant.Id);
					//	var regularUser1Password = "usertest123123";
					//	var regularUser1Role = "Tech";

					//	var regularUser2 = _NewTenantUser("User2", "Roomchecking", "user2@email.com", "rcuser2");
					//	var regularUser2Claims = _NewTenantUserClaims(regularUser2, tenant.Id);
					//	var regularUser2Password = "usertest123123";
					//	var regularUser2Role = "Cleaner";

					//	// DATA SEED PART

					//	await _SeedTenantRoles(roleManager, roles);
					//	await _SeedTenantUser(userManager, adminUser, adminUserPassword, adminUserRole, adminUserClaims);
					//	await _SeedTenantUser(userManager, regularUser1, regularUser1Password, regularUser1Role, regularUser1Claims);
					//	await _SeedTenantUser(userManager, regularUser2, regularUser2Password, regularUser2Role, regularUser2Claims);
					//	await _SeedTenantHotels(databaseContext, hotels);

					//	await databaseContext.SaveChangesAsync();
#endif
					// DISPOSE PART

					databaseContext.Dispose();
					userManager.Dispose();
					roleManager.Dispose();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}

		public static async Task MigrateMultitenantAsync(IMasterDatabaseContext masterContext, IServiceProvider serviceProvider)
		{
			var hotelGroupTenants = await masterContext.HotelGroupTenants.ToListAsync();

			foreach (var tenant in hotelGroupTenants)
			{
				//if (tenant.Key != "test_TSH") continue;
				//if (tenant.Key != "Barriere") continue;

				// TENANT INITIALIZATION PART
				var databaseContext = _NewDatbaseContext(tenant, serviceProvider);

				// CREATING THE DB IF NECESSARY
				await databaseContext.Database.MigrateAsync();

				// DISPOSE PART
				databaseContext.Dispose();
			}
		}

		#region Private Seed Methods
		private static DatabaseContext _NewDatbaseContext(HotelGroupTenant tenant, IServiceProvider serviceProvider)
		{
			var tenantData = new HotelGroupTenantData
			{
				ConnectionString = tenant.ConnectionString,
				Name = tenant.Name,
				Id = tenant.Id,
				IsActive = tenant.IsActive,
				Key = tenant.Key
			};
			var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
			var context = new DatabaseContext(optionsBuilder.Options, Options.Create(new OperationalStoreOptions()), new ArrayHotelGroupTenantProvider(tenantData));
			return context;
		}

		private static UserManager<User> _NewUserManager(DatabaseContext context, IServiceProvider serviceProvider)
		{
			var userStore = new UserStore<User, Role, DatabaseContext, Guid>(context);
			var identityOptions = new IdentityOptions();
			var passwordHasher = new PasswordHasher<User>();
			var userValidators = new List<IUserValidator<User>>();
			var passwordValidators = new List<IPasswordValidator<User>>();
			var keyNormalizer = new UpperInvariantLookupNormalizer();
			var userErrors = new IdentityErrorDescriber();
			var userManager = new UserManager<User>(userStore, Options.Create(identityOptions), passwordHasher, userValidators, passwordValidators, keyNormalizer, userErrors, serviceProvider, null);
			return userManager;
		}

		private static RoleManager<Role> _NewRoleManager(DatabaseContext context)
		{
			var roleStore = new RoleStore<Role, DatabaseContext, Guid>(context);
			var roleValidators = new List<IRoleValidator<Role>>();
			var keyNormalizer = new UpperInvariantLookupNormalizer();
			var roleErrors = new IdentityErrorDescriber();
			var roleManager = new RoleManager<Role>(roleStore, roleValidators, keyNormalizer, roleErrors, null);
			return roleManager;
		}

		private static IEnumerable<HotelGroupTenant> _NewTestHotelGroupTenants(IMasterDatabaseContext masterDatabaseContext)
		{
			var roomchecking = new HotelGroupTenant
			{
				Id = Guid.NewGuid(),
				IsActive = true,
				Name = "Roomchecking",
				Key = "Roomchecking",
			};
			var group1 = new HotelGroupTenant
			{
				Id = Guid.NewGuid(),
				IsActive = true,
				Name = "Test hotel group 1",
				Key = "test_group_1"
			};
			var group2 = new HotelGroupTenant
			{
				Id = Guid.NewGuid(),
				IsActive = true,
				Name = "Test hotel group 2",
				Key = "test_group_2"
			};
			var group3 = new HotelGroupTenant
			{
				Id = Guid.NewGuid(),
				IsActive = true,
				Name = "Test hotel group 3",
				Key = "test_group_3"
			};

			roomchecking.ConnectionString = $"User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_{roomchecking.Key};Pooling=true;";
			group1.ConnectionString = $"User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_{group1.Key};Pooling=true;";
			group2.ConnectionString = $"User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_{group2.Key};Pooling=true;";
			group3.ConnectionString = $"User ID=postgres;Password=19921134;Host=localhost;Port=5432;Database=hgtest_{group3.Key};Pooling=true;";

			return new HotelGroupTenant[] {
				roomchecking,
				group1,
				group2,
				group3,
			};
		}

		private static IEnumerable<Hotel> _NewTenantHotels(int globalHotelCounter, int numberOfHotelsToAdd)
		{
			var hotels = new List<Hotel>();

			for (int i = 0; i < numberOfHotelsToAdd; i++)
			{
				var hotelId = (i + globalHotelCounter).ToString();
				if(globalHotelCounter == 0 && i == 0)
				{
					hotelId = "55d06baa99295b3a52000000";
				}
				else if(globalHotelCounter == 1 && i == 1)
				{
					hotelId = "5da303d2dc98ff001084c282";
				}
				hotels.Add(_NewTenantHotel(hotelId, globalHotelCounter + i + 1));
			}

			return hotels;
		}

		private static Hotel _NewTenantHotel(string hotelId, int hotelNumber)
		{
			return new Hotel
			{
				Id = hotelId,
				CreatedAt = DateTimeOffset.UtcNow,
				ModifiedAt = DateTime.UtcNow,
				Name = $"Hotel {hotelNumber}",
				//HotelPlugins = null,
				Rooms = null,
				Settings = null,

			};
		}

		private static User _NewTenantUser(string firstName, string lastName, string email, string userName)
		{
			return new User
			{
				Email = email,
				FirstName = firstName,
				LastName = lastName,
				UserName = userName
			};
		}

		private static IEnumerable<Role> _NewTenantRoles()
		{
			return new Role[]
			{
				//new Role { Id = Guid.NewGuid(), NormalizedName = "Admin", Name = "Admin" },
				//new Role { Id = Guid.NewGuid(), NormalizedName = "Tech", Name = "Tech" },
				//new Role { Id = Guid.NewGuid(), NormalizedName = "Cleaner", Name = "Cleaner" },
				//new Role { Id = Guid.NewGuid(), NormalizedName = "Host", Name = "Host" },
				//new Role { Id = Guid.NewGuid(), NormalizedName = "Runner", Name = "Runner" }
			};
		}

		private static List<Claim> _NewTenantUserClaims(User user, Guid hotelGroupId)
		{
			var claims = new List<Claim>
			{
				new Claim("given_name", user.FirstName),
				new Claim("family_name", user.LastName),
				new Claim("hotel_group_id", hotelGroupId.ToString()),
			};

			return claims;
		}

		private static async Task _SeedHotelGroupTenants(IMasterDatabaseContext context, IEnumerable<HotelGroupTenant> tenants)
		{
			foreach (var tenant in tenants)
			{
				var tenantExists = await context.HotelGroupTenants.AnyAsync(t => t.Name == tenant.Name);
				if (!tenantExists)
				{
					await context.HotelGroupTenants.AddAsync(tenant);
				}
			}
		}

		private static async Task _SeedTenantHotels(IDatabaseContext context, IEnumerable<Hotel> hotels)
		{
			foreach (var hotel in hotels)
			{
				var hotelExists = await context.Hotels.AnyAsync(h => h.Id == hotel.Id);
				if (!hotelExists)
				{
					await context.Hotels.AddAsync(hotel);
				}
			}
		}

		private static async Task _SeedTenantRoles(RoleManager<Role> roleManager, IEnumerable<Role> roles)
		{
			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role.Name))
				{
					await roleManager.CreateAsync(role);
				}
			}
		}

		private static async Task _SeedTenantUser(UserManager<User> userManager, User user, string password, string role, IEnumerable<Claim> claims)
		{
			if (await userManager.FindByNameAsync(user.UserName) == null)
			{
				var result = await userManager.CreateAsync(user, password);

				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, role);
					await userManager.AddClaimsAsync(user, claims);
				}
			}
		}
		#endregion
	}
}