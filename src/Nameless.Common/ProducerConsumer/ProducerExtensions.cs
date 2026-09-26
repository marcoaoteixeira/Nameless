namespace Nameless.ProducerConsumer;

/// <summary>
///     <see cref="IProducer"/> extension methods.
/// </summary>
public static class ProducerExtensions {
    /// <param name="self">
    ///     The current <see cref="IProducer"/> instance.
    /// </param>
    extension(IProducer self) {
        /// <summary>
        ///     Produces a message for the specified topic.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous execution.
        /// </returns>
        public Task ProduceAsync(string topic, object message, CancellationToken cancellationToken) {
            return self.ProduceAsync(topic, message, context: [], cancellationToken);
        }
    }
}