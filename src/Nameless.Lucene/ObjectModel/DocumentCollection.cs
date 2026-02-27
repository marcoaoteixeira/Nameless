using System.Collections;
using Lucene.Net.Index;

namespace Nameless.Lucene.ObjectModel;

public class DocumentCollection : IEnumerable<IEnumerable<IIndexableField>> {
    private readonly List<IEnumerable<IIndexableField>> _inner;

    public int Count => _inner.Count;

    public DocumentCollection() {
        _inner = [];
    }

    public DocumentCollection(IEnumerable<IEnumerable<IIndexableField>> collection) {
        _inner = [.. collection];
    }

    public void Add(IEnumerable<IIndexableField> document) {
        _inner.Add(document);
    }

    public IEnumerator<IEnumerable<IIndexableField>> GetEnumerator() {
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}