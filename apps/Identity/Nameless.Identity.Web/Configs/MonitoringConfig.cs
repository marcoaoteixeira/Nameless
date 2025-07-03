using Nameless.Identity.Web.Infrastructure;
using Nameless.Web.Discoverability;
using Nameless.Web.HealthChecks;
using Nameless.Web.OpenTelemetry;
using Serilog;

namespace Nameless.Identity.Web.Configs;

public static class MonitoringConfig {
    public static WebApplicationBuilder ConfigureMonitoringServices(this WebApplicationBuilder self) {
        self.Services.AddHttpContextAccessor();

        self.RegisterOpenTelemetry()
            .RegisterHealthChecks()
            .RegisterDiscoverability();

        self.Services.AddSerilog((_, config) => {
            config.ReadFrom
                  .Configuration(self.Configuration)

                  .Enrich.FromLogContext()
                  .Enrich.WithCorrelationId()

                  .WriteTo.Console()
                  .WriteTo.OpenTelemetry(options => {
                      var openTelemetryExporterEndpoint = self.Configuration[OpenTelemetryConstants.OPEN_TELEMETRY_EXPORTER_ENDPOINT];
                      if (!string.IsNullOrWhiteSpace(openTelemetryExporterEndpoint)) {
                          options.Endpoint = openTelemetryExporterEndpoint;
                      }
                  });
        });

        return self;
    }

    public static WebApplication UseMonitoringServices(this WebApplication self) {
        self.UseHealthChecks(self.Environment);
        self.UseSerilogRequestLogging();

        return self;
    }
}
