using FluentValidation.AspNetCore;
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
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.Reservations.Commands.SynchronizeReservations;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.Extensions;
using Planner.Common.Infrastructure;
using Planner.Common.MediatrCustom;
using Planner.Common.Shared;
using Planner.Domain.Entities;
using Planner.Infrastructure;
using Planner.Persistence;
using Planner.RccSynchronization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Planner.WebAdminUi
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		private System.Type[] _GetAdminMediatrTypes()
		{
			var adminNamespacePrefix = "Planner.Application.Admin.";

			var adminTypes = typeof(IDatabaseContext).GetTypeInfo()
				.Assembly
				.GetTypes()
				.Where(t =>
					t.IsClass && t.Namespace.IsNotNull() &&
					t.Namespace.StartsWith(adminNamespacePrefix) &&
					t.GetInterfaces().Any(i => i.Name.StartsWith("IRequestHandler"))
					)
				.ToArray();

			return new System.Type[] { adminTypes[0] };
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//var mediatrTypes = this._GetAdminMediatrRequests();

			//services.AddAutoMapper(new Assembly[] { typeof(AutoMapperProfile).GetTypeInfo().Assembly });

			// Sets the DB context for the application and defines what entries to read from appsettings.json
			//services.AddDbContext<MasterDatabaseContext>(options => options.UseNpgsql(Configuration.GetConnectionString("MasterConnection"), b => b.MigrationsAssembly("Planner.Persistence")));
			//services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultTenantConnection"), b => b.MigrationsAssembly("Planner.Persistence")));

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

			services.AddDefaultIdentity<MasterUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<Role>()
				.AddEntityFrameworkStores<MasterDatabaseContext>();

			var rsaCertificate = new X509Certificate2(Path.Combine("plannerIdentity.pfx"), "WFi0VKnHBi5y1lWgZ4Bw");

			services.AddIdentityServer()
				.AddSigningCredential(rsaCertificate)
				.AddApiAuthorization<MasterUser, MasterDatabaseContext>(options =>
				{
					//Configure Identity Server to put the name and role claims into the ID token and access token.
					options.IdentityResources["openid"].UserClaims.Add("name");
					options.ApiResources.Single().UserClaims.Add("name");

					var client = options.Clients.First(c => c.ClientName == "PlannerWebAdminUi");

					client.UpdateAccessTokenClaimsOnRefresh = true;
				});

			//services.AddScoped<IProfileService, ProfileService>();

			// Prevent the default mapping for roles in the JWT token handler
			//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

			services.AddOpenIddict()
				// Register the OpenIddict core components.
				.AddCore(options =>
				{
					// Configure OpenIddict to use the EF Core stores/models.
					options.UseEntityFrameworkCore()
						.UseDbContext<MasterDatabaseContext>();
				});

			services.AddAuthentication()
				.AddIdentityServerJwt();


			services
				.AddIdentityCore<MasterUser>(identityOptions =>
				{
					// configure identity options
					identityOptions.Password.RequireDigit = false;
					identityOptions.Password.RequireLowercase = false;
					identityOptions.Password.RequireUppercase = false;
					identityOptions.Password.RequireNonAlphanumeric = false;
					identityOptions.Password.RequiredLength = 8;
					identityOptions.User.RequireUniqueEmail = true;
					identityOptions.SignIn.RequireConfirmedEmail = false;
					identityOptions.SignIn.RequireConfirmedAccount = false;
					identityOptions.SignIn.RequireConfirmedPhoneNumber = false;
				})
				.AddRoles<Role>()
				.AddUserManager<UserManager<MasterUser>>()
				.AddRoleManager<RoleManager<Role>>()
				.AddSignInManager<SignInManager<MasterUser>>()
				.AddEntityFrameworkStores<MasterDatabaseContext>()
				.AddDefaultTokenProviders();

			services.AddMvc()
				.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Application.IAmWebApplicationHandler>());

			services.AddSwaggerDocument();

			// Suppress model state invalid filter because we plugged in RequestValidationBehavior
			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.SuppressModelStateInvalidFilter = true;
			});

			services.AddControllersWithViews();
			services.AddRazorPages();
			// In production, the Angular files will be served from this directory
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});

			services.AddMediatR(mustImplementInterface: true, interfaceToImplement: typeof(Application.Admin.IAmAdminApplicationHandler), typeof(IDatabaseContext).GetTypeInfo().Assembly);

			services.AddTransient<MasterDatabaseContext, MasterDatabaseContext>();
			services.AddTransient<IMasterDatabaseContext, MasterDatabaseContext>();
			services.AddTransient<DatabaseContext, DatabaseContext>();
			services.AddTransient<IDatabaseContext, DatabaseContext>();
			services.AddTransient<UserManager<MasterUser>, UserManager<MasterUser>>();
			services.AddTransient<RoleManager<Role>, RoleManager<Role>>();
			services.AddTransient<SignInManager<MasterUser>, SignInManager<MasterUser>>();
			services.AddTransient<IFileService, FileService>();
			services.AddTransient<ISystemTaskGenerator, SystemTaskGenerator>();
			services.AddTransient<IRccApiClient, RccApiClient>();
			services.AddTransient<IReservationsSynchronizer, ReservationsSynchronizer>();
			services.AddTransient<ICleaningProvider, CleaningProvider>();
			services.AddTransient<ITenantManager, TenantManager>();
			services.AddTransient<ICleaningGeneratorService, CleaningGeneratorService>();

			services.AddTransient<IPasswordHasher<Domain.Entities.User>, PasswordHasher<Domain.Entities.User>>();
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);






            //services.AddSingleton<ITelemetryInitializer>(new ApplicationInsightsTelemetryInitializer("PlannerAdmin"));
            //services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);


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
			app.UseIdentityServer();
			app.UseAuthorization();

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
				endpoints.MapRazorPages();
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
		}
	}

	//public class DummyUserIdentityImplementation : IUserStore<User>, IUserConfirmation<User>,  IUserClaimsPrincipalFactory<User>, IPasswordHasher<User>
	//{
	//	public Task<bool> IsConfirmedAsync(UserManager<User> manager, User user)
	//	{
	//		throw new NotImplementedException();
	//	}
	//	public Task<ClaimsPrincipal> CreateAsync(User user)
	//	{
	//		throw new NotImplementedException();
	//	}
	//	public string HashPassword(User user, string password)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
	//	{
	//		throw new NotImplementedException();
	//	}
	//	public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public void Dispose()
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
	//	{
	//		throw new NotImplementedException();
	//	}
	//}
}
