using System;

namespace Nameless {
    /// <summary>
    /// Null Object Pattern implementation for IDisposable. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullDisposable : IDisposable {

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of NullDisposable.
        /// </summary>
        public static IDisposable Instance { get; } = new NullDisposable ();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullDisposable () { }

        #endregion

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullDisposable () { }

        #endregion

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose () { }

        #endregion
    }
}