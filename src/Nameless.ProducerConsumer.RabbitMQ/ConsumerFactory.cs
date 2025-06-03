using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Default implementation of <see cref="IConsumerFactory"/> for RabbitMQ consumer factory.
/// </summary>
public sealed class ConsumerFactory : IConsumerFactory {
    private readonly IChannelFactory _channelFactory;
    private readonly ILogger<ConsumerFactory> _consumerFactoryLogger;
    private readonly ILogger<Consumer> _consumerLogger;

    /// <summary>
    /// Initializes a new instance of <see cref="ConsumerFactory"/>.
    /// </summary>
    /// <param name="channelFactory">The channel factory.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ConsumerFactory(IChannelFactory channelFactory, ILoggerFactory loggerFactory) {
        _channelFactory = Prevent.Argument.Null(channelFactory);

        Prevent.Argument.Null(loggerFactory);

        _consumerFactoryLogger = loggerFactory.CreateLogger<ConsumerFactory>();
        _consumerLogger = loggerFactory.CreateLogger<Consumer>();
    }

    /// <inheritdoc />
    public async Task<IConsumer> CreateAsync(string topic, Args args, CancellationToken cancellationToken) {
        IChannel? channel = null;

        try {
            channel = await _channelFactory.CreateAsync(topic, cancellationToken)
                                           .ConfigureAwait(continueOnCapturedContext: false);

            return new Consumer(channel, topic, _consumerLogger);
        }
        catch (Exception ex) {
            _consumerFactoryLogger.UnhandledErrorWhileCreatingConsumer(ex);

            throw;
        }
        finally {
            if (channel is not null) {
                await channel.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}