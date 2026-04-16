using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nameless.Helpers;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Nameless.Web.Observability;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/>.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Registers OpenTelemetry services in the application builder.
        /// </summary>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterOpenTelemetry(Action<OpenTelemetryRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.Logging.AddOpenTelemetry(settings.ConfigureLogger);

            // To add gRPC instrumentation for OpenTelemetry
            // Check: https://learn.microsoft.com/en-us/aspnet/core/grpc/diagnostics?view=aspnetcore-9.0
            var builder = self.Services
                              .AddOpenTelemetry()

                              .WithMetrics(metrics => metrics.AddMeter(settings.MetricMeters)
                                                             .AddHttpClientInstrumentation()
                                                             .AddRuntimeInstrumentation()
                                                             .AddAspNetCoreInstrumentation())

                              .WithTracing(tracing => tracing.AddSource(settings.ActivitySources)
                                                             .AddHttpClientInstrumentation(settings.ConfigureHttpClientTraceInstrumentation)
                                                             .AddAspNetCoreInstrumentation(settings.ConfigureAspNetCoreTraceInstrumentation))

                              .ConfigureResource(settings.ConfigureResources);

            var openTelemetryEndpointUrl = self.Configuration[
                CoreConstants.OpenTelemetry.ExporterEndpointConfigName
            ];
            if (!string.IsNullOrWhiteSpace(openTelemetryEndpointUrl)) {
                builder.UseOtlpExporter();
            }

            return self;
        }
    }
}