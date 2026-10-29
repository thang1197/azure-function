using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace AzureFunction.Configuration.ApplicationInsight
{
    public class ApplicationInsightConfiguration : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Component.Version = "1.0.0";
            telemetry.Context.Cloud.RoleName = "azure-function";
            telemetry.Context.Cloud.RoleInstance = "task-management";
        }
    }
}