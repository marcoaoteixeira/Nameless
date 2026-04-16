using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Logging.Serilog;
using Nameless.Web.Serilog;
using Serilog;
using Serilog.Events;

namespace Nameless.Web.Hosting.Configs;

public static class LoggingConfig {
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
        public WebApplicationBuilder ConfigureLogging(WebHostSettings settings) {
            if (settings.DisableLogging) { return self; }

            self.Services.RegisterSerilog(
                settings.SerilogRegistrationConfiguration ?? DefaultSerilogConfiguration
            );

            return self;
        }
    }

    private static void DefaultSerilogConfiguration(SerilogRegistration registration) {
        registration.OverrideSerilogConfiguration = (provider, config) => {
            var configuration = provider.GetRequiredService<IConfiguration>();

            config
                // Defines from where it should get its configurations.
                .ReadFrom.Configuration(configuration)

                // Enrich the log message with data from other locations.
                .Enrich.FromLogContext()

                // Enrich with request CorrelationId
                .Enrich.WithCorrelationId(provider)

                // Write to console sink
                .WriteTo.Console()

                // Write to OpenTelemetry sink
                .WriteTo.OpenTelemetry(opts => opts.Endpoint = configuration[
                    CoreConstants.OpenTelemetry.ExporterEndpointConfigName
                ])

                // Override minimum levels
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
        };
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the logging service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseLogging(WebHostSettings settings) {
            if (settings.DisableLogging) { return self; }

            self.UseSerilogRequestLogging();

            return self;
        }
    }
}