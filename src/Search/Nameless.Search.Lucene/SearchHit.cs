using Lucene.Net.Documents;
using Lucene.Net.Index;
using LuceneDocument = Lucene.Net.Documents.Document;

namespace Nameless.Search.Lucene;

/// <summary>
/// Default implementation of <see cref="ISearchHit"/>.
/// </summary>
public sealed class SearchHit : ISearchHit {
    private readonly LuceneDocument _document;

    /// <summary>
    /// Initializes a new instance of <see cref="SearchHit"/>
    /// </summary>
    /// <param name="document">A Lucene.NET document object.</param>
    /// <param name="score">The score.</param>
    public SearchHit(LuceneDocument document, float score) {
        _document = Prevent.Argument.Null(document);

        Score = score;
    }

    /// <inheritdoc />
    public string DocumentID
        => GetDocumentID();

    /// <inheritdoc />
    public float Score { get; }

    /// <inheritdoc />
    public int? GetInt(string name) {
        var field = GetField(name);

        return field is not null &&
               int.TryParse(field.GetStringValue(), out var result)
            ? result
            : null;
    }

    /// <inheritdoc />
    public double? GetDouble(string name) {
        var field = GetField(name);

        return field is not null &&
               double.TryParse(field.GetStringValue(), out var result)
            ? result
            : null;
    }

    /// <inheritdoc />
    public bool? GetBoolean(string name)
        => GetInt(name) is > 0;

    /// <inheritdoc />
    public string? GetString(string name)
        => GetField(name)?.GetStringValue();

    /// <inheritdoc />
    public DateTimeOffset? GetDateTimeOffset(string name) {
        var field = GetField(name);

        DateTimeOffset? result = null;

        try {
            result = field is not null
                ? DateTools.StringToDate(field.GetStringValue())
                : null;
        } catch { /* swallow */ }

        return result;
    }

    private string GetDocumentID() {
        const string fieldName = nameof(ISearchHit.DocumentID);

        var id = GetString(fieldName);

        return Prevent.Argument.Null(paramValue: id, paramName: fieldName);
    }

    private IIndexableField? GetField(string name)
        => _document.GetField(name);
}