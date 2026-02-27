namespace Nameless.Bootstrap.Notification;

public static class StepProgressExtensions {
    extension(IProgress<StepProgress> self) {
        public void ReportInformation(string stepName, string message, Dictionary<string, object>? metadata = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Information,
                StepName = stepName,
                Message = message,
                Metadata = metadata,
            });
        }

        public void ReportComplete(string stepName, string? message = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Complete,
                StepName = stepName,
                Message = message ?? $"Step '{stepName}' completed.",
                PercentComplete = 100
            });
        }

        public void ReportFailure(string stepName, string message, Exception? exception = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Failure,
                StepName = stepName,
                Message = message,
                Metadata = exception is not null
                    ? new Dictionary<string, object> {
                        ["Exception"] = exception.Message,
                        ["ExceptionType"] = exception.GetType().Name
                    }
                    : []
            });
        }

        public void ReportRetrying(string stepName, int attempt, int maxAttempts, TimeSpan delay) {
            self.Report(new StepProgress {
                Type = StepProgressType.Retrying,
                StepName = stepName,
                Message = $"Retrying '{stepName}', attempt {attempt}/{maxAttempts}: waiting {delay.TotalSeconds:F1}s...",
                Metadata = new Dictionary<string, object> {
                    ["Attempt"] = attempt,
                    ["MaxAttempts"] = maxAttempts,
                    ["DelaySeconds"] = delay.TotalSeconds
                }
            });
        }

        public void ReportRunning(string stepName, string message, int? percentComplete = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Running,
                StepName = stepName,
                Message = message,
                PercentComplete = percentComplete
            });
        }

        public void ReportStart(string stepName, string? message = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Start,
                StepName = stepName,
                Message = message ?? $"Starting step '{stepName}'...",
                PercentComplete = 0
            });
        }
    }
}
