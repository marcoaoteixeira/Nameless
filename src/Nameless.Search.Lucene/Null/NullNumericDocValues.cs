using Lucene.Net.Index;

namespace Nameless.Search.Lucene.Null;

public sealed class NullNumericDocValues : NumericDocValues {
    public static NumericDocValues Instance { get; } = new NullNumericDocValues();

    static NullNumericDocValues() { }

    private NullNumericDocValues() { }

    public override long Get(int docID) {
        return 0L;
    }
}