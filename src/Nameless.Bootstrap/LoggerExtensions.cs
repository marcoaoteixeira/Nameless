using Microsoft.Extensions.Logging;
using Nameless.Bootstrap.Execution;
using Nameless.Bootstrap.Resilience;

namespace Nameless.Bootstrap;

internal static class LoggerExtensions {
    #region Bootstrapper

    private static readonly Action<ILogger, Exception?> BootstrapperUnavailableStepsDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] There are no steps available to execute. Probably cause are no steps were registered or there is not a single step enabled."
        );

    private static readonly Action<ILogger, int, Exception?> BootstrapperStartingDelegate
        = LoggerMessage.Define<int>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Starting Bootstrapper with {TotalSteps} available steps..."
        );

    private static readonly Action<ILogger, int, int, Exception?> BootstrapperStepDependencyGraphBuiltDelegate
        = LoggerMessage.Define<int, int>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Step dependency graph built: {LevelCount} execution levels with a total of {StepCount} steps."
        );

    private static readonly Action<ILogger, Exception> BootstrapperFailureDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "[BOOTSTRAP] An error occurred while executing Bootstrapper. See logs for more details."
        );

    private static readonly Action<ILogger, long, int, int, Exception?> BootstrapperFinishedDelegate
        = LoggerMessage.Define<long, int, int>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Bootstrapper took {ElapsedMilliseconds}ms to complete with {SuccessCount}/{TotalSteps} steps executed."
        );

    private static readonly Action<ILogger, string, Exception?> BootstrapperStepStartingDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Executing step: {StepName}"
        );

    private static readonly Action<ILogger, string, Exception> BootstrapperStepFailureDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "[BOOTSTRAP] An error occurred while executing step '{StepName}'."
        );

    private static readonly Action<ILogger, string, long, Exception?> BootstrapperStepFinishedDelegate
        = LoggerMessage.Define<string, long>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Step '{StepName}' took {ElapsedMilliseconds}ms to complete."
        );

    private static readonly Action<ILogger, Exception?> BootstrapperExecutingStepsSequentiallyDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Executing steps in sequence..."
        );

    private static readonly Action<ILogger, int, int, string, Exception?> BootstrapperExecutingStepsStartingDelegate
        = LoggerMessage.Define<int, int, string>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Executing step {Current} of {Total}: '{StepName}'"
        );

    private static readonly Action<ILogger, double, double, double, Exception?> BootstrapperWriteExecutionStatisticsDelegate
        = LoggerMessage.Define<double, double, double>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Execution Statistics => Mean: {Avg:F2}ms, Max: {Max:F2}ms, Min: {Min:F2}ms"
        );

    extension(ILogger<Bootstrapper> self) {
        internal void UnavailableSteps() {
            BootstrapperUnavailableStepsDelegate(self, null /* exception */);
        }

        internal void Starting(int totalSteps) {
            BootstrapperStartingDelegate(self, totalSteps, null /* exception */);
        }

        internal void StepDependencyGraphBuilt(int levels, int steps) {
            BootstrapperStepDependencyGraphBuiltDelegate(self, levels, steps, null /* exception */);
        }

        internal void Failure(Exception exception) {
            BootstrapperFailureDelegate(self, exception);
        }

        internal void Finished(long elapsedMilliseconds, int successCount, int totaSteps) {
            BootstrapperFinishedDelegate(self, elapsedMilliseconds, successCount, totaSteps, null /* exception */);
        }

        internal void StepStarting(string stepName) {
            BootstrapperStepStartingDelegate(self, stepName, null /* exception */);
        }

        internal void StepFailure(string stepName, Exception exception) {
            BootstrapperStepFailureDelegate(self, stepName, exception);
        }

        internal void StepFinished(string stepName, long elapsedMilliseconds) {
            BootstrapperStepFinishedDelegate(self, stepName, elapsedMilliseconds, null /* exception */);
        }

        internal void ExecutingStepsSequentially() {
            BootstrapperExecutingStepsSequentiallyDelegate(self, null /* exception */);
        }

        internal void ExecutingStepsStarting(int current, int total, string stepName) {
            BootstrapperExecutingStepsStartingDelegate(self, current, total, stepName, null /* exception */);
        }

        internal void WriteExecutionStatistics(StepExecutionResult[] results) {
            if (results.Length == 0) { return; }

            var avgDuration = results.Average(result => result.Duration.TotalMilliseconds);
            var maxDuration = results.Max(result => result.Duration.TotalMilliseconds);
            var minDuration = results.Min(result => result.Duration.TotalMilliseconds);

            BootstrapperWriteExecutionStatisticsDelegate(self, avgDuration, maxDuration, minDuration, null /* exception */);
        }
    }

    #endregion

    #region ParallelBootstrapper

    private static readonly Action<ILogger, Exception?> ParallelBootstrapperExecutingStepsInParallelDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Executing steps in parallel..."
        );

    private static readonly Action<ILogger, int, int, int, Exception?> ParallelBootstrapperExecutingStepExecutionLevelDelegate
        = LoggerMessage.Define<int, int, int>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Executing step level {CurrentLevel} of {LevelCount} with {StepCount} step(s)."
        );

    private static readonly Action<ILogger, string, Exception?> ParallelBootstrapperExecutingOneStepInSequenceDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Executing only one step in the current level: '{StepName}'"
        );

    private static readonly Action<ILogger, int, string, Exception?> ParallelBootstrapperExecutingMultipleStepsInParallelDelegate
        = LoggerMessage.Define<int, string>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "[BOOTSTRAP] Executing {Count} steps in the current level (parallel): '{StepNames}'"
        );

    extension(ILogger<ParallelBootstrapper> self) {
        internal void ExecutingStepsInParallel() {
            ParallelBootstrapperExecutingStepsInParallelDelegate(self, null /* exception */);
        }

        internal void ExecutingStepExecutionLevel(int currentLevel, int levelCount, int stepCount) {
            ParallelBootstrapperExecutingStepExecutionLevelDelegate(self, currentLevel, levelCount, stepCount, null /* exception */);
        }

        internal void ExecutingOneStepInSequence(string stepName) {
            ParallelBootstrapperExecutingOneStepInSequenceDelegate(self, stepName, null /* exception */);
        }

        internal void ExecutingMultipleStepsInParallel(string[] stepNames) {
            ParallelBootstrapperExecutingMultipleStepsInParallelDelegate(
                self,
                stepNames.Length,
                string.Join(", ", stepNames),
                null /* exception */
            );
        }
    }

    #endregion

    #region RetryPolicyFactory

    private static readonly Action<ILogger, string, int, int, double, Exception?> RetryPolicyFactoryWarningOnRetryDelegate
        = LoggerMessage.Define<string, int, int, double>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "[BOOTSTRAP] Retrying step '{StepName}' due failure, attempt {CurrentAttempt} of {MaxAttempts}. Waiting delay of {Delay}ms before retry."
        );

    extension(ILogger<RetryPolicyFactory> self) {
        internal void WarningOnRetry(string stepName, int currentAttempt, int maxAttempts, double delay, Exception? exception) {
            RetryPolicyFactoryWarningOnRetryDelegate(self, stepName, currentAttempt, maxAttempts, delay, exception);
        }
    }

    #endregion
}