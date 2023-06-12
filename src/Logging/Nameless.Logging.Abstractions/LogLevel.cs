namespace Nameless.Logging {

    /// <summary>
    /// Log level flags.
    /// </summary>
    [Flags]
    public enum LogLevel {

        /// <summary>
        /// Disabled
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Debug
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Information
        /// </summary>
        Information = 2,

        /// <summary>
        /// Warning
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Error
        /// </summary>
        Error = 8,

        /// <summary>
        /// Critical
        /// </summary>
        Critical = 16,

        /// <summary>
        /// Fatal
        /// </summary>
        Fatal = 32,

        /// <summary>
        /// Audit
        /// </summary>
        Audit = 65536
    }
}
