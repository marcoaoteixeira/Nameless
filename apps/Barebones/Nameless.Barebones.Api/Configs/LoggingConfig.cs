using Nameless.Web.OpenTelemetry;
using Serilog;
using Serilog.Events;

namespace Nameless.Barebones.Api.Configs;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods for configuring accessors.
/// </summary>
public static class LoggingConfig {
    /// <summary>
    ///     Configures the services required for accessors in the
    ///     <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="WebApplicationBuilder"/> so other actions
    ///     can be chained.
    /// </returns>
    /// <remarks>
    ///     Responsible for registering services that provide access to the
    ///     current HTTP context and correlation ID.
    /// </remarks>
    public static WebApplicationBuilder RegisterLogging(this WebApplicationBuilder self) {
        self.Services.AddSerilog((_, config) => {
            config.ReadFrom
                  .Configuration(self.Configuration)

                  .Enrich.FromLogContext()
                  //.Enrich.WithCorrelationId(provider.GetRequiredService<ICorrelationAccessor>())

                  .WriteTo.Console()
                  .WriteTo.OpenTelemetry(options => {
                      var openTelemetryExporterEndpoint = self.Configuration[OpenTelemetryConstants.OPEN_TELEMETRY_EXPORTER_ENDPOINT];
                      if (!string.IsNullOrWhiteSpace(openTelemetryExporterEndpoint)) {
                          options.Endpoint = openTelemetryExporterEndpoint;
                      }
                  })

                  .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                  .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                  .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
        });

        return self;
    }
}
