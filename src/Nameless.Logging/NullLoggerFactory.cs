using System;

namespace Nameless.Logging {

    /// <summary>
    /// Null Object Pattern implementation for ILoggerFactory. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullLoggerFactory : ILoggerFactory {

        #region Private Static Read-Only Fields

        private static readonly ILoggerFactory _instance = new NullLoggerFactory();

        #endregion Private Static Read-Only Fields

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of NullLoggerFactory.
        /// </summary>
        public static ILoggerFactory Instance => _instance;

        #endregion Public Static Properties

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullLoggerFactory() {
        }

        #endregion Static Constructors

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullLoggerFactory() {
        }

        #endregion Private Constructors

        #region ILoggerFactory Members

        /// <inheritdoc />
        public ILogger CreateLogger(Type type) => NullLogger.Instance;

        #endregion ILoggerFactory Members
    }
}