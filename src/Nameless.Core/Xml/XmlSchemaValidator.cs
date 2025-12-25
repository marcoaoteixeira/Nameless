using System.Xml;
using System.Xml.Schema;

namespace Nameless.Xml;

public sealed class XmlSchemaValidator : IXmlSchemaValidator {
    /// <inheritdoc />
    public bool Validate(Stream xml, Stream schema) {
        var success = false;
        var settings = new XmlReaderSettings();
        var xmlSchema = XmlSchema.Read(schema, validationEventHandler: null)
                     ?? throw new InvalidOperationException("XmlSchema is null.");

        settings.Schemas.Add(xmlSchema);
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
        settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
        settings.ValidationEventHandler += (_, _) => success = true;

        var xmlPreviousPosition = xml.Position;
        var schemaPreviousPosition = schema.Position;

        using var xmlReader = XmlReader.Create(xml, settings);
        while (xmlReader.Read()) {
            /* The "Read" action triggers the validation event handler */
        }

        xml.Position = xmlPreviousPosition;
        schema.Position = schemaPreviousPosition;

        return success;
    }
}