namespace Nameless.ProducerConsumer;

/// <summary>
///     Interface for a consumer that can consume messages from a queue.
/// </summary>
public interface IConsumer : IDisposable, IAsyncDisposable {
    /// <summary>
    ///     Gets the topic that this consumer is associated with.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Starts a consumer that will listen to a queue using the specified
    ///     message handler and arguments.
    /// </summary>
    /// <param name="handler">The message handler.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous operation.
    ///     The result of the <see cref="Task{TResult}" /> is a string indicating
    ///     the consumer identifier.
    /// </returns>
    Task StartAsync(MessageHandlerDelegate handler, Args args, CancellationToken cancellationToken);

}