using System.Data;
using Microsoft.Data.SqlClient;

namespace Nameless.Data.SQLServer {
    public sealed class DbConnectionFactory : IDbConnectionFactory {
        #region Private Read-Only Fields

        private readonly SQLServerOptions _options;

        #endregion

        #region Public Constructors

        public DbConnectionFactory()
            : this(SQLServerOptions.Default) { }

        public DbConnectionFactory(SQLServerOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region IDbConnectionManager Members

        public string ProviderName => "Microsoft SQL Server";

        public IDbConnection CreateDbConnection() {
            var connectionString = _options.GetConnectionString();
            var result = new SqlConnection(connectionString);

            return result;
        }

        #endregion
    }
}
