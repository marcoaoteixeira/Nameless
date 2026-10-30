using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.Hosting.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers OpenTelemetry (tracing, metrics and logging).
/// </summary>
public class OpenTelemetryWebHostFeature : HostFeature<OpenTelemetryFeatureOptions> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);

        SetOpenTelemetryLogger(builder, options);
        SetOpenTelemetry(builder, options);
    }

    private static void SetOpenTelemetryLogger(IHostApplicationBuilder self, OpenTelemetryFeatureOptions featureOptions) {
        if (!featureOptions.UseOpenTelemetryLogging) { return; }

        self.Logging.AddOpenTelemetry(featureOptions.ConfigureOpenTelemetryLogging ?? (options => {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
        }));
    }

    private static void SetOpenTelemetry(IHostApplicationBuilder self, OpenTelemetryFeatureOptions featureOptions) {
        var builder = self.Services.AddOpenTelemetry();

        builder
            .WithMetrics(metrics => {
                // Insert metrics for library
                metrics.AddMeter(typeof(Nameless.AssemblyMarker).Assembly.GetName().Name!);
                metrics.AddMeter(typeof(AssemblyMarker).Assembly.GetName().Name!);

                if (featureOptions.OverwriteMeterProviderConfiguration) {
                    featureOptions.ConfigureMeterProvider?.Invoke(metrics);

                    return;
                }

                metrics.AddMeter([.. featureOptions.Meters]);
                metrics.AddHttpClientInstrumentation();
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddRuntimeInstrumentation();

                featureOptions.ConfigureMeterProvider?.Invoke(metrics);
            })
            .WithTracing(tracing => {
                // Insert tracing for library
                tracing.AddSource(typeof(Nameless.AssemblyMarker).Assembly.GetName().Name!);
                tracing.AddSource(typeof(AssemblyMarker).Assembly.GetName().Name!);

                if (featureOptions.OverwriteTracerProviderConfiguration) {
                    featureOptions.ConfigureTracerProvider?.Invoke(tracing);

                    return;
                }

                tracing.AddSource([.. featureOptions.TracingSources]);
                tracing.AddHttpClientInstrumentation(featureOptions.ConfigureHttpClientTraceInstrumentation);
                tracing.AddAspNetCoreInstrumentation(featureOptions.AspNetCoreTracerName, featureOptions.ConfigureAspNetCoreTraceInstrumentation);

                featureOptions.ConfigureTracerProvider?.Invoke(tracing);
            })
            .ConfigureResource(featureOptions.ConfigureResources);

        var exporterEndpoint = self.Configuration[WebConstants.OpenTelemetry.ExporterEndpointConfigKey];
        if (!string.IsNullOrWhiteSpace(exporterEndpoint)) {
            builder.UseOtlpExporter();
        }
    }
}
