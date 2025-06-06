using Microsoft.Extensions.Logging;

namespace Nameless.Mailing.MailKit;

/// <summary>
/// <see cref="ILogger"/> extension methods.
/// </summary>
internal static class LoggerExtension {
    private static readonly Action<ILogger, Exception> SendMessageErrorDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.SendMessageErrorEvent,
            formatString: "An error occurred while sending the email.");

    private static readonly Action<ILogger, string, Exception?> EmailSentResultDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: Events.EmailSentResultEvent,
            formatString: "The message was sent and the result is: {Result}");

    internal static void SendMessageError(this ILogger self, Exception exception) {
        SendMessageErrorDelegate(self, exception);
    }

    internal static void EmailSentResult(this ILogger self, string result) {
        EmailSentResultDelegate(self, result, null /* exception */);
    }

    internal static class Events {
        internal static readonly EventId SendMessageErrorEvent = new(1, nameof(SendMessageError));
        internal static readonly EventId EmailSentResultEvent = new(2, nameof(EmailSentResult));
    }
}