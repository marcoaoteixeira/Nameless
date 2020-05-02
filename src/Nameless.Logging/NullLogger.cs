using System;

namespace Nameless.Logging {

    /// <summary>
    /// Null Object Pattern implementation for ILogger. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullLogger : ILogger {

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of NullLogger.
        /// </summary>
        public static ILogger Instance { get; } = new NullLogger ();

        #endregion Public Static Properties

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullLogger () { }

        #endregion Static Constructors

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullLogger () { }

        #endregion Private Constructors

        #region ILogger Members

        /// <inheritdoc />
        public bool IsEnabled (LogLevel level) => false;

        /// <inheritdoc />
        public void Log (LogLevel level, Exception exception, string format, params object[] args) { }

        #endregion ILogger Members
    }
}