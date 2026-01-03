using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Nameless.Lucene.Internals;

/// <summary>
///     <see cref="Document" /> extension methods.
/// </summary>
internal static class DocumentExtensions {
    /// <param name="self">
    ///     The current <see cref="Document" />.
    /// </param>
    extension(Document self)
    {
        /// <summary>
        ///     Converts <see cref="Document" /> to <see cref="IEnumerable{T}" />
        ///     of <see cref="IIndexableField"/>.
        /// </summary>
        /// <returns>
        ///     A collection of <see cref="IIndexableField" />.
        /// </returns>
        internal IEnumerable<IIndexableField> ToIndexableFields() {
            return self.Select(item => item.ToIndexableField());
        }

        /// <summary>
        ///     Creates a <see cref="BooleanClause" /> to delete the current
        ///     document from the index.
        /// </summary>
        /// <returns>
        ///     The <see cref="BooleanClause" /> to delete the current
        ///     document from the index.
        /// </returns>
        internal BooleanClause CreateDeleteBooleanClause() {
            var term = new Term(nameof(Document.ID), self.ID);
            var termQuery = new TermQuery(term);

            return new BooleanClause(termQuery, Occur.SHOULD);
        }
    }
}