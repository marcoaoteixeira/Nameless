namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed class ExchangeOptions {
        #region Public Static Read-Only Fields

        public static ExchangeOptions Default => new();

        #endregion

        #region Private Fields

        private Dictionary<string, object>? _arguments;
        private QueueOptions[]? _queues;
        private string? _name;

        #endregion

        #region Public Properties

        public string Name {
            get => _name ??= Root.Defaults.EXCHANGE_NAME;
            set => _name = value;
        }

        public ExchangeType Type { get; set; }

        public bool Durable { get; set; } = true;

        public bool AutoDelete { get; set; }

        public Dictionary<string, object> Arguments {
            get => _arguments ??= [];
            set => _arguments = value;
        }

        public QueueOptions[] Queues {
            get => _queues ??= [];
            set => _queues = value;
        }

        #endregion
    }
}
