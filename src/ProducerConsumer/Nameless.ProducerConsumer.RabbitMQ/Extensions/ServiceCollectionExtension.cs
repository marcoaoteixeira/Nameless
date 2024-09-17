using Microsoft.Extensions.DependencyInjection;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using Nameless.ProducerConsumer.RabbitMQ.Services.Impl;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ServiceCollectionExtension {
    private const string CHANNEL_TOKEN = $"{nameof(IModel)}::f26120de-83b1-4ffb-aab9-78a6dfda160b";

    public static IServiceCollection RegisterProducerConsumer(this IServiceCollection self, Action<RabbitMQOptions>? configure = null)
        => self.AddKeyedSingleton(serviceKey: CHANNEL_TOKEN,
                                  implementationFactory: (provider, _) => CreateChannel(provider, configure))
               .AddSingleton<IProducerService>(provider => new ProducerService(channel: provider.GetRequiredKeyedService<IModel>(CHANNEL_TOKEN),
                                                                               logger: provider.GetLogger<ProducerService>()))
               .AddSingleton<IConsumerService>(provider => new ConsumerService(channel: provider.GetRequiredKeyedService<IModel>(CHANNEL_TOKEN),
                                                                               logger: provider.GetLogger<ConsumerService>()));

    private static IModel CreateChannel(IServiceProvider provider, Action<RabbitMQOptions>? configure = null) {
        var options = provider.GetOptions<RabbitMQOptions>();

        configure?.Invoke(options.Value);

        var channelFactory = new ChannelFactory(options: options.Value,
                                                logger: provider.GetLogger<ChannelFactory>());

        var channel = channelFactory.CreateChannel();

        StartUp(channel, options.Value);

        return channel;
    }

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
}