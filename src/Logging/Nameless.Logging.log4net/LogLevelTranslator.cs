using L4N_Level = log4net.Core.Level;

namespace Nameless.Logging.log4net {

    internal static class LogLevelTranslator {

        #region Internal Static Methods

        internal static L4N_Level Translate(LogLevel level, bool overrideCriticalLevel = false) {
            return level switch {
                LogLevel.Debug => L4N_Level.Debug,
                LogLevel.Information => L4N_Level.Info,
                LogLevel.Warning => L4N_Level.Warn,
                LogLevel.Error => L4N_Level.Error,
                LogLevel.Critical => overrideCriticalLevel ? L4N_Level.Fatal : L4N_Level.Critical,
                LogLevel.Fatal => L4N_Level.Fatal,
                _ => L4N_Level.Off
            };
        }

        #endregion
    }
}
