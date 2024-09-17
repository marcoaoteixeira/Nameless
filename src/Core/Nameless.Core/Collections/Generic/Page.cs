using System.Collections;

namespace Nameless.Collections.Generic;

/// <summary>
/// Represents a page of items.
/// </summary>
/// <typeparam name="T">Type of the page items.</typeparam>
public sealed class Page<T> : IEnumerable<T> {
    private readonly T[] _items;

    /// <summary>
    /// Gets the page number
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Gets the page defined (expected) size.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets the page total number of items.
    /// </summary>
    public int Count => _items.Length;

    /// <summary>
    /// Initializes a new instance of <see cref="Page{T}"/>.
    /// </summary>
    /// <param name="items">The items of this page.</param>
    /// <param name="number">The page number.</param>
    /// <param name="size">The page size.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="items"/> is <c>null</c>.
    /// </exception>
    public Page(T[] items, int number, int size) {
        _items = Prevent.Argument.Null(items);

        Number = Prevent.Argument.LowerOrEqual(number, to: 0);
        Size = Prevent.Argument.LowerOrEqual(size, to: 0);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => (IEnumerator<T>)_items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => _items.GetEnumerator();
}