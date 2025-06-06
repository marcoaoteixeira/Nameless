using Microsoft.Extensions.Logging;
using Nameless.PubSub.RabbitMQ.Infrastructure;
using Nameless.PubSub.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ;

public sealed class Publisher : IPublisher {
    private readonly IChannelFactory _channelFactory;
    private readonly ILogger<Publisher> _logger;
    private readonly TimeProvider _timeProvider;

    public Publisher(IChannelFactory channelFactory,
                     TimeProvider timeProvider,
                     ILogger<Publisher> logger) {
        _channelFactory = Prevent.Argument.Null(channelFactory);
        _timeProvider = Prevent.Argument.Null(timeProvider);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Parameter <paramref name="topic" /> will be a routing key.
    ///     The exchange name will be set through extension method
    ///     <see cref="PublisherArgsExtensions.GetExchangeName" />.
    /// </remarks>
    public async Task PublishAsync(string topic,
                                   object message,
                                   PublisherArgs args,
                                   CancellationToken cancellationToken) {
        Prevent.Argument.Null(topic);
        Prevent.Argument.Null(message);

        try {
            await using var channel = await _channelFactory.CreateChannelAsync(args.GetExchangeName(), cancellationToken)
                                                           .ConfigureAwait(false);

            var properties = CreateProperties(args);
            var buffer = CreateEnvelope(message, properties).GetBuffer();

            foreach (var routingKey in args.GetRoutingKeys()) {
                await channel.BasicPublishAsync(topic,
                                  routingKey,
                                  args.GetMandatory(),
                                  properties,
                                  buffer,
                                  cancellationToken)
                             .ConfigureAwait(false);
            }
        }
        catch (Exception ex) {
            _logger.UnhandledErrorWhilePublishing(ex);

            throw;
        }
    }

    private Envelope CreateEnvelope(object message, BasicProperties properties) {
        return new Envelope(message, properties.MessageId, properties.CorrelationId, _timeProvider.GetUtcNow());
    }

    private static BasicProperties CreateProperties(PublisherArgs args) {
        return new BasicProperties().FillWith(args);
    }
}