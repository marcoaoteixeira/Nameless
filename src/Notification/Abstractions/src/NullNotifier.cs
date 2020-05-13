using System;
using System.Collections.Generic;

namespace Nameless.Notification {

    /// <summary>
    /// Null Object Pattern implementation for INotifier. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullNotifier : INotifier {

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of NullNotifier.
        /// </summary>
        public static INotifier Instance { get; } = new NullNotifier ();

        #endregion Public Static Properties

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullNotifier () { }

        #endregion Static Constructors

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullNotifier () { }

        #endregion Private Constructors

        #region INotifier Members

        /// <inheritdoc />
        public void Add (NotifyType type, string message) { }

        /// <inheritdoc />
        public IEnumerable<NotifyEntry> Flush () => Array.Empty<NotifyEntry> ();

        #endregion INotifier Members
    }
}