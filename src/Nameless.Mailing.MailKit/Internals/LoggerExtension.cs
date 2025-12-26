using Microsoft.Extensions.Logging;

namespace Nameless.Mailing.MailKit.Internals;

/// <summary>
/// <see cref="ILogger"/> extension methods.
/// </summary>
internal static class LoggerExtension {
    private static readonly Action<ILogger, Exception> SendMessageErrorDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            Events.SendMessageErrorEvent,
            formatString: "An error occurred while sending the email.");

    private static readonly Action<ILogger, string, Exception?> EmailSentResultDelegate
        = LoggerMessage.Define<string>(
            LogLevel.Information,
            Events.EmailSentResultEvent,
            formatString: "The message was sent and the result is: {Result}");

    extension(ILogger self) {
        internal void SendMessageError(Exception exception) {
            SendMessageErrorDelegate(self, exception);
        }

        internal void EmailSentResult(string result) {
            EmailSentResultDelegate(self, result, arg3: null /* exception */);
        }
    }

    internal static class Events {
        internal static readonly EventId SendMessageErrorEvent = new(id: 5001, nameof(SendMessageError));
        internal static readonly EventId EmailSentResultEvent = new(id: 5002, nameof(EmailSentResult));
    }
}