using System;
using System.Collections.Generic;

namespace Nameless.Search.Lucene {

    /// <summary>
    /// Default implementation of <see cref="IDocumentIndex"/>.
    /// </summary>
    public sealed class DocumentIndex : IDocumentIndex {

        #region Private Read-Only Fields

        private readonly IDictionary<string, DocumentIndexEntry> _entries = new Dictionary<string, DocumentIndexEntry>(StringComparer.CurrentCultureIgnoreCase);

        #endregion Private Read-Only Fields

        #region Public Properties

        /// <summary>
        /// Gets the document index entries.
        /// </summary>
        public IEnumerable<KeyValuePair<string, DocumentIndexEntry>> Entries {
            get { return _entries; }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DocumentIndex"/>
        /// </summary>
        /// <param name="id">The document ID.</param>
        public DocumentIndex(string id) {
            SetID(id);
        }

        #endregion Public Constructors

        #region Public Inner Classes

        /// <summary>
        /// Enumerator for indexable types.
        /// </summary>
        public enum IndexableType {
            /// <summary>
            /// Integers
            /// </summary>
            Integer,
            /// <summary>
            /// String
            /// </summary>
            Text,
            /// <summary>
            /// Date/Time
            /// </summary>
            DateTime,
            /// <summary>
            /// Boolean
            /// </summary>
            Boolean,
            /// <summary>
            /// Float point or decimal numbers.
            /// </summary>
            Number
        }

        /// <summary>
        /// Document index entry.
        /// </summary>
        public class DocumentIndexEntry {

            #region Public Properties

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public object Value { get; set; }
            /// <summary>
            /// Gets or sets the indexable type.
            /// </summary>
            public IndexableType Type { get; set; }
            /// <summary>
            /// Gets or sets the document index options.
            /// </summary>
            public DocumentIndexOptions Options { get; set; }

            #endregion Public Properties

            #region Public Constructors

            /// <summary>
            /// Initializes a new instance of <see cref="DocumentIndexEntry"/>
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="type">The indexable type.</param>
            /// <param name="options">The options.</param>
            public DocumentIndexEntry(object value, IndexableType type, DocumentIndexOptions options) {
                Value = value;
                Type = type;
                Options = options;
            }

            #endregion Public Constructors
        }

        #endregion Public Inner Classes

        #region IDocumentIndex Members

        /// <summary>
        /// Gets the document ID.
        /// </summary>
        public string DocumentID { get; private set; }

        /// <inheritdoc />
        public IDocumentIndex SetID(string id) {
            DocumentID = id;

            _entries[nameof(ISearchHit.DocumentID)] = new DocumentIndexEntry(
                value: id,
                type: IndexableType.Text,
                options: DocumentIndexOptions.Store);

            return this;
        }

        /// <inheritdoc />
        public IDocumentIndex Set(string name, string value, DocumentIndexOptions options) {
            Prevent.ParameterNullOrWhiteSpace(name, nameof(name));

            _entries[name] = new DocumentIndexEntry(
                value: value,
                type: IndexableType.Text,
                options: options);

            return this;
        }

        /// <inheritdoc />
        public IDocumentIndex Set(string name, DateTimeOffset value, DocumentIndexOptions options) {
            Prevent.ParameterNullOrWhiteSpace(name, nameof(name));

            _entries[name] = new DocumentIndexEntry(
                value: value,
                type: IndexableType.DateTime,
                options: options);

            return this;
        }

        /// <inheritdoc />
        public IDocumentIndex Set(string name, int value, DocumentIndexOptions options) {
            Prevent.ParameterNullOrWhiteSpace(name, nameof(name));

            _entries[name] = new DocumentIndexEntry(
                value: value,
                type: IndexableType.Integer,
                options: options);

            return this;
        }

        /// <inheritdoc />
        public IDocumentIndex Set(string name, bool value, DocumentIndexOptions options) {
            Prevent.ParameterNullOrWhiteSpace(name, nameof(name));

            _entries[name] = new DocumentIndexEntry(
                value: value,
                type: IndexableType.Boolean,
                options: options);

            return this;
        }

        /// <inheritdoc />
        public IDocumentIndex Set(string name, double value, DocumentIndexOptions options) {
            Prevent.ParameterNullOrWhiteSpace(name, nameof(name));

            _entries[name] = new DocumentIndexEntry(
                value: value,
                type: IndexableType.Number,
                options: options);

            return this;
        }

        #endregion IDocumentIndex Members
    }
}