namespace Nameless.Web;

/// <summary>
///     Constants for easy use.
/// </summary>
internal static class WebConstants {
    /// <summary>
    ///     Provides constants for OpenTelemetry configuration.
    /// </summary>
    internal static class OpenTelemetry {
        /// <summary>
        ///     Gets the name of the environment variable that specifies the
        ///     OpenTelemetry exporter endpoint.
        /// </summary>
        internal const string ExporterEndpointConfigKey = "OTEL_EXPORTER_OTLP_ENDPOINT";
    }

    /// <summary>
    ///     List of policies defined as "default".
    /// </summary>
    internal static class Policies {
        internal static class Cors {
            internal const string AllowEverything = "CP: Allow_Everything";
        }

        internal static class OutputCache {
            internal const string OneSecond = "OC: 1_Seconds";
            internal const string FiveSeconds = "OC: 5_Seconds";
            internal const string FifteenSeconds = "OC: 15_Seconds";
            internal const string ThirtySeconds = "OC: 30_Seconds";
            internal const string OneMinute = "OC: 1_Minute";
        }

        internal static class RateLimiter {
            /// <summary>
            ///     A rate limiter policy that has a window of 60 seconds,
            ///     permits up to 10 requests and has 4 window segments.
            /// </summary>
            internal const string SlidingWindow = "RL: Sliding_Window_60_Seconds_10_Requests_4_Segments";
        }

        internal static class RequestTimeout {
            internal const string OneSecond = "RT: 1_Seconds";
            internal const string FiveSeconds = "RT: 5_Seconds";
            internal const string FifteenSeconds = "RT: 15_Seconds";
            internal const string ThirtySeconds = "RT: 30_Seconds";
            internal const string OneMinute = "RT: 1_Minute";
        }
    }

    /// <summary>
    ///     A collection of content types.
    /// </summary>
    internal static class ContentTypes {
        /// <summary>
        ///     Default content type text plain.
        /// </summary>
        internal const string Text = "text/plain; charset=utf-8";

        /// <summary>
        ///     JSON content type.
        /// </summary>
        internal const string Json = "application/json";

        /// <summary>
        ///     JSON content type with charset UTF-8.
        /// </summary>
        internal const string JsonWithCharset = "application/json; charset=utf-8";

        /// <summary>
        ///     Problem content type.
        /// </summary>
        internal const string ProblemJson = "application/problem+json";

        /// <summary>
        ///     Binary content type.
        /// </summary>
        internal const string Binary = "application/octet-stream";
    }

    /// <summary>
    ///     A collection of constants to use as SyntaxAttribute syntax.
    /// </summary>
    internal static class Syntaxes {
        /// <summary>
        ///     Route syntax.
        /// </summary>
        internal const string Route = "Route";
    }
}