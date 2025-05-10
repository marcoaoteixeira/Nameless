namespace Nameless.Collections.Generic;

/// <summary>
/// Extension methods for <see cref="IQueryable{T}"/>.
/// </summary>
public static class QueryableExtensions {
    /// <summary>
    /// Converts the <see cref="IQueryable{T}"/> to a page.
    /// </summary>
    /// <typeparam name="TItem">Type of the page item.</typeparam>
    /// <param name="self">The current <see cref="IQueryable{T}"/></param>
    /// <param name="number">The page number.</param>
    /// <param name="size">The page size.</param>
    /// <returns>An <see cref="IPage{TItem}"/> representing the desired page for the query.</returns>
    public static IPage<TItem> ToPage<TItem>(this IQueryable<TItem> self, int number, int size)
        => new Page<TItem>(self, number, size);

    /// <summary>
    /// Converts the <see cref="IQueryable{T}"/> to a collection of <see cref="IPage{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">Type of the item.</typeparam>
    /// <param name="self">The current query instance.</param>
    /// <param name="size">The page size.</param>
    /// <param name="initialNumber">
    /// The initial page number. If the value was not provided or is lower than <c>1</c>, it will be set to <c>1</c>.
    /// </param>
    /// <returns>
    /// A collection of <see cref="IPage{TItem}"/> representing the pages for the query.
    /// </returns>
    public static IEnumerable<IPage<TItem>> Paginate<TItem>(this IQueryable<TItem> self, int size, int initialNumber = 1) {
        initialNumber = initialNumber >= 1 ? initialNumber : 1;

        var totalItems = self.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / size);
        for (var pageNumber = initialNumber; pageNumber < totalPages + 1; pageNumber++) {
            yield return new Page<TItem>(self, pageNumber, size);
        }
    }
}
