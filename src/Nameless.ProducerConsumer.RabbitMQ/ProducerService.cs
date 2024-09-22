using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public sealed class ProducerService : IProducerService {
    private readonly IModel _channel;
    private readonly ILogger _logger;

    public ProducerService(IModel channel, ILogger<ProducerService> logger) {
        _channel = Prevent.Argument.Null(channel);
        _logger = Prevent.Argument.Null(logger);
    }

    public Task ProduceAsync(string topic, object message, ProducerArgs? args = null, CancellationToken cancellationToken = default) {
        var innerArgs = args ?? ProducerArgs.Empty;
        var properties = CreateProperties(_channel, innerArgs);
        var envelope = CreateEnvelope(message, properties);

        try {
            var routingKeys = innerArgs.GetRoutingKeys()
                                       .Append(topic);
            var batch = _channel.CreateBasicPublishBatch();
            var buffer = envelope.CreateBuffer();

            foreach (var routingKey in routingKeys) {
                batch.Add(exchange: innerArgs.GetExchangeName(),
                          routingKey: routingKey,
                          mandatory: innerArgs.GetMandatory(),
                          properties: properties,
                          body: buffer);
            }

            batch.Publish();
        } catch (Exception ex) { _logger.MessagePublishError(ex); throw; }

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