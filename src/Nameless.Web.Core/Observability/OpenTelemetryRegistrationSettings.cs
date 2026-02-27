using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace Nameless.Web.Observability;

/// <summary>
///     OpenTelemetry options for configuring logging and tracing in the application.
/// </summary>
public class OpenTelemetryRegistrationSettings {
    /// <summary>
    ///     Gets or sets a list of registered metric meters.
    /// </summary>
    public string[] MetricMeters { get; set; } = [];

    /// <summary>
    ///     Gets or sets the activity sources.
    /// </summary>
    public string[] ActivitySources { get; set; } = [];

    /// <summary>
    ///     Gets or sets the action to configure OpenTelemetry logger options.
    /// </summary>
    public Action<OpenTelemetryLoggerOptions> ConfigureLogger { get; set; } = logging => {
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
    };

    /// <summary>
    ///     Gets or sets the action to configure ASP.NET Core trace instrumentation options.
    /// </summary>
    public Action<AspNetCoreTraceInstrumentationOptions> ConfigureAspNetCoreTraceInstrumentation { get; set; } = _ => { };

    /// <summary>
    ///     Gets or sets the action to configure HTTP client trace instrumentation options.
    /// </summary>
    public Action<HttpClientTraceInstrumentationOptions> ConfigureHttpClientTraceInstrumentation { get; set; } = _ => { };

    /// <summary>
    ///     Gets or sets the action to configure OpenTelemetry resource
    ///     builder.
    /// </summary>
    public Action<ResourceBuilder> ConfigureResources { get; set; } = _ => { };
}