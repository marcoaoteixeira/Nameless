using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Services.Impl {
    public sealed class ChannelManager : IChannelManager, IDisposable {
        #region Private Read-Only Fields

        private readonly IConnection _connection;

        #endregion

        #region Private Fields

        private IModel? _channel;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public ChannelManager(IConnection connection) {
            _connection = Guard.Against.Null(connection, nameof(connection));
        }

        #endregion

        #region Destructor

        ~ChannelManager() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(ChannelManager));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _channel?.Dispose();
            }

            _channel = null;
            _disposed = true;
        }

        #endregion

        #region IChannelManager Members

        public IModel GetChannel() {
            BlockAccessAfterDispose();

            return _channel ??= _connection.CreateModel();
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
