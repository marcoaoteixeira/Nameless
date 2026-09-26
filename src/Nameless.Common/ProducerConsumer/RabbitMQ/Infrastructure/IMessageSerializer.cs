using Nameless.ProducerConsumer.RabbitMQ.ObjectModel;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
///     Defines serialization and deserialization of RabbitMQ messages to and from a byte buffer.
/// </summary>
public interface IMessageSerializer {
    /// <summary>
    ///     Serializes the <paramref name="value"/> to a
    ///     <see cref="byte"/> array.
    /// </summary>
    /// <param name="value">
    ///     The message content.
    /// </param>
    /// <param name="context">
    ///     The context, provides message header and metadata.
    /// </param>
    /// <returns>
    ///     A byte array representing the serialized message.
    /// </returns>
    byte[] Serialize<T>(T value, Context context);

    /// <summary>
    ///     Deserializes the incoming <paramref name="buffer"/> into a
    ///     <see cref="Message{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">
    ///     The message content type.
    /// </typeparam>
    /// <param name="buffer">
    ///     The raw byte buffer to deserialize.
    /// </param>
    /// <returns>
    ///     The message instance.
    /// </returns>
    Message<T> Deserialize<T>(byte[] buffer);
}