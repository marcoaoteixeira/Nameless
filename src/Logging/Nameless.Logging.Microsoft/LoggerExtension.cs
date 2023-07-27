using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Logging.Microsoft {
    public static class LoggerExtension {
        #region Public Static Methods

        public static ILogger On(this ILogger self, bool condition)
            => condition ? self : NullLogger.Instance;

        #endregion
    }
}
