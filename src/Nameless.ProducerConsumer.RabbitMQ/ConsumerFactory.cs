using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Default implementation of <see cref="IConsumerFactory"/> for RabbitMQ consumer factory.
/// </summary>
public sealed class ConsumerFactory : IConsumerFactory {
    private readonly IChannelFactory _channelFactory;
    private readonly IChannelConfigurator _channelConfigurator;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<ConsumerFactory> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ConsumerFactory"/>.
    /// </summary>
    /// <param name="channelFactory">The channel factory.</param>
    /// <param name="channelConfigurator">The channel configurator.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ConsumerFactory(IChannelFactory channelFactory, IChannelConfigurator channelConfigurator, ILoggerFactory loggerFactory) {
        _channelFactory = Prevent.Argument.Null(channelFactory);
        _channelConfigurator = Prevent.Argument.Null(channelConfigurator);
        _loggerFactory = Prevent.Argument.Null(loggerFactory);
        _logger = loggerFactory.CreateLogger<ConsumerFactory>();
    }

    /// <inheritdoc />
    public async Task<IConsumer<TMessage>> CreateAsync<TMessage>(string topic, Parameters parameters, CancellationToken cancellationToken)
        where TMessage : notnull {
        IChannel? channel = null;

        try {
            channel = await _channelFactory.CreateAsync(cancellationToken)
                                           .ConfigureAwait(false);

            await _channelConfigurator.ConfigureAsync(channel,
                                           topic,
                                           parameters.GetUsePrefetch(),
                                           cancellationToken)
                                      .ConfigureAwait(false);

            var consumerLogger = _loggerFactory.CreateLogger<Consumer<TMessage>>();

            return new Consumer<TMessage>(topic, channel, consumerLogger);
        }
        catch (Exception ex) {
            _logger.UnhandledErrorWhileCreatingConsumer(ex);

            // if we have a channel, we probably should dispose it.
            if (channel is not null) {
                await channel.DisposeAsync()
                             .ConfigureAwait(false);
            }

            throw;
        }
    }
}