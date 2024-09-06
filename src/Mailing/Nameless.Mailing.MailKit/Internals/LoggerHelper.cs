using Microsoft.Extensions.Logging;

namespace Nameless.Mailing.MailKit.Internals;
internal static class LoggerHelper {
    internal static Action<ILogger, Exception?> SendMessageFailure = LoggerMessage.Define(
        logLevel: LogLevel.Error,
        eventId: default,
        formatString: "Error while sending mail.",
        options: null
    );
}
