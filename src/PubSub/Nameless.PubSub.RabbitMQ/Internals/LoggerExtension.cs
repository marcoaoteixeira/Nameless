using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace Nameless.PubSub.RabbitMQ;

internal static class LoggerExtension {
    private static readonly Action<ILogger<ChannelFactory>, string, Exception?> MissingExchangeDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "Could not found exchange with name: {ExchangeName}",
                                       options: null);

    internal static void MissingExchange(this ILogger<ChannelFactory> self, string exchangeName)
        => MissingExchangeDelegate(self, exchangeName, null /* exception */);

    private static readonly Action<ILogger<Publisher>, Exception> UnhandledErrorWhilePublishingDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurred while publishing a message.",
                               options: null);

    internal static void UnhandledErrorWhilePublishing(this ILogger<Publisher> self, Exception exception)
        => UnhandledErrorWhilePublishingDelegate(self, exception);

    private static readonly Action<ILogger<Subscriber>, string, Exception?> StartingSubscriptionDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Starting subscription with consumer tag: {ConsumerTag}",
                                       options: null);

    internal static void StartingSubscription(this ILogger<Subscriber> self, string consumerTag)
        => StartingSubscriptionDelegate(self, consumerTag, null /* exception */);

    private static readonly Action<ILogger<Subscriber>, string, string, Exception?> SubscriptionStartedDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Information,
                                               eventId: default,
                                               formatString: "Subscription started. Consumer tag: {ConsumerTag} | Startup result: {StartupResult}",
                                               options: null);

    internal static void SubscriptionStarted(this ILogger<Subscriber> self, string consumerTag, string startupResult)
        => SubscriptionStartedDelegate(self, consumerTag, startupResult, null /* exception */);

    private static readonly Action<ILogger<Subscriber>, Exception> DeserializeEnvelopeFailedDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurred while deserializing the envelope.",
                               options: null);

    internal static void DeserializeEnvelopeFailed(this ILogger<Subscriber> self, Exception exception)
        => DeserializeEnvelopeFailedDelegate(self, exception);

    private static readonly Action<ILogger<Subscriber>, string, string?, string?, Exception?> EnvelopeDeserializationNullDelegate
        = LoggerMessage.Define<string, string?, string?>(logLevel: LogLevel.Warning,
                                                       eventId: default,
                                                       formatString: "The envelope deserialization returned null. Consumer Tag: {ConsumerTag} | Message ID: {MessageId} | Correlation ID: {CorrelationId}",
                                                       options: null);

    internal static void EnvelopeDeserializationNull(this ILogger<Subscriber> self, BasicDeliverEventArgs args)
        => EnvelopeDeserializationNullDelegate(self,
                                               args.ConsumerTag,
                                               args.BasicProperties.MessageId,
                                               args.BasicProperties.CorrelationId,
                                               null /* exception */);

    private static readonly Action<ILogger<Subscriber>, Exception> MessageHandlerCreationFailedDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurred while creating the message handler.",
                               options: null);

    internal static void MessageHandlerCreationFailed(this ILogger<Subscriber> self, Exception exception)
        => MessageHandlerCreationFailedDelegate(self, exception);

    private static readonly Action<ILogger<Subscriber>, Exception?> MessageHandlerIsNullDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "It was not possible to create the message handler. Target might be garbage collected.",
                               options: null);

    internal static void MessageHandlerIsNull(this ILogger<Subscriber> self)
        => MessageHandlerIsNullDelegate(self, null /* exception */);
}