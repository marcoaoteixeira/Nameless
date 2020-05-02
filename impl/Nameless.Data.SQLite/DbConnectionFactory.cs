using System.Data;
using System.Data.SQLite;

namespace Nameless.Data.SQLite {
    public class DbConnectionFactory : IDbConnectionFactory {
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

        public string ProviderName => "SQLITE";

        public IDbConnection Create () => new SQLiteConnection (_settings.ConnectionString);

        #endregion
    }
}