using Nameless.Services;

namespace Nameless;

/// <summary>
/// <see cref="IXmlSchemaValidator"/> extension methods.
/// </summary>
public static class XmlSchemaValidatorExtension {
    /// <summary>
    /// Validates a XML file.
    /// </summary>
    /// <param name="self">The source <see cref="IXmlSchemaValidator"/>.</param>
    /// <param name="xmlFilePath">The XML file path.</param>
    /// <param name="schemaFilePath">The XML schema file path.</param>
    /// <returns><c>true</c> if is valid; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="xmlFilePath"/> or
    /// <paramref name="schemaFilePath"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="xmlFilePath"/> or
    /// if <paramref name="schemaFilePath"/> is empty or white spaces.
    /// </exception>
    public static bool Validate(this IXmlSchemaValidator self, string xmlFilePath, string schemaFilePath) {
        Prevent.Argument.Null(self);
        Prevent.Argument.NullOrWhiteSpace(schemaFilePath);
        Prevent.Argument.NullOrWhiteSpace(xmlFilePath);

        using var xml = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read);
        using var schema = new FileStream(schemaFilePath, FileMode.Open, FileAccess.Read);
        return self.Validate(xml, schema);
    }

    /// <summary>
    /// Validates a XML buffer.
    /// </summary>
    /// <param name="self">The source <see cref="IXmlSchemaValidator"/>.</param>
    /// <param name="xmlBuffer">The XML buffer.</param>
    /// <param name="schemaBuffer">The XML schema buffer.</param>
    /// <returns><c>true</c> if is valid; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="xmlBuffer"/> or
    /// <paramref name="schemaBuffer"/> is <c>null</c>.
    /// </exception>
    public static bool Validate(this IXmlSchemaValidator self, byte[] xmlBuffer, byte[] schemaBuffer) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(schemaBuffer);
        Prevent.Argument.Null(xmlBuffer);

        using var xml = new MemoryStream();
        xml.Write(xmlBuffer, 0, xmlBuffer.Length);
        xml.Seek(0, SeekOrigin.Begin);

        using var schema = new MemoryStream();
        schema.Write(schemaBuffer, 0, schemaBuffer.Length);
        schema.Seek(0, SeekOrigin.Begin);

        return self.Validate(xml, schema);
    }
}