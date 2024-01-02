using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Data.SQLServer {
    public sealed class DbConnectionManager : IDbConnectionManager, IDisposable {
        #region Private Read-Only Fields

        private readonly SQLServerOptions _options;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private SqlConnection? _connection;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public DbConnectionManager()
            : this(SQLServerOptions.Default, NullLogger.Instance) { }

        public DbConnectionManager(SQLServerOptions options)
            : this(options, NullLogger.Instance) { }

        public DbConnectionManager(SQLServerOptions options, ILogger logger) {
            _options = Guard.Against.Null(options, nameof(options));
            _logger = Guard.Against.Null(logger, nameof(logger));
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
                if (_connection is not null) {
                    _connection.StateChange -= StateChangeHandler;
                    _connection.InfoMessage -= InfoMessageHandler;

                    _connection.Dispose();
                }
            }

            _connection = null;
            _disposed = true;
        }

        private SqlConnection CreateDbConnection() {
            var connectionString = _options.GetConnectionString();
            var connection = new SqlConnection(connectionString);

            connection.StateChange += StateChangeHandler;
            connection.InfoMessage += InfoMessageHandler;

            return connection;
        }

        private void InfoMessageHandler(object sender, SqlInfoMessageEventArgs e)
            => _logger.LogDebug(
                message: "{Message}",
                args: e.Message
            );

        private void StateChangeHandler(object sender, StateChangeEventArgs e)
            => _logger.LogDebug(
                message: "SQL Server connection change: {OriginalState} => {CurrentState}",
                args: new object[] { e.OriginalState, e.CurrentState }
            );

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
