namespace Nameless {
    /// <summary>
    /// A simple package with guard clauses. A guard clause (https://en.wikipedia.org/wiki/Guard_(computer_science))
    /// is a software pattern that simplifies complex functions by
    /// "failing fast", checking for invalid inputs up front and immediately
    /// failing if any are found. And if you thought about the
    /// Gárda Síochána (https://www.garda.ie/ga/), the national police service
    /// of Ireland, you're totally right!
    /// </summary>
    public sealed class Garda : IGuardClause {

        #region Private Static Read-Only Fields

        private static readonly IGuardClause _instance = new Garda();

        #endregion

        #region Public Static Read-Only Properties

        /// <summary>
        /// Gets the unique instance of <see cref="Garda" />.
        /// </summary>
        public static IGuardClause Prevent => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static Garda() { }

        #endregion

        #region Private Constructors

        private Garda() { }

        #endregion
    }
}
