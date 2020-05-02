using System.Globalization;

namespace Nameless.Localization {
    public static class LocalizationUtils {
        #region Public Static Methods

        public static string[] GetCultures (CultureInfo culture) {
            return !culture.IsNeutralCulture ?
                new [] { culture.Name, culture.Parent.Name, string.Empty } :
                new [] { culture.Name, string.Empty };
        }

        #endregion
    }
}