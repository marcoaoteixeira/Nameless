namespace Nameless.Windows;

/// <summary>
///     Constants &amp; Static Data
/// </summary>
public static class WindowsConstants {
    /// <summary>
    ///     Well-Known Document Extensions
    /// </summary>
    public static class WellKnownDocumentExtensions {
        /// <summary>
        ///     Gets an array with all well-known document extensions
        /// </summary>
        public static string[] AsArray { get; } = [Txt, Doc, Docx, Rtf, Pdf, Xps];

        /// <summary>
        ///     Plain-text document extension.
        /// </summary>
        public const string Txt = ".txt";

        /// <summary>
        ///     Word document extension.
        /// </summary>
        public const string Doc = ".doc";

        /// <summary>
        ///     (New) Word document extension.
        /// </summary>
        public const string Docx = ".docx";

        /// <summary>
        ///     Rich text format document extension.
        /// </summary>
        public const string Rtf = ".rtf";

        /// <summary>
        ///     PDF document extension.
        /// </summary>
        public const string Pdf = ".pdf";

        /// <summary>
        ///     XPS document extension.
        /// </summary>
        public const string Xps = ".xps";
    }
}
