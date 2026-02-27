using System.Collections;
using Lucene.Net.Index;

namespace Nameless.Lucene.ObjectModel;

public class ScoreDocument : IEnumerable<IIndexableField> {
    private readonly List<IIndexableField> _inner;

    public float Score { get; set; }

    public ScoreDocument() {
        _inner = [];
    }

    public ScoreDocument(IEnumerable<IIndexableField> collection, float score) {
        _inner = new List<IIndexableField>(collection);

        Score = score;
    }

    public void Add(IIndexableField value) {
        _inner.Add(value);
    }

    public IEnumerator<IIndexableField> GetEnumerator() {
        return _inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}