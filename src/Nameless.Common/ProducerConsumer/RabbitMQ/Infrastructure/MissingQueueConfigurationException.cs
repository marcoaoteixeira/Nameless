namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
///     Missing Queue Configuration Exception
/// </summary>
public class MissingQueueConfigurationException : Exception {
    /// <summary>
    ///     Gets the queue's name.
    /// </summary>
    public string QueueName { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="MissingQueueConfigurationException"/> class.
    /// </summary>
    /// <param name="queueName">
    ///     The queue's name.
    /// </param>
    public MissingQueueConfigurationException(string queueName)
        : base($"Configuration for queue '{queueName}' is missing.", innerException: null) { QueueName = queueName; }
}
