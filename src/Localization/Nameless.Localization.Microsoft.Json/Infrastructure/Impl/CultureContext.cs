using System.Globalization;

namespace Nameless.Localization.Microsoft.Json.Infrastructure.Impl {
    [Singleton]
    public sealed class CultureContext : ICultureContext {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="CultureContext" />.
        /// </summary>
        public static ICultureContext Instance { get; } = new CultureContext();

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
            CultureInfo culture;

            culture = Thread.CurrentThread.CurrentUICulture;
            if (!string.IsNullOrWhiteSpace(culture.Name)) {
                return culture;
            }

            culture = Thread.CurrentThread.CurrentCulture;
            if (!string.IsNullOrWhiteSpace(culture.Name)) {
                return culture;
            }

            culture = new CultureInfo("en-US");

            return culture;
        }

        #endregion
    }
}
