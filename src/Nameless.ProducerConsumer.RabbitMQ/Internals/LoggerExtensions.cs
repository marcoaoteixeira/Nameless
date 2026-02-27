using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;
using RabbitMQ.Client.Events;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> QueueSettingsNotFoundDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Warning,
            Events.QueueSettingsNotFoundEvent,
            formatString: "Queue settings for '{QueueName}' not found.");

    private static readonly Action<ILogger, string, Exception?> ExchangeNotFoundDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Warning,
            Events.ExchangeNotFoundEvent,
            formatString: "Exchange settings for '{ExchangeName}' not found.");

    private static readonly Action<ILogger, object, Exception> BrokerUnreachableDelegate
        = LoggerMessage.Define<object>(
            LogLevel.Error,
            Events.BrokerUnreachableEvent,
            formatString: "Unable to connect to broker. Server settings: {@ServerSettings}");

    private static readonly Action<ILogger, Exception> UnhandledErrorWhileProducingMessageDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.UnhandledErrorWhileProducingMessageEvent,
            formatString: "Unhandled error while producing message.");

    private static readonly Action<ILogger, Exception> UnhandledErrorWhileCreatingProducerDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.UnhandledErrorWhileCreatingProducerEvent,
            formatString: "Unhandled error while creating producer.");

    private static readonly Action<ILogger, Exception> UnhandledErrorWhileCreatingConsumerDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.UnhandledErrorWhileCreatingConsumerEvent,
            formatString: "Unhandled error while creating consumer.");

    private static readonly Action<ILogger, string, Exception> EnvelopeDeserializationErrorDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            Events.EnvelopeDeserializationErrorEvent,
            formatString: "An error occured while deserializing the Envelope on consumer '{ConsumerTag}'.");

    private static readonly Action<ILogger, BasicDeliverEventArgs, Exception?> EnvelopeDeserializationFailureDelegate
        = LoggerMessage.Define<BasicDeliverEventArgs>(
            LogLevel.Warning,
            Events.EnvelopeDeserializationFailureEvent,
            formatString:
            "Envelope was deserialized successfully but return is null. Delivery arguments: {@BasicDeliverEventArgs}");

    private static readonly Action<ILogger, Exception?> ConsumerAlreadyStartedDelegate
        = LoggerMessage.Define(
            LogLevel.Information,
            Events.ConsumerAlreadyStartedEvent,
            formatString: "Consumer already started.");

    private static readonly Action<ILogger, string, string, Exception?> ConsumerShutdownDelegate
        = LoggerMessage.Define<string, string>(
            LogLevel.Information,
            Events.ConsumerShutdownEvent,
            formatString: "Consumer '{ConsumerTag}' was shutdown. Reason: {Reason}");

    private static readonly Action<ILogger, string, Exception> MessageHandlerThrownExceptionDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Error,
            Events.MessageHandlerThrownExceptionEvent,
            formatString: "Message handler thrown exception during execution on consumer '{ConsumerTag}'.");

    private static readonly Action<ILogger, string, string, Parameters, Exception?> ConsumerStartedDelegate
        = LoggerMessage.Define<string, string, Parameters>(
            LogLevel.Information,
            Events.ConsumerStartedEvent,
            formatString: "Consumer '{ConsumerTag}' started with status '{StartupStatus}'. Arguments: {@Parameters}");

    private static readonly Action<ILogger, Envelope, Exception?> EnvelopeReceivedDelegate
        = LoggerMessage.Define<Envelope>(
            LogLevel.Debug,
            Events.EnvelopeReceivedEvent,
            formatString: "Envelope received: {@Envelope}");

    private static readonly Action<ILogger, string, Exception?> UnableDeserializeMessageDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Warning,
            Events.UnableDeserializeMessageEvent,
            formatString: "Unable to deserialize message to type '{MessageType}'.");

    extension(ILogger<ChannelConfigurator> self) {
        internal void QueueSettingsNotFound(string queueName) {
            QueueSettingsNotFoundDelegate(self, queueName, arg3: null /* exception */);
        }

        internal void ExchangeNotFound(string exchangeName) {
            ExchangeNotFoundDelegate(self, exchangeName, arg3: null /* exception */);
        }
    }

    internal static void BrokerUnreachable(this ILogger<ConnectionManager> self, ServerOptions server,
        Exception exception) {
        BrokerUnreachableDelegate(self, new {
            server.Hostname,
            server.Port,
            server.VirtualHost,
            server.Protocol,
            server.UseCredentials,
            SslIsAvailable = server.Ssl.IsAvailable
        }, exception);
    }

    extension(ILogger logger) {
        internal void UnhandledErrorWhileProducingMessage(Exception exception) {
            UnhandledErrorWhileProducingMessageDelegate(logger, exception);
        }

        internal void UnhandledErrorWhileCreatingProducer(Exception exception) {
            UnhandledErrorWhileCreatingProducerDelegate(logger, exception);
        }

        internal void UnhandledErrorWhileCreatingConsumer(Exception exception) {
            UnhandledErrorWhileCreatingConsumerDelegate(logger, exception);
        }

        internal void EnvelopeDeserializationError(string consumerTag, Exception exception) {
            EnvelopeDeserializationErrorDelegate(logger, consumerTag, exception);
        }

        internal void EnvelopeDeserializationFailure(BasicDeliverEventArgs args) {
            EnvelopeDeserializationFailureDelegate(logger, args, arg3: null /* exception */);
        }

        internal void ConsumerAlreadyStarted() {
            ConsumerAlreadyStartedDelegate(logger, arg2: null /* exception */);
        }

        internal void ConsumerShutdown(string consumerTag, string reason) {
            ConsumerShutdownDelegate(logger, consumerTag, reason, arg4: null /* exception */);
        }

        internal void MessageHandlerThrownException(string consumerTag, Exception exception) {
            MessageHandlerThrownExceptionDelegate(logger, consumerTag, exception);
        }

        internal void ConsumerStarted(string startupStatus, string consumerTag, Parameters parameters) {
            ConsumerStartedDelegate(logger, startupStatus, consumerTag, parameters, arg5: null /* exception */);
        }

        internal void EnvelopeReceived(Envelope envelope) {
            EnvelopeReceivedDelegate(logger, envelope, arg3: null /* exception */);
        }

        internal void UnableDeserializeMessage(Type messageType) {
            UnableDeserializeMessageDelegate(logger, messageType.Name, arg3: null /* exception */);
        }
    }

    internal static class Events {
        internal static readonly EventId QueueSettingsNotFoundEvent = new(id: 8001, nameof(QueueSettingsNotFound));
        internal static readonly EventId ExchangeNotFoundEvent = new(id: 8002, nameof(ExchangeNotFound));
        internal static readonly EventId BrokerUnreachableEvent = new(id: 8003, nameof(BrokerUnreachable));

        internal static readonly EventId UnhandledErrorWhileProducingMessageEvent =
            new(id: 8004, nameof(UnhandledErrorWhileProducingMessage));

        internal static readonly EventId UnhandledErrorWhileCreatingProducerEvent =
            new(id: 8005, nameof(UnhandledErrorWhileCreatingProducer));

        internal static readonly EventId UnhandledErrorWhileCreatingConsumerEvent =
            new(id: 8006, nameof(UnhandledErrorWhileCreatingConsumer));

        internal static readonly EventId EnvelopeDeserializationFailureEvent =
            new(id: 8007, nameof(EnvelopeDeserializationFailure));

        internal static readonly EventId EnvelopeDeserializationErrorEvent =
            new(id: 8008, nameof(EnvelopeDeserializationError));

        internal static readonly EventId ConsumerAlreadyStartedEvent = new(id: 8009, nameof(ConsumerAlreadyStarted));
        internal static readonly EventId ConsumerShutdownEvent = new(id: 8010, nameof(ConsumerShutdown));

        internal static readonly EventId MessageHandlerThrownExceptionEvent =
            new(id: 8011, nameof(MessageHandlerThrownException));

        internal static readonly EventId ConsumerStartedEvent = new(id: 8012, nameof(ConsumerStarted));
        internal static readonly EventId EnvelopeReceivedEvent = new(id: 8013, nameof(EnvelopeReceived));

        internal static readonly EventId
            UnableDeserializeMessageEvent = new(id: 8014, nameof(UnableDeserializeMessage));
    }
}