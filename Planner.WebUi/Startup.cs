using FluentValidation.AspNetCore;
using IdentityModel.Jwk;
using Microsoft.AspNetCore.Authentication;
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
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Infrastructure.AutoMapper;
using Planner.Application.Infrastructure.PusherClient;
using Planner.Application.Interfaces;
using Planner.Application.Reservations.Commands.SynchronizeReservations;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.AppSettings;
using Planner.Common.Infrastructure;
using Planner.Common.MediatrCustom;
using Planner.Common.Shared;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using Planner.Infrastructure;
using Planner.Persistence;
using Planner.RccSynchronization;
using Planner.WebUi.Security;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Planner.WebUi
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<PusherChannelsSettings>(Configuration.GetSection(nameof(PusherChannelsSettings)));
			services.Configure<PusherBeamsSettings>(Configuration.GetSection(nameof(PusherBeamsSettings)));

			var rsaCertificate = new X509Certificate2(Path.Combine("plannerIdentity.pfx"), "WFi0VKnHBi5y1lWgZ4Bw");

			services.AddAutoMapper(new Assembly[] { typeof(AutoMapperProfile).GetTypeInfo().Assembly });

			services.AddDbContext<MasterDatabaseContext>(options =>
			{
				options.UseNpgsql(Configuration.GetConnectionString("MasterConnection"), b => b.MigrationsAssembly("Planner.Persistence"));
				options.UseOpenIddict();
			});
			services.AddDbContext<DatabaseContext>(options =>
			{
				options.UseNpgsql(Configuration.GetConnectionString("DefaultTenantConnection"), b => b.MigrationsAssembly("Planner.Persistence"));
			});

			services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<Role>()
				.AddEntityFrameworkStores<DatabaseContext>();

			services
				.AddAuthentication(options => {


				})
				.AddJwtBearer(options => {
					//options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
					//{
					//	ValidateIssuerSigningKey = true,
					//	IssuerSigningKey = new Microsoft.IdentityModel.Tokens.AsymmetricSecurityKey()

					//};
					var publicJwk = new JsonWebKey
					{
						Kid = "030276DC535A36600DDC63ABD5A3437FC5A47F6C",
						Alg = "RS256",
						E = "AQAB",
						N = "uDCldVaVi96-IGFOIC-qnEe6j2A_X9iZm3__RnFslXcNrabRrTE0QyfcCg4p2WnWuClcXBXmofwkmiFaipBO3cbn5QklQ29_I0Oqka6-ORBg9oUDHRbHdXyrGSDgDQ1WKNKNxjqQdC0eGIoSXipBuZFGMo5e24m764Ogg7jUMU-d8xHI_pNSyDAwbVOCsOX-vur6vBY7GKILmolepLGDh4JXra4DAzo4V3DQJump_INkKTfeTFYDjtHHbzOD3naHlBk79EvnkHxWM0-Imr_7EDS_pWhoajHkikzP75kEC92q9788-7xfgQUSsBFuQtq-rDw2XXx3RSIr-af-sI-Kaw",
						Kty = "RSA",
						Use = "sig",
						X5t = "AwJ23FNaNmAN3GOr1aNDf8Wkf2w",
						X5c = new string[] { "MIID/zCCAuegAwIBAgIUQEfP+6HwTUT1NitN4tp1m95WNbwwDQYJKoZIhvcNAQELBQAwgY4xCzAJBgNVBAYTAkZSMQ8wDQYDVQQIDAZGcmFuY2UxDjAMBgNVBAcMBVBhcmlzMRUwEwYDVQQKDAxSb29tY2hlY2tpbmcxDDAKBgNVBAsMA0RldjEQMA4GA1UEAwwHUGxhbm5lcjEnMCUGCSqGSIb3DQEJARYYbmlrb2xhLm1hcmtlemljQGxpdmUuY29tMB4XDTIxMDQwNDIwMDgzMVoXDTQ4MDgyMDIwMDgzMVowgY4xCzAJBgNVBAYTAkZSMQ8wDQYDVQQIDAZGcmFuY2UxDjAMBgNVBAcMBVBhcmlzMRUwEwYDVQQKDAxSb29tY2hlY2tpbmcxDDAKBgNVBAsMA0RldjEQMA4GA1UEAwwHUGxhbm5lcjEnMCUGCSqGSIb3DQEJARYYbmlrb2xhLm1hcmtlemljQGxpdmUuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAuDCldVaVi96+IGFOIC+qnEe6j2A/X9iZm3//RnFslXcNrabRrTE0QyfcCg4p2WnWuClcXBXmofwkmiFaipBO3cbn5QklQ29/I0Oqka6+ORBg9oUDHRbHdXyrGSDgDQ1WKNKNxjqQdC0eGIoSXipBuZFGMo5e24m764Ogg7jUMU+d8xHI/pNSyDAwbVOCsOX+vur6vBY7GKILmolepLGDh4JXra4DAzo4V3DQJump/INkKTfeTFYDjtHHbzOD3naHlBk79EvnkHxWM0+Imr/7EDS/pWhoajHkikzP75kEC92q9788+7xfgQUSsBFuQtq+rDw2XXx3RSIr+af+sI+KawIDAQABo1MwUTAdBgNVHQ4EFgQUvl3EhydEbFloEmCSosDsRD8HvhMwHwYDVR0jBBgwFoAUvl3EhydEbFloEmCSosDsRD8HvhMwDwYDVR0TAQH/BAUwAwEB/zANBgkqhkiG9w0BAQsFAAOCAQEAW1pPDmOOeDirOb8yAoJFq3asPll/RNNij72cXPqPr4zUSPun7PoqjCNYIUf4AkTCHKzvogmnPH6LCwmifoo8GUzxvck3SwElrcqvzQfP0LZi83YbYh5pLIDz0+ku5vH2TNjIWYgdGTV4KxH5Fy6L+zLNg0iqCu//yRC3zIo2QkVk4Mp7arJNdFkOZdDJwLKljQWFgIyiXu5PLdTDgJGvgC7jPwkTMb7dc9CRzTj7EbprfAhtA2xMW4RahMEBR0T+3xpF/l8eld9uD08QtkfE3Pt1fwoT0VRC94DetvvSKvywphKNDEFZHUwav3lmhXzp7vX6tazRBnjyYMEIMZvSAw==" },
					};

					options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new Microsoft.IdentityModel.Tokens.X509SecurityKey(rsaCertificate)
					};

				});

			services
				.AddIdentityCore<User>(identityOptions =>
				{
					// configure identity options
					identityOptions.Password.RequireDigit = false;
					identityOptions.Password.RequireLowercase = false;
					identityOptions.Password.RequireUppercase = false;
					identityOptions.Password.RequireNonAlphanumeric = false;
					identityOptions.Password.RequiredLength = 4;
					
					identityOptions.User.RequireUniqueEmail = false;
					identityOptions.SignIn.RequireConfirmedEmail = false;
					identityOptions.SignIn.RequireConfirmedAccount = false;
					identityOptions.SignIn.RequireConfirmedPhoneNumber = false;
				})
				.AddRoles<Role>()
				.AddUserManager<UserManager<User>>()
				.AddRoleManager<RoleManager<Role>>()
				.AddSignInManager<SignInManager<User>>()
				.AddEntityFrameworkStores<DatabaseContext>()
				.AddDefaultTokenProviders();

			// Configure Identity to use the same JWT claims as OpenIddict instead
			// of the legacy WS-Federation claims it uses by default (ClaimTypes),
			// which saves you from doing the mapping in your authorization controller.
			services.Configure<IdentityOptions>(options =>
			{
				options.ClaimsIdentity.UserNameClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Name;
				options.ClaimsIdentity.UserIdClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject;
				options.ClaimsIdentity.RoleClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Role;
			});



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
					//options.SetAccessTokenLifetime(TimeSpan.FromDays(1));
					options.SetAccessTokenLifetime(TimeSpan.FromDays(90));
					//options.SetAccessTokenLifetime(TimeSpan.FromMinutes(1));

					options
						.AllowAuthorizationCodeFlow()
						.RequireProofKeyForCodeExchange();

					options
						.AllowClientCredentialsFlow();

					options
						.AllowRefreshTokenFlow();

					options
						.AllowPasswordFlow();

					options
						.SetAuthorizationEndpointUris("/connect/authorize")
						.SetTokenEndpointUris("/connect/token")
						.SetLogoutEndpointUris("/connect/logout")
						.SetUserinfoEndpointUris("/connect/userinfo");

					// Encryption and signing of tokens
					options
						.AddEncryptionCertificate(rsaCertificate)
						.AddSigningCertificate(rsaCertificate)
						.DisableAccessTokenEncryption();

					// Register scopes (permissions)
					options
						.RegisterScopes(OpenIddict.Abstractions.OpenIddictConstants.Permissions.Scopes.Email)
						.RegisterScopes(OpenIddict.Abstractions.OpenIddictConstants.Permissions.Scopes.Profile)
						.RegisterScopes(OpenIddict.Abstractions.OpenIddictConstants.Permissions.Scopes.Roles)
						.RegisterScopes("hotelgroupid")
						.RegisterScopes("hotelid")
						.RegisterScopes("api");

					// Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
					options
						.UseAspNetCore()
						.EnableTokenEndpointPassthrough()
						.EnableLogoutEndpointPassthrough()
						.EnableAuthorizationEndpointPassthrough()
						.EnableUserinfoEndpointPassthrough();

					options.DisableScopeValidation();
				}
			);


			services.AddMvc()
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<IDatabaseContext>())
				//.AddJsonOptions(o => {
				//	//o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
				//	  //.ReferenceHandling = ReferenceHandling.Preserve
				//})
				;


			services.AddSwaggerDocument();

			// Suppress model state invalid filter because we plugged in RequestValidationBehavior
			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.SuppressModelStateInvalidFilter = true;
			});

			services.AddCors();
			services.AddControllersWithViews();
			//services.AddRazorPages();
			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});


			services.AddMediatR(mustImplementInterface: true, interfaceToImplement: typeof(Application.IAmWebApplicationHandler), typeof(IDatabaseContext).GetTypeInfo().Assembly);

			services.AddSignalR(o => { o.EnableDetailedErrors = true; });

			services.AddScoped<IHotelGroupTenantProvider, HotelGroupTenantProvider>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			//services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
			services.AddTransient<MasterDatabaseContext, MasterDatabaseContext>();
			services.AddTransient<IMasterDatabaseContext, MasterDatabaseContext>();
			services.AddTransient<DatabaseContext, DatabaseContext>();
			services.AddTransient<IDatabaseContext, DatabaseContext>();
			services.AddTransient<UserManager<User>, UserManager<User>>();
			services.AddTransient<RoleManager<Role>, RoleManager<Role>>();
			services.AddTransient<SignInManager<User>, SignInManager<User>>();
			services.AddTransient<IFileService, FileService>();
			services.AddTransient<ISystemTaskGenerator, SystemTaskGenerator>();
			services.AddTransient<IRccApiClient, RccApiClient>();
			services.AddTransient<IReservationsSynchronizer, ReservationsSynchronizer>();
			services.AddTransient<ICleaningProvider, CleaningProvider>();
			services.AddTransient<ICleaningGeneratorService, CleaningGeneratorService>();
			services.AddTransient<ISystemEventsService, SystemEventsService>();
			services.AddTransient<IPusherBeamsClient, PusherBeamsClient>();
			services.AddTransient<IPusherChannelsClient, PusherChannelsClient>();

			services.ConfigurePolicies();
			services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

			//services.ConfigureApplicationCookie(options =>
			//{
			//	options.LoginPath = new PathString("/authentication/login");
			//	//other properties
			//});

			//services.AddSingleton<ICorsPolicyService>((container) =>
			//{
			//	var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
			//	return new DefaultCorsPolicyService(logger)
			//	{
			//		AllowAll = true,
			//		//AllowedOrigins = { "https://foo", "https://bar" }
			//	};
			//});
			//services.AddSingleton<ITelemetryInitializer>(new ApplicationInsightsTelemetryInitializer("PlannerWeb"));
			//         services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			if (!env.IsDevelopment())
			{
				app.UseSpaStaticFiles();
			}

			app.UseRouting();

			var forwardingOptions = new ForwardedHeadersOptions()
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			};
			forwardingOptions.KnownNetworks.Clear(); // Loopback by default, this should be temporary
			forwardingOptions.KnownProxies.Clear(); // Update to include

			app.UseForwardedHeaders(forwardingOptions);

			app.UseAuthentication();
			//app.UseIdentityServer();
			app.UseAuthorization();

			// TODO: UNCOMMENT THIS PART ONCE THE JONATHAN HAS THE INFO
			// TODO: UNCOMMENT THIS PART ONCE THE JONATHAN HAS THE INFO
			// TODO: UNCOMMENT THIS PART ONCE THE JONATHAN HAS THE INFO
			#if DEBUG
			//// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseOpenApi();
			app.UseSwaggerUi3();
			#endif

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller}/{action=Index}/{id?}");

				//endpoints.MapHub<Application.CleaningPlans.Commands.GenerateCpsatCleaningPlan.CleaningPlanHub>("/hubs/cleaning-plan");

				endpoints.MapHub<Application.Infrastructure.Signalr.Hubs.RoomsOverviewHub>("/hubs/rooms-overview");
				endpoints.MapHub<Application.Infrastructure.Signalr.Hubs.UserHub>("/hubs/users");
				endpoints.MapHub<Application.Infrastructure.Signalr.Hubs.CleaningPlannerHub>("/hubs/cleanings");
				endpoints.MapHub<Application.Infrastructure.Signalr.Hubs.CpsatCleaningPlannerHub>("/hubs/cpsat-cleaning-planner");
				endpoints.MapHub<Application.Infrastructure.Signalr.Hubs.TaskHub>("/hubs/tasks");
			});

			app.UseSpa(spa =>
			{
				// To learn more about options for serving an Angular SPA from ASP.NET Core,
				// see https://go.microsoft.com/fwlink/?linkid=864501

				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment())
				{
					//spa.UseAngularCliServer(npmScript: "start");
					spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
				}
			});

			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDQxMzc1QDMxMzkyZTMxMmUzMElhZm9hdno3czlxYVpWWGpnT01RZXJBbGc5akxXNmZ3UGs5WnFpR0toTkU9;NDQxMzc2QDMxMzkyZTMxMmUzMG1aaFNocXNJQmNhNi9PNUcvSC9sZ0FwdEJKS1dBdFY1djAxWFBUeXYxTUE9");
		}


	
	}
}
