using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene.Null;

public sealed class NullSortedDocValues : SortedDocValues {
    public static SortedDocValues Instance { get; } = new NullSortedDocValues();

    static NullSortedDocValues() { }

    private NullSortedDocValues() { }

    public override int ValueCount => 0;

    public override int GetOrd(int docID) {
        return 0;
    }

    public override void LookupOrd(int ord, BytesRef result) { }
}