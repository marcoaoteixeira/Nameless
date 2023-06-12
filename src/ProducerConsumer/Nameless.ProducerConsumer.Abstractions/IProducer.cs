namespace Nameless.ProducerConsumer {

    /// <summary>
    /// Defines what a producer should do.
    /// </summary>
    public interface IProducer {

        #region Methods

        /// <summary>
        /// Produces a message for a specific topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="arguments">The publisher arguments.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{String}"/> that represents the producer action.</returns>
        Task<string> ProduceAsync(string topic, object payload, Arguments arguments, CancellationToken cancellationToken = default);

        #endregion Methods
    }
}