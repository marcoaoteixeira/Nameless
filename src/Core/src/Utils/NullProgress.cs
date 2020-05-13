using System;

namespace Nameless {
    /// <summary>
    /// Null Object Pattern implementation for <see cref="IProgress{int}"/>. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullProgress : IProgress<int> {

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="NullProgress"/>.
        /// </summary>
        public static IProgress<int> Instance { get; } = new NullProgress ();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler not to mark type as beforefieldinit
        static NullProgress () { }

        #endregion

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullProgress () { }

        #endregion

        #region IProgress<int> Members

        /// <inheritdoc/>
        public void Report (int value) { }

        #endregion
    }

    /// <summary>
    /// Null Object Pattern implementation for <see cref="IProgress{T}"/>. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullProgress<T> : IProgress<T> {

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="NullProgress{T}"/>.
        /// </summary>
        public static IProgress<T> Instance { get; } = new NullProgress<T> ();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler not to mark type as beforefieldinit
        static NullProgress () { }

        #endregion

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullProgress () { }

        #endregion

        #region IProgress<T> Members

        /// <inheritdoc/>
        public void Report (T value) { }

        #endregion
    }
}