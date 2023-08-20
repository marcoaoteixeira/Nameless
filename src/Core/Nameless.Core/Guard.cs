namespace Nameless {
    /// <summary>
    /// A guard clause (https://en.wikipedia.org/wiki/Guard_(computer_science))
    /// is a software pattern that simplifies complex functions by
    /// "failing fast", checking for invalid inputs up front and immediately
    /// failing if any are found.
    /// </summary>
    public sealed class Guard {
        #region Public Static Read-Only Properties

        /// <summary>
        /// Gets the unique instance of <see cref="Guard" />.
        /// </summary>
        public static Guard Against { get; } = new();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static Guard() { }

        #endregion

        #region Private Constructors

        private Guard() { }

        #endregion
    }
}
