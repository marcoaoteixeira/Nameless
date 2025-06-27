using Microsoft.Extensions.DependencyInjection;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Register the Producer/Consumer services for RabbitMQ.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions ca be chained.
    /// </returns>
    public static IServiceCollection ConfigureProducerConsumerServices(this IServiceCollection self, Action<RabbitMQOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddSingleton<IConnectionManager, ConnectionManager>()
                   .AddSingleton<IChannelConfigurator, ChannelConfigurator>()
                   .AddSingleton<IChannelFactory, ChannelFactory>()
                   .AddSingleton<IProducerFactory, ProducerFactory>()
                   .AddSingleton<IConsumerFactory, ConsumerFactory>();
    }
}