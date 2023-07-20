namespace Nameless.Logging.Microsoft {
    internal static class LevelTranslator {
        #region Internal Static Methods

        internal static Level Translate(MSLogLevel level, bool overrideCriticalLevel = false) {
            return level switch {
                MSLogLevel.Debug => Level.Debug,
                MSLogLevel.Information => Level.Information,
                MSLogLevel.Warning => Level.Warning,
                MSLogLevel.Error => Level.Error,
                MSLogLevel.Critical => overrideCriticalLevel ? Level.Fatal : Level.Critical,
                MSLogLevel.Trace => Level.All,
                _ => Level.None
            };
        }

        #endregion
    }
}
