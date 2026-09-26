namespace Nameless.Xml;

/// <summary>
///     <see cref="IXmlSchemaValidator" /> extension methods.
/// </summary>
public static class XmlSchemaValidatorExtension {
    private const int OFFSET_ZERO = 0;

    /// <param name="self">The source <see cref="IXmlSchemaValidator" />.</param>
    extension(IXmlSchemaValidator self) {
        /// <summary>
        ///     Validates the XML file.
        /// </summary>
        /// <param name="xmlFilePath">The XML file path.</param>
        /// <param name="schemaFilePath">The XML schema file path.</param>
        /// <returns><see langword="true"/> if is valid; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="xmlFilePath" /> or
        ///     <paramref name="schemaFilePath" /> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="xmlFilePath" /> or
        ///     if <paramref name="schemaFilePath" /> is empty or white spaces.
        /// </exception>
        public bool Validate(string xmlFilePath, string schemaFilePath) {
            using var xml = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read);
            using var schema = new FileStream(schemaFilePath, FileMode.Open, FileAccess.Read);

            return self.Validate(xml, schema);
        }

        /// <summary>
        ///     Validates a XML buffer.
        /// </summary>
        /// <param name="xmlBuffer">The XML buffer.</param>
        /// <param name="schemaBuffer">The XML schema buffer.</param>
        /// <returns><see langword="true"/> if is valid; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="xmlBuffer" /> or
        ///     <paramref name="schemaBuffer" /> is <see langword="null"/>.
        /// </exception>
        public bool Validate(byte[] xmlBuffer, byte[] schemaBuffer) {
            using var xml = new MemoryStream();
            xml.Write(xmlBuffer, OFFSET_ZERO, xmlBuffer.Length);
            xml.Seek(OFFSET_ZERO, SeekOrigin.Begin);

            using var schema = new MemoryStream();
            schema.Write(schemaBuffer, OFFSET_ZERO, schemaBuffer.Length);
            schema.Seek(OFFSET_ZERO, SeekOrigin.Begin);

            return self.Validate(xml, schema);
        }
    }
}