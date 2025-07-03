using Microsoft.Extensions.Logging;

namespace Nameless.Infrastructure;

internal static class LoggerExtension {
    private static readonly Action<ILogger, Exception> ErrorOnAppDataFolderCreationDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: Events.ErrorOnAppDataFolderCreationEvent,
            formatString: "An error occured while creating the application data folder.");

    internal static void ErrorOnAppDataFolderCreation(this ILogger<ApplicationContext> self, Exception exception) {
        ErrorOnAppDataFolderCreationDelegate(self, exception);
    }

    internal static class Events {
        internal static readonly EventId ErrorOnAppDataFolderCreationEvent = new(1001, nameof(ErrorOnAppDataFolderCreation));
    }
}