using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
///     Default implementation of <see cref="IPage{TItem}" />.
/// </summary>
/// <typeparam name="TItem">Type of the page's items.</typeparam>
public class Page<TItem> : IPage<TItem> {
    private readonly TItem[] _items;

    /// <inheritdoc />
    public TItem this[int index] => _items[index];

    /// <inheritdoc />
    public int Start { get; }

    /// <inheritdoc />
    public int Limit { get; }

    /// <inheritdoc />
    public int Number => (Start / Limit) + 1;

    /// <inheritdoc />
    public int PageCount => (int)Math.Ceiling((double)TotalCount / Limit);

    /// <inheritdoc />
    public int TotalCount { get; }

    /// <inheritdoc />
    public bool HasPrevious => Number > 1;

    /// <inheritdoc />
    public bool HasNext => Number < PageCount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Page{TItem}"/> class.
    /// </summary>
    /// <param name="items">
    ///     An array of items of type <typeparamref name="TItem"/> that make
    ///     up the contents of the current page.
    /// </param>
    /// <param name="start">
    ///     The zero-based index of the first item in the page. Must be
    ///     greater than or equal to zero.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of items to include in the page. Must be
    ///     greater than zero.
    /// </param>
    /// <param name="totalCount">
    ///     The total number of items available in the entire collection,
    ///     which may be greater or equal to the number of items in this
    ///     page.
    /// </param>
    /// <remarks>
    ///     It's important to notice that the <paramref name="items"/> should
    ///     be a set of any larger collection of items.
    /// </remarks>
    public Page(IEnumerable<TItem> items, int start, int limit, int totalCount) {
        _items = [.. items];

        Throws.When.LowerThan(start, compare: 0);
        Throws.When.LowerThan(limit, compare: 1);
        Throws.When.LowerThan(
            totalCount,
            compare: _items.Length,
            message: $"Parameter '{nameof(totalCount)}' cannot be lower than the total number of items."
        );

        TotalCount = totalCount;
        Start = start >= totalCount ? totalCount - 1 : start;
        Limit = limit;
    }

    /// <inheritdoc />
    public IEnumerator<TItem> GetEnumerator() {
        return _items.AsEnumerable().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
