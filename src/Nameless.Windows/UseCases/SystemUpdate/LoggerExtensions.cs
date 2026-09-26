using Microsoft.Extensions.Logging;

namespace Nameless.Windows.UseCases.SystemUpdate;

internal static class LoggerExtensions {
    extension(ILogger<SystemUpdateRequestHandler> self) {
        internal void Failure(Exception ex) {
            Log.Failure(
                logger: self,
                tag: "SYSTEM_UPDATE",
                actionName: nameof(SystemUpdateRequestHandler.HandleAsync),
                exception: ex
            );
        }
    }
}