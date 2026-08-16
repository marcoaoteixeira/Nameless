using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Mailing.Mailkit;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGen)]
internal static partial class Log {
    private const string TAG = "MAILING";

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while send email message.")]
    internal static partial void DeliverAsyncFailure(ILogger<MailingService> logger, Exception exception, string tag = TAG);
}