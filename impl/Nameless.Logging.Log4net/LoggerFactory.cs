using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace Nameless.Logging.Log4net {

    /// <summary>
    /// log4net implementation of <see cref="ILoggerFactory"/>
    /// </summary>
    public sealed class LoggerFactory : ILoggerFactory, IDisposable {
        #region Private Read-Only Static Fields

        private readonly static ConcurrentDictionary<string, ILogger> Cache = new ConcurrentDictionary<string, ILogger> ();

        #endregion

        #region Private Read-Only Fields

        private readonly LoggingSettings _settings;

        #endregion

        #region Private Fields

        private ILoggerRepository _repository;
        private bool _disposed;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="LoggerFactory"/>
        /// </summary>
        /// <param name="settings">The logger settings, if any.</param>
        public LoggerFactory (LoggingSettings settings = null) {
            _settings = settings ?? new LoggingSettings ();

            Initialize ();
        }

        #endregion

        #region Destructor

        ~LoggerFactory () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static FileInfo GetConfigurationFilePath (string configurationFileName) {
            return !Path.IsPathRooted (configurationFileName) ?
                new FileInfo (Path.Combine (typeof (LoggerFactory).GetTypeInfo ().Assembly.GetDirectoryPath (), configurationFileName)) :
                new FileInfo (configurationFileName);
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            // Create logger repository
            var repositoryType = typeof (Hierarchy);
            if (!string.IsNullOrWhiteSpace (_settings.RepositoryName)) {
                try {
                    _repository = LogManager.CreateRepository (_settings.RepositoryName, repositoryType);
                } catch (LogException) {
                    _repository = null;
                }
            }
            if (_repository == null) {
                _repository = LogManager.CreateRepository (Assembly.GetExecutingAssembly (), repositoryType);
            }

            // Configure logger
            var configurationFileName = !string.IsNullOrWhiteSpace (_settings.ConfigurationFileName) ?
                _settings.ConfigurationFileName :
                LoggingSettings.DEFAULT_CONFIGURATION_FILE_NAME;
            var configurationFilePath = GetConfigurationFilePath (configurationFileName);
            if (_settings.Watch) {
                XmlConfigurator.ConfigureAndWatch (_repository, configurationFilePath);
            } else {
                XmlConfigurator.Configure (_repository, configurationFilePath);
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
                throw new ObjectDisposedException (nameof (LoggerFactory));
            }
        }

        private ILogger CreateLoggerImpl (string categoryName) {
            return new Logger (LogManager.GetLogger (_repository.Name, categoryName));
        }

        #endregion

        #region ILoggerFactory Members

        public ILogger CreateLogger (Type type) {
            Prevent.ParameterNull (type, nameof (type));

            BlockAccessAfterDispose ();

            return Cache.GetOrAdd (type.FullName, CreateLoggerImpl);
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}