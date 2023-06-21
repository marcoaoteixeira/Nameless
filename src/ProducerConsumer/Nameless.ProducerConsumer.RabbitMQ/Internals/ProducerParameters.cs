using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    internal class ProducerParameters {
        #region Internal Properties

        internal string ExchangeName { get; }
        internal string? AppId { get; }
        internal string? ClusterId { get; }
        internal string? ContentEncoding { get; }
        internal string? ContentType { get; }
        internal string? CorrelationId { get; }
        internal byte DeliveryMode { get; }
        internal string? Expiration { get; }
        internal IDictionary<string, object> Headers { get; }
        internal string? MessageId { get; }
        internal bool Persistent { get; }
        internal byte Priority { get; }
        internal string? ReplyTo { get; }
        internal PublicationAddress? ReplyToAddress { get; }
        internal AmqpTimestamp Timestamp { get; }
        internal string? Type { get; }
        internal string? UserId { get; }

        #endregion

        #region Internal Constructors

        internal ProducerParameters(IEnumerable<Parameter> parameters) {
            var dictionary = ParameterHelper.ToDictionary(parameters);

            ExchangeName = dictionary.Get(nameof(ExchangeName), Constants.DEFAULT_EXCHANGE_NAME);
            AppId = dictionary.Get<string?>(nameof(AppId));
            ClusterId = dictionary.Get<string?>(nameof(ClusterId));
            ContentEncoding = dictionary.Get<string?>(nameof(ContentEncoding));
            ContentType = dictionary.Get<string?>(nameof(ContentType));
            CorrelationId = dictionary.Get<string?>(nameof(CorrelationId));
            DeliveryMode = dictionary.Get<byte>(nameof(DeliveryMode));
            Expiration = dictionary.Get<string?>(nameof(Expiration));
            Headers = dictionary.Get(nameof(Headers), new Dictionary<string, object>());
            MessageId = dictionary.Get<string?>(nameof(MessageId));
            Persistent = dictionary.Get<bool>(nameof(Persistent));
            Priority = dictionary.Get<byte>(nameof(Priority));
            ReplyTo = dictionary.Get<string?>(nameof(ReplyTo));

            var replyToAddress = dictionary.Get<PublicationAddress?>(nameof(ReplyToAddress));
            if (replyToAddress != null) { ReplyToAddress = replyToAddress; }

            Timestamp = dictionary.Get<AmqpTimestamp>(nameof(Timestamp));
            Type = dictionary.Get<string?>(nameof(Type));
            UserId = dictionary.Get<string?>(nameof(UserId));
        }

        #endregion
    }
}
