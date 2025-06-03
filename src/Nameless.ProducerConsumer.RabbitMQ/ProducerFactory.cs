using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Default implementation of <see cref="IProducerFactory"/> for RabbitMQ producers.
/// </summary>
public sealed class ProducerFactory : IProducerFactory {
    private readonly IChannelFactory _channelFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ProducerFactory> _producerFactoryLogger;
    private readonly ILogger<Producer> _producerLogger;

    public ProducerFactory(IChannelFactory channelFactory, TimeProvider timeProvider, ILoggerFactory loggerFactory) {
        _channelFactory = Prevent.Argument.Null(channelFactory);
        _timeProvider = Prevent.Argument.Null(timeProvider);
        _producerFactoryLogger = loggerFactory.CreateLogger<ProducerFactory>();
        _producerLogger = loggerFactory.CreateLogger<Producer>();
    }

    /// <inheritdoc />
    public async Task<IProducer> CreateAsync(string topic, Args args, CancellationToken cancellationToken) {
        IChannel? channel = null;

        try {
            channel = await _channelFactory.CreateAsync(args.GetExchangeName(), cancellationToken)
                                           .ConfigureAwait(continueOnCapturedContext: false);

            return new Producer(channel, topic, _timeProvider, _producerLogger);
        }
        catch (Exception ex) {
            _producerFactoryLogger.UnhandledErrorWhileCreatingProducer(ex);

            throw;
        }
        finally {
            if (channel is not null) {
                await channel.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}