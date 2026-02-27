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

    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Register the Producer/Consumer services for RabbitMQ.
        /// </summary>
        /// <param name="configure">
        ///     The configuration action.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     ca be chained.
        /// </returns>
        public IServiceCollection RegisterProducerConsumer(Action<RabbitMQOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterProducerConsumer();
        }

        /// <summary>
        ///     Register the Producer/Consumer services for RabbitMQ.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     ca be chained.
        /// </returns>
        public IServiceCollection RegisterProducerConsumer(IConfiguration configuration) {
            var section = configuration.GetSection<RabbitMQOptions>();

            return self.Configure<RabbitMQOptions>(section)
                       .InnerRegisterProducerConsumer();
        }

        private IServiceCollection InnerRegisterProducerConsumer() {
            self.TryAddKeyedSingleton<IConnectionManager, ConnectionManager>(CONNECTION_MANAGER_KEY);
            self.TryAddKeyedSingleton<IChannelConfigurator, ChannelConfigurator>(CHANNEL_CONFIGURATOR_KEY);
            self.TryAddKeyedSingleton<IChannelFactory>(CHANNEL_FACTORY_KEY, ResolveChannelFactory);
            self.TryAddSingleton<IProducerFactory>(ResolveProducerFactory);
            self.TryAddSingleton<IConsumerFactory>(ResolveConsumerFactory);

            return self;
        }
    }

    private static ChannelFactory ResolveChannelFactory(IServiceProvider provider, object? _) {
        var connectionManager = provider.GetRequiredKeyedService<IConnectionManager>(
            CONNECTION_MANAGER_KEY
        );

        return new ChannelFactory(connectionManager);
    }

    private static ProducerFactory ResolveProducerFactory(IServiceProvider provider) {
        var channelFactory = provider.GetRequiredKeyedService<IChannelFactory>(
            CHANNEL_FACTORY_KEY
        );
        var channelConfigurator = provider.GetRequiredKeyedService<IChannelConfigurator>(
            CHANNEL_CONFIGURATOR_KEY
        );
        var timeProvider = provider.GetService<TimeProvider>() ?? TimeProvider.System;
        var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

        return new ProducerFactory(channelFactory, channelConfigurator, timeProvider, loggerFactory);
    }

    private static ConsumerFactory ResolveConsumerFactory(IServiceProvider provider) {
        var channelFactory = provider.GetRequiredKeyedService<IChannelFactory>(
            CHANNEL_FACTORY_KEY
        );
        var channelConfigurator = provider.GetRequiredKeyedService<IChannelConfigurator>(
            CHANNEL_CONFIGURATOR_KEY
        );
        var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

        return new ConsumerFactory(channelFactory, channelConfigurator, loggerFactory);
    }
}