using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Options;
using L4N_Hierarchy = log4net.Repository.Hierarchy.Hierarchy;
using L4N_ILoggerRepository = log4net.Repository.ILoggerRepository;
using L4N_LogManager = log4net.LogManager;
using L4N_XmlConfigurator = log4net.Config.XmlConfigurator;

namespace Nameless.Logging.log4net {

    public sealed class LoggerFactory : ILoggerFactory, IDisposable {

        #region Private Read-Only Static Fields

        private readonly static ConcurrentDictionary<string, ILogger> _cache = new();

        #endregion

        #region Private Read-Only Fields

        private readonly ILoggerEventFactory _loggerEventFactory;
        private readonly Log4netOptions _options;

        #endregion

        #region Private Fields

        private L4N_ILoggerRepository _repository = default!;
        private bool _disposed;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="LoggerFactory"/>
        /// </summary>
        /// <param name="options">The logger settings.</param>
        public LoggerFactory(ILoggerEventFactory loggerEventFactory, IOptions<Log4netOptions> options) {
            Prevent.Null(loggerEventFactory, nameof(loggerEventFactory));

            _loggerEventFactory = loggerEventFactory;
            _options = options.Value ?? Log4netOptions.Default;

            Initialize();
        }

        #endregion

        #region Destructor

        ~LoggerFactory() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static FileInfo GetConfigurationFilePath(string configurationFileName) {
            return Path.IsPathRooted(configurationFileName)
                ? new FileInfo(configurationFileName)
                : new FileInfo(typeof(LoggerFactory).Assembly.GetDirectoryPath(configurationFileName));
        }

        #endregion

        #region Private Methods

        private void Initialize() {
            // Create logger repository
            var repositoryType = typeof(L4N_Hierarchy);
            if (!string.IsNullOrWhiteSpace(_options.RepositoryName)) {
                try { _repository = L4N_LogManager.CreateRepository(_options.RepositoryName, repositoryType); }
                catch { }
            }
            _repository ??= L4N_LogManager.CreateRepository(Assembly.GetExecutingAssembly(), repositoryType);

            // Configure logger
            var configurationFilePath = GetConfigurationFilePath(_options.ConfigurationFileName);
            if (_options.ReloadOnChange) { L4N_XmlConfigurator.ConfigureAndWatch(_repository, configurationFilePath); }
            else { L4N_XmlConfigurator.Configure(_repository, configurationFilePath); }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _repository.Shutdown();
                _cache.Clear();
            }

            _repository = default!;
            _disposed = true;
        }

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(LoggerFactory));
            }
        }

        #endregion

        #region ILoggerFactory Members

        public ILogger CreateLogger(string source) {
            Prevent.NullOrWhiteSpaces(source, nameof(source));

            BlockAccessAfterDispose();

            return _cache.GetOrAdd(
                key: source,
                valueFactory: key => new Logger(
                    logger: L4N_LogManager.GetLogger(_repository.Name, key).Logger,
                    loggerEventFactory: _loggerEventFactory,
                    options: _options
                )
            );
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
