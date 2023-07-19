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
            Prevent.Against.Null(logger, nameof(logger));
            Prevent.Against.Null(loggerEventFactory, nameof(loggerEventFactory));
            Prevent.Against.Null(options, nameof(options));

            _logger = logger;
            _loggerEventFactory = loggerEventFactory;
            _options = options;
        }

        #endregion

        #region ILogger Members

        public bool IsEnabled(Level level) {
            var translation = LevelTranslator.Translate(level, _options.OverrideCriticalLevel);

            return translation != L4N_Level.Off && _logger.IsEnabledFor(translation);
        }

        public void Log(Level level, string message, Exception? exception = default, params object[] args) {
            Prevent.Against.NullOrWhiteSpace(message, nameof(message));

            if (!IsEnabled(level)) { return; }

            var logMessage = new LogMessage(level, message, exception, args);
            var loggingEvent = _loggerEventFactory.CreateLoggingEvent(
                message: in logMessage,
                logger: _logger,
                options: _options
            );

            if (loggingEvent == null) { return; }

            _logger.Log(loggingEvent);
        }

        #endregion
    }
}
