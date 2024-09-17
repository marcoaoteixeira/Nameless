namespace Nameless.Lucene;

public sealed record Field {
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

    /// <summary>
    /// Initializes a new instance of <see cref="Field"/>
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <param name="type">The indexable type.</param>
    /// <param name="options">The field options.</param>
    public Field(string name, object value, IndexableType type = IndexableType.Text, FieldOptions options = FieldOptions.None) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);
        Value = Prevent.Argument.Null(value);
        Type = type;
        Options = options;
    }
}