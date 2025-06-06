using Microsoft.Extensions.Logging;

namespace Nameless.Security;

internal static class LoggerExtensions {
    private static readonly Action<ILogger,
        Exception> EncryptionExceptionHandler
        = LoggerMessage.Define(LogLevel.Error,
            default,
            "Error while encryption phase.",
            null);

    private static readonly Action<ILogger,
        Exception> DecryptionExceptionHandler
        = LoggerMessage.Define(LogLevel.Error,
            default,
            "Error while decryption phase.",
            null);

    internal static void EncryptionException(this ILogger self, Exception exception) {
        EncryptionExceptionHandler(self, exception);
    }

    internal static void DecryptionException(this ILogger self, Exception exception) {
        DecryptionExceptionHandler(self, exception);
    }
}