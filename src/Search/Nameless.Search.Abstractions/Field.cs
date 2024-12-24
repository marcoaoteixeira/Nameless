namespace Nameless.Search;

/// <summary>
/// Represents a document field.
/// </summary>
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
    /// Gets the indexable type.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="IndexableType.String"/>
    /// </remarks>
    public IndexableType Type { get; }

    /// <summary>
    /// Gets the field options.
    /// </summary>
    /// <remarks>
    /// Default value is <see cref="FieldOptions.Store"/>
    /// </remarks>
    public FieldOptions Options { get; } = FieldOptions.Store;

    /// <summary>
    /// Initializes a new instance of <see cref="Field"/>
    /// </summary>
    /// <param name="name">The name of the field</param>
    /// <param name="value">The value of the field</param>
    /// <param name="type">The indexable type</param>
    /// <param name="options">The options</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/>
    /// or <paramref name="value"/> is <c>null</c>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public Field(string name, object value, IndexableType type, FieldOptions options) {
        if (!IsValueAssignable(value, type)) {
            throw new InvalidOperationException($"{nameof(IndexableType)} '{type}' does not match underlying type of parameter {nameof(value)}");
        }

        Name = Prevent.Argument.NullOrWhiteSpace(name);
        Value = Prevent.Argument.Null(value);
        Type = type;
        Options = options;
    }

    private static bool IsValueAssignable(object value, IndexableType type) {
        var valueType = value.GetType();

        return type switch {
            IndexableType.Boolean => valueType == typeof(bool),
            IndexableType.String => valueType == typeof(string),
            IndexableType.Byte => valueType == typeof(byte),
            IndexableType.Short => valueType == typeof(short),
            IndexableType.Integer => valueType == typeof(int),
            IndexableType.Long => valueType == typeof(long),
            IndexableType.Float => valueType == typeof(float),
            IndexableType.Double => valueType == typeof(double),
            IndexableType.DateTimeOffset => valueType == typeof(DateTimeOffset),
            IndexableType.DateTime => valueType == typeof(DateTime),
            _ => false
        };
    }
}