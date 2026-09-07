using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Notification;
using Nameless.Reporting;

namespace Nameless.Bootstrap;

internal static class StatusReporterExtensions {
    extension(IStatusReporter<Bootstrapper> self) {
        internal void ReportBootstrapperStarting() {
            self.ReportInfo(message: "Bootstrapper execution starting...");
        }

        internal void ReportBootstrapperComplete() {
            self.ReportInfo(message: "Bootstrapper execution completed.");
            self.Complete();
        }

        internal void ReportBootstrapperFault(Exception exception) {
            self.ReportError(message: "One or more steps failed.");
            self.Fault(exception);
        }

        internal void ReportStepProgress(StepProgress progress) {
            self.ReportInfo(message: $"{progress.StepName}: {progress.Message}");
        }
    }
}
