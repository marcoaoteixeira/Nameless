namespace Nameless.ProducerConsumer.RabbitMQ {

    /// <summary>
    /// Represents the envelope that will hold the data when
    /// sending a message for the RabbitMQ broker.
    /// </summary>
    public sealed class Message {

        #region Public Properties

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the message correlation id.
        /// </summary>
        public string CorrelationId { get; set; }
        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public object Payload { get; set; }
        /// <summary>
        /// Gets or sets the message published date.
        /// </summary>
        public DateTime PublishedAt { get; set; }

        #endregion

        #region Public Constructors

        public Message() : this(
            id: string.Empty,
            correlationId: string.Empty,
            payload: string.Empty,
            publishedAt: DateTime.MinValue
        ) { }

        /// <summary>
        /// Initializes a new instance of <see cref="Message"/>.
        /// </summary>
        public Message(string id, string correlationId, object payload, DateTime publishedAt) {
            Prevent.Null(id, nameof(id));
            Prevent.Null(correlationId, nameof(correlationId));
            Prevent.Null(payload, nameof(payload));

            Id = id;
            CorrelationId = correlationId;
            Payload = payload;
            PublishedAt = publishedAt;
        }

        #endregion
    }
}
