namespace Nameless.Logging.Microsoft {
    public sealed class LoggerAdapter : IMSLogger {
        #region Private Read-Only Fields

        private readonly ILogger _logger;
        private readonly IMSExternalScopeProvider _externalScopeProvider;

        #endregion

        #region Public Constructors

        public LoggerAdapter(ILogger logger, IMSExternalScopeProvider externalScopeProvider) {
            _logger = Prevent.Against.Null(logger, nameof(logger));
            _externalScopeProvider = Prevent.Against.Null(externalScopeProvider, nameof(externalScopeProvider));
        }

        #endregion

        #region MS_ILogger Members

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _externalScopeProvider.Push(state);

        public bool IsEnabled(MSLogLevel logLevel) => _logger.IsEnabled(LevelTranslator.Translate(logLevel));

        public void Log<TState>(MSLogLevel logLevel, MSEventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => _logger.Log(LevelTranslator.Translate(logLevel), formatter(state, exception), exception);

        #endregion
    }
}
