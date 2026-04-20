using Nameless.Resilience;

namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a consumer.
/// </summary>
/// <typeparam name="TMessage">
///     Type of the message this consumer can handle.
/// </typeparam>
public interface IConsumer<in TMessage> {
    /// <summary>
    ///     Gets the consumer name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the topic associated with the consumer.
    /// </summary>
    string Topic { get; }
    
    /// <summary>
    ///     Gets the retry policy.
    /// </summary>
    RetryPolicyConfiguration? RetryPolicy { get; }

    /// <summary>
    ///     Consumes the message.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    /// <param name="context">
    ///     The context.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous execution.
    /// </returns>
    Task ConsumeAsync(TMessage message, ConsumerContext context, CancellationToken cancellationToken);
}