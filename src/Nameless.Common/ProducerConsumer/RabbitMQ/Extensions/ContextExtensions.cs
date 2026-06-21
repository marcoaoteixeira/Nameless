using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     Extension properties for <see cref="Context"/> that expose standard RabbitMQ
///     <see cref="IBasicProperties"/> values stored in the context dictionary.
/// </summary>
public static class ContextExtensions {
    /// <param name="self">The current <see cref="Context"/> instance.</param>
    extension(Context self) {
        /// <summary>
        ///     Gets or sets the application ID of the message publisher.
        /// </summary>
        public string? AppId {
            get => self[nameof(IBasicProperties.AppId)] as string;
            set => self[nameof(IBasicProperties.AppId)] = value;
        }

        /// <summary>Gets or sets the cluster ID.</summary>
        public string? ClusterId {
            get => self[nameof(IBasicProperties.ClusterId)] as string;
            set => self[nameof(IBasicProperties.ClusterId)] = value;
        }

        /// <summary>Gets or sets the MIME content encoding of the message body.</summary>
        public string? ContentEncoding {
            get => self[nameof(IBasicProperties.ContentEncoding)] as string;
            set => self[nameof(IBasicProperties.ContentEncoding)] = value;
        }

        /// <summary>Gets or sets the MIME content type of the message body.</summary>
        public string? ContentType {
            get => self[nameof(IBasicProperties.ContentType)] as string;
            set => self[nameof(IBasicProperties.ContentType)] = value;
        }

        /// <summary>Gets or sets the correlation identifier used to correlate RPC responses.</summary>
        public string? CorrelationId {
            get => self[nameof(IBasicProperties.CorrelationId)] as string;
            set => self[nameof(IBasicProperties.CorrelationId)] = value;
        }

        /// <summary>Gets or sets the delivery mode (transient or persistent).</summary>
        public DeliveryModes DeliveryMode {
            get => self[nameof(IBasicProperties.DeliveryMode)] is DeliveryModes modes ? modes : default;
            set => self[nameof(IBasicProperties.DeliveryMode)] = value;
        }

        /// <summary>Gets or sets the message expiration specification.</summary>
        public string? Expiration {
            get => self[nameof(IBasicProperties.Expiration)] as string;
            set => self[nameof(IBasicProperties.Expiration)] = value;
        }

        /// <summary>Gets or sets the custom headers attached to the message.</summary>
        public IDictionary<string, object?> Headers {
            get => self[nameof(IBasicProperties.Headers)] as Dictionary<string, object?> ?? [];
            set => self[nameof(IBasicProperties.Headers)] = value;
        }

        /// <summary>Gets or sets the application-level message identifier.</summary>
        public string? MessageId {
            get => self[nameof(IBasicProperties.MessageId)] as string;
            set => self[nameof(IBasicProperties.MessageId)] = value;
        }

        /// <summary>Gets or sets a value indicating whether the message should be persisted to disk.</summary>
        public bool Persistent {
            get => self[nameof(IBasicProperties.Persistent)] is true;
            set => self[nameof(IBasicProperties.Persistent)] = value;
        }

        /// <summary>Gets or sets the message priority (0–9).</summary>
        public byte Priority {
            get => self[nameof(IBasicProperties.Priority)] is byte value ? value : (byte)0;
            set => self[nameof(IBasicProperties.Priority)] = value;
        }

        /// <summary>Gets or sets the address to which replies should be sent.</summary>
        public string? ReplyTo {
            get => self[nameof(IBasicProperties.ReplyTo)] as string;
            set => self[nameof(IBasicProperties.ReplyTo)] = value;
        }

        /// <summary>Gets or sets the structured reply-to address.</summary>
        public PublicationAddress? ReplyToAddress {
            get => self[nameof(IBasicProperties.ReplyToAddress)] as PublicationAddress;
            set => self[nameof(IBasicProperties.ReplyToAddress)] = value;
        }

        /// <summary>Gets or sets the message timestamp.</summary>
        public AmqpTimestamp Timestamp {
            get => self[nameof(IBasicProperties.Timestamp)] is AmqpTimestamp value ? value : default;
            set => self[nameof(IBasicProperties.Timestamp)] = value;
        }

        /// <summary>Gets or sets the message type name.</summary>
        public string? Type {
            get => self[nameof(IBasicProperties.Type)] as string;
            set => self[nameof(IBasicProperties.Type)] = value;
        }

        /// <summary>Gets or sets the user ID of the message publisher.</summary>
        public string? UserId {
            get => self[nameof(IBasicProperties.UserId)] as string;
            set => self[nameof(IBasicProperties.UserId)] = value;
        }
    }
}
