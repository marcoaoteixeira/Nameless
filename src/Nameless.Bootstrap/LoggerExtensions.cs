using Microsoft.Extensions.Logging;

namespace Nameless.Bootstrap;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> BootstrapperStartingDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Bootstrapper starting..."
        );

    private static readonly Action<ILogger, int, int, long, Exception> BootstrapperExceptionDelegate
        = LoggerMessage.Define<int, int, long>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "Bootstrapper execution failure. Steps executed: {CurrentStep}/{TotalSteps}. Total execution time: {ElapsedMilliseconds}ms"
        );

    private static readonly Action<ILogger, int, int, long, Exception?> BootstrapperCancelledDelegate
        = LoggerMessage.Define<int, int, long>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Bootstrapper execution cancelled. Steps executed: {CurrentStep}/{TotalSteps}. Total execution time: {ElapsedMilliseconds}ms"
        );

    private static readonly Action<ILogger, int, int, long, Exception?> BootstrapperSuccessDelegate
        = LoggerMessage.Define<int, int, long>(
            logLevel: LogLevel.Information,
            eventId: default,
            formatString: "Bootstrapper finished successfully. Steps executed: {CurrentStep}/{TotalSteps}. Total execution time: {ElapsedMilliseconds}ms"
        );

    private static readonly Action<ILogger, int, int, string, Exception?> StepStartingDelegate
        = LoggerMessage.Define<int, int, string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "[{CurrentStep}/{TotalSteps}] Step '{StepName}' starting..."
        );

    private static readonly Action<ILogger, int, int, string, long, Exception?> StepCancelledDelegate
        = LoggerMessage.Define<int, int, string, long>(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "[{CurrentStep}/{TotalSteps}] Step '{StepName}' cancelled. Total execution time: {ElapsedMilliseconds}ms"
        );

    private static readonly Action<ILogger, int, int, string, long, Exception> StepExceptionDelegate
        = LoggerMessage.Define<int, int, string, long>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "[{CurrentStep}/{TotalSteps}] Step '{StepName}' failure. Total execution time: {ElapsedMilliseconds}ms"
        );

    private static readonly Action<ILogger, int, int, string, long, Exception?> StepSuccessDelegate
        = LoggerMessage.Define<int, int, string, long>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "[{CurrentStep}/{TotalSteps}] Step '{StepName}' finished successfully. Total execution time: {ElapsedMilliseconds}ms"
        );

    extension(ILogger<Bootstrapper> self) {
        internal void BootstrapperStarting() {
            BootstrapperStartingDelegate(self, null /* exception */);
        }

        internal void BootstrapperException(int currentStep, int totalSteps, long elapsedMilliseconds, Exception exception) {
            BootstrapperExceptionDelegate(self, currentStep, totalSteps, elapsedMilliseconds, exception);
        }

        internal void BootstrapperCancelled(int currentStep, int totalSteps, long elapsedMilliseconds) {
            BootstrapperCancelledDelegate(self, currentStep, totalSteps, elapsedMilliseconds, null /* exception */);
        }

        internal void BootstrapperSuccess(int currentStep, int totalSteps, long elapsedMilliseconds) {
            BootstrapperSuccessDelegate(self, currentStep, totalSteps, elapsedMilliseconds, null /* exception */);
        }

        internal void StepStarting(IStep step, int currentStep, int totalSteps) {
            StepStartingDelegate(self, currentStep, totalSteps, step.Name, null /* exception */);
        }

        internal void StepCancelled(IStep step, int currentStep, int totalSteps, long elapsedMilliseconds) {
            StepCancelledDelegate(self, currentStep, totalSteps, step.Name, elapsedMilliseconds, null /* exception */);
        }

        internal void StepException(IStep step, int currentStep, int totalSteps, long elapsedMilliseconds, Exception exception) {
            StepExceptionDelegate(self, currentStep, totalSteps, step.Name, elapsedMilliseconds, exception);
        }

        internal void StepSuccess(IStep step, int currentStep, int totalSteps, long elapsedMilliseconds) {
            StepSuccessDelegate(self, currentStep, totalSteps, step.Name, elapsedMilliseconds, null /* exception */);
        }
    }
}