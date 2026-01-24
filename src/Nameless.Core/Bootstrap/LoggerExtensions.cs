using Microsoft.Extensions.Logging;

namespace Nameless.Bootstrap;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, Exception?> BootstrapperInitializingDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Bootstrapper is initializing..."
        );

    private static readonly Action<ILogger, Exception?> BootstrapperSuccessDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Bootstrapper finished successfully."
        );

    private static readonly Action<ILogger, Exception?> BootstrapperCancellationDelegate
        = LoggerMessage.Define(
            logLevel: LogLevel.Warning,
            eventId: default,
            formatString: "Bootstrapper execution cancelled."
        );

    private static readonly Action<ILogger, string, Exception?> StepInitializingDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Step '{StepName}' is initializing..."
        );

    private static readonly Action<ILogger, string, Exception?> StepSuccessDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Debug,
            eventId: default,
            formatString: "Step '{StepName}' finished successfully."
        );

    private static readonly Action<ILogger, string, string, Exception> StepExceptionDelegate
        = LoggerMessage.Define<string, string>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "Step '{StepName}' failed with error: {Error}"
        );

    extension(ILogger<Bootstrapper> self) {
        internal void BootstrapperInitializing() {
            BootstrapperInitializingDelegate(self, null /* exception */);
        }

        internal void BootstrapperSuccess() {
            BootstrapperSuccessDelegate(self, null /* exception */);
        }

        internal void BootstrapperCancellation() {
            BootstrapperCancellationDelegate(self, null /* exception */);
        }

        internal void StepInitializing(IStep step) {
            StepInitializingDelegate(self, step.Name, null /* exception */);
        }

        internal void StepSuccess(IStep step) {
            StepSuccessDelegate(self, step.Name, null /* exception */);
        }

        internal void StepException(IStep step, Exception exception) {
            StepExceptionDelegate(self, step.Name, exception.Message, exception);
        }
    }
}