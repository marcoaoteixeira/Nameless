using Microsoft.Extensions.Logging;

namespace Nameless.Logging.Microsoft {
    internal static class LevelTranslator {
        #region Internal Static Methods

        internal static Level Translate(LogLevel level, bool overrideCriticalLevel = false) {
            return level switch {
                LogLevel.Debug => Level.Debug,
                LogLevel.Information => Level.Information,
                LogLevel.Warning => Level.Warning,
                LogLevel.Error => Level.Error,
                LogLevel.Critical => overrideCriticalLevel ? Level.Fatal : Level.Critical,
                LogLevel.Trace => Level.All,
                _ => Level.None
            };
        }

        #endregion
    }
}
