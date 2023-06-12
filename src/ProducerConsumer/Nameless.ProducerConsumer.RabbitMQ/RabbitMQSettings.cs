using System.ComponentModel;

namespace Nameless.ProducerConsumer.RabbitMQ {

    public sealed class RabbitMQSettings {

        #region Private Fields

        private IList<ExchangeSettings>? _exchanges;

        #endregion

        #region Public Properties

        public ServerSettings Server { get; set; } = new();
        public IList<ExchangeSettings> Exchanges {
            get { return _exchanges ??= new List<ExchangeSettings>(); }
            set { _exchanges = value; }
        }

        #endregion
    }

    public sealed class ServerSettings {

        #region Public Properties

        public bool UseSsl { get; set; } = false;
        public string? ServerName { get; set; }
        public string? CertificatePath { get; set; }
        public string Hostname { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string? Username { get; set; } = "guest";
        public string? Password { get; set; } = "guest";

        #endregion

        #region Public Override Methods

        public override string ToString() {
            return string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Password)
                ? $"{(UseSsl ? "amqps" : "amqp")}://{Hostname}:{Port}/"
                : $"{(UseSsl ? "amqps" : "amqp")}://{Username}:{Password}@{Hostname}:{Port}/";
        }

        #endregion
    }

    public sealed class ExchangeSettings {

        #region Private Fields

        private IDictionary<string, object>? _arguments;
        private IList<QueueSettings>? _queues;

        #endregion

        #region Public Properties

        public string Name { get; set; }
        public ExchangeType Type { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments {
            get { return _arguments ??= new Dictionary<string, object>(); }
            set { _arguments = value; }
        }
        public IList<QueueSettings> Queues {
            get { return _queues ??= new List<QueueSettings>(); }
            set { _queues = value; }
        }

        #endregion

        #region Public Constructors

        public ExchangeSettings() {
            Name = Constants.DEFAULT_EXCHANGE_NAME;
        }

        #endregion
    }

    public sealed class QueueSettings {

        #region Private Fields

        private IDictionary<string, object>? _arguments;

        #endregion

        #region Public Properties

        public string? Name { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public string? RoutingKey { get; set; }
        public IDictionary<string, object> Arguments {
            get { return _arguments ??= new Dictionary<string, object>(); }
            set { _arguments = value; }
        }

        #endregion

        #region Public Constructors

        public QueueSettings() {
            Name = Constants.DEFAULT_QUEUE_NAME;
        }

        #endregion
    }

    public enum ExchangeType {
        [Description("topic")]
        Topic = 0,
        [Description("queue")]
        Queue = 1,
        [Description("fanout")]
        Fanout = 2,
        [Description("direct")]
        Direct = 4,
        [Description("headers")]
        Headers = 8
    }
}