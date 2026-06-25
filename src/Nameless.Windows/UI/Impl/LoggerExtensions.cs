using Microsoft.Extensions.Logging;

namespace Nameless.Windows.UI.Impl;

internal static class LoggerExtensions {
    extension(ILogger<WindowFactory> self) {
        internal void CreateFailure(Type windowType, Exception exception) {
            Log.WindowFactoryCreateFailure(self, windowType, exception);
        }
    }
}
