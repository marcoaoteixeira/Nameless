using System;

namespace Nameless.FileProvider {
    /// <summary>
    /// Null Object Pattern implementation for IWatchToken. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullWatchToken : IWatchToken {

        #region Private Static Read-Only Fields

        private static readonly IWatchToken _instance = new NullWatchToken ();

        #endregion Private Static Read-Only Fields

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of NullWatchToken.
        /// </summary>
        public static IWatchToken Instance => _instance;

        #endregion Public Static Properties

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullWatchToken () { }

        #endregion Static Constructors

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullWatchToken () { }

        #endregion Private Constructors

        #region IWatchToken Members

        /// <inheritdoc />
        public bool Changed => false;
        /// <inheritdoc />
        public bool ActiveCallback { get; set; }

        /// <inheritdoc />
        public IDisposable RegisterCallback (Action<object> callback, object state) {
            return NullDisposable.Instance;
        }

        #endregion IWatchToken Members
    }
}