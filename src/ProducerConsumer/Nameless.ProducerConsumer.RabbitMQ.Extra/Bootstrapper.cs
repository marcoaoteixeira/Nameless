﻿using Autofac;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    internal sealed class Bootstrapper : IStartable {
        #region Private Read-Only Fields

        private readonly IModel _channel;
        private readonly ProducerConsumerOptions _options;

        #endregion

        #region Internal Constructors

        internal Bootstrapper(IModel channel, ProducerConsumerOptions options) {
            _channel = channel;
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
            foreach (var exchange in _options.Exchanges) {
                ConfigureExchange(_channel, exchange);
                ConfigureQueues(_channel, exchange.Name, exchange.Queues);
            }
        }

        #endregion
    }
}
