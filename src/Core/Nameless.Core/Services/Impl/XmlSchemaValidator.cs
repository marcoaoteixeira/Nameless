using System.Xml;
using System.Xml.Schema;

namespace Nameless.Services.Impl {
    /// <summary>
    /// Default implementation of <see cref="IXmlSchemaValidator"/>.
    /// </summary>
    public sealed class XmlSchemaValidator : IXmlSchemaValidator {
        #region IXmlSchemaValidator Members

        /// <inheritdoc />
        public bool Validate(Stream schema, Stream xml) {
            Prevent.Against.Null(schema, nameof(schema));
            Prevent.Against.Null(xml, nameof(xml));

            var success = false;
            var settings = new XmlReaderSettings();
            var xmlSchema = XmlSchema.Read(stream: schema, validationEventHandler: null)
                ?? throw new InvalidOperationException("XmlSchema is null.");

            settings.Schemas.Add(xmlSchema);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (sender, args) => {
                success = true;
            };

            using var xmlReader = XmlReader.Create(xml, settings);
            while (xmlReader.Read()) { /* Do nothing */ }

            schema.Seek(offset: 0, origin: SeekOrigin.Begin);
            xml.Seek(offset: 0, origin: SeekOrigin.Begin);

            return success;
        }

        #endregion
    }
}
