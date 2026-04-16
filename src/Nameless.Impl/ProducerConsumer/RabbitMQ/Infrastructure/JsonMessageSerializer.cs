using System.Text.Json;
using Nameless.ProducerConsumer.RabbitMQ.ObjectModel;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

public class JsonMessageSerializer : IMessageSerializer {
    public Task<byte[]> SerializeAsync(object message, Context context, CancellationToken cancellationToken) {
        var envelope = new Envelope {
            Header = new Header {
                CorrelationID = context.CorrelationId,
                MessageID = context.MessageId,
                Timestamp = context.Timestamp.UnixTime
            },
            
            Message = message,
        };
        
        var result = JsonSerializer.SerializeToUtf8Bytes(envelope);

        return Task.FromResult(result);
    }

    public Task<TMessage> DeserializeAsync<TMessage>(byte[] buffer, Context context, CancellationToken cancellationToken) {
        var envelope = JsonSerializer.Deserialize<Envelope>(buffer);

        if (envelope is null) {
            throw new InvalidOperationException("Unable to deserialize buffer.");
        }

        var message = ((JsonElement)envelope.Message).Deserialize<TMessage>();

        if (message is null) {
            throw new InvalidOperationException("Unable to deserialize message.");
        }

        context.MessageId = envelope.Header.MessageID;
        context.CorrelationId = envelope.Header.CorrelationID;
        context.Timestamp = new AmqpTimestamp(envelope.Header.Timestamp);

        return Task.FromResult(message);
    }
}