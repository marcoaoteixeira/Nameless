using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;

namespace Nameless.Internals;

internal static class LoggerHighPerformanceExtension {
    private static readonly Action<ILogger, Exception> ErrorOnDataFolderCreationDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occurs while creating the application data folder.",
                               options: null);

    internal static void ErrorOnDataFolderCreation(this ILogger<ApplicationContext> self, Exception exception)
        => ErrorOnDataFolderCreationDelegate(self, exception);
}