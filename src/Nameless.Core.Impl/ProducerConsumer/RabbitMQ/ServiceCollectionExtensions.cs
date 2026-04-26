using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Register the Producer/Consumer services for RabbitMQ.
        /// </summary>
        /// <param name="registration">
        ///     The registration delegate.
        /// </param>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     ca be chained.
        /// </returns>
        public IServiceCollection RegisterProducerConsumer(Action<ProducerConsumerRegistration>? registration = null, IConfiguration? configuration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            //????
            self.ConfigureOptions<ServerOptions>(configuration);

            self.TryAddSingleton<IConnectionManager, ConnectionManager>();
            self.TryAddSingleton<IChannelFactory, ChannelFactory>();
            self.TryAddSingleton<IProducer, Producer>();
            self.TryAddSingleton<IMessageSerializer, JsonMessageSerializer>();
            
            self.TryAddEnumerable(
                descriptors: CreateConsumerServiceDescriptors(settings)
            );

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateConsumerServiceDescriptors(ProducerConsumerRegistration settings) {
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan(typeof(Consumer<>))
            : settings.Consumers;

        return implementations.Select(
            implementation => ServiceDescriptor.Singleton(
                typeof(IHostedService),
                implementation
            )
        );
    }
}