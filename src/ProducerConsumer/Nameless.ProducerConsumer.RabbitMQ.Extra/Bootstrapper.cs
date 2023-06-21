using Autofac;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {

    internal sealed class Bootstrapper : IStartable {

        #region Private Read-Only Fields

        private readonly IConnection _connection;
        private readonly ProducerConsumerOptions _options;

        #endregion

        #region Internal Constructors

        internal Bootstrapper(IConnection connection, ProducerConsumerOptions options) {
            _connection = connection;
            _options = options;
        }

        #endregion

        #region Private Static Methods

        private static void ConfigureExchange(IModel channel, ExchangeOptions exchange) {
            channel.ExchangeDeclare(
                exchange: exchange.Name,
                type: exchange.Type.GetDescription(),
                durable: exchange.Durable,
                autoDelete: exchange.AutoDelete,
                arguments: exchange.Arguments
            );
        }

        private static void ConfigureQueues(IModel channel, string exchangeName, IEnumerable<QueueOptions> queues) {
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
                    arguments: queue.Bindings
                );
            }
        }

        #endregion

        #region IStartable Members

        void IStartable.Start() {
            using var channel = _connection.CreateModel();

            foreach (var exchange in _options.Exchanges) {
                ConfigureExchange(channel, exchange);
                ConfigureQueues(channel, exchange.Name, exchange.Queues);
            }
        }

        #endregion
    }
}
