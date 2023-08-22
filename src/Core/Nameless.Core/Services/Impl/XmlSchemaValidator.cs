using System.Xml;
using System.Xml.Schema;

namespace Nameless.Services.Impl {
    /// <summary>
    /// Default implementation of <see cref="IXmlSchemaValidator"/>.
    /// </summary>
    [Singleton]
    public sealed class XmlSchemaValidator : IXmlSchemaValidator {
        #region Public Static Read-Only Properties

        public static IXmlSchemaValidator Instance { get; } = new XmlSchemaValidator();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static XmlSchemaValidator() { }

        #endregion

        #region Private Constructors

        private XmlSchemaValidator() { }

        #endregion

        #region IXmlSchemaValidator Members

        /// <inheritdoc />
        public bool Validate(Stream schema, Stream xml) {
            Guard.Against.Null(schema, nameof(schema));
            Guard.Against.Null(xml, nameof(xml));

            var success = false;
            var settings = new XmlReaderSettings();
            var xmlSchema = XmlSchema.Read(stream: schema, validationEventHandler: null)
                ?? throw new InvalidOperationException("XmlSchema is null.");

            settings.Schemas.Add(xmlSchema);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (sender, args) => success = true;

            using var xmlReader = XmlReader.Create(xml, settings);
            while (xmlReader.Read()) { /* Do nothing */ }

            schema.Seek(offset: 0, origin: SeekOrigin.Begin);
            xml.Seek(offset: 0, origin: SeekOrigin.Begin);

            return success;
        }

        #endregion
    }
}
