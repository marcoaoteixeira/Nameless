﻿using System.IO;

namespace Nameless.Services {

    /// <summary>
    /// Extension methods for <see cref="IXmlSchemaValidator"/>.
    /// </summary>
    public static class XmlSchemaValidatorExtension {

        #region Public Static Methods

        /// <summary>
        /// Validates a XML file.
        /// </summary>
        /// <param name="source">The source <see cref="IXmlSchemaValidator"/>.</param>
        /// <param name="schemaFilePath">The XML schema file path.</param>
        /// <param name="xmlFilePath">The XML file path.</param>
        /// <returns><c>true</c> if is valid; otherwise, <c>false</c>.</returns>
        public static bool Validate (this IXmlSchemaValidator source, string schemaFilePath, string xmlFilePath) {
            if (source == null) { return false; }

            Prevent.ParameterNullOrWhiteSpace (schemaFilePath, nameof (schemaFilePath));
            Prevent.ParameterNullOrWhiteSpace (xmlFilePath, nameof (xmlFilePath));

            using (var schema = new FileStream (schemaFilePath, FileMode.Open, FileAccess.Read))
            using (var xml = new FileStream (xmlFilePath, FileMode.Open, FileAccess.Read)) {
                return source.Validate (schema, xml);
            }
        }

        /// <summary>
        /// Validates a XML buffer.
        /// </summary>
        /// <param name="source">The source <see cref="IXmlSchemaValidator"/>.</param>
        /// <param name="schemaBuffer">The XML schema buffer.</param>
        /// <param name="xmlBuffer">The XML buffer.</param>
        /// <returns><c>true</c> if is valid; otherwise, <c>false</c>.</returns>
        public static bool Validate (this IXmlSchemaValidator source, byte[] schemaBuffer, byte[] xmlBuffer) {
            if (source == null) { return false; }

            using (var schema = new MemoryStream ())
            using (var xml = new MemoryStream ()) {
                schema.Write (schemaBuffer, 0, schemaBuffer.Length);
                xml.Write (xmlBuffer, 0, xmlBuffer.Length);

                schema.Seek (0, SeekOrigin.Begin);
                xml.Seek (0, SeekOrigin.Begin);

                return source.Validate (schema, xml);
            }
        }

        #endregion
    }
}