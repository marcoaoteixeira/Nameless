using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Microservices.Application.Monitoring;
using Nameless.Microservices.Infrastructure.Monitoring;
using Nameless.Web.Correlation;
using Nameless.Web.HealthChecks;
using Nameless.Web.OpenTelemetry;
using Serilog;

namespace Nameless.Microservices.Web.Configs;

public static class MonitoringConfig {
    public static WebApplicationBuilder ConfigureMonitoring(this WebApplicationBuilder self) {
        self.RegisterOpenTelemetry()
            .RegisterHealthChecks()
            .RegisterLogging()
            .RegisterCorrelationAccessor();

        self.Services.TryAddSingleton<IActivitySourceManager, ActivitySourceManager>();

        return self;
    }
    public static WebApplication UseMonitoring(this WebApplication self) {
        self.UseHealthChecks(self.Environment);
        self.UseCorrelation();
        self.UseSerilogRequestLogging();

        return self;
    }
}
