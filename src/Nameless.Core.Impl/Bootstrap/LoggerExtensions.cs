using Microsoft.Extensions.Logging;
using Nameless.Bootstrap.Execution;

namespace Nameless.Bootstrap;

internal static class LoggerExtensions {
    #region Bootstrapper

    extension(ILogger<Bootstrapper> self) {
        #region Bootstrapper

        internal void BootstrapStarting(int totalSteps) {
            Log.BootstrapStarting(self, totalSteps);
        }

        internal void StepDependencyGraphBuilt(int levels, int steps) {
            Log.BootstrapStepDependencyGraphBuilt(self, levels, steps);
        }

        internal void ExecutionMode(string mode) {
            Log.BootstrapExecutionMode(self, mode);
        }

        internal void Failure(Exception exception) {
            Log.Failure(self, "BOOTSTRAP", $"{nameof(Bootstrapper)}.{nameof(Bootstrapper.ExecuteAsync)}", exception);
        }

        internal void BootstrapFinished(long elapsedMilliseconds, int successCount, int totaSteps) {
            Log.BootstrapFinished(self, elapsedMilliseconds, successCount, totaSteps);
        }

        internal void WriteExecutionStatistics(StepExecutionResult[] results) {
            if (results.Length == 0) { return; }

            var avgDuration = results.Average(result => result.Duration.TotalMilliseconds);
            var maxDuration = results.Max(result => result.Duration.TotalMilliseconds);
            var minDuration = results.Min(result => result.Duration.TotalMilliseconds);

            Log.BootstrapWriteExecutionStatistics(self, avgDuration, maxDuration, minDuration);
        }

        #endregion

        #region Step

        internal void CurrentlyExecutingStep(int currentStep, int totalSteps, string stepName) {
            Log.BootstrapCurrentlyExecutingStep(self, currentStep, totalSteps, stepName);
        }

        internal void StepStarting(string stepName) {
            Log.BootstrapStepStarting(self, stepName);
        }

        internal void StepDisabled(string stepName) {
            Log.BootstrapStepDisabled(self, stepName);
        }

        internal void StepFailure(string stepName, Exception exception) {
            Log.BootstrapStepFailure(self, stepName, exception);
        }

        internal void StepFinished(string stepName, long elapsedMilliseconds) {
            Log.BootstrapStepFinished(self, stepName, elapsedMilliseconds);
        }

        #endregion
    }

    #endregion

    #region ParallelBootstrapper

    extension(ILogger<ParallelBootstrapper> self) {
        internal void ExecutingStepInLevel(int currentLevel, int levelCount, int stepCount) {
            Log.BootstrapExecutingStepInLevel(self, currentLevel, levelCount, stepCount);
        }

        internal void ExecutingSingleStepInLevel(string stepName) {
            Log.BootstrapExecutingSingleStepInLevel(self, stepName);
        }

        internal void ExecutingMultipleStepInLevel(string[] stepNames) {
            Log.BootstrapExecutingMultipleStepInLevel(
                self,
                stepNames.Length,
                string.Join(", ", stepNames)
            );
        }
    }

    #endregion
}