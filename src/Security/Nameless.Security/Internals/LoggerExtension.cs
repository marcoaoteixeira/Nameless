using Microsoft.Extensions.Logging;

namespace Nameless.Security.Internals;
internal static class LoggerExtension {
    private static readonly Action<ILogger,
        Exception> EncryptionExceptionHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while encryption phase.",
                               options: null);

    internal static void EncryptionException(this ILogger self, Exception exception)
        => EncryptionExceptionHandler(self, exception);

    private static readonly Action<ILogger,
        Exception> DecryptionExceptionHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while decryption phase.",
                               options: null);

    internal static void DecryptionException(this ILogger self, Exception exception)
        => DecryptionExceptionHandler(self, exception);
}
