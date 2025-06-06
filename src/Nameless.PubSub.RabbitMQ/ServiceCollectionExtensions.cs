using Microsoft.Extensions.DependencyInjection;
using Nameless.PubSub.RabbitMQ.Infrastructure;

namespace Nameless.PubSub.RabbitMQ;

public static class ServiceCollectionExtensions {
    public static IServiceCollection RegisterPubSubServices(this IServiceCollection self, Action<RabbitMQOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddSingleton<IConnectionManager, ConnectionManager>()
                   .AddSingleton<IChannelConfigurator, ChannelConfigurator>()
                   .AddSingleton<IChannelFactory, ChannelFactory>()
                   .AddSingleton<IPublisher, Publisher>()
                   .AddSingleton<ISubscriber, Subscriber>();
    }
}