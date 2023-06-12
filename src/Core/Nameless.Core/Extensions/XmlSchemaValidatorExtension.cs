using Nameless.Services;

namespace Nameless
{

    /// <summary>
    /// Extension methods for <see cref="IXmlSchemaValidator"/>.
    /// </summary>
    public static class XmlSchemaValidatorExtension {

        #region Public Static Methods

        /// <summary>
        /// Validates a XML file.
        /// </summary>
        /// <param name="self">The source <see cref="IXmlSchemaValidator"/>.</param>
        /// <param name="schemaFilePath">The XML schema file path.</param>
        /// <param name="xmlFilePath">The XML file path.</param>
        /// <returns><c>true</c> if is valid; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="self"/> is <c>null</c> or
        /// if <paramref name="schemaFilePath"/> is <c>null</c> or
        /// if <paramref name="xmlFilePath"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <paramref name="schemaFilePath"/> is empty or white spaces or
        /// if <paramref name="xmlFilePath"/> is empty or white spaces.
        /// </exception>
        public static bool Validate(this IXmlSchemaValidator self, string schemaFilePath, string xmlFilePath) {
            Prevent.Null(self, nameof(self));
            Prevent.NullOrWhiteSpaces(schemaFilePath, nameof(schemaFilePath));
            Prevent.NullOrWhiteSpaces(xmlFilePath, nameof(xmlFilePath));

            using var schema = new FileStream(schemaFilePath, FileMode.Open, FileAccess.Read);
            using var xml = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read);
            return self.Validate(schema, xml);
        }

        /// <summary>
        /// Validates a XML buffer.
        /// </summary>
        /// <param name="self">The source <see cref="IXmlSchemaValidator"/>.</param>
        /// <param name="schemaBuffer">The XML schema buffer.</param>
        /// <param name="xmlBuffer">The XML buffer.</param>
        /// <returns><c>true</c> if is valid; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static bool Validate(this IXmlSchemaValidator self, byte[] schemaBuffer, byte[] xmlBuffer) {
            Prevent.Null(self, nameof(self));

            using var schema = new MemoryStream();
            using var xml = new MemoryStream();
            schema.Write(schemaBuffer, 0, schemaBuffer.Length);
            xml.Write(xmlBuffer, 0, xmlBuffer.Length);

            schema.Seek(0, SeekOrigin.Begin);
            xml.Seek(0, SeekOrigin.Begin);

            return self.Validate(schema, xml);
        }

        #endregion
    }
}
