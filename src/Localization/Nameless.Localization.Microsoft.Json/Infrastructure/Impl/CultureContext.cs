using System.Globalization;

namespace Nameless.Localization.Microsoft.Json.Infrastructure.Impl {
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

        public CultureInfo GetCurrentCulture() {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var currentUICulture = Thread.CurrentThread.CurrentUICulture;
            
            if (!string.IsNullOrWhiteSpace(currentUICulture.Name)) {
                return currentUICulture;
            }

            if (!string.IsNullOrWhiteSpace(currentCulture.Name)) {
                return currentCulture;
            }

            return new CultureInfo("en-US");
        }

        #endregion
    }
}
