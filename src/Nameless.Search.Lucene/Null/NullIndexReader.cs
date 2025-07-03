using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene.Null;

public class NullIndexReader : AtomicReader {
    public static AtomicReader Instance { get; } = new NullIndexReader();

    static NullIndexReader() { }

    private NullIndexReader() { }

    public override int NumDocs => 0;
    public override int MaxDoc => 0;
    public override Fields Fields => NullFields.Instance;
    public override FieldInfos FieldInfos => new([]);
    public override IBits LiveDocs => NullBits.Instance;

    public override Fields GetTermVectors(int docID) {
        return NullFields.Instance;
    }

    public override void Document(int docID, StoredFieldVisitor visitor) { }

    protected override void DoClose() { }

    public override NumericDocValues GetNumericDocValues(string field) {
        return NullNumericDocValues.Instance;
    }

    public override BinaryDocValues GetBinaryDocValues(string field) {
        return NullBinaryDocValues.Instance;
    }

    public override SortedDocValues GetSortedDocValues(string field) {
        return NullSortedDocValues.Instance;
    }

    public override SortedSetDocValues GetSortedSetDocValues(string field) {
        return NullSortedSetDocValues.Instance;
    }

    public override IBits GetDocsWithField(string field) {
        return NullBits.Instance;
    }

    public override NumericDocValues GetNormValues(string field) {
        return NullNumericDocValues.Instance;
    }

    public override void CheckIntegrity() { }
}