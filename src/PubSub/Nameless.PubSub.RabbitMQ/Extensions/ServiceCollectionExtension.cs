using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.PubSub.RabbitMQ.Contracts;
using Nameless.PubSub.RabbitMQ.Options;

namespace Nameless.PubSub.RabbitMQ;

public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterRabbitMQPubSub(this IServiceCollection self, Action<RabbitMQOptions> configure)
        => self.Configure(configure)
               .RegisterServices();

    public static IServiceCollection RegisterRabbitMQPubSub(this IServiceCollection self, IConfigurationSection rabbitMQConfigSection)
        => self.Configure<RabbitMQOptions>(rabbitMQConfigSection)
               .RegisterServices();

    private static IServiceCollection RegisterServices(this IServiceCollection self)
        => self.AddSingleton<IConnectionManager, ConnectionManager>()
               .AddSingleton<IChannelFactory, ChannelFactory>()
               .AddSingleton<IPublisher, Publisher>()
               .AddSingleton<ISubscriber, Subscriber>();
}