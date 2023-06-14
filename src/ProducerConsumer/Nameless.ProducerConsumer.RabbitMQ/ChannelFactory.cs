using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {

    public sealed class ChannelFactory : IChannelFactory {

        #region Private Read-Only Fields

        private readonly IConnection _connection;

        #endregion

        #region Public Constructors

        public ChannelFactory(IConnection connection) {
            Prevent.Null(connection, nameof(connection));

            _connection = connection;
        }

        #endregion

        #region Private Static Methods

        private static void ConfigureExchange(IModel channel, ExchangeSettings exchange) {
            channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Type.GetDescription(),
                durable: exchange.Durable,
                autoDelete: exchange.AutoDelete,
                arguments: exchange.Arguments
            );
        }

        private static void ConfigureQueues(IModel channel, string exchangeName, QueueSettings[] queues) {
            foreach (var queue in queues) {
                channel.QueueDeclare(
                    queue: queue.Name,
                    durable: queue.Durable,
                    exclusive: queue.Exclusive,
                    autoDelete: queue.AutoDelete,
                    arguments: queue.Arguments
                );

                channel.QueueBind(
                    queue: queue.Name,
                    exchange: exchangeName,
                    routingKey: queue.RoutingKey ?? queue.Name,
                    arguments: new Dictionary<string, object>()
                );
            }
        }

        #endregion

        #region IChannelFactory Members

        public IModel Create(IEnumerable<ExchangeSettings> exchanges) {
            var channel = _connection.CreateModel();

            foreach (var exchange in exchanges) {
                ConfigureExchange(channel, exchange);
                ConfigureQueues(channel, exchange.Name, exchange.Queues.ToArray());
            }

            return channel;
        }

        #endregion
    }
}
