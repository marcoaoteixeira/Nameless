using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace Nameless.PubSub.RabbitMQ.Infrastructure;

internal static class LoggerExtension {
    private static readonly Action<ILogger, string, Exception?> MissingExchangeOptionsDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Missing exchange '{ExchangeName}' options in settings.");

    private static readonly Action<ILogger<Publisher>, Exception> UnhandledErrorWhilePublishingDelegate
        = LoggerMessage.Define(LogLevel.Error,
            default,
            "An error occurred while publishing a message.",
            null);

    private static readonly Action<ILogger, string, Exception?> CreatingSubscriptionDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "Creating subscription with consumer tag: {ConsumerTag}");

    private static readonly Action<ILogger, string, string, Exception?> SubscriptionCreationCompleteDelegate
        = LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "Subscription creation complete. Consumer tag: {ConsumerTag} | Startup result: {StartupResult}");

    private static readonly Action<ILogger<Subscriber>, Exception> DeserializeEnvelopeFailedDelegate
        = LoggerMessage.Define(LogLevel.Error,
            default,
            "An error occurred while deserializing the envelope.",
            null);

    private static readonly Action<ILogger<Subscriber>, string, string?, string?, Exception?>
        EnvelopeDeserializationNullDelegate
            = LoggerMessage.Define<string, string?, string?>(LogLevel.Warning,
                default,
                "The envelope deserialization returned null. Consumer Tag: {ConsumerTag} | Message ConsumerTag: {MessageId} | Correlation ConsumerTag: {CorrelationId}",
                null);

    private static readonly Action<ILogger<Subscriber>, Exception> MessageHandlerCreationFailedDelegate
        = LoggerMessage.Define(LogLevel.Error,
            default,
            "An error occurred while creating the message handler.",
            null);

    private static readonly Action<ILogger<Subscriber>, Exception?> MessageHandlerIsNullDelegate
        = LoggerMessage.Define(LogLevel.Warning,
            default,
            "It was not possible to create the message handler. Target might be garbage collected.",
            null);

    internal static void MissingExchangeOptions(this ILogger<ChannelConfigurator> self, string exchangeName) {
        MissingExchangeOptionsDelegate(self, exchangeName, null /* exception */);
    }

    internal static void UnhandledErrorWhilePublishing(this ILogger<Publisher> self, Exception exception) {
        UnhandledErrorWhilePublishingDelegate(self, exception);
    }

    internal static void CreatingSubscription(this ILogger<Subscriber> self, string consumerTag) {
        CreatingSubscriptionDelegate(self, consumerTag, null /* exception */);
    }

    internal static void SubscriptionCreationComplete(this ILogger<Subscriber> self, string consumerTag, string startupResult) {
        SubscriptionCreationCompleteDelegate(self, consumerTag, startupResult, null /* exception */);
    }

    internal static void DeserializeEnvelopeFailed(this ILogger<Subscriber> self, Exception exception) {
        DeserializeEnvelopeFailedDelegate(self, exception);
    }

    internal static void EnvelopeDeserializationNull(this ILogger<Subscriber> self, BasicDeliverEventArgs args) {
        EnvelopeDeserializationNullDelegate(self,
            args.ConsumerTag,
            args.BasicProperties.MessageId,
            args.BasicProperties.CorrelationId,
            null /* exception */);
    }

    internal static void MessageHandlerCreationFailed(this ILogger<Subscriber> self, Exception exception) {
        MessageHandlerCreationFailedDelegate(self, exception);
    }

    internal static void MessageHandlerIsNull(this ILogger<Subscriber> self) {
        MessageHandlerIsNullDelegate(self, null /* exception */);
    }
}