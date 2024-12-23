namespace Nameless.Collections.Generic;

public static class QueryableExtension {
    /// <summary>
    /// Returns the current <see cref="IQueryable{T}"/> as a <see cref="IPaginator{TItem}"/> object.
    /// </summary>
    /// <typeparam name="TItem">Type of the query items.</typeparam>
    /// <param name="self">The current <see cref="IQueryable{T}"/>.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>
    /// A <see cref="IPaginator{TItem}"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// If the value provided to <paramref name="pageSize"/> is lower than <c>1</c>,
    /// then the value will be set to <see cref="Paginator{TItem}.DEFAULT_PAGE_SIZE"/>.
    /// </remarks>
    public static IPaginator<TItem> AsPaginator<TItem>(this IQueryable<TItem> self, int pageSize)
        => new Paginator<TItem>(query: Prevent.Argument.Null(self),
                                pageSize: pageSize);
}
