using Nameless.Web.OpenTelemetry;
using Serilog;
using Serilog.Events;

namespace Nameless.Microservices.App.Configs;

public static class LoggingConfig {
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder self) {
        self.Services.AddSerilog((_, config) => {
            config
                // Defines from where to read the configuration
                .ReadFrom.Configuration(self.Configuration)

                // Enriches log events with additional context information
                .Enrich.FromLogContext()

                // Write logs to the console
                .WriteTo.Console()

                // Writes to OpenTelemetry sink
                .WriteTo.OpenTelemetry(options => {
                    var openTelemetryExportEndpoint = self.Configuration[OpenTelemetryConstants.OPEN_TELEMETRY_EXPORTER_ENDPOINT];
                    if (!string.IsNullOrEmpty(openTelemetryExportEndpoint)) {
                        options.Endpoint = openTelemetryExportEndpoint;
                    }
                })

                // Override minimum log level
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
        });

        return self;
    }

    public static WebApplication UseLogging(this WebApplication self) {
        // Use Serilog for logging
        self.UseSerilogRequestLogging();

        return self;
    }
}
