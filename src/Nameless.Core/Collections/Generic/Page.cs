namespace Nameless.Collections.Generic;

/// <summary>
///     Default implementation of <see cref="IPage{TItem}" />.
/// </summary>
/// <typeparam name="TItem">Type of the page's items.</typeparam>
public sealed class Page<TItem> : IPage<TItem> {
    public const int DEFAULT_NUMBER = 1;
    public const int DEFAULT_SIZE = 10;

    /// <summary>
    ///     Initializes a new instance of <see cref="Page{TItem}" />.
    /// </summary>
    /// <param name="query">The items of this page.</param>
    /// <param name="number">
    ///     The page number. If not provided, the default value is <see cref="DEFAULT_NUMBER" />.
    /// </param>
    /// <param name="size">
    ///     The page size. If not provided, the default value is <see cref="DEFAULT_SIZE" />.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="query" /> is <see langword="null"/>.
    /// </exception>
    /// ///
    /// <remarks>
    ///     If the value provided for <paramref name="size" /> is lower than <c>1</c>,
    ///     then the value will be set to <see cref="DEFAULT_SIZE" />.
    /// </remarks>
    public Page(IQueryable<TItem> query, int number = DEFAULT_NUMBER, int size = DEFAULT_SIZE) {
        Prevent.Argument.Null(query);

        Number = number >= 1 ? number : DEFAULT_NUMBER;
        Size = size >= 1 ? size : DEFAULT_SIZE;
        TotalItems = query.Count();
        TotalPages = (int)Math.Ceiling((double)TotalItems / Size);
        Items = query.Skip((Number - 1) * Size).Take(Size).ToArray();
    }

    /// <inheritdoc />
    public TItem this[int index] => Items[index];

    /// <inheritdoc />
    public int Number { get; }

    /// <inheritdoc />
    public int Size { get; }

    /// <inheritdoc />
    public int TotalItems { get; }

    /// <inheritdoc />
    public int TotalPages { get; }

    /// <inheritdoc />
    public TItem[] Items { get; }

    /// <inheritdoc />
    public int Count => Items.Length;

    /// <inheritdoc />
    public bool HasPrevious => Number > 1;

    /// <inheritdoc />
    public bool HasNext => Number < TotalPages;

    /// <inheritdoc />
    public IEnumerator<TItem> GetEnumerator() {
        return ((IEnumerable<TItem>)Items).GetEnumerator();
    }
}