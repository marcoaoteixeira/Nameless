namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed record RabbitMQOptions {
        #region Public Static Read-Only Properties

        public static RabbitMQOptions Default => new();

        #endregion

        #region Private Fields

        private ServerOptions? _server;
        private ExchangeOptions[]? _exchanges;

        #endregion

        #region Public Properties

        public ServerOptions Server {
            get => _server ??= ServerOptions.Default;
            set => _server = value;
        }

        public ExchangeOptions[] Exchanges {
            get => _exchanges ??= [];
            set => _exchanges = value;
        }

        #endregion
    }
}
