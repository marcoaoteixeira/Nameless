namespace Nameless.ProducerConsumer.RabbitMQ.Options {
    public sealed class QueueOptions {
        #region Public Static Read-Only Fields

        public static QueueOptions Default => new();

        #endregion

        #region Private Fields

        private Dictionary<string, object>? _arguments;
        private BindingOptions[]? _bindings;
        private string? _name;

        #endregion

        #region Public Properties

        public string Name {
            get => _name ??= Root.Defaults.QUEUE_NAME;
            set => _name = Guard.Against.NullOrWhiteSpace(value, nameof(value));
        }

        public bool Durable { get; set; } = true;

        public bool Exclusive { get; set; }

        public bool AutoDelete { get; set; }

        public Dictionary<string, object> Arguments {
            get => _arguments ??= [];
            set => _arguments = value;
        }

        public BindingOptions[] Bindings {
            get => _bindings ??= [BindingOptions.Default];
            set => _bindings = value.IsNullOrEmpty()
                ? [BindingOptions.Default]
                : value;
        }

        #endregion
    }
}
