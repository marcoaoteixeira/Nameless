namespace Nameless.Collections.Generic;

/// <summary>
///     Extension methods for <see cref="IQueryable{T}" />.
/// </summary>
public static class QueryableExtensions {
    /// <summary>
    ///     Creates a page from the current <see cref="IQueryable{T}" />.
    /// </summary>
    /// <typeparam name="TItem">
    ///     Type of the page item.
    /// </typeparam>
    /// <param name="self">
    ///     The current <see cref="IQueryable{T}" />
    /// </param>
    /// <param name="number">
    ///     The page number.
    /// </param>
    /// <param name="size">
    ///     The page size.
    /// </param>
    /// <returns>
    ///     An <see cref="IPage{TItem}" /> representing the desired
    ///     page from the query.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     If the current <paramref name="self"/> is <see langword="null"/>.
    /// </exception>
    public static IPage<TItem> CreatePage<TItem>(this IQueryable<TItem> self, int number = 1, int size = 10) {
        return new Page<TItem>(self, number, size);
    }
}