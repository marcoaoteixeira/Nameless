using System.Collections;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Nameless.Lucene.ObjectModel;

/// <summary>
///     Represents a Lucene document paired with its relevance score from a search result.
/// </summary>
public class ScoreDocument : IEnumerable<IIndexableField> {
    private readonly List<IIndexableField> _inner;

    /// <summary>
    ///     Gets or sets the relevance score assigned to this document by the search engine.
    /// </summary>
    public float Score { get; set; }

    /// <summary>
    ///     Initializes a new empty <see cref="ScoreDocument"/> with a score of zero.
    /// </summary>
    public ScoreDocument() {
        _inner = [];
    }

    /// <summary>
    ///     Initializes a new <see cref="ScoreDocument"/> from an existing sequence of
    ///     indexable fields and the associated score.
    /// </summary>
    /// <param name="collection">The indexable fields of the document.</param>
    /// <param name="score">The relevance score.</param>
    public ScoreDocument(IEnumerable<IIndexableField> collection, float score) {
        _inner = new List<IIndexableField>(collection);

        Score = score;
    }

    /// <summary>
    ///     Implicitly converts a <see cref="Document"/> to a <see cref="ScoreDocument"/>
    ///     with a score of zero.
    /// </summary>
    /// <param name="document">The Lucene document to convert.</param>
    public static implicit operator ScoreDocument(Document document) {
        return new ScoreDocument(document.Fields, score: 0F);
    }

    /// <summary>
    ///     Implicitly converts a <see cref="ScoreDocument"/> back to a Lucene <see cref="Document"/>.
    /// </summary>
    /// <param name="document">The score document to convert.</param>
    public static implicit operator Document(ScoreDocument document) {
        return [.. document._inner];
    }

    /// <summary>
    ///     Adds an <see cref="IIndexableField"/> to this document.
    /// </summary>
    /// <param name="value">The field to add.</param>
    public void Add(IIndexableField value) {
        _inner.Add(value);
    }

    /// <inheritdoc />
    public IEnumerator<IIndexableField> GetEnumerator() {
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}