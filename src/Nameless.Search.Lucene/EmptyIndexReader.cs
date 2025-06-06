using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene;

public sealed class EmptyBits : IBits {
    public bool Get(int index) {
        return false;
    }

    public int Length => 0;
}

public sealed class EmptyBinaryDocValues : BinaryDocValues {
    public override void Get(int docID, BytesRef result) { }
}

public sealed class EmptyNumericDocValues : NumericDocValues {
    public override long Get(int docID) {
        return 0L;
    }
}

public sealed class EmptySortedDocValues : SortedDocValues {
    public override int ValueCount => 0;

    public override int GetOrd(int docID) {
        return 0;
    }

    public override void LookupOrd(int ord, BytesRef result) { }
}

public sealed class EmptySortedSetDocValues : SortedSetDocValues {
    public override long ValueCount => 0L;

    public override long NextOrd() {
        return 0L;
    }

    public override void SetDocument(int docID) { }

    public override void LookupOrd(long ord, BytesRef result) { }
}

public sealed class EmptyTerms : Terms {
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

public sealed class EmptyFields : Fields {
    public override int Count => 0;

    public override IEnumerator<string> GetEnumerator() {
        return Enumerable.Empty<string>().GetEnumerator();
    }

    public override Terms GetTerms(string field) {
        return new EmptyTerms();
    }
}

public class EmptyIndexReader : AtomicReader {
    public override int NumDocs => 0;
    public override int MaxDoc => 0;
    public override Fields Fields => new EmptyFields();
    public override FieldInfos FieldInfos => new([]);
    public override IBits LiveDocs => new EmptyBits();

    public override Fields GetTermVectors(int docID) {
        return new EmptyFields();
    }

    public override void Document(int docID, StoredFieldVisitor visitor) { }

    protected override void DoClose() { }

    public override NumericDocValues GetNumericDocValues(string field) {
        return new EmptyNumericDocValues();
    }

    public override BinaryDocValues GetBinaryDocValues(string field) {
        return new EmptyBinaryDocValues();
    }

    public override SortedDocValues GetSortedDocValues(string field) {
        return new EmptySortedDocValues();
    }

    public override SortedSetDocValues GetSortedSetDocValues(string field) {
        return new EmptySortedSetDocValues();
    }

    public override IBits GetDocsWithField(string field) {
        return new EmptyBits();
    }

    public override NumericDocValues GetNormValues(string field) {
        return new EmptyNumericDocValues();
    }

    public override void CheckIntegrity() { }
}