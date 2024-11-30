using System.Net.Security;
using Microsoft.Extensions.Logging;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;
internal static class LoggerHighPerformanceExtension {
    private static readonly Action<ILogger, Exception> ErrorOnMessagePublishingDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while publishing message.",
                               options: null);

    internal static void ErrorOnMessagePublishing(this ILogger<Producer> self, Exception exception)
        => ErrorOnMessagePublishingDelegate(self, exception);

    private static readonly Action<ILogger, SslPolicyErrors, Exception?> ErrorOnCertificateValidationDelegate
        = LoggerMessage.Define<SslPolicyErrors>(logLevel: LogLevel.Warning,
                                               eventId: default,
                                               formatString: "Certificate validation error. SSLPolicyErrors: {SSLPolicyErrors}",
                                               options: null);

    internal static void ErrorOnCertificateValidation(this ILogger<ConnectionManager> self, SslPolicyErrors sslPolicyErrors)
        => ErrorOnCertificateValidationDelegate(self, sslPolicyErrors, null /* exception */);

    private static readonly Action<ILogger, string, string, Exception?> ConsumerRegistrationDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Information,
                                               eventId: default,
                                               formatString: "Registering consumer. Tag: {Tag} | Arguments: {Arguments}",
                                               options: null);

    internal static void ConsumerRegistration(this ILogger<Consumer> self, string tag, string arguments)
        => ConsumerRegistrationDelegate(self, tag, arguments, null /* exception */);

    private static readonly Action<ILogger, Exception> ErrorOnConsumerHandlerCreationDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Consumer handler creation failed.",
                               options: null);

    internal static void ErrorOnConsumerHandlerCreation(this ILogger<Consumer> self, Exception exception)
        => ErrorOnConsumerHandlerCreationDelegate(self, exception);

    private static readonly Action<ILogger, Exception?> MessageHandlerNotFoundDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "No suitable message handler found.",
                               options: null);

    internal static void MessageHandlerNotFound(this ILogger<Consumer> self)
        => MessageHandlerNotFoundDelegate(self, null /* exception */);

    private static readonly Action<ILogger, Exception> ErrorOnEnvelopeDeserializationDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs while deserializing the envelope.",
                               options: null);

    internal static void ErrorOnEnvelopeDeserialization(this ILogger<Consumer> self, Exception exception)
        => ErrorOnEnvelopeDeserializationDelegate(self, exception);

    private static readonly Action<ILogger, Exception?> NullEnvelopeDeserializedDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "Unable to deserialize the envelope.",
                               options: null);

    internal static void NullEnvelopeDeserialized(this ILogger<Consumer> self)
        => NullEnvelopeDeserializedDelegate(self, null /* exception */);

    private static readonly Action<ILogger, Exception?> EnvelopeMessageNotValidJsonElementDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "Message was not valid JSON.",
                               options: null);

    internal static void EnvelopeMessageNotValidJsonElement(this ILogger self)
        => EnvelopeMessageNotValidJsonElementDelegate(self, null /* exception */);

    private static readonly Action<ILogger, Exception> ErrorOnMessageDeserializationDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs while deserializing the message.",
                               options: null);

    internal static void ErrorOnMessageDeserialization(this ILogger<Consumer> self, Exception exception)
        => ErrorOnMessageDeserializationDelegate(self, exception);

    private static readonly Action<ILogger, string, Exception?> NullMessageDeserializationDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "Unable to deserialize the message. Expected message type: {MessageType}.",
                                       options: null);

    internal static void NullMessageDeserialization<T>(this ILogger<Consumer> self)
        => NullMessageDeserializationDelegate(self, typeof(T).Name, null /* exception */);

    private static readonly Action<ILogger, Exception> ErrorOnHandleMessageDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs when trying to execute the message handler.",
                               options: null);

    internal static void ErrorOnHandleMessage(this ILogger<Consumer> self, Exception exception)
        => ErrorOnHandleMessageDelegate(self, exception);

    private static readonly Action<ILogger, string, Exception?> ConsumerStartupResultDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                      eventId: default,
                                      formatString: "Basic consumer startup completed. Status: {Status}",
                                      options: null);

    internal static void ConsumerStartupResult(this ILogger<Consumer> self, string status)
        => ConsumerStartupResultDelegate(self, status, null /* exception */);

    private static readonly Action<ILogger, Exception> ConsumerRegistrationFailedDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Consumer registration failed.",
                               options: null);

    internal static void ConsumerRegistrationFailed(this ILogger<Consumer> self, Exception exception)
        => ConsumerRegistrationFailedDelegate(self, exception);

    private static readonly Action<ILogger, Exception> RemoveRegistrationFromCacheFailedDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs while removing consumer registration from cache.",
                               options: null);

    internal static void RemoveRegistrationFromCacheFailed(this ILogger<Consumer> self, Exception exception)
        => RemoveRegistrationFromCacheFailedDelegate(self, exception);

    private static readonly Action<ILogger, string, string, Exception?> RegisteringNewConsumerDelegate
        = LoggerMessage.Define<string, string>(logLevel: LogLevel.Information,
                                               eventId: default,
                                               formatString: "Registering new consumer with tag: '{Tag}' and arguments: {Arguments}",
                                               options: null);

    internal static void RegisteringNewConsumer(this ILogger<Consumer> self, string tag, string arguments)
        => RegisteringNewConsumerDelegate(self, tag, arguments, null /* exception */);
}
