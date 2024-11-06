using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddRabbitMQ(this IServiceCollection self, Action<RabbitMQOptions> configure)
        => self.Configure(configure)
               .RegisterRabbitMQServices();

    public static IServiceCollection AddRabbitMQ(this IServiceCollection self, IConfigurationSection rabbitMQConfigSection)
        => self.Configure<RabbitMQOptions>(rabbitMQConfigSection)
               .RegisterRabbitMQServices();

    private static IServiceCollection RegisterRabbitMQServices(this IServiceCollection self)
        => self.AddSingleton<IChannelManager>(ResolveChannelManager)
               .AddSingleton<IProducer, Producer>()
               .AddSingleton<IConsumer, Consumer>();

    private static ChannelManager ResolveChannelManager(IServiceProvider provider) {
        var options = provider.GetOptions<RabbitMQOptions>();
        var logger = provider.GetLogger<ChannelManager>();
        var channelFactory = new ChannelManager(options, logger);

        StartUp(channelFactory.GetChannel(), options.Value);

        return channelFactory;
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

    private static void DeclareBinding(IModel channel, ExchangeSettings exchange, QueueSettings queue, BindingSettings binding)
        => channel.QueueBind(queue: queue.Name,
                             exchange: exchange.Name,
                             routingKey: binding.RoutingKey,
                             arguments: binding.Arguments);

    private static void DeclareQueue(IModel channel, QueueSettings queue)
        => channel.QueueDeclare(queue: queue.Name,
                                durable: queue.Durable,
                                exclusive: queue.Exclusive,
                                autoDelete: queue.AutoDelete,
                                arguments: queue.Arguments);

    private static void DeclareExchange(IModel channel, ExchangeSettings exchange)
        => channel.ExchangeDeclare(exchange: exchange.Name,
                                   type: exchange.Type.GetDescription(),
                                   durable: exchange.Durable,
                                   autoDelete: exchange.AutoDelete,
                                   arguments: exchange.Arguments);
}