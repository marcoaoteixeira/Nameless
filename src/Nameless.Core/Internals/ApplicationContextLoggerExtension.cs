using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;

namespace Nameless.Internals;

internal static class ApplicationContextLoggerExtension {
    private static readonly Action<ILogger, Exception> CreateDataDirectoryPathFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occured while creating the application data directory.");

    internal static void CreateDataDirectoryPathFailure(this ILogger<ApplicationContext> self, Exception exception) {
        CreateDataDirectoryPathFailureDelegate(self, exception);
    }
}