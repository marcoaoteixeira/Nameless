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

            self.Logging.AddOpenTelemetry(options => {
                if (!settings.OverrideOpenTelemetryLoggerConfiguration) {
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                }

                settings.ConfigureOpenTelemetryLogger?.Invoke(options);
            });

            // To add gRPC instrumentation for OpenTelemetry
            // Check: https://learn.microsoft.com/en-us/aspnet/core/grpc/diagnostics?view=aspnetcore-9.0
            var builder = self.Services
                              .AddOpenTelemetry()

                              .WithMetrics(metrics => {
                                  if (!settings.OverrideMeterProviderConfiguration) {
                                      metrics.AddMeter(settings.MetricMeters)
                                             .AddHttpClientInstrumentation()
                                             .AddRuntimeInstrumentation()
                                             .AddAspNetCoreInstrumentation();
                                  }

                                  settings.ConfigureMeterProvider?.Invoke(metrics);
                              })

                              .WithTracing(tracing => {
                                  if (!settings.OverrideTracerProviderConfiguration) {
                                      tracing.AddSource(settings.ActivitySources)
                                             .AddHttpClientInstrumentation(settings.ConfigureHttpClientTraceInstrumentation)
                                             .AddAspNetCoreInstrumentation(settings.ConfigureAspNetCoreTraceInstrumentation);
                                  }

                                  settings.ConfigureTracerProvider?.Invoke(tracing);
                              })

                              .ConfigureResource(resources => {
                                  if (!settings.OverrideResources) {
                                      /* default resources configuration */
                                  }

                                  settings.ConfigureResources?.Invoke(resources);
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