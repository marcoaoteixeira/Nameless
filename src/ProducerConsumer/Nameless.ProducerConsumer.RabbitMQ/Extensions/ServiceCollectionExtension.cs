using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddRabbitMQ(this IServiceCollection self, Action<RabbitMQOptions> configure)
        => self.Configure(configure)
               .RegisterRabbitMQServices();

    public static IServiceCollection AddRabbitMQ(this IServiceCollection self, IConfigurationSection rabbitMQConfigSection)
        => self.Configure<RabbitMQOptions>(rabbitMQConfigSection)
               .RegisterRabbitMQServices();

    private static IServiceCollection RegisterRabbitMQServices(this IServiceCollection self)
        => self.AddSingleton<IConnectionManager, ConnectionManager>()
               .AddSingleton<IChannelProvider, ChannelProvider>()
               .AddSingleton<IProducer, Producer>()
               .AddSingleton<IConsumer, Consumer>();
}