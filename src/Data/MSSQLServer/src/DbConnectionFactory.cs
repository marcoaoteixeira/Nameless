using System.Data;
using System.Data.SqlClient;

namespace Nameless.Data.MSSQLServer {
    public sealed class DbConnectionFactory : IDbConnectionFactory {
        #region Private Read-Only Fields

        private readonly string _connectionString;

        #endregion

        #region Public Constructors

        public DbConnectionFactory (string connectionString) {
            Prevent.ParameterNull (connectionString, nameof (connectionString));

            _connectionString = connectionString;
        }

        #endregion

        #region IDbConnectionFactory Members

        public string ProviderName => "MSSQLSERVER";

        public IDbConnection Create () => new SqlConnection (_connectionString);

        #endregion
    }
}