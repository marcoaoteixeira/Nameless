using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene.Null;

public sealed class NullSortedSetDocValues : SortedSetDocValues {
    public static SortedSetDocValues Instance { get; } = new NullSortedSetDocValues();

    static NullSortedSetDocValues() { }

    private NullSortedSetDocValues() { }

    public override long ValueCount => 0L;

    public override long NextOrd() {
        return 0L;
    }

    public override void SetDocument(int docID) { }

    public override void LookupOrd(long ord, BytesRef result) { }
}