using Microsoft.Extensions.Logging;

namespace Nameless.Security.Internals;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> EncryptionExceptionHandler
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.EncryptionExceptionEvent,
            formatString: "Error while encryption phase.");

    private static readonly Action<ILogger, Exception> DecryptionExceptionHandler
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.DecryptionExceptionEvent,
            formatString: "Error while decryption phase.");

    internal static void EncryptionException(this ILogger self, Exception exception) {
        EncryptionExceptionHandler(self, exception);
    }

    internal static void DecryptionException(this ILogger self, Exception exception) {
        DecryptionExceptionHandler(self, exception);
    }

    internal static class Events {
        internal static readonly EventId EncryptionExceptionEvent = new(10001, nameof(EncryptionException));
        internal static readonly EventId DecryptionExceptionEvent = new(10002, nameof(DecryptionException));
    }
}