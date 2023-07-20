using L4N_Level = log4net.Core.Level;

namespace Nameless.Logging.log4net {
    internal static class LevelTranslator {
        #region Internal Static Methods

        internal static L4N_Level Translate(Level level, bool overrideCriticalLevel = false) {
            return level switch {
                Level.Debug => L4N_Level.Debug,
                Level.Information => L4N_Level.Info,
                Level.Warning => L4N_Level.Warn,
                Level.Error => L4N_Level.Error,
                Level.Critical => overrideCriticalLevel ? L4N_Level.Fatal : L4N_Level.Critical,
                Level.Fatal => L4N_Level.Fatal,
                Level.All => L4N_Level.All,
                _ => L4N_Level.Off
            };
        }

        #endregion
    }
}
