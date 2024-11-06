using Microsoft.Extensions.Logging;

namespace Nameless.Mailing.MailKit.Internals;
internal static class LoggerExtension {
    private static readonly Action<ILogger,
        Exception> SendMessageErrorHandler
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while sending mail.",
                               options: null);

    internal static void SendMessageError(this ILogger self, Exception exception)
        => SendMessageErrorHandler(self, exception);
}
