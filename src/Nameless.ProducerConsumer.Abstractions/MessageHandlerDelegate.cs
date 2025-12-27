namespace Nameless.ProducerConsumer;

/// <summary>
///     Delegate to a message handler.
/// </summary>
/// <typeparam name="TMessage">Type of the message</typeparam>
/// <param name="message">The message.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
public delegate Task MessageHandlerDelegate<in TMessage>(TMessage message, CancellationToken cancellationToken)
    where TMessage : notnull;