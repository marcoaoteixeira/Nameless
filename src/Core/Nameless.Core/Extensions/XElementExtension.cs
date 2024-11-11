using System.Xml.Linq;

namespace Nameless;

/// <summary>
/// <see cref="XElement"/> extension methods.
/// </summary>
public static class XElementExtension {
    /// <summary>
    /// Verifies if the attribute (specified by <paramref name="attributeName"/>) is
    /// present in the <see cref="XElement"/>.
    /// </summary>
    /// <param name="self">The self <see cref="XElement"/>.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <returns><c>true</c> if is present, otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool HasAttribute(this XElement self, string attributeName)
        => Prevent.Argument
                  .Null(self)
                  .Attribute(attributeName) is not null;
}