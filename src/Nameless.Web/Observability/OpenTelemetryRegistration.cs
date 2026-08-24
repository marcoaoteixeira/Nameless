using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Nameless.Web.Observability;

/// <summary>
///     OpenTelemetry options for configuring logging and tracing in the application.
/// </summary>
public class OpenTelemetryRegistration {
    public bool OverrideOpenTelemetryLoggerConfiguration { get; set; }
    
    public Action<OpenTelemetryLoggerOptions>? ConfigureOpenTelemetryLogger { get; set; }

    /// <summary>
    ///     Gets or sets a list of registered metric meters.
    /// </summary>
    public string[] MetricMeters { get; set; } = [];
    
    public bool OverrideMeterProviderConfiguration { get; set; }
    
    public Action<MeterProviderBuilder>? ConfigureMeterProvider { get; set; }

    /// <summary>
    ///     Gets or sets the activity sources.
    /// </summary>
    public string[] ActivitySources { get; set; } = [];
    
    public bool OverrideTracerProviderConfiguration { get; set; }
    
    public Action<TracerProviderBuilder>? ConfigureTracerProvider { get; set; }
    
    /// <summary>
    ///     Gets or sets the action to configure ASP.NET Core trace instrumentation options.
    /// </summary>
    public Action<AspNetCoreTraceInstrumentationOptions> ConfigureAspNetCoreTraceInstrumentation { get; set; } = _ => { };

    /// <summary>
    ///     Gets or sets the action to configure HTTP client trace instrumentation options.
    /// </summary>
    public Action<HttpClientTraceInstrumentationOptions> ConfigureHttpClientTraceInstrumentation { get; set; } = _ => { };

    public bool OverrideResources { get; set; }

    public Action<ResourceBuilder>? ConfigureResources { get; set; }
}