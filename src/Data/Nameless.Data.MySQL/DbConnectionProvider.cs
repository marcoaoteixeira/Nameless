using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MySql.Data.MySqlClient;

namespace Nameless.Data.MySQL {
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

        public string ProviderName => "MySQL";

        public IDbConnection Create() {
            var connection = new MySqlConnection(_options.ConnectionString);

            if (connection.State != ConnectionState.Open) {
                try { connection.Open(); }
                catch (Exception ex) { Logger.LogError(ex, ex.Message); throw; }
            }

            return connection;
        }

        #endregion
    }
}