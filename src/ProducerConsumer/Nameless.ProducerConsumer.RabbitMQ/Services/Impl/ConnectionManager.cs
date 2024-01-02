using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Services.Impl {
    public sealed class ConnectionManager : IConnectionManager, IDisposable {
        #region Private Read-Only Fields

        private readonly RabbitMQOptions _options;

        #endregion

        #region Private Fields

        private ConnectionFactory? _factory;
        private IConnection? _connection;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public ConnectionManager()
            : this(RabbitMQOptions.Default) { }

        public ConnectionManager(RabbitMQOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Private Methods

        private ConnectionFactory GetConnectionFactory() {
            if (_factory is null) {
                _factory = new ConnectionFactory {
                    HostName = _options.Server.Hostname,
                    Port = _options.Server.Port,
                    UserName = _options.Server.Username,
                    Password = _options.Server.Password
                };

                if (_options.Server.UseSsl) {
                    _factory.Ssl = new(
                        serverName: _options.Server.ServerName,
                        certificatePath: _options.Server.CertificatePath,
                        enabled: true
                    );
                }
            }

            return _factory;
        }

        private void BlockAccessAfterDispose()
            => ObjectDisposedException.ThrowIf(_disposed, typeof(ConnectionManager));

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _connection?.Dispose();
            }

            _connection = null;
            _factory = null;
            _disposed = true;
        }

        #endregion

        #region IConnectionManager Members

        public IConnection GetConnection() {
            BlockAccessAfterDispose();

            return _connection ??= GetConnectionFactory().CreateConnection();
        }

        #endregion

        #region Destructor

        ~ConnectionManager() {
            Dispose(disposing: false);
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
