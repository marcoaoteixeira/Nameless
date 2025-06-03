namespace Nameless.PubSub.RabbitMQ.Internals;

internal static class SubscriberArgsKeys {
    internal const string QUEUE_NAME = "QueueName";

    internal const string ACK_ON_SUCCESS = "AckOnSuccess";
    internal const string ACK_MULTIPLE = "AckMultiple";

    internal const string NACK_ON_FAILURE = "NAckOnFailure";
    internal const string NACK_MULTIPLE = "NAckMultiple";

    internal const string AUTO_ACK = "AutoAck";
    internal const string REQUEUE_ON_FAILURE = "RequeueOnFailure";
}