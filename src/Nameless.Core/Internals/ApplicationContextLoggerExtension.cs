using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;

namespace Nameless.Internals;

internal static class ApplicationContextLoggerExtension {
    private static readonly Action<ILogger, Exception> CreateDataDirectoryPathFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occured while creating the application data directory.");

    extension(ILogger<ApplicationContext> self) {
        internal void CreateDataDirectoryPathFailure(Exception exception) {
            CreateDataDirectoryPathFailureDelegate(self, exception);
        }
    }
}