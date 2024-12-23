using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Nameless.Lucene;

/// <summary>
/// Default implementation of <see cref="ISearchHit"/>.
/// </summary>
public sealed class SearchHit : ISearchHit {
    private readonly IDocument _document;

    /// <summary>
    /// Initializes a new instance of <see cref="SearchHit"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="score">The score.</param>
    public SearchHit(IDocument document, float score) {
        _document = Prevent.Argument.Null(document);

        Score = score;
    }

    /// <inheritdoc />
    public string IndexDocumentID
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
        var id = GetString(nameof(ISearchHit.IndexDocumentID));

        return id ?? throw new IndexDocumentMissingIDException();
    }

    private IIndexableField? GetField(string name)
        => _document.GetField(name);
}