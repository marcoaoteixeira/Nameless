namespace Nameless.Web;

/// <summary>
///     Constants for easy use.
/// </summary>
public static class WebConstants {
    /// <summary>
    ///     A collection of content types.
    /// </summary>
    public static class ContentTypes {
        /// <summary>
        ///     Default content type text plain.
        /// </summary>
        public const string Text = "text/plain; charset=utf-8";

        /// <summary>
        ///     JSON content type.
        /// </summary>
        public const string Json = "application/json";

        /// <summary>
        ///     JSON content type with charset UTF-8.
        /// </summary>
        public const string JsonWithCharset = "application/json; charset=utf-8";

        /// <summary>
        ///     Problem content type.
        /// </summary>
        public const string ProblemJson = "application/problem+json";

        /// <summary>
        ///     Binary content type.
        /// </summary>
        public const string Binary = "application/octet-stream";
    }

    /// <summary>
    ///     A collection of constants to use as SyntaxAttribute syntax.
    /// </summary>
    public static class Syntaxes {
        /// <summary>
        ///     Route syntax.
        /// </summary>
        public const string Route = "Route";
    }
}