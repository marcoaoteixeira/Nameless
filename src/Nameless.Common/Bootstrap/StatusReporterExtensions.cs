using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Notification;
using Nameless.Reporting;

namespace Nameless.Bootstrap;

internal static class StatusReporterExtensions {
    extension(IStatusReporter<Bootstrapper> self) {
        internal void ReportStart(DateTimeOffset time) {
            self.ReportInfo(
                GetFullMessage(time, "Starting...")
            );
        }

        internal void Fault(DateTimeOffset time, StepExecutionResult[] results) {
            if (results.Length == 0) { return; }

            var sb = new StringBuilder();

            sb.AppendLine(
                GetFullMessage(time, "Bootstrapper runs into some errors:")
            );

            foreach (var result in results) {
                sb.AppendLine(result.ToText());
            }

            self.ReportError(
                GetFullMessage(time, sb.ToString())
            );

            self.Fault(reason: "One or more steps failed.");
        }

        internal void Complete(DateTimeOffset time) {
            self.ReportInfo(
                GetFullMessage(time, "Finished!")
            );

            self.Complete();
        }

        internal void ReportStepStarting(DateTimeOffset time, IStep step) {
            self.ReportInfo(
                GetFullMessage(time, $"Step '{step.DisplayName}' starting...")
            );
        }

        internal void ReportStepProgress(DateTimeOffset time, StepProgress progress) {
            self.ReportInfo(
                GetFullMessage(time, progress.ToText())
            );
        }

        internal void ReportStepFailure(DateTimeOffset time, IStep step, Exception ex) {
            self.ReportError(
                GetFullMessage(time, $"Step '{step.DisplayName}' failed with error: {ex.Message}")
            );
        }

        internal void ReportStepFinish(DateTimeOffset time, IStep step) {
            self.ReportInfo(
                GetFullMessage(time, $"Step '{step.DisplayName}' finished!")
            );
        }
    }

    private static string GetFullMessage(DateTimeOffset time, string message) {
        return $"[BOOTSTRAPPER] {time:yyyy-MM-dd HH:mm:ss:fff} - {message}";
    }
}
