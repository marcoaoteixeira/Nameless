namespace Nameless.ProducerConsumer.RabbitMQ.Options;

/// <summary>
///     The consumer options.
/// </summary>
public record ConsumerOptions {
    /// <summary>
    ///     Gets or sets the name of the consumer.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the queue the consumer will be bound to.
    /// </summary>
    public string Queue { get; init; } = Constants.Queues.Default;

    /// <summary>
    ///     Gets or sets the type of the message.
    /// </summary>
    /// <remarks>
    ///     Must be a .NET full qualified user type name.
    /// </remarks>
    public string? MessageType { get; init; }

    /// <summary>
    ///     Gets or sets the retry policy for the queue.
    /// </summary>
    public RetryPolicyOptions? RetryPolicy { get; init; }
}
