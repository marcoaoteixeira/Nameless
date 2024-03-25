using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nameless.Helpers {
    /// <summary>
    /// <see cref="CultureInfo"/> helpers.
    /// </summary>
    public static class CultureInfoHelper {
        #region Public Static Methods

        /// <summary>
        /// Tries create a <see cref="CultureInfo"/> instance by its name.
        /// </summary>
        /// <param name="cultureName">The culture name</param>
        /// <param name="culture">The output</param>
        /// <param name="defaultCultureName"></param>
        /// <returns><c>true</c> if could retrieve; otherwise <c>false</c>.</returns>
        public static bool TryCreateCultureInfo(string cultureName, [NotNullWhen(returnValue: true)] out CultureInfo? culture) {
            culture = null;

            var result = false;
            try {
                culture = CultureInfo.GetCultureInfo(cultureName);
                result = true;
            } catch { }

            return result;
        }

        #endregion
    }
}