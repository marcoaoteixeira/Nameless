namespace Nameless.Lucene {

    public sealed class Field {

        #region Public Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// Gets or sets the indexable type.
        /// </summary>
        public IndexableType Type { get; } = IndexableType.Text;
        /// <summary>
        /// Gets or sets the document index options.
        /// </summary>
        public FieldOptions Options { get; } = FieldOptions.None;

        #endregion

        #region Private Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DocumentEntry"/>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The indexable type.</param>
        /// <param name="options">The options.</param>
        public Field(string name, object value, IndexableType type = IndexableType.Text, FieldOptions options = FieldOptions.None) {
            Prevent.NullOrWhiteSpaces(name, nameof(name));
            Prevent.Null(value, nameof(value));

            Name = name;
            Value = value;
            Type = type;
            Options = options;
        }

        #endregion
    }
}