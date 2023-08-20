namespace Nameless {
    [Singleton]
    public sealed class NullDisposable : IDisposable {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="NullDisposable" />.
        /// </summary>
        public static IDisposable Instance { get; } = new NullDisposable();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullDisposable() { }

        #endregion

        #region Private Constructors

        private NullDisposable() { }

        #endregion

        #region IDisposable Members

        public void Dispose() { }

        #endregion
    }
}