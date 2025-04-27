namespace Nameless.Collections.Generic;

/// <summary>
/// Extension methods for <see cref="IQueryable{T}"/>.
/// </summary>
public static class QueryableExtension {
    /// <summary>
    /// Converts the <see cref="IQueryable{T}"/> to a page.
    /// </summary>
    /// <typeparam name="TItem">Type of the page item.</typeparam>
    /// <param name="self">The current <see cref="IQueryable{T}"/></param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>An <see cref="IPage{TItem}"/> representing the desired page for the query.</returns>
    /// <remarks>
    /// The page number starts from 1. If the page number is less than 1, it will be set to <see cref="Page{TItem}.DEFAULT_NUMBER"/>.
    /// The page size must be greater than 0. If the page size is less than 1, it will be set to <see cref="Page{TItem}.DEFAULT_SIZE"/>.
    /// </remarks>
    public static IPage<TItem> ToPage<TItem>(this IQueryable<TItem> self, int pageNumber, int pageSize)
        => new Page<TItem>(query: self, number: pageNumber, size: pageSize);
}
