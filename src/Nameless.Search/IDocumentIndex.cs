using System;

namespace Nameless.Search {

    /// <summary>
    /// Defines methods for a document index.
    /// </summary>
    public interface IDocumentIndex {

        #region Methods

        /// <summary>
        /// Sets the document ID.
        /// </summary>
        /// <param name="id">Document ID.</param>
        /// <returns>The current instance of <see cref="IDocumentIndex"/>.</returns>
        IDocumentIndex SetID (string id);

        /// <summary>
        /// Adds a new <see cref="string"/> value to the document.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="options">The field options.</param>
        /// <returns>The current instance of <see cref="IDocumentIndex"/>.</returns>
        IDocumentIndex Set (string fieldName, string value, DocumentIndexOptions options = DocumentIndexOptions.None);

        /// <summary>
        /// Adds a new <see cref="DateTimeOffset"/> value to the document.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="options">The field options.</param>
        /// <returns>The current instance of <see cref="IDocumentIndex"/>.</returns>
        IDocumentIndex Set (string fieldName, DateTimeOffset value, DocumentIndexOptions options = DocumentIndexOptions.None);

        /// <summary>
        /// Adds a new <see cref="int"/> value to the document.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="options">The field options.</param>
        /// <returns>The current instance of <see cref="IDocumentIndex"/>.</returns>
        IDocumentIndex Set (string fieldName, int value, DocumentIndexOptions options = DocumentIndexOptions.None);

        /// <summary>
        /// Adds a new <see cref="bool"/> value to the document.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="options">The field options.</param>
        /// <returns>The current instance of <see cref="IDocumentIndex"/>.</returns>
        IDocumentIndex Set (string fieldName, bool value, DocumentIndexOptions options = DocumentIndexOptions.None);

        /// <summary>
        /// Adds a new <see cref="double"/> value to the document.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="options">The field options.</param>
        /// <returns>The current instance of <see cref="IDocumentIndex"/>.</returns>
        IDocumentIndex Set (string fieldName, double value, DocumentIndexOptions options = DocumentIndexOptions.None);

        #endregion Methods
    }
}