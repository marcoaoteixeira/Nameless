namespace Nameless.Logging {

    /// <summary>
    /// Defines the logger interface.
    /// </summary>
    public interface ILogger {

        #region Methods

        /// <summary>
        /// Check if the specified log level is enabled.
        /// </summary>
        /// <param name="logLevel">Log level.</param>
        /// <returns><c>true</c> if log level is enabled, otherwise, <c>false</c>.</returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// Writes the log information.
        /// </summary>
        /// <param name="logLevel">Log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception, if any.</param>
        /// <param name="args">The arguments that will be used in the message.</param>
        void Log(LogLevel logLevel, string message, Exception? exception = default, params object[] args);

        #endregion Methods
    }
}
