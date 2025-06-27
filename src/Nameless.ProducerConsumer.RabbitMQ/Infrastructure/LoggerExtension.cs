using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

internal static class LoggerExtension {
    private static readonly Action<ILogger, string, Exception?> QueueSettingsNotFoundDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.QueueSettingsNotFoundEvent,
            formatString: "Queue settings for '{QueueName}' not found.");

    private static readonly Action<ILogger, string, Exception?> ExchangeNotFoundDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.ExchangeNotFoundEvent,
            formatString: "Exchange settings for '{ExchangeName}' not found.");

    private static readonly Action<ILogger, object, Exception> BrokerUnreachableDelegate
        = LoggerMessage.Define<object>(
            logLevel: LogLevel.Error,
            eventId: Events.BrokerUnreachableEvent,
            formatString: "Unable to connect to broker. Server settings: {@ServerSettings}");

    internal static void QueueSettingsNotFound(this ILogger<ChannelConfigurator> self, string queueName) {
        QueueSettingsNotFoundDelegate(self, queueName, null /* exception */);
    }

    internal static void ExchangeNotFound(this ILogger<ChannelConfigurator> self, string exchangeName) {
        ExchangeNotFoundDelegate(self, exchangeName, null /* exception */);
    }

    internal static void BrokerUnreachable(this ILogger<ConnectionManager> self, ServerSettings server, Exception exception) {
        BrokerUnreachableDelegate(self, new {
            server.Hostname,
            server.Port,
            server.VirtualHost,
            server.Protocol,
            server.UseCredentials,
            SslIsAvailable = server.Ssl.IsAvailable
        }, exception);
    }

    internal static class Events {
        internal static readonly EventId QueueSettingsNotFoundEvent = new(1, nameof(QueueSettingsNotFound));
        internal static readonly EventId ExchangeNotFoundEvent = new(2, nameof(ExchangeNotFound));
        internal static readonly EventId BrokerUnreachableEvent = new(2, nameof(BrokerUnreachable));
    }
}