using System.Data;
using Microsoft.Data.Sqlite;
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

        #region IDbConnectionManager Members

        public string ProviderName => "SQLite";

        public IDbConnection CreateDbConnection() {
            var connectionString = _options.GetConnectionString();
            var result = new SqliteConnection(connectionString);

            result.Open();

            return result;
        }

        #endregion
    }
}