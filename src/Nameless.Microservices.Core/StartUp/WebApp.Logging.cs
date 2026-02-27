using Microsoft.AspNetCore.Builder;
using Nameless.Microservices.Infrastructure.Serilog;
using Nameless.Web.Observability;
using Serilog;
using Serilog.Events;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Serilog-based logging for the application, including
        ///     console and OpenTelemetry sinks, and enriches log events with
        ///     contextual information.
        /// </summary>
        /// <remarks>
        ///     This method sets up Serilog as the logging provider, reading
        ///     configuration from the application's configuration sources. It
        ///     enriches log events with contextual data such as correlation IDs
        ///     and log context, and writes logs to both the console and
        ///     OpenTelemetry endpoints if configured. Minimum log levels for
        ///     common ASP.NET Core components are overridden to reduce log
        ///     verbosity.
        /// </remarks>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder LoggingConfig(WebAppSettings settings) {
            if (settings.DisableLogging) { return self; }

            self.Services.AddSerilog((provider, config) => {
                config
                    // Defines from where it should get its configurations.
                    .ReadFrom.Configuration(self.Configuration)

                    // Enrich the log message with data from other locations.
                    .Enrich.FromLogContext()

                    // Enrich with request CorrelationId
                    .Enrich.WithCorrelationId(provider)

                    // Write to console sink
                    .WriteTo.Console()

                    // Write to OpenTelemetry sink
                    .WriteTo.OpenTelemetry(opts => opts.Endpoint = self.Configuration[
                        OpenTelemetryConstants.OPEN_TELEMETRY_EXPORTER_ENDPOINT
                    ])

                    // Override minimum levels
                    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
            });

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the logging service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseLogging(WebAppSettings settings) {
            if (settings.DisableLogging) { return self; }

            self.UseSerilogRequestLogging();

            return self;
        }
    }
}