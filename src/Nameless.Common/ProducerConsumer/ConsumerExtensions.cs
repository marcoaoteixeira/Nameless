namespace Nameless.ProducerConsumer;

/// <summary>
///     <see cref="IConsumer{TMessage}"/> extension methods.
/// </summary>
public static class ConsumerExtensions {
    /// <typeparam name="TMessage">
    ///     Type of the message.
    /// </typeparam>
    /// <param name="self">
    ///     The current <see cref="IConsumer{TMessage}"/> instance.
    /// </param>
    extension<TMessage>(IConsumer<TMessage> self) {
        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous execution.
        /// </returns>
        public Task HandleAsync(TMessage message, CancellationToken cancellationToken) {
            return self.ConsumeAsync(message, context: [], cancellationToken);
        }
    }
}