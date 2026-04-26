using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Nameless.Lucene.Empty;

/// <summary>
///     Empty implementation of <see cref="Terms" />.
/// </summary>
public sealed class EmptyTerms : Terms {
    /// <summary>
    ///     Gets the singleton instance of <see cref="EmptyTerms" />.
    /// </summary>
    public static Terms Instance { get; } = new EmptyTerms();

    /// <inheritdoc />
    public override IComparer<BytesRef> Comparer => Comparer<BytesRef>.Default;

    /// <inheritdoc />
    public override long Count => 0L;

    /// <inheritdoc />
    public override long SumTotalTermFreq => 0L;

    /// <inheritdoc />
    public override long SumDocFreq => 0L;

    /// <inheritdoc />
    public override int DocCount => 0;

    /// <inheritdoc />
    public override bool HasFreqs => false;

    /// <inheritdoc />
    public override bool HasOffsets => false;

    /// <inheritdoc />
    public override bool HasPositions => false;

    /// <inheritdoc />
    public override bool HasPayloads => false;

    static EmptyTerms() { }

    private EmptyTerms() { }

    /// <inheritdoc />
    public override TermsEnum GetEnumerator() {
        return TermsEnum.EMPTY;
    }
}