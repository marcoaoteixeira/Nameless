using System.Globalization;

namespace Nameless;

/// <summary>
/// <see cref="CultureInfo"/> extension methods.
/// </summary>
public static class CultureInfoExtension {
    /// <summary>
    /// Retrieves the culture tree.
    /// </summary>
    /// <param name="self">The current culture info.</param>
    /// <returns>An instance of <see cref="IEnumerable{CultureInfo}"/> with all lower cultures.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IEnumerable<CultureInfo> GetParents(this CultureInfo self) {
        Prevent.Argument.Null(self, nameof(self));

        var culture = new CultureInfo(self.Name);
        while (!string.IsNullOrWhiteSpace(culture.Name)) {
            yield return culture;
            culture = culture.Parent;
        }
    }
}