using Microsoft.Extensions.Logging;
using Nameless.WPF.Client.Views.Windows;

namespace Nameless.WPF.Client.Internals;

internal static class MainWindowLoggerExtensions {
    private static readonly Action<ILogger, Exception> SetWindowIconFailureDelegate =
        LoggerMessage.Define(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to set the main window icon."
        );

    extension(ILogger<MainWindow> self) {
        internal void SetWindowIconFailure(Exception exception) {
            SetWindowIconFailureDelegate(self, exception);
        }
    }
}
