namespace Nameless.Workers.Notification;

/// <summary>
///     Convenience extension methods for reporting <see cref="WorkerProgress" />
///     notifications from within a <see cref="Worker" /> implementation.
/// </summary>
public static class WorkerProgressExtensions {
    extension(Worker self) {
        /// <summary>
        ///     Reports a <see cref="WorkerProgressType.TickStarted" /> notification.
        /// </summary>
        /// <param name="message">Optional custom message.</param>
        public void ReportTickStarted(string? message = null) {
            self.ReportProgress(new WorkerProgress {
                Type = WorkerProgressType.TickStarted,
                WorkerName = self.Name,
                Message = message ?? $"Worker '{self.Name}' tick started.",
                PercentageComplete = 0,
                Timestamp = DateTimeOffset.UtcNow,
            });
        }

        /// <summary>
        ///     Reports a <see cref="WorkerProgressType.TickCompleted" /> notification.
        /// </summary>
        /// <param name="message">Optional custom message.</param>
        public void ReportTickCompleted(string? message = null) {
            self.ReportProgress(new WorkerProgress {
                Type = WorkerProgressType.TickCompleted,
                WorkerName = self.Name,
                Message = message ?? $"Worker '{self.Name}' tick completed.",
                PercentageComplete = 100,
                Timestamp = DateTimeOffset.UtcNow,
            });
        }

        /// <summary>
        ///     Reports a <see cref="WorkerProgressType.TickFailed" /> notification.
        /// </summary>
        /// <param name="exception">The exception that caused the failure.</param>
        /// <param name="message">Optional custom message.</param>
        public void ReportTickFailed(Exception exception, string? message = null) {
            Throws.When.Null(exception);

            self.ReportProgress(new WorkerProgress {
                Type = WorkerProgressType.TickFailed,
                WorkerName = self.Name,
                Message = message ?? $"Worker '{self.Name}' tick failed.",
                Metadata = new Dictionary<string, object> {
                    ["Exception"] = exception.Message,
                    ["ExceptionType"] = exception.GetType().Name,
                },
                Timestamp = DateTimeOffset.UtcNow,
            });
        }

        /// <summary>
        ///     Reports a <see cref="WorkerProgressType.Information" /> notification.
        /// </summary>
        /// <param name="message">The informational message.</param>
        /// <param name="percentageComplete">Optional completion percentage.</param>
        /// <param name="metadata">Optional additional metadata.</param>
        public void ReportInformation(string message, int? percentageComplete = null, Dictionary<string, object>? metadata = null) {
            self.ReportProgress(new WorkerProgress {
                Type = WorkerProgressType.Information,
                WorkerName = self.Name,
                Message = message,
                PercentageComplete = percentageComplete,
                Metadata = metadata,
                Timestamp = DateTimeOffset.UtcNow,
            });
        }

        /// <summary>
        ///     Reports a <see cref="WorkerProgressType.Cancelled" /> notification.
        /// </summary>
        /// <param name="message">Optional custom message.</param>
        public void ReportCancelled(string? message = null) {
            self.ReportProgress(new WorkerProgress {
                Type = WorkerProgressType.Cancelled,
                WorkerName = self.Name,
                Message = message ?? $"Worker '{self.Name}' was cancelled.",
                Timestamp = DateTimeOffset.UtcNow,
            });
        }
    }
}
