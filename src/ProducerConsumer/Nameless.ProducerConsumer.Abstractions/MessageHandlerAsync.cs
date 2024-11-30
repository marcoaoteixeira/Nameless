namespace Nameless.ProducerConsumer;

/// <summary>
/// A delegate to a message handler.
/// </summary>
/// <typeparam name="TMessage">The message type.</typeparam>
/// <param name="message">The message.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>A <see cref="Task"/> representing the message handling action.</returns>
public delegate Task MessageHandlerAsync<in TMessage>(TMessage message, CancellationToken cancellationToken);