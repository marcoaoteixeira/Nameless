namespace Nameless.ProducerConsumer.RabbitMQ {
    /// <summary>
    /// This class was proposed to be a root point for this assembly.
    /// 
    /// *** DO NOT IMPLEMENT ANYTHING HERE ***
    /// 
    /// But, it's allowed to use it as a repository for all constants or
    /// default values that you'll use throughout this project.
    /// </summary>
    public static class Root {
        #region Public Static Inner Classes

        public static class ConsumerArgsTokens {
            #region Public Constants

            public const string QUEUE_NAME = "QueueName";

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
            public const string ROUTING_KEYS = "RoutingKeys";
            public const string MANDATORY = "Mandatory";

            #endregion
        }

        #endregion

        #region Internal Static Inner Classes

        internal static class Defaults {
            #region Internal Constants

            internal const string EXCHANGE_NAME = "";
            internal const string QUEUE_NAME = "q.default";
            internal const string RABBITMQ_USER = "guest";
            internal const string RABBITMQ_PASS = "guest";

            #endregion
        }

        #endregion
    }
}
