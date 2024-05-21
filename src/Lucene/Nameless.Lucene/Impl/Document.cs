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

            _fields[nameof(ISearchHit.DocumentID)] = new Field(name: nameof(ISearchHit.DocumentID),
                                                               value: ID,
                                                               type: IndexableType.Text,
                                                               options: FieldOptions.Store);
        }

        #endregion

        #region Private Methods

        private Document Set(IndexableType type, string name, object value, FieldOptions options) {
            _fields[name] = new Field(name: name,
                                      value: value,
                                      type: type,
                                      options: options);

            return this;
        }

        #endregion

        #region IDocument Members

        /// <summary>
        /// Gets the document ID.
        /// </summary>
        public string ID { get; }

        /// <inheritdoc />
        public IDocument Set(string field, string value, FieldOptions options = FieldOptions.None)
            => Set(IndexableType.Text, field, value, options);

        /// <inheritdoc />
        public IDocument Set(string field, DateTimeOffset value, FieldOptions options = FieldOptions.None)
            => Set(IndexableType.DateTime, field, value, options);

        /// <inheritdoc />
        public IDocument Set(string field, int value, FieldOptions options = FieldOptions.None)
            => Set(IndexableType.Integer, field, value, options);

        /// <inheritdoc />
        public IDocument Set(string field, bool value, FieldOptions options = FieldOptions.None)
            => Set(IndexableType.Boolean, field, value, options);

        /// <inheritdoc />
        public IDocument Set(string field, double value, FieldOptions options = FieldOptions.None)
            => Set(IndexableType.Number, field, value, options);

        #endregion

        #region IEnumerable<Field> Members

        public IEnumerator<Field> GetEnumerator()
            => _fields.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _fields.Values.GetEnumerator();

        #endregion
    }
}
