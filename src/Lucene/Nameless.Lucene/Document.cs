namespace Nameless.Lucene {
    /// <summary>
    /// Default implementation of <see cref="IDocument"/>.
    /// </summary>
    public sealed class Document : IDocument {
        #region Private Read-Only Fields

        private readonly string _id;
        private readonly Dictionary<string, Field> _fields = new(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the document index entries.
        /// </summary>
        public Field[] Fields => _fields.Values.ToArray();

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Document"/>
        /// </summary>
        /// <param name="id">The document ID.</param>
        public Document(string id) {
            Garda.Prevent.NullOrWhiteSpace(id, nameof(id));

            _id = id;

            _fields[nameof(ISearchHit.DocumentID)] = new(
                name: nameof(ISearchHit.DocumentID),
                value: id,
                type: IndexableType.Text,
                options: FieldOptions.Store
            );
        }

        #endregion

        #region Private Methods

        private IDocument InnerSet(IndexableType type, string name, object value, FieldOptions options) {
            Garda.Prevent.NullOrWhiteSpace(name, nameof(name));

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
        public string ID => _id;

        /// <inheritdoc />
        public IDocument Set(string name, string value, FieldOptions options)
            => InnerSet(IndexableType.Text, name, value, options);

        /// <inheritdoc />
        public IDocument Set(string name, DateTimeOffset value, FieldOptions options)
            => InnerSet(IndexableType.DateTime, name, value, options);

        /// <inheritdoc />
        public IDocument Set(string name, int value, FieldOptions options)
            => InnerSet(IndexableType.Integer, name, value, options);

        /// <inheritdoc />
        public IDocument Set(string name, bool value, FieldOptions options)
            => InnerSet(IndexableType.Boolean, name, value, options);

        /// <inheritdoc />
        public IDocument Set(string name, double value, FieldOptions options)
            => InnerSet(IndexableType.Number, name, value, options);

        #endregion
    }
}
