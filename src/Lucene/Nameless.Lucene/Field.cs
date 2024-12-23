namespace Nameless.Lucene;

public sealed record Field {
    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Gets the value.
    /// </summary>
    public object? Value { get; }
    /// <summary>
    /// Gets the indexable type.
    /// </summary>
    public IndexableType Type { get; } = IndexableType.Text;
    /// <summary>
    /// Gets the field options.
    /// </summary>
    public FieldOptions Options { get; } = FieldOptions.None;

    /// <summary>
    /// Initializes a new instance of <see cref="Field"/>
    /// </summary>
    /// <param name="name">The name of the field</param>
    /// <param name="value">The value of the field</param>
    /// <param name="type">The indexable type</param>
    /// <param name="options">The options</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/> is <c>null</c>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public Field(string name, object? value, IndexableType type = IndexableType.Text, FieldOptions options = FieldOptions.Store) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);
        Value = value;
        Type = type;
        Options = options;
    }
}