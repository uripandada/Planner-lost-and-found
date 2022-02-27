using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.ServicesJobs.Commands.SetRoomsStatusCommand;
using Planner.Common.Shared;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.ServicesJobs.JobsDefinitions
{
    public class SetRoomsStatusJob: IJob
    {
        private readonly IMediator mediator;
        private readonly IMasterDatabaseContext masterDatabaseContext;
        private readonly IHotelGroupTenantProvider hotelGroupTenantProvider;
        private readonly TelemetryClient telemetryClient;

        public SetRoomsStatusJob(IMediator mediator, IMasterDatabaseContext masterDatabaseContext, IHotelGroupTenantProvider hotelGroupTenantProvider, TelemetryClient telemetryClient)
        {
            this.mediator = mediator;
            this.masterDatabaseContext = masterDatabaseContext;
            this.hotelGroupTenantProvider = hotelGroupTenantProvider;
            this.telemetryClient = telemetryClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var tennants = await this.masterDatabaseContext.HotelGroupTenants.ToListAsync();

            foreach (var currentTennant in tennants)
            {
                try
                {
                    hotelGroupTenantProvider.SetTenantId(currentTennant.Id);
                    await this.mediator.Send(new SetRoomsStatusCommand());
                }
                catch (Exception e)
                {
                    this.telemetryClient.TrackException(e);
                }
            }
        }
    }
}
