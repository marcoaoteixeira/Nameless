using System.Data;
using System.Data.SQLite;
using Nameless.Data.SQLite.Options;

namespace Nameless.Data.SQLite {
    public sealed class DbConnectionFactory : IDbConnectionFactory {
        #region Private Read-Only Fields

        private readonly SQLiteOptions _options;

        #endregion

        #region Public Constructors

        public DbConnectionFactory()
            : this(SQLiteOptions.Default) { }

        public DbConnectionFactory(SQLiteOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region IDbConnectionFactory Members

        public string ProviderName => "SQLite";

        public IDbConnection CreateDbConnection() {
            var connectionString = _options.GetConnectionString();
            var result = new SQLiteConnection(connectionString);

            return result;
        }

        #endregion
    }
}