namespace Nameless.Collections.Generic;

/// <summary>
///     Represents a page of items.
/// </summary>
/// <typeparam name="TItem">
///     Type of the item
/// </typeparam>
public interface IPage<out TItem> : IEnumerable<TItem> {
    /// <summary>
    ///     Gets the item at the specified index.
    /// </summary>
    /// <param name="index">
    ///     The index.
    /// </param>
    /// <returns>
    ///     The item at the specified index.
    /// </returns>
    TItem this[int index] { get; }

    /// <summary>
    ///     Gets the zero-based index at which the sequence starts.
    /// </summary>
    int Start { get; }

    /// <summary>
    ///     Gets the maximum number of items that can be included in
    ///     a single page.
    /// </summary>
    int Limit { get; }

    /// <summary>
    ///     Gets the current page (1-based) number.
    /// </summary>
    int Number { get; }

    /// <summary>
    ///     Gets the total number of pages.
    /// </summary>
    int PageCount { get; }

    /// <summary>
    ///     Gets the total number of items represented by all pages.
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    ///     Whether it has previous page or not.
    /// </summary>
    bool HasPrevious { get; }

    /// <summary>
    ///     Whether it has next page or not.
    /// </summary>
    bool HasNext { get; }
}