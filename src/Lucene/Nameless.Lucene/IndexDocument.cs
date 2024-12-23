using System.Collections;

namespace Nameless.Lucene;

/// <summary>
/// Default implementation of <see cref="IIndexDocument"/>.
/// </summary>
public sealed class IndexDocument : IIndexDocument {
    private readonly Dictionary<string, Field> _fields = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Gets the document ID.
    /// </summary>
    public string ID { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="IndexDocument"/>
    /// </summary>
    /// <param name="id">The document ID.</param>
    public IndexDocument(string id) {
        ID = Prevent.Argument.NullOrWhiteSpace(id);

        _fields[nameof(ISearchHit.IndexDocumentID)] = new Field(name: nameof(ISearchHit.IndexDocumentID),
                                                                value: ID,
                                                                type: IndexableType.Text,
                                                                options: FieldOptions.Store);
    }

    /// <inheritdoc />
    public IIndexDocument Set(string field, string value, FieldOptions options = FieldOptions.Store)
        => Set(IndexableType.Text, field, value, options);

    /// <inheritdoc />
    public IIndexDocument Set(string field, DateTimeOffset value, FieldOptions options = FieldOptions.Store)
        => Set(IndexableType.DateTime, field, value, options);

    /// <inheritdoc />
    public IIndexDocument Set(string field, int value, FieldOptions options = FieldOptions.Store)
        => Set(IndexableType.Integer, field, value, options);

    /// <inheritdoc />
    public IIndexDocument Set(string field, bool value, FieldOptions options = FieldOptions.Store)
        => Set(IndexableType.Boolean, field, value, options);

    /// <inheritdoc />
    public IIndexDocument Set(string field, double value, FieldOptions options = FieldOptions.Store)
        => Set(IndexableType.Number, field, value, options);

    public IEnumerator<Field> GetEnumerator()
        => _fields.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _fields.Values.GetEnumerator();

    private IndexDocument Set(IndexableType type, string name, object value, FieldOptions options) {
        _fields[name] = new Field(name, value, type, options);

        return this;
    }
}