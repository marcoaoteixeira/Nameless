using System;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene {

    /// <summary>
    /// Default implementation of <see cref="ISearchBit"/>.
    /// </summary>
    public sealed class SearchBit : ISearchBit {

        #region Private Read-Only Fields

        private readonly OpenBitSet _openBitSet;

        #endregion Private Read-Only Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="SearchBit"/>.
        /// </summary>
        /// <param name="openBitSet">The open bit set.</param>
        public SearchBit(OpenBitSet openBitSet) {
            Prevent.ParameterNull(openBitSet, nameof(openBitSet));

            _openBitSet = openBitSet;
        }

        #endregion Public Constructors

        #region Private Methods

        private ISearchBit Apply(ISearchBit other, Action<OpenBitSet, OpenBitSet> operation) {
            var bitset = (OpenBitSet)_openBitSet.Clone();
            var otherBitSet = other as SearchBit;

            if (otherBitSet == null) {
                throw new InvalidOperationException("The other bitset must be of type OpenBitSet");
            }

            operation(bitset, otherBitSet._openBitSet);

            return new SearchBit(bitset);
        }

        #endregion Private Methods

        #region ISearchBits Members

        /// <inheritdoc />
        public ISearchBit And(ISearchBit other) {
            return Apply(other, (left, right) => left.And(right));
        }

        /// <inheritdoc />
        public ISearchBit Or(ISearchBit other) {
            return Apply(other, (left, right) => left.Or(right));
        }

        /// <inheritdoc />
        public ISearchBit Xor(ISearchBit other) {
            return Apply(other, (left, right) => left.Xor(right));
        }

        /// <inheritdoc />
        public long Count() {
            return _openBitSet.Cardinality();
        }

        #endregion ISearchBits Members
    }
}