using Microsoft.Extensions.Logging;
using Nameless.Application;

namespace Nameless.Internals;

internal static class LoggerExtension {
    private static readonly Action<ILogger, Exception> ErrorOnAppDataFolderCreationDelegate
        = LoggerMessage.Define(logLevel: LogLevel.Error,
                               eventId: default,
                               formatString: "An error occured while creating the application data folder.",
                               options: null);

    internal static void ErrorOnAppDataFolderCreation(this ILogger<ApplicationContext> self, Exception exception)
        => ErrorOnAppDataFolderCreationDelegate(self, exception);
}