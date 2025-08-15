using Lucene.Net.Index;
using LuceneDocument = Lucene.Net.Documents.Document;

namespace Nameless.Search.Lucene;

/// <summary>
///     Default implementation of <see cref="ISearchHit" />.
/// </summary>
public sealed class SearchHit : ISearchHit {
    private readonly LuceneDocument _document;

    /// <summary>
    ///     Initializes a new instance of <see cref="SearchHit" />
    /// </summary>
    /// <param name="document">A Lucene.NET document object.</param>
    /// <param name="score">The score.</param>
    public SearchHit(LuceneDocument document, float score) {
        _document = Guard.Against.Null(document);

        Score = score;
    }

    /// <inheritdoc />
    public string DocumentID
        => GetDocumentID();

    /// <inheritdoc />
    public float Score { get; }

    /// <inheritdoc />
    public bool? GetBoolean(string name) {
        var value = GetString(name);

        return value is not null
            ? string.Equals(value, bool.TrueString, StringComparison.OrdinalIgnoreCase)
            : null;
    }

    /// <inheritdoc />
    public string? GetString(string name) {
        return GetField(name)?.GetStringValue();
    }

    /// <inheritdoc />
    public byte? GetByte(string name) {
        return GetField(name)?.GetByteValue();
    }

    /// <inheritdoc />
    public short? GetShort(string name) {
        return GetField(name)?.GetInt16Value();
    }

    /// <inheritdoc />
    public int? GetInteger(string name) {
        return GetField(name)?.GetInt32Value();
    }

    /// <inheritdoc />
    public long? GetLong(string name) {
        return GetField(name)?.GetInt64Value();
    }

    /// <inheritdoc />
    public float? GetFloat(string name) {
        return GetField(name)?.GetSingleValue();
    }

    /// <inheritdoc />
    public double? GetDouble(string name) {
        return GetField(name)?.GetDoubleValue();
    }

    /// <inheritdoc />
    public DateTimeOffset? GetDateTimeOffset(string name) {
        var value = GetLong(name);

        return value.HasValue
            ? DateTimeOffset.FromUnixTimeMilliseconds(value.Value)
            : null;
    }

    /// <inheritdoc />
    public DateTime? GetDateTime(string name) {
        var value = GetLong(name);

        return value.HasValue
            ? DateTime.FromBinary(value.Value)
            : null;
    }

    private string GetDocumentID() {
        const string FieldName = nameof(ISearchHit.DocumentID);

        var id = GetString(FieldName);

        return Guard.Against.Null(id, FieldName);
    }

    private IIndexableField? GetField(string name) {
        return _document.GetField(name);
    }
}