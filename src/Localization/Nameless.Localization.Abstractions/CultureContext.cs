using System.Globalization;

namespace Nameless.Localization {
    [Singleton]
    public sealed class CultureContext : ICultureContext {
        #region Private Static Read-Only Fields

        private static readonly CultureContext _instance = new();

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="CultureContext" />.
        /// </summary>
        public static ICultureContext Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static CultureContext() { }

        #endregion

        #region Private Constructors

        private CultureContext() { }

        #endregion

        #region ICultureContext Members

        public CultureInfo GetCurrentCulture() => Thread.CurrentThread.CurrentUICulture;

        #endregion
    }
}
