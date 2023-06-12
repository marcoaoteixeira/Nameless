namespace Nameless.Logging {

    /// <summary>
    /// Extension methods for <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtension {

        #region Public Static Methods

        #region Debug Log Methods

        /// <summary>
        /// Writes a debug log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Debug(this ILogger source, string message) {
            FilteredLog(source, LogLevel.Debug, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a debug log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Debug(this ILogger source, Exception exception, string message) {
            FilteredLog(source, LogLevel.Debug, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a debug log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Debug(this ILogger source, string format, params object[] args) {
            FilteredLog(source, LogLevel.Debug, null, format, args);
        }

        /// <summary>
        /// Writes a debug log line.
        /// </summary>
        /// <param name="self">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Debug(this ILogger self, Exception exception, string format, params object[] args) {
            FilteredLog(self, LogLevel.Debug, exception, format, args);
        }

        #endregion

        #region Information Log Methods

        /// <summary>
        /// Writes an information log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Information(this ILogger source, string message) {
            FilteredLog(source, LogLevel.Information, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an information log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Information(this ILogger source, Exception exception, string message) {
            FilteredLog(source, LogLevel.Information, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an information log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Information(this ILogger source, string format, params object[] args) {
            FilteredLog(source, LogLevel.Information, null, format, args);
        }

        /// <summary>
        /// Writes an information log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Information(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, LogLevel.Information, exception, format, args);
        }

        #endregion

        #region Warning Log Methods

        /// <summary>
        /// Writes a warning log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Warning(this ILogger source, string message) {
            FilteredLog(source, LogLevel.Warning, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a warning log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Warning(this ILogger source, Exception exception, string message) {
            FilteredLog(source, LogLevel.Warning, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a warning log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Warning(this ILogger source, string format, params object[] args) {
            FilteredLog(source, LogLevel.Warning, null, format, args);
        }

        /// <summary>
        /// Writes a warning log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Warning(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, LogLevel.Warning, exception, format, args);
        }

        #endregion

        #region Error Log Methods

        /// <summary>
        /// Writes an error log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Error(this ILogger source, string message) {
            FilteredLog(source, LogLevel.Error, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an error log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Error(this ILogger source, Exception exception, string message) {
            FilteredLog(source, LogLevel.Error, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an error log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Error(this ILogger source, string format, params object[] args) {
            FilteredLog(source, LogLevel.Error, null, format, args);
        }

        /// <summary>
        /// Writes an error log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Error(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, LogLevel.Error, exception, format, args);
        }

        #endregion

        #region Fatal Log Methods

        /// <summary>
        /// Writes a fatal log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Fatal(this ILogger source, string message) {
            FilteredLog(source, LogLevel.Fatal, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a fatal log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Fatal(this ILogger source, Exception exception, string message) {
            FilteredLog(source, LogLevel.Fatal, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a fatal log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Fatal(this ILogger source, string format, params object[] args) {
            FilteredLog(source, LogLevel.Fatal, null, format, args);
        }

        /// <summary>
        /// Writes a fatal log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Fatal(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, LogLevel.Fatal, exception, format, args);
        }

        #endregion

        #region Critical Log Methods

        /// <summary>
        /// Writes a critical log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Critical(this ILogger source, string message) {
            FilteredLog(source, LogLevel.Critical, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a critical log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Critical(this ILogger source, Exception exception, string message) {
            FilteredLog(source, LogLevel.Critical, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a critical log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Critical(this ILogger source, string format, params object[] args) {
            FilteredLog(source, LogLevel.Critical, null, format, args);
        }

        /// <summary>
        /// Writes a critical log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Critical(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, LogLevel.Critical, exception, format, args);
        }

        #endregion

        #region Audit Log Methods

        /// <summary>
        /// Writes an audit log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Audit(this ILogger source, string message) {
            FilteredLog(source, LogLevel.Audit, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an audit log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Audit(this ILogger source, Exception exception, string message) {
            FilteredLog(source, LogLevel.Audit, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an audit log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Audit(this ILogger source, string format, params object[] args) {
            FilteredLog(source, LogLevel.Audit, null, format, args);
        }

        /// <summary>
        /// Writes an audit log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Audit(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, LogLevel.Audit, exception, format, args);
        }

        #endregion

        #endregion

        #region Private Static Methods

        private static void FilteredLog(ILogger logger, LogLevel logLevel, Exception? exception, string message, params object[] args) {
            if (logger.IsEnabled(logLevel)) {
                logger.Log(logLevel, message, exception, args);
            }
        }

        #endregion
    }
}
