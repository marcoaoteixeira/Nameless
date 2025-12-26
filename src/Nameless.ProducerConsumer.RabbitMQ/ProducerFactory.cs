using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Default implementation of <see cref="IProducerFactory"/> for RabbitMQ producers.
/// </summary>
public sealed class ProducerFactory : IProducerFactory {
    private readonly IChannelFactory _channelFactory;
    private readonly IChannelConfigurator _channelConfigurator;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ProducerFactory> _logger;
    private readonly ILogger<Producer> _producerLogger;

    public ProducerFactory(IChannelFactory channelFactory,
        IChannelConfigurator channelConfigurator,
        TimeProvider timeProvider,
        ILoggerFactory loggerFactory) {
        _channelFactory = Guard.Against.Null(channelFactory);
        _channelConfigurator = Guard.Against.Null(channelConfigurator);
        _timeProvider = Guard.Against.Null(timeProvider);
        _logger = loggerFactory.CreateLogger<ProducerFactory>();
        _producerLogger = loggerFactory.CreateLogger<Producer>();
    }

    /// <inheritdoc />
    public async Task<IProducer> CreateAsync(string topic, Parameters parameters, CancellationToken cancellationToken) {
        IChannel? channel = null;

        try {
            channel = await _channelFactory.CreateAsync(cancellationToken)
                                           .ConfigureAwait(continueOnCapturedContext: false);

            await _channelConfigurator.ConfigureAsync(channel,
                                          topic,
                                          usePrefetch: false,
                                          cancellationToken)
                                      .ConfigureAwait(continueOnCapturedContext: false);

            return new Producer(topic, channel, _timeProvider, _producerLogger);
        }
        catch (Exception ex) {
            _logger.UnhandledErrorWhileCreatingProducer(ex);

            // if the channel was created, then dispose it.
            if (channel is not null) {
                await channel.DisposeAsync()
                             .ConfigureAwait(continueOnCapturedContext: false);
            }

            throw;
        }
    }
}