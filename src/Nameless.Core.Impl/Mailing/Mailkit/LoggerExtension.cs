using Microsoft.Extensions.Logging;

namespace Nameless.Mailing.Mailkit;

/// <summary>
/// <see cref="ILogger"/> extension methods.
/// </summary>
internal static class LoggerExtension {
    extension(ILogger<MailingService> self) {
        internal void DeliverFailure(Exception exception) {
            Log.Failure(self, "MAILING", nameof(MailingService.DeliverAsync), exception);
        }

        internal void DeliverResult(string result) {
            Log.MailingDeliverResult(self, result);
        }
    }
}