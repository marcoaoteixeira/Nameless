using Microsoft.Extensions.Logging;

namespace Nameless.Mailing.MailKit.Internals;
internal static class LoggerHandlers {
    internal static readonly Action<ILogger,
        Exception?> SendMessageFailure
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "Error while sending mail.",
                               options: null);
}
