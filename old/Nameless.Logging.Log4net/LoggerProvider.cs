using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Nameless.Logging.Log4net.Scope;

namespace Nameless.Logging.Log4net {

    /// <summary>
    /// log4net implementation of <see cref="Microsoft.Extensions.Logging.ILoggerProvider"/>
    /// </summary>
    public sealed class LoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider {
        #region Private Read-Only Static Fields

        private readonly static ConcurrentDictionary<string, Logger> Cache = new ConcurrentDictionary<string, Logger> ();

        #endregion

        #region Private Read-Only Fields

        private readonly ScopeFactory _scopeFactory;
        private readonly LoggingSettings _settings;

        #endregion

        #region Private Fields

        private log4net.Repository.ILoggerRepository _repository;
        private bool _disposed;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="LoggerProvider"/>
        /// </summary>
        /// <param name="scopeFactory">The scope factory, if any.</param>
        /// <param name="settings">The logger settings, if any.</param>
        public LoggerProvider (ScopeFactory scopeFactory = null, LoggingSettings settings = null) {
            _scopeFactory = scopeFactory ?? new ScopeFactory (new ScopeRegistry ());
            _settings = settings ?? new LoggingSettings ();

            Initialize ();
        }

        #endregion

        #region Destructor

        ~LoggerProvider () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static FileInfo GetConfigurationFilePath (string configurationFileName) {
            return !Path.IsPathRooted (configurationFileName) ?
                new FileInfo (Path.Combine (typeof (LoggerProvider).GetTypeInfo ().Assembly.GetDirectoryPath (), configurationFileName)) :
                new FileInfo (configurationFileName);
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            // Create logger repository
            var repositoryType = typeof (log4net.Repository.Hierarchy.Hierarchy);
            if (!string.IsNullOrWhiteSpace (_settings.RepositoryName)) {
                try {
                    _repository = log4net.LogManager.CreateRepository (_settings.RepositoryName, repositoryType);
                } catch (log4net.Core.LogException) {
                    _repository = null;
                }
                if (_repository == null) {
                    _repository = log4net.LogManager.CreateRepository (Assembly.GetExecutingAssembly (), repositoryType);
                }
            }

            // Configure logger
            var configurationFileName = !string.IsNullOrWhiteSpace (_settings.ConfigurationFileName) ?
                _settings.ConfigurationFileName :
                LoggingSettings.DEFAULT_CONFIGURATION_FILE_NAME;
            var configurationFilePath = GetConfigurationFilePath (configurationFileName);
            if (_settings.Watch) {
                log4net.Config.XmlConfigurator.ConfigureAndWatch (_repository, configurationFilePath);
            } else {
                log4net.Config.XmlConfigurator.Configure (_repository, configurationFilePath);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (_repository != null) {
                    _repository.Shutdown ();
                    Cache.Clear ();
                }
            }

            _repository = null;
            _disposed = true;
        }

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (nameof (LoggerProvider));
            }
        }

        private Logger CreateLoggerImpl (string categoryName) {
            
            throw new NotFiniteNumberException();
        }

        #endregion

        #region ILoggerProvider Members

        public Microsoft.Extensions.Logging.ILogger CreateLogger (string categoryName) {
            BlockAccessAfterDispose ();

            return Cache.GetOrAdd (categoryName, CreateLoggerImpl);
        }

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}