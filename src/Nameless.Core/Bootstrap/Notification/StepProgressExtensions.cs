namespace Nameless.Bootstrap.Notification;

public static class StepProgressExtensions {
    extension(IProgress<StepProgress> self) {
        /// <summary>
        ///     Reports an information message during step execution.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="message">The message.</param>
        /// <param name="metadata">The metadata, if any.</param>
        public void ReportInformation(string stepName, string message, Dictionary<string, object>? metadata = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Information,
                StepName = stepName,
                Message = message,
                Metadata = metadata,
                Timestamp = DateTimeOffset.UtcNow
            });
        }

        /// <summary>
        ///     Reports the step completion.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="message">The message.</param>
        public void ReportComplete(string stepName, string? message = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Complete,
                StepName = stepName,
                Message = message ?? $"Step '{stepName}' completed.",
                PercentageComplete = 100,
                Timestamp = DateTimeOffset.UtcNow
            });
        }

        /// <summary>
        ///     Reports the step failure.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
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
                    : [],
                Timestamp = DateTimeOffset.UtcNow
            });
        }

        /// <summary>
        ///     Reports a step retrying execution.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="attempt">The current attempt number.</param>
        /// <param name="maxAttempts">The maximum number of attempts.</param>
        /// <param name="delay">The delay between attempts.</param>
        public void ReportRetrying(string stepName, int attempt, int maxAttempts, TimeSpan delay) {
            self.Report(new StepProgress {
                Type = StepProgressType.Retrying,
                StepName = stepName,
                Message = $"Retrying '{stepName}', attempt {attempt}/{maxAttempts}: waiting {delay.TotalSeconds:F1}s...",
                Metadata = new Dictionary<string, object> {
                    ["Attempt"] = attempt,
                    ["MaxAttempts"] = maxAttempts,
                    ["DelaySeconds"] = delay.TotalSeconds
                },
                Timestamp = DateTimeOffset.UtcNow
            });
        }

        /// <summary>
        ///     Reports the current step execution run.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="message">The message.</param>
        /// <param name="percentageComplete">The percentage completed so far.</param>
        public void ReportRunning(string stepName, string message, int? percentageComplete = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Running,
                StepName = stepName,
                Message = message,
                PercentageComplete = percentageComplete,
                Timestamp = DateTimeOffset.UtcNow
            });
        }

        /// <summary>
        ///     Reports the step starting.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="message">The message.</param>
        public void ReportStart(string stepName, string? message = null) {
            self.Report(new StepProgress {
                Type = StepProgressType.Start,
                StepName = stepName,
                Message = message ?? $"Starting step '{stepName}'...",
                PercentageComplete = 0,
                Timestamp = DateTimeOffset.UtcNow
            });
        }
    }
}
