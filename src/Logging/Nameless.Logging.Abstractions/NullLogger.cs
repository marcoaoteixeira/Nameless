namespace Nameless.Logging {
    [Singleton]
    public sealed class NullLogger : ILogger {
        #region Private Static Read-Only Fields

        private static readonly NullLogger _instance = new();

        #endregion

        #region Public Static Read-Only Properties

        /// <summary>
        /// Gets the unique instance of NullLogger.
        /// </summary>
        public static ILogger Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullLogger() { }

        #endregion

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullLogger() { }

        #endregion Private Constructors

        #region ILogger Members

        /// <inheritdoc />
        public bool IsEnabled(Level level) => false;

        /// <inheritdoc />
        public void Log(Level logLevel, string message, Exception? exception = default, params object[] args) { }

        #endregion
    }
}
