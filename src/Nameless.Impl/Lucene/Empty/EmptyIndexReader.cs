using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="AtomicReader" />.
/// </summary>
public class EmptyIndexReader : AtomicReader {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptyIndexReader" />.
    /// </summary>
    public static AtomicReader Instance { get; } = new EmptyIndexReader();

    /// <inheritdoc />
    public override int NumDocs => 0;

    /// <inheritdoc />
    public override int MaxDoc => 0;

    /// <inheritdoc />
    public override Fields Fields => EmptyFields.Instance;

    /// <inheritdoc />
    public override FieldInfos FieldInfos => new([]);

    /// <inheritdoc />
    public override IBits LiveDocs => EmptyBits.Instance;

    static EmptyIndexReader() { }

    private EmptyIndexReader() { }

    /// <inheritdoc />
    public override Fields GetTermVectors(int docID) {
        return EmptyFields.Instance;
    }

    /// <inheritdoc />
    public override void Document(int docID, StoredFieldVisitor visitor) { }

    /// <inheritdoc />
    protected override void DoClose() { }

    /// <inheritdoc />
    public override NumericDocValues GetNumericDocValues(string field) {
        return EmptyNumericDocValues.Instance;
    }

    /// <inheritdoc />
    public override BinaryDocValues GetBinaryDocValues(string field) {
        return EmptyBinaryDocValues.Instance;
    }

    /// <inheritdoc />
    public override SortedDocValues GetSortedDocValues(string field) {
        return EmptySortedDocValues.Instance;
    }

    /// <inheritdoc />
    public override SortedSetDocValues GetSortedSetDocValues(string field) {
        return EmptySortedSetDocValues.Instance;
    }

    /// <inheritdoc />
    public override IBits GetDocsWithField(string field) {
        return EmptyBits.Instance;
    }

    /// <inheritdoc />
    public override NumericDocValues GetNormValues(string field) {
        return EmptyNumericDocValues.Instance;
    }

    /// <inheritdoc />
    public override void CheckIntegrity() { }
}