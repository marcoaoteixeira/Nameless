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
        /// <param name="message">The message.</param>
        /// <param name="parameters">The producer parameters.</param>
        void Produce(string topic, object message, params Parameter[] parameters);

        #endregion Methods
    }
}