using System.Xml;
using System.Xml.Schema;

namespace Nameless.Services.Impl;

/// <summary>
/// Singleton Pattern implementation for <see cref="IXmlSchemaValidator" />.
/// See <a href="https://en.wikipedia.org/wiki/Singleton_pattern">Singleton Pattern on Wikipedia</a>
/// </summary>
[Singleton]
public sealed class XmlSchemaValidator : IXmlSchemaValidator {
    public static IXmlSchemaValidator Instance { get; } = new XmlSchemaValidator();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static XmlSchemaValidator() { }

    private XmlSchemaValidator() { }

    /// <inheritdoc />
    public bool Validate(Stream xml, Stream schema) {
        Prevent.Argument.Null(xml, nameof(xml));
        Prevent.Argument.Null(schema, nameof(schema));

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