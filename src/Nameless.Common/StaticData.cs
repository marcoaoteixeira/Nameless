using System.Text;

namespace Nameless;

/// <summary>
///     Core library constant values.
/// </summary>
public static class StaticData {
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
        public const char BackwardSlash = '\\';

        /// <summary>
        ///     Colon
        /// </summary>
        public const char Colon = ':';

        /// <summary>
        ///     Comma
        /// </summary>
        public const char Comma = ',';

        /// <summary>
        ///     Dash
        /// </summary>
        public const char Dash = '-';

        /// <summary>
        ///     Dot
        /// </summary>
        public const char Dot = '.';

        /// <summary>
        ///     Forward slash
        /// </summary>
        public const char ForwardSlash = '/';

        /// <summary>
        ///     Pipe
        /// </summary>
        public const char Pipe = '|';

        /// <summary>
        ///     Semicolon
        /// </summary>
        public const char Semicolon = ';';

        /// <summary>
        ///     Sharp
        /// </summary>
        public const char Sharp = '#';

        /// <summary>
        ///     Space
        /// </summary>
        public const char Space = ' ';

        /// <summary>
        ///     Underscore
        /// </summary>
        public const char Underscore = '_';
    }

    /// <summary>
    ///     Provides constants for OpenTelemetry configuration.
    /// </summary>
    public static class OpenTelemetry {
        /// <summary>
        ///     Gets the name of the environment variable that specifies the
        ///     OpenTelemetry exporter endpoint.
        /// </summary>
        public const string ExporterEndpointConfigKey = "OTEL_EXPORTER_OTLP_ENDPOINT";
    }

    /// <summary>
    ///     Core library default values.
    /// </summary>
    public static class Defaults {
        /// <summary>
        ///     Gets the default encoding (UTF-8 without BOM)
        /// </summary>
        public static Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}
