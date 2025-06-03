using Microsoft.Extensions.Logging;

namespace Nameless.ProducerConsumer.RabbitMQ.Infrastructure;

internal static class LoggerExtension {
    private static readonly Action<ILogger, string, Exception?> ExchangeNotFoundDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.ExchangeNotFoundEvent,
            formatString: "Configuration for exchange '{ExchangeName}' not found.");

    internal static void ExchangeNotFound(this ILogger<ChannelConfigurator> self, string exchangeName) {
        ExchangeNotFoundDelegate(self, exchangeName, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId ExchangeNotFoundEvent = new(1, nameof(ExchangeNotFound));
    }
}