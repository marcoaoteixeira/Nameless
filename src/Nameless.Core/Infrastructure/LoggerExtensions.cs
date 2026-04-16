using Microsoft.Extensions.Logging;

namespace Nameless.Infrastructure;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception> CreateFileSystemForDataDirectoryFailureDelegate
        = LoggerMessage.Define(
            LogLevel.Error,
            eventId: default,
            formatString: "An error occured while creating the application data directory.");

    extension(ILogger<ApplicationContext> self) {
        internal void CreateFileSystemForDataDirectoryFailure(Exception exception) {
            CreateFileSystemForDataDirectoryFailureDelegate(self, exception);
        }
    }
}