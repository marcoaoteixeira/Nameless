using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Nameless.Web.OpenTelemetry;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <summary>
    ///     Registers OpenTelemetry services in the application builder.
    /// </summary>
    /// <param name="self">The current <see cref="WebApplicationBuilder"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    ///     The current <see cref="WebApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    public static WebApplicationBuilder RegisterOpenTelemetry(this WebApplicationBuilder self, Action<OpenTelemetryOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new OpenTelemetryOptions();

        innerConfigure(options);

        self.Logging.AddOpenTelemetry(options.ConfigureLogger);

        // To add gRPC instrumentation for OpenTelemetry
        // Check: https://learn.microsoft.com/en-us/aspnet/core/grpc/diagnostics?view=aspnetcore-9.0
        self.Services
            .AddOpenTelemetry()
            .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation()
                                           .AddHttpClientInstrumentation()
                                           .AddRuntimeInstrumentation())
            .WithTracing(tracing => tracing.AddSource(self.Environment.ApplicationName)
                                           .AddAspNetCoreInstrumentation(options.ConfigureAspNetCoreTraceInstrumentation)
                                           .AddHttpClientInstrumentation(options.ConfigureHttpClientTraceInstrumentation));

        var configValue = self.Configuration[OpenTelemetryConstants.OPEN_TELEMETRY_EXPORTER_ENDPOINT];
        var useOpenTelemetryExporters = !string.IsNullOrWhiteSpace(configValue);

        if (useOpenTelemetryExporters) {
            self.Services
                .AddOpenTelemetry()
                .UseOtlpExporter();
        }

        return self;
    }
}