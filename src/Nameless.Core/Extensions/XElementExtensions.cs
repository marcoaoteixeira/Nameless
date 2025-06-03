using System.Xml.Linq;

namespace Nameless;

/// <summary>
///     <see cref="XElement" /> extension methods.
/// </summary>
public static class XElementExtensions {
    /// <summary>
    ///     Verifies if the attribute (specified by <paramref name="attributeName" />) is
    ///     present in the <see cref="XElement" />.
    /// </summary>
    /// <param name="self">The self <see cref="XElement" />.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <returns><c>true</c> if is present, otherwise, <c>false</c>.</returns>
    public static bool HasAttribute(this XElement self, string attributeName) {
        return self.Attribute(attributeName) is not null;
    }
}