namespace Nameless.Web.OpenTelemetry;

/// <summary>
///     Provides constants for OpenTelemetry configuration.
/// </summary>
public static class OpenTelemetryConstants {
    /// <summary>
    ///     Gets the name of the environment variable that specifies the
    ///     OpenTelemetry exporter endpoint.
    /// </summary>
    public const string OPEN_TELEMETRY_EXPORTER_ENDPOINT = "OTEL_EXPORTER_OTLP_ENDPOINT";
}
