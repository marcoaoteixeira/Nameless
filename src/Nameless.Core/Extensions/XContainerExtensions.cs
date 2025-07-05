using System.Xml.Linq;
using System.Xml.XPath;

namespace Nameless;

/// <summary>
///     <see cref="XContainer" /> extension methods.
/// </summary>
public static class XContainerExtensions {
    private const string ElementPathPattern = "./{0}[@{1}='{2}']";

    /// <summary>
    ///     Verifies if the <paramref name="elementName" /> is present into the <paramref name="self" />
    ///     <see cref="XContainer" />.
    /// </summary>
    /// <param name="self">The self <see cref="XContainer" />.</param>
    /// <param name="elementName">The element name.</param>
    /// <returns><see langword="true"/> if exists, otherwise, <see langword="false"/>.</returns>
    public static bool HasElement(this XContainer self, string elementName) {
        return self.Element(elementName) is not null;
    }

    /// <summary>
    ///     Verifies if the <paramref name="elementName" /> with attribute (specified by <paramref name="attributeName" />) and
    ///     attribute value (specified by <paramref name="attributeValue" />) is present into the <paramref name="self" />
    ///     <see cref="XContainer" />.
    /// </summary>
    /// <param name="self">The <see cref="XContainer" /> element.</param>
    /// <param name="elementName">The name of the element.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <param name="attributeValue">The attribute value.</param>
    /// <returns><see langword="true"/> if it has the attribute; otherwise <see langword="false"/>.</returns>
    public static bool HasElement(this XContainer self, string elementName, string attributeName,
                                  string attributeValue) {
        var expression = string.Format(ElementPathPattern, elementName, attributeName, attributeValue);

        return self.XPathSelectElement(expression) is not null;
    }
}