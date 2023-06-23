using L4N_ILogger = log4net.Core.ILogger;
using L4N_Level = log4net.Core.Level;

namespace Nameless.Logging.log4net {

    public sealed class Logger : ILogger {

        #region Private Read-Only Fields

        private readonly L4N_ILogger _logger;
        private readonly ILoggerEventFactory _loggerEventFactory;
        private readonly Log4netOptions _options;

        #endregion

        #region Public Constructors

        public Logger(L4N_ILogger logger, ILoggerEventFactory loggerEventFactory, Log4netOptions options) {
            Prevent.Null(logger, nameof(logger));
            Prevent.Null(loggerEventFactory, nameof(loggerEventFactory));
            Prevent.Null(options, nameof(options));

            _logger = logger;
            _loggerEventFactory = loggerEventFactory;
            _options = options;
        }

        #endregion

        #region ILogger Members

        public bool IsEnabled(LogLevel logLevel) {
            var level = LogLevelTranslator.Translate(logLevel, _options.OverrideCriticalLevel);

            return level != L4N_Level.Off && _logger.IsEnabledFor(level);
        }

        public void Log(LogLevel logLevel, string message, Exception? exception = default, params object[] args) {
            Prevent.NullOrWhiteSpaces(message, nameof(message));

            if (!IsEnabled(logLevel)) { return; }

            var logMessage = new LogMessage(logLevel, message, exception, args);
            var loggingEvent = _loggerEventFactory.CreateLoggingEvent(
                message: in logMessage,
                logger: _logger,
                options: _options
            );

            if (loggingEvent is null) { return; }

            _logger.Log(loggingEvent);
        }

        #endregion
    }
}
