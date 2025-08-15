namespace Nameless.Collections.Generic;

/// <summary>
///     Default implementation of <see cref="IPage{TItem}" />.
/// </summary>
/// <typeparam name="TItem">Type of the page's items.</typeparam>
public sealed class Page<TItem> : IPage<TItem> {
    private readonly IQueryable<TItem> _query;
    private readonly Lazy<TItem[]> _items;
    private readonly Lazy<int> _totalItems;
    private readonly Lazy<int> _totalPages;

    /// <inheritdoc />
    public TItem this[int index] => Items[index];

    /// <inheritdoc />
    public int Number { get; }

    /// <inheritdoc />
    public int Size { get; }

    /// <inheritdoc />
    public int TotalItems => _totalItems.Value;

    /// <inheritdoc />
    public int TotalPages => _totalPages.Value;

    /// <inheritdoc />
    public TItem[] Items => _items.Value;

    /// <inheritdoc />
    public int Count => Items.Length;

    /// <inheritdoc />
    public bool HasPrevious => Number > 1;

    /// <inheritdoc />
    public bool HasNext => Number < TotalPages;

    /// <summary>
    ///     Initializes a new instance of <see cref="Page{TItem}" /> class.
    /// </summary>
    /// <param name="query">
    ///     A <see cref="IQueryable{T}"/> to get the page items.
    /// </param>
    /// <param name="number">
    ///     The page number. If not provided, the default value is <c>1</c>.
    /// </param>
    /// <param name="size">
    ///     The page size. If not provided, the default value is <c>10</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="query" /> is <see langword="null"/>.
    /// </exception>
    public Page(IQueryable<TItem> query, int number = 1, int size = 10) {
        Number = number >= 1 ? number : 1;
        Size = size >= 1 ? size : 10;

        _query = Guard.Against.Null(query);
        _items = new Lazy<TItem[]>(GetItems);
        _totalItems = new Lazy<int>(GetTotalItems);
        _totalPages = new Lazy<int>(GetTotalPages);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     🦆
    /// </remarks>
    public IEnumerator<TItem> GetEnumerator() {
        return ((IEnumerable<TItem>)Items).GetEnumerator();
    }

    private TItem[] GetItems() {
        return _query.Skip((Number - 1) * Size).Take(Size).ToArray();
    }

    private int GetTotalItems() {
        return _query.Count();
    }

    private int GetTotalPages() {
        return (int)Math.Ceiling((double)TotalItems / Size);
    }
}