using System.Globalization;

namespace Nameless;

/// <summary>
///     <see cref="CultureInfo" /> extension methods.
/// </summary>
public static class CultureInfoExtensions {
    /// <param name="self">
    ///     The current culture info.
    /// </param>
    extension(CultureInfo self) {
        /// <summary>
        ///     Retrieves the culture tree.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IEnumerable{CultureInfo}" /> with all
        ///     lower cultures.
        /// </returns>
        public IEnumerable<CultureInfo> GetParents() {
            var culture = new CultureInfo(self.Name);
            while (!culture.Equals(culture.Parent)) {
                yield return culture;
                culture = culture.Parent;
            }
        }
    }
}