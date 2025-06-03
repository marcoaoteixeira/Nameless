using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;
public static class ArgsExtensions {
    // Producer args
    public static string GetExchangeName(this Args self) {
        return self[ProducerArgsKeys.EXCHANGE_NAME] as string ?? Internals.Defaults.EXCHANGE_NAME;
    }

    public static void SetExchangeName(this Args self, string? value) {
        self[ProducerArgsKeys.EXCHANGE_NAME] = value;
    }

    public static bool HasRoutingKeys(this Args self) {
        return self.GetRoutingKeys().Length > 0;
    }

    public static string[] GetRoutingKeys(this Args self) {
        return self[ProducerArgsKeys.ROUTING_KEYS] as string[] ?? [string.Empty];
    }

    public static void SetRoutingKeys(this Args self, string[]? value) {
        self[ProducerArgsKeys.ROUTING_KEYS] = value;
    }

    public static bool GetMandatory(this Args self) {
        return self[ProducerArgsKeys.MANDATORY] is true;
    }

    public static void SetMandatory(this Args self, bool? value) {
        self[ProducerArgsKeys.MANDATORY] = value;
    }

    public static string? GetAppId(this Args self) {
        return self[nameof(IBasicProperties.AppId)] as string;
    }

    public static void SetAppId(this Args self, string? value) {
        self[nameof(IBasicProperties.AppId)] = value;
    }

    public static string? GetClusterId(this Args self) {
        return self[nameof(IBasicProperties.ClusterId)] as string;
    }

    public static void SetClusterId(this Args self, string? value) {
        self[nameof(IBasicProperties.ClusterId)] = value;
    }

    public static string? GetContentEncoding(this Args self) {
        return self[nameof(IBasicProperties.ContentEncoding)] as string;
    }

    public static void SetContentEncoding(this Args self, string? value) {
        self[nameof(IBasicProperties.ContentEncoding)] = value;
    }

    public static string? GetContentType(this Args self) {
        return self[nameof(IBasicProperties.ContentType)] as string;
    }

    public static void SetContentType(this Args self, string? value) {
        self[nameof(IBasicProperties.ContentType)] = value;
    }

    public static string? GetCorrelationId(this Args self) {
        return self[nameof(IBasicProperties.CorrelationId)] as string;
    }

    public static void SetCorrelationId(this Args self, string? value) {
        self[nameof(IBasicProperties.CorrelationId)] = value;
    }

    public static DeliveryModes GetDeliveryMode(this Args self) {
        return self[nameof(IBasicProperties.DeliveryMode)] is DeliveryModes deliveryModes
            ? deliveryModes
            : DeliveryModes.Transient;
    }

    public static void SetDeliveryMode(this Args self, byte? value) {
        self[nameof(IBasicProperties.DeliveryMode)] = value;
    }

    public static string? GetExpiration(this Args self) {
        return self[nameof(IBasicProperties.Expiration)] as string;
    }

    public static void SetExpiration(this Args self, string? value) {
        self[nameof(IBasicProperties.Expiration)] = value;
    }

    public static IDictionary<string, object?> GetHeaders(this Args self) {
        return self[nameof(IBasicProperties.Headers)] as Dictionary<string, object?> ?? [];
    }

    public static void SetHeaders(this Args self, IDictionary<string, object>? value) {
        self[nameof(IBasicProperties.Headers)] = value;
    }

    public static string? GetMessageId(this Args self) {
        return self[nameof(IBasicProperties.MessageId)] as string;
    }

    public static void SetMessageId(this Args self, string? value) {
        self[nameof(IBasicProperties.MessageId)] = value;
    }

    public static bool GetPersistent(this Args self) {
        return self[nameof(IBasicProperties.Persistent)] is true;
    }

    public static void SetPersistent(this Args self, bool? value) {
        self[nameof(IBasicProperties.Persistent)] = value;
    }

    public static byte GetPriority(this Args self) {
        return self[nameof(IBasicProperties.Priority)] is byte value
            ? value
            : (byte)0;
    }

    public static void SetPriority(this Args self, byte? value) {
        self[nameof(IBasicProperties.Priority)] = value;
    }

    public static string? GetReplyTo(this Args self) {
        return self[nameof(IBasicProperties.ReplyTo)] as string;
    }

    public static void SetReplyTo(this Args self, string? value) {
        self[nameof(IBasicProperties.ReplyTo)] = value;
    }

    public static PublicationAddress? GetReplyToAddress(this Args self) {
        return self[nameof(IBasicProperties.ReplyToAddress)] as PublicationAddress;
    }

    public static void SetReplyToAddress(this Args self, PublicationAddress? value) {
        self[nameof(IBasicProperties.ReplyToAddress)] = value;
    }

    public static AmqpTimestamp GetTimestamp(this Args self) {
        return self[nameof(IBasicProperties.Timestamp)] is AmqpTimestamp value
            ? value
            : default;
    }

    public static void SetTimestamp(this Args self, AmqpTimestamp? value) {
        self[nameof(IBasicProperties.Timestamp)] = value;
    }

    public static string? GetTypeProp(this Args self) {
        return self[nameof(IBasicProperties.Type)] as string;
    }

    public static void SetTypeProp(this Args self, string? value) {
        self[nameof(IBasicProperties.Type)] = value;
    }

    public static string? GetUserId(this Args self) {
        return self[nameof(IBasicProperties.UserId)] as string;
    }

    public static void SetUserId(this Args self, string? value) {
        self[nameof(IBasicProperties.UserId)] = value;
    }

    // Consumer args
    public static string GetQueueName(this Args self) {
        return self[ConsumerArgsKeys.QUEUE_NAME] as string ?? Internals.Defaults.QUEUE_NAME;
    }

    public static void SetQueueName(this Args self, string? value) {
        self[ConsumerArgsKeys.QUEUE_NAME] = value;
    }

    public static bool GetAckOnSuccess(this Args self) {
        return self[ConsumerArgsKeys.ACK_ON_SUCCESS] is true;
    }

    public static void SetAckOnSuccess(this Args self, bool? value) {
        self[ConsumerArgsKeys.ACK_ON_SUCCESS] = value;
    }

    public static bool GetAckMultiple(this Args self) {
        return self[ConsumerArgsKeys.ACK_MULTIPLE] is true;
    }

    public static void SetAckMultiple(this Args self, bool? value) {
        self[ConsumerArgsKeys.ACK_MULTIPLE] = value;
    }

    public static bool GetNAckOnFailure(this Args self) {
        return self[ConsumerArgsKeys.NACK_ON_FAILURE] is true;
    }

    public static void SetNAckOnFailure(this Args self, bool? value) {
        self[ConsumerArgsKeys.NACK_ON_FAILURE] = value;
    }

    public static bool GetNAckMultiple(this Args self) {
        return self[ConsumerArgsKeys.NACK_MULTIPLE] is true;
    }

    public static void SetNAckMultiple(this Args self, bool? value) {
        self[ConsumerArgsKeys.NACK_MULTIPLE] = value;
    }

    public static bool GetAutoAck(this Args self) {
        return self[ConsumerArgsKeys.AUTO_ACK] is true;
    }

    public static void SetAutoAck(this Args self, bool? value) {
        self[ConsumerArgsKeys.AUTO_ACK] = value;
    }

    public static bool GetRequeueOnFailure(this Args self) {
        return self[ConsumerArgsKeys.REQUEUE_ON_FAILURE] is true;
    }

    public static void SetRequeueOnFailure(this Args self, bool? value) {
        self[ConsumerArgsKeys.REQUEUE_ON_FAILURE] = value;
    }
}
