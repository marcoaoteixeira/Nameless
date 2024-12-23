using Lucene.Net.Util;

namespace Nameless.Lucene;

/// <summary>
/// Default implementation of <see cref="ISearchBit"/>.
/// </summary>
public sealed class SearchBit : ISearchBit {
    private readonly OpenBitSet _openBitSet;

    /// <summary>
    /// Initializes a new instance of <see cref="SearchBit"/>.
    /// </summary>
    /// <param name="openBitSet">The open bit set.</param>
    public SearchBit(OpenBitSet openBitSet) {
        _openBitSet = Prevent.Argument.Null(openBitSet);
    }

    /// <inheritdoc />
    public ISearchBit And(ISearchBit other)
        => Apply(other, (left, right) => left.And(right));

    /// <inheritdoc />
    public ISearchBit Or(ISearchBit other)
        => Apply(other, (left, right) => left.Or(right));

    /// <inheritdoc />
    public ISearchBit Xor(ISearchBit other)
        => Apply(other, (left, right) => left.Xor(right));

    /// <inheritdoc />
    public long Count()
        => _openBitSet.Cardinality;

    private SearchBit Apply(ISearchBit other, Action<OpenBitSet, OpenBitSet> operation) {
        if (other is not SearchBit otherBitSet) {
            throw new InvalidOperationException("The other bitset must be of type OpenBitSet");
        }

        var bitset = (OpenBitSet)_openBitSet.Clone();

        operation(bitset, otherBitSet._openBitSet);

        return new SearchBit(bitset);
    }
}