using System.Data;
using System.Data.SQLite;

namespace Nameless.Data.SQLite {
    public class DbConnectionFactory : IDbConnectionFactory {
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

        public string ProviderName => "SQLITE";

        public IDbConnection Create () => new SQLiteConnection (_connectionString);

        #endregion
    }
}