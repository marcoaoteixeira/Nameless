using Nameless.PubSub.RabbitMQ.Internals;

namespace Nameless.PubSub.RabbitMQ;

public static class SubscriberArgsExtensions {
    public static string GetQueueName(this SubscriberArgs self) {
        return self[SubscriberArgsKeys.QUEUE_NAME] as string ?? Internals.Defaults.QueueName;
    }

    public static void SetQueueName(this SubscriberArgs self, string? value) {
        self[SubscriberArgsKeys.QUEUE_NAME] = value;
    }

    public static bool GetAckOnSuccess(this SubscriberArgs self) {
        return self[SubscriberArgsKeys.ACK_ON_SUCCESS] is true;
    }

    public static void SetAckOnSuccess(this SubscriberArgs self, bool? value) {
        self[SubscriberArgsKeys.ACK_ON_SUCCESS] = value;
    }

    public static bool GetAckMultiple(this SubscriberArgs self) {
        return self[SubscriberArgsKeys.ACK_MULTIPLE] is true;
    }

    public static void SetAckMultiple(this SubscriberArgs self, bool? value) {
        self[SubscriberArgsKeys.ACK_MULTIPLE] = value;
    }

    public static bool GetNAckOnFailure(this SubscriberArgs self) {
        return self[SubscriberArgsKeys.NACK_ON_FAILURE] is true;
    }

    public static void SetNAckOnFailure(this SubscriberArgs self, bool? value) {
        self[SubscriberArgsKeys.NACK_ON_FAILURE] = value;
    }

    public static bool GetNAckMultiple(this SubscriberArgs self) {
        return self[SubscriberArgsKeys.NACK_MULTIPLE] is true;
    }

    public static void SetNAckMultiple(this SubscriberArgs self, bool? value) {
        self[SubscriberArgsKeys.NACK_MULTIPLE] = value;
    }

    public static bool GetAutoAck(this SubscriberArgs self) {
        return self[SubscriberArgsKeys.AUTO_ACK] is true;
    }

    public static void SetAutoAck(this SubscriberArgs self, bool? value) {
        self[SubscriberArgsKeys.AUTO_ACK] = value;
    }

    public static bool GetRequeueOnFailure(this SubscriberArgs self) {
        return self[SubscriberArgsKeys.REQUEUE_ON_FAILURE] is true;
    }

    public static void SetRequeueOnFailure(this SubscriberArgs self, bool? value) {
        self[SubscriberArgsKeys.REQUEUE_ON_FAILURE] = value;
    }
}