using System.Collections;

namespace Nameless.Search;

/// <summary>
///     Default implementation of <see cref="IDocument" />.
/// </summary>
public sealed class Document : IDocument {
    private readonly Dictionary<string, Field> _fields = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of <see cref="Document" />
    /// </summary>
    /// <param name="id">The document ID.</param>
    public Document(string id) {
        ID = Guard.Against.NullOrWhiteSpace(id);

        _fields[nameof(ISearchHit.DocumentID)] = new Field(nameof(ISearchHit.DocumentID),
            ID,
            IndexableType.String,
            FieldOptions.Store);
    }

    /// <summary>
    ///     Gets the document ID.
    /// </summary>
    public string ID { get; }

    /// <inheritdoc />
    public IDocument Set(string field, bool value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.Boolean);
    }

    /// <inheritdoc />
    public IDocument Set(string field, string value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.String);
    }

    /// <inheritdoc />
    public IDocument Set(string field, byte value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.Byte);
    }

    /// <inheritdoc />
    public IDocument Set(string field, short value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.Short);
    }

    /// <inheritdoc />
    public IDocument Set(string field, int value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.Integer);
    }

    /// <inheritdoc />
    public IDocument Set(string field, long value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.Long);
    }

    /// <inheritdoc />
    public IDocument Set(string field, float value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.Float);
    }

    /// <inheritdoc />
    public IDocument Set(string field, double value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.Double);
    }

    /// <inheritdoc />
    public IDocument Set(string field, DateTimeOffset value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.DateTimeOffset);
    }

    /// <inheritdoc />
    public IDocument Set(string field, DateTime value, FieldOptions options = FieldOptions.Store) {
        return Set(field, value, options, IndexableType.DateTime);
    }

    /// <inheritdoc />
    public IEnumerator<Field> GetEnumerator() {
        return _fields.Values.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    private Document Set(string name, object value, FieldOptions options, IndexableType type) {
        _fields[name] = new Field(name, value, type, options);

        return this;
    }
}