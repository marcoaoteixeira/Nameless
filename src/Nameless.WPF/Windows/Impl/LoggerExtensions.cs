using Microsoft.Extensions.Logging;

namespace Nameless.WPF.Windows.Impl;

internal static class LoggerExtensions {
    private static readonly Action<ILogger, string, Exception?> CreateWindowFailureDelegate
        = LoggerMessage.Define<string>(
            logLevel: LogLevel.Error,
            eventId: default,
            formatString: "An error occurred while trying to create a new instance of the window '{Window}'. The error may be caused by the window not being registered in the dependency container."
        );

    extension(ILogger<WindowFactory> self) {
        internal void CreateWindowFailure(Type windowType, Exception exception) {
            CreateWindowFailureDelegate(self, windowType.Name, exception);
        }
    }
}
