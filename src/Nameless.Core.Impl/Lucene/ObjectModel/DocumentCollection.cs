using System.Collections;
using Lucene.Net.Documents;

namespace Nameless.Lucene.ObjectModel;

public class DocumentCollection : IEnumerable<Document> {
    private readonly List<Document> _inner;

    public int Count => _inner.Count;

    public DocumentCollection() {
        _inner = [];
    }

    public DocumentCollection(IEnumerable<Document> collection) {
        _inner = [.. collection];
    }

    public void Add(Document document) {
        _inner.Add(document);
    }

    public IEnumerator<Document> GetEnumerator() {
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}