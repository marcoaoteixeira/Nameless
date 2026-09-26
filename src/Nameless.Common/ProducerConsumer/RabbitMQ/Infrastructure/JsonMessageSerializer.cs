using System.Text.Json;
using Nameless.ProducerConsumer.RabbitMQ.ObjectModel;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
///     <see cref="IMessageSerializer"/> implementation for JSON messages.
/// </summary>
public class JsonMessageSerializer : IMessageSerializer {
    /// <inheritdoc />
    public byte[] Serialize<T>(T value, Context context) {
        var message = new Message<T> {
            Header = new Header {
                CorrelationID = context.CorrelationId,
                MessageID = context.MessageId,
                Timestamp = context.Timestamp.UnixTime
            },
            
            Content = value,
        };

        return JsonSerializer.SerializeToUtf8Bytes(message);
    }

    /// <inheritdoc />
    public Message<T> Deserialize<T>(byte[] buffer) {
        var message = JsonSerializer.Deserialize<Message<T>>(buffer);

        return message ?? throw new InvalidOperationException(
            "Unable to deserialize buffer."
        );
    }
}