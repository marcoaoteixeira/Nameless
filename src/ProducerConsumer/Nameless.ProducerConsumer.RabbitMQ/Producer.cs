using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class Producer : IProducer {
    private readonly IChannelManager _channelManager;
    private readonly ILogger<Producer> _logger;

    private IModel Channel
        => _channelManager.GetChannel();

    public Producer(IChannelManager channelManager, ILogger<Producer> logger) {
        _channelManager = Prevent.Argument.Null(channelManager);
        _logger = Prevent.Argument.Null(logger);
    }

    public Task ProduceAsync(string topic, object message, ProducerArgs? args = null, CancellationToken cancellationToken = default) {
        var innerArgs = args ?? ProducerArgs.Empty;
        var properties = CreateProperties(Channel, innerArgs);
        var envelope = CreateEnvelope(message, properties);

        try {
            var routingKeys = innerArgs.GetRoutingKeys()
                                       .Append(topic);
            var batch = Channel.CreateBasicPublishBatch();
            var buffer = envelope.CreateBuffer();

            foreach (var routingKey in routingKeys) {
                batch.Add(exchange: innerArgs.GetExchangeName(),
                          routingKey: routingKey,
                          mandatory: innerArgs.GetMandatory(),
                          properties: properties,
                          body: buffer);
            }

            batch.Publish();
        } catch (Exception ex) { _logger.ErrorOnMessagePublishing(ex); throw; }

        return Task.CompletedTask;
    }

    private static Envelope CreateEnvelope(object message, IBasicProperties properties)
        => new() {
            Message = message,
            MessageId = properties.MessageId,
            CorrelationId = properties.CorrelationId,
            PublishedAt = DateTime.UtcNow
        };

    private static IBasicProperties CreateProperties(IModel channel, ProducerArgs innerArgs)
        => channel.CreateBasicProperties()
                  .FillWith(innerArgs);
}