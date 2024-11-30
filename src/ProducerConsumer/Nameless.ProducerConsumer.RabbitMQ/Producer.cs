using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using Nameless.Services;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class Producer : IProducer {
    private readonly IChannelProvider _channelProvider;
    private readonly IClock _clock;
    private readonly ILogger<Producer> _logger;

    public Producer(IChannelProvider channelProvider, IClock clock, ILogger<Producer> logger) {
        _channelProvider = Prevent.Argument.Null(channelProvider);
        _clock = Prevent.Argument.Null(clock);
        _logger = Prevent.Argument.Null(logger);
    }

    public async Task ProduceAsync(string topic,
                                   object message,
                                   ProducerArgs args,
                                   CancellationToken cancellationToken) {
        try {
            var properties = CreateProperties(args);
            var envelope = CreateEnvelope(message, properties);

            await using var channel = await _channelProvider.CreateChannelAsync(args.GetExchangeName(),
                                                                                cancellationToken)
                                                            .ConfigureAwait(continueOnCapturedContext: false);

            var routingKeys = args.GetRoutingKeys()
                                  .Append(topic);

            var buffer = envelope.CreateBuffer();

            foreach (var routingKey in routingKeys) {
                await channel.BasicPublishAsync(args.GetExchangeName(),
                                                routingKey,
                                                mandatory: args.GetMandatory(),
                                                basicProperties: properties,
                                                body: buffer,
                                                cancellationToken)
                             .ConfigureAwait(continueOnCapturedContext: false);
            }
        } catch (Exception ex) {
            _logger.ErrorOnMessagePublishing(ex);

            throw;
        }
    }

    private Envelope CreateEnvelope(object message, BasicProperties properties)
        => new() {
            Message = message,
            MessageId = properties.MessageId,
            CorrelationId = properties.CorrelationId,
            PublishedAt = _clock.GetUtcNowOffset()
        };

    private static BasicProperties CreateProperties(ProducerArgs innerArgs)
        => new BasicProperties().FillWith(innerArgs);
}