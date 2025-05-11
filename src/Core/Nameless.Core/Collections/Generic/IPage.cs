namespace Nameless.Collections.Generic;

/// <summary>
/// Represents a page of items.
/// </summary>
/// <typeparam name="TItem">Type of the item</typeparam>
public interface IPage<out TItem> {
    /// <summary>
    /// Gets the item at the specified index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>
    /// The item at the specified index.
    /// </returns>
    TItem this[int index] { get; }
    /// <summary>
    /// Gets the page number.
    /// </summary>
    int Number { get; }
    /// <summary>
    /// Gets the page size.
    /// </summary>
    int Size { get; }
    /// <summary>
    /// Gets the total number of items considering all pages.
    /// </summary>
    int TotalItems { get; }
    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    int TotalPages { get; }
    /// <summary>
    /// Gets the page items.
    /// </summary>
    TItem[] Items { get; }
    /// <summary>
    /// Gets the total number of items for this page.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Whether it has previous page or not.
    /// </summary>
    bool HasPrevious { get; }
    /// <summary>
    /// Whether it has next page or not.
    /// </summary>
    bool HasNext { get; }
    /// <summary>
    /// Gets an enumerator for this page.
    /// </summary>
    IEnumerator<TItem> GetEnumerator();
}