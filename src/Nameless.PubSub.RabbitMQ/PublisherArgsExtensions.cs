using Nameless.PubSub.RabbitMQ.Internals;
using RabbitMQ.Client;

namespace Nameless.PubSub.RabbitMQ;

public static class PublisherArgsExtensions {
    public static string GetExchangeName(this PublisherArgs self) {
        return self[PublisherArgsKeys.EXCHANGE_NAME] as string ?? Internals.Defaults.ExchangeName;
    }

    public static void SetExchangeName(this PublisherArgs self, string? value) {
        self[PublisherArgsKeys.EXCHANGE_NAME] = value;
    }

    public static bool HasRoutingKeys(this PublisherArgs self) {
        return self.GetRoutingKeys().Length > 0;
    }

    public static string[] GetRoutingKeys(this PublisherArgs self) {
        return self[PublisherArgsKeys.ROUTING_KEYS] as string[] ?? [string.Empty];
    }

    public static void SetRoutingKeys(this PublisherArgs self, string[]? value) {
        self[PublisherArgsKeys.ROUTING_KEYS] = value;
    }

    public static bool GetMandatory(this PublisherArgs self) {
        return self[PublisherArgsKeys.MANDATORY] is true;
    }

    public static void SetMandatory(this PublisherArgs self, bool? value) {
        self[PublisherArgsKeys.MANDATORY] = value;
    }

    public static string? GetAppId(this PublisherArgs self) {
        return self[nameof(IBasicProperties.AppId)] as string;
    }

    public static void SetAppId(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.AppId)] = value;
    }

    public static string? GetClusterId(this PublisherArgs self) {
        return self[nameof(IBasicProperties.ClusterId)] as string;
    }

    public static void SetClusterId(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.ClusterId)] = value;
    }

    public static string? GetContentEncoding(this PublisherArgs self) {
        return self[nameof(IBasicProperties.ContentEncoding)] as string;
    }

    public static void SetContentEncoding(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.ContentEncoding)] = value;
    }

    public static string? GetContentType(this PublisherArgs self) {
        return self[nameof(IBasicProperties.ContentType)] as string;
    }

    public static void SetContentType(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.ContentType)] = value;
    }

    public static string? GetCorrelationId(this PublisherArgs self) {
        return self[nameof(IBasicProperties.CorrelationId)] as string;
    }

    public static void SetCorrelationId(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.CorrelationId)] = value;
    }

    public static DeliveryModes GetDeliveryMode(this PublisherArgs self) {
        return self[nameof(IBasicProperties.DeliveryMode)] is DeliveryModes deliveryModes
            ? deliveryModes
            : DeliveryModes.Transient;
    }

    public static void SetDeliveryMode(this PublisherArgs self, byte? value) {
        self[nameof(IBasicProperties.DeliveryMode)] = value;
    }

    public static string? GetExpiration(this PublisherArgs self) {
        return self[nameof(IBasicProperties.Expiration)] as string;
    }

    public static void SetExpiration(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.Expiration)] = value;
    }

    public static IDictionary<string, object?> GetHeaders(this PublisherArgs self) {
        return self[nameof(IBasicProperties.Headers)] as Dictionary<string, object?> ?? [];
    }

    public static void SetHeaders(this PublisherArgs self, IDictionary<string, object>? value) {
        self[nameof(IBasicProperties.Headers)] = value;
    }

    public static string? GetMessageId(this PublisherArgs self) {
        return self[nameof(IBasicProperties.MessageId)] as string;
    }

    public static void SetMessageId(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.MessageId)] = value;
    }

    public static bool GetPersistent(this PublisherArgs self) {
        return self[nameof(IBasicProperties.Persistent)] is true;
    }

    public static void SetPersistent(this PublisherArgs self, bool? value) {
        self[nameof(IBasicProperties.Persistent)] = value;
    }

    public static byte GetPriority(this PublisherArgs self) {
        return self[nameof(IBasicProperties.Priority)] is byte value
            ? value
            : (byte)0;
    }

    public static void SetPriority(this PublisherArgs self, byte? value) {
        self[nameof(IBasicProperties.Priority)] = value;
    }

    public static string? GetReplyTo(this PublisherArgs self) {
        return self[nameof(IBasicProperties.ReplyTo)] as string;
    }

    public static void SetReplyTo(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.ReplyTo)] = value;
    }

    public static PublicationAddress? GetReplyToAddress(this PublisherArgs self) {
        return self[nameof(IBasicProperties.ReplyToAddress)] as PublicationAddress;
    }

    public static void SetReplyToAddress(this PublisherArgs self, PublicationAddress? value) {
        self[nameof(IBasicProperties.ReplyToAddress)] = value;
    }

    public static AmqpTimestamp GetTimestamp(this PublisherArgs self) {
        return self[nameof(IBasicProperties.Timestamp)] is AmqpTimestamp value
            ? value
            : default;
    }

    public static void SetTimestamp(this PublisherArgs self, AmqpTimestamp? value) {
        self[nameof(IBasicProperties.Timestamp)] = value;
    }

    public static string? GetTypeProp(this PublisherArgs self) {
        return self[nameof(IBasicProperties.Type)] as string;
    }

    public static void SetTypeProp(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.Type)] = value;
    }

    public static string? GetUserId(this PublisherArgs self) {
        return self[nameof(IBasicProperties.UserId)] as string;
    }

    public static void SetUserId(this PublisherArgs self, string? value) {
        self[nameof(IBasicProperties.UserId)] = value;
    }
}