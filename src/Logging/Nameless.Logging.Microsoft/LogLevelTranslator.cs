using MS_LogLevel = Microsoft.Extensions.Logging.LogLevel;
using N_LogLevel = Nameless.Logging.LogLevel;

namespace Nameless.Logging.Microsoft {

    internal static class LogLevelTranslator {

        #region Internal Static Methods

        internal static N_LogLevel Translate(MS_LogLevel level, bool overrideCriticalLevel = false) {
            return level switch {
                MS_LogLevel.Debug => N_LogLevel.Debug,
                MS_LogLevel.Information => N_LogLevel.Information,
                MS_LogLevel.Warning => N_LogLevel.Warning,
                MS_LogLevel.Error => N_LogLevel.Error,
                MS_LogLevel.Critical => overrideCriticalLevel ? N_LogLevel.Fatal : N_LogLevel.Critical,
                _ => N_LogLevel.Disabled
            };
        }

        #endregion
    }
}
