using System.Net.Security;
using Microsoft.Extensions.Logging;

namespace Nameless.ProducerConsumer.RabbitMQ.Internals;
internal static class LoggerHandlers {
    internal static readonly Action<ILogger,
        Exception?> MessagePublishError
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while publishing message.",
                               options: null);

    internal static readonly Action<ILogger,
        SslPolicyErrors,
        Exception?> CertificateValidationFailure
        = LoggerMessage.Define<SslPolicyErrors>(logLevel: LogLevel.Warning,
                                               eventId: default,
                                               formatString: "Certificate validation error: {SSLPolicyErrors}",
                                               options: null);

    internal static readonly Action<ILogger,
        string,
        Exception?> ConsumerRegistration
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Initialize registration of consumer: {Tag}",
                                       options: null);

    internal static readonly Action<ILogger,
        string,
        Exception?> ConsumerArguments
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "Consumer arguments: {Arguments}",
                                       options: null);

    internal static readonly Action<ILogger,
        Exception?> ConsumerHandlerCreationFailure
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "Consumer handler creation failed.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception?> MessageHandlerNotFound
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "No suitable message handler found.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception?> EnvelopeDeserializationException
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Envelope deserialization throws exception.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception?> EnvelopeDeserializationFailure
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Envelope deserialization failed.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception?> EnvelopeMessageNotValidJsonElement
        = LoggerMessage.Define(logLevel: LogLevel.Warning,
                               eventId: default,
                               formatString: "Message was not valid JSON.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception?> MessageDeserializationException
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Message deserialization throws exception.",
                               options: null);

    internal static readonly Action<ILogger,
        string,
        Exception?> MessageDeserializationFailure
        = LoggerMessage.Define<string>(logLevel: LogLevel.Warning,
                                       eventId: default,
                                       formatString: "Unable to deserialize the message to expecting type {MessageType}.",
                                       options: null);

    internal static readonly Action<ILogger,
        Exception?> MessageHandlerException
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error when handling the message.",
                               options: null);
}
