using Microsoft.Extensions.Logging;

namespace Nameless.Security.Internals;
internal static class LoggerHandlers {
    internal static readonly Action<ILogger,
        Exception?> EncryptionException
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while encryption phase.",
                               options: null);

    internal static readonly Action<ILogger,
        Exception?> DecryptionException
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while decryption phase.",
                               options: null);
}
