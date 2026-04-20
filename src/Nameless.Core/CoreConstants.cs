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
        /// <summary>
        ///     Backward slash
        /// </summary>
        public const string BackwardSlash = "\\";

        /// <summary>
        ///     Colon
        /// </summary>
        public const string Colon = ":";

        /// <summary>
        ///     Comma
        /// </summary>
        public const string Comma = ",";

        /// <summary>
        ///     Dash
        /// </summary>
        public const string Dash = "-";

        /// <summary>
        ///     Dot
        /// </summary>
        public const string Dot = ".";

        /// <summary>
        ///     Forward slash
        /// </summary>
        public const string ForwardSlash = "/";

        /// <summary>
        ///     Pipe
        /// </summary>
        public const string Pipe = "|";

        /// <summary>
        ///     Semicolon
        /// </summary>
        public const string Semicolon = ";";

        /// <summary>
        ///     Sharp
        /// </summary>
        public const string Sharp = "#";

        /// <summary>
        ///     Space
        /// </summary>
        public const string Space = " ";

        /// <summary>
        ///     Underscore
        /// </summary>
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
