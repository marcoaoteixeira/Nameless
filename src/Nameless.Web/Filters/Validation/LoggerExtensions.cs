using Microsoft.Extensions.Logging;

namespace Nameless.Web.Filters.Validation;

internal static class LoggerExtensions {
    extension(ILogger<ValidationFilterBase> self) {
        internal void ValidationServiceUnavailable() {
            Log.ValidationServiceUnavailable(self);
        }
    }
}