using Microsoft.Extensions.Logging;
using Nameless.Services;
using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ;

public sealed class Publisher : IPublisher {
    private readonly IChannelFactory _channelFactory;
    private readonly IClock _clock;
    private readonly ILogger<Publisher> _logger;

    public Publisher(IChannelFactory channelFactory,
                     IClock clock,
                     ILogger<Publisher> logger) {
        _channelFactory = Prevent.Argument.Null(channelFactory);
        _clock = Prevent.Argument.Null(clock);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Parameter <paramref name="topic"/> will be a routing key.
    /// The exchange name will be set through extension method
    /// <see cref="PublisherArgsExtension.GetExchangeName"/>.
    /// </remarks>
    public async Task PublishAsync(string topic,
                                   object message,
                                   PublisherArgs args,
                                   CancellationToken cancellationToken) {
        Prevent.Argument.Null(topic);
        Prevent.Argument.Null(message);

        try {
            var properties = CreateProperties(args);
            var envelope = CreateEnvelope(message, properties);

            await using var channel = await _channelFactory.CreateChannelAsync(args.GetExchangeName(), cancellationToken)
                                                            .ConfigureAwait(continueOnCapturedContext: false);

            var routingKeys = args.GetRoutingKeys().Length > 0
                ? args.GetRoutingKeys()
                : [string.Empty];

            var buffer = envelope.CreateBuffer();

            foreach (var routingKey in routingKeys) {
                await channel.BasicPublishAsync(topic,
                                                routingKey,
                                                mandatory: args.GetMandatory(),
                                                basicProperties: properties,
                                                body: buffer,
                                                cancellationToken)
                             .ConfigureAwait(continueOnCapturedContext: false);
            }
        } catch (Exception ex) {
            _logger.UnhandledErrorWhilePublishing(ex);

            throw;
        }
    }

    private Envelope CreateEnvelope(object message, BasicProperties properties)
        => new(message,
               properties.MessageId,
               properties.CorrelationId,
               _clock.GetUtcNowOffset());

    private static BasicProperties CreateProperties(PublisherArgs args)
        => new BasicProperties().FillWith(args);
}
