using System.Net.Security;
using Microsoft.Extensions.Logging;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;
internal static class LoggerExtension {
    private static readonly Action<ILogger,
        Exception?> MessagePublishErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while publishing message.",
                               options: null);

    internal static void MessagePublishError(this ILogger self, Exception exception)
        => MessagePublishErrorHandler(self, exception);

    private static readonly Action<ILogger,
        SslPolicyErrors,
        Exception?> CertificateValidationFailureHandler
        = LoggerMessage.Define<SslPolicyErrors>(logLevel: LogLevel.Warning,
                                               eventId: default,
                                               formatString: "Certificate validation error: {SSLPolicyErrors}",
                                               options: null);

    internal static void CertificateValidationFailure(this ILogger self, SslPolicyErrors sslPolicyErrors)
        => CertificateValidationFailureHandler(self, sslPolicyErrors, null /* exception */);

    private static readonly Action<ILogger,
        string,
        Exception?> ConsumerRegistrationHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Initialize registration of consumer: {Tag}",
                                       options: null);

    internal static void ConsumerRegistration(this ILogger self, string tag)
        => ConsumerRegistrationHandler(self, tag, null /* exception */);

    private static readonly Action<ILogger,
        string,
        Exception?> ConsumerArgumentsHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Consumer arguments: {Arguments}",
                                       options: null);

    internal static void ConsumerArguments(this ILogger self, string arguments)
        => ConsumerArgumentsHandler(self, arguments, null /* exception */);

    private static readonly Action<ILogger,
        Exception?> ConsumerHandlerCreationFailureHandler
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "Consumer handler creation failed.",
                               options: null);

    internal static void ConsumerHandlerCreationFailure(this ILogger self, Exception exception)
        => ConsumerHandlerCreationFailureHandler(self, exception);

    private static readonly Action<ILogger,
        Exception?> MessageHandlerNotFoundHandler
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "No suitable message handler found.",
                               options: null);

    internal static void MessageHandlerNotFound(this ILogger self)
        => MessageHandlerNotFoundHandler(self, null /* exception */);

    private static readonly Action<ILogger,
        Exception?> EnvelopeDeserializationExceptionHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Envelope deserialization throws exception.",
                               options: null);

    internal static void EnvelopeDeserializationException(this ILogger self, Exception exception)
        => EnvelopeDeserializationExceptionHandler(self, exception);

    private static readonly Action<ILogger,
        Exception?> EnvelopeDeserializationFailureHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Envelope deserialization failed.",
                               options: null);

    internal static void EnvelopeDeserializationFailure(this ILogger self)
        => EnvelopeDeserializationFailureHandler(self, null /* exception */);

    private static readonly Action<ILogger,
        Exception?> EnvelopeMessageNotValidJsonElementHandler
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "Message was not valid JSON.",
                               options: null);

    internal static void EnvelopeMessageNotValidJsonElement(this ILogger self)
        => EnvelopeMessageNotValidJsonElementHandler(self, null /* exception */);

    private static readonly Action<ILogger,
        Exception?> MessageDeserializationExceptionHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Message deserialization throws exception.",
                               options: null);

    internal static void MessageDeserializationException(this ILogger self, Exception exception)
        => MessageDeserializationExceptionHandler(self, exception);

    private static readonly Action<ILogger,
        string,
        Exception?> MessageDeserializationFailureHandler
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "Unable to deserialize the message to expecting type {MessageType}.",
                                       options: null);

    internal static void MessageDeserializationFailure<T>(this ILogger self)
        => MessageDeserializationFailureHandler(self, typeof(T).Name, null /* exception */);

    private static readonly Action<ILogger,
        Exception?> MessageHandlerExceptionHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error when handling the message.",
                               options: null);

    internal static void MessageHandlerException(this ILogger self, Exception exception)
        => MessageHandlerExceptionHandler(self, exception);
}
