using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ContextExtensions {
    extension(Context self) {
        public string? AppId {
            get => self[nameof(IBasicProperties.AppId)] as string;
            set => self[nameof(IBasicProperties.AppId)] = value;
        }

        public string? ClusterId {
            get => self[nameof(IBasicProperties.ClusterId)] as string;
            set => self[nameof(IBasicProperties.ClusterId)] = value;
        }

        public string? ContentEncoding {
            get => self[nameof(IBasicProperties.ContentEncoding)] as string;
            set => self[nameof(IBasicProperties.ContentEncoding)] = value;
        }

        public string? ContentType {
            get => self[nameof(IBasicProperties.ContentType)] as string;
            set => self[nameof(IBasicProperties.ContentType)] = value;
        }

        public string? CorrelationId {
            get => self[nameof(IBasicProperties.CorrelationId)] as string;
            set => self[nameof(IBasicProperties.CorrelationId)] = value;
        }

        public DeliveryModes DeliveryMode {
            get => self[nameof(IBasicProperties.DeliveryMode)] is DeliveryModes modes ? modes : default;
            set => self[nameof(IBasicProperties.DeliveryMode)] = value;
        }

        public string? Expiration {
            get => self[nameof(IBasicProperties.Expiration)] as string;
            set => self[nameof(IBasicProperties.Expiration)] = value;
        }

        public IDictionary<string, object?> Headers {
            get => self[nameof(IBasicProperties.Headers)] as Dictionary<string, object?> ?? [];
            set => self[nameof(IBasicProperties.Headers)] = value;
        }

        public string? MessageId {
            get => self[nameof(IBasicProperties.MessageId)] as string;
            set => self[nameof(IBasicProperties.MessageId)] = value;
        }

        public bool Persistent {
            get => self[nameof(IBasicProperties.Persistent)] is true;
            set => self[nameof(IBasicProperties.Persistent)] = value;
        }

        public byte Priority {
            get => self[nameof(IBasicProperties.Priority)] is byte value ? value : (byte)0;
            set => self[nameof(IBasicProperties.Priority)] = value;
        }

        public string? ReplyTo {
            get => self[nameof(IBasicProperties.ReplyTo)] as string;
            set => self[nameof(IBasicProperties.ReplyTo)] = value;
        }

        public PublicationAddress? ReplyToAddress {
            get => self[nameof(IBasicProperties.ReplyToAddress)] as PublicationAddress;
            set => self[nameof(IBasicProperties.ReplyToAddress)] = value;
        }

        public AmqpTimestamp Timestamp {
            get => self[nameof(IBasicProperties.Timestamp)] is AmqpTimestamp value ? value : default;
            set => self[nameof(IBasicProperties.Timestamp)] = value;
        }

        public string? Type {
            get => self[nameof(IBasicProperties.Type)] as string;
            set => self[nameof(IBasicProperties.Type)] = value;
        }

        public string? UserId {
            get => self[nameof(IBasicProperties.UserId)] as string;
            set => self[nameof(IBasicProperties.UserId)] = value;
        }
    }
}
