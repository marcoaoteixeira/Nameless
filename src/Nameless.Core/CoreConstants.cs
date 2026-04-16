namespace Nameless;

/// <summary>
///     Core library constant values.
/// </summary>
public static class CoreConstants {
    /// <summary>
    ///     Static class that holds parameters' names for environment variables.
    /// </summary>
    public static class EnvironmentTokens {
        /// <summary>
        ///     EnvironmentName parameter: DOTNET_RUNNING_IN_CONTAINER
        /// </summary>
        /// <remarks>
        ///     Commonly used in web applications that runs in containers.
        /// </remarks>
        public const string DotnetRunningInContainer = "DOTNET_RUNNING_IN_CONTAINER";
    }

    /// <summary>
    ///     Static class containing a list of strings with common separator values.
    /// </summary>
    public static class Separators {
        public const string BackwardSlash = "\\";
        public const string Colon = ":";
        public const string Comma = ",";
        public const string Dash = "-";
        public const string Dot = ".";
        public const string ForwardSlash = "/";
        public const string Pipe = "|";
        public const string Semicolon = ";";
        public const string Sharp = "#";
        public const string Space = " ";
        public const string Underscore = "_";
    }

    /// <summary>
    ///     Provides constants for OpenTelemetry configuration.
    /// </summary>
    public static class OpenTelemetry {
        /// <summary>
        ///     Gets the name of the environment variable that specifies the
        ///     OpenTelemetry exporter endpoint.
        /// </summary>
        public const string ExporterEndpointConfigName = "OTEL_EXPORTER_OTLP_ENDPOINT";
    }
}
