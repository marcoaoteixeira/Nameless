﻿using Microsoft.Extensions.DependencyInjection;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public static class ServiceCollectionExtension {
        #region Private Constants

        private const string CHANNEL_TOKEN = $"{nameof(IModel)}::f26120de-83b1-4ffb-aab9-78a6dfda160b";

        #endregion

        #region Public Static Methods

        public static IServiceCollection RegisterProducerConsumer(this IServiceCollection self, Action<RabbitMQOptions>? configure = null)
            => self
               .AddKeyedSingleton(serviceKey: CHANNEL_TOKEN,
                                  implementationFactory: (provider, _) => {
                                      var options = provider.GetPocoOptions<RabbitMQOptions>();

                                      configure?.Invoke(options);

                                      var channelFactory = new ChannelFactory(
                                          options: options,
                                          logger: provider.GetLogger<ChannelFactory>()
                                      );

                                      var channel = channelFactory.CreateChannel();

                                      StartUp(channel, options);

                                      return channel;
                                  })
               .AddSingleton<IProducerService>(provider => new ProducerService(channel: provider.GetRequiredKeyedService<IModel>(CHANNEL_TOKEN),
                                                                               logger: provider.GetLogger<ProducerService>()))
               .AddSingleton<IConsumerService>(provider => new ConsumerService(channel: provider.GetRequiredKeyedService<IModel>(CHANNEL_TOKEN),
                                                                               logger: provider.GetLogger<ConsumerService>()));

        #endregion

        #region Private Static Methods

        private static void StartUp(IModel channel, RabbitMQOptions options) {
            // when we declare an exchange/queue, if the exchange/queue
            // doesn't exist, it will be created for us. Otherwise,
            // RabbitMQ will just ignore.

            foreach (var exchange in options.Exchanges) {
                // let's declare our exchange
                DeclareExchange(channel, exchange);

                foreach (var queue in exchange.Queues) {
                    // let's declare our queue
                    DeclareQueue(channel, queue);

                    // let's declare our bindings
                    foreach (var binding in queue.Bindings) {
                        DeclareBinding(channel, exchange, queue, binding);
                    }
                }
            }
        }

        private static void DeclareBinding(IModel channel, ExchangeOptions exchange, QueueOptions queue, BindingOptions binding)
            => channel.QueueBind(queue: queue.Name,
                                 exchange: exchange.Name,
                                 routingKey: binding.RoutingKey,
                                 arguments: binding.Arguments);

        private static void DeclareQueue(IModel channel, QueueOptions queue)
            => channel.QueueDeclare(queue: queue.Name,
                                    durable: queue.Durable,
                                    exclusive: queue.Exclusive,
                                    autoDelete: queue.AutoDelete,
                                    arguments: queue.Arguments);

        private static void DeclareExchange(IModel channel, ExchangeOptions exchange)
            => channel.ExchangeDeclare(exchange: exchange.Name,
                                       type: exchange.Type.GetDescription(),
                                       durable: exchange.Durable,
                                       autoDelete: exchange.AutoDelete,
                                       arguments: exchange.Arguments);

        #endregion
    }
}
