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
        var xmlSchema = XmlSchema.Read(stream: schema, validationEventHandler: null)
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

        schema.Seek(offset: 0, origin: SeekOrigin.Begin);
        xml.Seek(offset: 0, origin: SeekOrigin.Begin);

        return success;
    }
}