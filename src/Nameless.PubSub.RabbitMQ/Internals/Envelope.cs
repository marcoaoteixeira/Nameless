using System.Text.Json;

namespace Nameless.PubSub.RabbitMQ.Internals;

/// <summary>
///     Represents the envelope that will hold the data when
///     sending a message for the RabbitMQ broker.
/// </summary>
/// <param name="Message">The message.</param>
/// <param name="MessageId">The message identification.</param>
/// <param name="CorrelationId">The correlation identification.</param>
/// <param name="PublishedAt">The publishing date.</param>
internal sealed record Envelope(
    object Message,
    string? MessageId,
    string? CorrelationId,
    DateTimeOffset PublishedAt) {
    /// <summary>
    /// Retrieves a buffer from the envelope using JSON serialization.
    /// </summary>
    /// <returns>
    /// A <see cref="ReadOnlyMemory{T}"/> containing the serialized envelope data.
    /// </returns>
    internal ReadOnlyMemory<byte> GetBuffer() {
        return JsonSerializer.SerializeToUtf8Bytes(this);
    }
}