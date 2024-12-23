namespace Nameless.PubSub;

/// <summary>
/// Delegate to a message handler.
/// </summary>
/// <param name="message">The message.</param>
/// <param name="cancellationToken">The cancellation token.</param>
/// <returns>A <see cref="Task"/> representing the message handling action.</returns>
public delegate Task MessageHandlerAsync(object message,
                                         CancellationToken cancellationToken);