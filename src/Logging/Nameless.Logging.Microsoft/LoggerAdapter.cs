using Nameless;
using MS_EventId = Microsoft.Extensions.Logging.EventId;
using MS_IExternalScopeProvider = Microsoft.Extensions.Logging.IExternalScopeProvider;
using MS_ILogger = Microsoft.Extensions.Logging.ILogger;
using MS_LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Nameless.Logging.Microsoft {

    public sealed class LoggerAdapter : MS_ILogger {

        #region Private Read-Only Fields

        private readonly ILogger _logger;
        private readonly MS_IExternalScopeProvider _externalScopeProvider;

        #endregion

        #region Public Constructors

        public LoggerAdapter(ILogger logger, MS_IExternalScopeProvider externalScopeProvider) {
            Prevent.Null(logger, nameof(logger));
            Prevent.Null(externalScopeProvider, nameof(externalScopeProvider));

            _logger = logger;
            _externalScopeProvider = externalScopeProvider;
        }

        #endregion

        #region MS_ILogger Members

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _externalScopeProvider.Push(state);

        public bool IsEnabled(MS_LogLevel logLevel) => _logger.IsEnabled(LogLevelTranslator.Translate(logLevel));

        public void Log<TState>(MS_LogLevel logLevel, MS_EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => _logger.Log(LogLevelTranslator.Translate(logLevel), formatter(state, exception), exception);

        #endregion
    }
}
