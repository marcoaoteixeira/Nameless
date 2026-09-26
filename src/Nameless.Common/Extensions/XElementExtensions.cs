using System.Xml.Linq;

namespace Nameless;

/// <summary>
///     <see cref="XElement" /> extension methods.
/// </summary>
public static class XElementExtensions {
    /// <param name="self">The self <see cref="XElement" />.</param>
    extension(XElement self) {
        /// <summary>
        ///     Verifies if the attribute (specified by <paramref name="attributeName" />) is
        ///     present in the <see cref="XElement" />.
        /// </summary>
        /// <param name="attributeName">The attribute name.</param>
        /// <returns><see langword="true"/> if is present, otherwise, <see langword="false"/>.</returns>
        public bool HasAttribute(string attributeName) {
            return self.Attribute(attributeName) is not null;
        }
    }
}