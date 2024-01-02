namespace Nameless.ProducerConsumer.RabbitMQ {
    /// <summary>
    /// Represents the envelope that will hold the data when
    /// sending a message for the RabbitMQ broker.
    /// </summary>
    public sealed record Envelope {
        #region Public Properties

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public object Message { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public string MessageId { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the message correlation id.
        /// </summary>
        public string CorrelationId { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the message published date.
        /// </summary>
        public DateTime PublishedAt { get; init; }

        #endregion
    }
}
