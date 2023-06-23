using System.Data;
using MySql.Data.MySqlClient;
using Nameless.Logging;

namespace Nameless.Data.MySQL {

    public sealed class DbConnectionProvider : IDbConnectionFactory {

        #region Private Read-Only Fields

        private readonly DatabaseOptions _options;

        #endregion

        #region Private Fields

        private ILogger _logger = null!;

        #endregion

        #region Public Properties

        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Public Constructors

        public DbConnectionProvider(DatabaseOptions options) {
            _options = options ?? DatabaseOptions.Default;
        }

        #endregion

        #region IDbConnectionProvider Members

        public string ProviderName => "MySQL";

        public IDbConnection Create() {
            var connection = new MySqlConnection(_options.ConnectionString);

            if (connection.State != ConnectionState.Open) {
                try { connection.Open(); }
                catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }
            }

            return connection;
        }

        #endregion
    }
}