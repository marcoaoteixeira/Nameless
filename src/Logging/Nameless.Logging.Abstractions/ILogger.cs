namespace Nameless.Logging {
    /// <summary>
    /// Interface to implement a logger.
    /// </summary>
    public interface ILogger {
        #region Methods

        /// <summary>
        /// Check if the specified log level is enabled.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <returns><c>true</c> if log level is enabled, otherwise, <c>false</c>.</returns>
        bool IsEnabled(Level level);

        /// <summary>
        /// Writes the log information.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception, if any.</param>
        /// <param name="args">The arguments that will be used in the message.</param>
        void Log(Level level, string message, Exception? exception = null, params object[] args);

        #endregion
    }
}
