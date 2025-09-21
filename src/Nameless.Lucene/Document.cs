using System.Collections;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="IDocument" />.
/// </summary>
public sealed class Document : IDocument {
    /// <summary>
    ///     Gets the reserved name for the document ID field.
    /// </summary>
    public const string RESERVED_ID_NAME = "__document_id__";

    private readonly Dictionary<string, Field> _fields = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of <see cref="Document" />
    /// </summary>
    /// <param name="id">The document ID.</param>
    public Document(string id) {
        ID = Guard.Against.NullOrWhiteSpace(id);

        _fields[RESERVED_ID_NAME] = new Field(
            name: RESERVED_ID_NAME,
            value: ID,
            type: IndexableType.String,
            options: FieldOptions.Store
        );
    }

    /// <summary>
    ///     Gets the document ID.
    /// </summary>
    public string ID { get; }

    /// <inheritdoc />
    public IDocument Set(string name, bool value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Boolean);
    }

    /// <inheritdoc />
    public IDocument Set(string name, string value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.String);
    }

    /// <inheritdoc />
    public IDocument Set(string name, byte value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Byte);
    }

    /// <inheritdoc />
    public IDocument Set(string name, short value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Short);
    }

    /// <inheritdoc />
    public IDocument Set(string name, int value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Integer);
    }

    /// <inheritdoc />
    public IDocument Set(string name, long value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Long);
    }

    /// <inheritdoc />
    public IDocument Set(string name, float value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Float);
    }

    /// <inheritdoc />
    public IDocument Set(string name, double value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Double);
    }

    /// <inheritdoc />
    public IDocument Set(string name, DateTimeOffset value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.DateTimeOffset);
    }

    /// <inheritdoc />
    public IDocument Set(string name, DateTime value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.DateTime);
    }

    /// <inheritdoc />
    public IDocument Set(string name, DateOnly value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.DateOnly);
    }

    /// <inheritdoc />
    public IDocument Set(string name, TimeOnly value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.TimeOnly);
    }

    /// <inheritdoc />
    public IDocument Set(string name, TimeSpan value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.TimeSpan);
    }

    /// <inheritdoc />
    public IDocument Set(string name, Enum value, FieldOptions options = FieldOptions.Store) {
        return Set(name, value, options, IndexableType.Enum);
    }

    /// <inheritdoc />
    public IEnumerator<Field> GetEnumerator() {
        return _fields.Values.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    private Document Set(string name, object? value, FieldOptions options, IndexableType type) {
        Guard.Against.NullOrWhiteSpace(name);

        if (value is null) { return this; }

        if (string.Equals(name, RESERVED_ID_NAME, StringComparison.InvariantCultureIgnoreCase)) {
            throw new InvalidOperationException($"Field name '{RESERVED_ID_NAME}' is reserved.");
        }

        _fields[name] = new Field(name, value, type, options);

        return this;
    }
}