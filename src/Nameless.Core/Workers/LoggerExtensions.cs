using Microsoft.Extensions.Logging;

namespace Nameless.Workers;

internal static class LoggerExtensions {
    extension(ILogger<Worker> self) {
        internal void Failure(Exception exception) {
            Log.Failure(
                self,
                "WORKER",
                $"{nameof(Worker)}.{nameof(Worker.DoWorkAsync)}",
                exception
            );
        }
    }
}
