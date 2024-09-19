namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
/// The only purpose of this class is to be a "marker" for this
/// assembly.
/// 
/// *** DO NOT IMPLEMENT IMPORTANT THINGS HERE ***
/// 
/// But, it's OK to use it as a repository for all constants or default
/// values that you'll use throughout this assembly.
/// </summary>
public static class Root {
    public static class ConsumerArgsTokens {
        public const string QUEUE_NAME = "QueueName";

        public const string ACK_ON_SUCCESS = "AckOnSuccess";
        public const string ACK_MULTIPLE = "AckMultiple";

        public const string NACK_ON_FAILURE = "NAckOnFailure";
        public const string NACK_MULTIPLE = "NAckMultiple";

        public const string AUTO_ACK = "AutoAck";
        public const string REQUEUE_ON_FAILURE = "RequeueOnFailure";
    }

    public static class ProducerArgsTokens {
        public const string EXCHANGE_NAME = "ExchangeName";
        public const string ROUTING_KEYS = "RoutingKeys";
        public const string MANDATORY = "Mandatory";
    }

    internal static class Defaults {
        internal const string EXCHANGE_NAME = "";
        internal const string QUEUE_NAME = "q.default";
        internal const string RABBITMQ_USER = "guest";
        internal const string RABBITMQ_PASS = "guest";
    }
}