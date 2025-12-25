using System.Globalization;

namespace Nameless;

/// <summary>
///     <see cref="CultureInfo" /> extension methods.
/// </summary>
public static class CultureInfoExtensions {
    /// <summary>
    ///     Retrieves the culture tree.
    /// </summary>
    /// <param name="self">
    ///     The current culture info.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="IEnumerable{CultureInfo}" /> with all
    ///     lower cultures.
    /// </returns>
    public static IEnumerable<CultureInfo> GetParents(this CultureInfo self) {
        var culture = new CultureInfo(self.Name);
        while (!culture.Equals(culture.Parent)) {
            yield return culture;
            culture = culture.Parent;
        }
    }
}