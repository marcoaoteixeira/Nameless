namespace Nameless.Logging.Microsoft {
    public sealed class LoggerProvider : IMSLoggerProvider {
        #region Private Read-Only Fields

        private readonly ILoggerFactory _loggerFactory;
        private readonly IMSExternalScopeProvider _externalScopeProvider;

        #endregion

        #region Public Constructors

        public LoggerProvider(ILoggerFactory loggerFactory, IMSExternalScopeProvider externalScopeProvider) {
            _loggerFactory = Prevent.Against.Null(loggerFactory, nameof(loggerFactory));
            _externalScopeProvider = Prevent.Against.Null(externalScopeProvider, nameof(externalScopeProvider));
        }

        #endregion

        #region MS_ILoggerProvider Members

        public IMSLogger CreateLogger(string categoryName)
            => new LoggerAdapter(_loggerFactory.CreateLogger(categoryName), _externalScopeProvider);

        public void Dispose() { }

        #endregion
    }
}
