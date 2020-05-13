using System.Globalization;

namespace Nameless.Helpers {
    public static class CultureInfoHelper {
        #region Public Static Methods

        public static bool TryGetCultureInfo (string cultureName, out CultureInfo culture, string defaultCultureName = null) {
            try {
                culture = CultureInfo.GetCultureInfo (cultureName);
                return true;
            } catch (CultureNotFoundException) {
                if (!string.IsNullOrWhiteSpace (defaultCultureName)) {
                    try { culture = CultureInfo.GetCultureInfo (defaultCultureName); }
                    catch (CultureNotFoundException) { culture = CultureInfo.CurrentCulture; }
                } else { culture = CultureInfo.CurrentCulture; }
            }
            return false;
        }

        #endregion
    }
}