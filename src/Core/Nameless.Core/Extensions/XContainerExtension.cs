using System.Xml.Linq;
using System.Xml.XPath;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="XContainer"/>.
    /// </summary>
    public static class XContainerExtension {

        #region Public Static Methods

        /// <summary>
        /// Verifies if the <paramref name="elementName"/> is present into the <paramref name="self"/> <see cref="XContainer"/>.
        /// </summary>
        /// <param name="self">The self <see cref="XContainer"/>.</param>
        /// <param name="elementName">The element name.</param>
        /// <returns><c>true</c> if exists, otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool HasElement(this XContainer self, string elementName) {
            Prevent.Null(self, nameof(self));
            Prevent.NullOrWhiteSpaces(elementName, nameof(elementName));

            return self.Element(elementName) != default;
        }

        /// <summary>
        /// Verifies if the <paramref name="elementName"/> with attribute (specified by <paramref name="attributeName"/>) and attribute value (specified by <paramref name="attributeValue"/>) is present into the <paramref name="self"/> <see cref="XContainer"/>.
        /// </summary>
        /// <param name="self">The <see cref="XContainer"/> element.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns><c>true</c> if it has the attribute; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool HasElement(this XContainer self, string elementName, string attributeName, string attributeValue) {
            Prevent.Null(self, nameof(self));
            Prevent.NullOrWhiteSpaces(elementName, nameof(elementName));
            Prevent.NullOrWhiteSpaces(attributeName, nameof(attributeName));
            Prevent.NullOrWhiteSpaces(attributeValue, nameof(attributeValue));

            const string expressionPattern = "./{0}[@{1}='{2}']";

            var expression = string.Format(expressionPattern, elementName, attributeName, attributeValue);

            return self.XPathSelectElement(expression) != default;
        }

        #endregion
    }
}