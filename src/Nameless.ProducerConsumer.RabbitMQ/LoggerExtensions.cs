using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ;
internal static class LoggerExtensions {
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

    private static readonly Action<ILogger, string, string, Args, Exception?> ConsumerStartedDelegate
        = LoggerMessage.Define<string, string, Args>(
            logLevel: LogLevel.Information,
            eventId: Events.ConsumerStartedEvent,
            formatString: "Consumer '{ConsumerTag}' started with status '{StartupStatus}'. Arguments: {@Args}");

    internal static void UnhandledErrorWhileProducingMessage(this ILogger<Producer> logger, Exception exception) {
        UnhandledErrorWhileProducingMessageDelegate(logger, exception);
    }

    internal static void UnhandledErrorWhileCreatingProducer(this ILogger<ProducerFactory> logger, Exception exception) {
        UnhandledErrorWhileCreatingProducerDelegate(logger, exception);
    }

    internal static void UnhandledErrorWhileCreatingConsumer(this ILogger<ConsumerFactory> logger, Exception exception) {
        UnhandledErrorWhileCreatingConsumerDelegate(logger, exception);
    }

    internal static void EnvelopeDeserializationError(this ILogger<Consumer> logger, string consumerTag, Exception exception) {
        EnvelopeDeserializationErrorDelegate(logger, consumerTag, exception);
    }

    internal static void EnvelopeDeserializationFailure(this ILogger<Consumer> logger, BasicDeliverEventArgs args) {
        EnvelopeDeserializationFailureDelegate(logger, args, null /* exception */);
    }

    internal static void ConsumerAlreadyStarted(this ILogger<Consumer> logger) {
        ConsumerAlreadyStartedDelegate(logger, null /* exception */);
    }

    internal static void ConsumerShutdown(this ILogger<Consumer> logger, string consumerTag, string reason) {
        ConsumerShutdownDelegate(logger, consumerTag, reason, null /* exception */);
    }

    internal static void MessageHandlerThrownException(this ILogger<Consumer> logger, string consumerTag, Exception exception) {
        MessageHandlerThrownExceptionDelegate(logger, consumerTag, exception);
    }

    internal static void ConsumerStarted(this ILogger<Consumer> logger, string startupStatus, string consumerTag, Args args) {
        ConsumerStartedDelegate(logger, startupStatus, consumerTag, args, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId UnhandledErrorWhileProducingMessageEvent = new(1, nameof(UnhandledErrorWhileProducingMessage));
        internal static readonly EventId UnhandledErrorWhileCreatingProducerEvent = new(2, nameof(UnhandledErrorWhileCreatingProducer));
        internal static readonly EventId UnhandledErrorWhileCreatingConsumerEvent = new(3, nameof(UnhandledErrorWhileCreatingConsumer));
        internal static readonly EventId EnvelopeDeserializationFailureEvent = new(4, nameof(EnvelopeDeserializationFailure));
        internal static readonly EventId EnvelopeDeserializationErrorEvent = new(5, nameof(EnvelopeDeserializationError));
        internal static readonly EventId ConsumerAlreadyStartedEvent = new(6, nameof(ConsumerAlreadyStarted));
        internal static readonly EventId ConsumerShutdownEvent = new(7, nameof(ConsumerShutdown));
        internal static readonly EventId MessageHandlerThrownExceptionEvent = new(8, nameof(MessageHandlerThrownException));
        internal static readonly EventId ConsumerStartedEvent = new(9, nameof(ConsumerStarted));
    }
}
