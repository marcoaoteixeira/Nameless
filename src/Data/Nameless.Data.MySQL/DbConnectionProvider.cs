using System.Data;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Nameless.Logging;

namespace Nameless.Data.MySQL {

    public sealed class DbConnectionProvider : IDbConnectionFactory {

        #region Private Read-Only Fields

        private readonly DatabaseOptions _options;

        #endregion

        #region Private Fields

        private ILogger _logger = default!;

        #endregion

        #region Public Properties

        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        public DbConnectionProvider(IOptions<DatabaseOptions> options) {
            _options = options.Value ?? DatabaseOptions.Default;
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