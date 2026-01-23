using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Nameless.Web.Observability;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <summary>
    ///     Registers OpenTelemetry services in the application builder.
    /// </summary>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type that implements <see cref="IHostApplicationBuilder"/>.
    /// </typeparam>
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    ///     The current <typeparamref name="THostApplicationBuilder"/>
    ///     instance so other actions can be chained.
    /// </returns>
    public static THostApplicationBuilder RegisterOpenTelemetry<THostApplicationBuilder>(
        this THostApplicationBuilder self, Action<OpenTelemetryOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        var innerConfigure = configure ?? (_ => { });
        var options = new OpenTelemetryOptions();

        innerConfigure(options);

        self.Logging.AddOpenTelemetry(options.ConfigureLogger);

        // To add gRPC instrumentation for OpenTelemetry
        // Check: https://learn.microsoft.com/en-us/aspnet/core/grpc/diagnostics?view=aspnetcore-9.0
        var openTelemetryBuilder = self.Services
                                       .AddOpenTelemetry()
                                       .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation()
                                                                      .AddHttpClientInstrumentation()
                                                                      .AddRuntimeInstrumentation())
                                       .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation(options.ConfigureAspNetCoreTraceInstrumentation)
                                                                      .AddHttpClientInstrumentation(options.ConfigureHttpClientTraceInstrumentation)
                                                                      .AddSource(options.ActivitySources))
                                       .ConfigureResource(options.ConfigureResources);

        var configValue = self.Configuration[OpenTelemetryConstants.OPEN_TELEMETRY_EXPORTER_ENDPOINT];
        var useOpenTelemetryExporters = !string.IsNullOrWhiteSpace(configValue);

        if (useOpenTelemetryExporters) {
            openTelemetryBuilder.UseOtlpExporter();
        }

        return self;
    }
}