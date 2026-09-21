namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

/// <summary>
///     No Retry Exception
/// </summary>
public class NoRetryException : Exception {
    /// <summary>
    ///     Whether it should requeue the message.
    /// </summary>
    public bool Requeue { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="NoRetryException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="requeue">
    ///     Whether it should requeue the message.
    /// </param>
    public NoRetryException(string message, bool requeue = false)
        : base(message, innerException: null) { Requeue = requeue; }
}