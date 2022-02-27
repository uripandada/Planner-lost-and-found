using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Common.Shared
{
    public class ApplicationInsightsTelemetryInitializer : ITelemetryInitializer
    {
        private string applicationRoleName;
        public ApplicationInsightsTelemetryInitializer(string applicationRoleName)
        {
            this.applicationRoleName= applicationRoleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                //set custom role name here
                telemetry.Context.Cloud.RoleName = this.applicationRoleName;
            }
        }
    }
}
