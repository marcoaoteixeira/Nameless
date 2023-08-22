using System.Xml.Linq;
using System.Xml.XPath;

namespace Nameless {
    /// <summary>
    /// <see cref="XContainer"/> extension methods.
    /// </summary>
    public static class XContainerExtension {
        #region Private Constants

        private const string ELEMENT_PATH_PATTERN = "./{0}[@{1}='{2}']";

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Verifies if the <paramref name="elementName"/> is present into the <paramref name="self"/> <see cref="XContainer"/>.
        /// </summary>
        /// <param name="self">The self <see cref="XContainer"/>.</param>
        /// <param name="elementName">The element name.</param>
        /// <returns><c>true</c> if exists, otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool HasElement(this XContainer self, string elementName)
            => self.Element(elementName) is not null;

        /// <summary>
        /// Verifies if the <paramref name="elementName"/> with attribute (specified by <paramref name="attributeName"/>) and attribute value (specified by <paramref name="attributeValue"/>) is present into the <paramref name="self"/> <see cref="XContainer"/>.
        /// </summary>
        /// <param name="self">The <see cref="XContainer"/> element.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns><c>true</c> if it has the attribute; otherwise <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="elementName"/> or <paramref name="attributeName"/> or <paramref name="attributeValue"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">if <paramref name="elementName"/> or <paramref name="attributeName"/> or <paramref name="attributeValue"/> is empty or white spaces.</exception>
        public static bool HasElement(this XContainer self, string elementName, string attributeName, string attributeValue) {
            Guard.Against.NullOrWhiteSpace(elementName, nameof(elementName));
            Guard.Against.NullOrWhiteSpace(attributeName, nameof(attributeName));
            Guard.Against.NullOrWhiteSpace(attributeValue, nameof(attributeValue));

            var expression = string.Format(ELEMENT_PATH_PATTERN, elementName, attributeName, attributeValue);

            return self.XPathSelectElement(expression) is not null;
        }

        #endregion
    }
}