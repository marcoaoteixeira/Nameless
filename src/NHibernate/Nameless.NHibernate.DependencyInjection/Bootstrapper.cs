using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Infrastructure;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate.DependencyInjection {
    public sealed class Bootstrapper : IStartable {
        #region Private Read-Only Fields

        private readonly IApplicationContext _appContext;
        private readonly ISession _session;
        private readonly Configuration _configuration;
        private NHibernateSchemaExportOptions _schemaExportOptions;

        #endregion

        #region Private Properties

        private string OutputFilePath
            => Path.Combine(_appContext.BasePath, _schemaExportOptions.OutputFolder, $"{DateTime.Now:yyyyMMdd_hhmmss_fff}_db_schema.txt");

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get => _logger ??= NullLogger.Instance;
            set => _logger = value;
        }

        #endregion

        #region Public Constructors

        public Bootstrapper(IApplicationContext appContext, ISession session, Configuration configuration, NHibernateSchemaExportOptions? schemaExportOptions = null) {
            _appContext = Guard.Against.Null(appContext, nameof(appContext));
            _session = Guard.Against.Null(session, nameof(session));
            _configuration = Guard.Against.Null(configuration, nameof(configuration));
            _schemaExportOptions = schemaExportOptions ?? NHibernateSchemaExportOptions.Default;
        }

        #endregion

        #region IStartable Members

        public void Start() {
            if (!_schemaExportOptions.ExecuteSchemaExport) {
                return;
            }

            using var writer = _schemaExportOptions.FileOutput ? File.CreateText(OutputFilePath) : TextWriter.Null;
            new SchemaExport(_configuration).Execute(
                useStdOut: _schemaExportOptions.ConsoleOutput,
                execute: true,
                justDrop: false,
                connection: _session.Connection,
                exportOutput: writer
            );
        }

        #endregion
    }
}
