using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene.Null;

public sealed class NullTerms : Terms {
    public static Terms Instance { get; } = new NullTerms();

    static NullTerms() { }

    private NullTerms() { }

    public override IComparer<BytesRef> Comparer => Comparer<BytesRef>.Default;
    public override long Count => 0L;
    public override long SumTotalTermFreq => 0L;
    public override long SumDocFreq => 0L;
    public override int DocCount => 0;
    public override bool HasFreqs => false;
    public override bool HasOffsets => false;
    public override bool HasPositions => false;
    public override bool HasPayloads => false;

    public override TermsEnum GetEnumerator() {
        return TermsEnum.EMPTY;
    }
}