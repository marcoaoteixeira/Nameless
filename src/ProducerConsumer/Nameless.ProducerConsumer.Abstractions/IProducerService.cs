namespace Nameless.ProducerConsumer {
    /// <summary>
    /// Defines what a producer should do.
    /// </summary>
    public interface IProducerService {
        #region Methods

        /// <summary>
        /// Produces a message for a specific topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        Task ProduceAsync(string topic, object message, ProducerArgs? args = null, CancellationToken cancellationToken = default);

        #endregion Methods
    }
}