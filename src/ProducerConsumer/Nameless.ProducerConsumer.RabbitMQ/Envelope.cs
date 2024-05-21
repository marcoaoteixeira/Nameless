namespace Nameless.ProducerConsumer.RabbitMQ {
    /// <summary>
    /// Represents the envelope that will hold the data when
    /// sending a message for the RabbitMQ broker.
    /// </summary>
    public sealed record Envelope(object Message, string MessageId, string CorrelationId, DateTime PublishedAt) {
        #region Public Properties

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public object Message { get; } = Message;
        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public string MessageId { get; } = MessageId;
        /// <summary>
        /// Gets or sets the message correlation id.
        /// </summary>
        public string CorrelationId { get; } = CorrelationId;

        /// <summary>
        /// Gets or sets the message published date.
        /// </summary>
        public DateTime PublishedAt { get; } = PublishedAt;

        #endregion
    }
}
