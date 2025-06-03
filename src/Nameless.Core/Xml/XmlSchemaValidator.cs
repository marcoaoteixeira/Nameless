using System.Xml;
using System.Xml.Schema;

namespace Nameless.Xml;

public sealed class XmlSchemaValidator : IXmlSchemaValidator {
    /// <inheritdoc />
    public bool Validate(Stream xml, Stream schema) {
        Prevent.Argument.Null(xml);
        Prevent.Argument.Null(schema);

        var success = false;
        var settings = new XmlReaderSettings();
        var xmlSchema = XmlSchema.Read(schema, null)
                     ?? throw new InvalidOperationException("XmlSchema is null.");

        settings.Schemas.Add(xmlSchema);
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
        settings.ValidationEventHandler += (_, _) => success = true;

        using var xmlReader = XmlReader.Create(xml, settings);
        while (xmlReader.Read()) {
            /* Do nothing */
        }

        schema.Seek(0, SeekOrigin.Begin);
        xml.Seek(0, SeekOrigin.Begin);

        return success;
    }
}