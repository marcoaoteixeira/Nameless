namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed record BindingOptions {
        #region Public Static Read-Only Fields

        public static BindingOptions Default => new();

        #endregion

        #region Private Fields

        private string? _routingKey;
        private Dictionary<string, object>? _arguments;

        #endregion

        #region Public Properties

        public string RoutingKey {
            get => _routingKey ??= string.Empty;
            set => _routingKey = value;
        }

        public Dictionary<string, object> Arguments {
            get => _arguments ??= [];
            set => _arguments = value;
        }

        #endregion
    }
}
