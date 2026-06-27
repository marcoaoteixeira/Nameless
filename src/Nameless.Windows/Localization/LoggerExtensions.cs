using Microsoft.Extensions.Logging;

namespace Nameless.Windows.Localization;

internal static class LoggerExtensions {
    extension(ILogger<ResourceLocalizer> self) {
        internal void MissingKey(string key) {
            Log.ResourceLocalizerMissingKey(self, key);
        }
    }
}
