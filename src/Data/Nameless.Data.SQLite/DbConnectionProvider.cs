using System.Data;
using Microsoft.Data.Sqlite;
using Nameless.Logging;

namespace Nameless.Data.SQLite {
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

        public string ProviderName => "SQLite";

        public IDbConnection Create() {
            var connection = new SqliteConnection(_options.ConnectionString);

            if (connection.State != ConnectionState.Open) {
                try { connection.Open(); }
                catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }
            }

            return connection;
        }

        #endregion
    }
}