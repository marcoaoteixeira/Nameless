namespace Nameless.Logging {
    /// <summary>
    /// <see cref="ILogger"/> extension methods.
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
            FilteredLog(source, Level.Debug, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a debug log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Debug(this ILogger source, Exception exception, string message) {
            FilteredLog(source, Level.Debug, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a debug log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Debug(this ILogger source, string format, params object[] args) {
            FilteredLog(source, Level.Debug, null, format, args);
        }

        /// <summary>
        /// Writes a debug log line.
        /// </summary>
        /// <param name="self">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Debug(this ILogger self, Exception exception, string format, params object[] args) {
            FilteredLog(self, Level.Debug, exception, format, args);
        }

        #endregion

        #region Information Log Methods

        /// <summary>
        /// Writes an information log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Information(this ILogger source, string message) {
            FilteredLog(source, Level.Information, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an information log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Information(this ILogger source, Exception exception, string message) {
            FilteredLog(source, Level.Information, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an information log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Information(this ILogger source, string format, params object[] args) {
            FilteredLog(source, Level.Information, null, format, args);
        }

        /// <summary>
        /// Writes an information log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Information(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, Level.Information, exception, format, args);
        }

        #endregion

        #region Warning Log Methods

        /// <summary>
        /// Writes a warning log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Warning(this ILogger source, string message) {
            FilteredLog(source, Level.Warning, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a warning log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Warning(this ILogger source, Exception exception, string message) {
            FilteredLog(source, Level.Warning, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a warning log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Warning(this ILogger source, string format, params object[] args) {
            FilteredLog(source, Level.Warning, null, format, args);
        }

        /// <summary>
        /// Writes a warning log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Warning(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, Level.Warning, exception, format, args);
        }

        #endregion

        #region Error Log Methods

        /// <summary>
        /// Writes an error log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Error(this ILogger source, string message) {
            FilteredLog(source, Level.Error, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an error log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Error(this ILogger source, Exception exception, string message) {
            FilteredLog(source, Level.Error, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an error log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Error(this ILogger source, string format, params object[] args) {
            FilteredLog(source, Level.Error, null, format, args);
        }

        /// <summary>
        /// Writes an error log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Error(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, Level.Error, exception, format, args);
        }

        #endregion

        #region Fatal Log Methods

        /// <summary>
        /// Writes a fatal log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Fatal(this ILogger source, string message) {
            FilteredLog(source, Level.Fatal, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a fatal log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Fatal(this ILogger source, Exception exception, string message) {
            FilteredLog(source, Level.Fatal, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a fatal log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Fatal(this ILogger source, string format, params object[] args) {
            FilteredLog(source, Level.Fatal, null, format, args);
        }

        /// <summary>
        /// Writes a fatal log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Fatal(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, Level.Fatal, exception, format, args);
        }

        #endregion

        #region Critical Log Methods

        /// <summary>
        /// Writes a critical log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Critical(this ILogger source, string message) {
            FilteredLog(source, Level.Critical, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a critical log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Critical(this ILogger source, Exception exception, string message) {
            FilteredLog(source, Level.Critical, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes a critical log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Critical(this ILogger source, string format, params object[] args) {
            FilteredLog(source, Level.Critical, null, format, args);
        }

        /// <summary>
        /// Writes a critical log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Critical(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, Level.Critical, exception, format, args);
        }

        #endregion

        #region Audit Log Methods

        /// <summary>
        /// Writes an audit log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="message">The message to log.</param>
        public static void Audit(this ILogger source, string message) {
            FilteredLog(source, Level.Audit, null, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an audit log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Audit(this ILogger source, Exception exception, string message) {
            FilteredLog(source, Level.Audit, exception, message, Array.Empty<object>());
        }

        /// <summary>
        /// Writes an audit log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Audit(this ILogger source, string format, params object[] args) {
            FilteredLog(source, Level.Audit, null, format, args);
        }

        /// <summary>
        /// Writes an audit log line.
        /// </summary>
        /// <param name="source">The source (<see cref="ILogger"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> to log.</param>
        /// <param name="format">The message format to log.</param>
        /// <param name="args">The message format arguments, if any.</param>
        public static void Audit(this ILogger source, Exception exception, string format, params object[] args) {
            FilteredLog(source, Level.Audit, exception, format, args);
        }

        #endregion

        public static ILogger On(this ILogger self, bool condition)
            => self != null && condition
                ? self
                : NullLogger.Instance;

        #endregion

        #region Private Static Methods

        private static void FilteredLog(ILogger logger, Level level, Exception? exception, string message, params object[] args) {
            if (logger.IsEnabled(level)) {
                logger.Log(level, message, exception, args);
            }
        }

        #endregion
    }
}
