namespace Nameless.Search;

/// <summary>
///     Represents a document field.
/// </summary>
public sealed record Field {
    /// <summary>
    ///     Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    ///     Gets the indexable type.
    /// </summary>
    /// <remarks>
    ///     Default value is <see cref="IndexableType.String" />
    /// </remarks>
    public IndexableType Type { get; } = IndexableType.String;

    /// <summary>
    ///     Gets the field options.
    /// </summary>
    /// <remarks>
    ///     Default value is <see cref="FieldOptions.Store" />
    /// </remarks>
    public FieldOptions Options { get; } = FieldOptions.Store;

    /// <summary>
    ///     Initializes a new instance of <see cref="Field" />
    /// </summary>
    /// <param name="name">The name of the field</param>
    /// <param name="value">The value of the field</param>
    /// <param name="type">The indexable type</param>
    /// <param name="options">The options</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="name" />
    ///     or <paramref name="value" /> is <see langword="null"/>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="name" /> is empty or white spaces.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="value" /> type is not the same as defined
    ///     by <paramref name="type" />.
    /// </exception>
    public Field(string name, object value, IndexableType type, FieldOptions options) {
        Name = Prevent.Argument.NullOrWhiteSpace(name);
        Value = Prevent.Argument.NullOrNoMatchingType(value, type, nameof(value));
        Type = type;
        Options = options;
    }
}