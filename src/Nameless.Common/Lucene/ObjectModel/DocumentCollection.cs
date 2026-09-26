using System.Collections;
using Lucene.Net.Documents;

namespace Nameless.Lucene.ObjectModel;

/// <summary>
///     A collection of Lucene <see cref="Document"/> instances.
/// </summary>
public class DocumentCollection : IEnumerable<Document> {
    private readonly List<Document> _inner;

    /// <summary>
    ///     Gets the number of documents in the collection.
    /// </summary>
    public int Count => _inner.Count;

    /// <summary>
    ///     Initializes a new empty <see cref="DocumentCollection"/>.
    /// </summary>
    public DocumentCollection() {
        _inner = [];
    }

    /// <summary>
    ///     Initializes a new <see cref="DocumentCollection"/> from an existing
    ///     sequence of <see cref="Document"/> instances.
    /// </summary>
    /// <param name="collection">The initial set of documents.</param>
    public DocumentCollection(IEnumerable<Document> collection) {
        _inner = [.. collection];
    }

    /// <summary>
    ///     Adds a <see cref="Document"/> to the collection.
    /// </summary>
    /// <param name="document">The document to add.</param>
    public void Add(Document document) {
        _inner.Add(document);
    }

    /// <inheritdoc />
    public IEnumerator<Document> GetEnumerator() {
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}