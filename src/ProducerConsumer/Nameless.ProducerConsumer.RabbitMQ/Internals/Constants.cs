namespace Nameless.ProducerConsumer.RabbitMQ.Internals;

internal static class Constants {
    internal static class ConsumerArgsTokens {
        internal const string QUEUE_NAME = "QueueName";

        internal const string ACK_ON_SUCCESS = "AckOnSuccess";
        internal const string ACK_MULTIPLE = "AckMultiple";

        internal const string NACK_ON_FAILURE = "NAckOnFailure";
        internal const string NACK_MULTIPLE = "NAckMultiple";

        internal const string AUTO_ACK = "AutoAck";
        internal const string REQUEUE_ON_FAILURE = "RequeueOnFailure";
    }

    internal static class ProducerArgsTokens {
        internal const string EXCHANGE_NAME = "ExchangeName";
        internal const string ROUTING_KEYS = "RoutingKeys";
        internal const string MANDATORY = "Mandatory";
    }
}