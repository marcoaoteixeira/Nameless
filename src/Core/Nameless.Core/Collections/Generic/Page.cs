using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
/// Represents a page of enumerable items.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Page<T> : IPage<T> {
    private T[] Items { get; }

    /// <inheritdoc />
    public int Index { get; }

    /// <inheritdoc />
    public int Number => Index + 1;

    /// <inheritdoc />
    public int Size { get; }

    /// <inheritdoc />
    public int Length => Items.Length;

    /// <inheritdoc />
    public int PageCount => (int)Math.Ceiling(Total / (decimal)Size);

    /// <inheritdoc />
    public int Total { get; }

    /// <inheritdoc />
    public bool HasNext => Number < PageCount;

    /// <inheritdoc />
    public bool HasPrevious => Number > 1;

    /// <summary>
    /// Initializes a new instance of <see cref="Page{T}"/>.
    /// </summary>
    /// <param name="items">The <see cref="IQueryable{T}"/> that will provide the items to this page.</param>
    /// <param name="index">The page index. Default is 0 (zero).</param>
    /// <param name="size">The page desired size. Default is 10.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="items"/> is <c>null</c>.
    /// </exception>
    public Page(IQueryable<T> items, int index = 0, int size = 10) {
        Prevent.Argument.Null(items, nameof(items));

        index = index >= 0 ? index : 0;
        size = size > 0 ? size : 10;

        Index = index;
        Size = size;
        Total = items.Count();
        Items = [
            .. items.Skip(index * size)
                    .Take(size)
        ];
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => (IEnumerator<T>)Items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => Items.GetEnumerator();
}