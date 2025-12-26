using Microsoft.Extensions.Logging;

namespace Nameless.Security.Internals;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> EncryptionExceptionHandler
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.EncryptionExceptionEvent,
            formatString: "Error while encryption phase.");

    private static readonly Action<ILogger, Exception> DecryptionExceptionHandler
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.DecryptionExceptionEvent,
            formatString: "Error while decryption phase.");

    extension(ILogger self) {
        internal void EncryptionException(Exception exception) {
            EncryptionExceptionHandler(self, exception);
        }

        internal void DecryptionException(Exception exception) {
            DecryptionExceptionHandler(self, exception);
        }
    }

    internal static class Events {
        internal static readonly EventId EncryptionExceptionEvent = new(id: 10001, nameof(EncryptionException));
        internal static readonly EventId DecryptionExceptionEvent = new(id: 10002, nameof(DecryptionException));
    }
}