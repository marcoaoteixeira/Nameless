using Microsoft.Extensions.Logging;

namespace Nameless.Mailing.MailKit.Internals;
internal static class LoggerExtension {
    private static readonly Action<ILogger, Exception> SendMessageErrorDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurred while sending the email.",
                               options: null);

    internal static void SendMessageError(this ILogger self, Exception exception)
        => SendMessageErrorDelegate(self, exception);

    private static readonly Action<ILogger, string, Exception?> EmailSentResultDelegate
        = LoggerMessage.Define<string>(logLevel: LogLevel.Information,
                                       eventId: default,
                                       formatString: "The message was sent and the result was: {Result}",
                                       options: null);

    internal static void EmailSentResult(this ILogger self, string result)
        => EmailSentResultDelegate(self, result, null /* exception */);
}
