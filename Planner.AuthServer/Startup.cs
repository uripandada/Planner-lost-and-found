using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Infrastructure.AutoMapper;
using Planner.Application.Interfaces;
using Planner.Application.Reservations.Commands.SynchronizeReservations;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.Infrastructure;
using Planner.Common.MediatrCustom;
using Planner.Common.Shared;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using Planner.Persistence;
using Planner.RccSynchronization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.AuthServer
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
				{
					options.LoginPath = "/account/login";
				});

			services.AddDbContext<MasterDatabaseContext>(options =>
			{
				options.UseNpgsql(Configuration.GetConnectionString("MasterConnection"), b => b.MigrationsAssembly("Planner.Persistence"));
				options.UseOpenIddict();
			});

			services.AddDbContext<DatabaseContext>(options => {
				options.UseNpgsql(Configuration.GetConnectionString("DefaultTenantConnection"), b => b.MigrationsAssembly("Planner.Persistence"));
			});
			services.AddScoped<IHotelGroupTenantProvider, HotelGroupTenantProvider>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddOpenIddict()
				// Register the OpenIddict core components.
				.AddCore(options =>
				{
					// Configure OpenIddict to use the EF Core stores/models.
					options.UseEntityFrameworkCore()
						.UseDbContext<MasterDatabaseContext>();
				})
				// Register the OpenIddict server components.
				.AddServer(options =>
				{
					options
						.AllowAuthorizationCodeFlow()
						.RequireProofKeyForCodeExchange();

					options
						.AllowClientCredentialsFlow();

					options
						.AllowRefreshTokenFlow();

					options
						.SetAuthorizationEndpointUris("/connect/authorize")
						.SetTokenEndpointUris("/connect/token")
						.SetUserinfoEndpointUris("/connect/userinfo");

					// Encryption and signing of tokens
					options
						.AddEphemeralEncryptionKey()
						.AddEphemeralSigningKey()
						.DisableAccessTokenEncryption();

					// Register scopes (permissions)
					options
						.RegisterScopes("api");

					// Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
					options
						.UseAspNetCore()
						.EnableTokenEndpointPassthrough()
						.EnableAuthorizationEndpointPassthrough()
						.EnableUserinfoEndpointPassthrough();
				}
			);

			services.AddTransient<MasterDatabaseContext, MasterDatabaseContext>();
			services.AddTransient<IMasterDatabaseContext, MasterDatabaseContext>();
			services.AddTransient<DatabaseContext, DatabaseContext>();
			services.AddTransient<IDatabaseContext, DatabaseContext>();

			services.AddHostedService<TestIdentityServerClientSeeder>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}
	}

	public class TestIdentityServerClientSeeder : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;

		public TestIdentityServerClientSeeder(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = _serviceProvider.CreateScope();

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

			if (await manager.FindByClientIdAsync("roomchecking-client", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "roomchecking-client",
					ClientSecret = "roomchecking-client-secret",
					DisplayName = "Roomchecking API client",
					PostLogoutRedirectUris = { new Uri("https://localhost:5001/authentication/logout-callback") },
					RedirectUris = { new Uri("https://localhost:5001/authentication/login-callback") },
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

			if (await manager.FindByClientIdAsync("roomchecking-client-testing", cancellationToken) is null)
			{
				await manager.CreateAsync(new OpenIddictApplicationDescriptor
				{
					ClientId = "roomchecking-client-testing",
					//ClientSecret = "roomchecking-client-secret",
					DisplayName = "Roomchecking API - Testing environment",
					PostLogoutRedirectUris = { new Uri("https://test-next.roomchecking.com//authentication/logout-callback") },
					RedirectUris = { new Uri("https://test-next.roomchecking.com//authentication/login-callback") },
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

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
