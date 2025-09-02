using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string CONNECTION_MANAGER_KEY = $"{nameof(IConnectionManager)} :: 27ec4ac9-4ac0-4341-afbb-87c0dcbd84eb";
    private const string CHANNEL_CONFIGURATOR_KEY = $"{nameof(IChannelConfigurator)} :: b1102d4d-3289-4b66-afc1-329828dc3c92";
    private const string CHANNEL_FACTORY_KEY = $"{nameof(IChannelFactory)} :: 910a347c-da4a-43c5-b795-ecd5cc2f3d96";

    /// <summary>
    ///     Register the Producer/Consumer services for RabbitMQ.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <param name="configure">
    ///     The configuration action.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     ca be chained.
    /// </returns>
    public static IServiceCollection RegisterProducerConsumer(this IServiceCollection self, Action<RabbitMQOptions>? configure = null) {
        self.Configure(configure ?? (_ => { }));

        return self.InnerRegisterProducerConsumer();
    }

    /// <summary>
    ///     Register the Producer/Consumer services for RabbitMQ.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     ca be chained.
    /// </returns>
    public static IServiceCollection RegisterProducerConsumer(this IServiceCollection self, IConfiguration configuration) {
        var section = configuration.GetSection(nameof(RabbitMQOptions));

        self.Configure<RabbitMQOptions>(section);

        return self.InnerRegisterProducerConsumer();
    }

    private static IServiceCollection InnerRegisterProducerConsumer(this IServiceCollection self) {
        self.TryAddKeyedSingleton<IConnectionManager, ConnectionManager>(CONNECTION_MANAGER_KEY);
        self.TryAddKeyedSingleton<IChannelConfigurator, ChannelConfigurator>(CHANNEL_CONFIGURATOR_KEY);
        self.TryAddKeyedSingleton(CHANNEL_FACTORY_KEY, ResolveChannelFactory);
        self.TryAddSingleton(ResolveProducerFactory);
        self.TryAddSingleton(ResolveConsumerFactory);

        return self;
    }

    private static IChannelFactory ResolveChannelFactory(IServiceProvider provider) {
        var connectionManager = provider.GetRequiredKeyedService<IConnectionManager>(CONNECTION_MANAGER_KEY);

        return new ChannelFactory(connectionManager);
    }

    private static IProducerFactory ResolveProducerFactory(IServiceProvider provider) {
        var channelFactory = provider.GetRequiredKeyedService<IChannelFactory>(CHANNEL_FACTORY_KEY);
        var channelConfigurator = provider.GetRequiredKeyedService<IChannelConfigurator>(CHANNEL_CONFIGURATOR_KEY);
        var timeProvider = provider.GetService<TimeProvider>() ?? TimeProvider.System;
        var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

        return new ProducerFactory(channelFactory, channelConfigurator, timeProvider, loggerFactory);
    }

    private static IConsumerFactory ResolveConsumerFactory(IServiceProvider provider) {
        var channelFactory = provider.GetRequiredKeyedService<IChannelFactory>(CHANNEL_FACTORY_KEY);
        var channelConfigurator = provider.GetRequiredKeyedService<IChannelConfigurator>(CHANNEL_CONFIGURATOR_KEY);
        var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

        return new ConsumerFactory(channelFactory, channelConfigurator, loggerFactory);
    }
}