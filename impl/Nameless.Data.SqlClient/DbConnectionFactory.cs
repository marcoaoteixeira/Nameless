using System.Data;
using System.Data.SqlClient;

namespace Nameless.Data.SqlClient {
    public sealed class DbConnectionFactory : IDbConnectionFactory {
        #region Private Read-Only Fields

        private readonly DatabaseSettings _settings;

        #endregion

        #region Public Constructors

        public DbConnectionFactory (DatabaseSettings settings) {
            Prevent.ParameterNull (settings, nameof (settings));

            _settings = settings;
        }

        #endregion

        #region IDbConnectionFactory Members

        public string ProviderName => "MSSQLSERVER";

        public IDbConnection Create () => new SqlConnection (_settings.ConnectionString);

        #endregion
    }
}