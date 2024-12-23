namespace Nameless.Collections.Generic;

/// <summary>
/// Paginator contract.
/// </summary>
/// <typeparam name="TItem">Type of the page's items.</typeparam>
public interface IPaginator<TItem> : IEnumerable<Page<TItem>> {
    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    int PageSize { get; }
}