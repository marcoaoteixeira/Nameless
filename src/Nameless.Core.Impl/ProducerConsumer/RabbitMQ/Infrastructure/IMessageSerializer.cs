namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
///     Defines serialization and deserialization of RabbitMQ messages to and from a byte buffer.
/// </summary>
public interface IMessageSerializer {
    /// <summary>
    ///     Serializes the given <paramref name="message"/> to a byte array.
    /// </summary>
    /// <param name="message">The message object to serialize.</param>
    /// <param name="context">The messaging context providing headers and metadata.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A byte array representing the serialized message.</returns>
    Task<byte[]> SerializeAsync(object message, Context context, CancellationToken cancellationToken);

    /// <summary>
    ///     Deserializes a <typeparamref name="TMessage"/> from the given byte <paramref name="buffer"/>.
    /// </summary>
    /// <typeparam name="TMessage">The expected message type.</typeparam>
    /// <param name="buffer">The raw byte buffer to deserialize.</param>
    /// <param name="context">The messaging context to populate with envelope metadata.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The deserialized <typeparamref name="TMessage"/> instance.</returns>
    Task<TMessage> DeserializeAsync<TMessage>(byte[] buffer, Context context, CancellationToken cancellationToken);
}