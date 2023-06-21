namespace Nameless.ProducerConsumer {

    /// <summary>
    /// Defines what a consumer should do.
    /// </summary>
    public interface IConsumer {

        #region Methods

        /// <summary>
        /// Registers a handle to process a message for a specific topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="handler">The message event handler.</param>
        /// <param name="parameters">The consumer parameters.</param>
        /// <returns>Returns a <see cref="Registration{T}"/> representing the consumer registration.</returns>
        Registration<T> Register<T>(string topic, MessageEventHandler<T> handler, params Parameter[] parameters);

        /// <summary>
        /// Unregisters a consumer.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns><c>true</c> if unregistered; otherwise, <c>false</c>.</returns>
        bool Unregister<T>(Registration<T> registration);

        #endregion
    }
}