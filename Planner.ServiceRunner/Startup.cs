using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Application.Reservations.Commands.SynchronizeReservations;
using Planner.Application.ServicesJobs;
using Planner.Application.ServicesJobs.JobsDefinitions;
using Planner.Common.MediatrCustom;
using Planner.Common.Shared;
using Planner.Persistence;
using Planner.RccSynchronization;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Planner.ServiceRunner
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MasterDatabaseContext>(options => options.UseNpgsql(Configuration.GetConnectionString("MasterConnection"), b => b.MigrationsAssembly("Planner.Persistence")));
            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultTenantConnection"), b => b.MigrationsAssembly("Planner.Persistence")));

            services.AddScoped<IHotelGroupTenantProvider, HotelGroupTenantProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMediatR(mustImplementInterface: true, interfaceToImplement: typeof(Application.ServicesJobs.IAmServiceJobsApplicationHandler), typeof(IDatabaseContext).GetTypeInfo().Assembly);
            services.AddTransient<IRccApiClient, RccApiClient>();
            services.AddTransient<IReservationsSynchronizer, ReservationsSynchronizer>();
            services.AddTransient<IDatabaseContext, DatabaseContext>();
            services.AddTransient<IMasterDatabaseContext, MasterDatabaseContext>();

            services.AddQuartz(q =>
            {
                q.SchedulerId = "Runner";

                q.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    options.AllowDefaultConstructor = true;
                });

                // these are the defaults
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                q.ScheduleJob<SyncronizeReservationsJob>(trigger =>
                {
                    // 0 0/5 * 1/1 * ? * -> every 5 minutes -> FOR START IT IS LIKE THAT
#if DEBUG
                    trigger.StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(600).RepeatForever());
#else
                    trigger.WithSchedule(CronScheduleBuilder.CronSchedule("0 0/5 * 1/1 * ? *").WithMisfireHandlingInstructionDoNothing())
                                 .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Second));
#endif
                });


                q.ScheduleJob<SetRoomsStatusJob>(trigger =>
                 {
                     // 0 0 0/1 1/1 * ? * -> every hour forever
#if DEBUG
                     trigger.StartNow().WithSimpleSchedule(x => x.WithIntervalInSeconds(600).RepeatForever());
#else
                     trigger.WithSchedule(CronScheduleBuilder.CronSchedule("0 0 0/1 1/1 * ? *").WithMisfireHandlingInstructionDoNothing())
                                  .StartAt(DateBuilder.FutureDate(1, IntervalUnit.Second));
#endif
                 });

            }).AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
