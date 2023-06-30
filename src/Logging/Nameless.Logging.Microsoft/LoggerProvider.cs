using MS_IExternalScopeProvider = Microsoft.Extensions.Logging.IExternalScopeProvider;
using MS_ILogger = Microsoft.Extensions.Logging.ILogger;
using MS_ILoggerProvider = Microsoft.Extensions.Logging.ILoggerProvider;

namespace Nameless.Logging.Microsoft {

    public sealed class LoggerProvider : MS_ILoggerProvider {

        #region Private Read-Only Fields

        private readonly ILoggerFactory _loggerFactory;
        private readonly MS_IExternalScopeProvider _externalScopeProvider;

        #endregion

        #region Public Constructors

        public LoggerProvider(ILoggerFactory loggerFactory, MS_IExternalScopeProvider externalScopeProvider) {
            Garda.Prevent.Null(loggerFactory, nameof(loggerFactory));

            _loggerFactory = loggerFactory;
            _externalScopeProvider = externalScopeProvider;
        }

        #endregion

        #region MS_ILoggerProvider Members

        public MS_ILogger CreateLogger(string categoryName)
            => new LoggerAdapter(_loggerFactory.CreateLogger(categoryName), _externalScopeProvider);

        public void Dispose() { }

        #endregion
    }
}
