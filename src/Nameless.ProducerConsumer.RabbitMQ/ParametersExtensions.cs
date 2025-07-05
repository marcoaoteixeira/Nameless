using Nameless.ProducerConsumer.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ParametersExtensions {
    #region Basic Properties Parameters

    public static string? GetAppId(this Parameters self) {
        return self[nameof(IBasicProperties.AppId)] as string;
    }

    public static void SetAppId(this Parameters self, string? value) {
        self[nameof(IBasicProperties.AppId)] = value;
    }

    public static string? GetClusterId(this Parameters self) {
        return self[nameof(IBasicProperties.ClusterId)] as string;
    }

    public static void SetClusterId(this Parameters self, string? value) {
        self[nameof(IBasicProperties.ClusterId)] = value;
    }

    public static string? GetContentEncoding(this Parameters self) {
        return self[nameof(IBasicProperties.ContentEncoding)] as string;
    }

    public static void SetContentEncoding(this Parameters self, string? value) {
        self[nameof(IBasicProperties.ContentEncoding)] = value;
    }

    public static string? GetContentType(this Parameters self) {
        return self[nameof(IBasicProperties.ContentType)] as string;
    }

    public static void SetContentType(this Parameters self, string? value) {
        self[nameof(IBasicProperties.ContentType)] = value;
    }

    public static string? GetCorrelationId(this Parameters self) {
        return self[nameof(IBasicProperties.CorrelationId)] as string;
    }

    public static void SetCorrelationId(this Parameters self, string? value) {
        self[nameof(IBasicProperties.CorrelationId)] = value;
    }

    public static DeliveryModes GetDeliveryMode(this Parameters self) {
        return self[nameof(IBasicProperties.DeliveryMode)] is DeliveryModes modes ? modes : default;
    }

    public static void SetDeliveryMode(this Parameters self, byte? value) {
        self[nameof(IBasicProperties.DeliveryMode)] = value;
    }

    public static string? GetExpiration(this Parameters self) {
        return self[nameof(IBasicProperties.Expiration)] as string;
    }

    public static void SetExpiration(this Parameters self, string? value) {
        self[nameof(IBasicProperties.Expiration)] = value;
    }

    public static IDictionary<string, object?> GetHeaders(this Parameters self) {
        return self[nameof(IBasicProperties.Headers)] as Dictionary<string, object?> ?? [];
    }

    public static void SetHeaders(this Parameters self, IDictionary<string, object>? value) {
        self[nameof(IBasicProperties.Headers)] = value;
    }

    public static string? GetMessageId(this Parameters self) {
        return self[nameof(IBasicProperties.MessageId)] as string;
    }

    public static void SetMessageId(this Parameters self, string? value) {
        self[nameof(IBasicProperties.MessageId)] = value;
    }

    public static bool GetPersistent(this Parameters self) {
        return self[nameof(IBasicProperties.Persistent)] is true;
    }

    public static void SetPersistent(this Parameters self, bool? value) {
        self[nameof(IBasicProperties.Persistent)] = value;
    }

    public static byte GetPriority(this Parameters self) {
        return self[nameof(IBasicProperties.Priority)] is byte value ? value : (byte)0;
    }

    public static void SetPriority(this Parameters self, byte? value) {
        self[nameof(IBasicProperties.Priority)] = value;
    }

    public static string? GetReplyTo(this Parameters self) {
        return self[nameof(IBasicProperties.ReplyTo)] as string;
    }

    public static void SetReplyTo(this Parameters self, string? value) {
        self[nameof(IBasicProperties.ReplyTo)] = value;
    }

    public static PublicationAddress? GetReplyToAddress(this Parameters self) {
        return self[nameof(IBasicProperties.ReplyToAddress)] as PublicationAddress;
    }

    public static void SetReplyToAddress(this Parameters self, PublicationAddress? value) {
        self[nameof(IBasicProperties.ReplyToAddress)] = value;
    }

    public static AmqpTimestamp GetTimestamp(this Parameters self) {
        return self[nameof(IBasicProperties.Timestamp)] is AmqpTimestamp value ? value : default;
    }

    public static void SetTimestamp(this Parameters self, AmqpTimestamp? value) {
        self[nameof(IBasicProperties.Timestamp)] = value;
    }

    public static string? GetTypeProp(this Parameters self) {
        return self[nameof(IBasicProperties.Type)] as string;
    }

    public static void SetTypeProp(this Parameters self, string? value) {
        self[nameof(IBasicProperties.Type)] = value;
    }

    public static string? GetUserId(this Parameters self) {
        return self[nameof(IBasicProperties.UserId)] as string;
    }

    public static void SetUserId(this Parameters self, string? value) {
        self[nameof(IBasicProperties.UserId)] = value;
    }

    #endregion

    #region Producer Parameters

    public static string GetExchangeName(this Parameters self) {
        return self[ProducerParametersNames.EXCHANGE_NAME] as string ?? Internals.Defaults.EXCHANGE_NAME;
    }

    public static void SetExchangeName(this Parameters self, string? value) {
        self[ProducerParametersNames.EXCHANGE_NAME] = value;
    }

    public static bool HasRoutingKeys(this Parameters self) {
        return self.GetRoutingKeys().Length > 0;
    }

    public static string[] GetRoutingKeys(this Parameters self) {
        return self[ProducerParametersNames.ROUTING_KEYS] as string[] ?? [];
    }

    public static void SetRoutingKeys(this Parameters self, string[]? value) {
        self[ProducerParametersNames.ROUTING_KEYS] = value;
    }

    public static bool GetMandatory(this Parameters self) {
        return self[ProducerParametersNames.MANDATORY] is true;
    }

    public static void SetMandatory(this Parameters self, bool? value) {
        self[ProducerParametersNames.MANDATORY] = value;
    }

    public static bool GetUsePrefetch(this Parameters self) {
        return self[ProducerParametersNames.USE_PREFETCH] is true;
    }

    public static void SetUsePrefetch(this Parameters self, bool? value) {
        self[ProducerParametersNames.USE_PREFETCH] = value;
    }

    #endregion

    #region Consumer Parameters

    public static string GetQueueName(this Parameters self) {
        return self[ConsumerParameterNames.QUEUE_NAME] as string ?? Internals.Defaults.QUEUE_NAME;
    }

    public static void SetQueueName(this Parameters self, string? value) {
        self[ConsumerParameterNames.QUEUE_NAME] = value;
    }

    public static bool GetAckOnSuccess(this Parameters self) {
        return self[ConsumerParameterNames.ACK_ON_SUCCESS] is true;
    }

    public static void SetAckOnSuccess(this Parameters self, bool? value) {
        self[ConsumerParameterNames.ACK_ON_SUCCESS] = value;
    }

    public static bool GetAckMultiple(this Parameters self) {
        return self[ConsumerParameterNames.ACK_MULTIPLE] is true;
    }

    public static void SetAckMultiple(this Parameters self, bool? value) {
        self[ConsumerParameterNames.ACK_MULTIPLE] = value;
    }

    public static bool GetNAckOnFailure(this Parameters self) {
        return self[ConsumerParameterNames.NACK_ON_FAILURE] is true;
    }

    public static void SetNAckOnFailure(this Parameters self, bool? value) {
        self[ConsumerParameterNames.NACK_ON_FAILURE] = value;
    }

    public static bool GetNAckMultiple(this Parameters self) {
        return self[ConsumerParameterNames.NACK_MULTIPLE] is true;
    }

    public static void SetNAckMultiple(this Parameters self, bool? value) {
        self[ConsumerParameterNames.NACK_MULTIPLE] = value;
    }

    public static bool GetAutoAck(this Parameters self) {
        return self[ConsumerParameterNames.AUTO_ACK] is true;
    }

    public static void SetAutoAck(this Parameters self, bool? value) {
        self[ConsumerParameterNames.AUTO_ACK] = value;
    }

    public static bool GetRequeueOnFailure(this Parameters self) {
        return self[ConsumerParameterNames.REQUEUE_ON_FAILURE] is true;
    }

    public static void SetRequeueOnFailure(this Parameters self, bool? value) {
        self[ConsumerParameterNames.REQUEUE_ON_FAILURE] = value;
    }

    #endregion
}
