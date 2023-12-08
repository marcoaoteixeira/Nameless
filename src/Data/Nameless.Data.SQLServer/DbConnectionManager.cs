using System.Data;
using Microsoft.Data.SqlClient;

namespace Nameless.Data.SQLServer {
    public sealed class DbConnectionManager : IDbConnectionManager, IDisposable {
        #region Private Read-Only Fields

        private readonly SQLServerOptions _options;

        #endregion

        #region Private Fields

        private SqlConnection? _connection;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public DbConnectionManager(SQLServerOptions? options = null) {
            _options = options ?? SQLServerOptions.Default;
        }

        #endregion

        #region Destructor

        ~DbConnectionManager() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose()
            => ObjectDisposedException.ThrowIf(_disposed, typeof(DbConnectionManager));

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _connection?.Dispose();
            }

            _connection = null;
            _disposed = true;
        }

        private SqlConnection CreateDbConnection() {
            var connectionString = _options.GetConnectionString();
            var connection = new SqlConnection(connectionString);

            connection.Open();

            return connection;
        }

        #endregion

        #region IDbConnectionManager Members

        public string ProviderName => "Microsoft SQL Server";

        public IDbConnection GetDbConnection() {
            BlockAccessAfterDispose();

            return _connection ??= CreateDbConnection();
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
