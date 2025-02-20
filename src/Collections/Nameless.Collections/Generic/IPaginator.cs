namespace Nameless.Collections.Generic;

/// <summary>
/// Paginator contract.
/// </summary>
/// <typeparam name="TItem">Type of the page's items.</typeparam>
public interface IPaginator<TItem> {
    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Gets the pages.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IPage{TItem}"/>.</returns>
    IEnumerable<IPage<TItem>> GetPages();
}