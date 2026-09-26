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
        /// <param name="configure">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterOpenTelemetry(Action<OpenTelemetryRegistration>? configure = null) {
            var registration = ActionHelper.FromDelegate(configure);

            self.Logging.AddOpenTelemetry(options => {
                if (!registration.OverrideOpenTelemetryLoggerConfiguration) {
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                }

                registration.ConfigureOpenTelemetryLogger?.Invoke(options);
            });

            // To add gRPC instrumentation for OpenTelemetry
            // Check: https://learn.microsoft.com/en-us/aspnet/core/grpc/diagnostics?view=aspnetcore-9.0
            var builder = self.Services
                              .AddOpenTelemetry()

                              .WithMetrics(metrics => {
                                  if (!registration.OverrideMeterProviderConfiguration) {
                                      metrics.AddMeter(registration.MetricMeters)
                                             .AddHttpClientInstrumentation()
                                             .AddRuntimeInstrumentation()
                                             .AddAspNetCoreInstrumentation();
                                  }

                                  registration.ConfigureMeterProvider?.Invoke(metrics);
                              })

                              .WithTracing(tracing => {
                                  if (!registration.OverrideTracerProviderConfiguration) {
                                      tracing.AddSource(registration.ActivitySources)
                                             .AddHttpClientInstrumentation(registration.ConfigureHttpClientTraceInstrumentation)
                                             .AddAspNetCoreInstrumentation(registration.ConfigureAspNetCoreTraceInstrumentation);
                                  }

                                  registration.ConfigureTracerProvider?.Invoke(tracing);
                              })

                              .ConfigureResource(resources => {
                                  if (!registration.OverrideResources) {
                                      /* default resources configuration */
                                  }

                                  registration.ConfigureResources?.Invoke(resources);
                              });

            var openTelemetryEndpointUrl = self.Configuration[
                StaticData.OpenTelemetry.ExporterEndpointConfigKey
            ];
            if (!string.IsNullOrWhiteSpace(openTelemetryEndpointUrl)) {
                builder.UseOtlpExporter();
            }

            return self;
        }
    }
}