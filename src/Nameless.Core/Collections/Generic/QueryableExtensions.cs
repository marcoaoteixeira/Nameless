namespace Nameless.Collections.Generic;

/// <summary>
///     Extension methods for <see cref="IQueryable{T}" />.
/// </summary>
public static class QueryableExtensions {
    /// <param name="self">
    ///     The current <see cref="IQueryable{T}"/>.
    /// </param>
    /// <typeparam name="TItem">
    ///     Type of the queryable items.
    /// </typeparam>
    extension<TItem>(IQueryable<TItem> self) {
        /// <summary>
        ///     Creates a paginated view of the items.
        /// </summary>
        /// <remarks>
        ///     Use this method to implement pagination when working with
        ///     large collections, allowing data to be retrieved and displayed
        ///     in smaller, more manageable segments.
        /// </remarks>
        /// <param name="start">
        ///     The zero-based index of the first item to include in the page.
        ///     Must be greater than or equal to zero.
        /// </param>
        /// <param name="limit">
        ///     The maximum number of items to include in the page. Must be
        ///     greater than zero.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IPage{TItem}"/> that represents the
        ///     requested page of items.
        /// </returns>
        public IPage<TItem> ToPage(int start = 0, int limit = 10) {
            return new Page<TItem>(
                items: self.Skip(start).Take(limit),
                start: start,
                limit: limit,
                totalCount: self.Count()
            );
        }
    }
}