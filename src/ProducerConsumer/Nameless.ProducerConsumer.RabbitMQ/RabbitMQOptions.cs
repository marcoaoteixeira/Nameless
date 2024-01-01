using System.ComponentModel;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public sealed class RabbitMQOptions {
        #region Private Fields

        private List<ExchangeOptions>? _exchanges;

        #endregion

        #region Public Static Read-Only Properties

        public static RabbitMQOptions Default => new();

        #endregion

        #region Public Properties

        public ServerOptions Server { get; set; } = new();
        public List<ExchangeOptions> Exchanges {
            get => _exchanges ??= [];
            set => _exchanges = value;
        }

        #endregion
    }

    public sealed class ServerOptions {
        #region Public Constructors

        public ServerOptions() {
            Username = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_USER)
                ?? Root.Defaults.RABBITMQ_USER;
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.RABBITMQ_PASS)
                ?? Root.Defaults.RABBITMQ_PASS;
        }

        #endregion

        #region Public Properties

        public bool UseSsl { get; set; } = false;
        public string? ServerName { get; set; }
        public string? CertificatePath { get; set; }
        public string Hostname { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string? Username { get; }
        public string? Password { get; }

        #endregion

        #region Public Override Methods

        public override string ToString() => string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Password)
                ? $"{(UseSsl ? "amqps" : "amqp")}://{Hostname}:{Port}/"
                : $"{(UseSsl ? "amqps" : "amqp")}://{Username}:{Password}@{Hostname}:{Port}/";

        #endregion
    }

    public enum ExchangeType {
        [Description("direct")]
        Direct = 0,
        [Description("topic")]
        Topic = 1,
        [Description("queue")]
        Queue = 2,
        [Description("fanout")]
        Fanout = 4,
        [Description("headers")]
        Headers = 8
    }

    public sealed class ExchangeOptions {
        #region Private Fields

        private Dictionary<string, object>? _arguments;
        private List<QueueOptions>? _queues;

        #endregion

        #region Public Properties

        public string Name { get; set; }
        public ExchangeType Type { get; set; }
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; }
        public Dictionary<string, object> Arguments {
            get => _arguments ??= [];
            set => _arguments = value;
        }
        public List<QueueOptions> Queues {
            get => _queues ??= [];
            set => _queues = value;
        }

        #endregion

        #region Public Constructors

        public ExchangeOptions() {
            Name = Root.Defaults.EXCHANGE_NAME;
        }

        #endregion
    }

    public sealed class QueueOptions {

        #region Private Fields

        private Dictionary<string, object>? _arguments;
        private Dictionary<string, object>? _bindings;

        #endregion

        #region Public Properties

        public string? Name { get; set; }
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public string? RoutingKey { get; set; }
        public Dictionary<string, object> Arguments {
            get => _arguments ??= [];
            set => _arguments = value;
        }
        public Dictionary<string, object> Bindings {
            get => _bindings ??= [];
            set => _bindings = value;
        }

        #endregion

        #region Public Constructors

        public QueueOptions() {
            Name = Root.Defaults.QUEUE_NAME;
        }

        #endregion
    }
}
