using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Workers;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.InternalCode)]
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

        internal void StatusChanged(string workerName, WorkerStatus status) {
            Log.WorkerStatusChanged(self, "WORKER", workerName, status);
        }
    }
}
