﻿using RabbitMQ.Client;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ProducerArgsExtension {
    public static string GetExchangeName(this ProducerArgs self) {
        var arg = self[Internals.Constants.ProducerArgsTokens.EXCHANGE_NAME] ?? Internals.Defaults.EXCHANGE_NAME;

        return (string)arg;
    }

    public static ProducerArgs SetExchangeName(this ProducerArgs self, string value) {
        self[Internals.Constants.ProducerArgsTokens.EXCHANGE_NAME] = value.WithFallback(Internals.Defaults.EXCHANGE_NAME);
        
        return self;
    }

    public static string[] GetRoutingKeys(this ProducerArgs self) {
        var arg = self[Internals.Constants.ProducerArgsTokens.ROUTING_KEYS] ?? Array.Empty<string>();

        return (string[])arg;
    }

    public static ProducerArgs SetRoutingKeys(this ProducerArgs self, string[] value) {
        self[Internals.Constants.ProducerArgsTokens.ROUTING_KEYS] = value;

        return self;
    }

    public static bool GetMandatory(this ProducerArgs self) {
        var arg = self[Internals.Constants.ProducerArgsTokens.MANDATORY] ?? false;

        return (bool)arg;
    }

    public static ProducerArgs SetMandatory(this ProducerArgs self, bool value) {
        self[Internals.Constants.ProducerArgsTokens.MANDATORY] = value;

        return self;
    }

    public static string? GetAppId(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.AppId)];

        return (string?)arg;
    }

    public static ProducerArgs SetAppId(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.AppId)] = value;
        }

        return self;
    }

    public static string? GetClusterId(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.ClearAppId)];

        return (string?)arg;
    }

    public static ProducerArgs SetClusterId(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.ClusterId)] = value;
        }

        return self;
    }

    public static string? GetContentEncoding(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.ContentEncoding)];

        return (string?)arg;
    }

    public static ProducerArgs SetContentEncoding(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.ContentEncoding)] = value;
        }

        return self;
    }

    public static string? GetContentType(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.ContentType)];

        return (string?)arg;
    }

    public static ProducerArgs SetContentType(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.ContentType)] = value;
        }

        return self;
    }

    public static string? GetCorrelationId(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.CorrelationId)];

        return (string?)arg;
    }

    public static ProducerArgs SetCorrelationId(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.CorrelationId)] = value;
        }

        return self;
    }

    public static DeliveryModes GetDeliveryMode(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.DeliveryMode)] ?? DeliveryModes.Transient;

        return (DeliveryModes)arg;
    }

    public static ProducerArgs SetDeliveryMode(this ProducerArgs self, byte value) {
        self[nameof(IBasicProperties.DeliveryMode)] = value;

        return self;
    }

    public static string? GetExpiration(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.Expiration)];

        return (string?)arg;
    }

    public static ProducerArgs SetExpiration(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.Expiration)] = value;
        }

        return self;
    }

    public static IDictionary<string, object?> GetHeaders(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.Headers)] ?? new Dictionary<string, object?>();

        return (IDictionary<string, object?>)arg;
    }

    public static ProducerArgs SetHeaders(this ProducerArgs self, IDictionary<string, object> value) {
        self[nameof(IBasicProperties.Headers)] = value;

        return self;
    }

    public static string? GetMessageId(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.MessageId)];

        return (string?)arg;
    }

    public static ProducerArgs SetMessageId(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.MessageId)] = value;
        }

        return self;
    }

    public static bool GetPersistent(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.Persistent)] ?? false;

        return (bool)arg;
    }

    public static ProducerArgs SetPersistent(this ProducerArgs self, bool value) {
        self[nameof(IBasicProperties.Persistent)] = value;

        return self;
    }

    public static byte GetPriority(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.Priority)] ?? byte.MinValue;

        return (byte)arg;
    }

    public static ProducerArgs SetPriority(this ProducerArgs self, byte value) {
        self[nameof(IBasicProperties.Priority)] = value;

        return self;
    }

    public static string? GetReplyTo(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.ReplyTo)];

        return (string?)arg;
    }

    public static ProducerArgs SetReplyTo(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.ReplyTo)] = value;
        }

        return self;
    }

    public static PublicationAddress? GetReplyToAddress(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.ReplyToAddress)];

        return (PublicationAddress?)arg;
    }

    public static ProducerArgs SetReplyToAddress(this ProducerArgs self, PublicationAddress? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.ReplyToAddress)] = value;
        }

        return self;
    }

    public static AmqpTimestamp GetTimestamp(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.Timestamp)] ?? default(AmqpTimestamp);

        return (AmqpTimestamp)arg;
    }

    public static ProducerArgs SetTimestamp(this ProducerArgs self, AmqpTimestamp value) {
        self[nameof(IBasicProperties.Timestamp)] = value;

        return self;
    }

    public static string? GetTypeProp(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.Type)];

        return (string?)arg;
    }

    public static ProducerArgs SetTypeProp(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.Type)] = value;
        }

        return self;
    }

    public static string? GetUserId(this ProducerArgs self) {
        var arg = self[nameof(IBasicProperties.UserId)];

        return (string?)arg;
    }

    public static ProducerArgs SetUserId(this ProducerArgs self, string? value) {
        if (value is not null) {
            self[nameof(IBasicProperties.UserId)] = value;
        }

        return self;
    }
}