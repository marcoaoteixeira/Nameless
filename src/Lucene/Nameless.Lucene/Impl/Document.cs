using System.Collections;

namespace Nameless.Lucene.Impl {
    /// <summary>
    /// Default implementation of <see cref="IDocument"/>.
    /// </summary>
    public sealed class Document : IDocument {
        #region Private Read-Only Fields

        private readonly Dictionary<string, Field> _fields = new(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Document"/>
        /// </summary>
        /// <param name="id">The document ID.</param>
        public Document(string id) {
            ID = Guard.Against.NullOrWhiteSpace(id, nameof(id));

            _fields[nameof(ISearchHit.DocumentID)] = new(
                name: nameof(ISearchHit.DocumentID),
                value: ID,
                type: IndexableType.Text,
                options: FieldOptions.Store
            );
        }

        #endregion

        #region Private Methods

        private Document Set(IndexableType type, string name, object value, FieldOptions options) {
            _fields[name] = new(
                name: name,
                value: value,
                type: type,
                options: options
            );

            return this;
        }

        #endregion

        #region IDocumentIndex Members

        /// <summary>
        /// Gets the document ID.
        /// </summary>
        public string ID { get; }

        /// <inheritdoc />
        public IDocument Set(string name, string value, FieldOptions options)
            => Set(IndexableType.Text, name, value, options);

        /// <inheritdoc />
        public IDocument Set(string name, DateTimeOffset value, FieldOptions options)
            => Set(IndexableType.DateTime, name, value, options);

        /// <inheritdoc />
        public IDocument Set(string name, int value, FieldOptions options)
            => Set(IndexableType.Integer, name, value, options);

        /// <inheritdoc />
        public IDocument Set(string name, bool value, FieldOptions options)
            => Set(IndexableType.Boolean, name, value, options);

        /// <inheritdoc />
        public IDocument Set(string name, double value, FieldOptions options)
            => Set(IndexableType.Number, name, value, options);

        #endregion

        #region IEnumerable<Field> Members

        public IEnumerator<Field> GetEnumerator()
            => _fields.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _fields.Values.GetEnumerator();

        #endregion
    }
}
