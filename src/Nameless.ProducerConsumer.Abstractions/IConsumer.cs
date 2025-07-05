namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a consumer.
/// </summary>
/// <typeparam name="TMessage">Type of the message.</typeparam>
public interface IConsumer<out TMessage> : IDisposable, IAsyncDisposable
    where TMessage : notnull {
    /// <summary>
    ///     Gets the topic that this consumer is associated with.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Starts a consumer.
    /// </summary>
    /// <param name="handler">The message handler.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    Task StartAsync(MessageHandlerDelegate<TMessage> handler, Parameters parameters, CancellationToken cancellationToken);
}