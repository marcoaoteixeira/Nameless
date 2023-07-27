using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Data.MSSQLServer {
    public sealed class DbConnectionProvider : IDbConnectionFactory {
        #region Private Read-Only Fields

        private readonly DatabaseOptions _options;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Public Constructors

        public DbConnectionProvider(DatabaseOptions? options = null) {
            _options = options ?? DatabaseOptions.Default;
        }

        #endregion

        #region IDbConnectionProvider Members

        public string ProviderName => "Microsoft SQL Server";

        public IDbConnection Create() {
            var connection = new SqlConnection(_options.ConnectionString);

            if (connection.State != ConnectionState.Open) {
                try { connection.Open(); }
                catch (Exception ex) { Logger.LogError(ex, ex.Message); throw; }
            }

            return connection;
        }

        #endregion
    }
}