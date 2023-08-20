namespace Nameless.ProducerConsumer.RabbitMQ {
    public static class Root {
        #region Public Static Inner Classes

        public static class EnvTokens {
            #region Public Constants

            public const string RABBITMQ_USER = nameof(RABBITMQ_USER);
            public const string RABBITMQ_PASS = nameof(RABBITMQ_PASS);

            #endregion
        }

        public static class ConsumerArgsTokens {
            #region Public Constants

            public const string ACK_ON_SUCCESS = "AckOnSuccess";
            public const string ACK_MULTIPLE = "AckMultiple";

            public const string NACK_ON_FAILURE = "NAckOnFailure";
            public const string NACK_MULTIPLE = "NAckMultiple";

            public const string AUTO_ACK = "AutoAck";
            public const string REQUEUE_ON_FAILURE = "RequeueOnFailure";

            #endregion
        }

        public static class ProducerArgsTokens {
            #region Public Constants

            public const string EXCHANGE_NAME = "ExchangeName";

            #endregion
        }

        #endregion

        #region Internal Static Inner Classes

        internal static class Defaults {
            #region Internal Constants

            internal const string EXCHANGE_NAME = "";
            internal const string QUEUE_NAME = "default";
            internal const string RABBITMQ_USER = "guest";
            internal const string RABBITMQ_PASS = "guest";

            #endregion
        }

        #endregion
    }
}
