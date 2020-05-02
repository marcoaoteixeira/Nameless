using System;
using System.Globalization;
using Nameless.Logging.Log4net.Scope;
using log4net;

namespace Nameless.Logging.Log4net {

    /// <summary>
    /// log4net implementation of <see cref="Microsoft.Extensions.Logging.ILogger"/>
    /// </summary>
    public sealed class Logger : Microsoft.Extensions.Logging.ILogger {

        #region Private Static Read-Only Fields

        private static readonly log4net.Core.Level AuditLevel = new log4net.Core.Level (2000000000, "AUDIT");

        #endregion

        #region Private Read-Only Fields

        private readonly string _categoryName;
        private readonly log4net.Repository.ILoggerRepository _repository;
        private readonly ScopeFactory _scopeFactory;

        #endregion

        #region Private Fields

        private log4net.ILog _log;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initalizes a new instance of <see cref="Logger"/>
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="log4net.Repository.ILoggerRepository">The logger repository.</param>
        /// <param name="scopeFactory">The scope factory.</param>
        public Logger (string categoryName, log4net.Repository.ILoggerRepository repository, ScopeFactory scopeFactory) {
            Prevent.ParameterNullOrWhiteSpace (categoryName, nameof (categoryName));
            Prevent.ParameterNull (repository, nameof (repository));
            Prevent.ParameterNull (scopeFactory, nameof (scopeFactory));

            _categoryName = categoryName;
            _repository = repository;
            _scopeFactory = scopeFactory;

            Initialize ();
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            _log = log4net.LogManager.GetLogger (_repository.Name, _categoryName);
        }

        private void Log (LogLevel level, Exception exception, string format) {
            switch (level) {
                case LogLevel.Debug:
                    _log.Debug (format, exception);
                    break;

                case LogLevel.Information:
                    _log.Info (format, exception);
                    break;

                case LogLevel.Warning:
                    _log.Warn (format, exception);
                    break;

                case LogLevel.Error:
                    _log.Error (format, exception);
                    break;

                case LogLevel.Fatal:
                    _log.Fatal (format, exception);
                    break;

                case LogLevel.Audit:
                    _log.Logger.Log (Type.GetType (_log.Logger.Name), AuditLevel, format, exception);
                    break;
            }
        }

        private void LogFormat (LogLevel level, string format, params object[] args) {
            switch (level) {
                case LogLevel.Debug:
                    _log.DebugFormat (CultureInfo.CurrentCulture, format, args);
                    break;

                case LogLevel.Information:
                    _log.InfoFormat (CultureInfo.CurrentCulture, format, args);
                    break;

                case LogLevel.Warning:
                    _log.WarnFormat (CultureInfo.CurrentCulture, format, args);
                    break;

                case LogLevel.Error:
                    _log.ErrorFormat (CultureInfo.CurrentCulture, format, args);
                    break;

                case LogLevel.Fatal:
                    _log.FatalFormat (CultureInfo.CurrentCulture, format, args);
                    break;

                case LogLevel.Audit:
                    _log.Logger.Log (Type.GetType (_log.Logger.Name), AuditLevel, string.Format (format, args), null);
                    break;
            }
        }

        #endregion

        #region ILogger Members

        public IDisposable BeginScope<TState> (TState state) => _scopeFactory.BeginScope (state);

        public bool IsEnabled (Microsoft.Extensions.Logging.LogLevel logLevel) {
            switch (logLevel) {
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    return _log.IsDebugEnabled;

                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return _log.IsInfoEnabled;

                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return _log.IsWarnEnabled;

                case Microsoft.Extensions.Logging.LogLevel.Error:
                    return _log.IsErrorEnabled;

                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return _log.IsFatalEnabled;

                case Microsoft.Extensions.Logging.LogLevel.None:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException (nameof (logLevel));
            }
        }

        public void Log<TState> (Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            Prevent.ParameterNull (formatter, nameof (formatter));

            if (!IsEnabled (logLevel)) { return; }

            var message = formatter (state, exception);
            if (string.IsNullOrEmpty (message) && exception == null) { return; } // Nothing to log

            switch (logLevel) {
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                    
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    break;
                case Microsoft.Extensions.Logging.LogLevel.None:
                    break;
            }

            throw new NotImplementedException ();
        }

        #endregion
    }
}