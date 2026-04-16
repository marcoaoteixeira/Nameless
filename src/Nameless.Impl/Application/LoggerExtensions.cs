using Microsoft.Extensions.Logging;

namespace Nameless.Application;

internal static class LoggerExtensions {
    extension(ILogger<ApplicationContext> self) {
        internal void CreateFileSystemProviderFailure(Exception exception) {
            Log.Failure(
                self,
                "APPLICATION_CONTEXT",
                $"{nameof(ApplicationContext)}.CreateFileSystemProvider",
                exception
            );
        }
    }
}