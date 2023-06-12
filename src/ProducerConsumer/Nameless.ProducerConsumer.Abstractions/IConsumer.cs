namespace Nameless.ProducerConsumer {

    /// <summary>
    /// Defines what a consumer should do.
    /// </summary>
    public interface IConsumer {

        #region Methods

        /// <summary>
        /// Registers a callback to handle the message for a specific topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="callback">The callback handler.</param>
        /// <param name="arguments">The consumer arguments.</param>
        /// <returns>Returns the <see cref="Registration{T}"/> instance.</returns>
        Registration<T> Register<T>(string topic, Action<T> callback, Arguments arguments);

        /// <summary>
        /// Unregisters a consumer.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns><c>true</c> if unregistered; otherwise, <c>false</c>.</returns>
        bool Unregister<T>(Registration<T> registration);

        #endregion
    }
}