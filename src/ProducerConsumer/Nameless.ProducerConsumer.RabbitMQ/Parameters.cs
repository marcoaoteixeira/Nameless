using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ {
    public static class Parameters {
        #region Public Static Inner Classes

        public static class Consumer {
            #region Public Constatns

            /// <summary>
            /// AckOnSuccess is <see cref="bool"/>.
            /// </summary>
            public const string AckOnSuccess = nameof(AckOnSuccess);
            /// <summary>
            /// AckMultiple is <see cref="bool"/>.
            /// </summary>
            public const string AckMultiple = nameof(AckMultiple);
            /// <summary>
            /// NAckOnFailure is <see cref="bool"/>.
            /// </summary>
            public const string NAckOnFailure = nameof(NAckOnFailure);
            /// <summary>
            /// NAckMultiple is <see cref="bool"/>.
            /// </summary>
            public const string NAckMultiple = nameof(NAckMultiple);
            /// <summary>
            /// RequeueOnFailure is <see cref="bool"/>.
            /// </summary>
            public const string RequeueOnFailure = nameof(RequeueOnFailure);
            /// <summary>
            /// AutoAck is <see cref="bool"/>.
            /// </summary>
            public const string AutoAck = nameof(AutoAck);

            #endregion
        }

        public static class Producer {
            #region Public Constants

            /// <summary>
            /// <see cref="ExchangeName"/> is <see cref="string"/>.
            /// </summary>
            public const string ExchangeName = nameof(ExchangeName);
            /// <summary>
            /// <see cref="AppId"/> is <see cref="string"/>.
            /// </summary>
            public const string AppId = nameof(IBasicProperties.AppId);
            /// <summary>
            /// <see cref="ClusterId"/> is <see cref="string"/>.
            /// </summary>
            public const string ClusterId = nameof(IBasicProperties.ClusterId);
            /// <summary>
            /// <see cref="ContentEncoding"/> is <see cref="string"/>.
            /// </summary>
            public const string ContentEncoding = nameof(IBasicProperties.ContentEncoding);
            /// <summary>
            /// <see cref="ContentType"/> is <see cref="string"/>.
            /// </summary>
            public const string ContentType = nameof(IBasicProperties.ContentType);
            /// <summary>
            /// <see cref="CorrelationId"/> is <see cref="string"/>.
            /// </summary>
            public const string CorrelationId = nameof(IBasicProperties.CorrelationId);
            /// <summary>
            /// <see cref="DeliveryMode"/> is <see cref="byte"/>.
            /// </summary>
            public const string DeliveryMode = nameof(IBasicProperties.DeliveryMode);
            /// <summary>
            /// <see cref="Expiration"/> is <see cref="string"/>.
            /// </summary>
            public const string Expiration = nameof(IBasicProperties.Expiration);
            /// <summary>
            /// <see cref="Headers"/> is <see cref="IDictionary{String, Object}"/>.
            /// </summary>
            public const string Headers = nameof(IBasicProperties.Headers);
            /// <summary>
            /// <see cref="MessageId"/> is <see cref="string"/>.
            /// </summary>
            public const string MessageId = nameof(IBasicProperties.MessageId);
            /// <summary>
            /// <see cref="Persistent"/> is <see cref="bool"/>.
            /// </summary>
            public const string Persistent = nameof(IBasicProperties.Persistent);
            /// <summary>
            /// <see cref="Priority"/> is <see cref="byte"/>.
            /// </summary>
            public const string Priority = nameof(IBasicProperties.Priority);
            /// <summary>
            /// <see cref="ReplyTo"/> is <see cref="string"/>.
            /// </summary>
            public const string ReplyTo = nameof(IBasicProperties.ReplyTo);
            /// <summary>
            /// <see cref="ReplyToAddress"/> is <see cref="PublicationAddress"/>.
            /// </summary>
            public const string ReplyToAddress = nameof(IBasicProperties.ReplyToAddress);
            /// <summary>
            /// <see cref="Timestamp"/> is <see cref="AmqpTimestamp"/>.
            /// </summary>
            public const string Timestamp = nameof(IBasicProperties.Timestamp);
            /// <summary>
            /// <see cref="Type"/> is <see cref="string"/>.
            /// </summary>
            public const string Type = nameof(IBasicProperties.Type);
            /// <summary>
            /// <see cref="UserId"/> is <see cref="string"/>.
            /// </summary>
            public const string UserId = nameof(IBasicProperties.UserId);

            #endregion
        }

        #endregion
    }
}
