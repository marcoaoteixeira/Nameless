using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene.Null;

public sealed class NullBinaryDocValues : BinaryDocValues {
    public static BinaryDocValues Instance { get; } = new NullBinaryDocValues();

    static NullBinaryDocValues() { }

    private NullBinaryDocValues() { }

    public override void Get(int docID, BytesRef result) { }
}