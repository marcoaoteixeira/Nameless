using Microsoft.Extensions.Logging;

namespace Nameless.Infrastructure;

internal static class LoggerExtension {
    private static readonly Action<ILogger, Exception> ErrorOnAppDataFolderCreationDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: new EventId((int)Events.ErrorOnAppDataFolderCreation, nameof(ErrorOnAppDataFolderCreation)),
            formatString: "An error occured while creating the application data folder.",
            options: null);

    internal static void ErrorOnAppDataFolderCreation(this ILogger<ApplicationContext> self, Exception exception) {
        ErrorOnAppDataFolderCreationDelegate(self, exception);
    }

    private enum Events {
        ErrorOnAppDataFolderCreation = 1
    }
}