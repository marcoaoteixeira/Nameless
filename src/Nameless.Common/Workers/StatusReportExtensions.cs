using Nameless.Reporting;

namespace Nameless.Workers;

internal static class StatusReportExtensions {
    extension(IStatusReporter self) {
        internal void Idle(PeriodicWorker worker) {
            self.ReportInfo($"Periodic worker '{worker.Name}' is idle.");
        }

        internal void Running(PeriodicWorker worker) {
            self.ReportInfo($"Periodic worker '{worker.Name}' is running.");
        }

        internal void Cancelled(PeriodicWorker worker) {
            self.ReportInfo($"Periodic worker '{worker.Name}' operation was cancelled.");
        }

        internal void Stop(PeriodicWorker worker) {
            self.ReportInfo($"Periodic worker '{worker.Name}' has stopped.");
        }
    }
}
