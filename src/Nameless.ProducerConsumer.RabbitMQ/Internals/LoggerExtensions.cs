using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;
internal static class LoggerExtensions {
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

    private static readonly Action<ILogger, Exception> UnhandledErrorWhileProducingMessageDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.UnhandledErrorWhileProducingMessageEvent,
            formatString: "Unhandled error while producing message.");

    private static readonly Action<ILogger, Exception> UnhandledErrorWhileCreatingProducerDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.UnhandledErrorWhileCreatingProducerEvent,
            formatString: "Unhandled error while creating producer.");

    private static readonly Action<ILogger, Exception> UnhandledErrorWhileCreatingConsumerDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.UnhandledErrorWhileCreatingConsumerEvent,
            formatString: "Unhandled error while creating consumer.");

    private static readonly Action<ILogger, string, Exception> EnvelopeDeserializationErrorDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: Events.EnvelopeDeserializationErrorEvent,
            formatString: "An error occured while deserializing the Envelope on consumer '{ConsumerTag}'.");

    private static readonly Action<ILogger, BasicDeliverEventArgs, Exception?> EnvelopeDeserializationFailureDelegate
        = LoggerMessage.Define<BasicDeliverEventArgs>(
            logLevel: LogLevel.Warning,
            eventId: Events.EnvelopeDeserializationFailureEvent,
            formatString: "Envelope was deserialized successfully but return is null. Delivery arguments: {@BasicDeliverEventArgs}");

    private static readonly Action<ILogger, Exception?> ConsumerAlreadyStartedDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Information,
            eventId: Events.ConsumerAlreadyStartedEvent,
            formatString: "Consumer already started.");

    private static readonly Action<ILogger, string, string, Exception?> ConsumerShutdownDelegate
        = LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Information,
            eventId: Events.ConsumerShutdownEvent,
            formatString: "Consumer '{ConsumerTag}' was shutdown. Reason: {Reason}");

    private static readonly Action<ILogger, string, Exception> MessageHandlerThrownExceptionDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: Events.MessageHandlerThrownExceptionEvent,
            formatString: "Message handler thrown exception during execution on consumer '{ConsumerTag}'.");

    private static readonly Action<ILogger, string, string, Parameters, Exception?> ConsumerStartedDelegate
        = LoggerMessage.Define<string, string, Parameters>(
            logLevel: LogLevel.Information,
            eventId: Events.ConsumerStartedEvent,
            formatString: "Consumer '{ConsumerTag}' started with status '{StartupStatus}'. Arguments: {@Parameters}");

    private static readonly Action<ILogger, Envelope, Exception?> EnvelopeReceivedDelegate
        = LoggerMessage.Define<Envelope>(
            logLevel: LogLevel.Debug,
            eventId: Events.EnvelopeReceivedEvent,
            formatString: "Envelope received: {@Envelope}");

    private static readonly Action<ILogger, string, Exception?> UnableDeserializeMessageDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Warning,
            eventId: Events.UnableDeserializeMessageEvent,
            formatString: "Unable to deserialize message to type '{MessageType}'.");

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

    internal static void UnhandledErrorWhileProducingMessage(this ILogger logger, Exception exception) {
        UnhandledErrorWhileProducingMessageDelegate(logger, exception);
    }

    internal static void UnhandledErrorWhileCreatingProducer(this ILogger logger, Exception exception) {
        UnhandledErrorWhileCreatingProducerDelegate(logger, exception);
    }

    internal static void UnhandledErrorWhileCreatingConsumer(this ILogger logger, Exception exception) {
        UnhandledErrorWhileCreatingConsumerDelegate(logger, exception);
    }

    internal static void EnvelopeDeserializationError(this ILogger logger, string consumerTag, Exception exception) {
        EnvelopeDeserializationErrorDelegate(logger, consumerTag, exception);
    }

    internal static void EnvelopeDeserializationFailure(this ILogger logger, BasicDeliverEventArgs args) {
        EnvelopeDeserializationFailureDelegate(logger, args, null /* exception */);
    }

    internal static void ConsumerAlreadyStarted(this ILogger logger) {
        ConsumerAlreadyStartedDelegate(logger, null /* exception */);
    }

    internal static void ConsumerShutdown(this ILogger logger, string consumerTag, string reason) {
        ConsumerShutdownDelegate(logger, consumerTag, reason, null /* exception */);
    }

    internal static void MessageHandlerThrownException(this ILogger logger, string consumerTag, Exception exception) {
        MessageHandlerThrownExceptionDelegate(logger, consumerTag, exception);
    }

    internal static void ConsumerStarted(this ILogger logger, string startupStatus, string consumerTag, Parameters parameters) {
        ConsumerStartedDelegate(logger, startupStatus, consumerTag, parameters, null /* exception */);
    }

    internal static void EnvelopeReceived(this ILogger logger, Envelope envelope) {
        EnvelopeReceivedDelegate(logger, envelope, null /* exception */);
    }

    internal static void UnableDeserializeMessage(this ILogger logger, Type messageType) {
        UnableDeserializeMessageDelegate(logger, messageType.Name, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId QueueSettingsNotFoundEvent = new(8001, nameof(QueueSettingsNotFound));
        internal static readonly EventId ExchangeNotFoundEvent = new(8002, nameof(ExchangeNotFound));
        internal static readonly EventId BrokerUnreachableEvent = new(8003, nameof(BrokerUnreachable));
        internal static readonly EventId UnhandledErrorWhileProducingMessageEvent = new(8004, nameof(UnhandledErrorWhileProducingMessage));
        internal static readonly EventId UnhandledErrorWhileCreatingProducerEvent = new(8005, nameof(UnhandledErrorWhileCreatingProducer));
        internal static readonly EventId UnhandledErrorWhileCreatingConsumerEvent = new(8006, nameof(UnhandledErrorWhileCreatingConsumer));
        internal static readonly EventId EnvelopeDeserializationFailureEvent = new(8007, nameof(EnvelopeDeserializationFailure));
        internal static readonly EventId EnvelopeDeserializationErrorEvent = new(8008, nameof(EnvelopeDeserializationError));
        internal static readonly EventId ConsumerAlreadyStartedEvent = new(8009, nameof(ConsumerAlreadyStarted));
        internal static readonly EventId ConsumerShutdownEvent = new(8010, nameof(ConsumerShutdown));
        internal static readonly EventId MessageHandlerThrownExceptionEvent = new(8011, nameof(MessageHandlerThrownException));
        internal static readonly EventId ConsumerStartedEvent = new(8012, nameof(ConsumerStarted));
        internal static readonly EventId EnvelopeReceivedEvent = new(8013, nameof(EnvelopeReceived));
        internal static readonly EventId UnableDeserializeMessageEvent = new(8014, nameof(UnableDeserializeMessage));
    }
}
