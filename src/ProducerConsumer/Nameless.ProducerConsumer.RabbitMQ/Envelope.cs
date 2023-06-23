using System.Text;
using System.Text.Json;

namespace Nameless.ProducerConsumer.RabbitMQ {

    /// <summary>
    /// Represents the envelope that will hold the data when
    /// sending a message for the RabbitMQ broker.
    /// </summary>
    public sealed class Envelope {

        #region Public Properties

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public object Message { get; set; }
        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// Gets or sets the message correlation id.
        /// </summary>
        public string CorrelationId { get; set; }
        /// <summary>
        /// Gets or sets the message published date.
        /// </summary>
        public DateTime PublishedAt { get; set; }

        #endregion

        #region Public Constructors

        public Envelope() : this(message: string.Empty) { }

        /// <summary>
        /// Initializes a new instance of <see cref="Envelope"/>.
        /// </summary>
        public Envelope(object message, string? messageId = default, string? correlationId = default, DateTime publishedAt = default) {
            Prevent.Null(message, nameof(message));

            Message = message;
            MessageId = messageId ?? string.Empty;
            CorrelationId = correlationId ?? string.Empty;
            PublishedAt = publishedAt;
        }

        #endregion

        #region Public Methods

        public byte[] CreateBuffer() {
            var json = JsonSerializer.Serialize(this);

            return Encoding.UTF8.GetBytes(json);
        }

        #endregion
    }
}
